using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRouteManager : MonoBehaviour
{
    public List<Transform> patrolRoutePoints;

    // Start is called before the first frame update
    void Awake()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        for (var index = 1; index < children.Length; index++)
        {
            Transform childTransform = children[index];
            patrolRoutePoints.Add(childTransform);
        }
        
        
    }
}
