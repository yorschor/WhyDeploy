using WDBase;

namespace WDCore.Serialisation.Results;

public class OperationTypeNotFoundResult<T> : ErrorResult<T>
{
	public OperationTypeNotFoundResult(string message) : base(message)
	{
	}

	public OperationTypeNotFoundResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
	{
	}
}