using System.Collections.Generic;

public class LivingBones : MonsterModel
{
	public override MonsterStats[] NormalLevelStats =>
	[
		new MonsterStats()
		{
			Health = 5,
			Move = 2,
			Attack = 1,
			Traits = [new TargetsTrait(2)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 1,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 5,
			Move = 3,
			Attack = 2,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 3,
			Attack = 2,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 3,
			Attack = 3,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 9,
			Move = 3,
			Attack = 3,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 4,
			Attack = 3,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 13,
			Move = 4,
			Attack = 3,
			Traits = [new TargetsTrait(2), new ShieldTrait(1)]
		},
	];

	public override MonsterStats[] EliteLevelStats =>
	[
		new MonsterStats()
		{
			Health = 6,
			Move = 4,
			Attack = 2,
			Traits = [new TargetsTrait(2)]
		},
		new MonsterStats()
		{
			Health = 6,
			Move = 4,
			Attack = 2,
			Traits = [new TargetsTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 7,
			Move = 4,
			Attack = 3,
			Traits = [new TargetsTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 10,
			Move = 4,
			Attack = 3,
			Traits = [new TargetsTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 4,
			Traits = [new TargetsTrait(3), new ShieldTrait(1)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 4,
			Attack = 4,
			Traits = [new TargetsTrait(3), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 11,
			Move = 6,
			Attack = 4,
			Traits = [new TargetsTrait(3), new ShieldTrait(2)]
		},
		new MonsterStats()
		{
			Health = 14,
			Move = 6,
			Attack = 4,
			Traits = [new TargetsTrait(3), new ShieldTrait(2)]
		},
	];

	public override string Name => "Living Bones";

	public override string AssetPath => "res://Content/Monsters/LivingBones";

	public override int MaxStandeeCount => 10;

	public override IEnumerable<MonsterAbilityCardModel> Deck => LivingBonesAbilityCard.Deck;
}