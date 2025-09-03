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

			int adjustedValue = amdCardValueAppliedParameters.AMDCardValue.GetAttackModifierValue(attackAbilityState);
			attackAbilityState.SingleTargetAdjustAttackValue(adjustedValue);
	}

	protected int GetAttackModifierValue(AttackAbility.State attackAbilityState)
	{
		int attackModifierValue = 0;
		if(IsCrit)
		{
			attackModifierValue = attackAbilityState.AbilityAttackValue;
		}
		else if(IsNull)
		{
			attackModifierValue = -attackAbilityState.AbilityAttackValue;
		}
		else if(Value.HasValue)
		{
			attackModifierValue = Value.Value;
		}
		return attackModifierValue;
	}

	public (int, bool) GetScore(AttackAbility.State attackAbilityState)
	{
		return (GetAttackModifierValue(attackAbilityState), false);
	}
}