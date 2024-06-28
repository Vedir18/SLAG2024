using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] protected float dedicatedMultiplier = 0.01f;
    protected float _currentDedication;

    [SerializeField] protected float _attackCooldown = 10.0f;
    protected float _lastAttack;

    public Enemy EnemyTarget;
    public Ally AllyTarget;

    protected Animator animator;
    [Header("Visual")]
    [SerializeField] private SkinnedMeshRenderer _hatRenderer;
    [SerializeField] private SkinnedMeshRenderer _bodyRenderer;
    private Material _unitMaterial1, _unitMaterial2;
    [SerializeField] private TrailRenderer _lineRenderer;


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
    private void Start()
    {
        SetMaterials();
        _currentMotivation = maxMotivation;
        _currentDedication = baseDedicated * dedicatedMultiplier;
        _state = UnitState.ChoosingTarget;
        _lastAttack = Time.time - _attackCooldown;
        _rb = GetComponent<Rigidbody>();

        Manager = FindObjectOfType<UnitManager>();
        animator = gameObject.GetComponent<Animator>();
    }

    protected void SetMaterials()
    {
        _unitMaterial1 = _bodyRenderer.material;
        _unitMaterial2 = _hatRenderer.material;
        Debug.Log($"Object: {gameObject.name} materials are 1: {_unitMaterial1} 2: {_unitMaterial2}");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"CB: {_currentBrave / 100f} | CM: {_currentMotivation / maxMotivation}");
        _unitMaterial1.SetFloat("_outline", _currentBrave / 100f);
        _unitMaterial1.SetFloat("_damaged", 1 - _currentMotivation / maxMotivation);
        _unitMaterial2.SetFloat("_outline", _currentBrave / 100f);
        _unitMaterial2.SetFloat("_damaged", 1 - _currentMotivation / maxMotivation);
        _lineRenderer.enabled = _currentMotivation > 10;

    }

    protected float GetDistanceToEnemyTarget()
    {
        return Vector3.Distance(_rb.transform.position, EnemyTarget.transform.position);
    }
    protected float GetDistanceToAllyTarget()
    {
        return Vector3.Distance(_rb.transform.position, AllyTarget.transform.position);
    }
    protected float GetDistanceToTarget(Vector3 Location)
    {
        return Vector3.Distance(_rb.transform.position, Location);
    }


    public virtual void Behave()
    {
    }

    public void GoTo(Vector3 location, float speed)
    {
        Vector3 direction = location - _rb.transform.position;
        _rb.transform.forward = direction;
        direction = direction.normalized * speed * (1 + (_currentDedication / 100f));
        _rb.velocity = new Vector3(direction.x, 2, direction.z);
    }

    protected void FixedUpdate()
    {
        Behave();
    }

    public virtual void Attacked(float damage)
    {
        ModifyMotivated(-damage);
        if (animator != null)
        {
            //animator.SetTrigger("T_hit");
        }
    }

    public void ModifyMotivated(float delta)
    {
        _currentMotivation += delta;
        _currentMotivation = Mathf.Clamp(_currentMotivation, 0, maxMotivation);
    }
    public void ModifyBrave(float delta)
    {
        _currentBrave += delta;
        _currentBrave = Mathf.Clamp(_currentBrave, 0, 100);
    }
    public void ModifyDedicated(float delta)
    {
        _currentDedication += delta;
        _currentDedication = Mathf.Clamp(_currentDedication, 0, 100);
    }
    public void Shout()
    {
        if (animator != null)
        {
            animator.SetTrigger("T_shout");
        }
    }

    public void PlayAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("T_kick");
        }
    }

}
