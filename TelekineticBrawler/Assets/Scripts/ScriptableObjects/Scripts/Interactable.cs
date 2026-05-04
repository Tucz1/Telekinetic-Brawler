using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    bool IsLooking { get; set; }
    bool IsHeld { get; set; }
    void Interact();
    void Drop();
    void Throw(float timeHeld);
    void Break();
}


public class Interactable : MonoBehaviour, IInteractable
{
    [Header("Weapon")]
    [SerializeField] WeaponData weaponData;
    [SerializeField] Rigidbody weaponRB;
    [SerializeField] Transform weaponRoot;
    [SerializeField] Transform weaponTransform;
    [SerializeField] Vector3 weaponThrowRotation;
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject brokenWeapon;
    [SerializeField] float minDamageThreshold = 40;
    [SerializeField] float impactFrameDamageThreshold = 500;

    [Header("Outline")]
    [SerializeField] private Outline outline;
    // adjust delays in seconds
    [SerializeField] private float outlineEnableDelay = 0f;
    [SerializeField] private float outlineDisableDelay = 0f;

    // stores currently running routine (see below)
    private Coroutine lookingRoutine;
    // backing field for the IsLooking property
    private bool isLooking;
    public bool IsHeld { get; set; }

    TelekinesisController telekinesis;
    TimeWarp timeWarp;
    public event Action ImpactFrame;

    // private Vector3 lastPos;
    private Vector3 currentVelocity;
    private Vector3 trueVelocity;
    private Vector3 lastVelocity;

    IEnumerator pushRoutine = null;
    bool isPushing;



    private void Awake()
    {
        if (!outline) outline = GetComponent<Outline>();
        outline.enabled = false;

        telekinesis = FindFirstObjectByType<TelekinesisController>();
        timeWarp = FindAnyObjectByType<TimeWarp>();

    }

    void Update()
    {
        currentVelocity = weaponTransform.position;

        if (lastVelocity == null) return;

        switch (weaponRB.isKinematic)
        {
            case true:
                trueVelocity = (currentVelocity - lastVelocity) * 100;
                break;

            case false:
                trueVelocity = weaponRB.linearVelocity;
                break;
        }
        
        // Debug.Log($"Velocity: {trueVelocity.magnitude}");
    }

    private void LateUpdate()
    {
        if ((weaponTransform != null) && IsHeld) { lastVelocity = currentVelocity; }
    }

    // void Start()
    // {
    //     lastPos = transform.position;
    // }

    // void FixedUpdate()
    // {
    //     if (lastPos != transform.position)
    //     {
    //         currentSpeed = transform.position - lastPos;
    //         currentSpeed /= Time.deltaTime;
    //         lastPos = transform.position;
    //     }
    //     print(currentSpeed.magnitude);
    // }

    public bool IsLooking
    {
        // when accessing the property simply return the value
        get => isLooking;

        // when assigning the property apply visuals
        set
        {
            if (IsHeld) { isLooking = false; return; }

            // same value ignore to save some work
            if (isLooking == value) return;

            // store the new value in the backing field
            isLooking = value;

            // if one was running cancel the current routine
            if (lookingRoutine != null) StopCoroutine(lookingRoutine);

            // start a new routine to apply the outline delayed
            lookingRoutine = StartCoroutine(EnabledOutlineDelayed(value));
        }
    }

    public void Interact()
    {
        Debug.Log($"Interacted with {name}", this);
        IsHeld = true;
        outline.enabled = false;
        telekinesis.AttachItem(this, weaponData, weaponRB, weaponRoot, weaponTransform, weaponCollider, weaponThrowRotation);
    }

    public void Drop()
    {
        if (!IsHeld) { return; }
        Debug.Log($"Dropped item {name}", this);
        IsHeld = false;
        telekinesis.DropItem();
    }

    public void Throw(float _timeHeld)
    {
        if (!IsHeld) { return; }

        var force = weaponData.MinThrowStrength;
        var timeHeld = _timeHeld; // If the player holds for 2 seconds, the timeheld becomes 3. This is for the exponent, and it is as high as we let the player charge
        Debug.Log($"Threw item {name}", this);
        IsHeld = false;

        var t0 = Mathf.InverseLerp(0, telekinesis.maxThrowChargeDuration, timeHeld);
        var t1 = Mathf.Lerp(0.5f, telekinesis.maxThrowChargeDuration, t0);
        

        Debug.Log($"Lerped Time Held: {t1}");

        force = Mathf.Pow(force, t1);

        Debug.Log($"Force: {force}");

        telekinesis.ThrowItem(force);
    }

    public void Break()
    {
        var obj = Instantiate(brokenWeapon);

        telekinesis.MoveToWeapon(obj);

        Destroy(this.gameObject, 0.2f);
    }


    // This routine simply has an initial delay and then
    // applies the target state to the outline
    private IEnumerator EnabledOutlineDelayed(bool enable)
    {
        // wait for the according delay - you can of course adjust this according to your needs
        yield return new WaitForSeconds(enable ? outlineEnableDelay : outlineDisableDelay);

        // apply state
        outline.enabled = enable;

        // reset the routine field just to be sure
        lookingRoutine = null;
    }

    void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.CompareTag("Enemy"))
        {
            var limb = collision.gameObject.GetComponentInChildren<LimbScript>();
            var scoreManager = FindAnyObjectByType<ScoreManager>();

            if (limb == null) { Debug.LogError($"Limb script not found {collision.gameObject}"); return; }

            Debug.Log($"Weapon damage type: {weaponData.DamageType}");

            var damage = CalculateDamage();
            var stagger = CalculateStagger(damage);

            if (damage < minDamageThreshold) {Debug.Log("Did not cross damage threshold, returning..."); return;}

            Debug.Log($"Damage dealt: {damage} \n Stagger dealt: {stagger}");

            // If damage || stagger > threshold
            // StartCoroutine(timeWarp.ImpactFrame());

            if (damage > impactFrameDamageThreshold) StartCoroutine(timeWarp.ImpactFrame());

            limb.TakeDamage(damage, stagger, weaponData);
            scoreManager.AddPoints((int)damage + (int)stagger);

            StartCoroutine(telekinesis.OnHit());
        }

        // Deal damage



        // Take damage


        if (collision.gameObject.CompareTag("Environment") && IsHeld && !isPushing)
        {
            isPushing = true;
            telekinesis.canInfluence = false;
            pushRoutine = telekinesis.PushBack();
            telekinesis.StartCoroutine(pushRoutine);
        }

    }

    private float CalculateDamage()
    {
        var damage = trueVelocity.magnitude * weaponData.Damage;
        return damage;
    }

    private float CalculateStagger(float damage)
    {
        float stagger;
        switch (weaponData.DamageType)
        {
            case DamageType.Blunt:
                stagger = damage * 1.5f;
                break;

            case DamageType.Slashing:
                stagger = damage;
                break;

            case DamageType.Piercing:
                stagger = damage * 0.5f;
                break;

            default:
                Debug.LogError("Damage type not found");
                stagger = damage;
                break;
        }

        return stagger;
    }

    // void OnCollisionStay(Collision collision)
    // {

    // }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            if (pushRoutine != null)
            {
                isPushing = false;
                telekinesis.canInfluence = true;
                telekinesis.StopCoroutine(pushRoutine);
            }
        }
    }
}

