
# Enemy Skin Kit

This mod provides tools for creating vanilla enemy skins for lethal company from within the unity editor. Most end users will see this as a dependency for other skin mods.


## Prerequisites

- Lethal Company
- Installation of Unity 2022.3.9f1
- Enemy Skin Kit and its dependencies
- Quite a bit of patience so that unity may chug as it is want to do

It's recommended that you also use Evaisa's starter project which can be found here:
https://github.com/EvaisaDev/LethalCompanyUnityTemplate

Use of Thunderkit is also an optional recommendation:
https://github.com/PassivePicasso/ThunderKit

## Setup with Thunderkit

1. Create up a new 3D (HDRP) project in Unity (or get a project template like this one https://github.com/EvaisaDev/LethalCompanyUnityTemplate).

2. Now, we're going to get ThunderKit. ThunderKit is a package manager for making mods in the unity editor. Go to Window > Package Manager to open up the package manager window.

3. Click the plus dropdown in the upper left hand corner, and from that menu select "Add package from git url."

4. Paste this url in the input field and click the "add" button.
```
https://github.com/PassivePicasso/ThunderKit.git
```

5. Once the the ThunderKit package is installed, a new option will be added to the menu bar. Navigate to Tools > Thunderkit > Settings to open the Thunderkit settings menu.

6. We're going to use this menu to make a package out of Lethal Company's code. Go to "ThunderKit Settings" and in the "Locate and Load game files for project" section, click browse and navigate to your Lethal Company.exe (probably in your steamapps folder).

7. Click "import" and let Thunderkit do its thing. It'll ask you to restart your project a couple of times. After it's done, you'll have the necessary game code that this and other mods depend on.

ThunderKit might not import the Assembly-CSharp.dll from Lethal Company. If this is the case, navigate to the same folder you found your Lethal Company.exe in, go to "Lethal Company_Data"/Managed and copy (DO NOT MOVE) Assembly-CSharp.dll to <YOUR_UNITY_PROJECT>/Packages/LethalCompany.

Also 0Harmony2.dll and BepinEx.Harmony.dll (both of them are in <YOUR_UNITY_PROJECT>/Packages/BepInExPack/BepInExPack/BepInEx/core) might give you compiler errors. For use with this project, you can delete them.

8. At this point we're nearly done with the setup. Now we're gonna get this mod's code, along with its dependencies. Go back to the Thunderstore Settings menu and navigate to "Package Source Settings." Click "Add" and from the dropdown select "Thunderstore Source." This will add a new entry to the package source list called "ThunderStoreSource."

9. Select the new entry and name it to whatever pleases you in the "Package Source" field. I just call it "Lethal Company Mods." In the url field, paste this url:
```
https://thunderstore.io/c/lethal-company
```

10. Give unity a minute to connect to ThunderStore. Navigate to Tools > ThunderKit > Packages. If the package source you named has a silly number next to it, you've successfully connected to Thunderstore and can now download mods. In the search bar in the top right, type in "EnemySkinKit" and look for this mod. Once you've found it, click "install" in the top right.

11. Verify that the mod has been installed by right clicking anywhere in the project window (the little file explorer at the bottom of unity's default layout) and go to Create. If you find options for "EnemySkinKit" it's installed and you should be good to go.

## Codeless Method

1. Right click anywhere in the assets window and select Create > Enemy Skin Kit > Skins > Enemy-You-Want-To-Skin
2. Name your skin file, then set the label, guid, and icon fields in the inspector 
3. In the inspector, fill out the remaining fields for your skin. These fields come in the form of several "Action" types that will let you replace, hide, or keep sound effects, materials, and meshes.

#### Material Action - Operates on a material
- ActionType - determines what the action does when the skin is applied
  - Retain - Keeps the material as is
  - Replace - Replaces the material with "ReplacementMaterial"
- ReplacementMaterial - the material that will replace the vanilla material when ActionType is set to Replace

#### Static Mesh Action - Operates on one of the enemy's static meshes (like the spider's fangs or the jester's skull)
- ActionType - determines what the action does when the skin is applied
  - Retain - Leaves the mesh as is.
  - Hide - Disables the mesh's renderer, effectively hiding it.
  - Replace - Replaces the static mesh with "ReplacementMesh"
- ReplacementMesh - the static mesh that will replace the vanilla mesh when ActionType is set to Replace

#### Skinned Mesh Action - Operates on one of the enemy's skinned meshes (normally the one "body" mesh)
- ActionType - determines what the action does when the skin is applied
  - Retain - Leaves the mesh as is.
  - Hide - Disables the mesh's renderer, effectively hiding it.
  - Replace - Hides the vanilla skeletal mesh and puts a prefab in place to copy its animation.
- ArmatureMap - A data structure mapping the bones from your modded mesh to the vanilla mesh.*
- ReplacementObject - the prefab that will be instantiated, placed into the enemy's hierarchy, and made to copy its animations when ActionType is set to Replace. This prefab must contain a skeletal mesh component in its hierarchy, along with the skeleton. Really it should just be whatever prefab importing your .fbx or .dae or whatever spits out.

#### Audio Action - Operates on one of the enemy's sounds
- ActionType - determines what the action does when the skin is applied
  - Retain - Leaves the sound as is.
  - Mute - Replaces the sound with nothing.
  - Replace - Replaces the sound with ReplacementClip
- ReplacementClip - the clip that will play instead of the vanilla audio clip when ActionType is set to Replace

#### Audio List Action - Operates on a set of sounds the enemy picks from
- ActionType - determines what the action does when the skin is applied
  - Retain - Leaves the sound list as is.
  - Mute - Replaces the vanilla list of audio clips with an empty list
  - Replace - Replaces the list of clips with ReplacementClips
- ReplacementClips - the list of audio clips that the enemy will pick from when ActionType is set to Replace

4. Right click anywhere in the assets window and select Create > Enemy Skin Kit > Mod

5. Click the "Create Mod" button.

6. Locate your mod zip in the unity project folder (one level above assets) 

7. Install your mod in whatever way that pleases you and verify that it works as intended.
You can pretty easily test you mod with the ThunderStore mod manager by using its "Import Local Mod" feature.

## Additional Logic Method

If you want to take advantage of the ease of using the scriptable objects, but want to use them in a larger mod, then all you have to do is load your skin scriptable object from an asset bundle and register it with the EnemySkinRegistry.
```csharp
//whatever you're doing to retrieve your skin scriptable object from your asset bundle
EnemySkinRegistry.RegisterSkin(mySkin);
```
The BaseSkin and BaseSkinner classes implement the necessary interfaces to register with the EnemySkinRegistry.

You're also free to add child classes to the existing Skin and Skinner types to extend them with whatever is to your liking. Permissions for these classes are pretty lax. If a member isn't public in one of these classes, it's protected. Don't you do nothing crazy though.

If you'd prefer something leaner or want something more custom, consider looking at the developer section of the EnemySkinRegistry mod page.

### *So About Those Armature Maps...
Lethal Company skinned meshes have proven difficult to work with, at least for me. Reskinning a mesh in the usual way is probably possible, but it seems no matter what I try, when I go to replace a skinned mesh in a skinned mesh component, it ends up a scrambled mess. Get in touch with me if you've got this figured out. Until then, my solution is "Armature Maps," which copy the animation from the vanilla mesh over to your modded mesh at runtime. These were a planned feature anyway, because they have the benefit of allowing you to adjust the proportions of your model. They're just more expensive computationally.
To get an armature map set up, right click in the Project Window, and select Create > EnemySkinKit > Animation > Armature Mapping. This Scriptable Object will first display two fields: a "source" hierarchy and a "destination" hierarchy. "Source" is the hierarchy of the vanilla enemy armature. I've included a prefab for each one of these in the mods package. You should be able to find the prefab you need in <YOUR_UNITY_PROJECT>/Packages/EnemySkinKit/EnemyRigs/<ENEMY_YOU_WANT_TO_SKIN>. For destination, drag in the part of your prefab object that has the skinned mesh component. Once both of those fields have been added, the ArmatureMap in the inspector should expand and show the mapping of source bones to destination bones. If a source bone name matches a destination bone name, the ArmatureMap will automatically map them. At runtime, a source bone will have its animation copied by whatever destination bone has been mapped to it. Once your ArmatureMap is set up in this way, you can add it to the SkinnedMeshAction.

That's about as hairy as it gets. I'll have blender projects that you can reskin and export from, so that this step becomes easy-peasy drag-and-drop in simple cases. But if you're reading this now, I'm still getting this mod set up and I don't have a source to point you at just yet.

### ToDos

- Testing to ensure these skins are client-side only.
- Currently, not all sound effects can be replaced. This is because some are stored in scriptable objects, but these skins work at an instance level. So full sound replacement will have to wait on the EnemySkinRegistry finishing all of the enemy dispatchers. Until then, most sounds can at least be muted via the "mute creature effects" and "mute creature voice" toggles on each enemy skin object. 
