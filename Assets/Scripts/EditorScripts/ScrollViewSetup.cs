using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSetup : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public RectTransform content;
    public Scrollbar verticalScrollbar;

    void Start()
    {
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.verticalScrollbar = verticalScrollbar;
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.anchoredPosition = Vector2.zero;

        if (viewport.GetComponent<Image>() == null)
            viewport.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        if (viewport.GetComponent<Mask>() == null)
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;

        if (content.GetComponent<VerticalLayoutGroup>() == null)
            content.gameObject.AddComponent<VerticalLayoutGroup>();
        if (content.GetComponent<ContentSizeFitter>() == null)
        {
            var csf = content.gameObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
