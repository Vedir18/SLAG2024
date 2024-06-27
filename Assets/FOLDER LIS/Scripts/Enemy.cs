using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void EnemyDeath();
public class Enemy : Unit
{
    private float _health = 3;
   
    public event EnemyDeath OnEnemyDeath;
    // Start is called before the first frame update
    void Start()
    {
        _currentMotivation = 100;
        _currentDedication = baseDedicated * dedicatedMultiplier;
        _state = UnitState.ChoosingTarget;
        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attacked()
    {
        _currentMotivation = _currentMotivation - 10;
        if(_currentMotivation <= 0)
        {
            Die();
        }
    }
        
    public void Die()
    {
        Debug.Log("Enemy died. Invoking event..");
        IsDead = true;
        OnEnemyDeath?.Invoke();
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
  
    public override void Behave()
    {
        if (_state == UnitState.ChoosingTarget)
        {
            // choose target
            AllyTarget = Manager.GetNearestAlly(_rb.transform.position);
            if (AllyTarget != null)
            {
                _state = UnitState.ChasingTarget;
                AllyTarget.OnAllyDeath += ChangeTarget;
            }
            else
            {
                // We probably just won
                _state = UnitState.Idle;
            }
        }
        else if (_state == UnitState.ChasingTarget)
        {
            if (GetDistanceToAllyTarget() <= DistanceToTarget)
            {
                _state = UnitState.Attacking;
            }
            else
            {
                GoTo(AllyTarget.transform.position, _currentDedication);
            }
            // check if we already got to the target (DistToTarget)
            // if we did, change state to attacking
            // if not, keep chasing (go to our target)
        }
        else if (_state == UnitState.Attacking)
        {
            if (GetDistanceToAllyTarget() > DistanceToAttack)
            {
                _state = UnitState.ChasingTarget;
            }
            else if (Time.time > _lastAttack + _attackCooldown)
            {
                _lastAttack = Time.time;
                AllyTarget.Attacked();
                //Attack();
            }
        }
    }
    private void ChangeTarget()
    {
        AllyTarget = null;
        _state = UnitState.ChoosingTarget;
    }
}
