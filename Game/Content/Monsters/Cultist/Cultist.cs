using System.Collections.Generic;

public class Cultist : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 4,
			Move = 2,
			Attack = 1,
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 1,
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 1,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 1,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 3,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 3,
			Attack = 2,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 3,
			Attack = 2,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 7,
			Move = 2,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 2,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 12,
			Move = 2,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 3,
			Attack = 2,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 18,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 22,
			Move = 3,
			Attack = 3,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
		new MonsterStats()
		{
			Health = 25,
			Move = 3,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Curse)]
		},
	];

	public override string Name => "Cultist";

	public override string AssetPath => "res://Content/Monsters/Cultist";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => CultistAbilityCard.Deck;
}