using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using UnityEditor;
//using Unity.VisualScripting;

public class Atuador_Mov : MonoBehaviour
{
    /*
    [DllImport("__Internal")]
    private static extern string checkMoveLeft();

    [DllImport("__Internal")]
    private static extern string checkMoveRight();

    [DllImport("__Internal")]
    private static extern void callIOChannels();
    */

    
    private Rigidbody rb;
    //private Boolean control1 = false;
    //private Boolean control2 = false;

    //_JR_
    private int lasValuePin_1 = 0;
    private int lasValuePin_2 = 0;
    private bool raspberryIsAvailable = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //_JR_
        if (SystemModel.InputPin(39) == 0) // raspberry is gone
        {
            raspberryIsAvailable = false;            
        }
        if ((raspberryIsAvailable == false) && (SystemModel.InputPin(39) == 1))
        {
            raspberryIsAvailable = true;  // raspberry is ON again.
            SystemModel.OutputPin(1, lasValuePin_1);
            SystemModel.OutputPin(2, lasValuePin_2);
        }
    }


    private void FixedUpdate()
    {

        float speed = SliderControl.value;
        /*
        if (GPIO.InputPin(11) == 0)
            control1 = false;

        if (GPIO.InputPin(12) == 0)
            control2 = false;
        */
        //_JR_
        Vector3 desiredVelocity = Vector3.zero;

        if (SystemModel.InputPin(11) == 1)
        {
            desiredVelocity += Vector3.right;
        }

        if (SystemModel.InputPin(12) == 1)
        {
            desiredVelocity += Vector3.left;
        }

        rb.linearVelocity = speed * Time.deltaTime *desiredVelocity;


        /*
        if (GPIO.InputPin(11) == 0 && GPIO.InputPin(12) == 0)
        {
            rb.velocity = Vector3.zero;
            //print("Stop_1");
        }

        if (GPIO.InputPin(11) == 1)// && !control1) 
        {
            //rb.AddForce(Vector3.right * Time.deltaTime * speed, ForceMode.VelocityChange);
            rb.velocity = Vector3.right * Time.deltaTime * speed;
            //print("Forward_1");
            control1 = true;
        }

        if (GPIO.InputPin(12) == 1)// && !control2)
        {
            //rb.AddForce(Vector3.left * Time.deltaTime * speed, ForceMode.VelocityChange);
            rb.velocity = Vector3.left * Time.deltaTime * speed;
            //print("Recuar_1");
            control2 = true;
        }
        */
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other != null)
        {
            

            if(other.gameObject.tag == "1")
            {
                SystemModel.OutputPin(1, 1);
                lasValuePin_1 = 1;
                //print("on Trigger enter, me: " + tag + " other: " + other.gameObject.tag);
            }
            if (other.gameObject.tag == "2")
            {
                SystemModel.OutputPin(2, 1);
                lasValuePin_2 = 1;
                //print("on Trigger enter, me: " + tag + " other: " + other.gameObject.tag);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            //print("on Trigger exit, me: " + tag + " other: " + other.gameObject.tag);

            if (other.gameObject.tag == "1")
            {
                SystemModel.OutputPin(1, 0);
                lasValuePin_1 = 0;
            }
            if (other.gameObject.tag == "2")
            {
                SystemModel.OutputPin(2, 0);
                lasValuePin_2 = 0;
            }
        }
    }

    
}
