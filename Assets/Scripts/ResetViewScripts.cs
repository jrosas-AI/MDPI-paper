using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetViewScripts : MonoBehaviour
{


    public bool ResetView = false;
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
    public GameObject FreeCam_button;
    private CamPosition View1_script;
    private CamPosition View2_script;
    private CamPosition View3_script;
    private CamPosition View4_script;
    private CamPosition View5_script;
    private ButtonFreeCam FreeCam_script;
    private Color white;
    private Vector3 Start_Position1;
    private Vector3 Start_Position2;
    private Vector3 Start_Position3;
    private Vector3 Start_Position4;
    private Vector3 Start_Position5;
    public GameObject Cam;

    // Start is called before the first frame update
    void Start()
    {
        Start_Position1 = View1.transform.position;
        Start_Position2 = View2.transform.position;
        Start_Position3 = View3.transform.position;
        Start_Position4 = View4.transform.position;
        Start_Position5 = View5.transform.position;
        UnityEngine.ColorUtility.TryParseHtmlString("#FFFFFF", out white);
        View1_script = View1.GetComponent<CamPosition>();
        View2_script = View2.GetComponent<CamPosition>();
        View3_script = View3.GetComponent<CamPosition>();
        View4_script = View4.GetComponent<CamPosition>();
        View5_script = View5.GetComponent<CamPosition>();
        FreeCam_script = Cam.GetComponent<ButtonFreeCam>();
    }

    public void OnResetView()
    {
        View1.transform.position = Start_Position1;
        View2.transform.position = Start_Position2;
        View3.transform.position = Start_Position3;
        View4.transform.position = Start_Position4;
        View5.transform.position = Start_Position5;
        Cam.GetComponent<FreeCam>().enabled = false;
        View1_script.control = false;
        View2_script.control = false;
        View3_script.control = false;
        View4_script.control = false;
        View5_script.control = false;
        FreeCam_script.control = false;
        View1_button.GetComponent<Image>().color = white;
        View2_button.GetComponent<Image>().color = white;
        View3_button.GetComponent<Image>().color = white;
        View4_button.GetComponent<Image>().color = white;
        View5_button.GetComponent<Image>().color = white;
        FreeCam_button.GetComponent<Image>().color = white;

    }
    
}
