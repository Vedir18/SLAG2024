using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private Rigidbody _rb;
    private Animator _animator;
    [SerializeField] private float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        input = new Vector2(x, y);
        input.Normalize();
        if(input.magnitude > 0.01f)
        {
            _animator.SetBool("B_iswalking", true);
        }
        else
        {
            _animator.SetBool("B_iswalking", false);
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector3(input.x, 0, input.y) * speed;
    }
}
