using System.Collections;
using System.Linq;
using Fractural.Tasks;

public partial class Chieftain : Character
{
	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new();

		ScenarioCheckEvents.IsMountedCheckEvent.Subscribe(this, subscriber,
			parameters => parameters.Figure == this,
			async parameters =>
			{
				if(Hex.GetHexObjectsOfType<Summon>().Any(summon => summon.Stats.Traits.Any(trait => trait is MountTrait)))
				{
					parameters.SetIsMounted();
				}

				await GDTask.CompletedTask;
			}
		);
	}
}