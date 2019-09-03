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
  - Windows 10 SDK (10.0.18362.0)

### Project Setup

1. Install Unity and Visual Studio
2. Clone the project
3. Start Unity, select open and choose the folder Frontend/VIAProMa in the project files
4. Once the project is open, go to Window > Asset Store (or press Ctrl + 9).
Search and download "PUN 2 - FREE" by Exit Games.
5. Once PUN has been downloaded, an import window will appear.
Select everything and click on import.
6. After the successful import of the assets, a window should appear which asks for an app ID.
7. Create a free Photon Engine accout under [https://www.photonengine.com/en/pun](https://www.photonengine.com/en/pun).
8. Register a Photon Realtime application in order to generate an app ID.
9. Enter the app ID in the dialog window in Unity.
If the window has not appeared or has already been closed, go to Window > Photon Unity Networking > Highlight Server Settings and enter the app ID in the inspector under "App Id Realtime".
10. Run the application once in the editor in order to initialize the collection which keeps track of networked objects.