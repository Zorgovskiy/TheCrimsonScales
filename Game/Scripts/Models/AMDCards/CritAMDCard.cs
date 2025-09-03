public class CritAMDCard : AMDCard
{
	public override bool Reshuffles => true;
	public override bool IsCrit => true;

	public CritAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}