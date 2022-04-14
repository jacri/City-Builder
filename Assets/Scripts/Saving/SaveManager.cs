using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    // ===== Public Variables =====================================================================

    [Header("World")]
    public string saveName;
    public Transform roadParent;

    [Space(10)]
    [Header("Prefabs")]

    public GameObject road;
    
    // ===== Private Variables ====================================================================

    string str;

    // ===== Public Functions =====================================================================

    public void Save ()
    {
        string dir = string.Format("{0}/Saves/{1}", Application.dataPath, saveName);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string roadsFile = string.Format("{0}/Saves/{1}/rds.save", Application.dataPath, saveName);
        using (FileStream fs = File.OpenWrite(roadsFile))
        {
            StreamWriter sw = new StreamWriter(fs);
            Debug.Log("Saving Roads to " + roadsFile);

            List<string> roads = new List<string> ();
            foreach (Road r in roadParent.GetComponentsInChildren<Road>())
                roads.Add(r.name);
                
            roads.Sort();
            roads.ForEach(r => Debug.Log(r));

            foreach (Road r in roadParent.GetComponentsInChildren<Road>())
                sw.Write(string.Format("{0},{1},{2}\n", r.transform.position.x, r.transform.position.y, r.transform.position.z));

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
    }

}