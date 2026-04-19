using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16f / 9f;
    [SerializeField] private float orthographicSize = 16.875f;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        cam.orthographicSize = orthographicSize;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }
}