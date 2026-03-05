using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Objects/Sword")]
class Sword : WeaponData
{
	public override void UseAbility(AbilityInfo info)
	{
		Debug.Log("Sword Ability Activated");
		
		// Ability logic? Or maybe just a trigger?
	}
	
	// Break consistent across all items & objects, so no need to declare?
	
}
