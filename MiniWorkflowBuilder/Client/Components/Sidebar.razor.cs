using Microsoft.AspNetCore.Components.Web;

namespace MiniWorkflowBuilder.Client.Components
{
    public partial class Sidebar
    {
        public record ToolbarItem(string Label, string Src, string Type);


        private List<ToolbarItem> ToolbarIcons = new()
        {
            new("Start", "icons/start.svg", "RegularBlock"),
            new("If", "icons/if.svg", "ifblock"),
            new("Switch", "icons/switch.svg", "switchblock"),
            new("ForEach", "icons/foreach.svg", "Foreach"),
            new("Scope", "icons/scope.svg", "scopeblock"),
            new("End", "icons/end.svg", "endblock"),
        };
    }
}
