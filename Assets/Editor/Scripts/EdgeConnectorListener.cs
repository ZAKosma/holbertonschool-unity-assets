using DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Scripts
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        public void OnDrop(GraphView graphView, Edge edge)
        {
            // Handle edge drop (connect) event
            var sourcePortView = edge.output as Port;
            var targetPortView = edge.input as Port;

            if (sourcePortView != null && targetPortView != null)
            {
                var sourceNode = sourcePortView.node as DialogueNode;
                var targetNode = targetPortView.node as DialogueNode;

                if (sourceNode != null && targetNode != null)
                {
                    var dialogueOption = sourcePortView.userData as DialogueOption;
                    if (dialogueOption != null)
                    {
                        dialogueOption.TargetNode = targetNode.NodeData;
                    }
                }
            }
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // Handle edge drop outside any port event
        }
    }
}