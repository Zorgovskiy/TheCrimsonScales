using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Penitence : HierophantPrayerCardModel<Penitence.CardTop, Penitence.CardBottom>
{
	public override string Name => "Penitence";
	protected override int AtlasIndex => 7 - 6;

	public class CardTop : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.AbilityState.Performer == state.Performer && 
							canApplyParameters.Value < 0 && 
							!canApplyParameters.AMDCard.IsNull,
						async applyParameters =>
						{
							applyParameters.SetValue(0);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.398f, 0.306f)),
						new UseSlot(new Vector2(0.603f, 0.306f))
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}

	public class CardBottom : HierophantPrayerCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						canApplyParameters =>
							state.Performer.EnemiesWith(canApplyParameters.AbilityState.Performer) &&
							canApplyParameters.Value > 0 &&
							!canApplyParameters.AMDCard.IsCrit,
						async applyParameters =>
						{
							applyParameters.SetValue(0);

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.398f, 0.772f)),
						new UseSlot(new Vector2(0.603f, 0.772f))
					]
				)
				.Build())
		];

		protected override bool Persistent => true;
	}
}