using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust.UI
{
    /// <summary>
    /// 토글 버튼 그룹. 토글 버튼들의 선택을 관리한다.
    /// 현재 단일 선택만 지원한다.
    /// </summary>
    public class CustomToggleGroup : MonoBehaviour
    {
        /// <summary>
        /// 정해진 개수 이상으로 선택되었을 때의 처리 방식
        /// </summary>
        public enum OverSelectPolicy
        {
            DeselectFirst, // 처음 선택된 버튼을 Deselect
            DeselectLast, // 마지막 선택된 버튼을 Deselect
            BlockOverSelect, // 추가 선택 불가
        }

        [SerializeField] private int _maxSelectCount = 1;
        [SerializeField] private OverSelectPolicy _overSelectPolicy = OverSelectPolicy.DeselectFirst;

        /// <summary>
        /// OverSelectPolicy가 BlockOverSelect일 때, 선택이 불가능할 때 발생하는 이벤트
        /// </summary>
        public event System.Action<CustomToggleGroup> OverSelect;
        public event System.Action<CustomToggleButton> OnSelect;

        private List<CustomToggleButton> _toggleButtons = new List<CustomToggleButton>();
        /// <summary>
        /// Select된 버튼들. 순서대로 선택된 버튼들이 들어간다.
        /// 최대 선택 가능 개수를 넘어서면, OverSelectPolicy에 따라 처리한다.
        /// </summary>
        private List<CustomToggleButton> _selectedButtons = new List<CustomToggleButton>();

        public void AddToggleButton(CustomToggleButton toggleButton)
        {
            if(toggleButton == null)
            {
                return;
            }

            if (!_toggleButtons.Contains(toggleButton))
            {
                _toggleButtons.Add(toggleButton);
            }
        }

        public void RemoveToggleButton(CustomToggleButton toggleButton)
        {
            if (toggleButton == null)
            {
                return;
            }

            if (_toggleButtons.Contains(toggleButton))
            {
                _toggleButtons.Remove(toggleButton);
            }
        }

        public void SelectToggleButton(CustomToggleButton toggleButton)
        {
            if(toggleButton == null)
            {
                return;
            }
            if(_selectedButtons.Contains(toggleButton))
            {
                return;
            }

            // 선택된 버튼이 최대 선택 가능 개수보다 많으면
            // 다른 Button을 정해진 규칙에 따라 DeSelect
            if(_selectedButtons.Count >= _maxSelectCount)
            {
                switch(_overSelectPolicy)
                {
                    case OverSelectPolicy.DeselectFirst:
                        _selectedButtons[0].Select(false);
                        _selectedButtons.RemoveAt(0);
                        break;
                    case OverSelectPolicy.DeselectLast:
                        _selectedButtons[^1].Select(false);
                        _selectedButtons.RemoveAt(_selectedButtons.Count - 1);
                        break;
                    case OverSelectPolicy.BlockOverSelect:
                        OverSelect?.Invoke(this);
                        return;
                }
            }

            _selectedButtons.Add(toggleButton);
            toggleButton.Select(true);
            OnSelect?.Invoke(toggleButton);
        }
    }
}
