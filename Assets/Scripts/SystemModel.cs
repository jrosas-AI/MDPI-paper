using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SystemModel : MonoBehaviour
{

#if !UNITY_EDITOR
#if UNITY_WEBGL
    static private bool javaScriptLoaded = false;
#endif
#endif

    [DllImport("__Internal")]
    private static extern void startReceivingSystemValues();

    [DllImport("__Internal")]
    private static extern int output(int GPIO_ID, int Value);


    [DllImport("__Internal")]
    private static extern int input(int GPIO_ID);

    [DllImport("__Internal")]
    private static extern void jsSetVariableValue(String variableName, String value);

    [DllImport("__Internal")]
    private static extern String jsGetVariableValue(String variableName);


    public static void OutputPin(int gpio_id, int value)
    {
#if UNITY_EDITOR
        MyWebClient.Instance.PostPinValue(gpio_id, value);
#elif UNITY_WEBGL
        if(!javaScriptLoaded) {
            javaScriptLoaded = true;
            startReceivingSystemValues();
        }
        output(gpio_id, value);
#endif

    }

    public static int InputPin(int gpio_id)
    {

#if UNITY_EDITOR
        return MyWebClient.Instance.input(gpio_id);

#elif UNITY_WEBGL
        if(!javaScriptLoaded) {
            javaScriptLoaded = true;
            startReceivingSystemValues();
        }
        return input(gpio_id);
#endif


    }


    public static String getVariableValue(string variableName) 
    {

#if UNITY_EDITOR
        return MyWebClient.Instance.getVariableValue(variableName);

#elif UNITY_WEBGL
        if(!javaScriptLoaded) {
            javaScriptLoaded = true;
            startReceivingSystemValues();
        }
        return getVariableValue(variableName);
#endif
    }


    public static void setVariableValue(string variableName, string value)
    {
#if UNITY_EDITOR
        MyWebClient.Instance.setVariableValue(variableName, value);
#elif UNITY_WEBGL
        if(!javaScriptLoaded) {
            javaScriptLoaded = true;
            startReceivingSystemValues();
        }
        output(gpio_id, value);
#endif

    }

}
