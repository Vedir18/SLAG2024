using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 input;
    private Rigidbody _rb;
    private Animator _animator;
    [SerializeField] private float speed = 1;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private MovementManager movementManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerSkillManager = GetComponent<PlayerSkillManager>();
        movementManager = GetComponent<MovementManager>();

        playerSkillManager.Initialize(FindObjectOfType<BeatManager>());
        movementManager.Initialize();
        _animator = GetComponent<Animator>();
    }

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

        inputManager.UpdateInput();
        playerSkillManager.ProcessTick(inputManager);
    }

    private void FixedUpdate()
    {
        movementManager.ProcessTick(inputManager, playerSkillManager.CurrentInstrument==1&&playerSkillManager.SkillPerformance>=1);
    }
}
