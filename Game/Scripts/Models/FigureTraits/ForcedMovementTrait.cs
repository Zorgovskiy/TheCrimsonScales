using Fractural.Tasks;

public class ForcedMovementTrait(ForcedMovementType type, int amount) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters =>
				parameters.AbilityState is AttackAbility.State &&
				parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				AttackAbility.State attackAbilityState = (AttackAbility.State)parameters.AbilityState;

				if(type == ForcedMovementType.Push)
				{
					attackAbilityState.AbilityAdjustPush(amount);
				}
				else if(type == ForcedMovementType.Pull)
				{
					attackAbilityState.AbilityAdjustPull(amount);
				}
				else if(type == ForcedMovementType.Swing)
				{
					attackAbilityState.AbilityAdjustSwing(amount);
				}

				await GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.AbilityStartedEvent.Unsubscribe(figure, this);
	}
}