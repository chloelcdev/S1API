using System;

using UnityEngine.Events;
#if (IL2CPP)
using Il2CppScheduleOne.Dialogue;
#elif (MONO)
using ScheduleOne.Dialogue;
#endif
public static class DialogueChoiceListener
{
    private static string expectedChoiceLabel;
    private static Action callback;

    public static void Register(DialogueHandler handlerRef, string label, Action action)
    {
        expectedChoiceLabel = label;
        callback = action;

        if (handlerRef != null)
        {
            void ForwardCall() => OnChoice();

            // ✅ IL2CPP-safe: explicit method binding via wrapper
            handlerRef.onDialogueChoiceChosen.AddListener((UnityAction<string>)delegate (string choice)
            {
                if (choice == expectedChoiceLabel)
                    ((UnityAction)ForwardCall).Invoke();
            });
        }
    }

    private static void OnChoice()
    {
        callback?.Invoke();
        callback = null; // optional: remove if one-time use
    }
}