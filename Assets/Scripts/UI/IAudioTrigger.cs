using static AudioManager;

public interface IAudioTrigger
{
    public void PlayClickingAudio();
    public void PlayClickingAudio(object _) => AudioManager.Instance.PlayUISFX(UISFXtype.ClickButton);
}
