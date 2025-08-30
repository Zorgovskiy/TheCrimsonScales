﻿using System;
using Fractural.Tasks;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that reduces the damage suffered from attacks.
/// </summary>
public class ShieldAbility : ActiveAbility<ShieldAbility.State>
{
	public class State : ActiveAbilityState
	{
	}

	private Func<ScenarioEvents.SufferDamage.Parameters, bool> _customCanApply;
	private bool _customCanApplyReplaceFully;

	public DynamicInt<State> ShieldValue { get; private set; }
	public RangeType? RequiredRangeType { get; private set; }
	public bool Pierceable { get; private set; } = true;

	public bool ConditionalValue => RequiredRangeType.HasValue || _customCanApply != null;

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in ShieldAbility. Enables inheritors of ShieldAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending ShieldAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IShieldValueStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ShieldAbility, new()
	{
		public interface IShieldValueStep
		{
			TBuilder WithShieldValue(DynamicInt<State> shieldValue);
		}

		public TBuilder WithCustomCanApply(Func<ScenarioEvents.SufferDamage.Parameters, bool> customCanApply)
		{
			Obj._customCanApply = customCanApply;
			return (TBuilder)this;
		}

		public TBuilder WithCustomCanApplyReplaceFully(bool customCanApplyReplaceFully)
		{
			Obj._customCanApplyReplaceFully = customCanApplyReplaceFully;
			return (TBuilder)this;
		}

		public TBuilder WithShieldValue(DynamicInt<State> shieldValue)
		{
			Obj.ShieldValue = shieldValue;
			return (TBuilder)this;
		}

		public TBuilder WithRequiredRangeType(RangeType rangeType)
		{
			Obj.RequiredRangeType = rangeType;
			return (TBuilder)this;
		}

		public TBuilder WithPierceable(bool pierceable)
		{
			Obj.Pierceable = pierceable;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class ShieldBuilder : AbstractBuilder<ShieldBuilder, ShieldAbility>
	{
		internal ShieldBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of ShieldBuilder.
	/// </summary>
	/// <returns></returns>
	public static ShieldBuilder.IShieldValueStep Builder()
	{
		return new ShieldBuilder();
	}

	public ShieldAbility() { }

	protected override async GDTask Perform(State abilityState)
	{
		await AskConfirmAndActivate(abilityState);
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);

		ScenarioCheckEvents.ShieldCheckEvent.Subscribe(abilityState, this,
			parameters =>
				parameters.Figure == abilityState.Performer,
			parameters =>
			{
				if(ConditionalValue)
				{
					parameters.SetExtraValue();
				}
				else
				{
					int shieldValue = ShieldValue.GetValue(abilityState);
					parameters.AdjustShield(shieldValue);
				}
			}
		);

		//abilityState.Performer.UpdateShield();

		ScenarioEvents.SufferDamageEvent.Subscribe(abilityState, this,
			parameters =>
			{
				bool canApply =
					parameters.Figure == abilityState.Performer && parameters.FromAttack &&
					(!RequiredRangeType.HasValue || parameters.PotentialAttackAbilityState.SingleTargetRangeType == RequiredRangeType);

				if(_customCanApply != null)
				{
					if(_customCanApplyReplaceFully)
					{
						return _customCanApply(parameters);
					}

					canApply = canApply && _customCanApply(parameters);
				}

				return canApply;
			},
			async parameters =>
			{
				int shieldValue = ShieldValue.GetValue(abilityState);

				if(Pierceable)
				{
					parameters.AdjustShield(shieldValue);
				}
				else
				{
					parameters.AdjustUnpierceableShield(shieldValue);
				}

				await GDTask.CompletedTask;
			}
		);

		AppController.Instance.AudioController.PlayFastForwardable(SFX.Shield, delay: 0f);
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(abilityState, this);

		//abilityState.Performer.UpdateShield();

		ScenarioEvents.SufferDamageEvent.Unsubscribe(abilityState, this);
	}

	protected override string DefaultHintText(State abilityState)
	{
		return $"Perform the {Icons.HintText(Icons.Shield)} ability?";
	}
}