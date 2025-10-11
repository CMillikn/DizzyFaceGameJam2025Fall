using UnityEngine;

public class Conrad_BouncyScript : MonoBehaviour
{
    //How bouncy this object is
    public float bounceAmount;
    //Floats to force more fun bounce in the event of weakass incoming speed
    public float weakIncoming;
    public float forcedBounceAmount;
    public Conrad_BallScript ballScript;

    private void OnCollisionEnter2D(Collision2D col)
    {
        ballScript = col.gameObject.GetComponent<Conrad_BallScript>();
        if (ballScript != null)
        {
            Debug.Log(col.rigidbody.linearVelocityY.ToString());
            if (col.rigidbody.linearVelocityY < weakIncoming)
            {
                col.rigidbody.linearVelocityY = (forcedBounceAmount * Time.deltaTime);
            }
            col.rigidbody.linearVelocity = Vector2.zero;
            col.rigidbody.AddForce(transform.up * (bounceAmount), ForceMode2D.Impulse);
        }
    }
}
