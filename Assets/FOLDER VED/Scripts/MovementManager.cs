using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float fastSpeed;

    private Rigidbody rb;

    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ProcessTick(InputManager inputManager, bool fast)
    {
        float speed = fast?fastSpeed:playerSpeed;
        rb.velocity = new Vector3(inputManager.MovementInput.x, 0, inputManager.MovementInput.y) * speed;
        if(inputManager.MovementInput != Vector2.zero) rb.transform.forward = rb.velocity;
    }

}
