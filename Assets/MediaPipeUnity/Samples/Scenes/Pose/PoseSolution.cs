using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity.Sample;
using UnityEngine;

public class PoseSolution : MonoBehaviour
{
  [SerializeField] private bool hideLegacyConfigWindows = true;
  [SerializeField] private bool hideLeftPanel = false;
  private BaseRunner _baseRunner;
  private bool has_changes;
  
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
  }

  void DisableLegacyWindows(Transform mainCanvas)
  {
    var containerPanel = mainCanvas.GetChild(0);
    var headerPanel = containerPanel.GetChild(1);
    headerPanel.gameObject.SetActive(false);
    var footerPanel = containerPanel.GetChild(2);
    footerPanel.gameObject.SetActive(false);
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
      _baseRunner.Play();
    }
    else
    {
      _baseRunner.Resume();
    }
  }
}
