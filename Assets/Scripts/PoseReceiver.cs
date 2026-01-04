using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using UnityEngine;

public static class PoseReceiver
{
  private static PoseLandmarkerResult currentResult;
  private static int currentTimeStampSecond=0;
  private static int resultsPerSecond=0;
  private static int FPS;
  public static void ReceiveResult(PoseLandmarkerResult result)
  {
    if (result.Equals(currentResult))
    {
      Debug.LogError("Pose landmarker result is already in use");
    }
    var newCurrentSystemSecond = DateTime.Now.Second;
    if (newCurrentSystemSecond != currentTimeStampSecond)
    {
      FPS = resultsPerSecond;
      resultsPerSecond = 0;
      currentTimeStampSecond = newCurrentSystemSecond;
    }
    else
    {
      resultsPerSecond++;
    }
  }

  public static int GetFPS()
  {
    return FPS;
  }
}
