using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Scenario058 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario058.tscn";

	public override int ScenarioNumber => 58;
	public override string Name => "Penitentiary Lockdown";

	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<SoloScenario>();
	// TODO: Requirement - level 5 Chainguard

	public override string IntroductionText =>
		"""
		You knew this would happen. Governor Beetleworth was crooked and, worse in Gloomhaven, he was weak. It was an open secret for years that he was on the take; everything could be bought—a cell further away from the sewers, a meal with identifiable meat—but he obviously went too far this time.

		The Assistant, a capable but limited man, found you in the Sleeping Lion, desperate to tell you the story. The short version seems to be that Beetleworth had allowed one of the inmates to take his own exercise—but once he had the key, he slit Beetleworth’s throat and made for the hills, kindly letting the rest of the inmates out before escaping.

		The Assistant (you don’t remember his actual name) senses an opportunity, and is keen to recover the situation, but is incapable of herding the various miscreants back into their cells. So, just like they always do, he winds up at your door—or tavern table in this case. “I just need them back—now!” he pleads. “And whatever you do, don’t kill any of them. I just need things back the way they were.”

		You raise an eyebrow, take a drink, and wait. “Can you do it? And how much?”

		You wait. He knows your price. He just hasn’t realized it yet. “I haven’t got much, but think of the good you’ll be doing to Gloomhaven” the Assistant urges. You smile at that, take another drink, and softly tell him that he knows what you want.

		The Assistant looks confused for a second, then his eyes widen. “That was not approved for use, and should never have been shown to prison employees!” You wait some more. You have time. He doesn’t.

		The Assistant squirms in his seat, wrestling with his conscience, before angrily whispering “Fine! But no-one finds out, no-one gets killed and it gets done now!” You nod in acceptance and drain your drink before getting up. A job’s a job, and it’ll be a good opportunity to catch up with some old acquaintances too.
		""";

	public override string ConclusionText =>
		"""
		Most of the inmates recognized you, and although some tried their luck with you, most of them were too sensible. Order restored, you walk out of the gate, toying with the package the Assistant—presumably soon to be the new Governor—gave you. Job well done.
		""";

	public override List<MonsterModel> MonsterModels { get; } =
	[
		ModelDB.Monster<BanditArcher>(),
		ModelDB.Monster<BanditGuard>(),
	];

	public override List<SavedReward> Rewards =>
	[
		// Gain one of the following as an item and the other as an item design:
		new GainItemDesignReward(ModelDB.Item<ClawTrap>()),
		new GainItemDesignReward(ModelDB.Item<ClampTrap>()),
	];

	private CustomScenarioGoal _jailGoal;
	private bool _spawnTriggered = false;

	public class WardenBasicAbilityCardTop : AbilityCardSideModel
	{
		public override AbilityCardSideType AbilityCardSideType => AbilityCardSideType.BasicTop;

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder().WithPush(1).Build())
		];
	}

	public class WardenBasicAbilityCardBottom : AbilityCardSideModel
	{
		public override AbilityCardSideType AbilityCardSideType => AbilityCardSideType.BasicBottom;

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).WithMoveType(MoveType.Jump).Build())
		];
	}
	
	public virtual AbilityCardSideModel WardenBasicTop => ModelDB.AbilityCardSide<WardenBasicAbilityCardTop>();
	public virtual AbilityCardSideModel WardenBasicBottom => ModelDB.AbilityCardSide<WardenBasicAbilityCardBottom>();

	public override async GDTask InitializeAfterFirstRoomRevealed()
	{
		await base.InitializeAfterFirstRoomRevealed();

		_jailGoal = await AddGoal(new CustomScenarioGoal(textParameters => "Lock all enemies back in their cells.", hasProgress: true, maxProgress: 8));

		AddScenarioRule(textParameters =>
			"You must lock the bandits back in their cells, but if any enemy dies, the scenario is immediately lost.");
		AddScenarioRule(textParameters =>
			$"Perform “{Icons.Inline(Icons.Heal)}4, Self” if you loot a treasure.");
		AddScenarioRule(textParameters =>
			"You may forgo a top action while standing adjacent to the door hex to permanently close the door and lock a cell.");
		AddScenarioRule(textParameters =>
			"You cannot lock a cell unless there are exactly two enemies inside the room being locked.");
		AddScenarioRule(textParameters =>
			"You cannot lock a cell if an enemy is occupying the door hex of the cell you wish to lock.");
		AddScenarioRule(textParameters =>
			$"All basic top actions are {Icons.Inline(Icons.Push)}1, {Icons.Inline(Icons.Range)}1, all basic bottom actions are {Icons.Inline(Icons.Move)}2, {Icons.Inline(Icons.Jump)}");

		foreach(Treasure treasure in GameController.Instance.Map.Treasures)
		{
			treasure.SetObtainLootFunction(async character =>
			{
				ActionState actionState = new(character, [HealAbility.Builder().WithHealValue(4).WithTarget(Target.Self).Build()]);
				await actionState.Perform();
			});
		}

		foreach(AbilityCard abilityCard in GameController.Instance.CharacterManager.FirstAlive().Cards)
		{
			abilityCard.BasicTop = new AbilityCardSide(abilityCard, WardenBasicTop);
			abilityCard.BasicBottom = new AbilityCardSide(abilityCard, WardenBasicBottom);
		}

		// Win when all cells are locked
		ScenarioEvents.RoundEndedEvent.Subscribe(this,
			parameters => true,
			async parameters =>
			{
				if(_jailGoal.Progress == 2 && !_spawnTriggered)
				{
					await AbilityCmd.SpawnMonster(ModelDB.Monster<BanditArcher>(), MonsterType.Elite, GameController.Instance.Map.Markers
						.First(marker => marker.MarkerType == Marker.Type.a).Hex);
					await AbilityCmd.SpawnMonster(ModelDB.Monster<BanditGuard>(), MonsterType.Elite, GameController.Instance.Map.Markers
						.First(marker => marker.MarkerType == Marker.Type.b).Hex);

					_spawnTriggered = true;
				}
			}
		);

		// Lose when an enemy is killed
		ScenarioEvents.FigureKilledEvent.Subscribe(this,
			parameters => parameters.Figure.Alignment == Alignment.Monsters,
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
				bool bool0 = !parameters.ForgoneAction;
				// Only forgo top action to close the door
				bool bool1 = parameters.AbilityCardSide.AbilityCardSideType is AbilityCardSideType.Top or AbilityCardSideType.BasicTop;
				// Adjacent to the door hex
				bool bool2 = RangeHelper.GetHexesInRange(parameters.Performer.Hex, 1, false).Contains(doorHex);
				// Standing outside the room
				bool bool3 = !roomHexes.Contains(parameters.Performer.Hex);
				// Nobody is in the doorway
				bool bool4 = doorHex.IsUnoccupied();
				// Exactly 2 enemies in the room
				bool bool5 = roomHexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
										.Count(figure => figure.EnemiesWith(parameters.Performer)) == 2;

				return bool0 && bool1 && bool2 && bool3 && bool4 && bool5;
			},
			async parameters =>
			{
				parameters.ForgoAction();

				//Doors = this.GetChildrenOfType<Door>();
				//foreach(Door door in Doors)
				//{
				//	door.Hide();
		//
				//	Hex hex = GetHex(GlobalPositionToCoords(door.GlobalPosition), false);
		//
				//	foreach(Room room in Rooms)
				//	{
				//	}
				//}
				// Create a new door/obstacle/wall!?!?

				Door door = await AbilityCmd.CreateOverlayTile<Door>(doorHex,
						ResourceLoader.Load<PackedScene>("res://Content/OverlayTiles/Doors/StoneDoorVertical1H.tscn"));

				await door.Lock();

				await _jailGoal.AdjustProgress(1);
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Lock),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"Lock the cell door.")
		);
	}
}