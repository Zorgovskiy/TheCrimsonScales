using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class RegalBeast : ChieftainCardModel<RegalBeast.CardTop, RegalBeast.CardBottom>
{
	public override string Name => "Regal Beast";
	public override int Level => 9;
	public override int Initiative => 81;
	protected override int AtlasIndex => 28;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 8,
					Move = 3,
					Attack = 3,
					Traits = 
					[
						new AllAttacksGainDisadvantageTrait(),
						new MountTrait(
							async (owner, mount) => 
							{
								ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(owner, this,
									parameters => parameters.AbilityState.Performer == owner,
									async parameters =>
									{
										parameters.SetCannotGainDisadvantage();

										await GDTask.CompletedTask;
									}
								);

								ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(owner, this,
									parameters => parameters.Attacker == owner,
									parameters => parameters.SetDisadvantage(false),
									order: 100
								);

								await GDTask.CompletedTask;
							},
							async (owner, mount) => 
							{ 
								ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(owner, this);
								ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(owner, this);

								await GDTask.CompletedTask;
							}
						),
					]
				})
				.WithName("Sabretooth Tiger")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/sabretooth_tiger_AI.png")
				.Build()
			),
		];

		protected override int XP => 6;
		protected override bool Persistent => true;
		protected override bool Loss => true;
		protected override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					IEnumerable<AbilityCard> selectedAbilityCards =
						await AbilityCmd.SelectAbilityCards((Character)state.Performer, CardState.Lost, 0, 3, 
							canSelectFunc: abilityCard => abilityCard.Top.Abilities.Concat(abilityCard.Bottom.Abilities).Any(cardAbility => cardAbility.Ability is SummonAbility),
							hintText: $"Select up to 3 lost cards with summon abilities to recover");

					foreach(AbilityCard abilityCard in selectedAbilityCards)
					{
						await AbilityCmd.ReturnToHand(abilityCard);
					}

					IEnumerable<AbilityCardSide> abilitySides = selectedAbilityCards.SelectMany<AbilityCard, AbilityCardSide>(abilityCard => [abilityCard.Top, abilityCard.Bottom]);
					IEnumerable<AbilityCardSide> abilitySidesToPerform = abilitySides.Where(abilityCardSide => abilityCardSide.Abilities.Any(cardAbility => cardAbility.Ability is SummonAbility));

					foreach(AbilityCardSide abilitySide in abilitySidesToPerform)
					{
						await abilitySide.Perform(state.Performer);

						state.SetPerformed();
					}
				})
				.Build())
		];

		protected override bool Loss => true;
		protected override bool Unrecoverable => true;
	}
}