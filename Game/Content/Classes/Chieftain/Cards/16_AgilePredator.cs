using System.Collections.Generic;
using Fractural.Tasks;

public class AgilePredator : ChieftainCardModel<AgilePredator.CardTop, AgilePredator.CardBottom>
{
	public override string Name => "Agile Predator";
	public override int Level => 3;
	public override int Initiative => 90;
	protected override int AtlasIndex => 16;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 5,
					Move = 3,
					Attack = 1,
					Traits = 
					[
						new RetaliateTrait(1),
						new AllAttacksGainDisadvantageTrait(),
						new MountTrait(AbilityCard.OriginalOwner),
					]
				})
				.WithName("Black Panther")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/black_panther_AI.png")
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
						.WithDistance(1)
						.WithOnAbilityStarted(async moveState =>
						{
							Summon summonToMove = AbilityCard.OriginalOwner.Summons.Find(summon => moveState.Performer == summon);
							moveState.AdjustMoveValue(summonToMove?.Stats.Move ?? 0);

							await GDTask.CompletedTask;
						})
						.Build()
				])				
				.WithCustomGetTargets((grantState, figures) =>
				{
					figures.AddRange(AbilityCard.OriginalOwner.Summons);
				})
				.WithTarget(Target.Allies)
				.WithRange(3)
				.Build()
			),
		];
	}
}