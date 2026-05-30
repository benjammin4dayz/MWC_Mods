using HutongGames.PlayMaker;
using MSCLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarHorns;

public class CarHorns : Mod
{
    public static SettingsKeybind Horn { get; private set; }
    public override string ID => "CarHorns";
    public override string Name => "Car Horns";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.1.0";
    public override string Description => "Adds a horn button to the Corris, Sorbet, and Taxi.";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
        SetupFunction(Setup.Update, Mod_Update);
        SetupFunction(Setup.ModSettings, Mod_Settings);
    }

    private FsmBool _corrisWheelInstalled;
    private Horn _corrisHorn;
    private readonly string[] _hornNames = ["Standard", "Dixie"];
    private readonly List<AudioClip> _horns = [null];
    private SettingsDropDownList _corrisHornType;
    private SettingsDropDownList _sorbetHornType;
    private SettingsDropDownList _taxiHornType;

    private void Mod_Settings()
    {
        Horn = Keybind.Add("horn", "Horn", KeyCode.T);

        _ = Settings.AddHeader("Horn Type");
        _corrisHornType = Settings.AddDropDownList("horn_type_corris", "Corris", _hornNames, 0);
        _sorbetHornType = Settings.AddDropDownList("horn_type_sorbet", "Sorbet", _hornNames, 0);
        _taxiHornType = Settings.AddDropDownList("horn_type_taxi", "Taxi", _hornNames, 0);
        _ = Settings.AddText("<b><color=red>Changes will only take effect in the main menu</color></b>");
    }

    private void Mod_OnLoad()
    {
        var ab = LoadAssets.LoadBundle("CarHorns.CarHornsUnity.AssetBundles.horns.unity3d");
        _horns.Add(ab.LoadAsset<AudioClip>("dixie"));
        ab.Unload(false);

        try
        {
            var corris = GameObject.Find("CORRIS")
                ?.transform
                ?? throw new Exception("Missing Corris");
            var corrisWheel = corris.Find("WheelPivot/Steering/SteeringPivotCorris/VINP_SteeringWheel")
                ?? throw new Exception("Missing Corris steering wheel");
            _corrisWheelInstalled = corrisWheel.GetPlayMaker("Data")
                ?.FsmVariables.FindFsmBool("Installed")
                ?? throw new Exception("Missing Corris steering wheel install state");
            var corrisHornAudio = corris.Find("Simulation/CarHorn")
                ?.gameObject
                ?? throw new Exception("Missing Corris horn");

            var customHorn = _horns[_corrisHornType.GetSelectedItemIndex()];
            if (customHorn != null)
            {
                corrisHornAudio.GetComponent<AudioSource>().clip = customHorn;
            }

            _corrisHorn = AddHornButton(corrisWheel, corrisHornAudio);
        }
        catch (Exception ex)
        {
            Log($"Corris setup failed: {ex.Message}");
        }

        try
        {
            var sorbet = GameObject.Find("SORBET(190-200psi)")
                ?.transform
                ?? throw new Exception("Missing Sorbet");
            var sorbetWheel = sorbet.Find("Functions/SteeringColumnPivot/Steering/SteeringPivotSorbett/steering_wheel")
                ?? throw new Exception("Missing Sorbet steering wheel");
            var sorbetHornAudio = sorbet.Find("Simulation/CarHorn")
                ?.gameObject
                ?? throw new Exception("Missing Sorbet horn");

            var customHorn = _horns[_sorbetHornType.GetSelectedItemIndex()];
            if (customHorn != null)
            {
                sorbetHornAudio.GetComponent<AudioSource>().clip = customHorn;
            }

            _ = AddHornButton(sorbetWheel, sorbetHornAudio);
        }
        catch (Exception ex)
        {
            Log($"Sorbet setup failed: {ex.Message}");
        }

        try
        {
            var taxi = GameObject.Find("JOBS")?.transform.Find("TAXIJOB/MACHTWAGEN")
                ?? throw new Exception("Missing taxi");
            var taxiWheel = taxi.Find("LOD/SteeringColumnPivot/Steering/SteeringPivotTaxi/steering_wheel")
                ?? throw new Exception("Missing taxi steering wheel");
            var taxiHornAudio = taxi.Find("Simulation/CarHorn")?.gameObject
                ?? throw new Exception("Missing taxi horn");

            var customHorn = _horns[_taxiHornType.GetSelectedItemIndex()];
            if (customHorn != null)
            {
                taxiHornAudio.GetComponent<AudioSource>().clip = customHorn;
            }

            _ = AddHornButton(taxiWheel, taxiHornAudio);
        }
        catch (Exception ex)
        {
            Log($"Taxi setup failed: {ex.Message}");
        }
    }
    private void Mod_Update()
    {
        _corrisHorn.enabled = _corrisWheelInstalled.Value;
    }

    private Horn AddHornButton(Transform parent, GameObject audio)
    {
        var button =
            new GameObject("ButtonHorn") { layer = LayerMask.NameToLayer("Dashboard") }
            .AddComponent<SphereCollider>();
        button.isTrigger = true;
        button.radius = 0.035f;
        button.transform.SetParent(parent, false);

        var horn = button.gameObject.AddComponent<Horn>();
        horn.audio = audio;
        horn.trigger = button;

        return horn;
    }

    private void Log(string msg) => ModConsole.Log($"<b>[CarHorns]</b> {msg}");
}
