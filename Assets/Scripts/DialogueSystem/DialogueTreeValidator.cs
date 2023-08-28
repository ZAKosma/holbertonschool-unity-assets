using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;

public static class DialogueTreeValidator
{
    public static bool ValidateDialogueTree(DialogueTree dialogueTree)
    {
        // Validation logic
        return ValidateNoUnconnectedNodes(dialogueTree) && ValidateNoCircularReferences(dialogueTree);
    }

    private static bool ValidateNoUnconnectedNodes(DialogueTree dialogueTree)
    {
        foreach (DialogueNodeData node in dialogueTree.nodes)
        {
            // Check if the node is connected
            if (!node.IsConnected())
            {
                Debug.LogError("Dialogue tree contains unconnected node: " + node.name);
                return false;
            }
        }

        return true;
    }

    private static bool ValidateNoCircularReferences(DialogueTree dialogueTree)
    {
        HashSet<DialogueNodeData> visitedNodes = new HashSet<DialogueNodeData>();

        foreach (DialogueNodeData node in dialogueTree.nodes)
        {
            if (!DepthFirstSearch(node, visitedNodes))
            {
                Debug.LogError("Dialogue tree contains a circular reference.");
                return false;
            }
        }

        return true;
    }

    private static bool DepthFirstSearch(DialogueNodeData nodeData, HashSet<DialogueNodeData> visitedNodes)
    {
        if (visitedNodes.Contains(nodeData))
        {
            return false; // Node has already been visited, so there is a circular reference.
        }

        visitedNodes.Add(nodeData);

        foreach (DialogueOption option in nodeData.options)
        {
            // Perform the depth-first search on the target node of the option
            if (!DepthFirstSearch(option.TargetNode, visitedNodes))
            {
                return false;
            }
        }

        visitedNodes.Remove(nodeData); // Backtracking

        return true;
    }

}