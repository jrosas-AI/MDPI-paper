using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinaryComponentControl : MonoBehaviour
{
    public Boolean state;
    private Boolean stateBefore;

    public int pin = -1;
    public Sprite spritOn;
    public Sprite spriteOff;
    private Image currentImage;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pin != -1)
        {
            if (SystemModel.InputPin(pin) == 1)
            {
                state = true;
            }
            else
            {
                state = false;
            }
        }


        if (state && !stateBefore)
        {
            stateBefore = state;
            currentImage.sprite = spritOn;
        }
        if (!state && stateBefore)
        {
            stateBefore = state;
            currentImage.sprite = spriteOff;
        }
    }

    public void onClickStateSwitch()
    {
        state = !state;
    }

}
