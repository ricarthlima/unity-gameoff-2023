using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualBeatIndicatorController : MonoBehaviour
{
    GameController gameController;



    private float timeStart;
    private float timePassed;

    private float innerBPM;



    float bpmTime()
    {
        return (60f / innerBPM) * gameController.beatsToBeat;
    }
}
