using UnityEngine;
using System.Collections.Generic;

public class ConveyorBeltControl : MonoBehaviour
{
    public GameObject stripPrefab;
    public GameObject stripRedPrefab;
    public float stripLength = 0.01f; // Length of each strip in the conveyor
    public float nominalSpeed = 0.05f; // Nominal speed of the conveyor

    public float speed = 0;

    public float referenceSpeed = 0.1f;
    public float sensorSpeed = 0f;

    public float gap = 0.002f; // Gap between strips
    public bool isMoving = true;
    public float weight = 0f; // Weight of the load on the conveyor
    public float motorPower = 5.0f; // Motor power in kilowatts

    private List<GameObject> strips = new List<GameObject>();
    private float conveyorLength;
    private float halfConveyorLength;
    private float stripHeightOffset = 0.5f; // Offset to place strips above the conveyor

    void Start()
    {
        conveyorLength = 1f;  // Length in local coordinates
        halfConveyorLength = conveyorLength / 2f;
        InitializeStrips();
    }

    void FixedUpdate()
    {
        if (SystemModel.getVariableValue("Q5") == "1")
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Calculate the speed based on the weight
        speed = CalculateSpeed(weight);

        if (isMoving)
        {
            MoveStrips();
        }
    }

    void InitializeStrips()
    {
        float currentPosition = -halfConveyorLength + stripLength / 2f;
        bool isFirstStripRed = true;
        while (currentPosition < halfConveyorLength)
        {
            CreateStripAtPosition(currentPosition, isFirstStripRed);
            currentPosition += stripLength + gap; // Add gap between strips
            isFirstStripRed = false;
        }
    }

    void CreateStripAtPosition(float position, bool isRed = false)
    {
        GameObject strip;
        if (isRed)
        {
            strip = Instantiate(stripRedPrefab, transform);
            strip.tag = "RedStrip";
        }
        else
        {
            strip = Instantiate(stripPrefab, transform);
        }

        strip.transform.localPosition = new Vector3(position, stripHeightOffset, 0); // Use localPosition for relative positioning and set Y position above the conveyor
        strip.transform.localScale = new Vector3(stripLength, transform.localScale.y, transform.localScale.z);
        strips.Add(strip);
    }

    void MoveStrips()
    {
        float moveDistance = speed * Time.deltaTime;

        // Move all strips to the left
        for (int i = strips.Count - 1; i >= 0; i--)
        {
            strips[i].transform.localPosition += Vector3.left * moveDistance;

            // Check if strip is out of bounds and needs to be repositioned
            if (strips[i].transform.localPosition.x < -halfConveyorLength)
            {
                // Reparent children of the strip to the conveyor before destroying the strip
                ReparentChildren(strips[i]);

                // Determine if the strip is red
                bool isRed = strips[i].CompareTag("RedStrip");

                // Destroy the strip and remove it from the list
                Destroy(strips[i]);
                strips.RemoveAt(i);

                // Create a new strip at the end of the conveyor
                CreateStripAtPosition(halfConveyorLength + stripLength / 2f + gap / 2f, isRed); // Adjust position for gap
            }
        }
    }

    void ReparentChildren(GameObject strip)
    {
        // Reparent all children of the strip to the conveyor
        while (strip.transform.childCount > 0)
        {
            print("childCount:" + strip.transform.childCount);
            Transform child = strip.transform.GetChild(0);
            child.SetParent(transform);
        }
    }

    float CalculateSpeed(float weight)
    {
        // Assuming linear relation between weight and speed for simplicity
        // You can use more complex equations or models based on your requirements

        SystemModel.setVariableValue("referenceSpeed", "" + referenceSpeed);
      

        string powerValueString = SystemModel.getVariableValue("motorPower");
        float.TryParse(powerValueString, out motorPower);

        float currentSpeed =0;
        string speedValueString = SystemModel.getVariableValue("speed");
        float.TryParse(powerValueString, out currentSpeed);


        return currentSpeed;
    }
}
