using System.Collections.Generic;
using Fractural.Tasks;

public class OutrunTheEnemy : ChieftainCardModel<OutrunTheEnemy.CardTop, OutrunTheEnemy.CardBottom>
{
	public override string Name => "Outrun the Enemy";
	public override int Level => 1;
	public override int Initiative => 87;
	protected override int AtlasIndex => 1;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 4,
					Move = 3,
					Attack = 1,
					Traits = 
					[
						new MountTrait(AbilityCard.OriginalOwner, 
							async mountSummon => 
							{
								ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(mountSummon, this,
									parameters => parameters.Figure == mountSummon,
									parameters => parameters.AdjustInitiative(-10)
								);
								await GDTask.CompletedTask;
							},
							async mountSummon => 
							{ 
								ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(mountSummon, this);
								await GDTask.CompletedTask;
							}
						)
					]
				})
				.WithName("Speedy Ostrich")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/speedy_ostrich_AI.png")
				.Build()
			),
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(grantState =>
				[
					MoveAbility.Builder()
						.WithDistance(0)
						.WithOnAbilityStarted(async moveState =>
						{
							Summon summonToMove = AbilityCard.OriginalOwner.Summons.Find(summon => moveState.Performer == summon);
							moveState.AdjustMoveValue(summonToMove?.Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.WithDuringMovementSubscription(
							ScenarioEvents.DuringMovement.Subscription.ConsumeElement(Element.Earth,
								applyFunction: async applyParameters =>
								{
									applyParameters.AbilityState.AdjustMoveValue(2);

									await AbilityCmd.GainXP(applyParameters.Performer, 1);
								},
								effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")
							)
						)
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(AbilityCard.OriginalOwner.Summons);
				})
				.Build()
			),
		];
	}
}