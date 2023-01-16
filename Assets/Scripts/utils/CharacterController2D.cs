using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f; // Amount of force added when the player jumps.
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform[] m_GroundChecks;                           // An array of positions marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Transform m_WallCheck;                          // A position marking where to check for walls
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    private Animator anim;
    const float k_GroundedRadius = .5f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private bool m_AgainstWall;         // Whether or not the player is against a wall.
    private bool m_AgainstCeiling;      // Whether or not the player is against a ceiling.
    const float k_CeilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
    const float k_WallRadius = .2f;     // Radius of the overlap circle to determine if the player can move against a wall
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public UnityEvent OnWallEvent;
    public UnityEvent OnCeilingEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnWallEvent == null)
            OnWallEvent = new UnityEvent();

        if (OnCeilingEvent == null)
            OnCeilingEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
        anim = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to any of the ground check positions hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        for (int i = 0; i < m_GroundChecks.Length; i++)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundChecks[i].position, k_GroundedRadius, m_WhatIsGround);
            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].gameObject != gameObject && colliders[j].attachedRigidbody.mass > 1f &&
                Mathf.Abs(Vector2.Angle(colliders[j].transform.up, Vector2.up)) < 45f)
                {
                    m_Grounded = true;
                    if (!wasGrounded)
                        OnLandEvent.Invoke();
                }
            }
        }
        // Check if the player is against a wall
        bool wasAgainstWall = m_AgainstWall;
        m_AgainstWall = false;
        Collider2D[] wallColliders = Physics2D.OverlapCircleAll(m_WallCheck.position, k_WallRadius, m_WhatIsGround);
        for (int i = 0; i < wallColliders.Length; i++)
        {
            if (wallColliders[i].gameObject != gameObject && wallColliders[i].attachedRigidbody.mass > 1f &&
                Mathf.Abs(Vector2.Angle(wallColliders[i].transform.up, Vector2.up)) < 45f)
            {
                m_AgainstWall = true;
                if (!wasAgainstWall)
                    OnWallEvent.Invoke();
            }
        }

        // Check if the player is against a ceiling
        bool wasAgainstCeiling = m_AgainstCeiling;
        m_AgainstCeiling = false;
        Collider2D[] ceilingColliders = Physics2D.OverlapCircleAll(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround);
        for (int i = 0; i < ceilingColliders.Length; i++)
        {
            if (ceilingColliders[i].gameObject != gameObject && ceilingColliders[i].attachedRigidbody.mass > 1f &&
                Mathf.Abs(Vector2.Angle(ceilingColliders[i].transform.up, Vector2.up)) > 135f)
            {
                m_AgainstCeiling = true;
                if (!wasAgainstCeiling)
                    OnCeilingEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }
        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }
        if (m_Grounded)
        {
            anim.SetBool("isJumping", false);
        }
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent
    .Invoke(true);
                }
                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 120f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            anim.SetBool("isJumping", true);
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
