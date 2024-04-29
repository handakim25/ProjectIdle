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
        private List<CustomToggleButton> _toggleButtons = new List<CustomToggleButton>();
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

            if(_selectedButtons.Count > 0)
            {
                _selectedButtons[0].SelectButton();
            }

            _selectedButtons.Clear();
            _selectedButtons.Add(toggleButton);
        }
    }
}
