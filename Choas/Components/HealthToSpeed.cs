using CustomPlayerEffects;
using Mirror;
using LabApi;
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
        public LabApi.Features.Wrappers.Player plr;
        private void Update()
        {
            if (!NetworkServer.active || plr == null || !plr.IsAlive || plr.Health == lastHP) return;
            int sVal = Mathf.RoundToInt(Mathf.Lerp(StartValue, EndValue, 1 - plr.Health / plr.MaxHealth));
            if (sVal < 0)
            {
                plr.DisableEffect<MovementBoost>();
                plr.GetEffect<Slowness>().ForceIntensity((byte)Mathf.Abs(sVal));
            }
            else
            {
                plr.GetEffect<MovementBoost>().ForceIntensity((byte)Mathf.Abs(sVal));
                plr.DisableEffect<Slowness>();
            }
        }
    }
}
