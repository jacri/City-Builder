using UnityEngine;

public class ZoneBuildingList : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("Building Lists")]

    public GameObject[] residential;
    public GameObject[] commercial;
    public GameObject[] industrial;

    [Space(10)]
    [Header("Building Parents")]

    public Transform comParent;
    public Transform indParent;
    public Transform resParent;

    // ===== Private Variables ====================================================================

    private int residentialBUildings;
    private int commercialBuildings;
    private int industrialBuildings;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        residentialBUildings = residential.Length;
        commercialBuildings  = commercial.Length;
        industrialBuildings  = industrial.Length;
    }

    // ===== Public Functions =====================================================================

    public GameObject RandomBuilding (Zone.Type type) => type switch 
    {
        Zone.Type.Residential => residential[Random.Range(0, residentialBUildings)],
        Zone.Type.Commercial  => commercial [Random.Range(0, commercialBuildings)],
        Zone.Type.Industrial  => industrial [Random.Range(0, industrialBuildings)],

        _ => throw new System.ArgumentException(),
    };

    public Transform Parent (Zone.Type type) => type switch 
    {
        Zone.Type.Residential => resParent,
        Zone.Type.Commercial  => comParent,
        Zone.Type.Industrial  => indParent,

        _ => throw new System.ArgumentException(),
    };
}
