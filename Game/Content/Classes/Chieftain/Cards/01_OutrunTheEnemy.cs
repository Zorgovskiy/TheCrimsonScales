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
						MountTrait.Builder()
							.WithCharacterOwner(AbilityCard.OriginalOwner)
							.WithOnMounted(async characterOwner => 
							{ 
								ScenarioCheckEvents.InitiativeCheckEvent.Subscribe(characterOwner, this,
									parameters => parameters.Figure == characterOwner,
									parameters => parameters.AdjustInitiative(-10)
								);
								await GDTask.CompletedTask;
							})
							.WithOnUnmounted(async characterOwner => 
							{ 
								ScenarioCheckEvents.InitiativeCheckEvent.Unsubscribe(characterOwner, this);
								await GDTask.CompletedTask;
							})
							.Build(),
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
							
							if(await AbilityCmd.AskConsumeElement(grantState.Performer, Element.Earth,
						   		$"+2{Icons.Inline(Icons.Move)}"))
							{
								moveState.AdjustMoveValue(2);
								await AbilityCmd.GainXP(grantState.Performer, 1);
							}

							await GDTask.CompletedTask;
						})
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(AbilityCard.OriginalOwner.Summons);
				})
				.WithTarget(Target.Allies)
				.Build()
			),
		];
	}
}