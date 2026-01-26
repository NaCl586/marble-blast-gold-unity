using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public AudioClip bumperClip;
    public Animator anim;

    public void CollisionEnter()
    {
        Movement.instance.bounce = 15;
        GameManager.instance.PlayAudioClip(bumperClip);
        anim.SetTrigger("Hit");
    }
}