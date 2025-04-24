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
public static class DialogueInjector
{
    private static List<DialogueInjection> pendingInjections = new List<DialogueInjection>();
    private static bool isHooked = false;

    public static void Register(DialogueInjection injection)
    {
        pendingInjections.Add(injection);
        HookUpdateLoop();
    }

    private static void HookUpdateLoop()
    {
        if (isHooked) return;
        isHooked = true;

        MelonCoroutines.Start(WaitForNPCsAndInject());
    }

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
