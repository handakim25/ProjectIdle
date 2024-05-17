using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Gust.UI.Option
{
    // Slider의 값을 변경 시키고 Slider의 OnValueChanged를 이용해서 다른 UI를 변경한다.
    // 하나의 기준값은 Slider의 value이다.

    public class OptionSliderButtonWithText : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private CustomButton _minusButton;
        [SerializeField] private CustomButton _plusButton;
        [SerializeField] private TextMeshProUGUI _valueText;
        [Tooltip("Plus, Minus 버튼을 눌렀을 때 변하는 값")]
        [SerializeField] private int _tickValue = 1;

        /// <summary>
        /// Called when the value of the slider changes. Param : slider value
        /// </summary>
        public UnityEvent<float> onValueChanged;

        private void Awake()
        {
            if(_slider != null)
            {
                _slider.onValueChanged.AddListener(OnValueChagned);
                UpdateValueText(_slider.value);
            }
            if(_minusButton != null)
            {
                _minusButton.PointerClickEvent.AddListener(() => _slider.value -= _tickValue);
                _minusButton.PointerPressEvent.AddListener(() => _slider.value -= _tickValue);
            }
            if(_plusButton != null)
            {
                _plusButton.PointerClickEvent.AddListener(() => _slider.value += _tickValue);
                _plusButton.PointerPressEvent.AddListener(() => _slider.value += _tickValue);
            }
        }

        // @Memo
        // Slider를 기준으로 설정할 것
        // Slider의 값을 변경하면 Slider OnValueChanged가 호출되서 다른 GUI를 변경

        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }

        public string ValueText
        {
            get
            {
                return _valueText != null ? _valueText.text : string.Empty;
            }
        }

        private void UpdateValueText(float value)
        {
            if(_valueText != null)
            {
                // float의 소수점 이하를 버리는 형식은 d가 아니라 0이다.
                _valueText.text = value.ToString("0");
            }
        }

        private void OnValueChagned(float value)
        {
            // Update Value Text
            UpdateValueText(value);

            // Notiryf
            onValueChanged?.Invoke(value);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 자식 오브젝트에서 Slider, Text, Button을 찾아서 할당
        /// </summary>
        [ContextMenu("FindUIElementsInChildren")]
        public void FindUIElementsInChildren()
        {
            _slider = GetComponentInChildren<Slider>();
            _valueText = GetComponentInChildren<TextMeshProUGUI>();
            GetComponentsInChildren<CustomButton>().ToList().ForEach(button =>
            {
                switch (button.name.ToLower())
                {
                    case var buttonName when buttonName.Contains("minus"):
                        _minusButton = button;
                        break;
                    case var buttonName when buttonName.Contains("plus"):
                        _plusButton = button;
                        break;
                }
            });
        }
#endif
    }
}
