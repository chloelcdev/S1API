using System;

using UnityEngine.Events;
#if (IL2CPP)
using Il2CppScheduleOne.Dialogue;
#elif (MONO)
using ScheduleOne.Dialogue;
#endif

/// <summary>
/// Provides functionality to register a listener for dialogue choice selection in a dialogue system.
/// This class is used to associate a specific choice label with a callback function
/// that will be invoked when the corresponding dialogue choice is selected.
/// </summary>
public static class DialogueChoiceListener
{
    /// <summary>
    /// Represents the label of the expected dialogue choice that triggers a specific action.
    /// Used within the dialogue handling system to compare against the player's choice and invoke
    /// associated actions if the choice matches.
    /// </summary>
    private static string expectedChoiceLabel;

    /// <summary>
    /// A delegate of type <see cref="Action"/> to be executed when the expected dialogue choice is selected.
    /// </summary>
    /// <remarks>
    /// The <c>callback</c> is registered to execute when a specific dialogue choice is selected and matches
    /// the expected label. It is invoked automatically once the choice condition is satisfied and is reset to null after execution.
    /// </remarks>
    private static Action callback;

    /// <summary>
    /// Registers a callback to be executed when a specific dialogue choice is selected.
    /// </summary>
    /// <param name="handlerRef">Reference to the <see cref="DialogueHandler"/> to listen for dialogue choices.</param>
    /// <param name="label">The label of the dialogue choice to listen for.</param>
    /// <param name="action">The callback action to execute when the specified choice is selected.</param>
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

    /// <summary>
    /// Executes the registered callback when a dialogue choice matches the expected label.
    /// </summary>
    /// <remarks>
    /// This method is invoked internally once the appropriate dialogue choice is selected.
    /// It ensures that the associated callback is executed and cleared afterward, enabling
    /// one-time invocation behavior.
    /// </remarks>
    private static void OnChoice()
    {
        callback?.Invoke();
        callback = null; // optional: remove if one-time use
    }
}