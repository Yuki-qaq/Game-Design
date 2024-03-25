using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthComponent health;

    public bool useAnimator = false;
    [ShowIf(nameof(useAnimator))] public Animator animator;
    [HideIf(nameof(useAnimator))] public Image bar;
    
    float ratio;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        health.HealthChangeEvent.AddListener(UpdateBar);

        ratio = 1 / health.MaxHealth;

        UpdateBar(health.Health);
    }

    public void UpdateBar(float currentHealth)
    {
        if (useAnimator && animator)
        {
            animator.SetFloat("Health", currentHealth);
            return;
        }

        bar.fillAmount = currentHealth * ratio;
    }

    private void OnDestroy()
    {
        health.HealthChangeEvent.RemoveListener(UpdateBar);
    }
}
