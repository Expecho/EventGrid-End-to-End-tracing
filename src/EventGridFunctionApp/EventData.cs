namespace EventGridFunctionApp
{
    public class EventData
    {
        public bool ThrowError { get; set; }
        public string OperationId { get; set; }
        public string ParentOperationId { get; set; }
    }
}