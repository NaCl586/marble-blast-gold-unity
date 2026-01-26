using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuperJump : Powerups
{
    [SerializeField] float superJumpHeight = 20f;
    public class OnUseSuperJump : UnityEvent { };
    public static OnUseSuperJump onUseSuperJump = new OnUseSuperJump();
    public static bool alreadyListened = false;

    public void Start()
    {
        if (!alreadyListened)
        {
            alreadyListened = true;
            onUseSuperJump.AddListener(UsePowerup);
        }
    }

    protected override void UsePowerup()
    {
        GameManager.instance.PlayAudioClip(useSound);

        Movement.instance.marbleVelocity += -GravitySystem.GravityDir.normalized * superJumpHeight;
    }
}
