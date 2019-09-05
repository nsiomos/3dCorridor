using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBehaviour : MonoBehaviour
{
    public class StarfighterCreatedEventArgs : EventArgs
    {
        public Starfighter starfighter;
    }

    private static SceneBehaviour instance;
    public static SceneBehaviour Instance{get { return instance; }}

    public const int horizontalQuadrantsPerSection = 5;
    public const int verticalQuadrantsPerSection = 3;
    public const int forwardSectionsToCreate = 3;

    private EventHandler<StarfighterCreatedEventArgs> starfighterCreated;
    public event EventHandler<StarfighterCreatedEventArgs> StarfighterCreated
    {
        add { starfighterCreated += value; }
        remove { starfighterCreated -= value; }
    }

    private int lastCreatedSection = -1;

    public Section section;
    public GameObject scenePopulators;

    public LevelMover levelMover;
    public Starfighter starfighter;

    protected virtual void OnStarfighterCreated(StarfighterCreatedEventArgs e)
    {
        starfighterCreated?.Invoke(this, e);
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        levelMover = Instantiate(levelMover, Vector3.zero, Quaternion.identity);
        starfighter = Instantiate(starfighter, levelMover.transform);
        starfighter.transform.parent = levelMover.transform;
        OnStarfighterCreated(new StarfighterCreatedEventArgs() { starfighter = starfighter });
    }

    private Vector3 GetSectionCenter(int z)
    {
        return Vector3.forward * z * (2 * Section.AddedQuadrantsEachLateralDirection + 1) * Quadrant.QuadrantSize;
    }

    private int GetCurrentSection(Vector3 position)
    {
        return (int)position.z / ((2 * Section.AddedQuadrantsEachLateralDirection + 1) * Quadrant.QuadrantSize);
    }
    private void CreateSection(int z)
    {
        Section createdSection = Instantiate(section, GetSectionCenter(z), Quaternion.identity);
        createdSection.name = $"Section_{z}";

        CanyonScenePopulator scenePopulator = scenePopulators.GetComponent<CanyonScenePopulator>();
        scenePopulator.PopulateSection(createdSection);
    }

    private void HandleCreateSection()
    {
        int currentSection = GetCurrentSection(levelMover.transform.position);
        while (lastCreatedSection < currentSection + forwardSectionsToCreate)
        {
            CreateSection(lastCreatedSection + 1);
            lastCreatedSection++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        HandleCreateSection();
    }
}
