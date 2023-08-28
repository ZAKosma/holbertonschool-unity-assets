using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueTree", menuName = "Dialogue System/Dialogue Tree")]
    public class DialogueTree : ScriptableObject
    {
        public List<DialogueNodeData> nodes;
    }
} 