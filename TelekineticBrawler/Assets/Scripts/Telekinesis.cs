using System;
using System.Collections;
using UnityEngine;

public class TelekinesisController : MonoBehaviour
{
    [Header("References")]
    private Transform weaponRoot;
    private Transform weaponLogic;
    public Camera mainCam;

    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private Transform nodeOne;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform tetherNode;

    [Header("Weapon")]
    private WeaponData weaponData;
    private Rigidbody weaponRB;

    private Vector3 lastTargetPos;

    [Header("Player Movement Compensation")]
    [SerializeField] Transform player;
    [SerializeField] float movementInfluence = 1f;
    public bool canInfluence;

    Vector3 screenCenter;
    Vector3 lastPlayerPos;
    Vector3 playerVelocity;
    private Vector3 lastDir;
    private float distance;

    public bool attachedItem;
    bool facingEnvironment;
    bool blocked;
    TetherBundle tether;
    // Interactable interactable;

    void Awake()
    {

        tether = GetComponent<TetherBundle>();
        // interactable = FindAnyObjectByType<Interactable>();

        // interactable.Held += AttachItem;
        // interactable.Dropped += RemoveItem;
    }

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;

        lastPlayerPos = player.position;

        screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);

    }

    void Update()
    {
        // if (weaponRoot != null)
        // {
        //     Debug.Log(weaponRoot.position);
        //     Debug.Log(weaponLogic.position);
        //     Debug.Log(nodeOne.position);
        // }






        if (!attachedItem) return;

        Debug.DrawLine(mainCam.transform.position, wallCheck.position, Color.blue);
        if (Physics.Linecast(mainCam.transform.position, wallCheck.position, environmentLayer))
        {
            // Debug.Log("Blocked");
            facingEnvironment = true;
        }
        else facingEnvironment = false;


        playerVelocity = (player.position - lastPlayerPos) / Time.deltaTime;
        lastPlayerPos = player.position;



        UpdateTargetPosition();



        Vector3 direction = nodeOne.position - weaponRoot.position;
        lastDir = direction;
        distance = direction.magnitude;
        float normalizedDistance = Mathf.Clamp01(distance / weaponData.MaxDistance);

        UpdatePosition(distance);
        UpdateRotation(direction, distance, normalizedDistance);
        UpdateRoll();

        if (canInfluence) weaponRoot.position += playerVelocity * Time.deltaTime * movementInfluence;

        lastTargetPos = nodeOne.position;

    }

    void UpdateTargetPosition()
    {
        // Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 2f);
        wallCheck.position = mainCam.ScreenToWorldPoint(screenCenter + (Vector3.forward * weaponData.DistanceHeld));

        if (!facingEnvironment && !blocked) nodeOne.position = mainCam.ScreenToWorldPoint(screenCenter + (Vector3.forward * weaponData.DistanceHeld));
    }


    void UpdatePosition(float distance)
    {
        float weightedSpeed = weaponData.BaseFollowSpeed + distance * weaponData.Weight;

        if ((!blocked && !facingEnvironment) || !facingEnvironment)
            weaponRoot.position = Vector3.MoveTowards(
                weaponRoot.position,
                nodeOne.position,
                weightedSpeed * Time.deltaTime
            );
    }

    void UpdateRotation(Vector3 direction, float distance, float normalizedDistance)
    {
        Quaternion targetRotation =
            distance > weaponData.Deadzone
            ? Quaternion.LookRotation(direction)
            : Quaternion.LookRotation(mainCam.transform.forward);

        float rotationSpeed = Mathf.Lerp(
            weaponData.BaseRotationSpeed,
            weaponData.MaxRotationSpeed,
            normalizedDistance
        );

        weaponRoot.rotation = Quaternion.Slerp(
            weaponRoot.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void UpdateRoll()
    {
        Vector3 currentScreenPos = mainCam.WorldToScreenPoint(nodeOne.position);
        Vector3 lastScreenPos = mainCam.WorldToScreenPoint(lastTargetPos);

        Vector3 delta = currentScreenPos - lastScreenPos;

        float rollAmount = Mathf.Clamp(
            -delta.x * weaponData.RollSensitivity,
            -weaponData.MaxRoll,
            weaponData.MaxRoll
        );

        Quaternion targetRoll = Quaternion.Euler(0f, 0f, -rollAmount);

        weaponLogic.localRotation = Quaternion.Lerp(
            weaponLogic.localRotation,
            targetRoll,
            weaponData.RollSmoothSpeed * Time.deltaTime
        );
    }


    public void AttachItem(Interactable _interactable,
                    WeaponData _weaponData,
                    Rigidbody _weaponRB,
                    Transform _weaponRoot,
                    Transform _weaponLogic)
    {

        // _interactable.Held += AttachItem;
        // _interactable.Dropped += RemoveItem;

        weaponData = _weaponData;
        weaponRB = _weaponRB;
        weaponRoot = _weaponRoot;
        weaponLogic = _weaponLogic;

        nodeOne.position = Vector3.up * weaponData.HoldHeightOffset;

        lastTargetPos = weaponRoot.position;


        weaponRB.isKinematic = true;
        weaponLogic.localPosition = Vector3.zero;
        weaponLogic.localRotation = Quaternion.identity;

        attachedItem = true;
        tether.CreateTethers(tetherNode, weaponRoot, weaponLogic.GetComponent<Collider>());
    }

    public void DropItem()
    {
        weaponRB.isKinematic = false;

        weaponRB.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);

        attachedItem = false;
        tether.ClearTethers();
    }

    public void ThrowItem()
    {
        weaponRB.isKinematic = false;

        // weaponRB.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);
        weaponRB.AddForce(transform.forward * weaponData.Weight, ForceMode.Impulse);

        attachedItem = false;
        tether.ClearTethers();
    }

    void ReversePosition(float distance)
    {
        float weightedSpeed = weaponData.BaseFollowSpeed + distance * weaponData.Weight;

        weaponRoot.position = Vector3.MoveTowards(
            weaponRoot.position,
            nodeOne.position,
            -weightedSpeed * Time.deltaTime
        );
    }

    public IEnumerator PushBack()
    {
        while (true)
        {
            // Debug.Log("Pushing back");
            if ((weaponRoot != null) && facingEnvironment) ReversePosition(distance);
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            Debug.Log("On");
            blocked = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            Debug.Log("Off");
            blocked = false;
        }
    }
}