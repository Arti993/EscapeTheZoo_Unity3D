public class WaterLevel : Level
{
    protected override void OnLevelPassed()
    {
        var waterPond = LevelsChanger.Instance.CurrentLevel.GetComponentInChildren<WaterPond>();

        if (waterPond != null)
            waterPond.StopPlaySounds();

        base.OnLevelPassed();
    }

    private void Awake()
    {
        DelayBeforeRestart = 3;

        var waterPond = LevelsChanger.Instance.CurrentLevel.GetComponentInChildren<WaterPond>();

        if (waterPond != null)
            waterPond.StartPlaySounds();
    }
}
