# Preface
* This document provides a brief [**overview**](https://github.com/QuasiFungi/momentum#overview) of the project, how to [**prepare**](https://github.com/QuasiFungi/momentum#preparation) this project for development on your PC and the [**softwares**](https://github.com/QuasiFungi/momentum#softwares) used for development
* I've left short "Tips" within the project folders that summarize how I did certain things
* All resources given here have been updated to match the latest devlog
* This project is distributed under the **MIT License**, so all project resources are **free** to download and use, commercially or otherwise
* Giving credit is **not** required, but it is appreciated~
---
# Overview
## Contents
The three main folders each contain,
* **Blender** - the **Blender Files** for *Models* of various objects used in the project
* **Build** - the **APK Files** used when testing the game on a phone
* **Unity** - the **Unity Project Files** for both the *Demonstrations* and *Test Level* shown in the devlog
## Structure
Unity stores all user generated files inside the **Assets** folder.
These files are visible by default in the **Project Window**.
Inside you'll find subfolders containing,
- **Animations** - *Controllers* and *Animation Clips* for all animated objects
- **Materials** - *Materials* used by objects, including *Physics Materials*
- **Models** - *Blender Models* imported in the FBX format
- **Prefabs** - reusable *Objects* with predefined/customizable settings
- **Resources** - *Objects* accessable via script that need to be instantiated at runtime
- **Scenes** - all *Levels* to be used in the game
- **Scripts** - code that controls all object *Behaviors* and defines the *Game Logic*
- **Settings** - various *Configuration* files
- **Shaders** - *Custom Shaders* used by materials
- **Sounds** - imported sound files for *Music* and *Sound Effects*
- **Textures** - *Image* files used by 3D and 2D object materials

Some additional things to note when using this project,
- **Layers** are used to group objects based on their **Behaviour Type**, which include Player/Breakable/Interactive/etc
- **Tags** are used to identify **Surface Types** for particle effects generated on object collision, like Concrete/Metal/Wood/etc
---
# Preparation
## Set up via Download:
To download the **Project Repository**,
* Click the *green button* that says **Code** at the top right next to the branch information
* In the *drop-down menu*, click **Download ZIP**
* Once the file has finished downloading, extract it's contents

These steps are *optional*,
* Move the Folder **momentum** found in the provided files, into your **Unity Project Directory**
* For me, this **Folder** is located at, *home/Downloads/momentum-master/Unity/momentum*
* And the target **Directory** is, *home/Unity/Projects/*

For the final step I *recommend* getting [Unity Hub](https://unity.com/unity-hub).
It makes it easier to keep track of Unity versions/updates and your Projects. I use version 2.4.6.
### A. Unity Hub v3.0.0 or later
* Open **Unity Hub** and select the *small downwards-pointing arrow* to the right of **Open**
* In the *drop-down menu* pick **Add project from disk**
* Navigate to the directory that contains the downloaded project
* Click **Open** with the folder **momentum** selected
* You should now have **momentum** visible in the listed projects in **Unity Hub**
### B. Unity Hub pre v3.0.0
* Open **Unity Hub** and select **Add**
* Navigate to the directory that contains the downloaded project
* With the folder **momentum** selected, click **Open**
* You should now have **momentum** visible in the listed projects in **Unity Hub**
### ~~C. Without Unity Hub~~
* ~~Launch **Unity**~~
* ~~From the *menu bar* click **File**~~
* ~~Then from the *drop-down menu* pick **Open Project...**~~
* ~~Select the Folder **momentum** found in the downloaded files and **Open**~~

## Set up via Git:
* If you don't have **Git** installed on your PC, [this](https://www.atlassian.com/git/tutorials/install-git) article explains the installation process for Mac, Windows and Linux
* Follow [these](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository#cloning-a-repository) instructions to clone this GitHub Repository
---
# Softwares
## [Unity](https://www.unity.com) v2020.3.33f1
Installed packages include,
- **2D Sprite** - edit sprite properties, like the border, pivot, etc
- **Shader Graph** - create custom material shaders using a node-based GUI
- **Input System** - better way of handling player input from varied devices/platforms
- **Universal RP** - easier, streamlined control over graphics settings
- **Visual Studio Code Editor** - add support for VSCode to function as the script editor
## [Blender](https://www.blender.org) v3.2.0
Alternate version used,
- **Blender Fracture Modifier** by **Martin Felke** - generate fractured versions of objects, like crates
## [GNU Image Manipulation Program](https://www.gimp.org) v2.10.28
Used as is.
## [Visual Studio Code](https://code.visualstudio.com) v1.68.1
Extention used,
- **C#** - quality of life features like syntax highlighting, etc
## [Audacity](https://www.audacityteam.org) v3.1.3
Used as is.