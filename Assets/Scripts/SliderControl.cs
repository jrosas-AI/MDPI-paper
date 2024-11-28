using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    static public float value=0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        value = slider.value;
    }
}
