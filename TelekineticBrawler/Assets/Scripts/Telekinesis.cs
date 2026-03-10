using UnityEngine;

public class TelekinesisController : MonoBehaviour
{
    [Header("References")]
    public Transform nodeTwoRoot;
    public Transform nodeTwo;
    public Camera mainCam;

    [SerializeField] private Transform nodeOne;

    [Header("Weapon")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Rigidbody weaponRB;

    private Vector3 lastTargetPos;

    [Header("Player Movement Compensation")]
    [SerializeField] Transform player;
    [SerializeField] float movementInfluence = 1f;

    Vector3 lastPlayerPos;
    Vector3 playerVelocity;
    private Vector3 lastDir;

    public bool attachedItem;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;

        lastTargetPos = nodeTwoRoot.position;

        lastPlayerPos = player.position;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (attachedItem)
            {
                Debug.Log("Removing item");
                RemoveItem();
            }
            else { Debug.Log("Attaching item"); AttachItem(); }
        }
        playerVelocity = (player.position - lastPlayerPos) / Time.deltaTime;
        lastPlayerPos = player.position;

        UpdateTargetPosition();

        if (!attachedItem) return;

        Vector3 direction = nodeOne.position - nodeTwoRoot.position;
        lastDir = direction;
        float distance = direction.magnitude;
        float normalizedDistance = Mathf.Clamp01(distance / weaponData.MaxDistance);

        UpdatePosition(distance);
        UpdateRotation(direction, distance, normalizedDistance);
        UpdateRoll();

        nodeTwoRoot.position += playerVelocity * Time.deltaTime * movementInfluence;

        lastTargetPos = nodeOne.position;
    }

    void UpdateTargetPosition()
    {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 2f);
        nodeOne.position = mainCam.ScreenToWorldPoint(screenCenter);
    }


    void UpdatePosition(float distance)
    {
        float weightedSpeed = weaponData.BaseFollowSpeed + distance * weaponData.Weight;

        nodeTwoRoot.position = Vector3.MoveTowards(
            nodeTwoRoot.position,
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

        nodeTwoRoot.rotation = Quaternion.Slerp(
            nodeTwoRoot.rotation,
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

        nodeTwo.localRotation = Quaternion.Lerp(
            nodeTwo.localRotation,
            targetRoll,
            weaponData.RollSmoothSpeed * Time.deltaTime
        );
    }


    void AttachItem()
    {
        weaponRB.isKinematic = true;
        weaponTransform.SetParent(nodeTwo);
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;

        attachedItem = true;
    }

    void RemoveItem()
    {
        weaponRB.isKinematic = false;
        weaponTransform.SetParent(null);

        weaponRB.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);

        attachedItem = false;
    }
}