using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;


public delegate void AllyDeath();
public class Ally : Unit
{
    
    public event AllyDeath OnAllyDeath;
    void Start()
    {
        _currentMotivation = maxMotivation;
        _currentDedication = baseDedicated * dedicatedMultiplier;
        Emotions.Add(Emotion.Motivated, 100.0f);
        Emotions.Add(Emotion.Brave, 100.0f);
        Emotions.Add(Emotion.Dedicated, 100.0f);
        _state = UnitState.ChoosingTarget;
        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();
    }

  
    public override void Behave()
    {
        if(_state == UnitState.ChoosingTarget)
        {
            Debug.Log("Choosing new target..");
            // choose target
            EnemyTarget = Manager.GetNearestEnemy(_rb.transform.position);
            if(EnemyTarget != null )
            {
                _state = UnitState.ChasingTarget;
                Debug.Log("Chasing new target!");
                EnemyTarget.OnEnemyDeath += ChangeTarget;
            }
            else
            {
                // We probably just won
                _state = UnitState.Idle;
            }
        }
        else if(_state == UnitState.ChasingTarget)
        {
            Debug.Log("Chasing target");
            if (GetDistanceToTarget() <= DistanceToTarget)
            {
                _state = UnitState.Attacking;
            }
            else
            {
                GoTo(EnemyTarget.transform.position, _currentDedication);
            }
            // check if we already got to the target (DistToTarget)
                // if we did, change state to attacking
                // if not, keep chasing (go to our target)
        }
        else if(_state == UnitState.Attacking)
        {
            Debug.Log("Attacking");
            if (GetDistanceToTarget() > DistanceToAttack)
            {
                _state = UnitState.ChasingTarget;
            }
            else if(Time.time > _lastAttack + _attackCooldown)
            {
                _lastAttack = Time.time;
                EnemyTarget.Attacked();
                Debug.Log("Attack");
                //Attack();
            }
            Debug.Log("End Attacking");
        }
    }

    private void ChangeTarget()
    {
        Debug.Log("Had to change target");
        EnemyTarget = null;
        _state = UnitState.ChoosingTarget;
    }
    public override void Attacked()
    {
        _currentMotivation -= 10;
        if (_currentMotivation <= 0)
    {
            OnAllyDeath?.Invoke();
        }
    }

}
