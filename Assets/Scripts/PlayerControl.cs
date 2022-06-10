using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public Animator animator;
    private Rigidbody2D rb2d;
    public AudioSource deathNoise;
    public AudioSource jumpNoise;
    public AudioSource landNoise;
    public AudioSource walkNoise;
    public AudioClip deathClip;
    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip walkClip;


    public float deathVolume = 0.75f; 
    public float jumpVolume = 0.5f;
    public float landVolume = 0.75f;
    public float walkVolume = 0.5f;
    public float stepTime = 0.5f;
    float timer;
    public float horizontalMov;
    public float verticalMov;
    public float speed;
    public float jumpforce;
    public bool isJumping = false;
    bool facingRight = true;
    //private bool m_FacingRight = true;  

    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask whatIsGround;

    private bool isGrounded = false;

    [SerializeField] private bool isDoubleJumpingEnabled;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        horizontalMov = Input.GetAxisRaw("Horizontal") * speed;
        verticalMov = Input.GetAxisRaw("Vertical") * speed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMov));


        if (horizontalMov != 0)
        {
            timer += Time.deltaTime;
            if (timer > stepTime)
            {
                walkNoise.PlayOneShot(walkClip, walkVolume);
                timer = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Debug.Log("isJumping");

                rb2d.AddForce(new Vector2(0.0f, jumpforce * speed));
                isGrounded = false;
                isDoubleJumpingEnabled = true;
            }

            else if (isDoubleJumpingEnabled)
            {
                rb2d.AddForce(new Vector2(0.0f, jumpforce * speed));
                isGrounded = false;
                isDoubleJumpingEnabled = false;

            }
        }

    }

    void FixedUpdate()
    {
        if (horizontalMov > 0 && !facingRight)
        {
            Flip();
        }
        if (horizontalMov < 0 && facingRight)
        {
            Flip();
        }

        if (horizontalMov > 0.1f || horizontalMov < -0.1f)
        {
            rb2d.AddForce(new Vector2(horizontalMov * speed, 0f), ForceMode2D.Impulse);
        }

        isGrounded = GroundCheck();

        rb2d.velocity = new Vector2(horizontalMov, rb2d.velocity.y);

    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, whatIsGround);
    }

    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            landNoise.PlayOneShot(landClip, landVolume);
            isJumping = false;
            animator.SetBool("Jump", false);
        }

        else if(collision.gameObject.tag == "Spike")
        {
            FindObjectOfType<GameOverScreen>().GameOver();
            deathNoise.PlayOneShot(deathClip, deathVolume);
        }

        else if(collision.gameObject.tag == "Bullet")
        {
            FindObjectOfType<GameOverScreen>().GameOver();
            deathNoise.PlayOneShot(deathClip, deathVolume);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJumping = true;
            animator.SetBool("Jump", true);
            jumpNoise.PlayOneShot(jumpClip, jumpVolume);

        }

       
    }

    

}
