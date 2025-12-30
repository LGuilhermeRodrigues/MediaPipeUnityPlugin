using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SidePanel : MonoBehaviour
{
    [SerializeField] PoseSolution poseSolution;
  
    private bool panel_hidden = true;
    VisualElement screen;
    VisualElement left_panel;
    VisualElement button_image;
  
    void Start()
    {
      var root = GetComponent<UIDocument>().rootVisualElement;
      screen = root.Q<VisualElement>("Screen");
      var openButton = root.Q("OpenButton") as Button;
      button_image = openButton.Q<VisualElement>("Image");
      openButton?.RegisterCallback<ClickEvent>(evt => Toggle());
      left_panel = root.Q("LeftPanel");
      
    }

    void Toggle()
    {
      panel_hidden  = !panel_hidden;
      if (panel_hidden)
      {
        screen.RemoveFromClassList("gui-paused-screen");
        left_panel.AddToClassList("gui-hidden-panel");
        button_image.AddToClassList("gui-button-image-open");
        button_image.RemoveFromClassList("gui-button-image-close");
        poseSolution.Resume();
      }
      else
      {
        screen.AddToClassList("gui-paused-screen");
        left_panel.RemoveFromClassList("gui-hidden-panel");
        button_image.RemoveFromClassList("gui-button-image-open");
        button_image.AddToClassList("gui-button-image-close");
        poseSolution.Pause();
      }
    }
}
