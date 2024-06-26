using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    private InputManager inputManager;
    private BeatManager beatManager;
    private int skillPerformance = 0;
    public int SkillPerformance => skillPerformance;
    private int goodHits = 0;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        beatManager = GetComponent<BeatManager>();
        beatManager.Initialize(this);
    }

    void Update()
    {
        inputManager.UpdateInput();
        beatManager.ProcessTick(inputManager);
    }

    public void GoodHit()
    {
        goodHits += 1;
        if(goodHits >=2)
        {
            goodHits = 0;
            skillPerformance++;
            if(skillPerformance > 2) skillPerformance = 2;
        }

        Debug.Log($"SkillPerf: {skillPerformance}");
    }

    public void FailHit()
    {
        goodHits = 0;
        skillPerformance -= 1;
        if(skillPerformance < 0) skillPerformance = 0;
        Debug.Log($"SkillPerf: {skillPerformance}");
    }

}
