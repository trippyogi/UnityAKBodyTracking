using UnityEngine;

public class CameraFlipper : MonoBehaviour
{
    [Tooltip("Flip by x axis")]
    public bool FlipByX;
    private Camera _cam;


    void Start()
    {
        _cam = GetComponent<Camera>();
    }

    // Flip front camera to be aligned in directions with depth image on scene.
    void OnPreCull()
    {
        if (!FlipByX) return;
        _cam.ResetWorldToCameraMatrix();
        _cam.ResetProjectionMatrix();
        var scale = new Vector3(-1, 1, 1);
        _cam.projectionMatrix = _cam.projectionMatrix * Matrix4x4.Scale(scale);
    }

    void OnPreRender()
    {
        if (FlipByX)
        {
            GL.invertCulling = true;
        }
    }

    void OnPostRender()
    {
        if (FlipByX)
        {
            GL.invertCulling = false;
        }
    }
}