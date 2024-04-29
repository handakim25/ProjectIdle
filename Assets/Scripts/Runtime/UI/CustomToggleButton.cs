using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gust.UI
{
    /// <summary>
    /// 선택된 상태를 가지지만 Toggle Group을 가져서 최대 선택 개수가 정해진 버튼.
    /// </summary>
    public class CustomToggleButton : CustomStateButton
    {
        [Tooltip("Play 시에 Toggle Group을 인스펙터에서 변경하는 것은 불가능")]
        [SerializeField] private CustomToggleGroup _toggleGroup;

        protected override void Awake()
        {
            base.Awake();
            if(_toggleGroup != null)
            {
                _toggleGroup.AddToggleButton(this);
            }
        }

        public override void SelectButton()
        {

        }
    }
}
