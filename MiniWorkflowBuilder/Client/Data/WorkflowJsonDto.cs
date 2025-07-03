namespace MiniWorkflowBuilder.Data
{
    public class WorkflowJsonDto
    {
        public string WorkFlowName { get; set; } = string.Empty;
        public List<WorkflowActivityDto> WorkflowActivities { get; set; } = new();
    }

    public class WorkflowActivityDto
    {
        public int StepNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<WorkflowActivityJumpDto> WorkflowActivityJumps { get; set; } = new();
    }

    public class WorkflowActivityJumpDto
    {
        public int JumpToStep { get; set; }
    }
}
