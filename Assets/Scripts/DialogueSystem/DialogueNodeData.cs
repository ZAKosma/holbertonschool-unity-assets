using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DialogueNode", menuName = "ScriptableObjects/DialogueNode", order = 1)]
    public class DialogueNodeData : ScriptableObject
    {
        public string speaker;
        public string dialogueLine;
        public List<DialogueOption> options;
    
        public Vector2 position; // This will store the node's position in the editor window
        public string id; // This will store the node's unique identifier

        public DialogueNodeData()
        {
            speaker = "";
            dialogueLine = "";
            options = new List<DialogueOption>();
        }
    
        // Method to check if the node has options (children)
        public bool IsConnected()
        {
            return options != null && options.Count > 0;
        }
    }
}