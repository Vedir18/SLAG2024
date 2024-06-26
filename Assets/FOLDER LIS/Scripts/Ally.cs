using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.VersionControl;
using UnityEngine;

public class Ally : Unit
{
    
    [SerializeField] private float maxMotivation = 100;
    private float _currentMotivation;

    [SerializeField] private float baseBrave = 50;
    [SerializeField] private float braveMultiplier = 100;
    private float _currentBrave;

    [SerializeField] private float baseDedicated = 100;
    [SerializeField] private float dedicatedMultiplier = 0.1f;
    private float _currentDedication;

    [SerializeField] private float _attackCooldown = 10.0f;
    private float _lastAttack;
    public enum Emotion
    {
        Motivated,
        Brave,
        Dedicated
    }
    public enum AllyState
    {
        Idle,
        Exhausted,
        ChoosingTarget,
        ChasingTarget,
        Attacking
    }
    private AllyState _state = AllyState.Idle;

    private Dictionary<Emotion, float> Emotions = new Dictionary<Emotion, float>();

    // Start is called before the first frame update
    void Start()
    {
        _currentMotivation = maxMotivation;
        _currentDedication = baseDedicated * dedicatedMultiplier;
        Emotions.Add(Emotion.Motivated, 100.0f);
        Emotions.Add(Emotion.Brave, 100.0f);
        Emotions.Add(Emotion.Dedicated, 100.0f);
        _state = AllyState.ChoosingTarget;
        Target = Manager.GetNearestEnemy(_rb.transform.position);
        if(Target != null)
        {
            _state = AllyState.ChasingTarget;
        }

        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();
    }

    private float GetDistanceToTarget()
    {
        return Vector3.Distance(_rb.transform.position, Target.position);
    }
    public override void Behave()
    {
        if(_state == AllyState.ChoosingTarget)
        {
            // choose target
            Debug.Log("Choosing target...");

            Target = Manager.GetNearestEnemy(_rb.transform.position);
            if(Target != null )
            {
                _state = AllyState.ChasingTarget;
            }
            else
            {
                // We probably just won
                _state = AllyState.Idle;
            }
        }
        else if(_state == AllyState.ChasingTarget)
        {
            if(GetDistanceToTarget() <= DistanceToTarget)
            {
                _state = AllyState.Attacking;
            }
            else
            {
                GoTo(Target.transform.position, _currentDedication);
            }
            // check if we already got to the target (DistToTarget)
                // if we did, change state to attacking
                // if not, keep chasing (go to our target)
        }
        else if(_state == AllyState.Attacking)
        {
            if(GetDistanceToTarget() > DistanceToAttack)
            {
                _state = AllyState.ChasingTarget;
            }
            else if(Time.time > _lastAttack + _attackCooldown)
            {
                _lastAttack = Time.time;
                Debug.Log("Attack");
                //Attack();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Behave();
    }
}
