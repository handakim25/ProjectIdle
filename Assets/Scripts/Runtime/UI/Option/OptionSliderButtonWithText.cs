using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gust.UI.Option
{
    public class OptionSliderButtonWithText : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private CustomButton _minusButton;
        [SerializeField] private CustomButton _plusButton;
        [SerializeField] private TextMeshProUGUI _valueText;

        private void Awake()
        {
            _minusButton.PointerClickEvent.AddListener(() => OnButtonClicked(-1));
            _plusButton.PointerClickEvent.AddListener(() => OnButtonClicked(1));
        }

        private void OnButtonClicked(float value)
        {
            _slider.value += value;
            _valueText.text = _slider.value.ToString();
        }
    }
}
