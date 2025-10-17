using System.Collections.Generic;

public abstract class ToxicImpAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/ToxicImp/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard0>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard1>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard2>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard3>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard4>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard5>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard6>(),
		ModelDB.MonsterAbilityCard<ToxicImpAbilityCard7>()
	];
}

public class ToxicImpAbilityCard0 : ToxicImpAbilityCard
{
	public override int Initiative => 05;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ShieldAbility.Builder().WithShieldValue(5).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build())
	];
}

public class ToxicImpAbilityCard1 : ToxicImpAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class ToxicImpAbilityCard2 : ToxicImpAbilityCard
{
	public override int Initiative => 37;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class ToxicImpAbilityCard3 : ToxicImpAbilityCard
{
	public override int Initiative => 42;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(HealAbility.Builder()
			.WithHealValue(2)
			.WithRange(3)
			.Build()),
	];
}

public class ToxicImpAbilityCard4 : ToxicImpAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class ToxicImpAbilityCard5 : ToxicImpAbilityCard
{
	public override int Initiative => 76;
	public override int CardIndex => 5;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1)),
	];
}

public class ToxicImpAbilityCard6 : ToxicImpAbilityCard
{
	public override int Initiative => 43;
	public override int CardIndex => 6;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1, targets: 2, conditions: [Conditions.Curse])),
	];
}

public class ToxicImpAbilityCard7 : ToxicImpAbilityCard
{
	public override int Initiative => 24;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Strengthen)
			.WithRange(2)
			.WithTarget(Target.Allies | Target.TargetAll)
			.Build()),
		new MonsterAbilityCardAbility(ConditionAbility.Builder()
			.WithConditions(Conditions.Muddle)
			.WithRange(2)
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),
	];
}