using AntlerShed.EnemySkinKit;
using AntlerShed.EnemySkinKit.Vanilla;
using AntlerShed.SkinRegistry;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace AntlerShed.EnemySkinKit
{
    [CreateAssetMenu(fileName = "SkinMod", menuName = "EnemySkinKit/Mod", order = 1)]
    public class SkinModBuilder : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The name of the mod")]
        private string modName;

        [SerializeField]
        [Tooltip("The name of the mod's author")]
        private string author;

        [SerializeField]
        [Tooltip("Optional place to specify an explicit mod GUID. If not speficied, the GUID will be \"<ModName>.<ModAuthor>\"")]
        private string modGUID;

        [SerializeField]
        [Tooltip("ReadMe file. This will be the text content of your mod's ThunderStore page. Talk about your mod in detail here.")]
        private TextAsset readMe;

        [SerializeField]
        [Tooltip("The icon of your mod that will appear on your mod's Thunderstore page and in the Thunderstore mod manager.")]
        private Texture2D icon;

        [SerializeField]
        [Tooltip("The major version of your mod. Start at one and add one every time you make a change that will break something if someone else depends on your mod (probably never, so this should be 1 in most cases).")]
        private uint major;

        [SerializeField]
        [Tooltip("The minor version of your mod. Start at zero and add one every time you add a new feature, like a new skin. Go back to zero if you update the major version.")]
        private uint minor;

        [SerializeField]
        [Tooltip("The patch part of your mod version. Start at zero and add one every time you make a change that fixes a problem with your mod. Reset to zero when updating the minor or major version.")]
        private uint patch;

        [SerializeField]
        [Tooltip("A short description of your mod that just tells people what it is. Shouldn't be more than a sentence or two.")]
        private string description;

        [SerializeField]
        [Tooltip("The skins that will be packaged with your mod.")]
        private BaseSkin[] skins;

        [SerializeField]
        [Tooltip("Any default configurations to include with your mod")]
        private DefaultSkinConfigurationView[] configs;

        private const string ASSET_BUNDLE_NAME = "SkinAssets";
        private const string PLUGINS_FOLDER = "plugins";
        private const string ASSETS_FOLDER = "AssetBundles";
        private const string STAGING_FOLDER_SUFFIX = "Staging";
        private const string BUILD_FOLDER = "EnemySkinKit";

        private static readonly Regex whitespace = new Regex(@"\s"); //dumb whitespace matcher. There's probably something built in for this, but whatever.

        private static readonly Regex wordDelim = new Regex(@"[\s_-][\w]"); //for attempting to put spaced words in the correct case before removing the spaces

        private static readonly Regex cSharpInvalid = new Regex(@"[^\w\d]"); //filters invalid characters

        private static readonly Regex fileNameInvalid = new Regex(@"[^\w\d_-]"); //filters invalid characters

        private static readonly Regex leadingDigits = new Regex(@"^\d+");

        private static readonly Regex validClassName = new Regex(@"\w+[\w\d]*"); //won't cause compiler to vomit

        private string sanitizedAssemblyName;

        private string sanitizedModName;

        private string stagingPath;

        private string pluginPath;

        private string assetsPath;

        private string sourceCodeFilePath;

        private string assemblyFilePath;

        public void BuildMod
        (
            string author, 
            string modName, 
            string modGUID,
            string description,
            TextAsset readMe,
            Texture2D icon,
            uint major, 
            uint minor, 
            uint patch,
            BaseSkin[] skins
        )
        {
            if (!Directory.Exists(BUILD_FOLDER))
            {
                Directory.CreateDirectory(BUILD_FOLDER);
            }
            
            if(icon == null || !AssetDatabase.GetAssetPath(icon).EndsWith(".png") || icon.width != 256 || icon.height != 256)
            {
                throw new InvalidIconException();
            }

            //sanitize incoming strings
            sanitizedModName = modName.Replace(" ", "_");
            sanitizedModName = fileNameInvalid.Replace(sanitizedModName, "");
            string sanitizedAuthor = author.Replace(" ", "_");
            sanitizedAuthor = fileNameInvalid.Replace(sanitizedAuthor, "");
            string sanitizedGUID = string.IsNullOrEmpty(modGUID) ? $"{sanitizedAuthor}.{sanitizedModName}" : whitespace.Replace(modGUID, "");
            string sanitizedPluginName = convertToValidCSharpName(sanitizedModName + "Plugin");
            string sanitizedNamespace = $"{convertToValidCSharpName(sanitizedAuthor) ?? ""}.LethalCompany.{convertToValidCSharpName(sanitizedModName) ?? ""}";
            sanitizedAssemblyName = convertToValidCSharpName(sanitizedModName);

            //check and see if anything remains after cleaning the input
            if (string.IsNullOrEmpty(sanitizedAuthor))
            {
                throw new InvalidAuthorException();
            }
            if (string.IsNullOrEmpty(sanitizedGUID))
            {
                throw new InvalidModGUIDException();
            }
            if(string.IsNullOrEmpty(sanitizedPluginName))
            {
                throw new InvalidPluginNameException();
            }
            if(sanitizedNamespace.Split(".", StringSplitOptions.None).Any((section) => string.IsNullOrEmpty(section)))
            {
                throw new InvalidNamespaceException();
            }
            if(string.IsNullOrEmpty(sanitizedAssemblyName))
            {
                throw new InvalidAssemblyNameException();
            }
            if (string.IsNullOrEmpty(sanitizedModName))
            {
                throw new InvalidModNameException();
            }

            Debug.Log(createSkinModSource(sanitizedGUID, sanitizedModName, "BundleName", major, minor, patch, sanitizedPluginName, sanitizedNamespace, skins, configs));
            stagingPath = Path.Combine( "Assets", $"{sanitizedModName}{STAGING_FOLDER_SUFFIX}");
            pluginPath = Path.Combine(new string[] { stagingPath, PLUGINS_FOLDER, sanitizedModName });
            assetsPath = Path.Combine( pluginPath, ASSETS_FOLDER );

            if(Directory.Exists(stagingPath))
            {
                Directory.Delete(stagingPath, true);
            }
            Directory.CreateDirectory(assetsPath);

            string assetBundleName = $"{sanitizedAuthor}_{sanitizedModName}_{ASSET_BUNDLE_NAME}";

            //Asset bundle creation
            AssetBundleBuild bundle = new AssetBundleBuild();
            bundle.assetBundleName = assetBundleName;
            bundle.assetNames = skins.Select((skin) => AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(skin), true))
                .Aggregate
                (
                    new List<string>(), (list, paths) => list.Concat(paths).ToList()
                )
                //need to find a better way of doing this.
                .Where((path) => !(path.EndsWith(".cs") || path.EndsWith(".asm") || path.EndsWith(".dll")))
                .ToHashSet()
                .ToArray();

            BuildAssetBundlesParameters buildInput = new BuildAssetBundlesParameters();
            buildInput.bundleDefinitions = new AssetBundleBuild[] { bundle };
            buildInput.options = BuildAssetBundleOptions.AssetBundleStripUnityVersion;
            buildInput.outputPath = assetsPath;
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildInput);
            


            //Metadata
            using (StreamWriter readMeFile = new StreamWriter(Path.Combine(stagingPath, "README.md"), false))
            {
                readMeFile.WriteLine(readMe == null ? "" : readMe.text);
            }
            using (StreamWriter manifestFile = new StreamWriter(Path.Combine(stagingPath, "manifest.json"), false))
            {
                manifestFile.WriteLine(createThunderstoreManifest(sanitizedModName, sanitizedAuthor, description, major, minor, patch));
            }
            File.Copy(AssetDatabase.GetAssetPath(icon), Path.Combine(stagingPath, "icon.png"));

            //Assembly and source code creation
            sourceCodeFilePath = Path.Combine(pluginPath, sanitizedPluginName + ".cs");
            using (StreamWriter pluginSourceFile = new StreamWriter(sourceCodeFilePath, false))
            {
                pluginSourceFile.WriteLine(createSkinModSource(sanitizedGUID, sanitizedModName, assetBundleName, major, minor, patch, sanitizedPluginName, sanitizedNamespace, skins, configs));
            }
            
            assemblyFilePath = Path.Combine(pluginPath, sanitizedAssemblyName + ".asmdef");
            using (StreamWriter assemblyFile = new StreamWriter(assemblyFilePath, false))
            {
                assemblyFile.WriteLine(createAssemblyDefinitionSource(sanitizedAssemblyName, sanitizedNamespace));
            }

            AssetDatabase.ImportAsset(sourceCodeFilePath);
            AssetDatabase.ImportAsset(assemblyFilePath);
            
            CompilationPipeline.compilationFinished += onCompilationFinished;
            CompilationPipeline.assemblyCompilationFinished += onAssemblyCompilationFinished;
        }

        //Thank you KingEnderbrine for showing me this. I surely would have wished for death if I didn't know it existed.
        private void onAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            //Check if this is the correct assembly
            if(assemblyPath.EndsWith($"{sanitizedAssemblyName}.dll"))
            {
                foreach (string path in Directory.EnumerateFiles(stagingPath, "*.meta", SearchOption.AllDirectories))
                {
                    File.Delete(path);
                }
                File.Delete(sourceCodeFilePath);
                //File.Delete($"{sourceCodeFilePath}.meta");
                File.Delete(assemblyFilePath);
                //File.Delete($"{assemblyFilePath}.meta");
                
                //create mod folder
                File.Copy(assemblyPath, Path.Combine(pluginPath, Path.GetFileName(assemblyPath)));

                string zipPath = Path.Combine(BUILD_FOLDER, sanitizedModName + ".zip");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                //Zip up mod folder
                ZipFile.CreateFromDirectory(stagingPath, zipPath);
                Directory.Delete(stagingPath, true);
                
            }

        }

        private void onCompilationFinished(object eventStore)
        {
            CompilationPipeline.assemblyCompilationFinished -= onAssemblyCompilationFinished;
            CompilationPipeline.compilationFinished -= onCompilationFinished;
        }

        public void BuildMod()
        {
            BuildMod
            (
                author, 
                modName, 
                modGUID,
                description,
                readMe,
                icon,
                major, 
                minor, 
                patch,
                skins
            );
        }

        private string createDefaultSkinConfigSource(DefaultSkinConfigurationView defaultConfig)
        {
            string configEntries = "";
            for(int i = 0; i < defaultConfig.defaultEntries.Length-1; i++)
            {
                configEntries += createDefaultSkinConfigEntrySource(defaultConfig.defaultEntries[i]) + ", ";
            }
            if(defaultConfig.defaultEntries.Length > 0)
            {
                configEntries += createDefaultSkinConfigEntrySource(defaultConfig.defaultEntries[defaultConfig.defaultEntries.Length - 1]);
            }
            
            return $"new DefaultSkinConfigData(new DefaultSkinConfigEntry[] {{ {configEntries} }}, {Math.Clamp( defaultConfig.defaultFrequency, 0.0f, 1.0f)}f, {Math.Clamp( defaultConfig.vanillaFallbackFrequency, 0.0f, 1.0f)}f)";
        }

        private string createDefaultSkinConfigEntrySource(DefaultMoonFrequencyView defaultConfig)
        {

            return $"new DefaultSkinConfigEntry(\"{defaultConfig.moonId}\", {defaultConfig.frequency}f)";
        }

        private string createTemplateSkinEntrySource(BaseSkin skin, DefaultSkinConfigurationView? defaultConfig)
        {
            return $"EnemySkinRegistry.RegisterSkin(bundle.LoadAsset<{skin.GetType().Name}>(\"{AssetDatabase.GetAssetPath(skin)}\"){(!defaultConfig.HasValue ? "" : $", {createDefaultSkinConfigSource(defaultConfig.Value)}")});";
        }

        private string createSkinModSource(string modGUID, string modName, string bundleName, uint major, uint minor, uint patch, string pluginClassName, string namespaceText, BaseSkin[] skins, DefaultSkinConfigurationView[] configs)
        {
            return
                "//This is generated code and shouldn't be modified unless it's taken out of its original environment\n" +
                "using AntlerShed.EnemySkinKit;\n" +
                "using AntlerShed.EnemySkinKit.Vanilla;\n" +
                "using AntlerShed.SkinRegistry;\n" +
                "using BepInEx;\n" +
                "using System.Reflection;\n" +
                "using UnityEngine;\n" +
                $"namespace {namespaceText}\n" +
                "{\n" +
                $"\t[BepInPlugin(\"{modGUID}\", \"{modName}\", \"{major}.{minor}.{patch}\")]\n" +
                $"\t[BepInDependency(\"{EnemySkinKit.modGUID}\")]\n" +
                $"\t[BepInDependency(\"{EnemySkinRegistry.modGUID}\")]\n" +
                $"\tpublic class {pluginClassName} : BaseUnityPlugin\n" +
                "\t{\n" +
                $"\t\tpublic const string modGUID = \"{modGUID}\";\n" +
                $"\t\tpublic const string modName = \"{modName}\";\n" +
                $"\t\tpublic const string modVersion = \"{major}.{minor}.{patch}\";\n" +
                "\t\tvoid Awake()\n" +
                "\t\t{\n" +
                "\t\t\tAssetBundle bundle = AssetBundle.LoadFromFile\n" +
                "\t\t\t(\n" +
                "\t\t\t\tSystem.IO.Path.Combine\n" +
                "\t\t\t\t(\n" +
                "\t\t\t\t\tSystem.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),\n" +
                $"\t\t\t\t\t\"AssetBundles/{bundleName}\"\n" +
                "\t\t\t\t)\n" +
                "\t\t\t);\n" +
                $"{skins.Select((skin) => "\t\t\t" + createTemplateSkinEntrySource(skin, configs.Any((cfg) => cfg.skinId.Equals(skin.Id)) ? configs.First((cfg)=>cfg.skinId.Equals(skin.Id)) : null)).Aggregate("", (str, line) => str + line)}\n" +
                "\t\t}\n" +
                "\t}\n"+
                "}";
        }

        private string createAssemblyDefinitionSource(string assemblyName, string namespaceText)
        {
            return "{\n" +
                $"\"name\": \"{assemblyName}\",\n" +
                $"\t\"rootNamespace\": \"{namespaceText}\",\n" +
                "\t\"references\": [],\n" +
                "\t\"includePlatforms\": [],\n" +
                "\t\"excludePlatforms\": [],\n" +
                "\t\"allowUnsafeCode\": false,\n" +
                "\t\"overrideReferences\": true,\n" +
                "\t\"precompiledReferences\": \n" +
                "\t[\n" +
                "\t\t\"BepInEx.dll\",\n" +
                "\t\t\"EnemySkinRegistry.dll\",\n" +
                "\t\t\"EnemySkinKit.dll\"\n " +
                "\t],\n" +
                "\t\"autoReferenced\": true,\n" +
                "\t\"defineConstraints\": []," +
                "\n\t\"versionDefines\": []," +
                "\n\t\"noEngineReferences\": false" +
                "\n}";
        }

        private string createThunderstoreManifest(string modName, string author, string description, uint major, uint minor, uint patch)
        {
            return "{\n" +
                $"\t\"name\": \"{modName}\",\n" +
                $"\t\"author\": \"{author}\",\n" +
                $"\t\"version_number\": \"{major}.{minor}.{patch}\",\n" +
                "\t\"website_url\": \"\",\n" +
                $"\t\"description\": \"{description}\",\n" +
                "\t\"dependencies\": [\"AntlerShed-EnemySkinKit-1.0.0\"]\n" +
                "}";
        }

        private string convertToValidCSharpName(string name)
        {
            string correctedName = leadingDigits.Replace(cSharpInvalid.Replace(wordDelim.Replace(name, (match) => match.ToString().ToUpper()), ""), "");
            if(!validClassName.IsMatch(correctedName))
            {
                //it ain't a valid name, oops
                return null;
            }
            return correctedName;
        }
    }
}
