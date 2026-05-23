using MSCLoader;
using System;

namespace JamsTweaksAndFixes;

public class JamsTweaksAndFixes : Mod
{
    public override string ID => "JamsTweaksAndFixes";
    public override string Name => "Jam's Tweaks & Fixes";
    public override string Author => "benjammin4dayz";
    public override string Version => "0.1.0";
    public override string Description => "Assorted tweaks and fixes.";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
        SetupFunction(Setup.ModSettings, Mod_Settings);
        SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
    }

    internal static void Log(string msg) => ModConsole.Log($"<b>[JamsTweaksAndFixes]</b> {msg}");

    private bool _isReloadWarned;
    private SettingsCheckBox _sofaFix;
    private SettingsCheckBox _betterApartmentFix;

    private void Mod_Settings()
    {
        _ = Settings.AddHeader("Vanilla Fixes");
        _sofaFix = SofaFix.AddSetting(WarnReloadOnce);

        _ = Settings.AddHeader("Mod Fixes");
        _betterApartmentFix = BetterApartmentFix.AddSetting(WarnReloadOnce);
    }
    private void Mod_SettingsLoaded()
    {
        LoadSavedSetting(_sofaFix);
        LoadSavedSetting(_betterApartmentFix);
    }

    private void Mod_OnLoad()
    {
        TryApply(_sofaFix, SofaFix.Apply);
        TryApply(_betterApartmentFix, BetterApartmentFix.Apply);
    }

    private void TryApply(SettingsCheckBox checkbox, Action Apply)
    {
        if (!checkbox.GetValue()) { return; }

        try
        {
            Apply.Invoke();
        }
        catch (Exception ex)
        {
            Log($"<color=red>{ex.Message}</color>");
        }
    }

    private void WarnReloadOnce()
    {
        if (_isReloadWarned || ModLoader.CurrentScene is CurrentScene.MainMenu) { return; }
        _isReloadWarned = true;
        ModUI.ShowMessage("You must reload for the changes to take effect.");
    }

    private static void LoadSavedSetting(SettingsCheckBox checkbox) =>
        checkbox.SetValue(checkbox.GetValue());
}
