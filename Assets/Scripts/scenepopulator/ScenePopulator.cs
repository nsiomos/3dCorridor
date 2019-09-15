using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScenePopulator : MonoBehaviour
{
    public void PopulateSection(Section section)
    {
        foreach (KeyValuePair<Vector3Int, Quadrant> quadrantEntry in section.GetQuadrants())
        {
            PopulateQuadrant(quadrantEntry.Value, quadrantEntry.Key.x, quadrantEntry.Key.y, quadrantEntry.Key.z);
        }
    }

    protected abstract void PopulateQuadrant(Quadrant quadrant, int x, int y, int z);
}
