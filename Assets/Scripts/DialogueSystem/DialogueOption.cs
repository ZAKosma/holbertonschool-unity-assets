namespace DialogueSystem
{
    [System.Serializable]
    public class DialogueOption
    {
        public string OptionText;
        public DialogueNodeData TargetNode;
        
        public DialogueOption()
        {
            OptionText = "New Choice";
        }
    }
}