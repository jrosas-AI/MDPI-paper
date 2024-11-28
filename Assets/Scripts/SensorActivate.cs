using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorActivate : MonoBehaviour
{
    public int sensor_number;
    public Material other_material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer my_renderer = GetComponent<MeshRenderer>();
        if (my_renderer != null)
        {
            Material my_material = my_renderer.material;
        }
    }
}
