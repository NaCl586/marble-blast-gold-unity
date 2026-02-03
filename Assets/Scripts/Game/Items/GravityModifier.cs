using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GravityModifier : Powerups
{
    public class OnResetGravity : UnityEvent { }
    public static OnResetGravity onResetGravity = new OnResetGravity();

    public class OnGravityChangedEvent : UnityEvent<Vector3, Vector3> { }
    public static OnGravityChangedEvent onGravityChanged = new OnGravityChangedEvent();

    private static bool isRotating;

    [SerializeField] private GameObject upVectorFrom;
    [SerializeField] private GameObject upVectorTo;

    [Header("Gravity Settings")]
    public float transitionTime = 0.5f;

    private Vector3 upVector;
    private bool triggered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterGlobalListeners()
    {
        // Ensure gravity can ALWAYS be reset
        onResetGravity.RemoveAllListeners();
        onResetGravity.AddListener(ResetGravityInternal);
    }

    private void Start()
    {
        upVector = upVectorTo.transform.position - upVectorFrom.transform.position;
        isRotating = false;
    }

    public static void ResetGravityGlobal()
    {
        ResetGravityInternal();
        onResetGravity?.Invoke();
        onGravityChanged?.Invoke(Vector3.down, Vector3.down);
    }

    protected override void UsePowerup()
    {
        if (triggered || isRotating)
            return;

        ApplyGravity(upVector);
    }

    void ApplyGravity(Vector3 targetDir)
    {
        StopAllCoroutines();
        StartCoroutine(ApplyGravityCoroutine(targetDir));
    }

    IEnumerator ApplyGravityCoroutine(Vector3 targetDir)
    {
        if (targetDir.sqrMagnitude < 0.001f)
            yield break;

        isRotating = true;
        triggered = true;

        targetDir.Normalize();

        Vector3 startGravity = GravitySystem.GravityDir.normalized;
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionTime);

            Vector3 newGravity = SafeLerp(startGravity, targetDir, t).normalized;
            GravitySystem.GravityDir = newGravity;

            if (Marble.instance != null)
                Marble.instance.gyrocopterBlades.transform.up = -newGravity;

            onGravityChanged?.Invoke(startGravity, newGravity);
            yield return null;
        }

        GravitySystem.GravityDir = targetDir;

        onGravityChanged?.Invoke(startGravity, targetDir);

        isRotating = false;
        triggered = false;
    }

    static void ResetGravityInternal()
    {
        GravitySystem.GravityDir = Vector3.down;

        if (Marble.instance != null)
            Marble.instance.gyrocopterBlades.transform.up = Vector3.up;

        isRotating = false;
    }

    static Vector3 SafeLerp(Vector3 start, Vector3 target, float t)
    {
        start.Normalize();
        target.Normalize();

        float dot = Vector3.Dot(start, target);

        if (dot < -0.9999f)
        {
            Vector3 intermediate = GetCameraClockwise90(start);

            return t < 0.5f
                ? Vector3.Lerp(start, intermediate, t * 2f).normalized
                : Vector3.Lerp(intermediate, target, (t - 0.5f) * 2f).normalized;
        }

        return Vector3.Lerp(start, target, t).normalized;
    }

    static Vector3 GetCameraClockwise90(Vector3 oldDir)
    {
        oldDir.Normalize();

        Vector3 axis = Camera.main != null
            ? Camera.main.transform.forward
            : Vector3.forward;

        // If axis is nearly parallel, choose a stable fallback
        if (Mathf.Abs(Vector3.Dot(axis, oldDir)) > 0.999f)
            axis = Camera.main != null
                ? Camera.main.transform.up
                : Vector3.up;

        axis.Normalize();

        // Clockwise as seen from camera
        return Quaternion.AngleAxis(-90f, axis) * oldDir;
    }
}
