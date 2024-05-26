using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Object.Prediction;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;
    private Rigidbody rb;

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
    }

    void Update()
    {
        if (IsOwner)
        {
            MoveData moveData = CollectInput();
            Move(moveData, false);
            MoveServerRpc(moveData);
        }
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
