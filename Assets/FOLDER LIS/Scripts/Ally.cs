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
        _currentBrave = 0;
        _currentMotivation = maxMotivation;
        _currentDedication = baseDedicated;
        _state = UnitState.ChoosingTarget;
        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();

        animator = gameObject.GetComponent<Animator>();
    }


    public override void Behave()
    {
        if (_state == UnitState.ChoosingTarget)
        {
            animator.SetBool("B_iswalking", false);
            // choose target
            EnemyTarget = Manager.GetNearestEnemy(_rb.transform.position);
            if (EnemyTarget != null)
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
        else if (_state == UnitState.ChasingTarget)
        {
            animator.SetBool("B_iswalking", true);
            if (GetDistanceToEnemyTarget() <= DistanceToTarget)
            {
                _state = UnitState.Attacking;
            }
            else
            {
                GoTo(EnemyTarget.transform.position, 1);
            }
            // check if we already got to the target (DistToTarget)
            // if we did, change state to attacking
            // if not, keep chasing (go to our target)
        }
        else if (_state == UnitState.Attacking)
        {
            if (GetDistanceToEnemyTarget() > DistanceToAttack)
            {
                _state = UnitState.ChasingTarget;
            }
            else if (Time.time > _lastAttack + _attackCooldown)
            {
                _lastAttack = Time.time;
                EnemyTarget.Attacked(_currentBrave/5 + baseBrave);
                animator.SetTrigger("T_kick");
                //Attack();
            }
        }
    }

    private void ChangeTarget()
    {
        EnemyTarget = null;
        _state = UnitState.ChoosingTarget;
    }
    public override void Attacked(float damage)
    {
        ModifyMotivated(-damage);
        if (_currentMotivation <= 0)
        {
            OnAllyDeath?.Invoke();
        }
    }

}
