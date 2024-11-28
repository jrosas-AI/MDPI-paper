using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CanvasScript : MonoBehaviour
{
    public Camera topCamera;
    public Camera camera_1;
    public Camera camera_2;
    public Camera camera_3;

    public Transform fruitCreationTransform;
    public List<GameObject> fruitPrefabs;
    public List<GameObject> applePrefabs;
    public List<GameObject> orangePrefabs;
    public List<GameObject> pearPrefabs;


    public float capturePeriod = 0.5f; // 1 second
    public static CanvasScript instance;

    public float putFruitPeriod = 2.0f;


    private string Q4_value_before = "";

    static private int putFruitMutex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //SystemModel.setVariableValue("Q5", "1");
        StartCoroutine(CapturePeriodically());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraCapture(1, null);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(PutFruits());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            putOneFruit(); // any fruit
        }

        if (Input.GetKeyDown(KeyCode.A))
        { 
            putOneFruit(0); // apple
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            putOneFruit(1); //oramge
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            putOneFruit(2); //pear
        }

        string Q4_valueNow = SystemModel.getVariableValue("Q4");
        if (Q4_valueNow == "1" && Q4_value_before != Q4_valueNow)
        {
            Q4_value_before = Q4_valueNow;
            //StartCoroutine(tryPutFruitAfterDelay());
            putOneFruit();
        }
        if (Q4_valueNow == "0")
        {
            Q4_value_before = "0";
        }

        // print("q4now=" + Q4_valueNow + " q4before=" + Q4_value_before);
    }


    private IEnumerator tryPutFruitAfterDelay()
    {
        if (putFruitMutex == 0)
        {
            putFruitMutex = 1;
            yield return new WaitForSeconds(putFruitPeriod);
            if (SystemModel.getVariableValue("Q4") == "1")
            {
                putOneFruit();
            }
            putFruitMutex = 0;
        }
    }

    private void putOneFruit(int whichSetParam = -1)
    {
        
        List<GameObject>[] fruitSets = { applePrefabs, orangePrefabs, pearPrefabs };
        int whichSet = UnityEngine.Random.Range(0, fruitSets.Length);        
        whichSet = (whichSetParam != -1) ? whichSetParam : whichSet;
        // whichSet = 2;
        int whichFruit = UnityEngine.Random.Range(0, fruitSets[whichSet].Count); // Gerar um índice aleatório       
        GameObject newFruit = Instantiate(fruitSets[whichSet][whichFruit]); // Instanciar a fruta aleatória
        
        /*
        int whichFruit = UnityEngine.Random.Range(0, fruitPrefabs.Count);
        //whichFruit = 2;
        GameObject newFruit = Instantiate(fruitPrefabs[whichFruit]);
        */

        newFruit.transform.position = fruitCreationTransform.position;

        float randomXRotation = UnityEngine.Random.Range(0f, 360f);
        float randomYRotation = UnityEngine.Random.Range(0f, 360f);
        float randomZRotation = UnityEngine.Random.Range(0f, 360f);        
        newFruit.transform.rotation = Quaternion.Euler(randomXRotation, randomYRotation, randomZRotation);
        float newScale = UnityEngine.Random.Range(0.6f, 1.4f);
        newFruit.transform.localScale = newScale * newFruit.transform.localScale;



        String [][]sets = new string[][]{
            new string[] { "apple", "apple", "apple","apple", "apple", "apple","apple", "apple", "apple", "apple", "apple", "apple" },
            new string[] {"orange", "orange","orange","orange","orange","orange","orange","orange","orange","orange","orange"},
            new string[] {"pear", "pear","pear","pear","pear","pear","pear","pear","pear","pear","pear" }
        };        
        print("set = " + whichSet + " fruit = " + whichFruit);
        SystemModel.setVariableValue("fruitGiven", sets[whichSet][whichFruit]);
        
        
        /*String[] set = new string[] { "apple", "orange", "pear" };            
        print("fruit = " + set[whichFruit] );        
        SystemModel.setVariableValue("fruitGiven", set[whichFruit]);
        */
    }

    private IEnumerator PutFruits()
    {
        int i = 0;
        while (true)
        {
            /*
            if( (0.001f*(float)i < putFruitPeriod) && SystemModel.getVariableValue("Q5")=="1")
            {
                yield return new WaitForSeconds(0.001f);
                i++;
            }            
            if(SystemModel.getVariableValue("Q5") == "0"){
                i = 0;
            }
            if (0.001f * (float)i >= putFruitPeriod && SystemModel.getVariableValue("Q5") == "1")
            {
                putOneFruit();
                i = 0;
            }
            */
            yield return new WaitForSeconds(3f);
            putOneFruit();
            print(i);
            i++;
        }
    }


    


    public void cameraCapture(int cameraNumber, Vector3? contactPoint)
    {
        Camera camera = Camera.main;
        switch (cameraNumber)
        {
            case 1: camera = camera_1; break;
            case 2: camera = camera_2; break;
            case 3: camera = camera_3; break;
        }
        StartCoroutine(CaptureAndSend(camera, "camera_" + cameraNumber, contactPoint));
    }

    public void onButtonWrite()
    {
        SystemModel.setVariableValue("I2", "" + (UnityEngine.Random.value * 100));
    }

    public void onButtonRead()
    {
        string value = SystemModel.getVariableValue("I2");
        print(value);
    }

    public void onGoForward()
    {
        SystemModel.setVariableValue("Q0", "1");
        SystemModel.setVariableValue("Q1", "0");
    }

    public void onGoBackward()
    {
        SystemModel.setVariableValue("Q0", "0");
        SystemModel.setVariableValue("Q1", "1");
    }

    public void onStop()
    {
        SystemModel.setVariableValue("Q0", "0");
        SystemModel.setVariableValue("Q1", "0");
    }

    public void onConveyorMove()
    {
        SystemModel.setVariableValue("I11", "1");
    }

    public void onConveyorStop()
    {
        SystemModel.setVariableValue("I11", "0");
    }

    public void onCameraCapture()
    {
    }

    private IEnumerator CapturePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(capturePeriod);
            StartCoroutine(CaptureAndSend(topCamera, "camera", null));
        }
    }

    
    private IEnumerator CaptureAndSend(Camera myCamera, string cameraName, Vector3? contactPoint)
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(myCamera.pixelWidth, myCamera.pixelHeight, 16);
        myCamera.targetTexture = renderTexture;
        myCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        myCamera.targetTexture = null;
        RenderTexture.active = null;

        Texture2D clippedTexture = null;
        if (contactPoint.HasValue)
        {
            Vector3 screenPoint = myCamera.WorldToScreenPoint(contactPoint.Value);
            float screenX = screenPoint.x+0;
            float screenY = screenPoint.y+0;
            int clipWidth = 500;
            int clipHeight = 500;
            int startX = Mathf.Clamp((int)screenX - clipWidth / 2, 0, texture.width - clipWidth);
            int startY = Mathf.Clamp((int)screenY - clipHeight / 2, 0, texture.height - clipHeight);

            // Adjust clipWidth and clipHeight to ensure they do not exceed texture bounds
            if (startX + clipWidth > texture.width)
            {
                clipWidth = texture.width - startX;
            }
            if (startY + clipHeight > texture.height)
            {
                clipHeight = texture.height - startY;
            }

            // Ensure clipWidth and clipHeight are positive
            clipWidth = Mathf.Max(clipWidth, 0);
            clipHeight = Mathf.Max(clipHeight, 0);

            clippedTexture = new Texture2D(clipWidth, clipHeight);
            clippedTexture.SetPixels(texture.GetPixels(startX, startY, clipWidth, clipHeight));
            clippedTexture.Apply();
        }

        byte[] imageBytes = (contactPoint.HasValue) ? clippedTexture.EncodeToPNG() : texture.EncodeToPNG();
        string base64String = System.Convert.ToBase64String(imageBytes);

        SystemModel.setVariableValue(cameraName + "Before", SystemModel.getVariableValue(cameraName + "Now"));
        SystemModel.setVariableValue(cameraName + "Now", base64String);

        SystemModel.setVariableValue(cameraName + "TimestampBefore", SystemModel.getVariableValue(cameraName + "TimestampNow"));
        string timestampNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
        SystemModel.setVariableValue(cameraName + "TimestampNow", timestampNow);

        Destroy(renderTexture);
        Destroy(texture);
        if (clippedTexture != null)
        {
            Destroy(clippedTexture);
        }
    }
    
    
}
