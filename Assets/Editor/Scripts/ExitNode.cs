using UnityEditor.Experimental.GraphView;

namespace Editor.Scripts
{
    public struct VoidStruct { }

    public class ExitNode : Node
    {
        public ExitNode()
        {
            title = "Exit";
            var port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(VoidStruct));
            inputContainer.Add(port);
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
        }
    }
}