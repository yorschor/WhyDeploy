using WDBase;

namespace WDCore.Events;

public class OperationResultEventArgs : EventArgs
{
    public OperationResultEventArgs(BaseOperation operation, Result result)
    {
        Operation = operation;
        ExecutionResult = result;
    }

    public BaseOperation Operation { get; }
    public Result ExecutionResult { get; }
}