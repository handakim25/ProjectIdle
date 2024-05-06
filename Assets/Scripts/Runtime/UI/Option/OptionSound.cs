using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Gust.Audio;

namespace Gust.UI.Option
{
    /// <summary>
    /// Option Menu에서 Sound 설정을 담당하는 Class
    /// 활성화 될 때 Sound 설정값을 읽어오고 비활성화 될 때 저장한다.
    /// Sound 변경은 Slider의 Value가 변경될 때 이루어진다.
    /// </summary>
    public class OptionSound : MonoBehaviour
    {
        // Slider : [0, 100], Volume : [0.0f, 1.0f]
        [SerializeField] private OptionSliderButtonWithText _masterVolumeSlider;
        [SerializeField] private OptionSliderButtonWithText _bgmVolumeSlider;
        [SerializeField] private OptionSliderButtonWithText _sfxVolumeSlider;
        [SerializeField] private OptionSliderButtonWithText _uiVolumeSlider;

        [SerializeField] private string _masterVolumeCheckSound;

        private GameSetting _setting;

        private void Start()
        {
            _setting = GameManager.Instance.GameSetting;

            if(_masterVolumeSlider != null)
            {
                _masterVolumeSlider.onValueChanged.AddListener(OnMaterVolumeChanged);
                _masterVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.MasterVolume);
            }
            
            if(_bgmVolumeSlider != null)
            {
                _bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
                _bgmVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.BGMVolume);
            }

            if(_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                _sfxVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.SFXVolume);
            }

            if(_uiVolumeSlider != null)
            {
                _uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
                _uiVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.UIVolume);
            }
        }

        private void OnEnable()
        {
            if(_setting == null)
            {
                return;
            }

            if(_masterVolumeSlider != null)
            {
                _masterVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.MasterVolume);
            }
            if(_bgmVolumeSlider != null)
            {
                _bgmVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.BGMVolume);
            }
            if(_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.SFXVolume);
            }
            if(_uiVolumeSlider != null)
            {
                _uiVolumeSlider.Value = SoundUtility.RatioToPercentage(_setting.UIVolume);
            }
        }

        private void OnDisable()
        {
            GameManager.Instance.SaveGameSetting();
        }

        // Slider는 [0,100], Volume은 [0.0f, 1.0f]로 변환하여 저장

        private void OnMaterVolumeChanged(float value)
        {
            _setting.MasterVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.MasterVolume = _setting.MasterVolume;
        }

        private void OnBGMVolumeChanged(float value)
        {
            _setting.BGMVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.BGMVolume = _setting.BGMVolume;
        }

        private void OnSFXVolumeChanged(float value)
        {
            _setting.SFXVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.SFXVolume = _setting.SFXVolume;
        }

        private void OnUIVolumeChanged(float value)
        {
            _setting.UIVolume = SoundUtility.PercentageToRatio(value);
            SoundManager.Instance.UIVolume = _setting.UIVolume;
        }
    }
}
