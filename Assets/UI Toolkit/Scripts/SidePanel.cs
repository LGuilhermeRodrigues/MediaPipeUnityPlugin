using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity.Sample;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

public class SidePanel : MonoBehaviour
{
    [SerializeField] PoseSolution poseSolution;
    private Bootstrap _bootstrap;
    [SerializeField] private StringTable stringTable;
    private bool panel_hidden = true;
    VisualElement screen;
    VisualElement left_panel;
    private Button open_button;
    VisualElement button_image;
    DropdownField dropdownFieldCameraSource;

    private void Awake()
    {
      var root = GetComponent<UIDocument>().rootVisualElement;
      screen = root.Q<VisualElement>("Screen");
      open_button = root.Q("OpenButton") as Button;
      open_button?.SetEnabled(false);
      button_image = open_button.Q<VisualElement>("Image");
      open_button?.RegisterCallback<ClickEvent>(evt => Toggle());
      left_panel = root.Q("LeftPanel");
      left_panel.AddToClassList("gui-hidden-panel");
      root.Query<Label>().ForEach(UpdateString);
      dropdownFieldCameraSource = root.Q<DropdownField>("DropdownCamera");
      dropdownFieldCameraSource.RegisterValueChangedCallback(v =>
      {
        CustomSettings.CameraName = v.newValue;
        poseSolution.has_changes  = true;
      });
    }

    void Start()
    {
      StartCoroutine(WaitForBootstrap());
    }
    
    private IEnumerator  WaitForBootstrap()
    {
      while (_bootstrap is null)
      {
        GameObject bootstrapObj = GameObject.Find("Bootstrap");
        if (bootstrapObj != null)
        {
          _bootstrap = bootstrapObj.GetComponent<Bootstrap>();
        }
        if (_bootstrap is null)
        {
          yield return new WaitForSeconds(0.1f);
        }
      }
      while (!_bootstrap.isFinished)
      {
        yield return new WaitForSeconds(0.1f);
      }
      open_button.SetEnabled(true);
      //UpdateFpsLabel();
      dropdownFieldCameraSource.choices.AddRange(CustomSettings.GetCameraNameOptions());
      var currentCamera = CustomSettings.CameraName;
      dropdownFieldCameraSource.SetValueWithoutNotify(currentCamera);
    }

    private void UpdateString(Label label)
    {
      var key = label.text;
      var entry = stringTable.GetEntry(key);
      if (entry == null) return;
      label.text = entry.Value;
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
