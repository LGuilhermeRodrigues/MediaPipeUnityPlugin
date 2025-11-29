# MediaPipe Unity Plugin Pose Tracker

⚠️ If you have openned Unity Editor before following the instructions below, you will have problems with .meta files and other errors.

This repo. was used to change the build settings of the MediaPipeUnityPlugin and create a smaller asset bundle, with only the pose tracking model.

Before opening the Unity Editor:

1. Download (MediaPipeUnityPlugin-all-stripped)[https://github.com/homuler/MediaPipeUnityPlugin/releases/tag/v0.16.3]
2. Copy pose_landmarker_*.bytes files from MediaPipeUnityPlugin-all-stripped\Packages\com.github.homuler.mediapipe\PackageResources\MediaPipe to MediaPipeUnityPlugin\Packages\com.github.homuler.mediapipe\PackageResources\MediaPipe (this step is necessary to run pose tracking in Unity Editor)
3. Copy pose_landmarker_*.bytes from MediaPipeUnityPlugin-all-stripped\Packages\com.github.homuler.mediapipe\PackageResources\MediaPipe to MediaPipeUnityPlugin\Assets\StreamingAssets and make sure you are not copying .meta files (this step is necessary to run pose tracking in Unity Build)
4. Copy Runtime folder from Packages\com.github.homuler.mediapipe to MediaPipeUnityPlugin\Packages\com.github.homuler.mediapipe
