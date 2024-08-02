using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Assets.SFK.Scripts.Gameplay
{
    public class ZiliaoSys : MonoBehaviour
    {

        public Transform cam;
        public Transform pos1;
        public Transform pos2;
        public float duration = 2;
        public GameObject nextLevel;

        // Use this for initialization
        void Start()
        {
            Move1();
        }

        void Move1()
        {
            cam.DOMove(pos1.position, duration).SetEase(Ease.InOutCubic).OnComplete(Move2);
            cam.DORotate(pos1.eulerAngles, duration).SetEase(Ease.InOutCubic);
        }
        void Move2()
        {
            cam.DOMove(pos2.position, duration).SetEase(Ease.InOutCubic).OnComplete(Move1);
            cam.DORotate(pos2.eulerAngles, duration).SetEase(Ease.InOutCubic);
        }

        private int countToggle = 0;

        public void ToggleCanvasGroup(CanvasGroup cg)
        {
            cg.DOKill();
            if (cg.alpha > 0)
                cg.DOFade(0, 0.35f);
            else
                cg.DOFade(1, 0.35f);

            CheckNextLevel();
        }

        void CheckNextLevel()
        {
            countToggle++;
            if (countToggle >= 4)
                nextLevel.SetActive(true);
        }

        public void OnClickNextLevel()
        {
            SceneManager.LoadScene("shilian 1");
        }
    }
}