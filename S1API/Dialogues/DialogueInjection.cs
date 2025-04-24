using System;

public class DialogueInjection
{
    public string NpcName;
    public string ContainerName;
    public string FromNodeGuid;
    public string ToNodeGuid;
    public string ChoiceLabel;
    public string ChoiceText;
    public Action OnConfirmed;

    public DialogueInjection(string npc, string container, string from, string to, string label, string text, Action onConfirmed)
    {
        NpcName = npc;
        ContainerName = container;
        FromNodeGuid = from;
        ToNodeGuid = to;
        ChoiceLabel = label;
        ChoiceText = text;
        OnConfirmed = onConfirmed;
    }
}