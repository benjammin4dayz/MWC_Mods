using MSCLoader;
using PlayerCore;
using UnityEngine;

namespace CheckYourWatch;

public class CheckYourWatch : Mod
{
    private readonly bool _isWinter = ModLoader.CurrentGame is Game.MyWinterCar;
    private Player _player;
    private SettingsManager _settings;
    private GameObject _crosshair;
    private GameObject _hud;

    public override string ID => "CheckYourWatch";
    public override string Name => "Check Your Watch";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.2.0";
    public override string Description => "Hides player stats and reveals them when checking your watch.";
    public override Game SupportedGames => Game.MySummerCar_And_MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
        SetupFunction(Setup.Update, Mod_Update);
        SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
        SetupFunction(Setup.ModSettings, Mod_Settings);
    }

    private void Mod_SettingsLoaded() => _settings.OnLoad();
    private void Mod_Settings() => _settings = new();

    private void Mod_OnLoad()
    {
        _player = new();

        var gui = GameObject.Find("GUI").transform;
        _crosshair = gui.Find("Crosshair").gameObject;
        _hud = gui.Find("HUD").gameObject;
    }
    private void Mod_Update()
    {
        _crosshair.SetActive(_settings.IsCrosshairEnabled);

        var isStatCritical = _settings.IsHudShownWhenCritical &&
            (
                _player.Thirst >= _settings.HudCriticalValue ||
                _player.Hunger >= _settings.HudCriticalValue ||
                _player.Stress >= _settings.HudCriticalValue ||
                _player.Urine >= _settings.HudCriticalValue ||
                _player.Fatigue >= _settings.HudCriticalValue
            );

        var isFreezing = _isWinter && _player.Temp <= 0;

        _hud.SetActive(cInput.GetButton("Watch") || isStatCritical || isFreezing);
    }
}
