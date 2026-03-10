using UnityEngine;

public enum DamageType
{
    Blunt,
    Slashing,
    Piercing
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

	[SerializeField] protected float damage;

	[Header("Movement")]
    [SerializeField] protected float baseFollowSpeed = 2f;
    [SerializeField] protected float weight = 6f;

    [Header("Rotation")]
    [SerializeField] protected float baseRotationSpeed = 10f;
    [SerializeField] protected float maxRotationSpeed = 20f;
    [SerializeField] protected float maxDistance = 5f;
    [SerializeField] protected float deadzone = 0.1f;

    [Header("Tilt")]
    [SerializeField] protected float maxRoll = 90f;
    [SerializeField] protected float rollSensitivity = 10f;
    [SerializeField] protected float rollSmoothSpeed = 7f;
	
	[Header("SFX")]
	[SerializeField] protected AudioClip hitSFX;
	
	public float Damage => damage;
	public float BaseFollowSpeed => baseFollowSpeed;
	public float Weight => weight;
	public float BaseRotationSpeed => baseRotationSpeed;
	public float MaxRotationSpeed => maxRotationSpeed;
	public float MaxDistance => maxDistance;
	public float Deadzone => deadzone;
	public float MaxRoll => maxRoll;
	public float RollSensitivity => rollSensitivity;
	public float RollSmoothSpeed => rollSmoothSpeed;
	public AudioClip HitSFX => hitSFX;



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
