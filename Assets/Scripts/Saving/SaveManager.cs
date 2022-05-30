using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SaveManager : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("World")]
    public string saveName;

    [Space(10)]
    [Header("Roads")]

    public GameObject road;
    public Transform roadParent;

    [Space(10)]
    [Header("Zones")]

    public GameObject[] zones;
    public Transform zoneParent;

    [Space(10)]
    [Header("Buildings")]

    public GameObject[][] buildings;
    public Transform[] buildingParents;

    
    // ===== Private Variables ====================================================================

    private string dir;
    private string cityFile;
    private string roadsFile;
    private string zonesFile;
    private string buildingFile;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        dir = $"{Application.dataPath}/Saves/{saveName}";

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        cityFile = $"{dir}/cty.save";
        roadsFile = $"{dir}/rds.save";
        zonesFile = $"{dir}/zns.save";
        buildingFile = $"{dir}/bld.save";

        buildings = new GameObject[3][];
        buildings[0] = zoneParent.GetComponent<ZoneBuildingList>().residential;
        buildings[1] = zoneParent.GetComponent<ZoneBuildingList>().commercial;
        buildings[2] = zoneParent.GetComponent<ZoneBuildingList>().industrial;
    }

    // ===== Public Functions =====================================================================

    public void Save ()
    {
        SaveCityInfo();
        SaveRoads();
        SaveZones();
        SaveBuildings();
    } 

    public void Load ()
    {
        LoadCityInfo();
        LoadRoads();
        LoadZones();
        LoadBuildings();

        foreach (Buildable b in FindObjectsOfType<Buildable>())
            b.GetAdjacent();
    }

    public void Quit () => Application.Quit();

    // ===== Private Saving Functions =============================================================

    private void SaveCityInfo ()
    {
        using (FileStream fs = File.OpenWrite(cityFile))
        {
            fs.SetLength(0);    // Clear file
            StreamWriter sw = new StreamWriter(fs);

            Economy eco = FindObjectOfType<Economy>();
            sw.Write($"{eco.money},{eco.resTaxRate},{eco.comTaxRate},{eco.indTaxRate},{eco.population}");

            sw.Close();
            fs.Close();
        }
    }

    private void SaveRoads ()
    {
        using (FileStream fs = File.OpenWrite(roadsFile))
        {
            fs.SetLength(0);    // Clear file
            StreamWriter sw = new StreamWriter(fs);

            List<Road> roadList = new List<Road>();
            foreach (Road r in roadParent.GetComponentsInChildren<Road>())
                roadList.Add(r);
                
            // Remove duplicate entries
            roadList = roadList.OrderBy(r => r.name).ToList();

            foreach (Road r in roadList)
                sw.Write($"{r.transform.position.x},{r.transform.position.y},{r.transform.position.z}\n");

            sw.Close();
            fs.Close();
        }
    }

    private void SaveZones ()
    {
        using (FileStream fs = File.OpenWrite(zonesFile))
        {
            fs.SetLength(0);    // Clear file
            StreamWriter sw = new StreamWriter(fs);

            List<Zone> zoneList = new List<Zone>();
            foreach (Zone z in zoneParent.GetComponentsInChildren<Zone>())
                zoneList.Add(z);

            zoneList = zoneList.OrderBy(z => z.name).ToList();

            foreach (Zone z in zoneList)
                sw.Write($"{(int)z.type},{z.transform.position.x},{z.transform.position.y},{z.transform.position.z}\n");

            sw.Close();
            fs.Close();
        }
    }

    private void SaveBuildings ()
    {
        using (FileStream fs = File.OpenWrite(buildingFile))
        {
            fs.SetLength(0);    // Clear file
            StreamWriter sw = new StreamWriter(fs);

            foreach (Buildable b in FindObjectsOfType<Buildable>())
            {
                // Subtracting 1 from buildingIndex to match with ZoneBuildingList array
                if (b.name.Contains("Residential Building"))
                    sw.Write($"{(int)0},{(int)int.Parse(Regex.Match(b.name, @"\d+").Value) - 1},{b.transform.position.x},{b.transform.position.y},{b.transform.position.z}\n");

                else if (b.name.Contains("Commercial Building"))
                    sw.Write($"{(int)1},{(int)int.Parse(Regex.Match(b.name, @"\d+").Value) - 1},{b.transform.position.x},{b.transform.position.y},{b.transform.position.z}\n");

                else if (b.name.Contains("Industrial Building"))
                    sw.Write($"{(int)2},{(int)int.Parse(Regex.Match(b.name, @"\d+").Value) - 1},{b.transform.position.x},{b.transform.position.y},{b.transform.position.z}\n");
            }

            sw.Close();
            fs.Close();
        }
    }

    // ===== Private Loading Functions =============================================================

    private void LoadCityInfo ()
    {
        using (FileStream fs = File.OpenRead(cityFile))
        {
            StreamReader sr = new StreamReader(fs);
            Economy eco = FindObjectOfType<Economy>();

            string line = sr.ReadLine();
            string[] arr = line.Split(',');

            eco.money = int.Parse(arr[0]);
            eco.UpdateMoneyText();
            eco.UpdateResTaxFromUI(float.Parse(arr[1]));
            eco.UpdateComTaxFromUI(float.Parse(arr[2]));
            eco.UpdateIndTaxFromUI(float.Parse(arr[3]));
            eco.population = int.Parse(arr[4]);
            eco.UpdatePopulationText();
            eco.UpdateDemand();

            sr.Close();
            fs.Close();
        }
    }

    private void LoadRoads ()
    {
        using (FileStream fs = File.OpenRead(roadsFile))
        {
            StreamReader sr = new StreamReader(fs);

            string line;
            string[] posArr;

            while ((line = sr.ReadLine()) != null)
            {
                posArr = line.Split(',');
                Vector3 pos = new Vector3(float.Parse(posArr[0]), float.Parse(posArr[1]), float.Parse(posArr[2]));
                Instantiate(road, pos, Quaternion.identity, roadParent);
            }

            sr.Close();
            fs.Close();
        }
    }

    private void LoadZones ()
    {
        using (FileStream fs = File.OpenRead(zonesFile))
        {
            StreamReader sr = new StreamReader(fs);

            string line;
            string[] arr;

            while ((line = sr.ReadLine()) != null)
            {
                arr = line.Split(',');
                int type = int.Parse(arr[0]);
                Vector3 pos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]));
                Instantiate(zones[type], pos, Quaternion.identity, zoneParent);
            }

            sr.Close();
            fs.Close();
        }
    }

    private void LoadBuildings ()
    {
        using (FileStream fs = File.OpenRead(buildingFile))
        {
            StreamReader sr = new StreamReader(fs);

            string line;
            string[] arr;

            while ((line = sr.ReadLine()) != null)
            {
                arr = line.Split(',');
                int type = int.Parse(arr[0]);
                int buildingIndex = int.Parse(arr[1]);
                Vector3 pos = new Vector3(float.Parse(arr[2]), float.Parse(arr[3]), float.Parse(arr[4]));
                Instantiate(buildings[type][buildingIndex], pos, Quaternion.identity, buildingParents[type]);
            }

            sr.Close();
            fs.Close();
        }
    }
}