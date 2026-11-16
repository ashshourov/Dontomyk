using System;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    public float Dopamine { get; private set; } = 50f;
    public float Social { get; private set; } = 50f;
    public float Kuru { get; private set; } = 26f;

    public event Action Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Apply(float dopamine, float social, float kuru)
    {
        Dopamine = dopamine;
        Social = social;
        Kuru = kuru;
        Changed?.Invoke();
    }
}
