using Godot;

public partial class ObjectiveInfoItem : FigureInfoItem<ObjectiveInfoItem.Parameters>
{
	public class Parameters(Objective hexObject) : FigureInfoItemParameters(hexObject)
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/InfoView/ObjectiveInfoItem.tscn";

		public Objective Objective { get; } = hexObject;
	}

	[Export]
	private Control _sceneAnchor;

	private Objective _objective;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_objective = parameters.Objective;

		//_titleLabel.SetText("Objective");

		PackedScene overlayTileScene = ResourceLoader.Load<PackedScene>(parameters.HexObject.SceneFilePath);
		Objective instance = overlayTileScene.Instantiate<Objective>();
		_sceneAnchor.AddChild(instance);
		instance.SetScale(0.6f * Vector2.One);
		FigureViewComponent figureViewComponent = instance.GetChildOfType<FigureViewComponent>();
		figureViewComponent.SetVisible(false);
	}
}