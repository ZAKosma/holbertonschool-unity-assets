using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public class DialogueGraphView : GraphView
    {
        public EdgeConnector<DialogueEdge> edgeConnector { get; private set; }
        public DialogueNodeEdgeConnectorListener edgeConnectorListener { get; private set; }

        
        private Vector2 defaultNodeSize = new Vector2(150, 200);


        public DialogueGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            
            edgeConnectorListener = new DialogueNodeEdgeConnectorListener();
            edgeConnector = new EdgeConnector<DialogueEdge>(edgeConnectorListener);

            CreateEntryNode(new Vector2(-200, -100));
            CreateExitNode(new Vector2(200, 100));

            
            // Register context click (right click) event to show node creation menu
            this.RegisterCallback<ContextClickEvent>(OnContextClick);

            this.graphViewChanged = OnGraphViewChanged;
        }
        
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
{
    // Check if edges have been created
    if (graphViewChange.edgesToCreate != null)
    {
        foreach (var edge in graphViewChange.edgesToCreate)
        {
            // Type check before casting
            if (edge.output.node is DialogueNode outputNode && edge.input.node is DialogueNode inputNode)
            {
                foreach (var outputPort in outputNode.outputContainer.Children())
                {
                    if (outputPort == edge.output)
                    {
                        var dialogueOption = outputPort.userData as DialogueOption;
                        dialogueOption.TargetNode = inputNode.NodeData;
                        break;
                    }
                }
            }
        }
    }

    // Check if elements have been removed
    if (graphViewChange.elementsToRemove != null)
    {
        foreach (var element in graphViewChange.elementsToRemove)
        {
            switch (element)
            {
                case Edge edge:
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    break;

                case DialogueNode dialogueNode:
                    DialogueNodesData.Remove(dialogueNode.NodeData);
                    break;

                case DialogueEdge dialogueEdge:
                    // Type check before casting
                    if (dialogueEdge.output.node is DialogueNode dialogueOutputNode && dialogueEdge.input.node is DialogueNode dialogueInputNode)
                    {
                        var outputOption = dialogueOutputNode.NodeData.Outputs.FirstOrDefault(x => x.TargetNode == dialogueInputNode.NodeData);
                        if (outputOption != null)
                        {
                            outputOption.TargetNode = null;
                        }
                    }
                    break;
            }
        }
    }

    return graphViewChange;
}


        
        private void OnContextClick(ContextClickEvent evt)
        {
            // Show node creation menu on context click (right click)
            Vector2 nodePosition = evt.localMousePosition;
            ShowNodeCreationMenu(nodePosition);
        }

        private void ShowNodeCreationMenu(Vector2 position)
        {
            // This method should show a node creation menu at the given position.
            // The specifics of how to show this menu depends on your application.

            // For now, let's just create a new node immediately:
            CreateNode("New Node", position);
        }

        private void OnEdgeDeletionRequest(Edge edge)
        {
            // Disconnect and delete the edge
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            this.Remove(edge);
        }

        public void CreateNode(string nodeName, Vector2 position)
        {
            // Create a new DialogueNodeData ScriptableObject
            var dialogueData = ScriptableObject.CreateInstance<DialogueSystem.DialogueNodeData>();
            dialogueData.speaker = nodeName;
            dialogueData.dialogueLine = "Insert dialogue here...";
            dialogueData.id = Guid.NewGuid().ToString();

            // Use dialogueData to create the node instead of nodeData
            var node = new DialogueNode(dialogueData.id, dialogueData, this, edgeConnectorListener);
            node.SetPosition(new Rect(position, Vector2.zero)); // This will set the node's position on the graph.

            AddElement(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // Get the list of all ports in the graph
            var allPorts = ports.ToList();

            // Filter out the ports that are on the same node as the starting port
            var compatiblePorts = allPorts.Where(port => port.node != startPort.node).ToList();

            // Further filter out ports that are of the same direction as the starting port
            // (i.e., don't connect input to input or output to output)
            compatiblePorts = compatiblePorts.Where(port => port.direction != startPort.direction).ToList();

            // You can add more filters here based on your needs

            return compatiblePorts;
        }

        public void CreateEntryNode(Vector2 position)
        {
            var node = new EntryNode
            {
                // configure the node as needed
            };
            node.SetPosition(new Rect(position, defaultNodeSize));
            AddElement(node);
        }

        public void CreateExitNode(Vector2 position)
        {
            var node = new ExitNode
            {
                // configure the node as needed
            };
            node.SetPosition(new Rect(position, defaultNodeSize));
            AddElement(node);
        }

    }
}