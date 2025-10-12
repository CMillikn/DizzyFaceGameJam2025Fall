using UnityEngine;
using UnityEngine.UI;

// This script makes a button play a click sound when pressed
// It requires a Button component to work

[RequireComponent(typeof(Button))]
public class dr_UIButtonSFX : MonoBehaviour
{
    // Optional: Replace default click sound with a custom one
    public AudioClip clickOverride;

    void Awake()
    {
        // Get the Button component on this GameObject
        var btn = GetComponent<Button>();
        // Add the PlayClickSound function to the button's onClick event
        btn.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        // Use custom clip if assigned, otherwise use default UI click from AudioManager
        var clip = clickOverride != null ? clickOverride : dr_AudioManager.I.uiClick;
        // Play the sound if a clip is available
        if (clip != null) dr_AudioManager.I.PlaySFX(clip);
    }
}
