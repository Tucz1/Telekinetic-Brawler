using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Objects/Morningstar")]
class Morningstar : WeaponData
{
	public override void UseAbility(AbilityInfo info)
	{
		Debug.Log("Morningstar Ability Activated");
		
		// Ability logic? Or maybe just a trigger?
	}
	
	// Break consistent across all items & objects, so no need to declare?
	
}