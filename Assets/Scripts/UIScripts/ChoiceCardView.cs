using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceCardView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image picture;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text smileValue;
    [SerializeField] private TMP_Text heartValue;
    [SerializeField] private TMP_Text hoursValue;
    [SerializeField] private Button button;

    public string ChoiceId { get; private set; }

    public void Bind(
        string choiceId,
        string titleText,
        Sprite backgroundSprite,
        Sprite pictureSprite,
        int smile,
        int heart,
        int hours,
        Action<string> onSelected)
    {
        EnsureWired();
        ChoiceId = choiceId;

        if (title != null)
            title.text = titleText;

        if (background != null && backgroundSprite != null)
            background.sprite = backgroundSprite;

        if (picture != null && pictureSprite != null)
        {
            picture.sprite = pictureSprite;
            picture.preserveAspect = true;
        }

        if (smileValue != null)
            smileValue.text = FormatDelta(smile);

        if (heartValue != null)
            heartValue.text = FormatDelta(heart);

        if (hoursValue != null)
            hoursValue.text = hours + "H";

        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        if (onSelected != null)
            button.onClick.AddListener(() => onSelected(ChoiceId));
    }

    private void EnsureWired()
    {
        if (background == null)
            background = GetComponent<Image>();

        if (picture == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].gameObject != gameObject && images[i].name == "Image")
                {
                    picture = images[i];
                    break;
                }
            }
        }

        TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            Transform parent = labels[i].transform.parent;
            string parentName = parent != null ? parent.name : string.Empty;
            if (smileValue == null && parentName == "Smile")
                smileValue = labels[i];
            else if (heartValue == null && parentName == "Life")
                heartValue = labels[i];
            else if (hoursValue == null && parentName == "Timer")
                hoursValue = labels[i];
            else if (title == null && parent == transform)
                title = labels[i];
        }

        if (button == null)
            button = GetComponent<Button>();

        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
            button.targetGraphic = background;
        }
    }

    public static string FormatDelta(int value)
    {
        if (value > 0)
            return "+" + value;

        return value.ToString();
    }
}
