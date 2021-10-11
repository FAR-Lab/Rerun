# Rerun documentation

# Overview

**Rerun** allows the user to record and play back activities from a VR scene. It is built on top of the Ultimate Replay 2.0 (**UR**) asset from Unity's Asset Store.

This repository is meant to be used as **a git submodule inside the Assets folder of a Unity Project**. See the Rerun Sample Project as an example of how Rerun can be used.

Extending Rerun requires knowledge of UR. See UR's user guide and samples for more information.

# Installation instructions

1. Do one of the following:
    * Add this repository as a git submodule in your Unity project, inside the Assets folder. 
    * Or, clone this repository and copy into your Assets folder.
3. Install the required packages and assets, see _Requirements_ below.
4. Follow the _Workflow_ instructions below for integrating Rerun into your project

# Requirements

## Software
The following assets are required for Rerun:

- [Ultimate Replay 2.0](https://assetstore.unity.com/packages/tools/camera/ultimate-replay-2-0-178602) (paid) - tested with version 2.1.8
- [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022)  - tested with version 33.0

The Rerun Sample Project comes with the Oculus Integration but users must bring their own version of Ultimate Replay 2.0 into any project using Rerun.

### Unity Editor
Rerun has been developed and tested with Unity 2020.3 (LTS). The exact version is 2020.3.17f, but more recent 2020.3 (LTS) versions should work fine.

## Hardware
Rerun has been tested with Oculus Quest 2, tethered to a PC via Oculus Link connection. Make sure that your device has hand tracking enabled.

# Limitations

- The .replay recordings must be opened in the same scene they were recorded in.

# Workflows

Rerun is currently set up for recording and playing back a VR user with hand tracking enabled. The workflow below assumes the required packages and assets have been set up correctly.

To use Rerun in your scene:

1. Drag all three prefabs in the Prefabs/Main folder into your scene hierarchy
    1. You must remove any existing OVRCameraRig, the Rerun prefab will replace this
2. Set the RerunOVRCameraRig as the Rig Source reference in the RerunManager inspector

You should now be able to record and play using the controls as indicated on the screen.

# References
The prefabs inside Prefabs/Main are most likely the only ones you will bring into your scene.

## RerunManager
RerunManager has public methods that can used to control recoring and playback through code.
The use of Begin and Stop is to match with Ultimate Replay API.

### Public methods
- `BeginRecording()`
- `StopRecording()`
- `ToggleRecording()` : Toggles the recording mode. If using a single input source for both begin/stop recording, then use this.
- `Play()` : Plays the currently loaeded recording. Only works if not recording.
- `Live()` : Regular live mode. Only works if not recording.
- `Open()` : Open a file dialog for loading a .replay file. Only works if not recording.

### Properties
- `recordingPrefix` : String prefix for file name. Use inspector, or property to set programmatically. For example, use to store session ID, user name, scene name etc., in the file name.
- `infoString` : String that contains information about active mode, name of file being recorded/played etc. Read only.

## RerunGUI
The RerunGUI prefab provides convenient recording and playback controls. It uses Unity's IMGUI which means that it doesn't show up in the view of the VR
The RerunGUI prefab provides convenient recording and playback controls:
- R: Start/stop recording
- P: Play the current recording
- L: Live view
- O: Open .replay files

During recording, the user must stop recording before switching modes

The playback 

## RerunOVRCameraRig
This prefab replaces the regular OVRCameraRig, for hand tracking. It has UR recording components on multiple game objects in the rig hierarchy, for recording the head pose and handtracking data.



# Samples

Sample scenes can be found within the Scenes folder. Currently the following samples are available:

## Simple VR
This sample shows how to use the main prefabs to record headpose and handtracking data, with keyboard recording controls and graphical overlay using RerunGUI.

See the Rerun Sample Project for a ready-to-use project.


