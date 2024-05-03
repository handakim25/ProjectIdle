using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust
{
    // Dictionary를 사용해도 되지만 일단은 간단하게 구현한다.
    // UI <-> GameSetting <-> Managers
    // Audio 같은 설정은 바로 적용되어야 한다.

    /// <summary>
    /// Game Setting을 저장하기 위한 Class
    /// Instance를 관리하고 이 Instance를 UI에 Display하거나 관련된 Class에서 설정을 사용한다.
    /// </summary>
    public class GameSetting
    {
        // Audio Settings
        /// <summary>
        /// Master Volume. [0.0f, 1.0f]
        /// </summary>
        public float MasterVolume { get; set; } = 1.0f;
        /// <summary>
        /// BGM Volume. [0.0f, 1.0f]
        /// </summary>
        public float BGMVolume { get; set; } = 1.0f;
        /// <summary>
        /// SFX Volume. [0.0f, 1.0f]
        /// </summary>
        public float SFXVolume { get; set; } = 1.0f;
        /// <summary>
        /// UI Volume. [0.0f, 1.0f]
        /// </summary>
        public float UIVolume { get; set; } = 1.0f;
    }

    /// <summary>
    /// Game Setting을 Load하는 Interface
    /// Setting을 이용해서 자신의 값을 설정한다.
    /// </summary>
    public interface IGameSettingLoad
    {
        public void LoadSetting(GameSetting setting);
    }

    public interface IGameSettignSave
    {
        public void SaveSetting(GameSetting setting);
    }

    public interface IGameSettingApply
    {
        public void ApplySetting(GameSetting setting);
    }
}
