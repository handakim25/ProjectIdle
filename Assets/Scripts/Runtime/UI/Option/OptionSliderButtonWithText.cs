using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace Gust.UI.Option
{
    public class OptionSliderButtonWithText : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private CustomButton _minusButton;
        [SerializeField] private CustomButton _plusButton;
        [SerializeField] private TextMeshProUGUI _valueText;
        [Tooltip("Plus, Minus 버튼을 눌렀을 때 변하는 값")]
        [SerializeField] private int _tickValue = 1;

        public UnityEvent<float> onValueChanged;

        public float Value
        {
            get => _slider.value;
            set => SetValue(value);
        }

        private void Awake()
        {
            _minusButton.PointerClickEvent.AddListener(() => OnButtonClicked(- _tickValue));
            _plusButton.PointerClickEvent.AddListener(() => OnButtonClicked(_tickValue));
            _slider.onValueChanged.AddListener((value) => Debug.Log($"Slider Value Changed : {value}"));
        }

        private void OnButtonClicked(float value)
        {
            SetValue(_slider.value + value);
        }

        public void SetValue(float value)
        {
            _slider.value = value;
            _valueText.text = _slider.value.ToString(); // Slider에서 값 제한이 있으므로 해당 값 제한을 이용해서 Text 설정
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
