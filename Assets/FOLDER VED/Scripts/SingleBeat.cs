using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBeat : MonoBehaviour
{
    public int BeatID;
    public int Loop;
    private float _timeDistanceScale;
    private float _timeDistanceScaleUI;
    private Transform _transform;
    private BeatManager _beatManager;
    private RectTransform _beatUI;

    public void Initialize(int beatID, float timeDistanceScale, float timeDistanceScaleUI, BeatManager beatManager, int loop, RectTransform beatUI)
    {
        _transform = transform;
        BeatID = beatID;
        Loop = loop;
        _timeDistanceScale = timeDistanceScale;
        _timeDistanceScaleUI = timeDistanceScaleUI;
        _beatManager = beatManager;
        _beatUI = beatUI;
    }

    public void MoveBeat()
    {
        float timeLeft = GetTimeLeft(BeatID, Loop - _beatManager.CurrentLoop, _beatManager);
        float dist = timeLeft * _timeDistanceScale;
        float distUI = timeLeft * _timeDistanceScaleUI;
        _transform.localPosition = new Vector3(dist, 0, 0);
        _beatUI.anchoredPosition3D = new Vector3(distUI, 0, 0);
    }

    public static float GetTimeLeft(int beatID, int loopDif, BeatManager beatManager)
    {
        return (float)beatManager.BeatDuration * beatID + (float)beatManager.LoopDuration * loopDif - (float)beatManager.TimeElapsed;
    }

    public void Fail()
    {
        _beatManager.BeatFailed(this);
        _beatManager.ReturnBeatUI(_beatUI);
    }

    public void Hit()
    {
        _beatManager.ReturnBeatUI(_beatUI);
    }

    public void FreeUI()
    {
        _beatManager.ReturnBeatUI(_beatUI);
    }
}
