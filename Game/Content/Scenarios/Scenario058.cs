using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario058 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario058.tscn";
	public override int ScenarioNumber => 58;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SoloScenario>();
	// TODO: Requirement - level 5 Chainguard

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Lock all enemies back in their cells. If any enemy dies, the scenario is immediately lost.");

	private int _lockedCells = 0;
	private bool _spawnTriggered = false;

	private AbilityCardSide CreateWardenBasicTopSide(AbilityCard abilityCard)
	{
		return new WardenBasicTopSide
		{
			AbilityCard = abilityCard
		};
	}

	private AbilityCardSide CreateWardenBasicBottomSide(AbilityCard abilityCard)
	{
		return new WardenBasicBottomSide
		{
			AbilityCard = abilityCard
		};
	}

	private class WardenBasicTopSide : AbilityCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder().WithPush(1).Build())
		];
	}

	private class WardenBasicBottomSide : AbilityCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).WithMoveType(MoveType.Jump).Build())
		];
	}

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		UpdateScenarioText("You may forgo a top action while standing adjacent to the door" +
							"hex to permanently close the door and lock a cell." +
							"You cannot lock a cell unless there are exactly two " +
							"enemies inside the room being locked. You cannot lock a cell " +
							"if an enemy is occupying the door hex of the cell you wish to lock." +
							System.Environment.NewLine + System.Environment.NewLine +
							$"Instead of {Icons.Attack} 2, all basic top actions are {Icons.Push} 1, {Icons.Range} 1." +
							$"Instead of {Icons.Move} 2, all basic bottom actions are {Icons.Move} 2, {Icons.Jump} 2");

		foreach(Treasure treasure in GameController.Instance.Map.Treasures)
		{
			treasure.SetObtainLootFunction(async character =>
			{
				ActionState actionState = new(character, [HealAbility.Builder().WithHealValue(4).WithTarget(Target.Self).Build()]);
				await actionState.Perform();
			});
		}

		//// Change basic top and basic bottom
		//ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(GameController.Instance.CharacterManager.FirstAlive(), this,
		//	parameters => !parameters.ForgoneAction &&
		//		(parameters.AbilityCardSide.IsBasicTop || parameters.AbilityCardSide.IsBasicBottom),
		//	async parameters =>
		//	{
		//		parameters.ForgoAction();
//
		//		ActionState actionState = new ActionState(parameters.Performer, parameters.AbilityCardSide.IsBasicTop ? 
		//			[PushAbility.Builder().WithPush(1).Build()] : 
		//			[MoveAbility.Builder().WithDistance(2).Build(),
		//			 MoveAbility.Builder().WithDistance(2).WithMoveType(MoveType.Jump).Build()]);
		//		await actionState.Perform();
		//	},
		//	EffectType.MandatoryAfterOptionals
		//);
//
		//// Change basic top and basic bottom
		//ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this, GameController.Instance.CharacterManager.FirstAlive(),
		//	parameters => !parameters.ForgoneAction &&
		//		(parameters.AbilityCardSide.IsBasicTop || parameters.AbilityCardSide.IsBasicBottom),
		//	async parameters =>
		//	{
		//		parameters.ForgoAction();
//
		//		ActionState actionState = new ActionState(parameters.Performer, parameters.AbilityCardSide.IsBasicTop ? 
		//			[PushAbility.Builder().WithPush(1).Build()] : 
		//			[MoveAbility.Builder().WithDistance(2).Build(),
		//			 MoveAbility.Builder().WithDistance(2).WithMoveType(MoveType.Jump).Build()]);
		//		await actionState.Perform();
		//	},
		//	EffectType.MandatoryAfterOptionals
		//);

		foreach(AbilityCard abilityCard in GameController.Instance.CharacterManager.FirstAlive().Cards)
		{
			abilityCard.BasicTop = CreateWardenBasicTopSide(abilityCard);
			abilityCard.BasicBottom = CreateWardenBasicBottomSide(abilityCard);
		}

		// Win when all cells are locked
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(_lockedCells == 2 && !_spawnTriggered)
				{
					await AbilityCmd.SpawnMonster(ModelDB.Monster<BanditArcher>(), MonsterType.Elite, GameController.Instance.Map.Markers
						.First(marker => marker.MarkerType == Marker.Type.a).Hex);
					await AbilityCmd.SpawnMonster(ModelDB.Monster<BanditGuard>(), MonsterType.Elite, GameController.Instance.Map.Markers
						.First(marker => marker.MarkerType == Marker.Type.b).Hex);

					_spawnTriggered = true;
				}
				else if(_lockedCells == 4)
				{
					await ((CustomScenarioGoals)ScenarioGoals).Win();
					// TODO: Get CLAW TRAP 34
					// TODO: GET CLAMP TRAP 56
				}
			}
		);

		// Lose when an enemy is killed
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure.Alignment == Alignment.Enemies,
			async parameters =>
			{
				ScenarioEvents.FigureKilledEvent.Unsubscribe(this);

				await AbilityCmd.Lose();
			}
		);
	}

	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters roomParameters)
	{
		Hex doorHex = roomParameters.OpenedDoor.Hex;
		List<Hex> roomHexes = roomParameters.Room.Hexes;

		// Forgo top action to lock a door
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters => 
			{
				// Didn't forgo action yet
				bool bool0 = parameters.ForgoneAction;
				// Only forgo top action to close the door
				bool bool1 = parameters.AbilityCardSide.IsBasicTop || parameters.AbilityCardSide.IsTop;
				// Adjacent to the door hex
				bool bool2 = RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1, false).Contains(doorHex);
				// Standing outside the room
				bool bool3 = !roomHexes.Contains(parameters.Performer.Hex);
				// Nobody is in the doorway
				bool bool4 = doorHex.IsUnoccupied();
				// Exactly 2 enemies in the room
				bool bool5 = roomHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
										.Where(figure => figure.EnemiesWith(parameters.Performer)).Count() == 2;

				return bool0 && bool1 && bool2 && bool3 && bool4 && bool5;
			},
			async parameters =>
			{
				parameters.ForgoAction();
				// Create a new door/obstacle/wall!?!?

				_lockedCells++;
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Lock),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"Lock the cell door.")
		);
	}
}