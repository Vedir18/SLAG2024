using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitManager Manager;
    private Transform _tr;
    float TimeT = 0;
    protected Rigidbody _rb;
    private float _speed = 1;
    public Transform Target;
    bool Updated = false;
    private UnitState _unitState = UnitState.Idle;
    // distance to target should be smaller
    [SerializeField] public float DistanceToTarget = 4.0f;
    [SerializeField] public float DistanceToAttack = 5.0f;

    public enum UnitState
    {
        Idle,
        Exhausted,
        ChoosingTarget,
        ChasingTarget,
        Attacking
    }


    // Start is called before the first frame update
    void Start()
    {
        _tr = transform;
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        TimeT += Time.deltaTime;

        if (TimeT > 2.0f && !Updated)
        {
            SetChasing(FindObjectOfType<PlayerController>().transform);
            Updated = true;
        }
        if (_unitState == UnitState.ChasingTarget)
        {
            GoTo(Target.position, _speed);
        }
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

    private void FixedUpdate()
    {
        
    }

    private void SetChasing(Transform target)
    {
        _unitState = UnitState.ChasingTarget;
        Target = target;
    }

}
