public class ClampTrap : CS2Item
{
	public override string Name => "Clamp Trap";
	public override int ItemNumber => 34;
	public override int ShopCount => 1;
	public override int Cost => 50;
	public override ItemType ItemType => ItemType.Small;
	public override ItemUseType ItemUseType => ItemUseType.Consume;

	protected override int AtlasIndex => 30;

	protected override void Subscribe()
	{
		base.Subscribe();

		SubscribeTurnEnded(
			canApply: character => character == Owner,
			apply: async character =>
			{
				await Use(async user =>
				{
					await AbilityCmd.CreateTraps(damage: 3, range: 1, conditions: [Conditions.Immobilize], performer: user);
				});
			}
		);
	}
}