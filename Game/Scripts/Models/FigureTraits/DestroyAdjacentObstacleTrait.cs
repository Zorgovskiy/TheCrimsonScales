using Fractural.Tasks;
using System.Collections.Generic;
using System.Linq;

public class DestroyAdjacentObstacleTrait() : FigureTrait
{
	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Subscribe(figure, this,
			parameters => parameters.AbilityState.Performer == figure,
			async parameters =>
			{
				List<Hex> adjacentHexList = new();
				RangeHelper.FindHexesInRange(parameters.AbilityState.Target.Hex, 1, true, adjacentHexList);

				// Select hexes that have a 1-hex obstacle
				List<Hex> selectedHexes =
					await AbilityCmd.SelectHexes(parameters.AbilityState, 
						list => list.AddRange(adjacentHexList
							.Where(hex => hex.GetHexObjectsOfType<Obstacle>()
								.Any(obstacle => obstacle.Hexes.Length == 1))),
						0, 1, true,	"Select a 1-hex obstacle to destroy");

				foreach(Hex selectedHex in selectedHexes)
				{
					await AbilityCmd.DestroyObstacle(selectedHex.GetHexObjectsOfType<Obstacle>()
						.FirstOrDefault(obstacle => obstacle.Hexes.Length == 1));
				}

				await GDTask.CompletedTask;
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioEvents.AfterAttackPerformedEvent.Unsubscribe(figure, this);
	}
}