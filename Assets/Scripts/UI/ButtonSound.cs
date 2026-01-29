using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    AudioSource buttonFx;

    Button button;
    Toggle toggle;

    public AudioClip hoverFx;
    public AudioClip clickFx;

    [Tooltip("For toggles: play sound only when toggled ON")]
    public bool playToggleOnOnly = false;

    void Awake()
    {
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        if (button)
        {
            button.onClick.AddListener(PlayClickSound);
        }

        if (toggle)
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void OnDestroy()
    {
        if (button)
            button.onClick.RemoveListener(PlayClickSound);

        if (toggle)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    // --- Hover (called from EventTrigger or IPointerEnter) ---
    public void HoverSound()
    {
        if (!IsInteractable()) return;

        buttonFx = GetComponent<AudioSource>();
        buttonFx.volume = PlayerPrefs.GetFloat("Audio_SoundVolume", 0.5f);

        if (hoverFx) buttonFx.PlayOneShot(hoverFx);
    }

    // --- Button click ---
    void PlayClickSound()
    {
        if (!IsInteractable()) return;

        buttonFx = GetComponent<AudioSource>();
        buttonFx.volume = PlayerPrefs.GetFloat("Audio_SoundVolume", 0.5f);

        if (clickFx) buttonFx.PlayOneShot(clickFx);
    }

    // --- Toggle click ---
    void OnToggleChanged(bool isOn)
    {
        if (!IsInteractable()) return;

        if (playToggleOnOnly && !isOn)
            return;

        buttonFx = GetComponent<AudioSource>();
        if (clickFx) buttonFx.PlayOneShot(clickFx);
    }

    bool IsInteractable()
    {
        if (button) return button.interactable;
        if (toggle) return toggle.interactable;
        return false;
    }
}
