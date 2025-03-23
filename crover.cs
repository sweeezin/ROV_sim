using UnityEngine;

public class RoverController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stickForce = 10f;
    public float gravityStrength = 9.81f; // Gravity strength towards ship
    public float gravityDistance = 5f; // Max distance to detect hull
    public float rotationSpeed = 5f; // Smooth rotation speed
    public LayerMask shipLayer;
    public Transform ship; // Assign ship Transform in Inspector

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable Unity gravity
    }

    void Update()
    {
        MoveRover();
        ApplyGravity();
        AlignToHull();
    }

    void MoveRover()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void ApplyGravity()
    {
        if (ship == null) return;

        // Gravity pulls rover towards the ship's position
        Vector3 directionToShip = (ship.position - transform.position).normalized;
        rb.AddForce(directionToShip * gravityStrength, ForceMode.Acceleration);
    }

    void AlignToHull()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, gravityDistance, shipLayer))
        {
            // Align rover’s up direction with the hull surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Apply stick force to keep rover attached
            rb.AddForce(-hit.normal * stickForce, ForceMode.Acceleration);
        }
    }
}
