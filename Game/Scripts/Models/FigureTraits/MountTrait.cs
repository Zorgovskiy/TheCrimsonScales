using Fractural.Tasks;
using System;

public class MountTrait(Figure characterOwner, Func<Figure, GDTask> onMounted = null, Func<Figure, GDTask> onDismounted = null) : FigureTrait
{
	private bool _mounted = false;

	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		// Allow entering the same hex to mount
		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Subscribe(figure, this,
			parameters => parameters.OtherFigure == figure && parameters.Figure == characterOwner,
			parameters =>
			{
				parameters.SetCanEnter();
			}
		);

		// Control the mount
		ScenarioCheckEvents.IsSummonControlledCheckEvent.Subscribe(figure, this,
			parameters => parameters.Summon == figure && _mounted,
			parameters =>
			{
				parameters.SetIsControlled();
			}
		);

		// Follow the mount when it moves or being forcefully moved
		ScenarioEvents.MoveTogetherCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && _mounted,
			parameters =>
			{
				parameters.SetOtherFigure(characterOwner);

				return GDTask.CompletedTask;
			}
		);

		// Trigger mounted effect
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => parameters.Figure == characterOwner,
			async parameters =>
			{
				if(!_mounted && onMounted != null && parameters.Hex == figure.Hex)
				{
					await onMounted(figure);
					_mounted = true;
				}
				else if (_mounted && onDismounted != null && parameters.Hex != figure.Hex)
				{
					await onDismounted(figure);
					_mounted = false;
				}
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.IsSummonControlledCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.MoveTogetherCheckEvent.Unsubscribe(figure, this);
	}
}