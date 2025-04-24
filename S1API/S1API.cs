using MelonLoader;
using S1API;

[assembly: MelonInfo(typeof(S1API.S1API), "S1API", "1.0.0", "KaBooMa")]

namespace S1API
{
    public class S1API : MelonMod
    {
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName != "Main") return;

            DialogueInjector.Register(new DialogueInjection(
                npc: "Ray",
                container: "EstateAgent_Sell",
                from: "8e2ef594-96d9-43f2-8cfa-6efaea823a56",
                to: "b6f6a4ce-849c-4047-b705-7a20440de0e0",
                label: "manor",
                text: "Hilltop Manor (<PRICE>)",
                onConfirmed: Success
            ));
        }

        private static void Success()
        {
            MelonLogger.Msg("it works!");
        }
    
    }
}