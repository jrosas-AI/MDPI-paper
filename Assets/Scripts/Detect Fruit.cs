using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using UnityEditor;
//using Unity.VisualScripting;

public class DetectFruit : MonoBehaviour
{

    public int sensor_number;

    //_JR_
    private bool raspberryIsAvailable = false;
    private int lastValue = 0;

    void Start()
    {
        
    }

    
    void Update()
    {
        //_JR_
        if(SystemModel.InputPin(39)==0) // raspberry is gone
        {
            raspberryIsAvailable = false;
;       }
        if( (raspberryIsAvailable == false)  && (SystemModel.InputPin(39) == 1))
        {
            raspberryIsAvailable=true;  // raspberry is ON again.
            SystemModel.OutputPin(sensor_number, lastValue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.tag == "Apple")
            {
                if (sensor_number == 3)
                {
                    SystemModel.OutputPin(3, 1);
                    lastValue = 1;
                }
                if (sensor_number == 6)
                {
                    SystemModel.OutputPin(6, 1);
                    lastValue = 1;
                }
                if (sensor_number == 10)
                {
                    SystemModel.OutputPin(10, 1);
                    lastValue = 1;
                }
                //print("on Trigger enter, me: " + tag + " other: " + other.gameObject.tag);
            }
            if(other.gameObject.tag == "Pear")
            {
                if (sensor_number == 3)
                {
                    SystemModel.OutputPin(3, 1);
                    lastValue = 1;
                }
                if (sensor_number == 9)
                {
                    SystemModel.OutputPin(9, 1);
                    lastValue = 1;
                }
                if (sensor_number == 10)
                {
                    SystemModel.OutputPin(10, 1);
                    lastValue = 1;
                }
                //print("on Trigger enter, me: " + tag + " other: " + other.gameObject.tag);
            }
            if (other.gameObject.tag == "Lemon")
            {
                if (sensor_number == 3)
                {
                    SystemModel.OutputPin(3, 1);
                    lastValue = 1;
                }
                if (sensor_number == 10)
                {
                    SystemModel.OutputPin(10, 1);
                    lastValue = 1;
                }
                //print("on Trigger enter, me: " + tag + " other: " + other.gameObject.tag);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            //print("on Trigger exit, me: " + tag + " other: " + other.gameObject.tag);

            if (other.gameObject.tag == "Apple")
            {
                if (sensor_number == 3)
                {
                    SystemModel.OutputPin(3, 0);
                    lastValue = 0;
                }
                if (sensor_number == 6)
                {
                    SystemModel.OutputPin(6, 0);
                    lastValue = 0;
                }
                if (sensor_number == 10)
                {
                    SystemModel.OutputPin(10, 0);
                    lastValue = 0;
                }
            }
            if (other.gameObject.tag == "Pear")
            {
                if (sensor_number == 3)
                {
                    SystemModel.OutputPin(3, 0);
                    lastValue = 0;
                }
                if (sensor_number == 9)
                {
                    SystemModel.OutputPin(9, 0);
                    lastValue = 0;
                }
                if (sensor_number == 10)
                {
                    SystemModel.OutputPin(10, 0);
                    lastValue = 0;
                }
            }
            if (other.gameObject.tag == "Lemon")
            {
                if (sensor_number == 3)
                {
                    SystemModel.OutputPin(3, 0);
                    lastValue = 0;
                }
                if (sensor_number == 10)
                {
                    SystemModel.OutputPin(10, 0);
                    lastValue = 0;
                }
            }
        }
    }
}
