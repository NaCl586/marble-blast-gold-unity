using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBinding : MonoBehaviour
{
    public static ControlBinding instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveLeft;
    public KeyCode moveRight;

    public KeyCode usePowerup;
    public KeyCode jump;

    public KeyCode rotateCameraUp;
    public KeyCode rotateCameraDown;
    public KeyCode rotateCameraLeft;
    public KeyCode rotateCameraRight;

    public KeyCode freelookKey;
    public float mouseSensitivity;
    public bool invertMouseYAxis;
    public bool alwaysFreeLook;

    public void Start()
    {
        moveForward = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Marble_Forward", "W"));
        moveBackward = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Marble_Back", "S"));
        moveLeft = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Marble_Left", "A"));
        moveRight = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Marble_Right", "D"));

        usePowerup = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Marble_Powerup", "Left Mouse Button"));
        jump = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Marble_Jump", "Space"));

        rotateCameraDown = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Cam_Down", "Down"));
        rotateCameraLeft = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Cam_Left", "Left"));
        rotateCameraRight = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Cam_Right", "Right"));
        rotateCameraUp = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Cam_Up", "Up"));

        freelookKey = Utils.ParseKeyCode(PlayerPrefs.GetString("Control_Mouse_Freelook", "Right Mouse Button"));

        mouseSensitivity = PlayerPrefs.GetFloat("Controls_MouseSensitivity", 1);
        invertMouseYAxis = PlayerPrefs.GetInt("Controls_Mouse_InvertYAxis", 0) == 1;
        alwaysFreeLook = PlayerPrefs.GetInt("Controls_Mouse_Freelook", 1) == 1;
    }

    public void AssignKey(string controlName, KeyCode keycode)
    {
        if (controlName == "Move Forward")
        {
            PlayerPrefs.SetString("Control_Marble_Forward", Utils.KeyCodeToString(keycode));
            moveForward = keycode;
        }
        else if (controlName == "Move Backward")
        {
            PlayerPrefs.SetString("Control_Marble_Back", Utils.KeyCodeToString(keycode));
            moveBackward = keycode;
        }
        else if (controlName == "Move Left")
        {
            PlayerPrefs.SetString("Control_Marble_Left", Utils.KeyCodeToString(keycode));
            moveLeft = keycode;
        }
        else if (controlName == "Move Right")
        {
            PlayerPrefs.SetString("Control_Marble_Right", Utils.KeyCodeToString(keycode));
            moveRight = keycode;
        }

        else if (controlName == "Use Powerup")
        {
            PlayerPrefs.SetString("Control_Marble_Powerup", Utils.KeyCodeToString(keycode));
            usePowerup = keycode;
        }
        else if (controlName == "Jump")
        {
            PlayerPrefs.SetString("Control_Marble_Jump", Utils.KeyCodeToString(keycode));
            jump = keycode;
        }

        else if (controlName == "Rotate Camera Down")
        {
            PlayerPrefs.SetString("Control_Cam_Down", Utils.KeyCodeToString(keycode));
            rotateCameraDown = keycode;
        }
        else if (controlName == "Rotate Camera Left")
        {
            PlayerPrefs.SetString("Control_Cam_Left", Utils.KeyCodeToString(keycode));
            rotateCameraLeft = keycode;
        }
        else if (controlName == "Rotate Camera Right")
        {
            PlayerPrefs.SetString("Control_Cam_Right", Utils.KeyCodeToString(keycode));
            rotateCameraRight = keycode;
        }
        else if (controlName == "Rotate Camera Up")
        {
            PlayerPrefs.SetString("Control_Cam_Up", Utils.KeyCodeToString(keycode));
            rotateCameraUp = keycode;
        }

        else if (controlName == "Free-Look Key")
        {
            PlayerPrefs.SetString("Control_Mouse_Freelook", Utils.KeyCodeToString(keycode));
            freelookKey = keycode;
        }

        PlayerPrefs.Save();
    }
}
