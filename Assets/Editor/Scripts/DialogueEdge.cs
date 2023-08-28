using UnityEditor.Experimental.GraphView;

namespace Editor.Scripts
{
    public class DialogueEdge : Edge
    {
        public DialogueEdge() : base()
        {
            // Construct the edge as needed
        }
    
        public DialogueEdge(Port output, Port input)
        {
            this.output = output;
            this.input = input;
        }
    }
}