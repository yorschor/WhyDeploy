using Newtonsoft.Json.Serialization;

namespace WDCore.Serialisation;

public class ShortTypeNameBinder : DefaultSerializationBinder
{
	public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
	{
		assemblyName = null!;
		typeName = serializedType.Name;
	}

	public override Type BindToType(string? assemblyName, string typeName)
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		var type = assemblies
			.SelectMany(a => a.GetTypes())
			.FirstOrDefault(t => t.Name == typeName);
		return type!;
	}
}