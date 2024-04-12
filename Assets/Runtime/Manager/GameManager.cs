using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Idle.Utility;

namespace Idle
{
    public sealed class GameManager : MonoSingleton<GameManager>, IManager
    {
        public bool IsInit => Progress >= 1.0f;

        public float Progress => 1.0f;
    }
}
