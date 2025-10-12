using UnityEngine;

public class Conrad_WallScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public SpriteRenderer wallSR;
    public GameObject wallHB;
    void Start()
    {
        wallSR.size = new Vector2(wallHB.transform.localScale.x*2,wallSR.size.y);
    }
}
