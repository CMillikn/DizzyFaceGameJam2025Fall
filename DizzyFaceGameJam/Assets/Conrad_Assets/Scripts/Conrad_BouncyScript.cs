using UnityEngine;

public class Conrad_BouncyScript : MonoBehaviour
{
    //How bouncy this object is
    public float bounceAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        col.rigidbody.AddForce(Vector2.Reflect(col.transform.position,transform.up) * (col.rigidbody.linearVelocity*Time.deltaTime), ForceMode2D.Impulse);
    }
}
