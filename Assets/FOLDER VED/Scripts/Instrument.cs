using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Instrument", menuName = "ScriptableObjects/SpawnInstrument")]
public class Instrument : ScriptableObject
{
    public Beatmap beatmap;
    public AudioClip instrumentAudio;
    public AudioClip backgroundAudio;
    
}
