using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSolution : MonoBehaviour
{
  [SerializeField] private bool disableLegacyConfigWindows = true;
  
  void Awake()
  {
    var mainCanvas = transform.GetChild(0);
    mainCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
    if (disableLegacyConfigWindows)
    {
      var containerPanel = mainCanvas.GetChild(0);
      var headerPanel = containerPanel.GetChild(1);
      headerPanel.gameObject.SetActive(false);
      var footerPanel = containerPanel.GetChild(2);
      footerPanel.gameObject.SetActive(false);
    }
  }
}
