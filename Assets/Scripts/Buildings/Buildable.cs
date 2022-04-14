using UnityEngine;

public class Buildable : MonoBehaviour
{
    // ===== Enumerations =========================================================================

    public enum Direction { Left, Right, Up, Down }

    // ===== Public Variables =====================================================================

    [Header("Adjacent Buildings")]

    public Buildable up;
    public Buildable down;
    public Buildable left;
    public Buildable right;

    [Space(10)]
    [Header("Utility Information")]

    public float buildCost;
    public bool hasPower;
    public bool hasRoadConnection;

    // ===== Private Variables ====================================================================

    protected Vector3 pos;
    protected RaycastHit hit;
    protected MeshRenderer rend;

    protected Material[] mats;
    protected Material[] deleteIndicator;

    // ===== Awake ================================================================================
    
    protected virtual void Awake ()
    {
        rend = GetComponent<MeshRenderer>();
        mats = rend.materials;
        pos = transform.position;
    }

    // ===== Start ================================================================================

    protected virtual void Start ()
    {
        BuildingManager.buildingPositions.Add((transform.position.x, transform.position.z));
    }

    // ===== Public Functions =====================================================================

    public virtual void GetAdjacent (bool recursive = false)
    {
        bool roadAdjacent = false;
        
        if (Physics.Raycast(pos, Vector3.forward, out hit, 0.75f))
        {
            up = hit.collider?.GetComponent<Buildable>();

            if (up != null)
            {
                up.down = this;
                hasPower = hasPower || up.hasPower;
                roadAdjacent = roadAdjacent || up.GetComponent<Road>();

                if (recursive) up.GetAdjacent();
            }
        }
            
        if (Physics.Raycast(pos, Vector3.back, out hit, 0.75f))
        {
            down = hit.collider?.GetComponent<Buildable>();

            if (down != null)
            {
                down.up = this;
                hasPower = hasPower || down.hasPower;
                roadAdjacent = roadAdjacent || down.GetComponent<Road>();

                if (recursive) down.GetAdjacent();
            }
        }

        if (Physics.Raycast(pos, Vector3.left, out hit, 0.75f))
        {
            left = hit.collider?.GetComponent<Buildable>();

            if (left != null)
            {
                left.right = this;
                hasPower = hasPower || left.hasPower;
                roadAdjacent = roadAdjacent || left.GetComponent<Road>();

                if (recursive) left.GetAdjacent();
            }
        }

        if (Physics.Raycast(pos, Vector3.right, out hit, 0.75f))
        {
            right = hit.collider?.GetComponent<Buildable>();

            if (right != null)
            {
                right.left = this;
                hasPower = hasPower || right.hasPower;
                roadAdjacent = roadAdjacent || right.GetComponent<Road>();

                if (recursive) right.GetAdjacent();
            }
        }

        hasRoadConnection = roadAdjacent;
    }

    public virtual void Remove (Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                up = null;
                break;

            case Direction.Down:
                down = null;
                break;

            case Direction.Left:
                left = null;
                break;

            case Direction.Right:
                right = null;
                break;
        }

        GetAdjacent();
    }

    public virtual void HighlightForDeletion (Material highlightMat)
    {
        rend = GetComponent<MeshRenderer>();
        mats = rend.materials;

        deleteIndicator = new Material[mats.Length];
        for (int i = 0; i < mats.Length; ++i)
            deleteIndicator[i] = highlightMat;

        rend.materials = deleteIndicator;
    }

    public virtual void StopDeletionHighlight ()
    {
        rend.materials = mats;
    }

    // ===== On Destroy ====================================================================

    protected virtual void OnDestroy() 
    {
        BuildingManager.buildingPositions.Remove((transform.position.x, transform.position.z));

        if (up != null)
            up.Remove(Direction.Down);

        if (down != null)
            down.Remove(Direction.Up);

        if (left != null)
            left.Remove(Direction.Right);

        if (right != null)
            right.Remove(Direction.Left);
    }
}
