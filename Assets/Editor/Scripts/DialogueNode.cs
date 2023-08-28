using System.Collections.Generic;
using DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Editor.Scripts
{
    public class DialogueNode : Node
    {
        public string Guid;
        public DialogueNodeData NodeData;

        private TextField dialogueTextField;
        private List<TextField> choicesTextFields = new List<TextField>();
        private DialogueGraphView _graphView; // reference to the graph view
        private DialogueNodeEdgeConnectorListener edgeConnectorListener;

        private Port _inputPort; // reference to the input port


        public DialogueNode(string guid, DialogueNodeData nodeData, DialogueGraphView graphView, DialogueNodeEdgeConnectorListener edgeConnectorListener)
        {
            this._graphView = graphView;
            this.edgeConnectorListener = edgeConnectorListener;

            // Create text field for dialogue
            dialogueTextField = new TextField("Dialogue");
            dialogueTextField.value = nodeData.dialogueLine;
            dialogueTextField.RegisterValueChangedCallback(evt =>
            {
                nodeData.dialogueLine = evt.newValue;
            });
            this.extensionContainer.Add(dialogueTextField);

            foreach (var choice in nodeData.options)
            {
                AddChoicePort(choice);
            }

            // Add choice button
            this.extensionContainer.Add(GenerateAddChoiceButton());

            // Add input port for connections from other nodes
            _inputPort = GeneratePort(Direction.Input); // This should have capacity multi.
            _inputPort.portName = "Input";
            inputContainer.Add(_inputPort);

            RefreshExpandedState();
            RefreshPorts();
        }

        private Button GenerateAddChoiceButton()
        {
            return new Button(() =>
            {
                AddChoicePort(new DialogueOption());
            })
            {
                text = "Add Choice"
            };
        }

        private void AddChoicePort(DialogueOption dialogueOption)
        {
            var generatedPort = GeneratePort(Direction.Output);
            generatedPort.portName = dialogueOption.OptionText;
            generatedPort.userData = dialogueOption;

            var edgeConnector = new EdgeConnector<DialogueEdge>(edgeConnectorListener);
            generatedPort.AddManipulator(edgeConnector);

            // Add listeners for the port events
            AddPortEventListeners(generatedPort);

            outputContainer.Add(generatedPort);
            RefreshPorts(); // Add this line to refresh ports after adding new ones.

            // Add TextField for choice text
            this.extensionContainer.Add(GenerateChoiceTextField(generatedPort, dialogueOption));
        }

        private TextField GenerateChoiceTextField(Port generatedPort, DialogueOption dialogueOption)
        {
            TextField choiceTextField = new TextField("Choice Text");
            choiceTextField.value = dialogueOption.OptionText;
            choiceTextField.RegisterValueChangedCallback(evt =>
            {
                generatedPort.portName = evt.newValue;
                dialogueOption.OptionText = evt.newValue;
            });
            choicesTextFields.Add(choiceTextField);
            return choiceTextField;
        }

        private void AddPortEventListeners(Port generatedPort)
        {
            generatedPort.RegisterCallback<DragUpdatedEvent>(evt => {
                var sourcePortView = ((Port)evt.currentTarget);
                var sourceOption = sourcePortView.userData as DialogueOption;

                if (sourceOption.TargetNode != null)
                {
                    sourceOption.TargetNode = null;
                }
            });

            generatedPort.RegisterCallback<ExecuteCommandEvent>(evt => {
                if (evt.commandName == "Connect")
                {
                    var sourcePortView = ((Port)evt.currentTarget);
                    var sourceOption = sourcePortView.userData as DialogueOption;
                    var targetPortView = ((Port)evt.target);
                    var targetNode = (DialogueNode)targetPortView.node;

                    sourceOption.TargetNode = targetNode.NodeData;
                }
            });

            generatedPort.RegisterCallback<MouseDownEvent>(evt => CreateEdgeForPort(evt, generatedPort));
        }

        private void CreateEdgeForPort(MouseDownEvent evt, Port generatedPort)
        {
            // Start edge dragging when the port is clicked
            if (evt.button == 0) // Left click
            {
                var edge = new DialogueEdge()
                {
                    output = generatedPort,
                    input = null, // Will be assigned when the edge is connected
                };
                edge.input.Add(new EdgeControl());

                this.AddToClassList("active");

                edge.pickingMode = PickingMode.Ignore; // Ignore the edge for events
                _graphView.AddElement(edge);

                evt.StopPropagation();
            }
        }



        private Port GeneratePort(Direction portDirection)
        {
            Port.Capacity capacity = (portDirection == Direction.Input) ? Port.Capacity.Multi : Port.Capacity.Single;
            return InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(DialogueOption));
        }


    }
}
