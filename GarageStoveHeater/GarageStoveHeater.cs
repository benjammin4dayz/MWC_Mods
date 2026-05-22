using HutongGames.PlayMaker;
using MSCLoader;
using System;
using UnityEngine;

namespace GarageStoveHeater;

public class GarageStoveHeater : Mod
{
    public override string ID => "GarageStoveHeater";
    public override string Name => "Garage Stove Heater";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.0.0";
    public override string Description => "Adds a functional wood stove to the parents' garage.";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
        SetupFunction(Setup.Update, Mod_Update);
    }

    private const float HEAT_EFFICIENCY = 0.18f;
    private FsmFloat _ambientTemp;
    private FsmFloat _heatSourceTemp;
    private Transform _setFire;

    private void Mod_OnLoad()
    {
        var stoveRef = GameObject.Find("CABIN/Cabin/woodstove")
            ?? throw new Exception("Missing woodstove");
        var garage = GameObject.Find("YARD/Building/Garage")
            ?? throw new Exception("Missing garage");
        var noRainGarage = GameObject.Find("YARD/Building/NoRainGarage")
            ?? throw new Exception("Missing NoRainGarage");
        _ambientTemp = noRainGarage.GetPlayMaker("Data")
            ?.FsmVariables.FindFsmFloat("Temp")
            ?? throw new Exception("Missing garage temperature");

        var stove = GameObject.Instantiate(stoveRef).transform;
        stove.name = "woodstove";
        stove.SetParent(garage.transform);
        stove.position = new(-14.57041f, -0.1688232f, -2.284346f);
        stove.eulerAngles = new(270f, 0f, 0f);
        stove.localScale = new(-1f, 1f, 1f);

        var stoveHandle = stove.Find("HatchPivot/woodstove_hatch/Handle")
            ?? throw new Exception("Missing woodstove handle");
        stoveHandle.gameObject.SetActive(true);
        _setFire = stove.Find("Fireplace/SetFire")
            ?? throw new Exception("Missing fireplace");

        var logic = _setFire.GetPlayMaker("Use")
            ?? throw new Exception("Missing stove heat logic");
        logic.FsmVariables.GetFsmFloat("HeatingEfficiency").Value = HEAT_EFFICIENCY;
        logic.FsmVariables.GetFsmGameObject("NoRain1").Value = noRainGarage;

        var heatSource = new GameObject("HeatSourceGarageStove");
        var heatFsm = heatSource.AddComponent<PlayMakerFSM>();
        heatFsm.FsmName = "Data";
        _heatSourceTemp = new("Temperature") { Value = _ambientTemp.Value };
        heatFsm.FsmVariables.FloatVariables =
        [
            new("Distance") { Value = 1f },
            _heatSourceTemp,
        ];
        heatSource.transform.SetParent(stove, false);
        var heatSources = GameObject.Find("PLAYER/BodyTemp")
            ?.GetArrayListProxy("HeatSources")
            ?? throw new Exception("Missing HeatSources");
        heatSources.arrayList.Add(heatSource);
    }

    private void Mod_Update()
    {
        var target = _setFire.gameObject.activeSelf
            ? 100f
            : _ambientTemp.Value;

        var speed = (_setFire.gameObject.activeSelf ? 1f : 0.5f)
            * HEAT_EFFICIENCY;

        _heatSourceTemp.Value = Mathf.MoveTowards(
            _heatSourceTemp.Value,
            target,
            speed * Time.deltaTime
        );
    }
}
