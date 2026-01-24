using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Marble : MonoBehaviour
{
    public static Marble instance { get; private set; }

    private Movement movement;
    private Transform startPoint;

    private void Awake()
    {
        // Enforce singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        movement = GetComponent<Movement>();

        // Cursor setup
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        startPoint = GameObject.Find("StartPad").transform.Find("Spawn");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (startPoint == null)
            return;

        GravityModifier.onResetGravity?.Invoke();

        movement.SetPosition(startPoint.position);

        CameraController.instance?.ResetCam();
    }
}
