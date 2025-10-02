using System.Collections.Generic;
using Godot;

public class ChieftainModel : ClassModel
{
	public override string Name => "Chieftain";
	public override MaxHealthValues MaxHealthValues => MaxHealthValues.Medium;
	public override int HandSize => 10;
	public override string AssetPath => "res://Content/Classes/Chieftain";
	public override Color PrimaryColor => Color.FromHtml("76c7c3");
	public override Color SecondaryColor => Color.FromHtml("5e7574");

	public override PackedScene Scene => ResourceLoader.Load<PackedScene>($"{AssetPath}/Chieftain.tscn");

	public override IList<AbilityCardModel> AbilityCards { get; } =
	[
		//ModelDB.Card<Chokehold>(),
		//ModelDB.Card<DragThroughDirt>(),
		//ModelDB.Card<FollowTheChains>(),
		//ModelDB.Card<LockingLinks>(),
		//ModelDB.Card<MercilessBeatdown>(),
		//ModelDB.Card<RustySpikes>(),
		//ModelDB.Card<SlammingShove>(),
		//ModelDB.Card<SpikeKnuckles>(),
		//ModelDB.Card<UntouchableKeeper>(),
		//ModelDB.Card<WrappedInMetal>(),

		//ModelDB.Card<GangingUp>(),
		//ModelDB.Card<RoundhouseSwing>(),
		//ModelDB.Card<VigorousSway>(),

		//ModelDB.Card<AgonizingClamp>(),
		//ModelDB.Card<IronThrust>(),
		//ModelDB.Card<LatchAndTow>(),
		//ModelDB.Card<SweepingCollision>(),
		//ModelDB.Card<DizzyingRelease>(),
		//ModelDB.Card<DoubleKO>(),
	];
}