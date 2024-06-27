using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
<<<<<<< HEAD
    private Vector2 input;
    private Rigidbody _rb;
    private Animator _animator;
    [SerializeField] private float speed = 1;
=======
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private MovementManager movementManager;
>>>>>>> 3609426cd90e25ab1bf7019b10d648fbb8e0b88c

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerSkillManager = GetComponent<PlayerSkillManager>();
        movementManager = GetComponent<MovementManager>();

        playerSkillManager.Initialize(FindObjectOfType<BeatManager>());
        movementManager.Initialize();
    }

    void Update()
    {
<<<<<<< HEAD
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
=======
        inputManager.UpdateInput();
        playerSkillManager.ProcessTick(inputManager);
>>>>>>> 3609426cd90e25ab1bf7019b10d648fbb8e0b88c
    }

    private void FixedUpdate()
    {
        movementManager.ProcessTick(inputManager);
    }
}
