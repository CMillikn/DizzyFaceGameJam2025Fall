using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class dr_UIButtonSFX : MonoBehaviour
{
    public AudioClip clickOverride;

    void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        var clip = clickOverride != null ? clickOverride : dr_AudioManager.I.uiClick;
        if (clip != null) dr_AudioManager.I.PlaySFX(clip);
    }
}
