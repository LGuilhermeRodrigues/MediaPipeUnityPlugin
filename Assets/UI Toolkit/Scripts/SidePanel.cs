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
    private Toggle toggleShowFPS;
    private DropdownField dropdownModelType;
    private Button mask_color_button;
    private VisualElement color_grid_panel;
    private Label labelFPS;

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
        poseSolution.NotifyChanges(true);
      });
      dropdownFieldCameraResolutionIndex = root.Q("DropdownResolution") as DropdownField;
      dropdownFieldCameraResolutionIndex.RegisterValueChangedCallback(evt =>
      {
        CustomSettings.CameraResolutionIndex = CameraResolutionTextToIndex(evt.newValue);
        poseSolution.NotifyChanges(true);
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
      dropdownModelType.choices.Clear();
      var newList = new List<string>();
      newList.Add("Leve");
      newList.Add("Médio");
      newList.Add("Pesado");
      dropdownModelType.choices.AddRange(newList);
      dropdownModelType.index = CustomSettings.ModelType;
      dropdownModelType.RegisterValueChangedCallback(evt =>
      {
        switch (evt.newValue)
        {
          case "Heavy":
          case "Pesado":
            CustomSettings.ModelType = 2;
            break;
          case "Full":
          case "Médio":
            CustomSettings.ModelType = 1;
            break;
          default:
            CustomSettings.ModelType = 0;
            break;
        }
        poseSolution.NotifyChanges();
      });
      mask_color_button = root.Q("SelectedColor") as Button;
      mask_color_button.style.backgroundColor = new StyleColor(CustomSettings.MaskColor);
      color_grid_panel = root.Q("color_grid_panel");
      mask_color_button?.RegisterCallback<ClickEvent>(evt => OpenColorGrid());
      CreateColorGrid(root);
      toggleShowFPS  = root.Q("FPS") as Toggle;
      toggleShowFPS.SetValueWithoutNotify(CustomSettings.ShowFPS);
      toggleShowFPS.RegisterValueChangedCallback(evt =>
      {
        CustomSettings.ShowFPS = evt.newValue;
      });
      labelFPS = root.Q<Label>("LabelFPS");
      UpdateFPSLabel();
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
    void UpdateFPSLabel()
    {
      var showFPS = CustomSettings.ShowFPS;
      labelFPS.style.display = showFPS ? DisplayStyle.Flex : DisplayStyle.None;
      labelFPS.text = "FPS: 0";
      if (showFPS)
      {
        InvokeRepeating(nameof(UpdateFPSLabelEverySecond), 1f, 1f);
      }
      else
      {
        CancelInvoke(nameof(UpdateFPSLabelEverySecond));
      }
    }
    void UpdateFPSLabelEverySecond()
    {
      labelFPS.text = "FPS: "+PoseReceiver.GetFPS();
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
        labelFPS.style.display = DisplayStyle.None;
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
      UpdateFPSLabel();
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
    
    void OpenColorGrid()
    {
      color_grid_panel.style.display = DisplayStyle.Flex;
      mask_color_button.style.display = DisplayStyle.None;
    }

    void OnColorSelected(Color c)
    {
      mask_color_button.style.display = DisplayStyle.Flex;
      mask_color_button.style.backgroundColor = new StyleColor(c);
      CustomSettings.MaskColor = c;
      color_grid_panel.style.display = DisplayStyle.None;
      poseSolution.NotifyChanges();
    }
    
    void AddCell(VisualElement grid,Color c)
    {
      var cell = new Button();
      cell.style.width = 30;
      cell.style.height = 30;
      cell.style.marginRight = 2;
      cell.style.marginBottom = 2;
      cell.style.backgroundColor = new StyleColor(c);
      cell.clicked += () => OnColorSelected(c);
      grid.Add(cell);
    }
    
    public void CreateColorGrid(VisualElement rootVisualElement)
    {
        var grid = rootVisualElement.Q("color_grid") as VisualElement;
        grid.style.flexDirection = FlexDirection.Row;
        grid.style.flexWrap = Wrap.Wrap;
        grid.style.justifyContent = Justify.FlexStart;
        grid.style.width = (30 + 6) * 12;

        // Generate 12 base hues evenly around the wheel.
        Color[] baseColors = new Color[12];
        float startHue = 0.66f; // start near blue
        for (int i = 0; i < 12; i++)
        {
            float h = (startHue + (i / 12f)) % 1f; // step around hue wheel
            baseColors[i] = Color.HSVToRGB(h, 1f, 1f);
        }

        // 1) Grayscale top row (12 columns)
        for (int col = 0; col < 12; col++)
        {
            float gray = col / 11f; // 0..1
            var grayColor = new Color(gray, gray, gray, 1f);
            AddCell(grid, grayColor);
        }

        // 2) 9 tone rows (total rows = 1 gray + 9 = 10)
        // We want row index 0..8 (9 rows). The middle row index (4) should be the base color.
        int toneRows = 9;
        int middleIndex = toneRows / 2; // 4 for 9 rows

        for (int row = 0; row < (toneRows - 1); row++)
        {
            foreach (Color baseColor in baseColors)
            {
                // Convert base color to HSV
                Color.RGBToHSV(baseColor, out float h, out float sBase, out float vBase);

                Color tone;
                if (row == middleIndex)
                {
                    // Middle row = exact base color
                    tone = baseColor;
                }
                else if (row < middleIndex)
                {
                    // Darker steps above middle: interpolate value from dark -> base (keep saturation high)
                    float t = row / (float)middleIndex; // 0..1 where 0 = darkest, 1 -> reach base at middle
                    float v = Mathf.Lerp(0.12f, vBase, t);    // dark to base brightness
                    float s = 1f;                             // keep saturated in darks
                    tone = Color.HSVToRGB(h, s, v);
                }
                else
                {
                    // Lighter steps below middle: desaturate and blend toward white
                    float t = (row - middleIndex) / (float)(toneRows - 1 - middleIndex); // 0..1 where 0 -> base, 1 -> lightest
                    // Slightly reduce value (or keep near 1) and reduce saturation
                    float v = Mathf.Lerp(vBase, 0.98f, t);       // keep value high for tints
                    float s = Mathf.Lerp(1f, 0.18f, t);          // become less saturated toward white
                    tone = Color.HSVToRGB(h, s, v);
                    // Blend more toward white visually
                    tone = Color.Lerp(tone, Color.white, t * 0.9f);
                }

                AddCell(grid, tone);
            }
        }
    }
}
