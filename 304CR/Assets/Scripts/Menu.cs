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
    Vector3 offset;
    /// 0 = wall, 1 = road, 2 = forest
    int nodeType;

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
        offset = new Vector3(0.5f, 0.0f, 0.5f); 
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(fpCamera == null)
        {
            fpCamera = GameObject.FindGameObjectWithTag("aiAgent").GetComponent<AI_AGENT_CONTROLLER>().getCamera();
        }
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 newObjectPosition = getMouseGrid();
            switch(nodeType)
            {
                case 0:
                    createWall(newObjectPosition);
                    break;
                case 1:
                    createRoad(newObjectPosition);
                    break;
                case 2:
                    createForest(newObjectPosition);
                    break;
                default:
                    Debug.Log("ERROR: OBJECT NOT SELECTED");
                    break;
            }
            AI_AGENT_CONTROLLER player = fpCamera.transform.parent.gameObject.GetComponent<AI_AGENT_CONTROLLER>();
            player.restart();
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

    Vector3 getMouseGrid()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 newPosition = ray.origin + (ray.direction*10.0f);
        
        newPosition += offset;
        newPosition.x = Mathf.FloorToInt(newPosition.x);
        newPosition.y = Mathf.FloorToInt(newPosition.y);
        newPosition.z = Mathf.FloorToInt(newPosition.z);

        return newPosition;
    }

    public void createWall(Vector3 newPosition)
    {
        AI_AGENT_CONTROLLER player = fpCamera.transform.parent.gameObject.GetComponent<AI_AGENT_CONTROLLER>();
        player.walls.Add(new Vector2((int)newPosition.x, (int)newPosition.z));
    }

    public void createForest(Vector3 newPosition)
    {
        AI_AGENT_CONTROLLER player = fpCamera.transform.parent.gameObject.GetComponent<AI_AGENT_CONTROLLER>();
        player.forests.Add(new Vector2((int)newPosition.x, (int)newPosition.z));
    }

    public void createRoad(Vector3 newPosition)
    {
        AI_AGENT_CONTROLLER player = fpCamera.transform.parent.gameObject.GetComponent<AI_AGENT_CONTROLLER>();
        player.roads.Add(new Vector2((int)newPosition.x, (int)newPosition.z));
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

    public void selectObject(int objectID)
    {
        nodeType = objectID;
    }
}
