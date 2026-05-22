using HutongGames.PlayMaker;
using MSCLoader;
using System;
using UnityEngine;

namespace CarHorns;

public class CarHorns : Mod
{
    public override string ID => "CarHorns";
    public override string Name => "Car Horns";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.0.0";
    public override string Description => "Adds a horn button to the Corris, Sorbet, and Taxi.";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
        SetupFunction(Setup.Update, Mod_Update);
    }

    private FsmBool _corrisWheelInstalled;
    private Horn _corrisHorn;

    private void Mod_OnLoad()
    {
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
