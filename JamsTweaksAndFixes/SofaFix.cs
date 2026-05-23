using MSCLoader;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace JamsTweaksAndFixes;

internal static class SofaFix
{
    private sealed class JTFPatch : MonoBehaviour
    {
        [SuppressMessage("Called by Unity", "IDE0051")]
        private void Update()
        {
            if (!CompareTag("PART")) { tag = "PART"; }
        }
    }

    public static SettingsCheckBox AddSetting(Action onChange)
    {
        var setting = Settings.AddCheckBox("fix_sofa", "Sofa", false, onChange);
        _ = Settings.AddText("Makes sofa pickable without needing to sleep on it first.");
        return setting;
    }

    public static void Apply()
    {
        var sofa = GameObject.Find("EQUIPMENTS/sofa(itemx)")
            ?? throw new Exception("Missing sofa");
        _ = sofa.AddComponent<JTFPatch>();

        JamsTweaksAndFixes.Log("Patched sofa(itemx)");
    }
}
