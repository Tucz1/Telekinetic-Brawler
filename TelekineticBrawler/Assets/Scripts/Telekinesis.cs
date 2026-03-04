using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] Camera mainCam;
	[SerializeField] Transform nodeOne;
    [SerializeField] Transform container;
	[SerializeField] Transform nodeTwo;
    [SerializeField] Transform testItem;
    Vector3 screenCenter;
    Vector3 holdPosition;

    public float smoothTime = 0.25f;
    public float maxSpeed = Mathf.Infinity;
    public float deadzone = 0.1f;
    public float baseRotationSpeed = 4f;
    public float maxRotationSpeed = 50f;
    public float maxDistance = 5f;

    Vector3 velocity;

    void Update()
    {
        var diff = transform.position - container.position;
        container.position += diff;

        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 2);
	    holdPosition = mainCam.ScreenToWorldPoint(screenCenter);
        nodeOne.position = holdPosition;

        Vector3 current = nodeTwo.position;
        Vector3 targetPos = nodeOne.position;

        nodeTwo.position = Vector3.SmoothDamp(
        nodeTwo.position,
        nodeOne.position,
        ref velocity,
        smoothTime,
        maxSpeed);

        nodeTwo.position = nodeTwo.position;
        
        // Node 3 could just be node 2?

        Vector3 direction = nodeOne.position - nodeTwo.position;

        // if (direction.sqrMagnitude < 0.0001f) return;
        
        float distance = direction.magnitude;

        float normalized = Mathf.Clamp01(distance / maxDistance);

        Quaternion desiredRotation;

        if (distance > deadzone)
        {
            // look at cursor
            desiredRotation = Quaternion.LookRotation(direction);
        }
        else
        {
            // Forward stance
            desiredRotation = Quaternion.LookRotation(nodeOne.forward);
        }

        float dynamicSpeed = Mathf.Lerp(baseRotationSpeed, maxRotationSpeed, normalized);

        nodeTwo.rotation = Quaternion.Slerp(
            nodeTwo.rotation,
            desiredRotation,
            dynamicSpeed * Time.deltaTime
        );

    }
}