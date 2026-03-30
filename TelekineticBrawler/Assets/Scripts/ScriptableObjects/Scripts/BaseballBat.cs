using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "Objects/BaseballBat")]
class BaseballBat : WeaponData
{
	public override void UseAbility(AbilityInfo info)
	{
		Debug.Log("BaseballBat Ability Activated");
		
		// Ability logic? Or maybe just a trigger?
	}
	
	// Break consistent across all items & objects, so no need to declare?
	
}
