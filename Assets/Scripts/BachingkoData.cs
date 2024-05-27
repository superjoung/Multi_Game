using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BachingkoData : MonoBehaviour
{
    public int index;
    public string description;
    public Vector3 firstPos;

    private void Awake()
    {
        firstPos = transform.position;
    }
}
