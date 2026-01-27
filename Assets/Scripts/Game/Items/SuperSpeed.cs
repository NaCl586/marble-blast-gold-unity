using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuperSpeed : Powerups
{
    [SerializeField] float superSpeedMultiplier = 25f;
    public class OnUseSuperSpeed : UnityEvent { };
    public static OnUseSuperSpeed onUseSuperSpeed = new OnUseSuperSpeed();
    public static bool alreadyListened = false;

    public void Start()
    {
        if (!alreadyListened)
        {
            alreadyListened = true;
            onUseSuperSpeed.AddListener(UsePowerup);
        }
    }

    protected override void UsePowerup()
    {
        GameManager.instance.PlayAudioClip(useSound);

        Movement.instance.ApplySurfaceBoost(superSpeedMultiplier);
    }
}
