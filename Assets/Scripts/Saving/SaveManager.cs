using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
    
    // ===== Private Variables ====================================================================

    private string dir;
    private string roadsFile;
    private string zonesFile;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        dir = $"{Application.dataPath}/Saves/{saveName}";

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        roadsFile = $"{dir}/rds.save";
        zonesFile = $"{dir}/zns.save";
    }

    // ===== Public Functions =====================================================================

    public void Save ()
    {
        SaveRoads();
        SaveZones();
    } 

    public void Load ()
    {
        LoadRoads();
        LoadZones();
    }

    // ===== Private Saving Functions =============================================================

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

    // ===== Private Loading Functions =============================================================

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

        foreach (Road r in roadParent.GetComponentsInChildren<Road>())
            r.GetAdjacent();
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

        foreach (Zone z in zoneParent.GetComponentsInChildren<Zone>())
            z.GetAdjacent();
    }
}