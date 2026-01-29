using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShockAbsorber : Powerups
{
    public class OnUseShockAbsorber : UnityEvent { };
    public static OnUseShockAbsorber onUseShockAbsorber = new OnUseShockAbsorber();
    public static bool alreadyListened = false;

    public void Start()
    {
        if (!alreadyListened)
        {
            alreadyListened = true;
            onUseShockAbsorber.AddListener(UsePowerup);
        }
    }

    public void OnDisable()
    {
        alreadyListened = false;
        onUseShockAbsorber.RemoveAllListeners();
    }

    protected override void UsePowerup()
    {
        Marble.instance.StopSound(PowerupType.SuperBounce);
        Marble.instance.PlaySound(PowerupType.ShockAbsorber);
        Marble.instance.UseShockAbsorber();
        GameManager.instance.sbsaActiveTime = Time.time;
    }
}
