using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace Gust.UI
{
    /// <summary>
    /// 버튼 클래스. 클릭을 하면 PressedScale로 스케일이 변경되고, 클릭을 떼면 원래 스케일로 돌아온다.
    /// </summary>
    public class CustomButton : MonoBehaviour, IPointerUpHandler, IPointerClickHandler, IPointerDownHandler
    {
        [Tooltip("Pointer Up Event")]
        public UnityEvent PointerUp;
        [Tooltip("Pointer Click Event")]
        public UnityEvent PointerClick;

        [Header("Button Animation")]
        [Tooltip("Press Scale")]
        [SerializeField] protected float _pressedScale = 0.9f;
        protected Vector3 _originScale;

        [SerializeField] protected float _tweenDuration = 0.1f;
        [SerializeField] protected Ease _tweenEaseType = Ease.OutCubic;

        protected virtual void Awake()
        {
            _originScale = transform.localScale;
        }

        private void OnDisable()
        {
            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
            }
            transform.localScale = _originScale;
        }

        // @Memo
        // OnPointerDown -> OnPointerUp -> OnPointerClick
        // 언제 애니메이션을 재생할 지 헷갈리므로
        // 애니메이션을 사용할 때는 OnPointerDown에서 애니메이션을 시작하고, OnPointerUp에서 애니메이션을 취소하기로 정한다.

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke();

            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
            }
            transform.localScale = _originScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Pressed 상태로 스케일 변경
            transform.DOScale(_originScale * _pressedScale, _tweenDuration)
                .SetEase(_tweenEaseType)
                .SetUpdate(true);
        }
    }
}
