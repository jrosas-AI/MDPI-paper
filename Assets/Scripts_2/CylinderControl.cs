using UnityEngine;

public class CylinderControl : MonoBehaviour
{
    public Rigidbody rbPiston;
    public float velocity = 1;

    public string commandVariable = "Q0";
    public string atRestSensor = "I4";
    public string atWorkSensor = "I5";

    void Start()
    {
        // Ensure the Rigidbody is not using gravity (optional, depending on your needs)
        // rbPiston.useGravity = false;
        //SystemModel.setVariableValue(commandVariable, "1");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 desiredVelocity = Vector3.zero;

        if (SystemModel.getVariableValue(commandVariable) == "1")
        {
            if (SystemModel.getVariableValue(atWorkSensor) != "1")
                desiredVelocity = Vector3.forward * velocity;
            else
                desiredVelocity = Vector3.zero;
        }

        else if (SystemModel.getVariableValue(commandVariable) == "0")
        {
            if (SystemModel.getVariableValue(atRestSensor) != "1")
                desiredVelocity = Vector3.back * velocity;
            else
                desiredVelocity = Vector3.zero;            
        }

        rbPiston.linearVelocity = desiredVelocity;

        if (desiredVelocity == Vector3.zero)
        {
            rbPiston.angularVelocity = Vector3.zero; // Optionally, stop any angular motion
            //print("zero");
        }
    }


}
