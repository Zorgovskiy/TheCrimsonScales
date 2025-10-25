using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class MasterTheReins : ChieftainCardModel<MasterTheReins.CardTop, MasterTheReins.CardBottom>
{
	public override string Name => "Master the Reins";
	public override int Level => 9;
	public override int Initiative => 30;
	protected override int AtlasIndex => 27;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					MoveAbility.Builder()
						.WithDistance(1)
						.WithOnAbilityStarted(async moveState =>
						{
							moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build(),

					AttackAbility.Builder()
						.WithDamage(1)
						.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
							parameters => parameters.Performer == grantState.Target,
							async parameters =>
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(((Summon)parameters.Performer).Stats.Attack ?? 0);

								int range = ((Summon)parameters.Performer).Stats.Range ?? 1;
								parameters.AbilityState.SingleTargetAdjustRange(range - 1);
								parameters.AbilityState.SingleTargetSetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);

								await GDTask.CompletedTask;
							}
						))
						.Build(),

					MoveAbility.Builder()
						.WithDistance(1)
						.WithOnAbilityStarted(async moveState =>
						{
							moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build(),
				])
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons);
				})
				.WithTarget(Target.Allies | Target.TargetAll)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					AbilityCard selectedAbilityCard =
						await AbilityCmd.SelectAbilityCard((Character)state.Performer, CardState.PersistentLoss,
							canSelectFunc: abilityCard => abilityCard.Top.Abilities.Concat(abilityCard.Bottom.Abilities).Any(cardAbility => cardAbility.Ability is SummonAbility),
							hintText: $"Select an active card with summon ability to attach to");

					Summon summon = ((SummonAbility.State)selectedAbilityCard.ActiveActionStates
						.SelectMany(actionState => actionState.AbilityStates)
						.FirstOrDefault(abilityState => abilityState is SummonAbility.State)).Summon;

					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						canApplyParameters => summon == canApplyParameters.Performer,
						async applyParameters =>
						{
							applyParameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						}
					);

					ScenarioCheckEvents.IsSummonControlledCheckEvent.Subscribe(state, this,
						parameters => parameters.Summon == summon,
						parameters =>
						{
							parameters.SetIsControlled();
						}
					);

					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApply: parameters => parameters.Figure == summon,
						apply: async parameters =>
						{
							ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

							await state.ActionState.RequestDiscardOrLose();
						}
					);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.IsSummonControlledCheckEvent.Unsubscribe(state, this);
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override bool Persistent => true;
	}
}