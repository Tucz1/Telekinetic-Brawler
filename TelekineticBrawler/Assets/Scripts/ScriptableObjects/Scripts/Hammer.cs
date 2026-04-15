using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Objects/Hammer")]
class Hammer : WeaponData
{
	public override void UseAbility(AbilityInfo info)
	{
		Debug.Log("Hammer Ability Activated");
		
		// Ability logic? Or maybe just a trigger?
	}
	
	// Break consistent across all items & objects, so no need to declare?
	
}