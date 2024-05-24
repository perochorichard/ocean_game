using UnityEngine;
using FishNet.Object;
public class PlayerCam : NetworkBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    private float xRotation;
    private float yRotation;

//    void Start()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//    }

    void Update()
    {
        Rotate();
    }

    [Client(RequireOwnership = true)]
    private void Rotate()
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
