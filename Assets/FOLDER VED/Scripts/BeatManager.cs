using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AudioSource[] backgroundAudio;
    [SerializeField] private float bufforTime;
    [SerializeField] private float backgroundVolume;
    [SerializeField] private float inactiveSkillVolume;
    [SerializeField] private float activeSkillVolume;
    [SerializeField] private float strongSkillVolume;

    [Header("Healing")]
    [SerializeField] private Sprite healingIcon;
    [SerializeField] private Beatmap healingBeatmap;
    [SerializeField] private AudioSource[] healingAudioTrack;

    [Header("Speed")]
    [SerializeField] private Sprite speedIcon;
    [SerializeField] private Beatmap speedBeatmap;
    [SerializeField] private AudioSource[] speedAudioTrack;

    [Header("Damage")]
    [SerializeField] private Sprite damageIcon;
    [SerializeField] private Beatmap damageBeatmap;
    [SerializeField] private AudioSource[] dmgAudioTrack;

    [Header("PhysicalBeats")]
    [SerializeField] private Transform center;
    [SerializeField] private GameObject beatPrefab;

    [Header("UI Refs")]
    [SerializeField] private BeatUI _beatUI;
    [SerializeField] private Image chosenInstrumentUI;
    [SerializeField] private Image spare1UI;
    [SerializeField] private Image spare2UI;
    private PlayerSkillManager _playerSkillManager;

    [Header("InputSettings")]
    [SerializeField] private float maxBeatDistance;
    [SerializeField] private float timeDistanceFactor;
    [SerializeField] private float timeDistanceFactorUI;

    [Header("MusicSettings")]
    [SerializeField] private float bpm;

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
        backgroundAudio[0].volume = backgroundVolume;
        backgroundAudio[1].volume = backgroundVolume;

        double time = AudioSettings.dspTime;
        backgroundAudio[0].PlayScheduled(time + .1f);
        healingAudioTrack[0].PlayScheduled(time + .1f);
        speedAudioTrack[0].PlayScheduled(time + .1f);
        dmgAudioTrack[0].PlayScheduled(time + .1f);
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

    public void SetActiveInstrumentVolume(int instrumentID, int skillPerformance)
    {
        float instrumentVolume = 0;
        switch (skillPerformance)
        {
            case 0:
                instrumentVolume = inactiveSkillVolume;
                break;
            case 1:
                instrumentVolume = activeSkillVolume;
                break;
            case 2:
                instrumentVolume = strongSkillVolume;
                break;
        }

        switch (instrumentID)
        {
            case 0:
                healingAudioTrack[0].volume = instrumentVolume;
                healingAudioTrack[1].volume = instrumentVolume;
                break;
            case 1:
                speedAudioTrack[0].volume = instrumentVolume;
                speedAudioTrack[1].volume = instrumentVolume;
                break;
            case 2:
                dmgAudioTrack[0].volume = instrumentVolume;
                dmgAudioTrack[1].volume = instrumentVolume;
                break;
        }
    }

    private void HandleMusicLoop()
    {
        double audioTime = AudioSettings.dspTime;
        if (TimeElapsed + bufforTime > LoopDuration)
        {
            backgroundAudio[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            healingAudioTrack[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            speedAudioTrack[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
            dmgAudioTrack[(CurrentLoop + 1) % 2].PlayScheduled(audioTime + TimeElapsed + bufforTime - LoopDuration);
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
                healingAudioTrack[0].volume = backgroundVolume;
                healingAudioTrack[1].volume = backgroundVolume;
                chosenInstrumentUI.sprite = healingIcon;
                spare1UI.sprite = speedIcon;
                spare2UI.sprite = damageIcon;
                break;
            case 1:
                currentBeatmap = speedBeatmap;
                speedAudioTrack[0].volume = backgroundVolume;
                speedAudioTrack[1].volume = backgroundVolume;
                chosenInstrumentUI.sprite = speedIcon;
                spare1UI.sprite = damageIcon;
                spare2UI.sprite = healingIcon;
                break;
            case 2:
                currentBeatmap = damageBeatmap;
                dmgAudioTrack[0].volume = backgroundVolume;
                dmgAudioTrack[1].volume = backgroundVolume;
                chosenInstrumentUI.sprite = damageIcon;
                spare1UI.sprite = healingIcon;
                spare2UI.sprite = speedIcon;
                break;
        }

        ResetBeats();
        SpawnInitialBeat();
        SpawnMissingBeats();
    }

    private void SilenceInstruments()
    {
        healingAudioTrack[0].volume = 0;
        healingAudioTrack[1].volume = 0;
        speedAudioTrack[0].volume = 0;
        speedAudioTrack[1].volume = 0;
        dmgAudioTrack[0].volume = 0;
        dmgAudioTrack[1].volume = 0;
    }
}
