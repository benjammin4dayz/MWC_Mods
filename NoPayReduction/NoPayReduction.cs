using MSCLoader;
using System;
using System.Text;
using UnityEngine;

namespace NoPayReduction;

public class NoPayReduction : Mod
{
    public override string ID => "NoPayReduction";
    public override string Name => "No Pay Reduction";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.0.0";
    public override string Description => "Disables time-based pay reduction for Sewage and Firewood jobs";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
    }

    private void Mod_OnLoad()
    {
        ModConsole.Log("<b>[NoPayReduction]</b> Disabling pay reduction for Sewage and Firewood jobs");

        var jobs = GameObject.Find("JOBS")
            ?? throw new Exception("Missing JOBS");

        var missing = new StringBuilder();

        for (var i = 1; i <= 5; i++)
        {
            var path = $"HouseShit{i}/WasteWell_2000litre/Shit/Level/ShitLevelTrigger";
            var level = jobs.transform.Find(path)?.GetPlayMaker("Level");
            if (!level)
            {
                missing.AppendLine(path);
                continue;
            }
            level.FsmVariables.GetFsmFloat("Penalty").Value = 0;
            level.FsmVariables.GetFsmFloat("PenaltyRate").Value = 0;
        }

        for (var i = 1; i <= 4; i++)
        {
            var path = $"HouseWood{i}/WoodJob{i}Point";
            var logic = jobs.transform.Find(path)?.GetPlayMaker("Logic");
            if (!logic)
            {
                missing.AppendLine(path);
                continue;
            }
            logic.FsmVariables.GetFsmFloat("Penalty").Value = 0;
            logic.FsmVariables.GetFsmFloat("PenaltyRate").Value = 0;
        }

        if (missing.Length > 0)
        {
            ModConsole.Warning($"<b>[NoPayReduction]</b> Finished with errors");
            Debug.Log(missing.ToString());
        }
    }
}
