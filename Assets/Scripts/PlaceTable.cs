using GoogleARCore;
using GoogleARCore.Examples.Common;
using System.Collections.Generic;
using UnityEngine;


public class PlaceTable : MonoBehaviour
{
    public bool online = false;
    public GameObject detectedPlanePrefab;
    public GameObject wictory;
    private GameObject replacingTableButton;
    private bool placementPoseIsValid = false;
    private Pose placmentPose;
    private bool start = false;
    private bool gameNow = false;
    private Anchor anchor;

    private void Start()
    {
        gameObject.name = "GameTable";
        start = true;
        wictory = Instantiate(wictory);
        GameObject.Find("SceneController").GetComponent<DetectedPlaneGenerator>().VisiblePlane(true);
        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show("Отсканируйте ровную поверхность на которую поместите игровой стол");

        replacingTableButton = GameObject.Find("ReplacingTablePosition").transform.GetChild(0).gameObject;
        replacingTableButton.GetComponent<ReplacingTablePosition>().SetLinkOnTable(gameObject);
    }
    public GameObject GetWictory()
    {
        return wictory;
    }

    public void Replacing()
    {
        start = true;
        gameObject.SetActive(true);
        GameObject sceneController = GameObject.Find("SceneController");
        gameObject.transform.SetParent(sceneController.transform);
        wictory.transform.SetParent(sceneController.transform);
        sceneController.GetComponent<DetectedPlaneGenerator>().VisiblePlane(true);
    }
    public void StopPlacing(TrackableHit hit)
    {
        if (placementPoseIsValid)
        {
            start = false;
            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show(false);
            GameObject sceneController = GameObject.Find("SceneController");
            sceneController.GetComponent<DetectedPlaneGenerator>().VisiblePlane(false);
            if (anchor != null)
            {
                gameObject.transform.SetParent(sceneController.transform); 
                wictory.transform.SetParent(sceneController.transform);
                Destroy(anchor);
            }

            anchor = hit.Trackable.CreateAnchor(new Pose(hit.Pose.position, Quaternion.identity));

            gameObject.transform.position = hit.Pose.position;
            gameObject.transform.SetParent(anchor.transform);
            wictory.transform.SetParent(anchor.transform);
            replacingTableButton.SetActive(true);

            //GameObject.Find("SceneController").GetComponent<DetectedPlaneGenerator>().DetectedPlanePrefab = null;
            if (online)
            {
                GameObject.Find("Netrwork").GetComponent<Client>().SendSetGameTable(true);
            }else if (!gameNow)
            {
                gameNow = true;
                GameObject.Find("SceneController").GetComponent<CardsMonitoring>().DistributionOfCards(-1);                
            }
        }
        else
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }
    }
    void Update()
    {
        if (start)
        {
            replacingTableButton.SetActive(false);
            GameObject.Find("SceneController").GetComponent<DetectedPlaneGenerator>().DetectedPlanePrefab = detectedPlanePrefab;
            Touch touch;
            if (Input.touchCount != 1 ||
                (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                UpdatePlacementPose();
                UpdatePlcamentTable();
            }
            else
            {
                TrackableHit hit =  UpdatePlacementPose();
                UpdatePlcamentTable();
                StopPlacing(hit);
            }
        }
    }
    private void UpdatePlcamentTable()
    {
        if (placementPoseIsValid)
        {
            gameObject.transform.SetPositionAndRotation(placmentPose.position, placmentPose.rotation);
            wictory.transform.SetPositionAndRotation(new Vector3(placmentPose.position.x, 
                placmentPose.position.y + 0.2f, placmentPose.position.z), placmentPose.rotation);
            wictory.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = true;
            }
        }
        else
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }
    }

    private TrackableHit UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        TrackableHit hit;
        TrackableHitFlags raycastFilter =
            TrackableHitFlags.PlaneWithinBounds |
            TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(screenCenter.x, screenCenter.y, raycastFilter, out hit))
        {
            placementPoseIsValid = true;
            placmentPose = hit.Pose;
            placmentPose.position = new Vector3(placmentPose.position.x, placmentPose.position.y - 0.09f, placmentPose.position.z);

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placmentPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        else
        {
            placementPoseIsValid = false;
        }
        return hit;
    }
}

