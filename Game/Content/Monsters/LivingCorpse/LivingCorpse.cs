using System.Collections.Generic;

public class LivingCorpse : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 1,
			Attack = 2,
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 1,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 1,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 1,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 2,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 4
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 2,
			Attack = 4,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 2,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 10,
			Move = 1,
			Attack = 3,
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 1,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 1,
			Attack = 4,
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 2,
			Attack = 5,
		},
		new MonsterStats()
		{
			Health = 15,
			Move = 2,
			Attack = 5,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 17,
			Move = 2,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 21,
			Move = 2,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
		new MonsterStats()
		{
			Health = 25,
			Move = 2,
			Attack = 6,
			Traits = [new ApplyConditionTrait(Conditions.Poison1)]
		},
	];

	public override string Name => "Living Corpse";

	public override string AssetPath => "res://Content/Monsters/LivingCorpse";

	public override int MaxStandeeCount => 6;

	public override IEnumerable<MonsterAbilityCardModel> Deck => LivingCorpseAbilityCard.Deck;
}