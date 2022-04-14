using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("World")]

    public GameObject grid;
    public GameObject finishButtons;
    public Material deletionHighlightMat;

    [Space(10)]
    [Header("Roads")]

    public GameObject road;
    public Vector3 roadOffset;
    public Transform roadParent;

    [Space(10)]
    [Header("Zones")]

    public GameObject resZone;
    public GameObject comZone;
    public GameObject indZone;
    public Vector3 zoneOffset;
    public Transform zoneParent;

    // ===== Public Static Variables ==============================================================

    public static HashSet<(float, float)> buildingPositions;

    // ===== Private Variables ====================================================================

    private bool loop;
    private bool confirmBuilding;
    private bool currnetlyBuilding = false;
  
    private Ray ray;
    private Vector3 pos;
    private RaycastHit hit;
    private List<GameObject> toBuild;

    // ===== Start ================================================================================
    
    private void Awake ()
    {
        toBuild = new List<GameObject>();
        buildingPositions = new HashSet<(float, float)>();
        Application.targetFrameRate = 120;
    }

    // ==== Build =================================================================================
    
    private IEnumerator BuildMutliple (Vector3 offset, GameObject building, Transform parent, Action<List<GameObject>, bool> finishAction = null)
    {
        while (currnetlyBuilding)
            yield return new WaitForEndOfFrame();

        toBuild.Clear();
        grid.SetActive(true);
        currnetlyBuilding = true;

        // Wait until click
        while (!Input.GetMouseButton(0))
            yield return new WaitForEndOfFrame();

        // Clicked
        loop = true;
        finishButtons.SetActive(true);

        while (loop)
        {
            if (Input.GetMouseButton(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && (hit.collider.name.Contains("Ground") || hit.collider.name.Contains("Tile")) && !EventSystem.current.IsPointerOverGameObject())
                {
                    pos = Vector3Int.RoundToInt(hit.point) + offset;

                    if (!buildingPositions.Contains((pos.x, pos.z)))
                    {
                        toBuild.Add(Instantiate(building, pos, Quaternion.identity, parent));
                        finishButtons.transform.position = new Vector3(pos.x, 1.5f, pos.z);
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        // Finished building
        grid.SetActive(false);
        finishButtons.SetActive(false);

        if (finishAction != null)
            finishAction.Invoke(toBuild, confirmBuilding);

        currnetlyBuilding = false;
    }

    public void Finish (bool confirm)
    {
        loop = false;
        confirmBuilding = confirm;
    }

    // ===== Zones ================================================================================

    public void StartPlacingZones (string typeString)
    {
        if (currnetlyBuilding) 
            Finish(true);

        GameObject zone = typeString switch 
        {
            "Commercial"  => comZone,
            "Residential" => resZone, 
            "Industrial"  => indZone,
            _ => throw new Exception("Error - please use valid zone type"),
        };

        StartCoroutine(BuildMutliple(zoneOffset, zone, zoneParent, FinishZoneFn));
    }

    private Action<List<GameObject>, bool> FinishZoneFn = delegate (List<GameObject> toBuild, bool confirmBuilding)
    {
        if (confirmBuilding == true)
            toBuild.ForEach(obj => 
            {
                obj.name = obj.GetComponent<Zone>().type + " Zone " + obj.transform.position;
                obj.GetComponent<Buildable>().GetAdjacent();
            });

        else
            toBuild.ForEach(obj => Destroy(obj));
    };

    // ===== Roads ================================================================================

    public void StartPlacingRoad ()
    {
        if (currnetlyBuilding) 
            Finish(true);

        StartCoroutine(BuildMutliple(roadOffset, road, roadParent, FinishRoad));
    }

    private Action<List<GameObject>, bool> FinishRoad = delegate (List<GameObject> toBuild, bool confirmBuilding)
    {
        List<Road> roads = toBuild.Select(obj => obj.GetComponent<Road>()).ToList();

        if (confirmBuilding == true)
        {
            roads.ForEach(obj => 
            {
                obj.name = "Road " + obj.transform.position;
                obj.GetAdjacent(true);
                obj.UpdateAdjacentMaterials(roads);
            });
        }

        // Destroy placeholder roads   
        else
            toBuild.ForEach(obj => Destroy(obj));
    };

    // ===== Destroy ==============================================================================

    private IEnumerator Destroy ()
    {
        while (currnetlyBuilding)
            yield return new WaitForEndOfFrame();

        toBuild.Clear();
        grid.SetActive(true);
        currnetlyBuilding = true;

        // Wait until click
        while (!Input.GetMouseButton(0))
            yield return new WaitForEndOfFrame();

        // Clicked
        loop = true;
        finishButtons.SetActive(true);
        
        while (loop)
        {
            if (Input.GetMouseButton(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit)  && !EventSystem.current.IsPointerOverGameObject() 
                    && hit.collider.GetComponent<Buildable>() && !toBuild.Contains(hit.collider.gameObject))
                {
                    toBuild.Add(hit.collider.gameObject);
                    pos = Vector3Int.RoundToInt(hit.point);
                    hit.collider.GetComponent<Buildable>().HighlightForDeletion(deletionHighlightMat);
                    finishButtons.transform.position = new Vector3(pos.x, 1.5f, pos.z);                   
                }
            }

            yield return new WaitForEndOfFrame();
        }

        // Finished building
        grid.SetActive(false);
        finishButtons.SetActive(false);

        if (confirmBuilding == true)
            toBuild.ForEach(obj => Destroy(obj));  

        else
            toBuild.ForEach(obj => obj.GetComponent<Buildable>().StopDeletionHighlight());

        currnetlyBuilding = false;
    }

    public void StartDestroyingBuildings () 
    {
        if (currnetlyBuilding) 
            Finish(true);

        StartCoroutine(Destroy());
    }
}