using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{

    [Header("Dungeon")]
    [SerializeField] private int dBPM;
    [SerializeField] private float dBeatsToPortal;
    [SerializeField] private float dInitialHorizonalRange;
    [SerializeField] private float dPortalsToEnd;

    [Header("Stairway")]
    [SerializeField] private int stairwayInitialBPM;

    [Header("Throne")]
    [SerializeField] private int throneInitialBPM;

    public int BPM(TowerLevel towerLevel)
    {
        switch (towerLevel)
        {
            case TowerLevel.dungeon: return dBPM;
            case TowerLevel.stairway: return stairwayInitialBPM;
            case TowerLevel.throne: return throneInitialBPM;
            default: return 60;
        }
    }

    public float GetBeatsToPortal(TowerLevel level)
    {
        switch (level)
        {
            case TowerLevel.dungeon: return dBeatsToPortal;
            case TowerLevel.stairway: return 1;
            case TowerLevel.throne: return 1;
            default: return 1;
        }
    }

    public float GetInitialHorizontalRange(TowerLevel towerLevel)
    {
        switch (towerLevel)
        {
            case TowerLevel.dungeon: return dInitialHorizonalRange;
            case TowerLevel.stairway: return 5;
            case TowerLevel.throne: return 5;
            default: return 5;
        }
    }

    public float GetPortalsToEnd(TowerLevel towerLevel)
    {
        switch (towerLevel)
        {
            case TowerLevel.dungeon: return dPortalsToEnd;
            case TowerLevel.stairway: return 5;
            case TowerLevel.throne: return 5;
            default: return 5;
        }
    }
}
