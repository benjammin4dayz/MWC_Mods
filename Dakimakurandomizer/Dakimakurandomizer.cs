using MSCLoader;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Dakimakurandomizer;

public class Dakimakurandomizer : Mod
{
    public override string ID => "Dakimakurandomizer";
    public override string Name => "Dakimakurandomizer";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.0.0";
    public override string Description => "An add-on for BodyPillow that randomly selects a cover image from your collection.";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
        SetupFunction(Setup.PreLoad, Mod_PreLoad);
        SetupFunction(Setup.ModSettings, Mod_Settings);
    }

    private const string BASE_MOD_ID = "BodyPillow";
    private const string BASE_MOD_URL = "https://www.nexusmods.com/mywintercar/mods/1925";
    private static readonly string[] s_extensions = [".jpg", ".jpeg", ".png"];

    private SettingsCheckBox _enabled;

    private string _baseAssetsPath;
    private string _randomizerAssetsPath;

    private string[] _shuffledAssets;
    private int _currentIndex = 0;

    private void Mod_Settings()
    {
        _enabled = Settings.AddCheckBox("enable", "Enable Randomizer", true);
        _ = Settings.AddText(string.Empty);

        _ = Settings.AddButton("Open Assets Folder",
            () =>
            {
                if (!Directory.Exists(_randomizerAssetsPath)) { return; }
                Application.OpenURL(_randomizerAssetsPath);
            },
            SettingsButton.ButtonIcon.Folder);

        _ = Settings.AddButton(
            $"{BASE_MOD_ID} on Nexus Mods",
            () => Application.OpenURL(BASE_MOD_URL),
            SettingsButton.ButtonIcon.NexusMods);
    }

    private void Mod_OnMenuLoad()
    {
        if (!ModLoader.IsModPresent(BASE_MOD_ID) || !_enabled.GetValue()) { return; }
        if (string.IsNullOrEmpty(_randomizerAssetsPath))
        {
            var baseMod = ModLoader.LoadedMods.Find(m => m.Name.Equals(BASE_MOD_ID));
            _baseAssetsPath = ModLoader.GetModAssetsFolder(baseMod);
            _randomizerAssetsPath = Path.Combine(_baseAssetsPath, Name);
        }

        if (!Directory.Exists(_baseAssetsPath))
        {
            ModConsole.Error($"[{Name}]: '{BASE_MOD_ID}' assets folder not found. Did you install the mod properly?");
            return;
        }

        if (!Directory.Exists(_randomizerAssetsPath))
        {
            _ = Directory.CreateDirectory(_randomizerAssetsPath);
            ModUI.ShowCustomMessage(
                $"Place all your <b>{BASE_MOD_ID}</b> cover images here:\n\n<b>{_randomizerAssetsPath}</b>\n\n\nOpen this folder now?",
                Name,
                [ModUI.CreateMessageBoxBtn("YES", () => Application.OpenURL(_randomizerAssetsPath)), ModUI.CreateMessageBoxBtn("NO")]
            );
        }
    }
    private void Mod_PreLoad()
    {
        if (!ModLoader.IsModPresent(BASE_MOD_ID) || !_enabled.GetValue()) { return; }
        if (!Directory.Exists(_randomizerAssetsPath)) { return; }

        var assets = Directory.GetFiles(_randomizerAssetsPath)
            .Where(f => s_extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
            .ToArray();
        if (assets.Length == 0) { return; }

        var oldCoverImages = Directory.GetFiles(_baseAssetsPath)
            .Where(f =>
                s_extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase) &&
                string.Equals(Path.GetFileNameWithoutExtension(f), "cover", StringComparison.OrdinalIgnoreCase)
            );
        foreach (var image in oldCoverImages)
        {
            try
            {
                File.Delete(image);
            }
            catch { }
        }

        if (_shuffledAssets == null || _shuffledAssets.Length != assets.Length)
        {
            _shuffledAssets = [.. assets.OrderBy(a => UnityEngine.Random.value)];
            _currentIndex = 0;
        }

        var asset = _shuffledAssets[_currentIndex];
        _currentIndex = (_currentIndex + 1) % _shuffledAssets.Length;

        var ext = Path.GetExtension(asset);
        File.Copy(asset, Path.Combine(_baseAssetsPath, "cover" + ext), true);
    }
}
