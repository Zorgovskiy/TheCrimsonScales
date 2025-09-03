public class NullAMDCard : AMDCard
{
	public override bool Reshuffles => true;
	public override bool IsNull => true;

	public NullAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}