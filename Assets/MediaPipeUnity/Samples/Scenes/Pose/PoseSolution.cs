using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using UnityEngine;
using UnityEngine.UI;

public class PoseSolution : MonoBehaviour
{
  [SerializeField] private bool hideLegacyConfigWindows = true;
  [SerializeField] private bool hideLeftPanel = false;
  private BaseRunner _baseRunner;
  private bool has_changes;
  private bool has_changes_in_resolution;
  private GameObject segmentationMaskObject;
  private bool _isFindingMask = false;
  
  void Awake()
  {
    // Mediapipe Plugin Main Canvas uses unity Canvas component, it needs the associate the scene camera 
    var mainCanvas = transform.GetChild(0);
    mainCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
    
    if (hideLegacyConfigWindows) DisableLegacyWindows(mainCanvas);
    
    // Hide SidePanelUIDocument
    if (hideLeftPanel) transform.GetChild(1).gameObject.SetActive(false);
  }

  private void Start()
  {
    GameObject mediapipeSolution = GameObject.Find("Solution");
    _baseRunner = mediapipeSolution.GetComponent<BaseRunner>();
    UpdateSegmentationMask();
  }

  void DisableLegacyWindows(Transform mainCanvas)
  {
    var containerPanel = mainCanvas.GetChild(0);
    var headerPanel = containerPanel.GetChild(1);
    headerPanel.gameObject.SetActive(false);
    var footerPanel = containerPanel.GetChild(2);
    footerPanel.gameObject.SetActive(false);
  }

  public void NotifyChanges(bool changeInResolution=false)
  {
    has_changes = true;
    has_changes_in_resolution = changeInResolution;
  }

  public void Pause()
  {
    _baseRunner.Pause();
    has_changes = false;
  }
  
  public void Resume()
  {
    if (has_changes)
    {
      if (has_changes_in_resolution)
      {
        segmentationMaskObject = null;
        GameObject mask = GameObject.Find("MaskOverlayAnnotation");
        var annotationInstance = mask.transform.parent.gameObject;
        Destroy(annotationInstance);
      }
      _baseRunner.Play();
      UpdateSegmentationMask();
    }
    else
    {
      _baseRunner.Resume();
    }
  }
  
  private IEnumerator FindSegmentationMask()
  {
    _isFindingMask = true;
    while (segmentationMaskObject is null)
    {
      Debug.Log("Finding SegmentationMask");
      GameObject mask = GameObject.Find("Mask Screen");
      if (mask != null)
      {
        segmentationMaskObject =  mask;
        _isFindingMask = false;
        UpdateSegmentationMask();
      }
      else
      {
        yield return new WaitForSeconds(0.1f);
      }
    }
  }
  
  private void UpdateSegmentationMask()
  {
    if (segmentationMaskObject != null)
    {
      var rawImage = segmentationMaskObject.GetComponent<RawImage>();
      if (rawImage != null)
      {
        rawImage.enabled = CustomSettings.SegmentationMask;
        if(rawImage.enabled){
          var width = rawImage.texture.width;
          var height = rawImage.texture.height;
          // Checking resolution again in case of an invalidid resolution
          var resolutionIndex = CustomSettings.CameraResolutionIndex;
          Debug.Log("Resolution Index: " + resolutionIndex);
          switch (resolutionIndex)
          {
            case 0:
              width = 320;
              height = 240;
              break;
            case 1:
              width = 640;
              height = 360;
              break;
            case 2:
              width = 640;
              height = 480;
              break;
            case 3:
              width = 1280;
              height = 720;
              break;
            case 4:
              width = 1920;
              height = 1080;
              break;
          }
          Texture2D newTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
          Color[] pixels = new Color[width * height];
          var fillColor = CustomSettings.MaskColor;
          for (int i = 0; i < pixels.Length; i++)
            pixels[i] = fillColor;
          newTex.SetPixels(pixels);
          newTex.Apply();
          rawImage.texture = newTex;
        }
      }
    }
    else
    {
      if (!_isFindingMask)
        StartCoroutine(FindSegmentationMask());
    }
  }
}
