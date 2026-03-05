using UnityEngine;

public class ItemPickup : Interactable // Attached to weapon/object prefabs
{
	[SerializeField] private WeaponData weaponData;
	
	public override void Interact()
	{
		// Uniform pickup logic. Variables come from weaponData
	}
}
