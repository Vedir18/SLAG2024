using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


public delegate void EnemyDeath();

public class Enemy : Unit
{
    // TODO: change this to animation time
    private float startedShoutingTime;
    private float animTimeToShout = 2;

    

    public event EnemyDeath OnEnemyDeath;
    [SerializeField] private Object _emptyTarget;
    [SerializeField] private float _speedWhenGoingToTheEdge = 100;
    private Vector3 _edgeTarget;
    [Header("Wave vars")]
    [SerializeField] private GameObject _buffWavePrefab;
    [SerializeField] private GameObject _debuffWavePrefab;
    [SerializeField] private float _waveLifetime = 5;
    [SerializeField] private float _waveRange = 20;
    [SerializeField] private bool _changeBrave;
    [SerializeField] private bool _changeDedicated;
    private enum EnemyState
    {
        DeadShouting,
        GoingToTheEdge,
        RegularlyShouting
    }
    private EnemyState _enemyState;
    // Start is called before the first frame update
    void Start()
    {
        _currentMotivation = 100;
        _currentDedication = baseDedicated * dedicatedMultiplier;
        _state = UnitState.ChoosingTarget;
        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();
        _speedWhenGoingToTheEdge *= dedicatedMultiplier;

        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attacked()
    {
        animator.SetTrigger("T_hit");
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
        _state = UnitState.UnitCustom;
        _enemyState = EnemyState.DeadShouting;
        startedShoutingTime = Time.time;

        // 1. Enemy shouts after death, adding anger to other enemies;
        // 2. Enemy goes to the edge of the map
        // 3. From the edge of the map, enemy regularly shouts and exhausts allies
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
        else if (_state == UnitState.UnitCustom)
        {
            Debug.Log(_enemyState);
            if(_enemyState == EnemyState.DeadShouting)
            {
                // TODO: playing animation
                if (Time.time > startedShoutingTime + animTimeToShout)
                {
                    _enemyState = EnemyState.GoingToTheEdge;
                    _edgeTarget = Manager.FindNearestEdge(_rb.transform.position);
                    Debug.Log("stopped shouting, now going to the edge");
                    SpawnBuffWave();
                }
            }
            else if (_enemyState == EnemyState.GoingToTheEdge)
            {

                GoTo(_edgeTarget, _speedWhenGoingToTheEdge);
                if(GetDistanceToTarget(_edgeTarget) < 0.1f)
                {
                    Debug.Log("Im on the edge");
                    _enemyState = EnemyState.RegularlyShouting;
                    Manager.AddShoutingEnemy(this);
                }
            }
            else if (_enemyState == EnemyState.RegularlyShouting)
            {

            }
        }
    }

    private void SpawnBuffWave()
    {
        EnemyWave newWave = Instantiate(_buffWavePrefab).GetComponent<EnemyWave>();
        newWave.Initialize(transform.position, _waveLifetime, _waveRange, _changeBrave, _changeDedicated);
    }
    public void SpawnDebuffWave()
    {
        EnemyWave newWave = Instantiate(_debuffWavePrefab).GetComponent<EnemyWave>();
        newWave.Initialize(transform.position, _waveLifetime, _waveRange, _changeBrave, _changeDedicated);
    }    
    private void ChangeTarget()
    {
        AllyTarget = null;
        _state = UnitState.ChoosingTarget;
    }

}
