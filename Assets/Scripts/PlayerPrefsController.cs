using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : MonoBehaviour
{
    [Header("Keys")]
    private const string keySoundBGM = "BGM";
    private const string keySoundMTR = "MTR";
    private const string keySoundSFX = "SFX";
    private const string keyRecHigh = "REC_HIGH";
    private const string keyRecTime = "REC_TIME";
    private const string keyRecPortalsDungeon = "REC_PORTALS_DUNGEON";

    private float soundBGM;
    private float soundMTR;
    private float soundSFX;
    private float recHigh;
    private float recTime;
    private int recPortalsDungeon;

    void Start()
    {
        InitializeAll();
    }

    private void InitializeAll()
    {
        soundBGM = PlayerPrefs.GetFloat(key: keySoundBGM, 1);
        soundMTR = PlayerPrefs.GetFloat(key: keySoundMTR, 1);
        soundSFX = PlayerPrefs.GetFloat(key: keySoundSFX, 1);
        recHigh = PlayerPrefs.GetFloat(key: keyRecHigh, 0);
        recTime = PlayerPrefs.GetFloat(key: keyRecTime, 0);
        recPortalsDungeon = PlayerPrefs.GetInt(key: keyRecPortalsDungeon, 0);
    }

    public void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        InitializeAll();
    }

    public float SoundBGM
    {
        get
        {
            return soundBGM;
        }
        set
        {
            soundBGM = value;
            PlayerPrefs.SetFloat(keySoundBGM, value);
        }
    }

    public float SoundMTR
    {
        get
        {
            return soundMTR;
        }
        set
        {
            soundMTR = value;
            PlayerPrefs.SetFloat(keySoundMTR, value);
        }
    }

    public float SoundSFX
    {
        get
        {
            return soundSFX;
        }
        set
        {
            soundSFX = value;
            PlayerPrefs.SetFloat(keySoundSFX, value);
        }
    }

    public float RecHigh
    {
        get
        {
            return recHigh;
        }
        set
        {
            recHigh = value;
            PlayerPrefs.SetFloat(keyRecHigh, value);
        }
    }

    public float RecTime
    {
        get
        {
            return recTime;
        }
        set
        {
            recTime = value;
            PlayerPrefs.SetFloat(keyRecTime, value);
        }
    }

    public int RecPortalsDungeon
    {
        get
        {
            return recPortalsDungeon;
        }
        set
        {
            recPortalsDungeon = value;
            PlayerPrefs.SetInt(keyRecPortalsDungeon, value);
        }
    }
}
