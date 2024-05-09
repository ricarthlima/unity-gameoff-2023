using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{

    [Header("Dungeon")]
    [SerializeField] private int dungeonInitialBPM;
    [SerializeField] private float dungeonInitialHorizontalRange;

    [SerializeField] private float dungeonMaxHorizontalRange;

    [Header("Stairway")]
    [SerializeField] private int stairwayInitialBPM;

    [Header("Throne")]
    [SerializeField] private int throneInitialBPM;

    public int GetInitialBPM(TowerLevel towerLevel)
    {
        switch (towerLevel)
        {
            case TowerLevel.dungeon: return dungeonInitialBPM;
            case TowerLevel.stairway: return stairwayInitialBPM;
            case TowerLevel.throne: return throneInitialBPM;
            default: return 60;
        }
    }

    public float GetInitialHorizontalRange(TowerLevel towerLevel)
    {
        switch (towerLevel)
        {
            case TowerLevel.dungeon: return dungeonInitialHorizontalRange;
            case TowerLevel.stairway: return 5;
            case TowerLevel.throne: return 5;
            default: return 5;
        }
    }

    public float GetMaxHorizontalRange(TowerLevel towerLevel)
    {
        switch (towerLevel)
        {
            case TowerLevel.dungeon: return dungeonMaxHorizontalRange;
            case TowerLevel.stairway: return 10;
            case TowerLevel.throne: return 10;
            default: return 10;
        }
    }
}
