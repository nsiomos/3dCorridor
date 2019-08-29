using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CanyonScenePopulator : ScenePopulator{

    public Transform groundPlane;
    public Transform cliff;

    private void CreateGroundPlane(Quadrant quadrant)
    {
        Transform createdGroundPlane = Instantiate(groundPlane, quadrant.transform.position, Quaternion.identity);
        createdGroundPlane.parent = quadrant.transform;
    }

    private void CreateCliffs(Transform quadrantCenter, Vector3 lookDirection)
    {/*
        for (int ey = SectionUtils.BottomQuadrantElement; ey <= SectionUtils.TopQuadrantElement; ey++)
        {
            for (int ez = SectionUtils.BackQuadrantElement; ez <= SectionUtils.FrontQuadrantElement; ez++)
            {
                Transform createdCliff = Instantiate(cliff, quadrantCenter.position + SectionUtils.GetQuadrantElementCenter(0, ey, ez), Quaternion.LookRotation(lookDirection, Vector3.up));
                createdCliff.parent = quadrantCenter;
            }
        }*/
    }

    protected override void PopulateQuadrant(Quadrant quadrant, int x, int y, int z)
    {
        if (y >= Section.TopQuadrant)
        {
            return;
        }

        if (y == Section.BottomQuadrant)
        {
            CreateGroundPlane(quadrant);
        }

        if (x == Section.LeftQuadrant)
        {
            //CreateCliffs(quadrantCenter, Vector3.right);
        }
        else if (x == Section.RightQuadrant)
        {
            //CreateCliffs(quadrantCenter, Vector3.left);
        }
    }
}
