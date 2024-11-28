using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;

[Serializable]
public class VariableStates
{
    public List<VariableStateEntry> variableStates;

    public Dictionary<string, string> ToDictionary()
    {
        return variableStates.ToDictionary(entry => entry.variableName, entry => entry.value);
    }
}

[Serializable]
public class VariableStateEntry
{
    public string variableName;
    public string value;
}

public class MyWebClient : MonoBehaviour
{
    private static MyWebClient _instance;

    private const string serverAddress = "http://127.0.0.1";
    private const int serverPort = 8089;

    private bool getBitValuesSemaphore = false;

    private Dictionary<string, string> variableStates = new Dictionary<string, string>();

    public static MyWebClient Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MyWebClient>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MyWebClient).Name);
                    _instance = singletonObject.AddComponent<MyWebClient>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this GameObject
            Destroy(gameObject);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        getVariablesValues(textReceivedHandler);
#endif
    }

    public int input(int GPIO_ID)
    {
        return 0;
    }

    public String getVariableValue(String variableName)
    {
        if (variableStates.ContainsKey(variableName))
        {
            return variableStates[variableName];
        }
        return "";
    }

    public void setVariableValue(String variableName, String value)
    {
        string url = serverAddress + ":" + serverPort + "/setVariable";
        StartCoroutine(SendPOSTRequest(url, "{\"variableName\":\"" + variableName + "\",\"value\":\"" + value + "\"}"));
    }

    // Method to call the /newPinFromSimulator endpoint with POST data
    public void PostPinValue(int GPIO_ID, int Value)
    {
        string url = serverAddress + ":" + serverPort + "/newPinFromSimulator";
        StartCoroutine(SendPOSTRequest(url, "{\"GPIO_ID\":" + GPIO_ID + ",\"Value\":" + Value + "}"));
    }

    // Coroutine to send a POST request to the specified URL with JSON data
    private IEnumerator SendPOSTRequest(string url, string json)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bytes);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            textReceivedHandler(responseText);
        }
    }

    // Your textReceivedHandler method
    public void textReceivedHandler(string text)
    {
        variableStates = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
    }

    // Coroutine to send a GET request to the specified URL
    private IEnumerator SendGETRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    public void getVariablesValues(Action<string> textReceivedHandler)
    {
        if (getBitValuesSemaphore)
        {
            return;
        }
        getBitValuesSemaphore = true;

        string url = serverAddress + ":" + serverPort + "/getVariables";
        StartCoroutine(getBitValuesRequest(url, textReceivedHandler));
    }

    private IEnumerator getBitValuesRequest(string url, Action<string> textReceivedHandler)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            getBitValuesSemaphore = false;
            Debug.LogError("Error: " + request.error);
            textReceivedHandler?.Invoke(null);
        }
        else
        {
            getBitValuesSemaphore = false;
            string responseText = request.downloadHandler.text;
            textReceivedHandler?.Invoke(responseText);
        }
    }
}
