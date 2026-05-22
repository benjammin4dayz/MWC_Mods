using MSCLoader;

namespace CheckYourWatch;

internal class SettingsManager
{
    private readonly SettingsCheckBox _showCrosshair;
    private readonly SettingsCheckBox _showHudOnCriticalValue;
    private readonly SettingsSlider _hudCriticalValue;

    public bool IsCrosshairEnabled { get; private set; }
    public bool IsHudShownWhenCritical { get; private set; }
    public float HudCriticalValue { get; private set; }

    public SettingsManager()
    {
        _showCrosshair = Settings.AddCheckBox("crosshair_enabled", "Show crosshair", true, OnToggleCrosshair);
        _showHudOnCriticalValue = Settings.AddCheckBox("critical_hud_enabled", "Show HUD when stat is critical", true, OnToggleCriticalHud);
        _hudCriticalValue = Settings.AddSlider("critical_hud_threshold", "Critical threshold", 100f, 195f, 100f, OnUpdateCriticalValue, 0, false);
    }

    public void OnLoad()
    {
        OnToggleCrosshair();
        OnToggleCriticalHud();
        OnUpdateCriticalValue();
    }

    private void OnToggleCrosshair() => IsCrosshairEnabled = _showCrosshair.GetValue();
    private void OnToggleCriticalHud()
    {
        var state = _showHudOnCriticalValue.GetValue();
        IsHudShownWhenCritical = state;
        _hudCriticalValue.SetVisibility(state);
    }
    private void OnUpdateCriticalValue() => HudCriticalValue = _hudCriticalValue.GetValue();
}
