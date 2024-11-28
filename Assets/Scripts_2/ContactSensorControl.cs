using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactSensorControl : MonoBehaviour
{
    // Start is called before the first frame update
    public string channelToActivate="I4";
       
    public string movingObject = "";
    void Start()
    {
        SystemModel.setVariableValue(channelToActivate, "0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
         print("ON_COLLISION_Enter: MY_name: " + name + "OTHER_name: " + collision.gameObject.name + " channel:"+ channelToActivate);
        if(collision.gameObject.name == movingObject)
        {
            SystemModel.setVariableValue(channelToActivate, "1");
        }
    }

    

    private void OnCollisionExit(Collision collision)
    {
        print("ON_COLLISION_Exit: MY_name: " + name + "OTHER_name: " + collision.gameObject.name + " channel:" + channelToActivate);
        if (collision.gameObject.name == movingObject)
        {
            SystemModel.setVariableValue(channelToActivate, "0");
        }
    }

}
