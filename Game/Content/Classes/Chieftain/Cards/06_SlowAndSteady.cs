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
						MountTrait.Builder()
							.WithCharacterOwner(AbilityCard.OriginalOwner)
							.WithOnMounted(async mountSummon => 
							{
								ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(mountSummon, this,
									canApply: parameters => 
										parameters.AbilityState.Target == AbilityCard.OriginalOwner ||
										parameters.AbilityState.Target == mountSummon,
									async parameters => 
									{
										// Make immunity
										parameters.AbilityState.SingleTargetAdjustPull(-parameters.AbilityState.SingleTargetPull);
										parameters.AbilityState.SingleTargetAdjustPush(-parameters.AbilityState.SingleTargetPush);
										parameters.AbilityState.SingleTargetAdjustSwing(-parameters.AbilityState.SingleTargetSwing);

										await GDTask.CompletedTask;
									}
								);
								await GDTask.CompletedTask;
							})
							.WithOnUnmounted(async mountSummon => 
							{ 
								ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(mountSummon, this);
								await GDTask.CompletedTask;
							})
							.Build(),
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