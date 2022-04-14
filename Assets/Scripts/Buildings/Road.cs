using UnityEngine;
using System.Collections.Generic;

public class Road : Buildable
{
    // ===== Public Variables =====================================================================

    [Space(10)]
    [Header("Materials")]

    public Material roadMat;
    public Material curbMat;

    // ===== Start ================================================================================
    
    protected override void Awake ()
    {
        base.Awake();

        mats = new Material[6] { curbMat, roadMat, roadMat, roadMat, roadMat, roadMat };
        rend = GetComponent<MeshRenderer>();
    }

    // ===== Public Functions =====================================================================

    public override void GetAdjacent (bool recursive = false)
    {
        base.GetAdjacent(recursive);
        UpdateMaterials();
    }
    
    public override void Remove (Direction dir)
    {
        bool updateMats = false;

        switch (dir)
        {
            case Direction.Up:
                updateMats = up != null;
                up = null;
                break;

            case Direction.Down:
                updateMats = down != null;
                down = null;
                break;

            case Direction.Left:
                updateMats = left != null;
                left = null;
                break;

            case Direction.Right:
                updateMats = right != null;
                right = null;
                break;
        }

        if (updateMats)
            UpdateMaterials();
    }

    public void UpdateMaterials ()
    {
        mats[0] = curbMat;
        mats[1] = up?.GetComponent<Road>()    != null ? roadMat : curbMat;
        mats[2] = right?.GetComponent<Road>() != null ? roadMat : curbMat;
        mats[3] = down?.GetComponent<Road>()  != null ? roadMat : curbMat;
        mats[4] = left?.GetComponent<Road>()  != null ? roadMat : curbMat;
        mats[5] = roadMat;
        rend.materials = mats;
    }

    public void UpdateAdjacentMaterials (List<Road> toBuild)
    {
        if (up != null && !toBuild.Contains(up?.GetComponent<Road>()))
            up.GetComponent<Road>()?.GetAdjacent();
            
        if (down != null && !toBuild.Contains(down?.GetComponent<Road>()))
            down.GetComponent<Road>()?.GetAdjacent();

        if (left != null && !toBuild.Contains(left?.GetComponent<Road>()))
            left.GetComponent<Road>()?.GetAdjacent();

        if (right != null && !toBuild.Contains(right?.GetComponent<Road>()))
            right.GetComponent<Road>()?.GetAdjacent();
    }

    public override void StopDeletionHighlight() => UpdateMaterials();
}