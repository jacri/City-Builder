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

    string str;

    // ===== Public Functions =====================================================================

    public void Save ()
    {
        string dir = $"{Application.dataPath}/Saves/{saveName}";

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string roadsFile = $"{Application.dataPath}/Saves/{saveName}/rds.save";
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

        string zonesFile = $"{Application.dataPath}/Saves/{saveName}/zns.save";
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

    public void Load ()
    {
        string roadsFile = string.Format("{0}/Saves/{1}/rds.save", Application.dataPath, saveName);
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

        string zonesFile = $"{Application.dataPath}/Saves/{saveName}/zns.save";
        using (FileStream fs = File.OpenRead(zonesFile))
        {
            StreamReader sr = new StreamReader(fs);

            string line;
            string[] arr;

            while ((line = sr.ReadLine()) != null)
            {
                arr = line.Split(',');
                GameObject zone = zones[int.Parse(arr[0])];
                Vector3 pos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]));
                Instantiate(zone, pos, Quaternion.identity, zoneParent);
            }

            sr.Close();
            fs.Close();
        }
    }
}