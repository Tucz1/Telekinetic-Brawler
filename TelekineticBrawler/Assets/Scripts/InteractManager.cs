using UnityEngine;
using UnityEngine.UI;

public class InteractManager : MonoBehaviour
{

    [Header("UI")]

    [SerializeField] Slider tempChargeSlider;

    [Header("Throw Settings")]
    private float timeHeld;

    [Header("Raycast Settings")]
    [SerializeField] float maxDistance;
    private bool holding;
    private IInteractable cachedInteractable;
    private IInteractable currentInteractable;
    TimeWarp timeWarp;
    TelekinesisController telekinesis;

    void Awake()
    {
        timeWarp = FindAnyObjectByType<TimeWarp>();
        telekinesis = FindAnyObjectByType<TelekinesisController>();
    }

    private void SetInteractable(IInteractable interactable)
    {
        // if is same instance (or both null) -> ignore
        if (currentInteractable == interactable) return;

        // otherwise if current focused exists -> reset
        if (currentInteractable != null) currentInteractable.IsLooking = false;

        // store new focused
        currentInteractable = interactable;

        // if not null -> set looking
        if (currentInteractable != null) currentInteractable.IsLooking = true;
    }

    void Update()
    {
        tempChargeSlider.value = Mathf.InverseLerp(0, telekinesis.maxThrowChargeDuration, timeHeld);
        // in general I'd use vars .. no need to have class fields for those
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, maxDistance) && !holding && !telekinesis.isThrowing)
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                // hitting an IInteractable -> store
                SetInteractable(interactable);
            }
            else
            {
                // hitting something that is not IInteractable -> reset
                SetInteractable(null);
            }
        }
        else
        {
            // hitting nothing at all -> reset
            SetInteractable(null);
        }

        // if currently focusing an IInteractable and click -> interact
        if (currentInteractable != null && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Mouse0))) // Pickup
        {
            currentInteractable.Interact();
            cachedInteractable = currentInteractable;
            holding = true;
            // Debug.Log(cachedInteractable);
        }

        if (Input.GetKeyDown(KeyCode.G)) // Drop
        {
            if (cachedInteractable == null) return;
            Debug.Log(cachedInteractable);
            SetInteractable(cachedInteractable);
            currentInteractable.Drop();
            holding = false;
        }

        if ((Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Mouse1)) && holding) // Throw
        {
            if (cachedInteractable == null) return;
            telekinesis.isThrowing = true;
            timeHeld += Time.deltaTime;
        }
        if ((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Mouse1)) && holding)
        {
            if (cachedInteractable == null) return;
            // Debug.Log($"Throwing item held for: {timeHeld}");
            // Debug.Log(cachedInteractable);
            SetInteractable(cachedInteractable);
            currentInteractable.Throw(timeHeld);
            timeHeld = 0;
            telekinesis.isThrowing = false;
            holding = false;
        }
    }

    public void RemoveFromHand()
    {

        if (cachedInteractable == null) return;

        SetInteractable(cachedInteractable);

        if (telekinesis.isThrowing) { telekinesis.isThrowing = false; timeHeld = 0; }
        holding = false;
    }

}
