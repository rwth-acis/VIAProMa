# VIAProMa

A Collaborative Mixed Reality Visualization Framework for Immersive Analytics

<p align="center">
    <img src="Frontend/Texture%20Source%20Files/Logo/Logo.png" alt="VIAProMa Logo" height="300" />
</p>

## Getting Started

### Prerequisites

- Recommended [Unity version](https://unity3d.com/de/get-unity/download/archive): 2018.3.14f1
- [Microsoft Mixed Reality Toolkit v2.0.0 RC2.1](https://github.com/microsoft/MixedRealityToolkit-Unity/releases/tag/v2.0.0-RC2.1) (already included in the project)
- [Photon PUN 2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) (download through Unity's asset store window in the editor)
- Visual Studio (tested with VS 2017)
- For HoloLens Development:
  - Windows 10 development machine
  - Windows 10 SDK ([10.0.18362.0](https://developer.microsoft.com/de-de/windows/downloads/windows-10-sdk))

### Project Setup

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

### Tests

In addition to the working logic, each of the project's features has been isolated into its own scene which contains a minimal working example for the feature.
This way, the functionality can be tested and new developers can experiment with these features in order to learn how they work.
The scenes are situated in the folder "Tests".

## Deployment

## Troubleshooting

**Problem:** When building the application, errors are shown that the namespace name 'HandJointKind', 'HandMeshObserver' and 'JointPose' are not found.

**Solution:** Download the Windows 10 SDK [10.0.18362.0](https://developer.microsoft.com/de-de/windows/downloads/windows-10-sdk).
After that, open the Build Settings Window and change the Target SDK Version and Minimum Platform Version to 10.0.18362.0.
