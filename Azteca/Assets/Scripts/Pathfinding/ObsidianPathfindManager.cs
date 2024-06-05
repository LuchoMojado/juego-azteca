using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObsidianPathfindManager : MonoBehaviour
{
    public static ObsidianPathfindManager instance;

    public List<Node> allNodes;

    public float neighborDistance;

    public LayerMask _wallLayer;

    void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
    }

    public Node FindNodeClosestToPos(Vector3 pos)
    {
        return allNodes.Where(x => !x.isBlocked).OrderBy(x => Vector3.Distance(pos, x.transform.position)).First();
    }
}
