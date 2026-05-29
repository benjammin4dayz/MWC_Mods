using HutongGames.PlayMaker;
using System;

namespace JamsTweaksAndFixes;

internal static class Helpers
{
    public class CallbackAction(Action callback) : FsmStateAction
    {
        public override void OnEnter()
        {
            callback?.Invoke();
            Finish();
        }
    }
}
