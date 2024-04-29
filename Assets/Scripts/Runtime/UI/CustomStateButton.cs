using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
using UnityEngine.EventSystems;

namespace Gust.UI
{
    /// <summary>
    /// 선택 상태를 가질 수 있는 버튼. Select가 되면 다시 Select될 수는 없다.
    /// Click 이벤트는 일단 발생하는 것으로 정한다.
    /// </summary>
    public class CustomStateButton : CustomButton
    {
        [Header("Selected State")]
        [SerializeField] private float _selectedScale;
        [SerializeField] private bool _isSelected;
        public bool IsSelected => _isSelected;
        private bool _isInit = false;

        [Tooltip("Select Event, Only Once")]
        public UnityEvent OnSelect;

        private void OnEnable()
        {
            if(!_isInit)
            {
            }
        }

        private void Start()
        {
            if(_isSelected)
            {
                SelectButton();
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            SelectButton();
        }

        public virtual void SelectButton()
        {
            if(_isSelected)
            {
                return;
            }

            _isSelected = true;
            OnSelect?.Invoke();
            transform.localScale = _originScale * _selectedScale;
        }
    }
}
