using UnityEngine.UI;

public class BossHealthBar : Bar
{
    private Monkey _boss;

    private void OnDisable()
    {
        if (_boss != null)
        {
            _boss.HealthChanged -= OnValueChanged;
        }
    }

    public void BindToBoss(Monkey monkeyKing)
    {
        _boss = monkeyKing;

        Slider.value = 1;

        _boss.HealthChanged += OnValueChanged;

        Player.Instance.Dying += OnPlayerDying;
    }

    private void OnPlayerDying()
    {
        Player.Instance.Dying -= OnPlayerDying;

        this.gameObject.SetActive(false);
    }
}
