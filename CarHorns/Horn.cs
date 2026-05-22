using HutongGames.PlayMaker;
using MSCLoader;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CarHorns;

internal class Horn : MonoBehaviour
{
    private FsmBool _guiUse;
    private bool _isInteractionVisible;

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
        _guiUse = FsmVariables.GlobalVariables.GetFsmBool("GUIuse");
    }

    [SuppressMessage("Called by Unity", "IDE0051")]
    private void Update()
    {
        var hit = UnifiedRaycast.GetHitInteraction(trigger);

        if (hit)
        {
            SetInteractionVisible(true);

            if (!audio.activeSelf && Input.GetMouseButtonDown(0))
            {
                audio.SetActive(true);
            }
        }
        else if (!hit && _isInteractionVisible)
        {
            SetInteractionVisible(false);
        }

        if (audio.activeSelf && Input.GetMouseButtonUp(0))
        {
            audio.SetActive(false);
        }
    }
}
