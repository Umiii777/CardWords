public class UnlockSlotUIController : StaticAdProcessor<UnlockSlotUIController, Row>, IAudioTrigger
{
    public static Row currentRow;

    public void OnClickUnlock() => OnClickReceive(currentRow);

    public void PlayClickingAudio() => (this as IAudioTrigger).PlayClickingAudio(default);
}
