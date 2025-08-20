using Fractural.Tasks;

public class IronHelmet : Prosperity1Item
{
	public override string Name => "Iron Helmet";
	public override int ItemNumber => 7;
	public override int ShopCount => 2;
	public override int Cost => 10;
	public override ItemType ItemType => ItemType.Head;
	public override ItemUseType ItemUseType => ItemUseType.Always;

	protected override int AtlasIndex => 12;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeAMDCardDrawn(
			canApply: canApplyParameters => canApplyParameters.AbilityState.Target == Owner && canApplyParameters.AMDCard.IsCrit,
			apply: async applyParameters =>
			{
				await Use(async user =>
				{
					applyParameters.SetCard(new BasicAMDCard(applyParameters.AMDCard, 0));

					await GDTask.CompletedTask;
				});
			}
		);
	}
}
