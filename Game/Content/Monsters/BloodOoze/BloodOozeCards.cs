using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public abstract class BloodOozeAbilityCard : MonsterAbilityCardModel
{
	public override string CardsAtlasPath => "res://Content/Monsters/BloodOoze/Cards.jpg";

	public static IEnumerable<MonsterAbilityCardModel> Deck { get; } =
	[
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard0>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard1>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard2>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard3>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard4>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard5>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard6>(),
		ModelDB.MonsterAbilityCard<BloodOozeAbilityCard7>()
	];
}

public class BloodOozeAbilityCard0 : BloodOozeAbilityCard
{
	public override int Initiative => 36;
	public override int CardIndex => 0;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, -1)),
	];
}

public class BloodOozeAbilityCard1 : BloodOozeAbilityCard
{
	public override int Initiative => 57;
	public override int CardIndex => 1;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, +0)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +0)),
	];
}

public class BloodOozeAbilityCard2 : BloodOozeAbilityCard
{
	public override int Initiative => 59;
	public override int CardIndex => 2;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(AttackAbility(monster, +0, targets: 2, conditions: [Conditions.Poison1])),
	];
}

public class BloodOozeAbilityCard3 : BloodOozeAbilityCard
{
	public override int Initiative => 66;
	public override int CardIndex => 3;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: 1)),
	];
}

public class BloodOozeAbilityCard4 : BloodOozeAbilityCard
{
	public override int Initiative => 94;
	public override int CardIndex => 4;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(null, state.Performer, 2);
			})
			.Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<BloodOoze>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				int level = state.Performer is Monster performingMonster
					? performingMonster.MonsterLevel
					: GameController.Instance.SavedScenario.ScenarioLevel;
				state.SetForcedHitPoints(Mathf.Min(state.MonsterModel.NormalLevelStats[level].Health, state.Performer.Health));

				await GDTask.CompletedTask;
			})
			.Build())
	];
}

public class BloodOozeAbilityCard5 : BloodOozeAbilityCard
{
	public override int Initiative => 94;
	public override int CardIndex => 5;
	public override bool Reshuffles => true;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await AbilityCmd.SufferDamage(null, state.Performer, 2);
			})
			.Build()),

		new MonsterAbilityCardAbility(MonsterSummonAbility.Builder()
			.WithMonsterModel(ModelDB.Monster<BloodOoze>())
			.WithMonsterType(MonsterType.Normal)
			.WithOnAbilityStarted(async state =>
			{
				int level = state.Performer is Monster performingMonster
					? performingMonster.MonsterLevel
					: GameController.Instance.SavedScenario.ScenarioLevel;
				state.SetForcedHitPoints(Mathf.Min(state.MonsterModel.NormalLevelStats[level].Health, state.Performer.Health));

				await GDTask.CompletedTask;
			})
			.Build())
	];
}

public class BloodOozeAbilityCard6 : BloodOozeAbilityCard
{
	public override int Initiative => 66;
	public override int CardIndex => 6;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(MoveAbility(monster, -1)),
		new MonsterAbilityCardAbility(LootAbility.Builder().WithRange(1).Build()),
		new MonsterAbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build())
	];
}

public class BloodOozeAbilityCard7 : BloodOozeAbilityCard
{
	public override int Initiative => 85;
	public override int CardIndex => 7;

	public override IEnumerable<MonsterAbilityCardAbility> GetAbilities(Monster monster) =>
	[
		new MonsterAbilityCardAbility(PushAbility.Builder()
			.WithPush(1)
			.WithConditions([Conditions.Poison1])
			.WithTarget(Target.Enemies | Target.TargetAll)
			.Build()),

		new MonsterAbilityCardAbility(AttackAbility(monster, +1, extraRange: -1))
	];
}