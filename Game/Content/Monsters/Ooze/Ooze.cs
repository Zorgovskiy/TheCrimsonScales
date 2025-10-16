using System.Collections.Generic;

public class Ooze : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 1,
			Attack = 2,
			Range = 2,
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 1,
			Attack = 2,
			Range = 2,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 8,
			Move = 1,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 2,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 2,
			Attack = 4,
			Range = 3,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 2,
			Attack = 4,
			Range = 3,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 8,
			Move = 1,
			Attack = 2,
			Range = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 1,
			Attack = 2,
			Range = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 1,
			Attack = 3,
			Range = 3,
			Traits = [new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 2,
			Attack = 3,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(1), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 16,
			Move = 3,
			Attack = 4,
			Range = 4,
			Traits = [new ShieldTrait(2), new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 3,
			Attack = 5,
			Range = 4,
			Traits = [new ShieldTrait(2), new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override string Name => "Ooze";

	public override string AssetPath => "res://Content/Monsters/Ooze";

	public override int MaxStandeeCount => 10;

	public override IEnumerable<MonsterAbilityCardModel> Deck => OozeAbilityCard.Deck;
}