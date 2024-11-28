using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MotorActuator : MonoBehaviour
{
    // Set rotate to equal false
    public bool rotate = false;

    // List of Rotatable Objects
    public List<GameObject> rotatableObjects;

    // Object Rotation Speed
    public float rotationSpeed = 20;


    public int pinForward = 0;
    public int pinBackward = 0;
    public float forwardDirection = 1.0f;


    public string description;

    private GameObject panelHeadHupDisplay = null;
    private TextMeshProUGUI textInfo = null;

    public bool InfoControl = false;


    private void Start()
    {
        // Set rotate to false at start. I did this because I use a button to start the rotation.
        // Set rotate to true at start, if you want it to rotate on start.
        rotate = false;

        textInfo = GameObject.Find("Canvas/headUpDisplay/textInfo").GetComponent<TextMeshProUGUI>();
        panelHeadHupDisplay = GameObject.Find("Canvas/headUpDisplay");
        panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(0.0f);

    }

    private void Update()
    {
        if (pinForward >= 0 || pinBackward>=0)
        {
            rotate = false;
        }
        if (pinForward >= 0)
        {            
            if (SystemModel.InputPin(pinForward) == 1)
            {
                rotate = true;
                rotationSpeed = (forwardDirection)*Mathf.Abs(rotationSpeed);
            }
        }
        if(pinBackward>=0)
        {
            if (SystemModel.InputPin(pinBackward) == 1)
            {
                rotate = true;
                rotationSpeed = (-1.0f)* (forwardDirection) * Mathf.Abs(rotationSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
        // If Not rotate
        if (!rotate)
        {
            // Return
            return;
        }
        else
        {
            // Foreach rotatableObject of type GameObject in rotatableObjects
            foreach (GameObject rotatableObject in rotatableObjects)
            {
                // Rotate the rotatableObject 
                // Change Vector3.up if it's not the desired rotation. I had it originally set to Vector3.forward.
                rotatableObject.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }
        }
    }
    // Used to make the object rotate on button press.
    public void RotateObject()
    {
        rotate = !rotate;
        return;
    }
    // Used to make the object rotate on button press and play animation.
    public void RotateObject(bool enable)
    {
        rotate = enable;
    }


    public void OnMouseDown()
    {
        if (InfoControl)
            InfoControl = false;
        else
            InfoControl = true;
        if (InfoControl)
        {
            for (int i = 1; i <= 10; i++)
            {
                string s = i.ToString();
                GameObject sensor = GameObject.FindWithTag(s);
                SensorChangeMat sensor_script = sensor.GetComponent<SensorChangeMat>();
                if (sensor_script.isClickingTheSensor)
                {
                    sensor_script.isClickingTheSensor = false;
                    sensor_script.textInfo.text = "";
                    sensor_script.panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
                    sensor_script.InfoControl = false;

                }

            }
            textInfo.text = description;
            panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }
        else
        {
            textInfo.text = "";
            panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        }

    }
}
