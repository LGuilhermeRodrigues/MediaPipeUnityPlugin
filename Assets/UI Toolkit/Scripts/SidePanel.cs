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
    VisualElement exitPauseScreen;
    VisualElement left_panel;
    private Button open_button;
    VisualElement button_image;
    DropdownField dropdownFieldCameraSource;
    private DropdownField dropdownFieldCameraResolutionIndex;
    private Toggle toggleIsCameraFlipped;
    private Toggle toggleSegmentationMask;
    private DropdownField dropdownModelType;

    private void Awake()
    {
      var root = GetComponent<UIDocument>().rootVisualElement;
      screen = root.Q<VisualElement>("Screen");
      exitPauseScreen = root.Q<VisualElement>("TogglePauseScreen");
      open_button?.RegisterCallback<ClickEvent>(evt => Toggle());
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
        poseSolution.NotifyChanges();
      });
      dropdownFieldCameraResolutionIndex = root.Q("DropdownResolution") as DropdownField;
      dropdownFieldCameraResolutionIndex.RegisterValueChangedCallback(evt =>
      {
        CustomSettings.CameraResolutionIndex = CameraResolutionTextToIndex(evt.newValue);
        poseSolution.NotifyChanges();
      });
      toggleIsCameraFlipped  = root.Q("Flipped") as Toggle;
      toggleIsCameraFlipped.SetValueWithoutNotify(CustomSettings.IsCameraFlipped);
      toggleIsCameraFlipped.RegisterValueChangedCallback(evt =>
      {
        CustomSettings.IsCameraFlipped = evt.newValue;
        poseSolution.NotifyChanges();
      });
      toggleSegmentationMask =  root.Q("Segmentation") as Toggle;
      toggleSegmentationMask?.SetValueWithoutNotify(CustomSettings.SegmentationMask);
      toggleSegmentationMask.RegisterValueChangedCallback(evt =>
      {
        CustomSettings.SegmentationMask = evt.newValue;
        poseSolution.NotifyChanges();
      });
      dropdownModelType  = root.Q("Model") as DropdownField;
      dropdownModelType.index = CustomSettings.ModelType;
      dropdownModelType.RegisterValueChangedCallback(evt =>
      {
        switch (evt.newValue)
        {
          case "Heavy":
            CustomSettings.ModelType = 2;
            break;
          case "Full":
            CustomSettings.ModelType = 1;
            break;
          default:
            CustomSettings.ModelType = 0;
            break;
        }
        poseSolution.NotifyChanges();
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

    private void CheckPossibleFallbacks()
    {
      // if the selected resolution was incompatible CameraResolutionIndex have changed when panels opens
      var selectedText = dropdownFieldCameraResolutionIndex.choices[CustomSettings.CameraResolutionIndex];
      dropdownFieldCameraResolutionIndex.SetValueWithoutNotify(selectedText);
    }

    private void UpdateString(Label label)
    {
      var key = label.text;
      var entry = stringTable.GetEntry(key);
      if (entry == null) return;
      label.text = entry.Value;
    }
    void OnExitPauseScreenClicked(ClickEvent evt)
    {
      Toggle();
    }
    void Toggle()
    {
      panel_hidden  = !panel_hidden;
      if (panel_hidden)
      {
        exitPauseScreen.style.height = new StyleLength(new Length(120f, LengthUnit.Pixel));
        exitPauseScreen.style.flexGrow = 0;
        exitPauseScreen.UnregisterCallback<ClickEvent>(OnExitPauseScreenClicked);
        screen.RemoveFromClassList("gui-paused-screen");
        left_panel.AddToClassList("gui-hidden-panel");
        button_image.AddToClassList("gui-button-image-open");
        button_image.RemoveFromClassList("gui-button-image-close");
        poseSolution.Resume();
      }
      else
      {
        exitPauseScreen.style.height = new StyleLength(StyleKeyword.Auto);
        exitPauseScreen.style.flexGrow = 1;
        exitPauseScreen.RegisterCallback<ClickEvent>(OnExitPauseScreenClicked);
        screen.AddToClassList("gui-paused-screen");
        left_panel.RemoveFromClassList("gui-hidden-panel");
        button_image.RemoveFromClassList("gui-button-image-open");
        button_image.AddToClassList("gui-button-image-close");
        poseSolution.Pause();
        CheckPossibleFallbacks();
      }
    }

    private void ClickOnScreen()
    {
      Debug.Log("click on screen");
      if (!panel_hidden)
      {
        Toggle();
      }
    }
    
    private int CameraResolutionTextToIndex(string text)
    {
      if (text.Contains("320x"))
        return 0;
      if (text.Contains("640x360"))
        return 1;
      if  (text.Contains("640x480"))
        return 2;
      if  (text.Contains("1280x"))
        return 3;
      if  (text.Contains("1920x"))
        return 4;
      return 0;
    }
    
}
