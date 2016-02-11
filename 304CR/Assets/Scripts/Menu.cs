using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour
{
    public Slider forestCostSlider;
    public Slider roadCostSlider;

    Camera mainCamera;
    Camera fpCamera;
    bool mainCameraOn = true;
    int forestCost;
    int roadCost;
    
    // Use this for initialization
    void Start ()
    {
        //setup cameras
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        fpCamera = GameObject.FindGameObjectWithTag("fpCamera").GetComponent<Camera>();
        fpCamera.gameObject.SetActive(false);
        //set costs
        forestCost = PlayerPrefs.GetInt(SaveManager.forestCost);
        roadCost = PlayerPrefs.GetInt(SaveManager.roadCost);
        if (forestCost != 0)
        {
            forestCost = (int)forestCostSlider.value;
        }
        if (roadCost != 0)
        {
            PlayerPrefs.SetInt(SaveManager.roadCost, roadCost);
        }
        
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(fpCamera == null)
        {
            fpCamera = GameObject.FindGameObjectWithTag("aiAgent").GetComponent<AI_AGENT_CONTROLLER>().getCamera();
        }
	}

    public void startDemo()
    {
        setCosts();
        AI_AGENT_CONTROLLER agent = GameObject.FindGameObjectWithTag("aiAgent").GetComponent<AI_AGENT_CONTROLLER>();
        agent.restart();
        fpCamera = GameObject.FindGameObjectWithTag("fpCamera").GetComponent<Camera>();
        //TODO: fix camera bug when first person camera selected and "Apply" clicked
    }

    Vector2 getMouseGrid()
    {
        return new Vector2(0,0);
    }

    public void createWall()
    {

    }

    public void createForest()
    {

    }

    public void createRoad()
    {

    }

    public void setCosts()
    {
        forestCost = (int)forestCostSlider.value;
        roadCost = (int)roadCostSlider.value;
        Debug.Log("NEW COST: " + forestCost);

        PlayerPrefs.SetInt(SaveManager.forestCost, forestCost);
        PlayerPrefs.SetInt(SaveManager.roadCost, roadCost);
        PlayerPrefs.Save();
    }

    public void switchCamera()
    {
        mainCameraOn = !mainCameraOn;
        mainCamera.gameObject.SetActive(mainCameraOn);
        fpCamera.gameObject.SetActive(!mainCameraOn);
    }
}
