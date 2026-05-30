using HutongGames.PlayMaker;
using MSCLoader;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CarHorns;

internal class Horn : MonoBehaviour
{
    private GameObject _player;
    private FsmBool _guiUse;
    private bool _isInteractionVisible;
    private bool _isClickInteraction;

    public GameObject audio;
    public Collider trigger;

    private void SetInteractionVisible(bool state)
    {
        _isInteractionVisible = state;
        _guiUse.Value = state;
    }

    [SuppressMessage("Called by Unity", "IDE0051")]
    private void Awake()
    {
        _player = GameObject.Find("PLAYER");
        _guiUse = FsmVariables.GlobalVariables.GetFsmBool("GUIuse");
    }

    [SuppressMessage("Called by Unity", "IDE0051")]
    private void Update()
    {
        var hit = UnifiedRaycast.GetHitInteraction(trigger);

        if (hit)
        {
            SetInteractionVisible(true);
        }
        else if (_isInteractionVisible)
        {
            SetInteractionVisible(false);
        }

        if (CarHorns.Horn.GetKeybindDown() && _player.transform.root == transform.root)
        {
            audio.SetActive(true);
        }
        if (CarHorns.Horn.GetKeybindUp())
        {
            audio.SetActive(false);
        }


        if (hit && Input.GetMouseButtonDown(0))
        {
            _isClickInteraction = true;
            audio.SetActive(true);
        }
        if (_isClickInteraction && Input.GetMouseButtonUp(0))
        {
            _isClickInteraction = false;
            audio.SetActive(false);
        }
    }
}
