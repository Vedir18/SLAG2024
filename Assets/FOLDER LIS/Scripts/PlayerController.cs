using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    }

    void Update()
    {
        inputManager.UpdateInput();
        playerSkillManager.ProcessTick(inputManager);
    }

    private void FixedUpdate()
    {
        movementManager.ProcessTick(inputManager);
    }
}
