using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float multiplierReduceVelocity;
    private float speedMultiplier = 0;

    private Rigidbody rb;

    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ProcessTick(InputManager inputManager)
    {
        ModifySpeed(multiplierReduceVelocity * Time.fixedDeltaTime);
        rb.velocity = new Vector3(inputManager.MovementInput.x, 0, inputManager.MovementInput.y) * playerSpeed * (100f + speedMultiplier) / 100f;
        if(inputManager.MovementInput != Vector2.zero) rb.transform.forward = rb.velocity;
    }

    public void ModifySpeed(float delta)
    {
        speedMultiplier += delta;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0, 100);
    }
}
