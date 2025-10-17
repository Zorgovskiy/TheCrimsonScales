using System.Collections.Generic;
using Godot;

public abstract class FlamingDrakeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/FlamingDrake/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<FlamingDrakeAbilityCard7>()
	];
}

public class FlamingDrakeAbilityCard0 : FlamingDrakeAbilityCard
{
	public override int Initiative => 32;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class FlamingDrakeAbilityCard1 : FlamingDrakeAbilityCard
{
	public override int Initiative => 52;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class FlamingDrakeAbilityCard2 : FlamingDrakeAbilityCard
{
	public override int Initiative => 57;
	public override int CardIndex => 2;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -1,
			aoePattern: new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			])
		)),
	];
}

public class FlamingDrakeAbilityCard3 : FlamingDrakeAbilityCard
{
	public override int Initiative => 27;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class FlamingDrakeAbilityCard4 : FlamingDrakeAbilityCard
{
	public override int Initiative => 87;
	public override int CardIndex => 4;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class FlamingDrakeAbilityCard5 : FlamingDrakeAbilityCard
{
	public override int Initiative => 89;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, conditions: [Conditions.Stun])),
	];
}

public class FlamingDrakeAbilityCard6 : FlamingDrakeAbilityCard
{
	public override int Initiative => 06;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(2).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithTarget(Target.Self)
			.Build())
	];
}

public class FlamingDrakeAbilityCard7 : FlamingDrakeAbilityCard
{
	public override int Initiative => 89;
	public override int CardIndex => 7;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, extraRange: -2,
			aoePattern: new AOEPattern([
				new AOEHex(Vector2I.Zero, AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
			]), conditions: [Conditions.Poison1]
		)),
	];
}