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
					await state.Performer.AddCondition(Conditions.Invisible);

					ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
						ScenarioCheckEvents.IsMountedCheckEvent.Fire(
							new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

					if(isMountedCheckParameters.IsMounted)
					{
						await isMountedCheckParameters.Mount.AddCondition(Conditions.Invisible);
					}
				})
				.WithOnDeactivate(async state => 
				{
					await AbilityCmd.RemoveCondition(state.Performer, Conditions.Invisible);
					
					ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
						ScenarioCheckEvents.IsMountedCheckEvent.Fire(
							new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

					if(isMountedCheckParameters.IsMounted)
					{
						await AbilityCmd.RemoveCondition(isMountedCheckParameters.Mount, Conditions.Invisible);
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
						parameters => true,
						parameters => 
						{
							ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
								ScenarioCheckEvents.IsMountedCheckEvent.Fire(
									new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

							if(isMountedCheckParameters.IsMounted && isMountedCheckParameters.Mount == parameters.PotentialTarget)
							{
								parameters.AdjustSortingInitiative(10);
							}
						}
					);

					ScenarioEvents.InitiativesSortedEvent.Subscribe(state, this,
						parameters => true,
						async parameters => 
						{
							ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
								ScenarioCheckEvents.IsMountedCheckEvent.Fire(
									new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

							if(isMountedCheckParameters.IsMounted)
							{
								ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(state, this,
									parameters => parameters.Figure == state.Performer,
									parameters => parameters.SetSortingInitiative(state.Performer.Initiative.SortingInitiative - 10)
								);

								state.Performer.UpdateInitiative();
								ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(state, this);
							}

							await GDTask.CompletedTask;
						}, 
						effectType: EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Active),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Act before your mounted summon.")
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.InitiativesSortedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}