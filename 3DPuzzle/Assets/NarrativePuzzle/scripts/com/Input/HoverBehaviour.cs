using UnityEngine;
using UnityEngine.EventSystems;

namespace com
{
    public class HoverBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public string title;

        [Multiline]
        public string description;

        public HoverPanelBehaviour hoverPanel;

        public void OnPointerDown(PointerEventData eventData)
        {
            hoverPanel.Show(title, description);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            hoverPanel.Hide();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverPanel.Hide();
        }
    }
}