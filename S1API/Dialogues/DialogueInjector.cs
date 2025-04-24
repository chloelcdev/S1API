using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
#if IL2CPP
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.Schedules;
#else
using ScheduleOne.Dialogue;
using ScheduleOne.NPCs;
using ScheduleOne.NPCs.Schedules;
#endif

/// <summary>
/// The DialogueInjector class provides functionality to dynamically inject dialogue options
/// into NPC dialogue containers during gameplay. This allows for runtime modification of
/// dialogue trees by registering custom dialogue injections.
/// </summary>
public static class DialogueInjector
{
    /// <summary>
    /// A private static list of pending dialogue injection requests awaiting processing.
    /// Each entry in the list represents a <see cref="DialogueInjection"/> that defines
    /// the parameters for dynamically adding dialogue options to NPCs in the game.
    /// </summary>
    private static List<DialogueInjection> pendingInjections = new List<DialogueInjection>();

    /// <summary>
    /// Indicates whether the dialogue injection system has been hooked into the update loop.
    /// This flag prevents multiple redundant hook attempts and ensures the injection process
    /// is only initiated when necessary.
    /// </summary>
    private static bool isHooked = false;

    /// Registers a new dialogue injection to be processed during the update loop.
    /// <param name="injection">The dialogue injection object containing details about the NPC,
    /// the dialogue structure, and callback for confirmation handling.</param>
    public static void Register(DialogueInjection injection)
    {
        pendingInjections.Add(injection);
        HookUpdateLoop();
    }

    /// <summary>
    /// Hooks the update loop if it is not already hooked. Ensures that pending dialogue injections
    /// are processed by starting a coroutine that waits for NPCs to be available and performs the injections.
    /// </summary>
    /// <remarks>
    /// This method is called internally when a new dialogue injection is registered. It guarantees that
    /// the process for injecting dialogues into NPCs starts only once, avoiding multiple parallel loops.
    /// </remarks>
    private static void HookUpdateLoop()
    {
        if (isHooked) return;
        isHooked = true;

        MelonCoroutines.Start(WaitForNPCsAndInject());
    }

    /// Waits until all NPCs are loaded and active in the scene, then processes and injects dialogue data
    /// from the pending injections list into the corresponding NPCs. The method ensures that dialogue
    /// modifications are applied dynamically during runtime by matching NPC names and dialogue configurations.
    /// This method operates using a coroutine, iterating frame by frame to determine when conditions are met
    /// for the dialogue injection to occur. If an NPC matching the injection criteria is found, the dialogue
    /// modifications are applied, and the injection is removed from the pending list.
    /// <returns>
    /// An enumerator to be executed as a coroutine that performs the NPC checks and dialogue injection process over time.
    /// </returns>
    private static System.Collections.IEnumerator WaitForNPCsAndInject()
    {
        while (pendingInjections.Count > 0)
        {
            for (int i = pendingInjections.Count - 1; i >= 0; i--)
            {
                var injection = pendingInjections[i];
                var npcs = GameObject.FindObjectsOfType<NPC>();
                NPC target = null;

                for (int j = 0; j < npcs.Length; j++)
                {
                    if (npcs[j] != null && npcs[j].name.Contains(injection.NpcName))
                    {
                        target = npcs[j];
                        break;
                    }
                }

                if (target != null)
                {
                    TryInject(injection, target);
                    pendingInjections.RemoveAt(i);
                }
            }

            yield return null; // Wait one frame
        }
    }

    /// Attempts to inject a dialogue choice and corresponding link into the specified NPC's dialogue structure.
    /// <param name="injection">The dialogue injection containing the desired configuration for the injected dialogue.</param>
    /// <param name="npc">The target NPC to which the dialogue choice will be added.</param>
    private static void TryInject(DialogueInjection injection, NPC npc)
    {
        var handler = npc.GetComponent<DialogueHandler>();
        var dialogueEvent = npc.GetComponentInChildren<NPCEvent_LocationDialogue>(true);
        if (dialogueEvent == null || dialogueEvent.DialogueOverride == null) return;

        if (dialogueEvent.DialogueOverride.name != injection.ContainerName) return;

        var container = dialogueEvent.DialogueOverride;
        if (container.DialogueNodeData == null) return;

        DialogueNodeData node = null;
        for (int i = 0; i < container.DialogueNodeData.Count; i++)
        {
            var n = container.DialogueNodeData[i];
            if (n != null && n.Guid == injection.FromNodeGuid)
            {
                node = n;
                break;
            }
        }

        if (node == null) return;

        var choice = new DialogueChoiceData
        {
            Guid = System.Guid.NewGuid().ToString(),
            ChoiceLabel = injection.ChoiceLabel,
            ChoiceText = injection.ChoiceText
        };

        var choiceList = new List<DialogueChoiceData>();
        if (node.choices != null)
            choiceList.AddRange(node.choices);

        choiceList.Add(choice);
        node.choices = choiceList.ToArray();

        var link = new NodeLinkData
        {
            BaseDialogueOrBranchNodeGuid = injection.FromNodeGuid,
            BaseChoiceOrOptionGUID = choice.Guid,
            TargetNodeGuid = injection.ToNodeGuid
        };

        if (container.NodeLinks == null)
            #if IL2CPP
            container.NodeLinks = new Il2CppSystem.Collections.Generic.List<NodeLinkData>();
#else
        container.NodeLinks = new List<NodeLinkData>();
#endif
        container.NodeLinks.Add(link);

        DialogueChoiceListener.Register(handler, injection.ChoiceLabel, injection.OnConfirmed);

        MelonLogger.Msg($"[DialogueInjector] Injected '{injection.ChoiceLabel}' into NPC '{npc.name}'");
    }
}
