using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionBoard : MonoBehaviour
{
    [SerializeField] private QuestionData[] questions;
    [SerializeField] private GameObject cardOne;
    [SerializeField] private GameObject cardTwo;
    [SerializeField] private GameObject cardThree;
    [SerializeField] private RectTransform questionRoot;
    [SerializeField] private TMP_Text headlineLabel;
    [SerializeField] private TMP_Text promptLabel;

    private ChoiceCardView[] cards;
    private RectTransform[] cardRects;
    private Vector3[] cardBaseScale;
    private RectTransform drawer;
    private int index;
    private bool busy;
    private float laidOutWidth;

    private void Start()
    {
        ResolveQuestion();
        BuildDrawer();
        SpawnCards();
        Show(0);
        if (drawer != null)
        {
            drawer.anchoredPosition = HiddenPosition();
            StartCoroutine(PlayArrival());
        }
    }

    private void ResolveQuestion()
    {
        if (questionRoot == null && transform.parent != null)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Transform child = transform.parent.GetChild(i);
                if (child == transform)
                    continue;

                TMP_Text[] labels = child.GetComponentsInChildren<TMP_Text>(true);
                for (int n = 0; n < labels.Length; n++)
                {
                    if (labels[n].text != null && labels[n].text.Contains("JUST NOW"))
                    {
                        questionRoot = child as RectTransform;
                        break;
                    }
                }

                if (questionRoot != null)
                    break;
            }
        }

        if (questionRoot == null || (headlineLabel != null && promptLabel != null))
            return;

        TMP_Text[] texts = questionRoot.GetComponentsInChildren<TMP_Text>(true);
        TMP_Text upper = null;
        TMP_Text lower = null;
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].text != null && texts[i].text.Contains("JUST NOW"))
                continue;

            if (upper == null)
            {
                upper = texts[i];
                continue;
            }

            if (texts[i].rectTransform.anchoredPosition.y > upper.rectTransform.anchoredPosition.y)
            {
                lower = upper;
                upper = texts[i];
            }
            else
            {
                lower = texts[i];
            }
        }

        if (headlineLabel == null)
            headlineLabel = upper;
        if (promptLabel == null)
            promptLabel = lower;
    }

    private void BuildDrawer()
    {
        GameObject drawerObject = new GameObject("QuestionDrawer", typeof(RectTransform));
        drawer = drawerObject.GetComponent<RectTransform>();
        drawer.SetParent(transform, false);
        drawer.anchorMin = Vector2.zero;
        drawer.anchorMax = Vector2.one;
        drawer.offsetMin = Vector2.zero;
        drawer.offsetMax = Vector2.zero;
        drawer.pivot = new Vector2(0.5f, 0.5f);
        Canvas.ForceUpdateCanvases();

        if (questionRoot != null)
            questionRoot.SetParent(drawer, true);
    }

    private void SpawnCards()
    {
        GameObject[] prefabs = { cardOne, cardTwo, cardThree };
        cards = new ChoiceCardView[prefabs.Length];
        if (drawer == null)
            return;

        cardRects = new RectTransform[prefabs.Length];
        cardBaseScale = new Vector3[prefabs.Length];

        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] == null)
            {
                Debug.LogError("QuestionBoard is missing a card prefab.", this);
                continue;
            }

            GameObject card = Instantiate(prefabs[i], drawer, false);
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            cardRects[i] = rect;
            cardBaseScale[i] = rect.localScale;

            ChoiceCardView view = card.GetComponent<ChoiceCardView>();
            if (view == null)
                view = card.AddComponent<ChoiceCardView>();
            cards[i] = view;
        }

        LayoutCards();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (cards == null || drawer == null)
            return;
        if (Mathf.Abs(drawer.rect.width - laidOutWidth) < 1f)
            return;

        LayoutCards();
    }

    private void LayoutCards()
    {
        if (drawer == null || cardRects == null)
            return;

        Canvas.ForceUpdateCanvases();
        float width = drawer.rect.width;
        if (width < 100f)
            width = ((RectTransform)transform).rect.width;
        if (width < 100f)
            return;

        float boardBottom = drawer.rect.yMin + drawer.rect.height * 0.22f;
        if (questionRoot != null)
        {
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(drawer, questionRoot);
            boardBottom = bounds.min.y;
        }

        float available = boardBottom - drawer.rect.yMin;
        if (available < 120f)
            available = drawer.rect.height * 0.28f;

        float slot = width / cardRects.Length;
        float centerY = boardBottom - available * 0.5f;
        laidOutWidth = width;

        for (int i = 0; i < cardRects.Length; i++)
        {
            RectTransform rect = cardRects[i];
            if (rect == null)
                continue;

            rect.localScale = cardBaseScale[i];
            float cardWidth = Mathf.Abs(rect.rect.width * rect.localScale.x);
            float cardHeight = Mathf.Abs(rect.rect.height * rect.localScale.y);
            float fit = 1f;
            if (cardWidth > slot * 0.9f)
                fit = Mathf.Min(fit, slot * 0.9f / Mathf.Max(cardWidth, 1f));
            if (cardHeight > available * 0.92f)
                fit = Mathf.Min(fit, available * 0.92f / Mathf.Max(cardHeight, 1f));
            rect.localScale = cardBaseScale[i] * fit;

            float x = (i - (cardRects.Length - 1) * 0.5f) * slot;
            rect.anchoredPosition = new Vector2(x, centerY);
        }
    }

    private void Show(int questionIndex)
    {
        if (questions == null || questions.Length == 0 || cards == null)
        {
            Debug.LogError("QuestionBoard has no questions.", this);
            return;
        }

        index = Mathf.Clamp(questionIndex, 0, questions.Length - 1);
        QuestionData question = questions[index];
        if (question == null)
            return;

        if (headlineLabel != null && !string.IsNullOrEmpty(question.headline))
            headlineLabel.text = question.headline;
        if (promptLabel != null)
            promptLabel.text = question.prompt;

        int count = question.choices == null ? 0 : question.choices.Length;
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == null)
                continue;

            bool visible = i < count;
            cards[i].gameObject.SetActive(visible);
            if (!visible)
                continue;

            ChoiceOption choice = question.choices[i];
            cards[i].Bind(
                choice.choiceId,
                choice.title,
                null,
                choice.picture,
                choice.smile,
                choice.heart,
                choice.hours,
                OnPicked);
        }
    }

    private void OnPicked(string choiceId)
    {
        if (busy || questions == null || questions.Length == 0)
            return;

        QuestionData question = questions[index];
        if (question != null && question.choices != null && PlayerState.Instance != null)
        {
            for (int i = 0; i < question.choices.Length; i++)
            {
                if (question.choices[i].choiceId != choiceId)
                    continue;

                ChoiceResolver.Resolve(PlayerState.Instance, question.choices[i]);
                break;
            }
        }

        StartCoroutine(DismissThenNext());
    }

    private IEnumerator PlayArrival()
    {
        busy = true;
        yield return MoveDrawer(Vector2.zero);
        busy = false;
    }

    private IEnumerator DismissThenNext()
    {
        busy = true;
        yield return MoveDrawer(HiddenPosition());
        Show((index + 1) % questions.Length);
        yield return MoveDrawer(Vector2.zero);
        busy = false;
    }

    private Vector2 HiddenPosition()
    {
        float height = drawer != null ? drawer.rect.height : Screen.height;
        if (height < 100f)
            height = Screen.height;
        return new Vector2(0f, -height);
    }

    private IEnumerator MoveDrawer(Vector2 target)
    {
        if (drawer == null)
            yield break;

        Vector2 start = drawer.anchoredPosition;
        float duration = 0.45f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            drawer.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        drawer.anchoredPosition = target;
    }
}
