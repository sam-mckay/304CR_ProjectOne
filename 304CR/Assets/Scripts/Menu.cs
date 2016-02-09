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
            forestCostSlider.value = forestCost;
        }
        if (roadCost != 0)
        {
            roadCostSlider.value = roadCost;
        }
        
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(fpCamera == null)
        {
            fpCamera = GameObject.FindGameObjectWithTag("fpCamera").GetComponent<Camera>();
        }
	}

    void startDemo()
    {
        setCosts();
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
        PlayerPrefs.SetInt(SaveManager.forestCost, forestCost);
        PlayerPrefs.SetInt(SaveManager.roadCost, roadCost);
    }

    public void switchCamera()
    {
        mainCameraOn = !mainCameraOn;
        mainCamera.gameObject.SetActive(mainCameraOn);
        fpCamera.gameObject.SetActive(!mainCameraOn);
    }
}
