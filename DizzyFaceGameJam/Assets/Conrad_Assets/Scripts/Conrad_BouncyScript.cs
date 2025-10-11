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
            Debug.Log(col.rigidbody.linearVelocity.magnitude.ToString());
            if (col.rigidbody.linearVelocity.magnitude < weakIncoming)
            {
                col.rigidbody.AddForce((transform.up) * (forcedBounceAmount * Time.deltaTime), ForceMode2D.Impulse);
            }
            else
            {
                col.rigidbody.AddForce(Vector2.Reflect(col.transform.position,transform.up) * (col.rigidbody.linearVelocity*Time.deltaTime), ForceMode2D.Impulse);
            }
        }
    }
}
