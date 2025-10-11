using System.Collections;
using UnityEngine;

public class Conrad_KillThis : MonoBehaviour
{
    //Self destruct timer
    public float killTime;
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(killTime);
        Destroy(gameObject);
    }
}
