using FishNet.Object;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    private float xRotation;
    private float yRotation;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;
        Camera camera = GetComponent<Camera>();
        camera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    [Client(RequireOwnership = true)]
    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
