using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] private float _health;
    [SerializeField] private SkinnedMeshRenderer _renderer;

    private float _currentHealth;
    protected float BlinkAfterTakeDamageTime = 0.3f;
    protected Color HurtColor = new Color(1f, 0.4f, 0.4f);
    protected Animator Animator;
    protected Coroutine CurrentCoroutine;

    public float CurrentHealth => _currentHealth;

    public event Action<float, float> HealthChanged;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();

        _currentHealth = _health;
    }

    public virtual void TakeDamage(float damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            HealthChanged?.Invoke(_currentHealth, _health);

            if (CurrentCoroutine == null)
                CurrentCoroutine = StartCoroutine(OnHurt());
        }
    }

    public void RestoreFullHealth()
    {
        _currentHealth = _health;

        HealthChanged?.Invoke(_currentHealth, _health);
    }

    private IEnumerator OnHurt()
    {
        List<MaterialColorChanger> materialColorChangers = new List<MaterialColorChanger>();

        foreach (var material in _renderer.materials)
        {
            MaterialColorChanger materialColorChanger = new MaterialColorChanger();

            materialColorChangers.Add(materialColorChanger);

            materialColorChanger.ChangeMaterialColor(material, HurtColor);
        }

        yield return new WaitForSeconds(BlinkAfterTakeDamageTime);

        foreach (var materialColorChanger in materialColorChangers)
        {
            materialColorChanger.ChangeMaterialColor(materialColorChanger.Material, materialColorChanger.PreviousColor);
        }

        CurrentCoroutine = null;
    }
}
