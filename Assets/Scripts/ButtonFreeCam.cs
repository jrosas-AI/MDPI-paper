using UnityEngine;
using UnityEngine.UI;

public class ButtonFreeCam : MonoBehaviour
{
    public GameObject Button;
    public GameObject View1;
    public GameObject View2;
    public GameObject View3;
    public GameObject View4;
    public GameObject View5;
    public GameObject View1_button;
    public GameObject View2_button;
    public GameObject View3_button;
    public GameObject View4_button;
    public GameObject View5_button;
    private CamPosition View1_script;
    private CamPosition View2_script;
    private CamPosition View3_script;
    private CamPosition View4_script;
    private CamPosition View5_script;
    public bool control = false;
    private Color white;
    

    void Start()
    {
        View1_script = View1.GetComponent<CamPosition>();
        View2_script = View2.GetComponent<CamPosition>();
        View3_script = View3.GetComponent<CamPosition>();
        View4_script = View4.GetComponent<CamPosition>();
        View5_script = View5.GetComponent<CamPosition>();
        UnityEngine.ColorUtility.TryParseHtmlString("#FFFFFF", out white);
    }

    void Update()
    {

    }

    public void onChangeFreeCam()
    {
        if (control == false)
        {
            GetComponent<FreeCam>().enabled = true;
            Color lightRed;
            UnityEngine.ColorUtility.TryParseHtmlString("#FF9999", out lightRed);
            Button.GetComponent<Image>().color = lightRed;
            View1_button.GetComponent<Image>().color = white;
            View2_button.GetComponent<Image>().color = white;
            View3_button.GetComponent<Image>().color = white;
            View4_button.GetComponent<Image>().color = white;
            View5_button.GetComponent<Image>().color = white;
            control = true;
        }
        else
        {
            GetComponent<FreeCam>().enabled = false;
            Button.GetComponent<Image>().color = white;
            if (View1_script.control)
            {
                View1_button.GetComponent<Image>().color = white;
                View1_script.control = false;
                View1_script.View.transform.position = transform.position;
                View1_script.View.transform.rotation = transform.rotation;
            }
            if (View2_script.control)
            {
                View2_button.GetComponent<Image>().color = white;
                View2_script.control = false;
                View2_script.View.transform.position = transform.position;
                View2_script.View.transform.rotation = transform.rotation;
            }
            if (View3_script.control)
            {
                View3_button.GetComponent<Image>().color = white;
                View3_script.control = false;
                View3_script.View.transform.position = transform.position;
                View3_script.View.transform.rotation = transform.rotation;
            }
            if (View4_script.control)
            {
                View4_button.GetComponent<Image>().color = white;
                View4_script.control = false;
                View4_script.View.transform.position = transform.position;
                View4_script.View.transform.rotation = transform.rotation;
            }
            if (View5_script.control)
            {
                View5_button.GetComponent<Image>().color = white;
                View5_script.control = false;
                View5_script.View.transform.position = transform.position;
                View5_script.View.transform.rotation = transform.rotation;
            }
            control = false;
        }
    }
}
