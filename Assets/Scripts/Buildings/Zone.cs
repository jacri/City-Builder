using UnityEngine;
using System.Collections;

public class Zone : Buildable
{
    // ===== Enumerations =========================================================================

    public enum Type { Commercial, Residential, Industrial }

    // ===== Public Variables =====================================================================

    [Header("Zone Information")]

    public Type type;
    public float minConstructionDelay;
    public float maxConstructionDelay;
    public float minConstructionTime;
    public float maxConstructionTime;
    public GameObject constructionIndicator;

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

    // ===== Private Functions ====================================================================

    private IEnumerator Build ()
    {
        yield return new WaitForSecondsRealtime(Random.Range(minConstructionDelay, maxConstructionDelay));

        GameObject conInd = Instantiate(constructionIndicator, transform.position, Quaternion.identity);

        yield return new WaitForSecondsRealtime(Random.Range(minConstructionTime, maxConstructionTime));

        GameObject building = Instantiate(buildingList.RandomBuilding(type), transform.position, Quaternion.identity, buildingList.Parent(type));
        building.GetComponent<Buildable>().GetAdjacent();

        Destroy(conInd);
        Destroy(gameObject);
    }

    // ===== Protected Functions ==================================================================

    protected override void OnDestroy()
    {
        // Overriden so the position is not removed from the building hashset
        // TEST
    }
}