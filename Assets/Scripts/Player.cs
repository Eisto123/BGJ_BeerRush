using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public PlayerControl playerControl;
    private Vector2 playerMovement;

    [Header("Movement Config")]
    public float autoMoveSpeed;
    public float leftRightSpeed;
    
    
    private Rigidbody2D rb;


    void Awake()
    {
        playerControl = new PlayerControl();
        rb = GetComponent<Rigidbody2D>();
    }


    void OnEnable()
    {
        playerControl.Enable();
    }

    void OnDisable()
    {
        playerControl.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement = playerControl.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        MoveForward();
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector2(playerMovement.x * leftRightSpeed * Time.deltaTime, rb.velocity.y);
    }

    private void MoveForward()
    {
        rb.velocity= Vector2.up * autoMoveSpeed;
    }
    
}
