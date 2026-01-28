using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    [HideInInspector] public AudioSource buttonFx;
    [HideInInspector] public bool enableSound;
    public bool isToggle = false;
    public AudioClip hoverFx;
    public AudioClip clickFx;
    Button b;

    void Start()
    {
        b = GetComponent<Button>();
        buttonFx = this.GetComponent<AudioSource>();

        if (buttonFx.gameObject.GetComponent<Button>())
            enableSound = buttonFx.gameObject.GetComponent<Button>().interactable;
    }

    public void HoverSound()
    {
        if (!b.IsInteractable()) return;
        if (enableSound || isToggle) buttonFx.PlayOneShot(hoverFx);
    }
    public void ClickSound()
    {
        if (!b.IsInteractable()) return;
        if (enableSound || isToggle) buttonFx.PlayOneShot(clickFx);
    }
}
