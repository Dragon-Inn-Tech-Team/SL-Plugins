using CustomPlayerEffects;
using Mirror;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Choas.Components
{
    public class HealthToSpeed : MonoBehaviour
    {
        private float lastHP = -1f;
        public short StartValue = -25;
        public short EndValue = 75;
        public Player plr;
        private void Update()
        {
            if (!NetworkServer.active || plr == null || !plr.IsAlive || plr.Health == lastHP) return;
            int sVal = Mathf.RoundToInt(Mathf.Lerp(StartValue, EndValue, 1 - plr.Health / plr.MaxHealth));
            if (sVal < 0)
            {
                plr.EffectsManager.DisableEffect<MovementBoost>();
                plr.EffectsManager.ChangeState<Slowness>((byte)Mathf.Abs(sVal));
            }
            else
            {
                plr.EffectsManager.ChangeState<MovementBoost>((byte)Mathf.Abs(sVal));
                plr.EffectsManager.DisableEffect<Slowness>();
            }
        }
    }
}
