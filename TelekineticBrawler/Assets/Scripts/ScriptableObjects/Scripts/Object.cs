using UnityEngine;

public enum DamageType
{
    Blunt,
    Slashing,
    Piercing
}

public enum Size
{
    Small,
    Medium,
    Big,
    Huge
}

public struct AbilityInfo
{
    // public data
    // public AbilityInfo()
    // {
        
    // }
}

// [CreateAssetMenu(fileName = "Object", menuName = "Scriptable Objects/Object")]
public class WeaponData : ScriptableObject
{
    [Header("WeaponType")]
	[SerializeField] protected DamageType damageType;
	
	[Header("Variables")]
	[SerializeField] protected float damage;
	[SerializeField] protected Size size;
	
	[Header("SFX")]
	[SerializeField] AudioClip hitSFX;
	
	public float Damage => damage;
	public Size Size => size;


	public virtual void UseAbility(AbilityInfo info)
	{
		Debug.Log("No Ability Assigned");
	}
	
	public void BreakItem()
	{
		Debug.Log("Breaking Item");
		
		// Break logic
	}
}
