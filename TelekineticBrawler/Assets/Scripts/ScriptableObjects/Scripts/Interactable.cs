using System;
using System.Collections;
using UnityEngine;


public interface IInteractable
{
    bool IsLooking { get; set; }
    bool IsHeld { get; set; }
    void Interact();
    void Drop();
}

// Source - https://stackoverflow.com/a/76617510
// Posted by derHugo, modified by community. See post 'Timeline' for change history
// Retrieved 2026-03-11, License - CC BY-SA 4.0

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private Outline outline;
    // adjust delays in seconds
    [SerializeField] private float outlineEnableDelay = 1f;
    [SerializeField] private float outlineDisableDelay = 1f;

    // stores currently running routine (see below)
    private Coroutine lookingRoutine;
    // backing field for the IsLooking property
    private bool isLooking;
    public bool IsHeld { get; set; }

    public event Action Held;
    public event Action Dropped;


    private void Awake()
    {
        if (!outline) outline = GetComponent<Outline>();
        outline.enabled = false;
    }

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
        Held?.Invoke();
    }

    public void Drop()
    {
        if (!IsHeld) { return; }
        Debug.Log($"Dropped item {name}", this);
        IsHeld = false;
        Dropped?.Invoke();
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
}

