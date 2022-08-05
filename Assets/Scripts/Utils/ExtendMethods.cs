using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendMethods
{
    private const float dotThrashold = 0.5f;
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var targetDir = target.position - transform.position;
        targetDir.Normalize();

        float dot = Vector3.Dot(transform.forward,targetDir);
        return dot >= dotThrashold;
    }
}
