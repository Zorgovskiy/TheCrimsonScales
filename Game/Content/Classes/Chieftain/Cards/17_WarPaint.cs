using System.Collections.Generic;
using Fractural.Tasks;

public class WarPaint : ChieftainCardModel<WarPaint.CardTop, WarPaint.CardBottom>
{
	public override string Name => "War Paint";
	public override int Level => 4;
	public override int Initiative => 28;
	protected override int AtlasIndex => 17;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					await AbilityCmd.AddCondition(state, state.Performer, Conditions.Invisible);

					ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
						ScenarioCheckEvents.IsMountedCheckEvent.Fire(
							new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

					if(isMountedCheckParameters.IsMounted)
					{
						await AbilityCmd.AddCondition(state, isMountedCheckParameters.Mount, Conditions.Invisible);
						state.SetCustomValue(this, "Mount", isMountedCheckParameters.Mount);
					}

					state.SetCustomValue(this, "IsMounted", isMountedCheckParameters.IsMounted);
				})
				.WithOnDeactivate(async state => 
				{
					await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible);

					if(state.GetCustomValue<bool>(this, "IsMounted"))
                    {
                        await AbilityCmd.RemoveCondition(state.GetCustomValue<Figure>(this, "Mount"), Conditions.Invisible);
                    }
				})
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];

		protected override bool Round => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.PotentialTargetCheckEvent.Subscribe(state, this,
						parameters => parameters.PotentialTarget == state.Performer,
						parameters =>
						{
							ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
								ScenarioCheckEvents.IsMountedCheckEvent.Fire(
									new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

							if(isMountedCheckParameters.IsMounted)
							{
								parameters.AdjustTargetSortingInitiative(-10);
							}
						}
					);

					ScenarioEvents.NextActiveFigureEvent.Subscribe(state, this,
						parameters =>
						{
							if(parameters.PreviousActiveFigure == state.Performer)
                            {
                                return false;
                            }

							ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
								ScenarioCheckEvents.IsMountedCheckEvent.Fire(
									new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));
							return isMountedCheckParameters.IsMounted && isMountedCheckParameters.Mount == parameters.NextActiveFigure;
						},
						async parameters =>
						{
							Figure mount = ScenarioCheckEvents.IsMountedCheckEvent.Fire(
									new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer)).Mount;

							ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(state, this,
								parameters => parameters.Figure == mount,
								parameters => parameters.SetSortingInitiative(state.Performer.Initiative.SortingInitiative + 1),
								order: 10
							);

							mount.UpdateInitiative();
							parameters.SetSortingRequired();

							ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						},
						effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters("res://Content/Classes/Chieftain/Icon.svg"),
						effectInfoViewParameters: new TextEffectInfoView.Parameters("Act before your mounted summon.")
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					state.Performer.UpdateInitiative();

					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.NextActiveFigureEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}