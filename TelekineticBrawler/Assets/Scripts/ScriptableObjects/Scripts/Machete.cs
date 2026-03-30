using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Objects/Machete")]
class Machete : WeaponData
{
	public override void UseAbility(AbilityInfo info)
	{
		Debug.Log("Machete Ability Activated");
		
		// Ability logic? Or maybe just a trigger?
	}
	
	// Break consistent across all items & objects, so no need to declare?
	
}
