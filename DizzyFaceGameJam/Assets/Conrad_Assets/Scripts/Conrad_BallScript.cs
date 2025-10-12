using System.Collections;
using UnityEngine;

public class Conrad_BallScript : MonoBehaviour
{
    // RB for the physics character
    public Rigidbody2D ballRB;
    public Collider2D ballCol;

    //Float for maximum velocity
    public float maxVelocity;


    //Tagscript for checking if collision is the killbox
    private Conrad_KillBox killboxScript;
    private Conrad_BouncyScript bouncyScript;

    public bool canBeHit;

    public float rehitTime;

    public Animator ballAN;

    public float spriteRotateSpeed;

    public SpriteRenderer ballSR;

    void Start()
    {

        ballRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballRB.linearVelocity.magnitude > maxVelocity)
        {
            ballRB.linearVelocity = Vector2.ClampMagnitude(ballRB.linearVelocity, maxVelocity);
        }

        ballSR.transform.Rotate(new Vector3(0,0,(spriteRotateSpeed * Time.deltaTime)));
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        bouncyScript = col.transform.GetComponent<Conrad_BouncyScript>();
        if (bouncyScript != null)
        {
            StartCoroutine(StartRehitTimer());
        }
        killboxScript = col.transform.GetComponent<Conrad_KillBox>();
        if (killboxScript != null )
        {
            Destroy(gameObject);
        }
    }

    public void WaitForRehit()
    {
        StartCoroutine(StartRehitTimer());
    }

    IEnumerator StartRehitTimer()
    {
        spriteRotateSpeed = Random.Range(Random.Range(-720,-360), Random.Range(720, 360));
        spriteRotateSpeed = spriteRotateSpeed * 2;
        ballAN.SetBool("isPain", true);
        int HitBall = LayerMask.NameToLayer("HitBall");
        int layerBall = LayerMask.NameToLayer("Ball");
        gameObject.layer = HitBall;
        yield return new WaitForSeconds(rehitTime);
        ballAN.SetBool("isPain", false);
        gameObject.layer = layerBall;
        spriteRotateSpeed = spriteRotateSpeed / 2;
    }
}
