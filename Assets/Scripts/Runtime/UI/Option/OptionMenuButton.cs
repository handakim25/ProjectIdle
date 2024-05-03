using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gust.UI.Option
{
    /// <summary>
    /// Left Option Menu. Click이 되었을 때 Color 변경
    /// </summary>
    public class OptionMenuButton : MonoBehaviour
    {
        [SerializeField] private CustomToggleButton _toggleButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;

        Color _originIconColor;
        Color _originLabelColor;

        private void Awake()
        {
            _toggleButton.OnValueChanged.AddListener(OnValueChangedHandler);
            _originIconColor = _icon.color;
            _originLabelColor = _label.color;
        }

        private void OnValueChangedHandler(bool isSelected)
        {
            _icon.color = isSelected ? Color.white : Color.gray;
            _label.color = isSelected ? Color.white : Color.gray;
        }
    }
}
