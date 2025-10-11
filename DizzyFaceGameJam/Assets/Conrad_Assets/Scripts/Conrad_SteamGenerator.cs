using UnityEngine;

public class Conrad_SteamGenerator : MonoBehaviour
{
    //Steam particles
    public GameObject steamPS;
    public GameObject instantiatedSteamPS;

    //Ball script
    public Conrad_BallScript ballScript;

    private void OnCollisionEnter2D(Collision2D col)
    {
        ballScript = col.gameObject.GetComponent<Conrad_BallScript>();
        if (ballScript != null)
        {
            instantiatedSteamPS = Instantiate(steamPS,col.transform.position,Quaternion.identity);
            instantiatedSteamPS.transform.parent = col.transform;
        }
    }
}
