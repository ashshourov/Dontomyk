using UnityEngine;

public static class ChoiceReaction
{
    public static void Play(ChoiceOption choice)
    {
        if (choice == null)
            return;

        BlandShapeController face = Object.FindAnyObjectByType<BlandShapeController>(FindObjectsInactive.Include);
        AnimatorTriggerTester body = Object.FindAnyObjectByType<AnimatorTriggerTester>(FindObjectsInactive.Include);

        if (face != null)
            face.ResetAllBlendShapes();

        switch (choice.selfMode)
        {
            case SelfMode.Superficial:
                if (face != null)
                    face.SetSmile(100f);
                if (body != null)
                    body.PlayHI();
                break;
            case SelfMode.Core:
                if (face != null)
                    face.SetSad(100f);
                if (body != null)
                    body.PlayTslap();
                break;
            case SelfMode.Defensive:
                if (face != null)
                    face.SetAmaze(100f);
                if (body != null)
                    body.PlayNo();
                break;
            default:
                if (face != null)
                    face.SetSmile(100f);
                if (body != null)
                    body.PlayYes();
                break;
        }
    }
}
