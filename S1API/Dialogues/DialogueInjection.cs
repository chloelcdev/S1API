using System;

/// <summary>
/// Represents a structure for defining custom dialogue injection into existing NPC dialogue flows.
/// </summary>
/// <remarks>
/// This class is utilized to dynamically modify or extend NPC dialogue options during runtime by specifying
/// the details of the dialogue to be injected, including the NPC, the dialogue container, the source
/// and target nodes, choice label, and choice text along with a confirmation callback.
/// </remarks>
public class DialogueInjection
{
    /// <summary>
    /// Represents the name of the Non-Playable Character (NPC) targeted for dialogue injection.
    /// This property specifies which NPC will be associated with a dialogue modification or addition.
    /// The value is typically used to identify the NPC within the game world, matching an NPC's name
    /// against this value to determine where to apply the dialogue injection.
    /// </summary>
    public string NpcName;

    /// <summary>
    /// Represents the name of the dialogue container where a specific dialogue injection is applied.
    /// This variable is used to identify and match the appropriate dialogue container for modifying or adding dialogue choices.
    /// </summary>
    public string ContainerName;

    /// <summary>
    /// The unique identifier of the starting dialogue node from which a new dialogue choice will originate.
    /// </summary>
    /// <remarks>
    /// This property is used to locate the specific dialogue node within a dialogue container to attach a new choice.
    /// It plays a critical role in ensuring that the correct node is modified during the dialogue injection process.
    /// </remarks>
    public string FromNodeGuid;

    /// <summary>
    /// Represents the unique identifier of the target node in a dialogue system.
    /// Used to establish a link between a dialogue choice and its associated target node.
    /// </summary>
    public string ToNodeGuid;

    /// <summary>
    /// Represents the label assigned to a dialogue choice that can be used
    /// to uniquely identify or describe the choice. This property is utilized
    /// during dialogue injection to associate actions or outcomes with a specific
    /// choice in a conversation flow.
    /// </summary>
    public string ChoiceLabel;

    /// <summary>
    /// Represents the text for a dialogue choice that can be presented to the player.
    /// This variable is used during the dialogue injection process to specify the
    /// content of a selectable dialogue option.
    /// </summary>
    public string ChoiceText;

    /// <summary>
    /// Represents an Action delegate that is invoked when a dialogue choice
    /// is confirmed by the user through interaction.
    /// </summary>
    public Action OnConfirmed;

    /// Represents a single dialogue injection used for dynamically adding custom dialogue
    /// options to NPC dialogue containers during gameplay. This class defines the details
    /// of the NPC, the container, dialogue nodes, and the callback for when the dialogue is confirmed.
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