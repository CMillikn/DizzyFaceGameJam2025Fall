using UnityEngine;

public class Conrad_CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Conrad_PlayerScript playerScript;
    public float cameraFollowSpeed;
    public Vector3 cameraOffset;
    public float cameraOffsetAmount;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraOffset = new Vector3(playerScript.transform.position.x, playerScript.transform.position.y + cameraOffsetAmount, -10);
        transform.position = Vector3.Lerp(transform.position, cameraOffset, cameraFollowSpeed *Time.deltaTime);
    }
}
