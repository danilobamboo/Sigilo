using UnityEngine;

public class RigidbodyMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float RunSpeed = 6.0f;
    public float WalkSpeed = 2.5f;
    public float jumpSpeed = 8.0f;
    public KeyCode ToggleWalkKey = KeyCode.LeftAlt;

    public Rigidbody PlayerBody;
    public Transform PlayerCamera;
    public Transform FeetTransform;
    public LayerMask FloorMask;

    public float Sensitivity;
    private float xRot;

    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;

    public bool IsRunning { get; private set; } = true;

    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (Input.GetKeyDown(ToggleWalkKey))
        {
            IsRunning = !IsRunning;
        }

        MovePlayer();
        MovePlayerCamera();
    }

    private void MovePlayer()
    {
        float currentSpeed = IsRunning ? RunSpeed : WalkSpeed;
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * currentSpeed;
        PlayerBody.linearVelocity = new Vector3(MoveVector.x, PlayerBody.linearVelocity.y, MoveVector.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.CheckSphere(FeetTransform.position, 0.1f, FloorMask))
            {
                PlayerBody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }
        }
    }

    private void MovePlayerCamera()
    {
        xRot -= PlayerMouseInput.y * Sensitivity;

        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
}