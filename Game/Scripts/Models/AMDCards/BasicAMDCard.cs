public class BasicAMDCard : AMDCard
{
	private readonly int _value;

	public BasicAMDCard(string textureAtlasPath, int atlasIndex, int textureAtlasColumnCount, int textureAtlasRowsCount, int value)
		: base(textureAtlasPath, atlasIndex, textureAtlasColumnCount, textureAtlasRowsCount)
	{
		_value = value;
	}

	protected override int? GetValue()
	{
		return _value;
	}
}