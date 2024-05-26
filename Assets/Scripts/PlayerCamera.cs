using FishNet.Object;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    private float xRotation = 0f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            //Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleCameraRotation();
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
