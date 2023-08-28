using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public class EntryNode : Node
    {
        public EntryNode()
        {
            title = "Entry";
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(VoidStruct));
            outputContainer.Add(port);
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
        }
    }
}