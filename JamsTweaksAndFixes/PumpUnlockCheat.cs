using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace JamsTweaksAndFixes;

internal static class PumpUnlockCheat
{
    private sealed class PumpUnlocker : MonoBehaviour
    {
        public static readonly string s_pump1 = "PERAPORTTI/Building/LOD300/FuelPumps_1-2/Pump1";
        public static readonly string s_pump2 = "PERAPORTTI/Building/LOD300/FuelPumps_1-2/Pump2";

        private readonly FsmFloat _cashReserve;
        private readonly FsmBool _lockedDiesel;
        private readonly FsmBool _lockedFuelOil;
        private readonly FsmBool _lockedGasoline;

        public PumpUnlocker()
        {
            var functions = gameObject.GetPlayMaker("Functions")?.Fsm
                ?? throw new Exception("Missing gas pump functions");

            _cashReserve = functions.GetFsmFloat("CashReserve");
            _lockedDiesel = functions.GetFsmBool("LockedDiesel");
            _lockedFuelOil = functions.GetFsmBool("LockedFuelOil");
            _lockedGasoline = functions.GetFsmBool("LockedGasoline");
        }

        [SuppressMessage("Called by Unity", "IDE0051")]
        private void Update()
        {
            _cashReserve.Value = 999f;
            _lockedDiesel.Value = false;
            _lockedFuelOil.Value = false;
            _lockedGasoline.Value = false;
        }
    }

    public static SettingsCheckBox AddSetting(Action onChange)
    {
        var setting = Settings.AddCheckBox("cheat_pump_unlock", "Unlock gas pumps", false, onChange);
        _ = Settings.AddText($"Makes gas pumps completely free to use.");
        return setting;
    }

    public static void Apply()
    {
        _ = GameObject.Find(PumpUnlocker.s_pump1).AddComponent<PumpUnlocker>();
        _ = GameObject.Find(PumpUnlocker.s_pump2).AddComponent<PumpUnlocker>();
        JamsTweaksAndFixes.Log("Activated Pump Unlock cheat");
    }
}
