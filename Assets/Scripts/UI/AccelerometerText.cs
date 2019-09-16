using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarfighterDefinitions;

public class AccelerometerText : MonoBehaviour
{
    private TextMeshProUGUI accelerometerText;

    private void UpdateText(Starfighter o)
    {
        accelerometerText.text = Mathf.Floor(o.accelerometer.Value) + "%";
    }

    private void SceneBehaviour_StarfighterCreated(object sender, SceneBehaviour.StarfighterCreatedEventArgs e)
    {
        e.starfighter.AccelerometerChanged += Starfighter_OnAccelerometerChanged;
        UpdateText(e.starfighter);
    }

    private void Starfighter_OnAccelerometerChanged(object sender, AccelerometerChangedEventArgs e)
    {
        Starfighter o = (Starfighter)sender;
        UpdateText(o);
    }

    private void Awake()
    {
        accelerometerText = GetComponent<TextMeshProUGUI>();
        SceneBehaviour.Instance.StarfighterCreated += SceneBehaviour_StarfighterCreated;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
