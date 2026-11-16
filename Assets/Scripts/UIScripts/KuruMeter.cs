using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KuruMeter : MonoBehaviour
{
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text numberLabel;
    [SerializeField] private Image arc;

    private float shown;

    private void Awake()
    {
        if (PlayerState.Instance == null && GetComponent<PlayerState>() == null)
            gameObject.AddComponent<PlayerState>();

        Image badge = GetComponent<Image>();
        if (badge != null)
            badge.preserveAspect = true;

        if (titleLabel == null)
            titleLabel = FindLabel(true);
        if (numberLabel == null)
            numberLabel = FindLabel(false);

        if (arc == null)
            arc = CreateArc();

        shown = PlayerState.Instance != null ? PlayerState.Instance.Kuru : 26f;
        ApplyShown();
    }

    private void OnEnable()
    {
        if (PlayerState.Instance != null)
            PlayerState.Instance.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (PlayerState.Instance != null)
            PlayerState.Instance.Changed -= Refresh;
    }

    private void Update()
    {
        if (PlayerState.Instance == null)
            return;

        shown = Mathf.MoveTowards(shown, PlayerState.Instance.Kuru, 40f * Time.deltaTime);
        ApplyShown();
    }

    private void Refresh()
    {
        if (PlayerState.Instance == null)
            return;

        if (numberLabel != null)
            numberLabel.text = Mathf.RoundToInt(PlayerState.Instance.Kuru).ToString();
    }

    private void ApplyShown()
    {
        if (numberLabel != null)
            numberLabel.text = Mathf.RoundToInt(shown).ToString();
        if (arc != null)
        {
            float amount = Mathf.Clamp01(shown / 100f);
            if (amount >= 0.999f)
            {
                arc.type = Image.Type.Simple;
            }
            else
            {
                arc.type = Image.Type.Filled;
                arc.fillMethod = Image.FillMethod.Radial360;
                arc.fillOrigin = (int)Image.Origin360.Top;
                arc.fillClockwise = true;
                arc.fillAmount = amount;
            }
        }

        FitLabels();
    }

    private void FitLabels()
    {
        Rect bounds = ((RectTransform)transform).rect;
        float diameter = Mathf.Min(Mathf.Abs(bounds.width), Mathf.Abs(bounds.height));
        if (diameter <= 1f)
            return;

        float width = diameter * 0.68f;
        FitLabel(titleLabel, diameter * 0.16f, width, diameter * 0.16f, diameter * 0.13f);
        FitLabel(numberLabel, -diameter * 0.08f, width, diameter * 0.34f, diameter * 0.32f);
    }

    private static void FitLabel(TMP_Text label, float centerY, float width, float height, float maxSize)
    {
        if (label == null)
            return;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(0f, centerY);

        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.overflowMode = TextOverflowModes.Overflow;
        label.enableAutoSizing = true;
        label.fontSizeMin = 10f;
        label.fontSizeMax = Mathf.Max(12f, maxSize);
        label.alignment = TextAlignmentOptions.Center;
    }

    private TMP_Text FindLabel(bool title)
    {
        TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            string value = labels[i].text == null ? string.Empty : labels[i].text.ToUpperInvariant();
            bool isTitle = value.Contains("KURU");
            if (isTitle == title)
                return labels[i];
        }

        return null;
    }

    private Image CreateArc()
    {
        GameObject arcObject = new GameObject("Kuru Arc", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        arcObject.transform.SetParent(transform, false);
        arcObject.transform.SetAsFirstSibling();

        RectTransform rect = arcObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.04f, 0.04f);
        rect.anchorMax = new Vector2(0.96f, 0.96f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = arcObject.GetComponent<Image>();
        image.sprite = RingSprite();
        image.preserveAspect = true;
        image.color = new Color(0.67f, 0.28f, 0.95f, 1f);
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = (int)Image.Origin360.Top;
        image.fillClockwise = true;
        image.fillAmount = 0.26f;
        image.raycastTarget = false;
        return image;
    }

    private static Sprite ringSprite;

    private static Sprite RingSprite()
    {
        if (ringSprite != null)
            return ringSprite;

        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        float center = (size - 1) * 0.5f;
        float outer = center * 0.98f;
        float inner = center * 0.86f;
        Color[] pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float outerFade = Mathf.Clamp01(outer - distance + 1.5f);
                float innerFade = Mathf.Clamp01(distance - inner + 1.5f);
                float alpha = Mathf.Clamp01(Mathf.Min(outerFade, innerFade));
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        ringSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        return ringSprite;
    }
}
