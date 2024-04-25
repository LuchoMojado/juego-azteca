using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Changes Y value of a vector to 0
    /// </summary>
    public static void MakeHorizontal(ref this Vector3 vector)
    {
        vector = new Vector3(vector.x, 0, vector.z);
    }

    /// <summary>
    /// Changes Y value of a quaternion by 'amount'
    /// </summary>
    public static Quaternion AddYRotation(this Quaternion quaternion, float amount)
    {
        var euler = quaternion.eulerAngles;
        return Quaternion.Euler(euler.x, euler.y + amount, euler.z);
    }
}
