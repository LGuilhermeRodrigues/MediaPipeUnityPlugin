# MediaPipe Unity Plugin Pose Tracker

This repo. was used to change the build settings of the MediaPipeUnityPlugin and create a smaller asset bundle, with only the pose tracker model.

Before opening the Unity Editor:

1. Download (MediaPipeUnityPlugin-all-stripped)[https://github.com/homuler/MediaPipeUnityPlugin/releases/tag/v0.16.3]
2. Copy pose_landmarker_* files from MediaPipeUnityPlugin-all-stripped\Packages\com.github.homuler.mediapipe\PackageResources\MediaPipe to MediaPipeUnityPlugin\Packages\com.github.homuler.mediapipe\PackageResources\MediaPipe
3. Copy pose_landmarker_*.bytes from MediaPipeUnityPlugin-all-stripped\Packages\com.github.homuler.mediapipe\PackageResources\MediaPipe to MediaPipeUnityPlugin\Assets\StreamingAssets (without .meta files)
4. Copy Runtime folder from Packages\com.github.homuler.mediapipe to MediaPipeUnityPlugin\Packages\com.github.homuler.mediapipe