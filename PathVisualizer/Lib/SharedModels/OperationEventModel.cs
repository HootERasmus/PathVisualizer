namespace Lib.SharedModels
{
    public class OperationEventModel
    {
        public OperationEventModel(string viewName, OperationType operationType)
        {
            ViewName = viewName;
            OperationType = operationType;
        }

        public string ViewName { get; set; }
        public OperationType OperationType { get; set; }
    }
}
