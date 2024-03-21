
# Enemy Skin Registry

This mod gives developers a means to register client-side enemy skins to be mutually exclusive with other registered enemy skins. It gives users the ability to control when and where those skins will appear, as well as how frequently.


## Features

#### Completed
- LethalConfig-accessible GUI for end users to set configurations 
- Registration of custom Skin implementations
- Configurable distribution of skins per enemy
- Configurable distribution of skins per moon

#### Implemented but not fully tested
- Custom Enemy Registration
- Custom Moon Registration
- Event Listeners for BaboonHawk and Bracken

#### Planned
- Event Listeners for remaining vanilla enemies

## User Guide

The enemy skin configuration is accessible through the Lethal Config menu.

The top bar will let you scroll through the registered enemies. Use the right and left arrows to navigate through them. The name of the enemy in the middle will have its configuration displayed in the rest of the ui below.

The panel in the lower left may be used to activate and de-activate registered skins. Click the skin's icon on the left side of each entry to toggle whether an enemy can spawn with this skin or not. Deactivation works mid round to allow for buggy skins to be removed if they are causing problems. This is the only time where adjusting the config will change any already-spawned skins.

The panel in the lower right is used to configure the likelihood of a skin being selected when an enemy of a particular type spawns. Each moon can be given its own distribution of skins. The "Default" distribution is for any moon that does not have an explicit distribution. Each skin on each moon can be given a frequency value between 0 and 100 (inclusive). The actual chance that a skin within a distribution is selected to be applied to an enemy is given by its frequency value over the sum total of all frequency values in that distribution (including the vanilla value). If the sum of all frequency values is zero, the vanilla appearance is selected and no skin is applied. Deactivated skins are not considered.

Finally, hit the "Save Config" button to save any changes. Changes are immediately applied to skin selection rates and will affect any enemies that spawn after. Changes will not affect enemies that have already spawned except in the case of deactivating skins.
## For Developers

### Adding a Custom Skin Implementation

This mod has a companion mod with default skin implementations that allows for a codeless approach. However, if you wish to create custom skin or adapt an existing skin to be compatible with this mod, doing so is fairly straightforward. 

1. #### Implement the Skin interface
Implementors of the Skin interface are required to provide a handful of metadata:
    
- Label - The name of the skin as it will show up in the configuration menu
- Id - The unique id of the skin. To help enforce uniqueness, it's recommended that ids follow the pattern "<AuthorName>.<SkinName>"
- EnemyId - The unique id string of the enemy type the Skin will be attached to.
- Icon - A Unity Texture2D to be used as the skin's icon so it can be previewed in the configuration menu

Along with this information, the Skin interface follows the abstract factory pattern to produce Skinners. Each time an enemy spawns, a Skin is picked based on the user configuration and asked to create an instance of Skinner. Each Skin has an enemy type and each Skinner is mapped to an instance of that corresponding type. 
```csharp
class MySkin : Skin
{
    public Skinner CreateSkinner()
    {
        //Return your skinner implementation from here
    }
}
```
2. #### Implement the Skinner interface
The Skinner interface is only required to implement two methods:
```csharp
class MySkinner : Skinner
{
    void Apply(GameObject enemy)
    {
        //Perform any logic here to modify the appearance of the enemy. All of it must be client-side.
        //This is also the point where an EventHandler is registered if your skinner makes use of it. To do so, call EnemySkinRegistry.RegisterEventHandler(enemy, MyEventHandler)
    }

    void Remove(GameObject enemy)
    {
        //Restore the enemy to its vanilla appearance, undoing all of the changes done by Apply.
        //Unregister the event handler by calling RemoveEventHandler(enemy) if you registered one.
    }
}
```

For vanilla enemies, the game object passed to these methods is the one containing the EnemyAI component. Applying a skin to a modded enemy will require the author of the enemy mod to add calls to the registry from their logic, or third-party patching. This is covered elswhere in this document.

3. #### Call RegisterSkin

Once you have implemented both of these classes, call the following from the Awake method in your plugin:
```csharp
EnemySkinRegistry.RegisterSkin(new MySkin());
```
Note that this does nothing for creating and reading from asset bundles. Resolution of dependencies on textures, models, animations, sounds, and other assets that a Skin or Skinner might have is the responsibility of the developer.

### Event Handlers

Enemy Event handlers are an in-progress feature to allow developers to fully overhaul an enemy's appearance without relying on asset replacement. Enemies will notify a handler class that a certain event has occured, e.g. a bracken snaps a player's neck or a blind dog lunges, and that handler will run some logic when that happens.

To receive events from an enemy, implement its corresponding EventHandler class and then call 
```csharp
EnemySkinRegistry.RegisterEnemyEventHandler(EnemyAI enemyInstanceAIComponent, EnemyEventHandler myEventHandler);
```

in your Skinner's Apply method, then call
```csharp
EnemySkinRegistry.RemoveEnemyEventHandler(EnemyAI enemyInstanceAIComponent);
```

in your Skinner's Remove method.

Multiple handlers per instance may be registered at a time.

### Registering a Modded Enemy

The only thing that needs to be done to add a modded enemy to the registry is a call to RegisterEnemy
```csharp
RegisterEnemy(string enemyId, string label);
```
The enemy id is the unique Id of the modded enemy type. Again, it is recommended to follow the pattern <ModAuthorName>.<ModdedEnemyType> to ensure that this id is unique even when other mods are installed. The label is how it will appear in the GUI.

From there, in your enemy's implementation, probably in its start method or at some point when it's spawning, call:
```csharp
Skin randomSkin = EnemySkinRegistry.PickSkin(myModdedEnemyId);
EnemySkinRegistry.ApplySkin(randomSkin, myModdedEnemyId, enemyGameObject);
```

For vanilla enemies, "enemyGameObject" is the game object that contains the EnemyAI component, but you can put whatever suits your fancy as long as anyone trying to change the appearance of your enemy can reasonably get at what they need to change the appearance of your modded enemy instance. If you do go against this convention, make sure anyone trying to make a skin mod for your enemy is aware of this.

Skins also have the option to register EventHandler implementations for enemies during the call to apply. To get a reference to the EventHandler for one of your modded enemy instances, call
```csharp
EnemySkinRegistry.GetEnemyEventHandler(enemyGameObject);
```
This will be null if no handler was registered or if the skin was removed.

Here, enemyGameObject is the same game object used in the call to Apply. All EnemyEventHandlers come with a handful of common events (Spawn, Hit, Die, Stunned etc.). These common events are handled by the patcher inculded in this mod. However, you can give other modders more functionality by extending the EnemyEventHandler interface with events epecific to your enemy.
```csharp
interface MyEnemyEventHandler : EnemyEventHandler()
{
    void OnDoThing(ModdedEnemyAI enemyAI, <OtherParams>);
    ...
}
```
and then in your enemy logic:
```csharp
class MyEnemy : EnemyAI
{
    //whoa, this is some snazzy custom enemy code

    void DoThing()
    {
        ...
        (EnemySkinRegistry.GetEnemyEventHandler(this) as MyEnemyEventHandler)?.OnDoThing(this);
        ...
    }

    //golly! even more immaculate enemy code!
}
```
This way, modders can have their skins do something in response to your enemy doing "Thing."

### Registering Modded Moons

To add your modded moon to be configurable in the menu, just call
```csharp
EnemySkinRegistry.RegisterMoon(string planetName, string configLabel);
```
Where "planetName" must match the field of the same name in your moon's SelectableLevel object. The field "configLabel" is how your moon will appear in the GUI. For the vanilla moons, I omitted the number, but you're free to do whatever pleases you. 