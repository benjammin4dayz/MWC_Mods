using MSCLoader;
using System;
using UnityEngine;

namespace JamsTweaksAndFixes;

internal static class BetterApartmentFix
{
    private const string MOD_ID = "BetterApartment";
    private const string PROBLEM_VERSION = "1.1";

    public static SettingsCheckBox AddSetting(Action onChange)
    {
        var setting = Settings.AddCheckBox("fix_better_apartment_table", "Hide BetterApartment table", false, onChange);
        _ = Settings.AddText($"Disables the overlapping phone table in {MOD_ID} v{PROBLEM_VERSION}");
        return setting;
    }

    public static void Apply()
    {
        if (!ModLoader.IsModPresent(MOD_ID)) { return; }
        if (ModLoader.GetModVersionByID(MOD_ID) != PROBLEM_VERSION)
        {
            JamsTweaksAndFixes.Log($"Refusing to patch {MOD_ID} because it was updated");
            return;
        }
        var table = GameObject.Find("BETTER_APARTMENT(Clone)")
            ?.transform.Find("APARTMENT/HALLWAY/table")
            ?.gameObject
            ?? throw new Exception("Missing BetterApartment table");
        table.SetActive(false);

        JamsTweaksAndFixes.Log("Patched BetterApartment table");
    }
}
