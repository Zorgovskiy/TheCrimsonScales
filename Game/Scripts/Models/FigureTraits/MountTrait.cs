using System.Linq;
using Fractural.Tasks;

public class MountTrait(Figure characterOwner) : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

        ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Subscribe(figure, this,
			parameters => parameters.OtherFigure == figure && parameters.Figure == characterOwner,
			parameters =>
			{
				parameters.SetCanEnter();
			}
		);

        ScenarioCheckEvents.IsSummonControlledCheckEvent.Subscribe(figure, this,
			parameters => parameters.Summon == figure && IsMounted(figure, characterOwner),
			parameters =>
			{
				parameters.SetIsControlled();
			}
		);

        ScenarioEvents.MoveTogetherCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && IsMounted(figure, characterOwner),
			parameters =>
			{
				parameters.SetOtherFigure(characterOwner);

				return GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Unsubscribe(figure, this);
	}

    private bool IsMounted(Figure figure, Figure owner)
    {
        return figure.Hex.GetHexObjectsOfType<Figure>().Contains(owner);
    }
}