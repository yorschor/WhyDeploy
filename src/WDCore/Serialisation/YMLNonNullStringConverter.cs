using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace WDCore.Serialisation;

public class NonNullStringConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(string);
    }

    public object ReadYaml(IParser parser, Type type)
    {
        if (parser.Current is not Scalar scalar) throw new YamlException("Expected a scalar value.");
        parser.MoveNext(); // Always advance the parser
        return scalar.Value ?? string.Empty;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        emitter.Emit(new Scalar(value?.ToString() ?? string.Empty));
    }
}