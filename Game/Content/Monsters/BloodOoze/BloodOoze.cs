using System.Collections.Generic;

public class BloodOoze : Ooze
{
	public override string Name => "BloodOoze";

	public override IEnumerable<MonsterAbilityCardModel> Deck => BloodOozeAbilityCard.Deck;
}