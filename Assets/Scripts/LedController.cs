using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LedController : MonoBehaviour
{
    public Boolean state;
    private Boolean stateBefore;

    public int pin=2;
    public Sprite ledOn;
    public Sprite ledOff;
    private Image currentImage;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    { 

        if(SystemModel.InputPin(pin) ==1)
        {
            state = true;
        } else
        {
            state = false;
        }


        if(state && !stateBefore)
        {
            stateBefore = state;
            currentImage.sprite = ledOn;
        }
        if (!state && stateBefore)
        {
            stateBefore = state;
            currentImage.sprite = ledOff;
        }
    }
}
