using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Gust.UI
{
    /// <summary>
    /// Select된 상태를 가질 수 있는 Button. Toggle Group에 속해 있을 경우 Toggle Group에 따라 Select가 동작한다.
    /// </summary>
    public class CustomToggleButton : CustomButton
    {
        [Header("Toggle")]
        [Tooltip("Play 시에 Toggle Group을 인스펙터에서 변경하는 것은 불가능")]
        [SerializeField] private CustomToggleGroup _toggleGroup;
        [SerializeField] private bool _isSelected;
        [SerializeField] private float _selectScale;
        public bool IsSelected => _isSelected;

        [SerializeField] private GameObject _normalBackground;
        [SerializeField] private GameObject _selectedBackground;

        public UnityEvent<bool> OnValueChanged;

        protected override void Awake()
        {
            base.Awake();
            if(_toggleGroup != null)
            {
                _toggleGroup.AddToggleButton(this);
            }
        }

        private void OnDestroy()
        {
            if(_toggleGroup != null)
            {
                _toggleGroup.RemoveToggleButton(this);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            // Toggle Group이 없을 경우에는 단독으로 동작
            if(_toggleGroup == null)
            {
                Select(!_isSelected);
                return;
            }
            else
            {
                _toggleGroup.SelectToggleButton(this);
            }
        }

        public void Select(bool isSelected)
        {
            Debug.Log($"Select: {name}, {isSelected}");
            if(_isSelected == isSelected)
            {
                return;
            }
            _isSelected = isSelected;

            OnValueChanged?.Invoke(isSelected);

            // Pointer Down -> Pointer Up -> Pointer Click
            // Pointer Up에서 Normal Scale을 처리하고
            // Pointer Click에서 Select Scale을 처리하게 된다.
            transform.localScale = _isSelected ? OrignalScale * _selectScale : OrignalScale;
            UpdateBackground(_isSelected);
        }

        private void UpdateBackground(bool isSelected)
        {
            if(_normalBackground != null)
            {
                _normalBackground.SetActive(!isSelected);
            }
            if(_selectedBackground != null)
            {
                _selectedBackground.SetActive(isSelected);
            }
        }
    }
}
