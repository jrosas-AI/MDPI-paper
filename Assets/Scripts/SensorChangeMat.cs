using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using UnityEditor;
//using Unity.VisualScripting;

using TMPro;

public class SensorChangeMat : MonoBehaviour
{
    
    public int pinNumber;
    public Material active_material;
    public Material inactive_material;
    public Material clickedMaterial;
    public string description;

    public Material currentStateMaterial;
    public GameObject panelHeadHupDisplay = null; 
    public TextMeshProUGUI textInfo = null;

    public bool isClickingTheSensor = false;
    public bool InfoControl = false;

    // Start is called before the first frame update
    void Start()
    {
        textInfo = GameObject.Find("Canvas/headUpDisplay/textInfo").GetComponent<TextMeshProUGUI>();    
        panelHeadHupDisplay = GameObject.Find("Canvas/headUpDisplay");
        panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isClickingTheSensor)
        {
            if (pinNumber == 1 || pinNumber == 2 || pinNumber == 3 || pinNumber == 4 || pinNumber == 5 || pinNumber == 6 || pinNumber == 7 || pinNumber == 8 || pinNumber == 9 || pinNumber == 10)
            {
                if (SystemModel.InputPin(pinNumber) == 1)
                    GetComponent<MeshRenderer>().material = active_material;
                else
                    GetComponent<MeshRenderer>().material = inactive_material;
                currentStateMaterial = GetComponent<MeshRenderer>().material;

            }
        }
    }

    public void OnMouseDown()
    {
        isClickingTheSensor = true;
        if (InfoControl)
            InfoControl = false;
        else
            InfoControl = true;
        Debug.Log("Clicked on " + gameObject.name);
        if (InfoControl)
        {
            for(int i = 1; i <= 10; i++)
            {
                if (i == pinNumber)
                {
                    i++;
                    if (i == 11)
                        break;
                }
                string s = i.ToString();
                GameObject sensor = GameObject.FindWithTag(s);
                SensorChangeMat sensor_script = sensor.GetComponent<SensorChangeMat>();
                if (sensor_script.isClickingTheSensor) {
                    sensor_script.isClickingTheSensor = false;
                    sensor_script.textInfo.text = "";
                    sensor_script.panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
                    sensor_script.InfoControl = false;

                }

            }
            currentStateMaterial = GetComponent<MeshRenderer>().material;
            GetComponent<MeshRenderer>().material = clickedMaterial;
            textInfo.text = description;
            panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }
        else
        {
            isClickingTheSensor = false;
            textInfo.text = "";
            panelHeadHupDisplay.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        }
    }
}
