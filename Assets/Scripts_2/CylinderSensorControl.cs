using System.Drawing;
using UnityEngine;

public class CylinderSensorControl : MonoBehaviour
{
    public int cameraNumber = 1;
    public string fruitClass = "";

    public string fruitTargetClass = "";

    private bool fruitHere = false;



    private void Update()
    {

        //string fruitIdentifiedName = "fruitIdentifiedName_" + cameraNumber;
        //SystemModel.setVariableValue(fruitIdentifiedName, "");


        //string fruitIdentifiedName = "fruitIdentifiedName_" + cameraNumber;
        //string fruitName = SystemModel.getVariableValue(fruitIdentifiedName);
        if (!fruitHere) {
            return;
        }

        fruitClass = SystemModel.getVariableValue("camera_" + cameraNumber + "_fruitType");

        if (fruitClass == fruitTargetClass)
        {
            SystemModel.setVariableValue("I2"+cameraNumber, "1");
            print(fruitClass);
        }        
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "fruit")
        {
            fruitHere = true;

            // Get the contact point coordinates from the position of the colliding object
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            //float x = contactPoint.x;
            //float y = contactPoint.y;
            //float z = contactPoint.z;

            // Debug.Log($"Contact Point: x = {x}, y = {y}, z = {z}");
            CanvasScript.instance.cameraCapture(cameraNumber, contactPoint);
                        
            SystemModel.setVariableValue("I"+ cameraNumber, "1");

            //string fruitIdentifiedName = "fruitIdentifiedName_" + cameraNumber;
            //SystemModel.setVariableValue(fruitIdentifiedName, "");

            string inputFruitIdentified = "I2" + cameraNumber;
            SystemModel.setVariableValue(inputFruitIdentified, "0");

        }
    }


    

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "fruit")
        {
            string inputFruitHere = "I" + cameraNumber;
            //string inputFruitIdentified = "I2" + cameraNumber;
            //string fruitIdentifiedName = "fruitIdentifiedName_" + cameraNumber;

            fruitHere = false;

            SystemModel.setVariableValue(inputFruitHere, "0");
            SystemModel.setVariableValue("camera_" + cameraNumber + "_fruitType", "");
            //SystemModel.setVariableValue(inputFruitIdentified, "0");
            //SystemModel.setVariableValue(fruitIdentifiedName, "");
        }
        
    }
    
}
