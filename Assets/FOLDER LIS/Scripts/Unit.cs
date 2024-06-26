using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitManager Manager;
    protected Rigidbody _rb;
    public bool IsDead = false;
    // distance to target should be smaller
    [SerializeField] public float DistanceToTarget = 4.0f;
    [SerializeField] public float DistanceToAttack = 5.0f;
    protected UnitState _state = UnitState.Idle;

    [SerializeField] protected float maxMotivation = 100;
    protected float _currentMotivation;

    [SerializeField] protected float baseBrave = 50;
    [SerializeField] protected float braveMultiplier = 100;
    protected float _currentBrave;

    [SerializeField] protected float baseDedicated = 100;
    [SerializeField] protected float dedicatedMultiplier = 0.1f;
    protected float _currentDedication;

    [SerializeField] protected float _attackCooldown = 10.0f;
    protected float _lastAttack;

    public Enemy EnemyTarget;

    protected Dictionary<Emotion, float> Emotions = new Dictionary<Emotion, float>();
    public enum Emotion
    {
        Motivated,
        Brave,
        Dedicated
    }

    protected enum UnitState
    {
        Idle,
        Exhausted,
        ChoosingTarget,
        ChasingTarget,
        Attacking,
        UnitCustom
    }


    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
    }
    protected float GetDistanceToTarget()
    {
        return Vector3.Distance(_rb.transform.position, EnemyTarget.transform.position);
    }

    public virtual void Behave()
    {
    }

    public void GoTo(Vector3 location, float speed)
    {
        
        Vector3 direction = location - _rb.transform.position;
        direction = direction.normalized * speed;
        _rb.velocity = new Vector3(direction.x, 2, direction.z);
    }

    protected void FixedUpdate()
    {
        Behave();
    }

    public virtual void Attacked()
    {
        
    }

}
