using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gust.UI.Option
{
    /// <summary>
    /// Option 메뉴에서 Button에 할당되는 클래스. Select에 따라서 Icon과 Label의 색상을 변경한다.
    /// 
    /// </summary>
    public class OptionButton : MonoBehaviour
    {
        [SerializeField] private CustomToggleButton _toggleButton;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;

        private void Awake()
        {
            _toggleButton.OnValueChanged.AddListener(OnValueChangedHandler);
        }

        private void OnValueChangedHandler(bool isSelected)
        {
            _icon.color = isSelected ? Color.white : Color.gray;
            _label.color = isSelected ? Color.white : Color.gray;
        }
    }
}
