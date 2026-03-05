using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [Header("References")]
    public Transform nodeTwoRoot;      
    public Transform nodeTwo;          
    public Camera mainCam;
    [SerializeField] Transform nodeOne;
    [SerializeField] Transform itemRoot;
    [SerializeField] Transform item;
    [SerializeField] Transform nodeContainer;

    [Header("Movement Settings")]
    public float smoothTime = 0.05f; 
    public float maxSpeed = 20f;     
    public float baseRotationSpeed = 720f;
    public float maxRotationSpeed = 1440f;
    public float maxDistance = 5f;
    public float deadzone = 0.1f;

    [Header("Tilt Settings")]
    public float maxRoll = 90f;     
    public float rollSensitivity = 20f; 
    public float lerpSpeed = 5;

    private Vector3 velocity;
    private Vector3 lastTargetPos;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        lastTargetPos = nodeTwoRoot.position;
    }

    void Update()
    {
        
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 2f);
        Vector3 holdPosition = mainCam.ScreenToWorldPoint(screenCenter);
        nodeOne.position = holdPosition;
        /// var newPos = nodeOne.position - nodeOne.forward;

        nodeTwoRoot.position = Vector3.SmoothDamp(
            nodeTwoRoot.position,
            nodeOne.position,
            ref velocity,
            smoothTime,
            maxSpeed
        );

        Vector3 direction = nodeOne.position - nodeTwoRoot.position;
        float distance = direction.magnitude;
        float normalized = Mathf.Clamp01(distance / maxDistance);

        Quaternion baseRotation;
        if (distance > deadzone)
            baseRotation = Quaternion.LookRotation(direction);
        else
            baseRotation = Quaternion.LookRotation(mainCam.transform.forward); // guard stance

        Vector3 screenPos = mainCam.WorldToScreenPoint(nodeOne.position);
        Vector3 lastScreenPos = mainCam.WorldToScreenPoint(lastTargetPos);

        Vector3 delta = screenPos - lastScreenPos; 

        var currentRot = nodeTwo.localRotation;

        float rollAmount = Mathf.Clamp(-delta.x * rollSensitivity, -maxRoll, maxRoll);

        nodeTwo.localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, -rollAmount), currentRot, lerpSpeed * Time.deltaTime); //Quaternion.Euler(0f, 0f, -rollAmount);

        float dynamicSpeed = Mathf.Lerp(baseRotationSpeed, maxRotationSpeed, normalized);
        nodeTwoRoot.rotation = Quaternion.Slerp(
            nodeTwoRoot.rotation,
            baseRotation,
            dynamicSpeed * Time.deltaTime
        );

        lastTargetPos = nodeOne.position;

        item.SetParent(nodeTwo);
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;

    }
}