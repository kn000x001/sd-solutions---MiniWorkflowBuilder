namespace MiniWorkflowBuilder.Data
{
    public class NodeData
    {
        public int StepNo { get; set; }
        public string Label { get; set; } = "";
        public List<FilterRow> Filters { get; set; } = new();

        public class FilterRow
        {
            public string FieldName { get; set; } = "";
            public string Condition { get; set; } = "";
            public string Value { get; set; } = "";
        }
    }
}
