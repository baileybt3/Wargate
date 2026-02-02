using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 900f;
    [SerializeField] private bool faceMoveDirection = true;

    private CharacterController _cc;

    private WargateInputActions _actions;

    private InputAction _moveAction;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Only owning client should enable input
        _actions = new WargateInputActions();
        _actions.Enable();
        _moveAction = _actions.Player.Movement;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _moveAction = null; 

        if (_actions != null)
        {
            _actions.Disable();
            _actions.Dispose();
            _actions = null;
        }
    }


    private void Update()
    {
        if (!IsOwner) return;
        if (!IsSpawned) return;

        var nm = NetworkManager.Singleton;
        if (nm == null || (!nm.IsClient && !nm.IsServer))
            return; // NetworkManager not started / not connected

        if (_moveAction == null) return;

        Vector2 input = _moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0f, input.y);
        if (move.sqrMagnitude > 1f) move.Normalize();

        Transform cam = Camera.main != null ? Camera.main.transform : null;
        if (cam != null)
        {
            Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
            Vector3 right = cam.right; right.y = 0f; right.Normalize();
            move = right * move.x + forward * move.z;
        }

        SubmitMoveRpc(move, Time.deltaTime);

        if (faceMoveDirection && move.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SubmitMoveRpc(Vector3 worldMoveDir, float deltaTime, RpcParams rpcParams = default)
    {
        // Movement validation
        if (worldMoveDir.sqrMagnitude > 1.01f)
            worldMoveDir = worldMoveDir.normalized;

        float dt = Mathf.Clamp(deltaTime, 0f, 0.05f);

        Vector3 displacement = worldMoveDir * moveSpeed * dt;

        _cc.Move(displacement);
    }


}