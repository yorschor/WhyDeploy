using System.Reflection;
using WDBase;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace WDCore.Serialisation;

public class YamlOperationTypeResolver : INodeTypeResolver
{
    public bool Resolve(NodeEvent? nodeEvent, ref Type currentType)
    {
        if (nodeEvent is MappingStart mapping && currentType == typeof(BaseOperation))
        {
            var operationType = Assembly.GetExecutingAssembly()
                .GetType($"{mapping.Tag}", false);

            if (operationType != null && !operationType.IsAbstract &&
                typeof(BaseOperation).IsAssignableFrom(operationType))
            {
                currentType = operationType;
                return true;
            }
        }

        return false;
    }
}