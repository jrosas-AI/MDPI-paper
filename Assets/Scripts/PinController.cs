using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PinController : MonoBehaviour
{    
    public int pinNumber = 2;    
    public TextMeshProUGUI labelTMPType = null;
    public TextMeshProUGUI labelTMPValue = null;

    private void Update()
    {        
        if(labelTMPValue != null)
        {
            int pinValue = SystemModel.InputPin(pinNumber);
            labelTMPValue.text = (pinValue == 0 ? "LOW" : "HIGH");
        }
    }

    public void SetPinTypeButton() 
    {
        /*
        if (labelTMPType != null)
        {
            labelTMPType.text = (labelTMPType.text == "INPUT" ? "OUTPUT" : "INPUT");
        }
        */
    }

    public void SetPinValueButton()
    {
        //print("pin Number:" + pinNumber);
        //labelTMPValue = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (labelTMPValue != null)
        {
            labelTMPValue.text = (labelTMPValue.text == "LOW" ? "HIGH" : "LOW");
            int value = labelTMPValue.text == "LOW" ? 0 : 1;

            //setPinValue(pinNumber, value); 
            SystemModel.OutputPin(pinNumber, value);
        }
    }
}
