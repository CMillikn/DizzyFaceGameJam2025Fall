using System.Collections;
using UnityEngine;

public class Conrad_PlayerScript : MonoBehaviour
{
    //Player rigidbody2D for physics interactions, side to side movement will be handled by direct translation
    public Rigidbody2D playerRB;

    //Player jump force
    public float jumpPower;

    //Player movement speed
    public float moveSpeed;

    //Bool to check if player is still grounded, used to prevent double jumping
    public bool isGrounded;

    //Script check in lieu of a tag to allow passthrough on platforms with the passthrough trigger box/tagscript
    private Conrad_PassThrough passTag;

    //Amount of time you're allowed to jump if you leave a platform
    public float coyoteDuration;

    //Double jump prevention timer applied after jump
    public float jumpCooldown;
    public bool hasJumped;

    //Steam particles
    public GameObject steamPS;

    //Ball script
    public Conrad_BallScript ballScript;

    //Animator stuff
    [SerializeField] private Animator _animator;

    //Current Vector2
    private Vector2 currentV2;
    public float maxSpeed;

    public bool canHitBall;
    public float ballInvincibility;
    public GameObject bouncers;

    void Update()
    {
        currentV2 = transform.position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                if (!hasJumped)
                {
                    StartCoroutine(DoubleJumpPrevention());
                    playerRB.linearVelocityY = jumpPower;
                    isGrounded = false;
                }
            }
        }

        playerRB.linearVelocity = new Vector2(Mathf.Clamp(playerRB.linearVelocityX, -maxSpeed, maxSpeed), playerRB.linearVelocityY);

        if (Input.GetKey(KeyCode.A))
        {
            playerRB.AddForce((-Vector2.right * (moveSpeed * Time.deltaTime)));
            //transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
            _animator.SetBool("isRunning", true);
        }

        if (Input.GetKey(KeyCode.D))
        {
            playerRB.AddForce((Vector2.right * (moveSpeed * Time.deltaTime)));
            //transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime));
            _animator.SetBool("isRunning", true);
        }

        if ((Input.GetKeyUp(KeyCode.A))||(Input.GetKeyUp(KeyCode.D)))
        {
            _animator.SetBool("isRunning", false);
        }

        if (playerRB.linearVelocityY > jumpPower)
        {
            playerRB.linearVelocityY = jumpPower;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        {
            ballScript = col.gameObject.GetComponent<Conrad_BallScript>();
            if (ballScript != null)
            {
                Instantiate(steamPS, new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity);
            }
            else
            {
                hasJumped = false;
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        StartCoroutine(CoyoteTime());
    }


    private void OnTriggerEnter2D(Collider2D trigCol)
    {
        passTag = trigCol.GetComponent<Conrad_PassThrough>();
        if (passTag != null)
        {
            Physics2D.IgnoreCollision(trigCol.GetComponent<Collider2D>(), GetComponent<Collider2D>(), ignore: true);
        }
    }

    private void OnTriggerExit2D(Collider2D trigCol)
    {
        passTag = trigCol.GetComponent<Conrad_PassThrough>();
        if (passTag != null)
        {
            Physics2D.IgnoreCollision(trigCol.GetComponent<Collider2D>(),GetComponent<Collider2D>(), ignore:false);
        }
    }



    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteDuration);
        isGrounded=false;
    }

    IEnumerator DoubleJumpPrevention()
    {
        hasJumped = true;
        isGrounded=false ;
        yield return new WaitForSeconds(jumpCooldown);
        isGrounded=true;
    }

}
