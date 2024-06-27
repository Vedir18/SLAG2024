using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    [Header("Skill vars")]
    [SerializeField] private float skillRange;
    [SerializeField] private float inactiveAlpha;
    [SerializeField] private float activeAlpha;
    [SerializeField] private float strongAlpha;
    [SerializeField] private Color healColor;
    [SerializeField] private float healSmall;
    [SerializeField] private float healBig;
    [SerializeField] private Color speedColor;
    [SerializeField] private float speedSmall;
    [SerializeField] private float speedBig;
    [SerializeField] private Color damageColor;
    [SerializeField] private float damageSmall;
    [SerializeField] private float damageBig;
    [SerializeField] private GameObject skillVisual;
    private Material skillMaterial;

    [SerializeField] private float boredomSpeed;
    private float healingExciteMeter, speedExciteMeter, damageExciteMeter;
    private int currentInstrument = 0;
    private BeatManager beatManager;
    private int skillPerformance = 0;
    public int SkillPerformance => skillPerformance;
    private int goodHits = 0;

    public void Initialize(BeatManager beatManager)
    {
        this.beatManager = beatManager;
        this.beatManager.Initialize(this);
        skillMaterial = skillVisual.GetComponent<MeshRenderer>().material;
    }

    public void ProcessTick(InputManager inputManager)
    {
        SwapInstrument(inputManager.InstrumentClicked);
        beatManager.ProcessTick(inputManager);
        ProcessSkill();
    }

    private void ModifySkillPerformance(int delta)
    {
        skillPerformance += delta;
        skillPerformance = Mathf.Clamp(skillPerformance, 0, 2);

        float skillAlpha = 1;
        switch (skillPerformance)
        {
            case 0:
                skillAlpha = inactiveAlpha; break;
            case 1:
                skillAlpha = activeAlpha; break;
            case 2:
                skillAlpha = strongAlpha; break;
        }
        skillMaterial.color = new Vector4(skillMaterial.color.r, skillMaterial.color.g, skillMaterial.color.b, skillAlpha);
    }

    public void GoodHit()
    {
        goodHits += 1;
        if (goodHits >= 2)
        {
            goodHits = 0;
            ModifySkillPerformance(1);
        }

        Debug.Log($"SkillPerf: {skillPerformance}");
    }

    public void FailHit()
    {
        goodHits = 0;
        ModifySkillPerformance(-1);
        Debug.Log($"SkillPerf: {skillPerformance}");
    }

    private void SwapInstrument(int instrumentClick)
    {
        if (instrumentClick != -1 && currentInstrument != instrumentClick)
        {
            currentInstrument = instrumentClick;
            skillPerformance = 0;
            beatManager.SwapInstrument(currentInstrument);
            Color skillColor = Color.white;
            switch(instrumentClick)
            {
                case 0:
                    skillColor = healColor;
                    break;
                case 1:
                    skillColor = speedColor;
                    break;
                case 2:
                    skillColor = damageColor;
                    break;
            }
            skillColor.a = inactiveAlpha;
            skillMaterial.color = skillColor;
        }
    }

    private void ProcessSkill()
    {
        if (skillPerformance > 0)
        {
            Ally[] affected = FindAffectedAllies();
            switch(currentInstrument)
            {
                case 0:
                    foreach (Ally ally in affected) ally.ModifyMotivated((skillPerformance == 1 ? healSmall : healBig) * Time.deltaTime);
                    break;
                case 1:
                    foreach (Ally ally in affected) ally.ModifyDedicated((skillPerformance == 1 ? speedSmall : speedBig) * Time.deltaTime);
                    break;
                case 2:
                    foreach (Ally ally in affected) ally.ModifyBrave((skillPerformance == 1 ? damageSmall : damageBig) * Time.deltaTime);
                    break;
            }
            Debug.Log($"ALLIES AFFECTED: {affected.Length}");
        }
    }

    private Ally[] FindAffectedAllies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, skillRange);
        List<Ally> hitAllies = new List<Ally>();
        foreach (Collider collider in hits)
        {
            Ally potential = collider.GetComponent<Ally>();
            if (potential != null)
            {
                hitAllies.Add(potential);
            }
        }

        return hitAllies.ToArray();
    }
}
