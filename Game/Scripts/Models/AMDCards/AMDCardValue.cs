using Fractural.Tasks;
using Godot;

public class AMDCardValue
{
	public bool IsCrit = false;
	public bool IsNull = false;
	public int? Value = 0;

	public AMDCardValue(bool isCrit, bool isNull, int? value)
	{
		IsCrit = isCrit;
		IsNull = isNull;
		Value = value;
	}

	public async GDTask Apply(AttackAbility.State attackAbilityState)
	{
		ScenarioEvents.AMDCardValueApplied.Parameters amdCardValueAppliedParameters =
			await ScenarioEvents.AMDCardValueAppliedEvent.CreatePrompt(
				new ScenarioEvents.AMDCardValueApplied.Parameters(attackAbilityState, this), attackAbilityState);

			int adjustedValue = amdCardValueAppliedParameters.AMDCardValue.GetModifiedAttackValue(attackAbilityState);
			attackAbilityState.SingleTargetAdjustAttackValue(adjustedValue);
	}

	protected int GetModifiedAttackValue(AttackAbility.State attackAbilityState)
	{
		int adjustedAttackValue = attackAbilityState.AbilityAttackValue;
		if(IsCrit)
		{
			adjustedAttackValue += attackAbilityState.AbilityAttackValue;
		}
		else if(IsNull)
		{
			adjustedAttackValue = 0;
		}
		else if(Value.HasValue)
		{
			adjustedAttackValue += Value.Value;
		}
		return adjustedAttackValue;
	}

	public (int, bool) GetScore(AttackAbility.State attackAbilityState)
	{
		return (GetModifiedAttackValue(attackAbilityState), false);
	}
}