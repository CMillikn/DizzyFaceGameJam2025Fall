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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                if (!hasJumped)
                {
                    StartCoroutine(DoubleJumpPrevention());
                    playerRB.linearVelocityY = jumpPower;
                    isGrounded = false;
                    //playerRB.AddForce(Vector2.up * (jumpPower*Time.deltaTime), ForceMode2D.Impulse);
                }
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime));
        }

        if (playerRB.linearVelocityY > jumpPower)
        {
            playerRB.linearVelocityY = jumpPower;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasJumped = false;
        isGrounded = true;
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
