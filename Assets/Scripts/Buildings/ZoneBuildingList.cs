using UnityEngine;

public class ZoneBuildingList : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("Building Lists")]
    public GameObject[] commercial;
    public GameObject[] industrial;
    public GameObject[] residential;

    [Space(10)]
    [Header("Building Parents")]

    public Transform comParent;
    public Transform indParent;
    public Transform resParent;

    // ===== Private Variables ====================================================================

    private int comBuildings;
    private int indBuildings;
    private int resBuildings;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        comBuildings = commercial.Length;
        indBuildings = industrial.Length;
        resBuildings = residential.Length;
    }

    // ===== Public Functions =====================================================================

    public GameObject RandomBuilding (Zone.Type type) => type switch 
    {
        Zone.Type.Commercial => commercial[Random.Range(0, comBuildings)],
        Zone.Type.Industrial => industrial[Random.Range(0, indBuildings)],
        _ => residential[Random.Range(0, resBuildings)],
    };

    public Transform Parent (Zone.Type type) => type switch 
    {
        Zone.Type.Commercial => comParent,
        Zone.Type.Industrial => indParent,
        _ => resParent,
    };
}
