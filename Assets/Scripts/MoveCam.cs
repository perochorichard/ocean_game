using FishNet.Object;
using UnityEngine;

public class MoveCam : NetworkBehaviour
{
    public Transform cameraPos;
    private void Update()
    {
        Transform();
    }

    [Client(RequireOwnership = true)]
    private void Transform()
    {
        transform.position = cameraPos.position;
    }
}
