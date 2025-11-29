using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseSolution : MonoBehaviour
{
  void Awake()
  {
    transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
  }
}
