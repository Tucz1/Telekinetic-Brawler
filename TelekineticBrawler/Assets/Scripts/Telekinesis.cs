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

    public bool attachedItem;
    bool facingEnvironment;
    bool blocked;
    TetherBundle tether;
    PauseMenu pauseMenu;
    public bool isThrowing;
    [SerializeField] private bool hitEnemy;
    [SerializeField] private WaitForSeconds staggerWaitForSeconds = new WaitForSeconds(0.08f);
    // Interactable interactable;

    void Awake()
    {

        tether = GetComponent<TetherBundle>();
        pauseMenu = FindAnyObjectByType<PauseMenu>();
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

        if (pauseMenu.ispaused) return;
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

        UpdateHandRoll();

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
        float weightedSpeed = !hitEnemy ? weaponData.BaseFollowSpeed + distance * weaponData.Weight :
                                        (weaponData.BaseFollowSpeed + distance * weaponData.Weight) / 10f;

        if ((!blocked && !facingEnvironment) || !facingEnvironment)
            weaponRoot.position = Vector3.MoveTowards(
                weaponRoot.position,
                nodeOne.position,
                weightedSpeed * Time.deltaTime
            );
    }

    void UpdateRotation(Vector3 direction, float distance, float normalizedDistance)
    {
        Quaternion targetRotation = Quaternion.identity;

        // if (!isThrowing)
        // {
            targetRotation =
            distance > weaponData.Deadzone
            ? Quaternion.LookRotation(direction)
            : Quaternion.LookRotation(mainCam.transform.forward);


        // }
        // else
        // {
        //     targetRotation = ;
        //     weaponRoot.rotation = targetRotation;
        // }


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
            Quaternion.LookRotation(mainCam.transform.up),
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

        nodeOne.position = Vector3.up * weaponData.HoldHeightOffset;

        lastTargetPos = weaponRoot.position;


        weaponRB.isKinematic = true;
        weaponRB.interpolation = RigidbodyInterpolation.None;
        weaponLogic.localPosition = Vector3.zero;
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
        weaponRB.AddForce(transform.forward * force, ForceMode.Impulse);

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