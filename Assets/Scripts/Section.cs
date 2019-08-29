using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public const int AddedQuadrantsEachHorizontalDirection = 2;
    public const int AddedQuadrantsEachVerticalDirection = 1;
    public const int AddedQuadrantsEachLateralDirection = 1;
    public const int HorizontalQuadrantsCount = 2 * Section.AddedQuadrantsEachHorizontalDirection + 1;
    public const int VerticalQuadrantsCount = 2 * Section.AddedQuadrantsEachVerticalDirection + 1;
    public const int LateralQuadrantsCount = 2 * Section.AddedQuadrantsEachLateralDirection + 1;

    public const int LeftQuadrant = -Section.AddedQuadrantsEachHorizontalDirection;
    public const int RightQuadrant = Section.AddedQuadrantsEachHorizontalDirection;
    public const int BottomQuadrant = -Section.AddedQuadrantsEachVerticalDirection;
    public const int TopQuadrant = Section.AddedQuadrantsEachVerticalDirection;
    public const int BackQuadrant = -Section.AddedQuadrantsEachLateralDirection;
    public const int FrontQuadrant = Section.AddedQuadrantsEachLateralDirection;

    private Dictionary<Vector3Int, Quadrant> quadrants = new Dictionary<Vector3Int, Quadrant>();
    public Quadrant quadrant;

    private Vector3 GetQuadrantCenter(int x, int y, int z)
    {
        return new Vector3(x, y, z) * Quadrant.QuadrantSize;
    }

    private void GenerateQuadrants()
    {
        for (int x = LeftQuadrant; x <= RightQuadrant; x++)
        {
            for (int y = BottomQuadrant; y <= TopQuadrant; y++)
            {
                for (int z = BackQuadrant; z <= FrontQuadrant; z++)
                {
                    Quadrant createdQuadrant = Instantiate(quadrant, transform.position + GetQuadrantCenter(x, y, z), Quaternion.identity);
                    createdQuadrant.transform.parent = transform;
                    createdQuadrant.name = $"Quadrant({x}, {y}, {z})";
                    quadrants.Add(new Vector3Int(x, y, z), createdQuadrant);
                }
            }
        }
    }

    void Awake()
    {
        GenerateQuadrants();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<Vector3Int, Quadrant> GetQuadrants()
    {
        return quadrants;
    }

    public Quadrant GetQuadrant(int x, int y, int z)
    {
        return quadrants[new Vector3Int(x, y, z)];
    }
}
