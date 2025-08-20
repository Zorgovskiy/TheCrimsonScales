using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BeaconOfHope : HierophantCardModel<BeaconOfHope.CardTop, BeaconOfHope.CardBottom>
{
	public override string Name => "Beacon of Hope";
	public override int Level => 4;
	public override int Initiative => 82;
	protected override int AtlasIndex => 29 - 18;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new HealAbility(3, range: 3)),

			new AbilityCardAbility(new AttackAbility(2,
				customGetTargets: (state, list) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					foreach(Figure targetedFigure in attackAbilityState.UniqueTargetedFigures)
					{
						if(!targetedFigure.IsDead)
						{
							foreach(Figure adjacentFigure in RangeHelper.GetFiguresInRange(targetedFigure.Hex, 1))
							{
								list.Add(adjacentFigure);
							}
						}
					}
				},
				conditionalAbilityCheck: state => AbilityCmd.HasPerformedAbility(state, 0)
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Light];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new ConditionAbility([Conditions.Bless, Conditions.Bless], range: 3)),

			new AbilityCardAbility(new UseSlotAbility([new UseSlot(new Vector2(0.5f, 0.9f))],
				async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApply: canApplyParameters =>
							state.Performer.AlliedWith(canApplyParameters.Performer) &&
							canApplyParameters.AMDCard is BlessAMDCard,
						apply: async applyParameters =>
						{
							ScenarioEvents.AfterAttackDamageSufferedEvent.Subscribe(state, this,
								canApply: canApplyParameters =>
									canApplyParameters.AbilityState == applyParameters.AbilityState,
								apply: async parameters =>
								{
									ScenarioEvents.AfterAttackDamageSufferedEvent.Unsubscribe(state, this);

									ActionState actionState = new ActionState(parameters.Performer, [new HealAbility(6, target: Target.Self)]);

									await actionState.Perform();

									await state.AdvanceUseSlot();

									await GDTask.CompletedTask;
								}
							);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override int XP => 1;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}