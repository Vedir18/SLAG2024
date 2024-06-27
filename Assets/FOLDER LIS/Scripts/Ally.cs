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
        SetMaterials();
        _currentMotivation = maxMotivation;
        _currentDedication = baseDedicated * dedicatedMultiplier;
        _state = UnitState.ChoosingTarget;
        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();
    }

  
    public override void Behave()
    {
        if(_state == UnitState.ChoosingTarget)
        {
            // choose target
            EnemyTarget = Manager.GetNearestEnemy(_rb.transform.position);
            if(EnemyTarget != null )
            {
                _state = UnitState.ChasingTarget;
                EnemyTarget.OnEnemyDeath += ChangeTarget;
            }
            else
            {
                // We probably just won
                _state = UnitState.Idle;
                _rb.velocity = Vector3.zero; ;
            }
        }
        else if(_state == UnitState.ChasingTarget)
        {
            if (GetDistanceToEnemyTarget() <= DistanceToTarget)
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
            if (GetDistanceToEnemyTarget() > DistanceToAttack)
            {
                _state = UnitState.ChasingTarget;
            }
            else if(Time.time > _lastAttack + _attackCooldown)
            {
                _lastAttack = Time.time;
                EnemyTarget.Attacked();
                //Attack();
            }
        }
    }

    private void ChangeTarget()
    {
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
