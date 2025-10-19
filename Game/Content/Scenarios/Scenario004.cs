using System.Linq;
using Fractural.Tasks;
using System.Collections.Generic;
using Godot;

public class Scenario004 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario004.tscn";
	public override int ScenarioNumber => 4;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario005>()];

	protected override ScenarioGoals CreateScenarioGoals() =>
		new CustomScenarioGoals("Kill all enemies and cure four sick warriors to win this scenario.");

	public override async GDTask StartAfterFirstRoomRevealed()
	{
		await base.StartAfterFirstRoomRevealed();

		// Give antidote if starting for the first time and didn't complete scenario 7
		if(!GameController.Instance.SavedScenarioProgress.Unlocked &&
			!GameController.Instance.SavedCampaign.CollectedPartyAchievements.Contains(PartyAchievement.FollowTheMoney))
		{
			TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
				new TargetSelectionPrompt(figures => figures.AddRange(GameController.Instance.Map.Figures.Where(figure => figure is Character)),
					true, true, null,
					() => $"Select a character to receive Pox Antidote." + System.Environment.NewLine + System.Environment.NewLine + 
							"During this scenario, this item is equipped" + System.Environment.NewLine +
							$"without it occupying an {Icons.Inline(Icons.GetItem(ItemType.Small))} item slot."),
				null);

			ItemModel itemModel = ModelDB.Item<PoxAntidote>();
			Character character = GameController.Instance.ReferenceManager.Get<Character>(targetAnswer.FigureReferenceId);

			await AbilityCmd.GiveItem(character, itemModel, true);
		}

		UpdateScenarioText(
			$"Any character may forgo the top action of their turn to perform a ”Heal{Icons.Inline(Icons.Heal)}1, Range{Icons.Inline(Icons.Range)}2” ability.");

		// Allow using Heal 1 instead of any top action
		ScenarioEvents.AbilityCardSideStartedEvent.Subscribe(this,
			parameters =>
				(parameters.AbilityCardSide.IsTop || parameters.AbilityCardSide.IsBasicTop) &&
				!parameters.ForgoneAction,
			async parameters =>
			{
				parameters.ForgoAction();

				ActionState actionState = new ActionState(parameters.Performer, [HealAbility.Builder().WithHealValue(1).WithRange(2).Build()]);
				await actionState.Perform();
			},
			EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.Heal),
			effectInfoViewParameters: new TextEffectInfoView.Parameters($"Heal{Icons.Inline(Icons.Heal)}1,Range {Icons.Inline(Icons.Range)}2")
		);
	}
	private List<InfectedWarrior> lista = [];
	protected override async GDTask OnRoomRevealed(ScenarioEvents.RoomRevealed.Parameters parameters)
    {
        UpdateScenarioText(
			$"Infected warriors, represented by {Icons.Marker(Marker.Type.a)} and {Icons.Marker(Marker.Type.b)} " +
			$"suffer from INFECT{Icons.Inline(Icons.GetCondition(Conditions.Infect))}. They are considered allies to you. " +
			"If you perform a heal ability targeting the infected warrior, you have successfully cured the warrior." +
			$"{Icons.Marker(Marker.Type.a)} are City Archers and {Icons.Marker(Marker.Type.b)} are City Guards.");

		foreach(Marker marker in GameController.Instance.Map.Markers)
		{
            InfectedWarrior warrior = new();
			MonsterModel monsterModel = marker.MarkerType == Marker.Type.a ? ModelDB.Monster<CityArcher>() : ModelDB.Monster<CityGuard>();
			warrior.Init(monsterModel);
			warrior._Ready();
			await warrior.Init(marker.Hex);
			lista.Add(warrior);
        }

		ScenarioEvents.AfterHealPerformedEvent.Subscribe(this,
			parameters => parameters.AbilityState.Target is InfectedWarrior,
			async parameters =>
			{
				InfectedWarrior infectedWarrior = parameters.AbilityState.Target as InfectedWarrior;
				Hex hex = infectedWarrior.Hex;
				MonsterModel monsterModel = infectedWarrior.MonsterModel;

				await infectedWarrior.Destroy();
				Monster monster = await AbilityCmd.SpawnAlly(monsterModel, MonsterType.Normal, hex);

				monster.SetHealth(4);
				monster.SetMaxHealth(4);
			}
		);

		await GDTask.CompletedTask;
    }
}

	public partial class InfectedWarrior : Figure
	{
		private string _name = "Infected Warrior";

		public override string DisplayName => _name;
		public override string DebugName => _name;

		public override AMDCardDeck AMDCardDeck => null;

		public MonsterModel MonsterModel;

		public void Init(MonsterModel monsterModel)
		{
			MonsterModel = monsterModel;
		}

		public override async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
		{
			await base.Init(originHex, rotationIndex, hexCanBeNull);

			SetAlignment(Alignment.Characters);
			SetEnemies(Alignment.Other);

			GameController.Instance.Map.RegisterFigure(this);

			UpdateInitiative();

			ScenarioEvents.InflictConditionEvent.Subscribe(this, this,
				parameters => parameters.Target == this,
				async parameters =>
				{
					parameters.SetPrevented(true);

					await GDTask.CompletedTask;
				}
			);

			ScenarioCheckEvents.CanBeTargetedCheckEvent.Subscribe(this, this,
				parameters =>
					parameters.PotentialTarget == this &&
					parameters.PotentialAbilityState != null &&
					parameters.PotentialAbilityState is not HealAbility.State,
				parameters =>
				{
					parameters.SetCannotBeTargeted();
				}
			);

			ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Subscribe(this, this,
				parameters => parameters.Figure == this,
				parameters =>
				{
					parameters.SetImmuneToForcedMovement();
				}
			);

			CanTakeTurn = false;
		}

		public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
		{
			await base.Destroy(immediately, forceDestroy);

			ScenarioEvents.InflictConditionEvent.Unsubscribe(this, this);
			ScenarioCheckEvents.CanBeTargetedCheckEvent.Unsubscribe(this, this);
			ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Unsubscribe(this, this);
		}

		public override void RoundEnd()
		{
			base.RoundEnd();

			CanTakeTurn = false;
		}

		protected override Initiative GetInitiative()
		{
			return new Initiative()
			{
				MainInitiative = 99,
				SortingInitiative = 99 * 10000000
			};
		}

		public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
		{
			base.AddInfoItemParameters(parametersList);

			parametersList.Add(new GenericInfoItem.Parameters(this, "Infected Warrior", "Read the Scenario goals and the special rules."));
		}
	}