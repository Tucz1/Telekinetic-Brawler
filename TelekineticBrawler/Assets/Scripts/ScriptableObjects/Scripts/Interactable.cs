using UnityEngine;

public interface IInteractable
{
	void Interact();
}

public abstract class Interactable : MonoBehaviour, IInteractable
{
    public abstract void Interact();
}
