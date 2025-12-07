public interface IAudioTrigger
{
    public void PlayClickingAudio();
    public void PlayClickingAudio(object _) => AudioManager.Instance.PlayUISFX(AudioManager.UISFXtype.ClickButton);
}
