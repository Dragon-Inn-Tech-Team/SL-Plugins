using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using Mirror;
using NVorbis;
using PlayerStatsSystem;
using RedRightHand;
using UnityEngine;
using Utils;

namespace Choas.Features.Jackenstein;

// Holy shit this is a mess

public class YOUR_TAKING_TOO_LONG : NetworkBehaviour
{
    public static readonly string le_pumpkin = "<line-height=0.75><cspace=-0.17><size=1><color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆\\n▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#00000000>▆▆▆▆▆\\n▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#00000000>▆▆▆▆\\n▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#ffffffff>▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆<color=#ffffffff>▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#ffffffff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#ffffffff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆<color=#ffffffff>▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆<color=#ed1c24ff>▆<color=#000000ff>▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆<color=#ffffffff>▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆<color=#ffffffff>▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆▆<color=#000000ff>▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ffffffff>▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#ffffffff>▆▆▆▆<color=#000000ff>▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆<color=#000000ff>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆\\n<color=#00000000>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆\\n<color=#00000000>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆\\n<color=#00000000>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆\\n▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆\\n▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆\\n▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆\\n▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆\\n▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆\\n▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆\\n▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆\\n▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆\\n▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆\\n▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆\\n▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆\\n▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆\\n▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆\\n▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#00000000>▆▆▆▆▆\\n▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#00000000>▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#00000000>▆▆▆<color=#ed1c24ff>▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆<color=#000000ff>▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆<color=#000000ff>▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆<color=#ed1c24ff>▆▆▆▆▆▆▆<color=#00000000>▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆▆\\n";
    public static YOUR_TAKING_TOO_LONG CreateYOUR_TAKING_TOO_LONG(Vector3 position, Quaternion rotation, bool startHidden = true, byte speakerControllerID = 69)
    {
        var pump = TextToy.Create(position, rotation, null, false);
        pump.TextFormat = le_pumpkin;
        if (startHidden)
        {
            pump.Scale = Vector3.zero;
        }
        pump.Spawn();
        
        var speaker = SpeakerToy.Create(pump.Transform);
        speaker.ControllerId = speakerControllerID;
        speaker.MinDistance = 5f;
        speaker.MaxDistance = 75f;
        speaker.IsSpatial = true;
        
        var yttl = pump.GameObject.AddComponent<YOUR_TAKING_TOO_LONG>();
        yttl.rb = pump.GameObject.AddComponent<Rigidbody>();
        yttl.rb.useGravity = false;
        yttl.rb.centerOfMass = Vector3.down;
        yttl.rb.linearDamping = 1f;
        yttl.rb.maxLinearVelocity = 50f;
        _ = pump.GameObject.AddComponent<NetworkRigidbodyUnreliable>();
        
        yttl.Pumpkin = pump;
        yttl.Speaker = speaker;
        
        return yttl;
    }

    public float[] explosionSfx;
    public Rigidbody rb;
    public SpeakerToy Speaker;
    public TextToy Pumpkin;
    public bool chaseMode = false, speaking = false, laughing = false;
    public List<Player> Targets = new List<Player>();
    public List<Player> Victims = new List<Player>();
    public float ChaseDuration = 20f;
    public LightsController? ChaseRoomLights = null;
    public bool LethalMode = false;

    public void StartChase(List<Player> targets, float duration = 20f, bool lethal = false)
    {
        Targets = targets;
        target = Targets.First();
        ChaseDuration = duration;
        LethalMode = lethal;
        Appear();
    }
    
