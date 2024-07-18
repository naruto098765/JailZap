using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeScript : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private Transform TransPoint1; // The first point of the slingshot (the handle)
    [SerializeField] private Transform TransPoint2; // The second point of the slingshot (the rubber band)

    [Header("Prefab")]
    [SerializeField] private Transform BallPrefab; // The ball prefab to be instantiated
    private LineRenderer _lineRenderer; // The line renderer for the rubber band
    private Transform _newBall; // The instance of the ball
    private Camera mainCamLocal; // The main camera

    [Header("Force Settings")]
    [SerializeField] private float maxForce = 1000f; // Maximum force applied to the ball
    [SerializeField] private float minForce = 100f; // Minimum force applied to the ball

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 3; // The line renderer will have three points

        mainCamLocal = Camera.main; // Get the main camera
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && BallPrefab != null)
        {
            // Instantiate the ball when the mouse button is pressed
            _newBall = Instantiate(BallPrefab, TransPoint1.position, Quaternion.identity); // Instantiate the ball at TransPoint1 position
            _newBall.GetComponent<Rigidbody>().isKinematic = true; // Set the ball to kinematic so it doesn't respond to physics yet
        }

        if (_newBall != null)
        {
            if (Input.GetMouseButton(0))
            {
                // Update the ball's position based on the mouse input
                Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamLocal.WorldToScreenPoint(TransPoint1.position).z);
                Vector3 worldPos = mainCamLocal.ScreenToWorldPoint(pos);

                // Calculate the offset based on the initial position
                Vector3 offset = worldPos - TransPoint1.position;

                // Adjust the ball's position with respect to the initial position, considering only x and y offsets
                _newBall.position = new Vector3(TransPoint1.position.x + offset.x, TransPoint1.position.y + offset.y, TransPoint1.position.z - offset.magnitude);

                _lineRenderer.SetPosition(1, _newBall.position); // Set the second point of the line renderer to the ball's position
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // Release the ball and apply force
                Rigidbody newBallRb = _newBall.GetComponent<Rigidbody>();
                newBallRb.isKinematic = false; // Enable physics

                // Calculate the force direction and magnitude
                Vector3 forceDirection = (TransPoint1.position - _newBall.position).normalized;
                float forceMagnitude = Mathf.Clamp((TransPoint1.position - _newBall.position).magnitude * 500, minForce, maxForce);
                newBallRb.AddForce(forceDirection * forceMagnitude); // Apply force to the ball
                _newBall = null; // Clear the ball reference

                // Reset the line renderer points to their original positions
                _lineRenderer.SetPosition(1, TransPoint1.position); // Set the second point of the line renderer to TransPoint1
            }

            if (TransPoint1 != null && TransPoint2 != null)
            {
                _lineRenderer.SetPosition(0, TransPoint1.position); // Set the first point of the line renderer to TransPoint1
                _lineRenderer.SetPosition(2, TransPoint2.position); // Set the third point of the line renderer to TransPoint2
                if (_newBall != null)
                {
                    _lineRenderer.SetPosition(1, _newBall.position); // Update the second point of the line renderer to the ball's position
                }
            }
        }
    }
}
