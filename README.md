# VIAProMa

A Collaborative Mixed Reality Visualization Framework for Immersive Analytics

<p align="center">
    <img src="Frontend/Texture%20Source%20Files/Logo/Logo.png" alt="VIAProMa Logo" height="300" />
</p>

## Getting Started

### Frontend

#### Prerequisites

- Recommended [Unity version](https://unity3d.com/de/get-unity/download/archive): 2019.4.5f1
- [Microsoft Mixed Reality Toolkit v2.6.1](https://github.com/microsoft/MixedRealityToolkit-Unity/releases/tag/v2.6.1) (already included in the project)
- [i5 Toolkit for Unity](https://github.com/rwth-acis/i5-Toolkit-for-Unity) (already included in the project)
- [Photon PUN 2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) (download through Unity's asset store window in the editor)
- Visual Studio (tested with VS 2019)
- For HoloLens Development:
  - Windows 10 development machine
  - Windows 10 SDK ([10.0.18362.0](https://developer.microsoft.com/de-de/windows/downloads/windows-10-sdk))
- For Android Development:
  - [ARCore SDK](https://github.com/google-ar/arcore-unity-sdk/releases) (tested with ARCore SDK for Unity v1.12.0)
  - Android SDK 7.0 (API Level 24) or later

#### Project Setup

1. Install Unity and Visual Studio
2. Clone the project
3. Start Unity, select open and choose the folder Frontend/VIAProMa in the project files
4. Once the project has been loaded, go to Window > Asset Store (or press Ctrl + 9).
Search and download "PUN 2 - FREE" by Exit Games.
To do so, the asset has to be aquired first, then downloaded and then imported.
5. Once PUN has been downloaded, an import window will appear.
Select everything and click on import.
6. After the successful import of the assets, a window should appear which asks for an app ID.
7. Create a free Photon Engine accout under [https://www.photonengine.com/en/pun](https://www.photonengine.com/en/pun).
8. Register a Photon PUN application in order to generate an app ID.
9. Enter the app ID in the dialog window in Unity.
If the window has not appeared or has already been closed, go to Window > Photon Unity Networking > Highlight Server Settings and enter the app ID in the inspector under "Settings/ App Id Realtime".
10. Again, go to the Asset Store and download "Photon Voice 2" by Exit Games.
11. In the import window, select everything that is selectable and click on import.
Some parts of the import are not selectable.
This is fine since they have already been imported with the Photon PUN 2 package.
12. In the the dashboard of the Photon Engine account, create a register a new Photon Voice application and copy its app ID.
13. After the import, go to Window > Photon Unity Networking > Highlight Server Settings and enter the voice app ID under "Settings/ App Id Voice".
14. Open the scene "Main Scene".
A prompt will appear which will ask to import TMP Essentials. Click on the upper button.
15. (Optional) It makes sense to scale the editor icons down.
This can be done under "Gizmos" in the top right of the 3D view.
Pull the top most slider next to "3D Icons" down until the icons in the scene have the right size
16. Run the application once in the editor in order to initialize the collection which keeps track of networked objects.

For Android development, additional steps can be found in ARCore's [quickstart guide](https://developers.google.com/ar/develop/unity/quickstart-android).

#### Tests

In addition to the working logic, each of the project's features has been isolated into its own scene which contains a minimal working example for the feature.
This way, the functionality can be tested and new developers can experiment with these features in order to learn how they work.
The scenes are situated in the folder "Tests".

### Backend

The backend folder contains a [las2peer project](https://github.com/rwth-acis/las2peer-template-project) which realizes a RESTful service.

#### Required Technologies

To install the backend the following technologies are required.
1. Java 8 (on Windows: make sure that the Java installation is placed in your path variables); if you type java –version in the command line, the output should similar to this:
   ```
   java version "1.8.0_231"
   Java(TM) SE Runtime Environment (build 1.8.0_231-b11)
   Java HotSpot(TM) 64-Bit Server VM (build 25.231-b11, mixed mode)
   ```
2. Apache Ant (on Windows: make sure that the Ant installation is placed in your path variables; if you type ant –version in the command line, you should get an output similar to this:
   ```
   Apache Ant(TM) version 1.10.1 compiled on February 2 2017
   ```

To develop the backend, you should also install an IDE, e.g. IntelliJ.

#### Building the Backend

You first have to fetch the dependencies by running `ant get_deps` in the backend folder where the build.xml is stored.

After that, run `ant all` in the backend folder.

#### Running the Backend

After the build, execute the corresponding “start_network” script in the “bin” folder of the backend.
It contains two scripts “start_network.bat” and “start_network.sh”. On Windows, execute the “start_network.bat” file. On Linux or Mac, first go back to the backend folder and execute `./bin/start_network.sh` from there.


## Troubleshooting

**Problem:** When building the application, errors are shown that the namespace name 'HandJointKind', 'HandMeshObserver' and 'JointPose' are not found.

**Solution:** Download the Windows 10 SDK [10.0.18362.0](https://developer.microsoft.com/de-de/windows/downloads/windows-10-sdk).
After that, open the Build Settings Window and change the Target SDK Version and Minimum Platform Version to 10.0.18362.0.

**Problem:** The shared room which was created in one app instance does not appear on the other app instance.

**Solution:** Make sure that both app instances use the same gameVersion which is specified in the launcher script. (Assets/Scripts/Multiplayer/Launcher.cs)
Also ensure that both app instances use the same Photon PUN version.
The PUN version can be seen in the Photon server settings.
Additionally, both instances must have the same Photon app-IDs in the server settings.
