using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gyrocopter : Powerups
{
    public class OnUseGyrocopter : UnityEvent { };
    public static OnUseGyrocopter onUseGyrocopter = new OnUseGyrocopter();
    public static bool alreadyListened = false;

    public void Start()
    {
        if (!alreadyListened)
        {
            alreadyListened = true;
            onUseGyrocopter.AddListener(UsePowerup);
        }
    }

    protected override void UsePowerup()
    {
        Marble.instance.UseGyrocopter();
        GameManager.instance.gyroActiveTime = Time.time;
    }
}
