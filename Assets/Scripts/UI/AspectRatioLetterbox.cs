using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class AspectRatioLetterbox : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void Update()
    {
        Apply();
    }

    void Apply()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            // Letterbox (top & bottom)
            cam.rect = new Rect(
                0f,
                (1f - scaleHeight) / 2f,
                1f,
                scaleHeight
            );
        }
        else
        {
            // Pillarbox (left & right)
            float scaleWidth = 1f / scaleHeight;

            cam.rect = new Rect(
                (1f - scaleWidth) / 2f,
                0f,
                scaleWidth,
                1f
            );
        }
    }
}
