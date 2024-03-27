using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
using DG.Tweening;

namespace com
{
    public class HoverPanelBehaviour : MonoBehaviour
    {
        public Text title;
        public Text content;

        public bool toLocalize;
        CanvasGroup _cg;

        public float targetAlpha = 0.6f;
        public float delay;
        public float duration = 0;

        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            Hide();
        }

        public void Show(string t, string c)
        {
            _cg.DOKill();
            title.text = t;
            content.text = c;

            if (duration > 0)
                _cg.DOFade(1, duration).SetDelay(delay).OnComplete(OnShowComplete);
            else
                _cg.alpha = targetAlpha;
        }

        void OnShowComplete()
        {
            // TutorialSystem.instance.TryTriggerTuto(TutoInteractionStep.Tuto_skillLongPress);
        }

        public void Hide()
        {
            _cg.DOKill();
            _cg.alpha = 0;
        }
    }
}