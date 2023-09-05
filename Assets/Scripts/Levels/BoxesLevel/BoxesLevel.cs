using StarterAssets;

public class BoxesLevel : Level
{
    private void Start()
    {
        ThirdPersonController.Instance.EnablePickUpOrDrop();

        ThirdPersonController.Instance.RotationSmoothTime = 0.4f;
    }

    protected override void OnLevelPassed()
    {
        base.OnLevelPassed();

        ThirdPersonController.Instance.RotationSmoothTime = 0.2f;

        ThirdPersonController.Instance.DisablePickUpOrDrop();
    }

}
