using System.Collections.Generic;
using Fractural.Tasks;

public class ImperviousArmor : ChieftainCardModel<ImperviousArmor.CardTop, ImperviousArmor.CardBottom>
{
	public override string Name => "Impervious Armor";
	public override int Level => 7;
	public override int Initiative => 86;
	protected override int AtlasIndex => 23;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 7,
					Move = 2,
					Attack = 2,
					Traits = 
					[
						new ShieldTrait(1),
						new PierceTrait(3),
						new MountTrait(
							async (owner, mount) => 
							{
								ScenarioCheckEvents.ShieldCheckEvent.Subscribe(owner, this,
									parameters => parameters.Figure == owner,
									parameters =>
									{
										parameters.AdjustShield(1);
									}
								);

								ScenarioEvents.SufferDamageEvent.Subscribe(owner, this,
									parameters => parameters.Figure == owner && parameters.FromAttack,
									async parameters =>
									{
										parameters.AdjustShield(1);

										await GDTask.CompletedTask;
									}
								);

								await GDTask.CompletedTask;
							},
							async (owner, mount) => 
							{
								ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(owner, this);
								ScenarioEvents.SufferDamageEvent.Unsubscribe(owner, this);

								await GDTask.CompletedTask;
							}
						),
					]
				})
				.WithName("Battle Rhinoceros")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/battle_rhinoceros_AI.png")
				.Build()
			),
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
		protected override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.Build()
			),

			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state => [ShieldAbility.Builder().WithShieldValue(2).Build()])
				.WithCustomGetTargets((state, figures) =>
				{
					ScenarioCheckEvents.IsMountedCheck.Parameters isMountedCheckParameters =
						ScenarioCheckEvents.IsMountedCheckEvent.Fire(
							new ScenarioCheckEvents.IsMountedCheck.Parameters(state.Performer));

					if(isMountedCheckParameters.IsMounted)
					{
						figures.Add(isMountedCheckParameters.Mount);
					}
				})
				.WithTarget(Target.Allies)
				.Build()
			)
		];

		protected override bool Round => true;
	}
}