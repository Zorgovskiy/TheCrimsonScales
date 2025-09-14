﻿using Fractural.Tasks;

public class Immobilize : ConditionModel
{
	public override string Name => "Immobilize";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Immobilize.svg";
	public override bool RemovedAtEndOfTurn => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioEvents.AbilityStartedEvent.Subscribe(Owner, this,
			parameters => parameters.Performer == Owner && parameters.AbilityState is MoveAbility.State,
			parameters =>
			{
				Node.Flash();
				parameters.SetIsBlocked(true);

				return GDTask.CompletedTask;
			},
			EffectType.MandatoryBeforeOptionals);

		ScenarioEvents.CanMoveFurtherCheckEvent.Subscribe(Owner, this,
			parameters => parameters.Performer == Owner,
			parameters =>
			{
				Node.Flash();
				parameters.SetCannotMoveFurther();

				return GDTask.CompletedTask;
			}
		);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(Owner, this);
		ScenarioEvents.CanMoveFurtherCheckEvent.Unsubscribe(Owner, this);
	}
}