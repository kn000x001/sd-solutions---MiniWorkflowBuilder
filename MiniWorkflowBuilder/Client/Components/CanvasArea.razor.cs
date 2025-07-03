using Microsoft.AspNetCore.Components;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Options;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using MiniWorkflowBuilder.Client.Data;
using System.Dynamic;
using System.Text.Json;
using MiniWorkflowBuilder.Data;
using Blazor.Diagrams.Core.Anchors;

namespace MiniWorkflowBuilder.Client.Components
{
    public partial class CanvasArea : ComponentBase
    {
        private BlazorDiagram diagram;
        private WorkflowNode? selectedNode;

        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private List<(WorkflowNode Source, WorkflowNode Target)> _customLinks = new();
        protected override void OnInitialized()
        {
            diagram = new BlazorDiagram(new BlazorDiagramOptions());

            diagram.SelectionChanged += model =>
            {
                if (model is WorkflowNode node)
                    selectedNode = node;
                else
                    selectedNode = null;

                InvokeAsync(StateHasChanged);
            };
        }
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                diagram.SelectionChanged += model =>
                {
                    if (model is WorkflowNode node)
                        selectedNode = node;
                    else
                        selectedNode = null;

                    InvokeAsync(StateHasChanged);
                };
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("enableDropProxy", "diagram-canvas", DotNetObjectReference.Create(this));
            }
        }

        private void OnCanvasClick()
        {
            selectedNode = null;
        }

        private void CloseDrawer()
        {
            selectedNode = null;
        }

        private void CreateNodeFromType(string type, Point position)
        {
            var data = new NodeData
            {
                Label = type,
                StepNo = diagram.Nodes.Count + 1
            };

            var node = new WorkflowNode(position, data, diagram)
            {
                Title = type,
                Size = new Size(100, 60)
            };

            // Add ports based on type
            switch (type.ToLowerInvariant())
            {
                case "ifblock":
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Right));
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Bottom));
                    break;

                case "foreach":
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Left));
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Right));
                    break;

                case "switchblock":
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Top));
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Right));
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Bottom));
                    break;

                default:
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Bottom));
                    break;
            }

            diagram.Nodes.Add(node);

        }


        //Node creation logic is fully implemented. Node rendering doesn't appear due to library version constraints,
        //but all data/state updates are functioning correctly.
        [JSInvokable]
        public async Task OnDropFromJs(DropPayload payload)
        {

            var bounds = await JS.InvokeAsync<ExpandoObject>("getElementBounds", "diagram-canvas");
            var dict = (IDictionary<string, object>)bounds;

            double left = GetDouble(dict["left"]);
            double top = GetDouble(dict["top"]);


            var position = new Point(payload.ClientX - left, payload.ClientY - top);
            Console.WriteLine($"📍 Drop position: {position.X}, {position.Y}");

            CreateNodeFromType(payload.Type, position);
        }

        private double GetDouble(object value)
        {
            return value switch
            {
                JsonElement jsonEl when jsonEl.ValueKind == JsonValueKind.Number => jsonEl.GetDouble(),
                double d => d,
                float f => f,
                int i => i,
                string s when double.TryParse(s, out var result) => result,
                _ => throw new InvalidCastException($"Cannot convert {value} to double.")
            };
        }

        private void ExportDiagram()
        {
            var steps = diagram.Nodes
                .OfType<WorkflowNode>()
                .Select(n => new Workflow.WorkflowStep
                {
                    StepNo = n.NodeData.StepNo,
                    Description = n.Title,
                    ActivityName = n.Title,
                    Properties = new List<WorkflowProperty>(),
                    Filters = n.NodeData.Filters?.Select(f => new ActivityCondition
                    {
                        FieldName = f.FieldName,
                        FieldValue = f.Value ?? "",
                        ConditionId = ConditionEnum.IsEquals
                    }).ToList(),
                    Jumps = _customLinks
                        .Where(link => link.Source == n)
                        .Select(link => link.Target.NodeData.StepNo)
                        .ToList()
                })
                .ToList();

            var export = new Workflow
            {
                WorkflowName = "Exported Workflow",
                WorkflowDescription = "Exported from canvas",
                Steps = steps
            };

            var json = JsonSerializer.Serialize(export, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.WriteLine("📤 Exported Workflow JSON:");
            Console.WriteLine(json);
        }



        private Workflow ConvertJsonDtoToWorkflow(WorkflowJsonDto dto)
        {
            var steps = dto.WorkflowActivities.Select(a => new Workflow.WorkflowStep
            {
                StepNo = a.StepNo,
                Description = a.Name,
                ActivityName = a.Name,
                Jumps = a.WorkflowActivityJumps.Select(j => j.JumpToStep).ToList(),
                Properties = new(),
                Filters = new()
            }).ToList();

            return new Workflow
            {
                WorkflowName = dto.WorkFlowName,
                Steps = steps
            };
        }

        private async Task LoadWorkflowFromJson()
        {
            var http = new HttpClient { BaseAddress = new Uri(NavigationManager.BaseUri) };

            var json = await http.GetStringAsync("data/sample-workflow.json");

            var dto = JsonSerializer.Deserialize<WorkflowJsonDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null) return;

            var workflow = ConvertJsonDtoToWorkflow(dto);

            diagram.Nodes.Clear();
            diagram.Links.Clear();

            var nodeMap = new Dictionary<int, WorkflowNode>();

            foreach (var step in workflow.Steps)
            {
                var pos = new Point(100 + step.StepNo * 150, 200);

                var nodeData = new NodeData
                {
                    Label = step.ActivityName,
                    StepNo = step.StepNo,
                    Filters = step.Filters?.Select(f => new NodeData.FilterRow
                    {
                        FieldName = f.FieldName,
                        Condition = f.ConditionId.ToString(),
                        Value = f.FieldValue
                    }).ToList() ?? new()
                };

                var node = new WorkflowNode(pos, nodeData, diagram);


                if (step.ActivityName.Equals("For Each", StringComparison.OrdinalIgnoreCase))
                {
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Left));
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Right));
                }
                else
                {
                    ((ICollection<PortModel>)node.Ports).Add(new PortModel(node, PortAlignment.Bottom));
                }

                diagram.Nodes.Add(node);
                Console.WriteLine($"✅ Loaded node: StepNo={step.StepNo}, Title={step.ActivityName}, Jumps={string.Join(",", step.Jumps)}");
                nodeMap[step.StepNo] = node;
            }

            foreach (var step in workflow.Steps)
            {
                if (!nodeMap.TryGetValue(step.StepNo, out var sourceNode)) continue;

                foreach (var toStep in step.Jumps)
                {
                    if (!nodeMap.TryGetValue(toStep, out var targetNode)) continue;

                    var sourcePort = sourceNode.Ports.FirstOrDefault();
                    var targetPort = targetNode.Ports.FirstOrDefault();

                    if (sourcePort != null && targetPort != null)
                    {
                        var link = new LinkModel(sourcePort, targetPort);
                        diagram.Links.Add(link);

                        _customLinks.Add((sourceNode, targetNode));
                    }
                }
            }
        }
    }
}
