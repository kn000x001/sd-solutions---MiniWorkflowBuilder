using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using MiniWorkflowBuilder.Data;
using Blazor.Diagrams.Core;

namespace MiniWorkflowBuilder.Client.Components
{
    public class WorkflowNode : NodeModel
    {
        public NodeData NodeData { get; set; }

        // ✅ Manually assigned Diagram reference
        public Diagram DiagramRef { get; }

        public WorkflowNode(Point position, NodeData data, Diagram diagram)
            : base(position)
        {
            NodeData = data;
            DiagramRef = diagram;
        }
    }
}
