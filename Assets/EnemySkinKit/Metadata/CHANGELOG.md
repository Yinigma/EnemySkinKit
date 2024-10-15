# Changelog

### 1.3.4
- Fate refused to change
- Rolled back to 1.2.5. Gonna try and play nice with soundAPI next time.

### 1.3.1
- Changed the version proper

### 1.3.0
- Refactored audio to avoid use of events

### 1.2.5
- Fixed Old Bird nest skin material always being applied
- Added sound replacer for coilhead's cooldown sound

### 1.2.4
- Added Maneater skinner
- Bonemapper now is best effort rather than strict. Armatures don't have to perfectly match now, it's just that anything that isn't perfectly matched isn't getting animated.

### 1.2.2
- Fixed bug with last update where NREs would get thrown if the ghost girl's main mesh wasn't replaced

### 1.2.1
- Changed ghost girl hide events to recursively include all renderers in the replacement game object and armature attachments. Don't put too much stuff in there. This ain't Risk of Rain 2.

### 1.2.0
- Added Manticoil skinner
- Added Circuit Bee skinner
- Added Roaming Locust skinner
- Added fields for skinning Mask Hornets
- Added fields for particle effects
- Added fields for replacing the diffuse and mask textures of the kidnapper fox
- Added field for setting the gradient color of the Hygrodere
- Added fields for setting the static mesh of the Old Bird's light, as well as the color of its spotlight
- Added option to separately set the nest fields of the Old Bird's nest skinner
- Fixed issue with REPLACE_MESH where mesh bounds weren't updating

### 1.1.7
- Fixed a bug with the last patch preventing audio lists from applying

### 1.1.6
- Little patch to prevent NREs on dev build
- Corrected a mislabelling on the hoarding bug skin scriptable object

### 1.1.5
- Fixed mismatched Bracken materials

### 1.1.4
- Version 1.1.4 courtesy of Thunderstore's handling of ordered lists. Everyone give them a hand.

### 1.1.3
- Updated readme to include issue with LLL soft dependency.


### 1.1.2
- Fixed null check on audio source action
- Added new material action type REPLACE_TEXTURE
- Fixed Material assignment on Tulip Snake
- Updated readme to include note on HDRP rendering modes and post-processing

### 1.1.1

- \*puts on dunce cap\* Updated Readme

### 1.1.0

- Deprecated Armature Map
- Added new skinned mesh action type REPLACE_MESH
- Added new Example models for the new REPLACE_MESH skinning method
- Added default Skins and Skinners for Tulip Snake, Barber, and Kidnapper Fox
- Swapped coilhead rust and body materials so they replace the correct assets

### 1.0.6

- Fixed crash when spider would not be assigned certain sound effects

### 1.0.5

- Fixed crash when snare flea would cling to the ceiling without a replaced sound effect.
- Fixed bug where ghost girl was visible to players not being haunted.
- Fixed mod building error message on file delete

### 1.0.4

- Added export settings to readme.


### 1.0.3

- Corrected some mismatched sounds.

### 1.0.2

- Fixed broken manifest.


### 1.0.1

- Fixed code generation to include default skin configurations.


### 1.0.0

- Added Default Skins and Skinners for the Old Bird and Butler.
- Reworked sounds to be more comprehensive. Most if not all previously excluded sounds that are in AnimationEvent objects, EnemyType scriptable objects, BehaviourState objects, MiscellaneousAnim objects, or unaccounted for AudioSource components have now been included in the default skins. Removed muting of creature voice and creature sfx as they should now be unecessary for complete sound replacement. Any sounds that require it will now be played using events.
- Added default skin configs to the codeless mod file generator.

### 0.2.8
- Fixed ghost girl skins not disappearing when the vanilla ghost girl would. 

### 0.2.7

- First usable version