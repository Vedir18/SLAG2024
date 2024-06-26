using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Beatmap", menuName = "ScriptableObjects/SpawnBeatmap", order = 1)]
public class Beatmap : ScriptableObject
{
    public bool[] beats;
}
