using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class TakeTheReins : ChieftainCardModel<TakeTheReins.CardTop, TakeTheReins.CardBottom>
{
	public override string Name => "Take the Reins";
	public override int Level => 3;
	public override int Initiative => 40;
	protected override int AtlasIndex => 15;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					AttackAbility.Builder()
						.WithDamage(1)
						.WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
							parameters => parameters.Performer == grantState.Target,
							async parameters =>
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(((Summon)parameters.Performer).Stats.Attack ?? 0);

								int range = ((Summon)parameters.Performer).Stats.Range ?? 1;
								parameters.AbilityState.SingleTargetAdjustRange(range);
								parameters.AbilityState.SingleTargetSetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);

								ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
									ScenarioCheckEvents.IsMountedCheckEvent.Fire(
										new ScenarioCheckEvents.IsMountedCheck.Parameters(grantState.Performer));

								if(isMountedCheckParameters.IsMounted && isMountedCheckParameters.Mount == parameters.Performer)
								{
									parameters.AbilityState.SingleTargetAdjustAttackValue(2);
								}

								await GDTask.CompletedTask;
							})
						)
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(((Character)grantState.Performer).Summons
						.Where(summon => RangeHelper.Distance(grantState.Performer.Hex, summon.Hex) <= 3));
				})
				.WithTarget(Target.Allies)
				.WithRange(3)
				.Build()
			),
		];
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(4).Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
						ScenarioCheckEvents.IsMountedCheckEvent.Fire(
							new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

					return isMountedCheckParameters.IsMounted;
				})
				.Build()
			)
		];
	}
}