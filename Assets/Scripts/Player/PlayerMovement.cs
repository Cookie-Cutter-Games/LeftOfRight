using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private CharacterController2D controller;
    public Rigidbody2D rb;
    private Animator anim;
    private float moveSpeed, jumpForce;
    public Joystick joystick;
    private float horizontalMove = 0f;
    bool isJumping = false;
    bool isCrouching = false;
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        gameObject.GetComponent<PlayerStatistics>().playerChangedMovementSpeedEvent += changeMovementSpeed;
        moveSpeed = gameObject.GetComponent<PlayerStatistics>().movementSpeed();
        jumpForce = 10f;
    }


    public void changeMovementSpeed(object sender, EventArgs e)
    {
        moveSpeed = gameObject.GetComponent<PlayerStatistics>().movementSpeed();
    }

    private void Update()
    {
        if (gameObject.transform.position.y <= -10f)
        {
            gameObject.transform.position = new Vector3(0, -3f, 0);
        }
        moveSpeed = gameObject.GetComponent<PlayerStatistics>().movementSpeed();
        horizontalMove = joystick.Horizontal * moveSpeed;
        float verticalmove = joystick.Vertical;
        if (!isJumping && Math.Abs(horizontalMove) >= 0.01)
        {
            anim.SetFloat("MovementSpeed", Math.Abs(horizontalMove));
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (verticalmove >= .75f && !isJumping)
        {

            isJumping = true;
            rb.velocity = Vector2.up * jumpForce;
        }
        //CROUCHING IS DIASBLED FOR BUG
        // if (verticalmove > -1f)
        // {
        //     isCrouching = false;
        // }
        // else if (verticalmove < -4.5f)
        // {
        //     isCrouching = true;
        // }
        if (verticalmove >= 0f)
        {
            isCrouching = false;
        }
    }
    private void FixedUpdate()
    {
        if (!gameObject.GetComponent<PlayerShootBullets>().isShooting)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
            controller.OnLandEvent.AddListener(grounded);
        }

    }

    public void grounded()
    {
        isJumping = false;
    }
}
