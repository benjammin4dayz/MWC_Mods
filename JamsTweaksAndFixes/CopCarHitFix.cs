using HutongGames.PlayMaker;
using MSCLoader;
using System;
using UnityEngine;
using static JamsTweaksAndFixes.Helpers;

namespace JamsTweaksAndFixes;

internal class CopCarHitFix
{
    public static SettingsCheckBox AddSetting(Action onChange)
    {
        var setting = Settings.AddCheckBox("fix_cop_carhit", "Cop ragdoll", false, onChange);
        _ = Settings.AddText("Makes cops ragdoll after being run over.");
        return setting;
    }

    public static void Apply()
    {
        var policja = GameObject.Find("TRAFFIC")
            ?.transform.Find("Police")
            ?.GetPlayMaker("Police")
            ?? throw new Exception("Missing police base");
        policja.InitializeFSM();

        for (var i = 1; i <= 2; i++)
        {
            var carHit = policja.GetVariable<FsmGameObject>($"CopAlc{i}")
                ?.Value
                ?.transform.Find("Pivot/Char/HumanTriggerCop")
                ?.GetPlayMaker("CarHit")
                ?? throw new Exception("Missing CarHit");
            carHit.InitializeFSM();

            var state = carHit.GetState("Accident")
                ?? throw new Exception("Missing 'Accident' state");
            state.Actions[0].Enabled = false;
            state.InsertAction(0,
                new CallbackAction(() =>
                    MasterAudio.PlaySound3DAndForget("StatusD", carHit.transform, false, 1, null, 0, "Hit")
                )
            );
        }

        JamsTweaksAndFixes.Log("Patched cop CarHit fsm");
    }
}
