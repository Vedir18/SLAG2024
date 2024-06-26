using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource beatSound;
    [SerializeField] private Transform center;
    [SerializeField] private GameObject beatPrefab;
    [Space]
    [SerializeField] private BeatUI _beatUI;
    [SerializeField] private Beatmap currentBeatmap;
    private PlayerSkillManager _playerSkillManager;

    [Header("Settings")]
    [SerializeField] private float bpm;
    [SerializeField] private float maxBeatDistance;
    [SerializeField] private float timeDistanceFactor;
    [SerializeField] private float timeDistanceFactorUI;
    private float beatDuration => 60f / bpm;
    private float timeElapsed => (float)music.timeSamples / music.clip.frequency;
    [Header("Inspect")]
    [SerializeField] private List<SingleBeat> activeBeats;
    [SerializeField] private List<SingleBeat> spareBeats;
    [SerializeField] private int beatCount = 20;


    public void Initialize(PlayerSkillManager playerSkillManager)
    {
        _playerSkillManager = playerSkillManager;
        _beatUI.Initialize(beatCount+1);

        spareBeats = new List<SingleBeat>();
        for (int i = 0; i < beatCount; i++)
        {
            SingleBeat newBeat = Instantiate(beatPrefab).GetComponent<SingleBeat>();
            newBeat.gameObject.SetActive(false);
            spareBeats.Add(newBeat);
        }

        activeBeats = new List<SingleBeat>();
        SingleBeat firstBeat = Instantiate(beatPrefab).GetComponent<SingleBeat>();
        firstBeat.Initialize(0, beatDuration, timeDistanceFactor, timeDistanceFactorUI, this, 0, _beatUI.ActivateNewBeat());
        activeBeats.Add(firstBeat);
        SingleBeat lastSpawnedBeat = activeBeats[activeBeats.Count - 1];
        (int, int, int) nextBeatData = GetNextBeatID(lastSpawnedBeat);
        while (lastSpawnedBeat.transform.localPosition.x + nextBeatData.Item3 * timeDistanceFactor * beatDuration < maxBeatDistance)
        {
            var newBeat = SpawnBeat(nextBeatData.Item1, nextBeatData.Item2);
            lastSpawnedBeat = newBeat;
            nextBeatData = GetNextBeatID(newBeat);
        }
        music.PlayScheduled(AudioSettings.dspTime + 2f);
        beatSound.PlayScheduled(AudioSettings.dspTime + .5f);
        beatSound.PlayScheduled(AudioSettings.dspTime + 1f);
        beatSound.PlayScheduled(AudioSettings.dspTime + 1.5f);
    }

    private SingleBeat SpawnBeat(int beatID, int beatNumber)
    {
        var newBeat = spareBeats[spareBeats.Count - 1];
        newBeat.Initialize(beatID, beatDuration, timeDistanceFactor, timeDistanceFactorUI, this, beatNumber, _beatUI.ActivateNewBeat());
        spareBeats.Remove(newBeat);
        activeBeats.Add(newBeat);
        newBeat.MoveBeats(timeElapsed);
        newBeat.gameObject.SetActive(true);
        return newBeat;
    }

    public void ProcessTick(InputManager inputManager)
    {
        //Checking for hit beats
        if (inputManager.Clicked)
        {
            Collider[] hits = Physics.OverlapSphere(center.position, .5f);
            bool succesfull = false;
            foreach(Collider hit in hits) 
            {
                SingleBeat beatHit = hit.GetComponent<SingleBeat>();
                if (beatHit != null)
                {
                    succesfull = true;
                    beatHit.Hit();
                    BeatHit(beatHit);
                    break;
                }
            }
            if (!succesfull) _playerSkillManager.FailHit();
        }

        foreach (SingleBeat beat in activeBeats)
        {
            beat.MoveBeats(timeElapsed);
        }

        SingleBeat lastSpawnedBeat = activeBeats[activeBeats.Count - 1];
        (int, int, int) nextBeatData = GetNextBeatID(lastSpawnedBeat);
        while (lastSpawnedBeat.transform.localPosition.x + nextBeatData.Item3 * timeDistanceFactor * beatDuration < maxBeatDistance)
        {
            var newBeat = SpawnBeat(nextBeatData.Item1, nextBeatData.Item2);
            lastSpawnedBeat = newBeat;
            nextBeatData = GetNextBeatID(newBeat);
        }


    }

    public void BeatFailed(SingleBeat beat)
    {
        RestBeat(beat);
        _playerSkillManager.FailHit();
        Debug.Log($"{beat.BeatID} failed");
    }

    public void BeatHit(SingleBeat beat)
    {
        RestBeat(beat);
        _playerSkillManager.GoodHit();
        Debug.Log($"{beat.BeatID} hit");
    }

    private void RestBeat(SingleBeat beat)
    {
        beat.gameObject.SetActive(false);
        activeBeats.Remove(beat);
        spareBeats.Add(beat);
    }

    private (int, int, int) GetNextBeatID(SingleBeat lastBeat)
    {
        int beatDist = 0;
        int nextBeatID = lastBeat.BeatID;
        int nextBeatNumber = lastBeat.BeatNumber;
        do
        {
            beatDist++;
            nextBeatID++;
            nextBeatNumber++;
            if (nextBeatID == currentBeatmap.beats.Length) nextBeatID = 0;
        } while (!currentBeatmap.beats[nextBeatID]);

        return (nextBeatID, nextBeatNumber, beatDist);
    }

    internal void ReturnBeatUI(RectTransform beatUI)
    {
        _beatUI.ReturnBeat(beatUI);
    }
}
