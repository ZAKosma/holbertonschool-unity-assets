using DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Scripts
{
    public class DialogueNodeEdgeConnectorListener : IEdgeConnectorListener
    {
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // Remove edge if it's dropped outside a port
            var dialogueEdge = (DialogueEdge)edge;
            edge.input?.Disconnect(edge);
            edge.output?.Disconnect(edge);
            edge.RemoveFromHierarchy();
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            // Cast to DialogueGraphView and DialogueEdge
            var dialogueGraphView = graphView as DialogueGraphView;
            var dialogueEdge = edge as DialogueEdge;

            if (dialogueGraphView != null && dialogueEdge != null)
            {
                // Your logic for when an edge is successfully connected goes here.
                // For instance, you might update your data model representing the dialogues:

                Node outputNode = edge.output.node;
                Node inputNode = edge.input.node;

                // Check if nodes are of type DialogueNode before casting
                if (outputNode is DialogueNode dialogueOutputNode && inputNode is DialogueNode dialogueInputNode)
                {
                    foreach (var outputPort in dialogueOutputNode.outputContainer.Children())
                    {
                        if (outputPort == edge.output)
                        {
                            var dialogueOption = outputPort.userData as DialogueOption;
                            dialogueOption.TargetNode = dialogueInputNode.NodeData;
                            break;
                        }
                    }
                }
            }
        }

    }

}