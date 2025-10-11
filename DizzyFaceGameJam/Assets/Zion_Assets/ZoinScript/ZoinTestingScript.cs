using UnityEngine;
using TMPro;

public class TestingByZoin : MonoBehaviour
{
    public TMP_Text zoinWasHere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        zoinWasHere.text = "Zoin was";
        zoinWasHere.text += " here!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
