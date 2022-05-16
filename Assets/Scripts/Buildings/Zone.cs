using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : Buildable
{
    // ===== Enumerations =========================================================================

    public enum Type 
    { 
        LowDensityCommercial,
        HighDensityCommercial,


        LowDensityIndustrial,
        HighDensityIndustrial,

        LowDensityResidential,
        HighDensityResidential,
    }

    // ===== Public Variables =====================================================================

    [Header("Zone Information")]

    public Type type;
    public float minConstructionDelay;
    public float maxConstructionDelay;
    public float minConstructionTime;
    public float maxConstructionTime;
    public GameObject constructionIndicator;

    // ===== Public Static Variables ==============================================================

    public static Dictionary<Type, int> zoneDensity = new Dictionary<Type, int>() 
    {
        { Type.LowDensityCommercial,  1 },
        { Type.LowDensityIndustrial,  1 },
        { Type.LowDensityResidential, 1 },

        { Type.HighDensityCommercial,  2 },
        { Type.HighDensityIndustrial,  2 },
        { Type.HighDensityResidential, 2 },
    };

    // ===== Protected Variables ==================================================================

    protected ZoneBuildingList buildingList;

    // ===== Start ================================================================================
    
    protected override void Start ()
    {
        base.Start();
        buildingList = FindObjectOfType<ZoneBuildingList>();
    }

    // ===== Public Functions =====================================================================

    public override void GetAdjacent(bool recursive = false)
    {
        base.GetAdjacent();
        StartCoroutine(Build());
    }

    // ===== Public Static Functions ==============================================================

    public static bool IsCommercial (Type t)  => t == Type.LowDensityCommercial  || t == Type.HighDensityCommercial;
    public static bool IsIndustrial (Type t)  => t == Type.LowDensityIndustrial  || t == Type.HighDensityIndustrial;
    public static bool IsResidential (Type t) => t == Type.LowDensityResidential || t == Type.HighDensityResidential;

    // ===== Private Functions ====================================================================

    private IEnumerator Build ()
    {
        yield return new WaitForSecondsRealtime(Random.Range(minConstructionDelay, maxConstructionDelay));

        GameObject conInd = Instantiate(constructionIndicator, transform.position, Quaternion.identity);

        yield return new WaitForSecondsRealtime(Random.Range(minConstructionTime, maxConstructionTime));

        GameObject building = Instantiate(buildingList.RandomBuilding(type), transform.position, Quaternion.identity, buildingList.Parent(type));
        building.GetComponent<Buildable>().GetAdjacent();
        building.transform.position += building.GetComponent<Buildable>().offset;

        FindObjectOfType<Economy>().BuildZone(type, zoneDensity[type]);

        Destroy(conInd);
        Destroy(gameObject);
    }

    // ===== Protected Functions ==================================================================

    protected override void OnDestroy()
    {
        // Overriden so the position is not removed from the building hashset
    }
}