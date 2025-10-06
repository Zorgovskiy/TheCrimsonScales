using System.Collections;
using System.Collections.Generic;
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
				Summon summon = Hex.GetHexObjectsOfType<Summon>()
					.FirstOrDefault(summon => summon.Stats.Traits.Any(trait => trait is MountTrait), null);
				if(summon != null)
				{
					parameters.SetIsMounted();
					parameters.SetMount(summon);
				}

				await GDTask.CompletedTask;
			}
		);
	}
}