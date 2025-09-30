using Fractural.Tasks;

public class Infect : ConditionModel
{
	public override string Name => "Infect";
	public override string IconPath => "res://Art/Icons/ConditionsAndEffects/Infect.svg";
	public override bool RemovedByHeal => true;
	public override bool IsNegative => true;

	public override async GDTask Add(Figure target, ConditionNode node)
	{
		await base.Add(target, node);

		ScenarioCheckEvents.ShieldCheckEvent.Subscribe(Owner, this,
			canApplyParameters =>
				canApplyParameters.Figure == target,
			applyParameters =>
			{
				applyParameters.SetPrevented();
			}
			, order: 100
		);

		ScenarioEvents.SufferDamageEvent.Subscribe(Owner, this,
			canApplyParameters => canApplyParameters.Figure == target && canApplyParameters.FromAttack,
			async applyParameters =>
			{
				applyParameters.SetShieldPrevented();

				await GDTask.CompletedTask;
			}
			, order: 100
		);
	}

	public override async GDTask Remove()
	{
		await base.Remove();

		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(Owner, this);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(Owner, this);
	}
}