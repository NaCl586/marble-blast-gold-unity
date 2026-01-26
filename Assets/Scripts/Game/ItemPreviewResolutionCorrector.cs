using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPreviewResolutionCorrector : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = Screen.width / 1280f;
    }
}
