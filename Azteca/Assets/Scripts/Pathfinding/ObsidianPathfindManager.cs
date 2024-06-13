using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObsidianPathfindManager : MonoBehaviour
{
    public static ObsidianPathfindManager instance;

    [HideInInspector] public List<Node> allNodes = new List<Node>();

    [SerializeField] Node _node;

    [SerializeField] Vector3 _centerNodePos;

    [SerializeField] int _longestRowLength, _shortestRowLength;
    [SerializeField] float _nodeDistance;

    public float neighborDistance;

    public LayerMask _wallLayer;

    void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;

        GenerateNodeGrid();
    }

    public void GenerateNodeGrid()
    {
        for (int k = 0; k < _longestRowLength; k++)
        {
            for (int j = 0; j < _shortestRowLength; j++)
            {
                for (int i = 0; i < _longestRowLength; i++)
                {
                    var xMult = i;
                    var zMult = j;

                    if (xMult % 2 != 0) xMult *= -1;
                    if (zMult % 2 != 0) zMult *= -1;

                    xMult = Mathf.FloorToInt(xMult * 0.5f);
                    zMult = Mathf.FloorToInt(zMult * 0.5f);

                    allNodes.Add(Instantiate(_node, _centerNodePos + new Vector3(_nodeDistance * xMult, 0, _nodeDistance * zMult), Quaternion.identity));
                }
            }
        }
    }

    public Node FindNodeClosestTo(Vector3 pos)
    {
        return allNodes.Where(x => !x.isBlocked).OrderBy(x => Vector3.Distance(pos, x.transform.position)).First();
    }
}
