using UnityEngine.UI;

public class BossHealthBar : Bar
{
    private Monkey _boss;

    public void BindToBoss(Monkey monkeyKing)
    {
        _boss = monkeyKing;

        Slider.value = 1;

        _boss.HealthChanged += OnValueChanged;

        Player.Instance.Dying += OnPlayerDying;
    }

    private void OnDisable()
    {
        if(_boss != null)
        {
            _boss.HealthChanged -= OnValueChanged;
        }
    }

    private void OnPlayerDying()
    {
        Player.Instance.Dying -= OnPlayerDying;

        this.gameObject.SetActive(false);
    }
}
