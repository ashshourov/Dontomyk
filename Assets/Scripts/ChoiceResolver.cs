using UnityEngine;

public static class ChoiceResolver
{
    // Document 3 playtest rate: 10 raw contribution points move the meter by 1.
    public const float ContributionPerMeterPoint = 10f;

    public static void Resolve(PlayerState player, ChoiceOption choice)
    {
        if (player == null || choice == null)
            return;

        float beforeDopamine = player.Dopamine;
        float beforeSocial = player.Social;
        float factor = Mathf.Max(0f, choice.selflessness);
        float contributionBefore = Contribution(beforeDopamine, beforeSocial, factor);

        float dopamine = ClampScore(beforeDopamine + choice.deltaDopamine);
        float socialDelta = choice.deltaKuru < 0 ? -choice.deltaDopamine : choice.deltaSocial;
        float social = ClampScore(beforeSocial + socialDelta);

        float contributionAfter = Contribution(dopamine, social, factor);
        float meterDelta = choice.deltaKuru + (contributionAfter - contributionBefore) / ContributionPerMeterPoint;
        float kuru = Mathf.Clamp(player.Kuru + meterDelta, 0f, 100f);

        player.Apply(dopamine, social, kuru);
    }

    public static float Contribution(float dopamine, float social, float selflessness)
    {
        float balance = ((100f - dopamine) + (100f - Mathf.Abs(social - 50f))) / 2f;
        return balance * selflessness;
    }

    private static float ClampScore(float value)
    {
        return Mathf.Clamp(value, 0f, 100f);
    }
}
