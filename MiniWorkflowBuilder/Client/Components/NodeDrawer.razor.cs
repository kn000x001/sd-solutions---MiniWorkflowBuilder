using Blazor.Diagrams.Core.Models;
using Microsoft.AspNetCore.Components;
using MiniWorkflowBuilder.Data;
using Blazor.Diagrams.Core.Geometry;

namespace MiniWorkflowBuilder.Client.Components
{
    public partial class NodeDrawer : ComponentBase
    {
        [Parameter] public WorkflowNode? Node { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }

        protected void AddFilter()
        {
            Node?.NodeData.Filters.Add(new NodeData.FilterRow());
        }

        protected void RemoveFilter(NodeData.FilterRow filter)
        {
            Node?.NodeData.Filters.Remove(filter);
        }

        protected void DeleteNode()
        {
            if (Node != null)
            {
                Node.DiagramRef.Nodes.Remove(Node);
                Node = null;
                OnClose.InvokeAsync();
            }
        }

        protected void DuplicateNode()
        {
            if (Node == null) return;

            var offset = new Point(30, 30);
            var diagram = Node.DiagramRef;

            var newData = new NodeData
            {
                Label = Node.NodeData.Label,
                StepNo = diagram.Nodes.Count + 1,
                Filters = Node.NodeData.Filters
                    .Select(f => new NodeData.FilterRow
                    {
                        FieldName = f.FieldName,
                        Condition = f.Condition,
                        Value = f.Value
                    }).ToList()
            };

            var newNode = new WorkflowNode(Node.Position + offset, newData, diagram)
            {
                Title = Node.Title,
                Size = Node.Size
            };

            ((ICollection<PortModel>)newNode.Ports).Add(new PortModel(newNode, PortAlignment.Bottom));

            ((ICollection<NodeModel>)diagram.Nodes).Add(newNode);
        }


        protected void RunNode()
        {
            if (Node != null)
            {
                Console.WriteLine($"▶ Running node: {Node.Title} ({Node.NodeData.Label})");
            }
        }

    }
}
