using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void MakeHorizontal(ref this Vector3 vector)
    {
        vector = new Vector3(vector.x, 0, vector.z);
    }
}
