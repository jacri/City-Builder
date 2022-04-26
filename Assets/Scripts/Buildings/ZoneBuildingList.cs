using UnityEngine;

public class ZoneBuildingList : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("Building Lists")]
    public GameObject[] lowDensityCommercial;
    public GameObject[] highDensityCommercial;

    public GameObject[] lowDensityIndustrial;
    public GameObject[] highDensityIndustrial;

    public GameObject[] lowDensityResidential;
    public GameObject[] highDensityResidential;

    [Space(10)]
    [Header("Building Parents")]

    public Transform comParent;
    public Transform indParent;
    public Transform resParent;

    // ===== Private Variables ====================================================================

    private int lowDensityComBuildings;
    private int highDensityComBuildings;

    private int lowDensityIndBuildings;
    private int highDensityIndBuildings;

    private int lowDensityResBuildings;
    private int highDensityResBuildings;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        lowDensityComBuildings = lowDensityCommercial.Length;
        highDensityComBuildings = highDensityCommercial.Length;

        lowDensityIndBuildings = lowDensityIndustrial.Length;
        highDensityIndBuildings = highDensityIndustrial.Length;

        lowDensityResBuildings = lowDensityResidential.Length;
        highDensityResBuildings = highDensityResidential.Length;
    }

    // ===== Public Functions =====================================================================

    public GameObject RandomBuilding (Zone.Type type) => type switch 
    {
        Zone.Type.LowDensityCommercial  => lowDensityCommercial[Random.Range(0, lowDensityComBuildings)],
        Zone.Type.HighDensityCommercial => highDensityCommercial[Random.Range(0, highDensityComBuildings)],

        Zone.Type.LowDensityIndustrial => lowDensityIndustrial[Random.Range(0, lowDensityIndBuildings)],
        Zone.Type.HighDensityIndustrial => highDensityIndustrial[Random.Range(0, highDensityIndBuildings)],

        Zone.Type.LowDensityResidential => lowDensityResidential[Random.Range(0, lowDensityResBuildings)],
        Zone.Type.HighDensityResidential => highDensityResidential[Random.Range(0, highDensityResBuildings)],

        _ => throw new System.ArgumentException(),
    };

    public Transform Parent (Zone.Type type) => type switch 
    {
        Zone.Type.LowDensityCommercial  or Zone.Type.HighDensityCommercial => comParent,
        Zone.Type.LowDensityIndustrial  or Zone.Type.HighDensityIndustrial => indParent,
        Zone.Type.LowDensityResidential or Zone.Type.HighDensityResidential => resParent,

        _ => throw new System.ArgumentException(),
    };
}
