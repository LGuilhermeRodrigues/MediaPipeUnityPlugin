using System.Collections.Generic;
using System.Globalization;
using Mediapipe.Unity.Sample;
using UnityEngine;

public static class CustomSettings
{
    private const string CameraNameKey = "CameraName";
    private const string CameraResolutionIndexKey = "CameraResolutionIndex";
    private const string CameraFlippedKey = "CameraFlippedIndex";
    private const string ShowFPSKey = "ShowFPS";
    private const string ModelTypeKey = "ModelType";
    private const string SegmentationMaskKey = "SegmentationMask";
    private const string MaskColorKey = "MaskColor";
    private const string DataFolderName = "BRAINN_XR Data";

    public static string CameraName
    {
        get
        {
            
            // Mediapipe Default Value:
            var imageSource = ImageSourceProvider.ImageSource;
            var mediapipeSourceName = imageSource.sourceName;
            var availableOptions = GetCameraNameOptions();
            if (mediapipeSourceName is null)
            {
                mediapipeSourceName = availableOptions[0];
            }
            
            // Check if the playerPrefs have another source or keep the Mediapipe Default
            var playerPref = PlayerPrefs.GetString(CameraNameKey, mediapipeSourceName);
            
            // check if the player prefs really exists
            // because the camera can be unplugged after the previous sessions
            if (availableOptions.Contains(playerPref))
            {
                return playerPref;
            }
            
            // return the default mediapipe one, ignoring the inexistent camera
            return mediapipeSourceName;
        }
        set
        {
            PlayerPrefs.SetString(CameraNameKey, value);
            PlayerPrefs.Save();
        }
    }

    public static List<string> GetCameraNameOptions()
    {
        var imageSource = ImageSourceProvider.ImageSource;
        var sourceNames = imageSource.sourceCandidateNames;
        var options = new List<string>(sourceNames);
        return options;
    }

    public static int CameraResolutionIndex
    {
        get
        {
            var savedPref = PlayerPrefs.HasKey(CameraResolutionIndexKey)
                ? PlayerPrefs.GetInt(CameraResolutionIndexKey)
                : 3;
            var imageSource = ImageSourceProvider.ImageSource;
            if (imageSource is null || imageSource.GetCurrentTexture() is null)
                return savedPref;
            var w = imageSource.GetCurrentTexture().width;
            var h = imageSource.GetCurrentTexture().height;
            var runningResolution = w+"x"+h;
            var runningResolutionIndex = 0;
            switch (runningResolution)
            {
                case "640x360":
                    runningResolutionIndex = 1;
                    break;
                case "640x480":
                    runningResolutionIndex = 2;
                    break;
                case "1280x720":
                    runningResolutionIndex = 3;
                    break;
                case "1920x1080":
                    runningResolutionIndex = 4;
                    break;
            }
            if (runningResolutionIndex != savedPref)
            {
                PlayerPrefs.SetInt(CameraResolutionIndexKey, runningResolutionIndex);
                PlayerPrefs.Save();
                Debug.Log("saved: " + runningResolutionIndex+ " replaced "+ runningResolution);
            }
            return runningResolutionIndex;
        }
        set
        {
            PlayerPrefs.SetInt(CameraResolutionIndexKey, value);
            PlayerPrefs.Save();
        }
    }
    
    public static bool IsCameraFlipped
    {
        get
        {
            if (PlayerPrefs.HasKey(CameraFlippedKey))
                return PlayerPrefs.GetInt(CameraFlippedKey) == 1;
            return false;
        }
        set
        {
            PlayerPrefs.SetInt(CameraFlippedKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    
    public static bool ShowFPS
    {
        get
        {
            if (PlayerPrefs.HasKey(ShowFPSKey))
                return PlayerPrefs.GetInt(ShowFPSKey) == 1;
            return false;
        }
        set
        {
            PlayerPrefs.SetInt(ShowFPSKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    
    public static int ModelType
    {
        get
        {
            if (PlayerPrefs.HasKey(ModelTypeKey))
                return PlayerPrefs.GetInt(ModelTypeKey);
            return 0;
        }
        set
        {
            PlayerPrefs.SetInt(ModelTypeKey, value);
            PlayerPrefs.Save();
        }
    }

    public static bool SegmentationMask
    {
        get
        {
            if (PlayerPrefs.HasKey(SegmentationMaskKey))
                return PlayerPrefs.GetInt(SegmentationMaskKey) == 1;
            return false;
        }
        set
        {
            PlayerPrefs.SetInt(SegmentationMaskKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static string ColorToString(Color c)
    {
        string formatted = string.Format(
            CultureInfo.InvariantCulture,
            "{0:F3};{1:F3};{2:F3}",
            c.r, c.g, c.b
        );
        return formatted;
    }

    public static Color StringToColor(string formatted)
    {
        string[] parts = formatted.Split(';');
        float r = float.Parse(parts[0], CultureInfo.InvariantCulture);
        float g = float.Parse(parts[1], CultureInfo.InvariantCulture);
        float b = float.Parse(parts[2], CultureInfo.InvariantCulture);
        return new  Color(r, g, b, 1f);
    } 

    
    public static Color MaskColor
    {
        get
        {
            if (PlayerPrefs.HasKey(MaskColorKey))
                return StringToColor(PlayerPrefs.GetString(MaskColorKey));
            return StringToColor("0.666,0.666,0.666");
        }
        set
        {
            PlayerPrefs.SetString(MaskColorKey,ColorToString(value));
            PlayerPrefs.Save();
        }
    }
}
