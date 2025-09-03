public class CurseAMDCard : AMDCard
{
	public override bool RemoveAfterDraw => true;
	public override bool IsNull => true;

	public CurseAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
	}
}