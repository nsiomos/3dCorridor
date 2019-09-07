using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfighterPlayerFireControl
{
    private Starfighter o;

    public StarfighterPlayerFireControl(Starfighter o)
    {
        this.o = o;
    }

    public bool CanFire(float time)
    {
        return time - o.LastFiredTime >= 1 / o.fireRate;
    }

    public void UpdateLastFiredTime(float time)
    {
        o.LastFiredTime = time;
    }
}
