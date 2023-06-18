using Scriban;
using WDBase;
using WDCore.Serialisation.Results;
using YamlDotNet.Serialization;

namespace WDCore.Serialisation;

public class WDYamlSerializer : IWDSerializer
{
    public Result<string> Serialize<T>(T obj)
    {
        // You can implement this later, for now, it will return an error.
        return new ErrorResult<string>("Serialization to YAML is not implemented.",
            new List<Error> { new("NotImplemented", "Serialization to YAML is not currently supported.") });
    }

    public Result<T> Deserialize<T>(string input)
    {
        try
        {
            var template = Template.Parse(input);
            var renderedYaml = template.Render();
            var operationToTypeMapping = GenerateOperationToTypeMappings();

            foreach (var kvp in operationToTypeMapping)
            {
                var oldPattern = $"- {kvp.Key}:";
                var newPattern = $"- !{kvp.Value.Name} ";
                renderedYaml = renderedYaml.Replace(oldPattern, newPattern);
            }

            var builder = new DeserializerBuilder();

            foreach (var kvp in operationToTypeMapping)
                builder = builder.WithTagMapping($"!{kvp.Value.Name}", kvp.Value);

            var deserializer = builder
                .WithTypeConverter(new NonNullStringConverter())
                .Build();
            var desObj = deserializer.Deserialize<T>(renderedYaml);
            return new SuccessResult<T>(desObj);
        }
        catch (Exception e)
        {
            if (e.InnerException is InvalidOperationException innerE)
            {
                return new OperationTypeNotFoundResult<T>("Failed to deserialize YAML to object. Hint: You might be missing a Module",
                    new List<Error> { new("DeserializationError", innerE.Message) });
            }
            
            return new ErrorResult<T>("Failed to deserialize YAML to object.",
                    new List<Error> { new("DeserializationError", e.Message) });
        }
    }

    private Dictionary<string, Type> GenerateOperationToTypeMappings()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(BaseOperation).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                        t != typeof(BaseOperation))
            .ToDictionary(t => t.Name.Replace("Operation", ""), t => t);
    }
}