    public void Appear()
    {
        Pumpkin.Scale = Vector3.zero;
        Pumpkin.Rotation = Quaternion.LookRotation((Targets.First().Position - Pumpkin.Position).NormalizeIgnoreY());

        Room? room = Targets.First().Room;
        if (room != null)
        {
            ChaseRoomLights = room.LightController;
        }
        
        if (ChaseRoomLights != null) ChaseRoomLights.OverrideLightsColor = new Color(0.1f, 0.1f, 0.1f);
        
        var reader = new NVorbis.VorbisReader(ChoasPlugin.Config.AudioClipFolderPath + "YOUR_TAKING_TOO_LONG.ogg");
        float[] buffer1 = new float[reader.TotalSamples];
        reader.ReadSamples(buffer1, 0, buffer1.Length);
        reader = new VorbisReader(ChoasPlugin.Config.AudioClipFolderPath + "EVIL_LAUGH.ogg");
        float[] buffer2 = new float[reader.TotalSamples];
        reader.ReadSamples(buffer2, 0, buffer2.Length);
        reader = new NVorbis.VorbisReader(ChoasPlugin.Config.AudioClipFolderPath + "deltaruneExplosion.ogg");
        explosionSfx = new float[reader.TotalSamples];
        reader.ReadSamples(explosionSfx, 0, explosionSfx.Length);
        
        MEC.Timing.CallDelayed(0.2f, () => { Pumpkin.Scale = Vector3.one; });
        MEC.Timing.CallDelayed(0.3f, () =>
        {
            Speaker.Play(buffer1, false); 
            Speaker.Play(buffer2); 
            chaseMode = true; 
            speaking = true;
        });

    }

    public void StopChase()
    {
        chaseMode = false;
        rb.useGravity = true;
        if (ChaseRoomLights != null) ChaseRoomLights.OverrideLightsColor = Color.clear;

        MEC.Timing.CallDelayed(3f, () =>
        {
            foreach (var player in Victims)
            {
                player.DisableEffect<Ensnared>();
                player.DisableEffect<Blindness>();
                player.DisableEffect<Deafened>();
                player.DisableEffect<Fade>();
            }
            NetworkServer.Destroy(Pumpkin.GameObject);
        });
    }
    
    private float chaseTime = 0f;
    private Player target;
    void Update()
    {
        if (!chaseMode) return;
        if (speaking)
        {
            if (Speaker.QueuedClipsCount == 0)
            {
                speaking = false;
                laughing = true;
                LightsController? roomLights = null;
                if (Room.TryGetRoomAtPosition(Targets.First().Position, out var room))
                {
                    roomLights = room.LightController;
                }
        
                if (roomLights != null) roomLights.OverrideLightsColor = new Color(0.3f, 0.08f, 0.08f);
            }

            return;
        }
        

        if (chaseTime >= ChaseDuration || Targets.Count == 0)
            StopChase();
        
        var lookVector = target.Position - (Pumpkin.Position + rb.centerOfMass);
        var targetVector = lookVector + target.Velocity - rb.linearVelocity;
        
        if (lookVector.sqrMagnitude < 1.5f)
        {
            if (LethalMode)
            {
                ExplosionUtils.ServerExplode(target.ReferenceHub, ExplosionType.PinkCandy);
            }
            else
            {
                target.EnableEffect<Flashed>(1, 1f, true);
                target.EnableEffect<Ensnared>(1, ChaseDuration - chaseTime);
                target.EnableEffect<Blindness>(1, ChaseDuration - chaseTime);
                target.EnableEffect<Deafened>(1, ChaseDuration - chaseTime);
                target.EnableEffect<Fade>(200, ChaseDuration - chaseTime);
            }

            Speaker.Play(explosionSfx, false);
            
            Victims.Add(target);
            Targets.Remove(target);
            
            if (Targets.Count == 0)
                StopChase();
            
            target = Targets.First();
            var smallestDist = (target.Position - (Pumpkin.Position + rb.centerOfMass)).sqrMagnitude;
            for (int i = Targets.Count - 1; i >= 0; i--)
            {
                var plr = Targets.ElementAt(i);
                if (!plr.IsAlive)
                {
                    Targets.RemoveAt(i);
                    continue;
                }
                var currDist = (plr.Position - (Pumpkin.Position + rb.centerOfMass)).sqrMagnitude;
                if (currDist < smallestDist)
                {
                    smallestDist = currDist;
                    target = plr;
                }
            }
        }
        
        Pumpkin.Rotation = Quaternion.LookRotation(lookVector.NormalizeIgnoreY());
        
        if (Vector3.Dot(targetVector, lookVector) < 0)
            targetVector = Vector3.Reflect(targetVector, lookVector);
        
        rb.AddForce(targetVector.normalized * (Time.deltaTime * 20f), ForceMode.VelocityChange);
        
        chaseTime += Time.deltaTime;
    }
}