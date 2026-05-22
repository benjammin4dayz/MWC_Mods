using MSCLoader;
using System;
using UnityEngine;

namespace PSKBrawl;

public class PSKBrawl : Mod
{
    public override string ID => "PSKBrawl";
    public override string Name => "PSK Brawl";
    public override string Author => "benjammin4dayz";
    public override string Version => "1.0.0";
    public override string Description => "Adds the ability to bonk some staff and a customer at PSK.";
    public override Game SupportedGames => Game.MyWinterCar;

    public override void ModSetup()
    {
        SetupFunction(Setup.OnLoad, Mod_OnLoad);
    }

    private void Mod_OnLoad()
    {
        try
        {
            PSKBrawlBehaviour.Attach();
        }
        catch (Exception ex)
        {
            ModConsole.Error($"[PSKBrawl]: Failed to attach behaviour: {ex.Message}");
        }
    }

    private class PSKBrawlBehaviour : MonoBehaviour
    {
        internal static void Attach()
        {
            var root = GameObject.Find("PERAPORTTI").transform.Find("Building/LOD100");
            root.gameObject.AddComponent<PSKBrawlBehaviour>();
        }

        // phone guy
        private Transform _sarkain;
        // burger guy
        private Transform _keijo;
        // manager
        private Transform _jouni;

        private void OnEnable()
        {
            _sarkain = transform.Find("Customers/CustomerSarkainPIVOT/TeppoSarkain/PhysicsPivot/Pivot/HumanTriggerCrime");
            _sarkain?.gameObject.SetActive(true);
            _keijo = transform.Find("Staff/BurgerRunnerPIVOT/Keijo/PhysicsPivot/Pivot/HumanTriggerCrime");
            _keijo?.gameObject.SetActive(true);
            _jouni = transform.Find("Staff/AlaCarteRunnerPIVOT/Jouni/PhysicsPivot/Pivot/HumanTriggerCrime");
            _jouni?.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _sarkain = null;
            _keijo = null;
            _jouni = null;
        }
    }
}
