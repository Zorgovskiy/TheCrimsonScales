using System.Collections;
using System.Linq;
using Fractural.Tasks;

public partial class Chieftain : Character
{
	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new();

		ScenarioEvents.IsMountedCheckEvent.Subscribe(this, subscriber,
			parameters => parameters.Performer == this,
			async parameters =>
			{
				if (Hex.GetHexObjectsOfType<Figure>().Any(figure => figure is Summon && Summons.Contains(figure)))
				{
					parameters.SetIsMounted();
				}

				await GDTask.CompletedTask;
			}
		);
	}
}