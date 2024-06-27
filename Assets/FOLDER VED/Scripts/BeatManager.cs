using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AudioSource[] backgroundAudio;
    [SerializeField] private float bufforTime;
    [SerializeField] private float backgroundVolume;

    [Header("Healing")]
    [SerializeField] private Beatmap healingBeatmap;
    [SerializeField] private AudioSource[] healingAudioBack;
    [SerializeField] private AudioSource[] healingAudioMain;

    [Header("Speed")]
    [SerializeField] private Beatmap speedBeatmap;
    [SerializeField] private AudioSource[] speedAudioBack;
    [SerializeField] private AudioSource[] speedAudioMain;

    [Header("Damage")]
    [SerializeField] private Beatmap damageBeatmap;
    [SerializeField] private AudioSource[] dmgAudioBack;
    [SerializeField] private AudioSource[] dmgAudioMain;

    [Header("PhysicalBeats")]
    [SerializeField] private Transform center;
    [SerializeField] private GameObject beatPrefab;

    [Header("UI Refs")]
    [SerializeField] private BeatUI _beatUI;
    private PlayerSkillManager _playerSkillManager;

    [Header("InputSettings")]
    [SerializeField] private float maxBeatDistance;
    [SerializeField] private float timeDistanceFactor;
    [SerializeField] private float timeDistanceFactorUI;

    [Header("MusicSettings")]
    [SerializeField] private float bpm;
    [SerializeField] private int beatsPerPhrase;



    [Header("Inspect")]
    [SerializeField] private Beatmap currentBeatmap;
    [SerializeField] private List<SingleBeat> activeBeats;
    [SerializeField] private List<SingleBeat> spareBeats;
    [SerializeField] private int beatCount = 20;
    public int CurrentLoop = 0;
    public float BeatDuration => 60f / bpm;
    public double TimeElapsed => (double)(backgroundAudio[CurrentLoop % 2].timeSamples) / backgroundAudio[CurrentLoop % 2].clip.frequency;
    public double LoopDuration => (double)healingBeatmap.beats.Length * BeatDuration;
    private int CurrentBeat => Mathf.FloorToInt((float)TimeElapsed / BeatDuration);


    public void Initialize(PlayerSkillManager playerSkillManager)
    {
        _playerSkillManager = playerSkillManager;
        _beatUI.Initialize(beatCount + 1);

        spareBeats = new List<SingleBeat>();
        for (int i = 0; i < beatCount; i++)
        {
            SingleBeat newBeat = Instantiate(beatPrefab).GetComponent<SingleBeat>();
            newBeat.gameObject.SetActive(false);
            newBeat.transform.parent = transform;
            spareBeats.Add(newBeat);
        }

        activeBeats = new List<SingleBeat>();

        SwapInstrument(0);
        backgroundAudio[0].Play();
    }

    private void ResetBeats()
    {
        foreach (SingleBeat beat in activeBeats)
        {
            beat.gameObject.SetActive(false);
            beat.FreeUI();
            spareBeats.Add(beat);
        }
        activeBeats.Clear();
    }

    private void SpawnInitialBeat()
    {
        SingleBeat initialBeat = SpawnBeat(CurrentBeat, CurrentLoop);
    }

    private SingleBeat SpawnBeat(int beatID, int beatLoop)
    {
        var newBeat = spareBeats[spareBeats.Count - 1];
        newBeat.Initialize(beatID, timeDistanceFactor, timeDistanceFactorUI, this, beatLoop, _beatUI.ActivateNewBeat());
        spareBeats.Remove(newBeat);
        activeBeats.Add(newBeat);
        newBeat.MoveBeat();
        newBeat.gameObject.SetActive(true);
        return newBeat;
    }

    public void ProcessTick(InputManager inputManager)
    {
        HandleMusicLoop();
        TryHittingABeat(inputManager.Clicked);
        MoveActiveBeats();
        SpawnMissingBeats();
    }

    private void HandleMusicLoop()
    {
        double audioTime = AudioSettings.dspTime;
        if (TimeElapsed + bufforTime > LoopDuration)
        {
            backgroundAudio[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            healingAudioBack[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            healingAudioMain[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            speedAudioBack[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            speedAudioMain[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            dmgAudioBack[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            dmgAudioMain[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
        }

        if (TimeElapsed >= LoopDuration)
        {
            CurrentLoop++;
        }
    }

    private void TryHittingABeat(bool clicked)
    {
        if (clicked)
        {
            Collider[] hits = Physics.OverlapSphere(center.position, .5f);
            bool succesfull = false;
            foreach (Collider hit in hits)
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
    }

    private void MoveActiveBeats()
    {
        foreach (SingleBeat beat in activeBeats)
        {
            beat.MoveBeat();
        }
    }

    private void SpawnMissingBeats()
    {
        SingleBeat lastSpawnedBeat = activeBeats[activeBeats.Count - 1];
        (int, int) nextBeatData = GetNextBeat(lastSpawnedBeat);

        while (SingleBeat.GetTimeLeft(nextBeatData.Item1, nextBeatData.Item2 - CurrentLoop, this) * timeDistanceFactor < maxBeatDistance)
        {
            var newBeat = SpawnBeat(nextBeatData.Item1, nextBeatData.Item2);
            nextBeatData = GetNextBeat(newBeat);
        }
    }


    public void BeatFailed(SingleBeat beat)
    {
        RestBeat(beat);
        _playerSkillManager.FailHit();
    }

    public void BeatHit(SingleBeat beat)
    {
        RestBeat(beat);
        _playerSkillManager.GoodHit();
    }

    private void RestBeat(SingleBeat beat)
    {
        beat.gameObject.SetActive(false);
        activeBeats.Remove(beat);
        spareBeats.Add(beat);
    }

    private (int, int) GetNextBeat(SingleBeat lastBeat)
    {
        int nextBeatID = lastBeat.BeatID;
        int nextBeatLoop = lastBeat.Loop;
        do
        {
            nextBeatID++;
            if (nextBeatID == currentBeatmap.beats.Length)
            {
                nextBeatLoop++;
                nextBeatID = 0;
            }
        } while (!currentBeatmap.beats[nextBeatID]);

        return (nextBeatID, nextBeatLoop);
    }

    public void ReturnBeatUI(RectTransform beatUI)
    {
        _beatUI.ReturnBeat(beatUI);
    }

    public void SwapInstrument(int instrumentID)
    {
        SilenceInstruments();

        switch (instrumentID)
        {
            case 0:
                currentBeatmap = healingBeatmap;
                healingAudioBack[0].volume = backgroundVolume;
                healingAudioBack[1].volume = backgroundVolume;
                healingAudioMain[0].volume = 1;
                healingAudioMain[1].volume = 1;
                break;
            case 1:
                currentBeatmap = speedBeatmap;
                speedAudioBack[0].volume = backgroundVolume;
                speedAudioBack[1].volume = backgroundVolume;
                speedAudioMain[0].volume = 1;
                speedAudioMain[1].volume = 1;
                break;
            case 2:
                currentBeatmap = damageBeatmap;
                dmgAudioBack[0].volume = backgroundVolume;
                dmgAudioBack[1].volume = backgroundVolume;
                dmgAudioMain[0].volume = 1;
                dmgAudioMain[1].volume = 1;
                break;
        }

        ResetBeats();
        SpawnInitialBeat();
        SpawnMissingBeats();
    }

    private void SilenceInstruments()
    {
        healingAudioBack[0].volume = 0;
        healingAudioBack[1].volume = 0;
        healingAudioMain[0].volume = 0;
        healingAudioMain[1].volume = 0;
        speedAudioBack[0].volume = 0;
        speedAudioBack[1].volume = 0;
        speedAudioMain[0].volume = 0;
        speedAudioMain[1].volume = 0;
        dmgAudioBack[0].volume = 0;
        dmgAudioBack[1].volume = 0;
        dmgAudioMain[0].volume = 0;
        dmgAudioMain[1].volume = 0;
    }
}
