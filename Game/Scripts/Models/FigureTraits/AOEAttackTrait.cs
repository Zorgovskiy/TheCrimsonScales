using Fractural.Tasks;

public class AOEAttackTrait(AOEPattern aoePattern) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.AbilityStartedEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && parameters.AbilityState is AttackAbility.State,
			async parameters =>
			{
				//add aoe
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