using UnityEngine;
using FishNet.Object;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    private float rotationX;
    private float rotationY;

    private Camera playerCamera;
    private Rigidbody rb;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            playerCamera.transform.SetParent(transform, false);
        }
    }

    // Struct to store movement data
    private struct MoveData
    {
        public float move;
        public float rotate;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleCameraRotation();

            MoveData moveData = CollectInput();
            Move(moveData, false);
            MoveServerRpc(moveData);
        }
    }

    private void HandleCameraRotation()
    {
        // Handle vertical rotation
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Handle horizontal rotation
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }

    // Collect input from the player
    private MoveData CollectInput()
    {
        MoveData moveData = new MoveData
        {
            move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime,
            rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime
        };
        return moveData;
    }

    // Move the player based on input
    private void Move(MoveData moveData, bool asServer)
    {
        Vector3 moveVector = transform.forward * moveData.move;
        rb.MovePosition(rb.position + moveVector);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0, moveData.rotate, 0)));
    }

    // Send movement data to the server
    [ServerRpc]
    private void MoveServerRpc(MoveData moveData)
    {
        Move(moveData, true);
        MoveObserversRpc(moveData);
    }

    // Send movement data to all clients
    [ObserversRpc]
    private void MoveObserversRpc(MoveData moveData)
    {
        if (!IsOwner)
        {
            Move(moveData, false);
        }
    }
}
