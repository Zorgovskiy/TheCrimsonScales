using System.Collections.Generic;
using Fractural.Tasks;

public class SlowAndSteady : ChieftainCardModel<SlowAndSteady.CardTop, SlowAndSteady.CardBottom>
{
	public override string Name => "Slow and steady";
	public override int Level => 1;
	public override int Initiative => 93;
	protected override int AtlasIndex => 6;

	public class CardTop : ChieftainCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithSummonStats(new SummonStats()
				{
					Health = 6,
					Move = 1,
					Attack = 1,
					Traits = 
					[
						new ShieldTrait(1),
						new MountTrait(AbilityCard.OriginalOwner,
							async mountSummon => 
							{
								ScenarioEvents.ForcedMovementCheckEvent.Subscribe(mountSummon, this,
									canApply: parameters => 
										parameters.AbilityState.Target == AbilityCard.OriginalOwner ||
										parameters.AbilityState.Target == mountSummon,
									async parameters => 
									{
										parameters.SetPrevented();

										await GDTask.CompletedTask;
									}
								);
								await GDTask.CompletedTask;
							},
							async mountSummon => 
							{ 
								ScenarioEvents.ForcedMovementCheckEvent.Unsubscribe(mountSummon, this);

								await GDTask.CompletedTask;
							}
						)
					]
				})
				.WithName("Giant Tortoise")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/giant_tortoise_AI.png")
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
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(2).Build()),
		];
	}
}