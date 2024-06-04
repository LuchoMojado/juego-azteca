using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
