using System.Collections.Generic;

public class Scenario004 : ScenarioModel
{
	public override string ScenePath => "res://Content/Scenarios/Scenario004.tscn";
	public override int ScenarioNumber => 4;
	public override ScenarioChain ScenarioChain => ModelDB.ScenarioChain<InfectiousScenarioChain>();
	//public override IEnumerable<ScenarioConnection> Connections => [new ScenarioConnection<Scenario005>()];

	protected override ScenarioGoals CreateScenarioGoals() => new KillAlLEnemiesScenarioGoals();
}

