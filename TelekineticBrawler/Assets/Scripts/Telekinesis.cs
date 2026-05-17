using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TelekinesisController : MonoBehaviour
{

    [Header("References")]
    [HideInInspector] public Transform weaponRoot;
    private Transform weaponLogic;
    public Camera mainCam;
    [SerializeField] Animator handAnimator;
    [SerializeField] Transform handRotation;
    [SerializeField] float handMaxRoll;

    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private Transform nodeOne;
    [SerializeField] private Transform wallCheck;

    [Header("Weapon")]
    private WeaponData weaponData;
    private Rigidbody weaponRB;
    private Vector3 weaponThrowRotation;
    [Range(0f, 3f)] public float maxThrowChargeDuration = 2f;

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
    private Vector3 previousAxis = Vector3.up;
    [SerializeField] float TargetVectorRotateSpeed = 180f;

    public bool attachedItem;
    bool facingEnvironment;
    TetherBundle tether;
    PauseMenu pauseMenu;
    public bool isThrowing;
    private bool hitEnemy;
    [SerializeField] private WaitForSeconds staggerWaitForSeconds = new WaitForSeconds(0.08f);

    void Awake()
    {

        tether = GetComponent<TetherBundle>();
        pauseMenu = FindAnyObjectByType<PauseMenu>();

    }

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;

        lastPlayerPos = player.position;

    }

    void OnDrawGizmos()
    {
        if (weaponData != null)
            Gizmos.DrawWireSphere(mainCam.transform.position, weaponData.DistanceHeld);
    }

    void Update()
    {

        // if (weaponRoot != null)
        // {
        //     Debug.Log(weaponRoot.position);
        //     Debug.Log(weaponLogic.position);
        //     Debug.Log(nodeOne.position);
        // }



        if (pauseMenu.ispaused) return;
        if (!attachedItem)
        {
            screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            nodeOne.position = mainCam.ScreenToWorldPoint(screenCenter + (Vector3.forward * 2));
            return;
        }

        if (Physics.Linecast(mainCam.transform.position, wallCheck.position, environmentLayer))
        {
            facingEnvironment = true;
        }
        else facingEnvironment = false;


        playerVelocity = (player.position - lastPlayerPos) / Time.deltaTime;
        lastPlayerPos = player.position;



        UpdateTargetPosition();

        Vector3 direction = nodeOne.position - weaponRoot.position;
        Debug.DrawLine(nodeOne.position, weaponRoot.position);
        // Debug.Log($"Direction: {direction}");
        lastDir = direction;
        distance = direction.magnitude;
        float normalizedDistance = Mathf.Clamp01(distance / weaponData.MaxDistance);

        UpdatePosition(distance);



        UpdateRotation(direction, distance, normalizedDistance);
        UpdateRoll();

        UpdateHandRoll();

        if (canInfluence) weaponRoot.position += playerVelocity * Time.deltaTime * movementInfluence;

        lastTargetPos = nodeOne.position;

    }

    void UpdateTargetPosition()
    {
        // Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 2f);
        wallCheck.position = mainCam.ScreenToWorldPoint(screenCenter + (Vector3.forward * weaponData.DistanceHeld));

        if (!facingEnvironment)
        {
            nodeOne.position = mainCam.ScreenToWorldPoint(screenCenter + (Vector3.forward * weaponData.DistanceHeld));
            return;
        }

        RaycastHit hit;

        Vector3 origin = mainCam.transform.position;
        Vector3 direction = (wallCheck.transform.position - origin).normalized;
        float distance = Vector3.Distance(origin, wallCheck.transform.position);

        Debug.DrawRay(origin, direction * distance, Color.cyan);

        if (Physics.Raycast(origin, direction, out hit, distance, environmentLayer))
        {
            nodeOne.position = hit.point;
        }
    }


    void UpdatePosition(float distance)
    {
        float weightedSpeed = !hitEnemy ? weaponData.BaseFollowSpeed + distance * weaponData.Weight :
                                        (weaponData.BaseFollowSpeed + distance * weaponData.Weight) / 10f;

        weaponRoot.position = Vector3.MoveTowards(
            weaponRoot.position,
            nodeOne.position,
            weightedSpeed * Time.deltaTime
        );

        // var direction = weaponRoot.position - mainCam.transform.position;
        // var dist = Vector3.Distance(mainCam.transform.position, weaponRoot.position);
        // var closestPoint = mainCam.transform.position + (direction * (weaponData.DistanceHeld / dist));

        // weaponRoot.position = closestPoint;

        if (distance < weaponData.Deadzone * 5) return;

        Vector3 center = mainCam.transform.position;

        float r = weaponData.DistanceHeld;

        Vector3 currentDir = (weaponRoot.position - center).normalized;

        Vector3 targetDir = (nodeOne.position - center).normalized;

        // I CREATE THE AXIS NOW
        Vector3 axis = Vector3.Cross(currentDir, targetDir);

        // Use previously stored axis if too close to limits
        if (axis.sqrMagnitude < 0.001f)
        {
            axis = previousAxis;
        }
        else previousAxis = axis;

        // Incrimentally adjust towards the target rotation so we don't pull 
        // mariokart wii level shortcuts out our ass
        Quaternion delta =
            Quaternion.AngleAxis(
                TargetVectorRotateSpeed * Time.deltaTime,
                axis.normalized
            );

        Vector3 newDir = delta * currentDir;

        weaponRoot.position = center + newDir * r;
    }

    void UpdateRotation(Vector3 direction, float distance, float normalizedDistance)
    {
        Quaternion targetRotation = Quaternion.identity;

        var upVector = Vector3.Cross(direction, mainCam.transform.right);

        targetRotation =
        distance > weaponData.Deadzone ? Quaternion.LookRotation(direction, upVector)
                                       : Quaternion.LookRotation(mainCam.transform.forward);



        float rotationSpeed = Mathf.Lerp(
            weaponData.BaseRotationSpeed,
            weaponData.MaxRotationSpeed,
            normalizedDistance
        );

        if (!isThrowing)
        {
            weaponRoot.rotation = Quaternion.Slerp(
                weaponRoot.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Quaternion.Euler(mainCam.WorldToScreenPoint(weaponThrowRotation))
            weaponRoot.rotation = Quaternion.Slerp(
            weaponRoot.rotation,
            Quaternion.LookRotation(-mainCam.transform.up, mainCam.transform.forward),
            rotationSpeed * Time.deltaTime);
        }
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

    void UpdateHandRoll()
    {
        Vector3 currentScreenPos = mainCam.WorldToScreenPoint(nodeOne.position);
        Vector3 lastScreenPos = mainCam.WorldToScreenPoint(lastTargetPos);

        Vector3 delta = currentScreenPos - lastScreenPos;

        float rollAmount = Mathf.Clamp(
            -delta.x * weaponData.RollSensitivity,
            -handMaxRoll,
            handMaxRoll
        );

        Quaternion targetRoll = Quaternion.Euler(0f, 0f, -rollAmount);

        handRotation.localRotation = Quaternion.Lerp(
            handRotation.localRotation,
            targetRoll,
            weaponData.RollSmoothSpeed * Time.deltaTime
        );
    }


    public void AttachItem(Interactable _interactable,
                    WeaponData _weaponData,
                    Rigidbody _weaponRB,
                    Transform _weaponRoot,
                    Transform _weaponLogic,
                    Collider _weaponMeshCollider,
                    Vector3 _weaponThrowRotation)
    {

        // _interactable.Held += AttachItem;
        // _interactable.Dropped += RemoveItem;

        weaponData = _weaponData;
        weaponRB = _weaponRB;
        weaponRoot = _weaponRoot;
        weaponLogic = _weaponLogic;
        weaponThrowRotation = _weaponThrowRotation;

        Debug.Log($"Collider: {_weaponMeshCollider}");
        // Debug.LogError("pause");

        hitEnemy = false;


        lastTargetPos = weaponRoot.position;

        // weaponRB.isKinematic = true;
        weaponRB.interpolation = RigidbodyInterpolation.None;
        weaponLogic.localRotation = Quaternion.identity;
        StartCoroutine(CreateNextFrame(weaponLogic, _weaponMeshCollider));

        handAnimator.Play("Pull In_Hand2");

        attachedItem = true;
    }


    IEnumerator CreateNextFrame(Transform a, Collider b)
    {
        yield return null;
        tether.CreateTethers(a, b);
    }

    public void DropItem()
    {
        weaponRB.isKinematic = false;
        weaponRB.interpolation = RigidbodyInterpolation.Interpolate;

        weaponRB.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);

        handAnimator.Play("Drop_Hand2");

        attachedItem = false;
        tether.ClearTethers();
    }

    public void ThrowItem(float force)
    {
        weaponRB.isKinematic = false;

        // weaponRB.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);
        weaponRB.AddForce(mainCam.transform.forward * force, ForceMode.Impulse);

        handAnimator.Play("Push Out_Hand2");

        attachedItem = false;
        tether.ClearTethers();
    }

    public IEnumerator OnHit()
    {
        hitEnemy = true;
        yield return staggerWaitForSeconds;
        hitEnemy = false;
    }

    public void MoveToWeapon(GameObject obj)
    {
        // if (weaponRoot || weaponLogic == null) return;

        weaponRB.isKinematic = false;

        attachedItem = false;
        tether.ClearTethers();

        //weaponRB.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);


        obj.transform.position = weaponRoot.position;
        obj.transform.rotation = weaponLogic.rotation;

        Rigidbody[] partRBs = obj.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody part in partRBs)
        {
            // Debug.Log("Adding force");
            part.AddForce(lastDir * weaponData.Weight, ForceMode.Impulse);
            //Debug.Log(lastDir);
        }

        weaponRoot.gameObject.SetActive(false);
    }
}