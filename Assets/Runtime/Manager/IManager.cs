using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust
{
    public interface IManager
    {
        /// <summary>
        /// Manager의 초기화가 완료되었는지 여부
        /// </summary>
        public bool IsInit { get; }
        /// <summary>
        /// Manager의 초기화 진행도. 0.0f ~ 1.0f
        /// </summary>
        public float Progress { get; }
    }
}
