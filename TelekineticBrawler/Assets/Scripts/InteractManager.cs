using UnityEngine;

public class InteractManager : MonoBehaviour
{
    // Source - https://stackoverflow.com/a/76617510
    // Posted by derHugo, modified by community. See post 'Timeline' for change history
    // Retrieved 2026-03-11, License - CC BY-SA 4.0

    // store currently focused instance!

    [Header("Raycast Settings")]
    [SerializeField] float maxDistance;
    private IInteractable currentInteractable;

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
        // in general I'd use vars .. no need to have class fields for those
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, maxDistance))
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
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentInteractable == null) return;
            currentInteractable.Drop();
        }
    }

}
