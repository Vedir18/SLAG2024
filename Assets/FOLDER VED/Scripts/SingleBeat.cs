using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBeat : MonoBehaviour
{
    [SerializeField] public int BeatID;
    [SerializeField] public int BeatNumber;
    [SerializeField] private float _beatDuration;
    private float _timeDistanceScale;
    private float _timeDistanceScaleUI;
    private Transform _transform;
    private BeatManager _beatManager;
    private RectTransform _beatUI;

    public void Initialize(int beatID, float beatDuration, float timeDistanceScale, float timeDistanceScaleUI, BeatManager beatManager, int beatNumber, RectTransform beatUI)
    {
        _transform = transform;
        BeatID = beatID;
        BeatNumber = beatNumber;
        _beatDuration = beatDuration;
        _timeDistanceScale = timeDistanceScale;
        _timeDistanceScaleUI = timeDistanceScaleUI;
        _beatManager = beatManager;
        _beatUI = beatUI;
    }

    public void MoveBeats(float elapsedTime)
    {
        float timeLeft = _beatDuration * BeatNumber - elapsedTime;
        float dist = timeLeft * _timeDistanceScale;
        float distUI = timeLeft * _timeDistanceScaleUI;
        _transform.localPosition = new Vector3(dist, 0, 0);
        _beatUI.anchoredPosition3D = new Vector3(distUI, 0, 0);
        //Debug.Log($"Beat {BeatID}:{BeatNumber} moved | TE: {elapsedTime} | BT: {_beatDuration * BeatNumber} | --: {timeLeft}");
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
}
