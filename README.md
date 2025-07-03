# MiniWorkflowBuilder

This is my implementation of the Blazor WebAssembly-based workflow builder.

## ✅ Features Completed (As Required by the Task)

- Drag-and-drop nodes (like `ifblock`, `foreach`, etc.) onto the canvas.
- Auto-positioning of dropped nodes using mouse coordinates.
- Edit node properties (label, filters, etc.) via a side drawer.
- Duplicate and delete nodes from the UI.
- Auto-generate node links by dragging ports.
- Export the current diagram as JSON using the `Export Diagram` button.
- Load an existing workflow from a JSON file (`sample-workflow.json`).
- Proper linking and jump mapping between workflow steps.

## ⚠️ Known Limitations

- **Node rendering (like icons or visual blocks on canvas)** doesn’t work fully because I am using version `3.0.3` of `Z.Blazor.Diagrams`,
- which doesn’t support custom `NodeComponent<>` rendering.
- I **couldn't upgrade** the diagram library to 3.1+ because that version isn't available via NuGet at the time of doing this project.
- Some style customization (like colored nodes or icons) is missing for the same reason.
- Nodes are **positioned and linked correctly**, but the canvas may not show visual feedback as expected but all actions are shown in the web browser console.

## 📦 How to Run

1. Clone the repo:
   ```bash
   git clone https://github.com/kn000x001/sd-solutions---MiniWorkflowBuilder.git
   cd MiniWorkflowBuilder

## 🎥 Demo

Here’s a quick demo of the project in action:

![Workflow Demo](demo.gif)