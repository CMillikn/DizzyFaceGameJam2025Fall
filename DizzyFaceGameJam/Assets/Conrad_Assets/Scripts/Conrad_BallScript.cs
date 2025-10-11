using UnityEngine;

public class Conrad_BallScript : MonoBehaviour
{
    // RB for the physics character
    public Rigidbody2D ballRB;

    //Float for maximum velocity

    public float maxVelocity;
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
    }
}
