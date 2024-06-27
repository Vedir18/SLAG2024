using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatUI : MonoBehaviour
{
    [SerializeField] private GameObject beatUIPrefab;
    private List<RectTransform> activeBeats;
    private List<RectTransform> spareBeats;
    private int _beatCount;

    public void Initialize(int beatCount)
    {
        _beatCount = beatCount;

        spareBeats = new List<RectTransform>();
        for(int i = 0; i < _beatCount; i++)
        {
            RectTransform newBeat = Instantiate(beatUIPrefab).GetComponent<RectTransform>();
            newBeat.SetParent(transform);
            newBeat.gameObject.SetActive(false);
            spareBeats.Add(newBeat);
        }

        activeBeats = new List<RectTransform>();
    }

    public RectTransform ActivateNewBeat()
    {
        RectTransform newBeat = spareBeats[spareBeats.Count - 1];
        activeBeats.Add(newBeat);
        newBeat.gameObject.SetActive(true);
        spareBeats.Remove(newBeat);
        return newBeat;
    }

    public void ReturnBeat(RectTransform beat)
    {
        beat.gameObject.SetActive(false);
        activeBeats.Remove(beat);
        spareBeats.Add(beat);
    }
}
