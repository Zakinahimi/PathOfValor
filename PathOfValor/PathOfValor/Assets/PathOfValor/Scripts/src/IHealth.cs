using System;

public interface IHealth
{
    float MaxHealth { get; }
    float CurrentHealth { get; }
    event Action<float, float> OnHealthChanged;
}
