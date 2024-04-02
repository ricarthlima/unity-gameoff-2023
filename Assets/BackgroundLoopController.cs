using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoopController : MonoBehaviour
{
    [SerializeField] private GameObject dungeonLoop;
    [SerializeField] private GameObject stairwayStart;
    [SerializeField] private GameObject stairwayLoop;

    public void SetDungeonLoop(){
        dungeonLoop.SetActive(true);
        stairwayStart.SetActive(false);
        stairwayLoop.SetActive(false);
    }

    public void SetStairwayStart(){
        dungeonLoop.SetActive(false);
        stairwayStart.SetActive(true);
        stairwayLoop.SetActive(false);
    }

    public void SetStairwayLoop(){
        dungeonLoop.SetActive(false);
        stairwayStart.SetActive(false);
        stairwayLoop.SetActive(true);
    }
}
