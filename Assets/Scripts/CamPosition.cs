using UnityEngine;
using UnityEngine.UI;

// https://www.youtube.com/watch?v=vrolg5A8jYE
public class CamPosition : MonoBehaviour
{
    public GameObject Camera;
    public GameObject View;
    public bool control = false;
    public GameObject View1;
    public GameObject View2;
    public GameObject View3;
    public GameObject View4;
    public GameObject Button;
    public GameObject View1_button;
    public GameObject View2_button;
    public GameObject View3_button;
    public GameObject View4_button;
    private CamPosition View1_script;
    private CamPosition View2_script;
    private CamPosition View3_script;
    private CamPosition View4_script;
    private ResetViewScripts ResetView_script;
    private Color white;
    private Color lightRed;
    
    private void Start()
    {
        View1_script = View1.GetComponent<CamPosition>();
        View2_script = View2.GetComponent<CamPosition>();
        View3_script = View3.GetComponent<CamPosition>();
        View4_script = View4.GetComponent<CamPosition>();
        UnityEngine.ColorUtility.TryParseHtmlString("#FFFFFF", out white);
        UnityEngine.ColorUtility.TryParseHtmlString("#FF9999", out lightRed);
    }

    private void Update()
    {
        
    }

    public void onChangeCamView()
    {
        if (Camera.GetComponent<FreeCam>().enabled == false)
        {
            Camera.transform.position = View.transform.position;
            Camera.transform.rotation = View.transform.rotation;
            Button.GetComponent<Image>().color = lightRed;
            View1_button.GetComponent<Image>().color = white;
            View2_button.GetComponent<Image>().color = white;
            View3_button.GetComponent<Image>().color = white;
            View4_button.GetComponent<Image>().color = white;
        }
        else
        {
            if (!control)
            {
                if (View1_script.control)
                {
                    View1_button.GetComponent<Image>().color = white;
                    View1_script.control = false;
                    View1_script.View.transform.position = Camera.transform.position;
                    View1_script.View.transform.rotation = Camera.transform.rotation;
                }
                if (View2_script.control)
                {
                    View2_button.GetComponent<Image>().color = white;
                    View2_script.control = false;
                    View2_script.View.transform.position = Camera.transform.position;
                    View2_script.View.transform.rotation = Camera.transform.rotation;
                }
                if (View3_script.control)
                {
                    View3_button.GetComponent<Image>().color = white;
                    View3_script.control = false;
                    View3_script.View.transform.position = Camera.transform.position;
                    View3_script.View.transform.rotation = Camera.transform.rotation;
                }
                if (View4_script.control)
                {
                    View4_button.GetComponent<Image>().color = white;
                    View4_script.control = false;
                    View4_script.View.transform.position = Camera.transform.position;
                    View4_script.View.transform.rotation = Camera.transform.rotation;
                }
                Camera.transform.position = View.transform.position;
                Camera.transform.rotation = View.transform.rotation;
                Camera.GetComponent<FreeCam>().enabled = true;
                Button.GetComponent<Image>().color = lightRed;
                control = true;
            }
            else
            {
                View.transform.position = Camera.transform.position;
                View.transform.rotation = Camera.transform.rotation;
                Camera.GetComponent<FreeCam>().enabled = true;
                Button.GetComponent<Image>().color = white;
                control = false;
            }
        }
    }
}
