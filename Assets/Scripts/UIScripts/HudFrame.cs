using UnityEngine;

// Keeps the in-game HUD at the portrait shape it was built for.
// A wider or shorter display letterboxes this panel instead of stretching it.
public class HudFrame : MonoBehaviour
{
    const float Width = 1080f;
    const float Height = 2340f;

    RectTransform rect;
    bool applying;

    private void Awake()
    {
        rect = (RectTransform)transform;
        Apply();
    }

    private void OnRectTransformDimensionsChange()
    {
        Apply();
    }

    private void Apply()
    {
        if (applying || rect == null)
            return;

        RectTransform parent = rect.parent as RectTransform;
        if (parent == null)
            return;

        float parentWidth = parent.rect.width;
        float parentHeight = parent.rect.height;
        if (parentWidth < 2f || parentHeight < 2f)
            return;

        float target = Width / Height;
        float fittedWidth;
        float fittedHeight;
        if (parentWidth / parentHeight > target)
        {
            fittedHeight = parentHeight;
            fittedWidth = fittedHeight * target;
        }
        else
        {
            fittedWidth = parentWidth;
            fittedHeight = fittedWidth / target;
        }

        if (rect.anchorMin == rect.anchorMax
            && Mathf.Abs(rect.rect.width - fittedWidth) < 0.5f
            && Mathf.Abs(rect.rect.height - fittedHeight) < 0.5f)
            return;

        applying = true;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(fittedWidth, fittedHeight);
        applying = false;
    }
}
