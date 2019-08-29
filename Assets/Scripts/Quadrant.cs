using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadrant : MonoBehaviour
{
    public const int QuadrantSize = 25;

    private Section parentSection;

    // Start is called before the first frame update
    void Start()
    {
        parentSection = GetComponentInParent<Section>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPositionRelativeFromCenter(Vector3 direction)
    {
        return transform.position + direction * 0.5f * QuadrantSize;
    }
}
