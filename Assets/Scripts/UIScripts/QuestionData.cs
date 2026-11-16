using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Overtime/Question")]
public class QuestionData : ScriptableObject
{
    public string questionId;
    public string headline;
    [TextArea(2, 4)] public string prompt;
    public ChoiceOption[] choices;
}

[Serializable]
public enum SelfMode
{
    Core,
    Superficial,
    Defensive,
    Reflective
}

[Serializable]
public class ChoiceOption
{
    public string choiceId;
    public string title;
    public Sprite picture;
    public int smile;
    public int heart;
    public int hours;
    public SelfMode selfMode;
    public int deltaDopamine;
    public int deltaSocial;
    public int deltaKuru;
    public float selflessness = 1f;
}
