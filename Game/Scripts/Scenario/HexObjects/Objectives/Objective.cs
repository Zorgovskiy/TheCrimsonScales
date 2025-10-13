﻿using System.Collections.Generic;
using Fractural.Tasks;

public partial class Objective : Figure
{
	private string _name;

	public override string DisplayName => _name;
	public override string DebugName => _name;

	public override AMDCardDeck AMDCardDeck => null;

	public void Init(int health, string name)
	{
		_name = name;

		SetMaxHealth(health);
		SetHealth(health);
	}

	public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		await base.Init(originHex, rotationIndex, hexCanBeNull);

		SetAlignment(Alignment.Enemies);
		SetEnemies(Alignment.Characters);

		GameController.Instance.Map.RegisterFigure(this);

		UpdateInitiative();

		ScenarioEvents.InflictConditionEvent.Subscribe(this, this,
			parameters => parameters.Target == this,
			async parameters =>
			{
				parameters.SetPrevented(true);

				await GDTask.CompletedTask;
			}
		);

		ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this, this,
			parameters =>
				parameters.PotentialTarget == this &&
				parameters.PotentialAbilityState != null &&
				parameters.PotentialAbilityState is not AttackAbility.State,
			parameters =>
			{
				parameters.SetCannotBeTargeted();
			}
		);

		ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Subscribe(this, this,
			parameters => parameters.Figure == this,
			parameters =>
			{
				parameters.SetImmuneToForcedMovement();
			}
		);
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		ScenarioEvents.InflictConditionEvent.Unsubscribe(this, this);
		ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(this, this);
		ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Unsubscribe(this, this);
	}

	public override void RoundEnd()
	{
		// Don't set CanTakeTurn to true, as objectives can never take turns
	}

	protected override Initiative GetInitiative()
	{
		return new Initiative()
		{
			MainInitiative = 99,
			SortingInitiative = 99 * 10000000
		};
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new ObjectiveInfoItem.Parameters(this));
	}
}