using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using UnityEditor;
//using Unity.VisualScripting;

public class ConveyorDriver : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string checkMoveLeft();

    [DllImport("__Internal")]
    private static extern string checkMoveRight();

    [DllImport("__Internal")]
    private static extern void callIOChannels();


    
    private Rigidbody rb;

    


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {

        float speed = SliderControl.value;


        if (SystemModel.InputPin(2) == 0 && SystemModel.InputPin(3) == 0)
        {
            //System.GC.Collect();
            rb.linearVelocity = Vector3.zero; // Stop the object if no movement input
        }

        if (SystemModel.InputPin(3) == 1 ) 
        {
            rb.AddForce(Vector3.right * Time.deltaTime * speed, ForceMode.VelocityChange);
            print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }

        if (SystemModel.InputPin(2) == 1 )
        {
            rb.AddForce(Vector3.left * Time.deltaTime * speed, ForceMode.VelocityChange);
            print("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
        }

        /*
        if (Input.GetKey(KeyCode.RightArrow) || MoveRight)
        {
            transform.position += new Vector3(+1 * Time.deltaTime, 0, 0);  //Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || MoveLeft)
        {
            transform.position += new Vector3(-1 * Time.deltaTime, 0, 0);  //Time.deltaTime;
        }
        */

    }


    //_JR_
    /*
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            int minSize = Math.Min("Basket_02".Length, collision.gameObject.name.Length);
            if (collision.gameObject.name.Substring(0, minSize) == "Basket_02")
            {
                print("on Collision works");
                collision.gameObject.transform.parent = transform;
            }
        }
    }


    
    private void OnTriggerEnter(Collider other) 
    {
        if (other != null)
        {
            print("on Trigger enter, me: " + tag + " other: " +  other.gameObject.tag);

            if(other.gameObject.tag == "sensor_left")
            {
                GPIO.OutputPin(4, 1);
            }
            if (other.gameObject.tag == "sensor_right")
            {
                GPIO.OutputPin(5, 1);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            print("on Trigger exit, me: " + tag + " other: " + other.gameObject.tag);

            if (other.gameObject.tag == "sensor_left")
            {
                GPIO.OutputPin(4, 0);
            }
            if (other.gameObject.tag == "sensor_right")
            {
                GPIO.OutputPin(5, 0);
            }
        }
    }

    */
}
