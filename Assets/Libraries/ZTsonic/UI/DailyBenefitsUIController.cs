// Generated from: DailyBenefitsUIController.ts
// Generated at: 2026-02-05T17:59:33.099Z
// WARNING: Do not modify this file manually

namespace ZTsonic
{
    public class __Anon_6373_3741513b
    {
        public required global::SystemUIManager Instance { get; set; }

        public required global::System.Action initingHome { get; set; }

        public required global::System.Action addingSteps { get; set; }

        public required global::System.Action<global::Row> unlockingSlot { get; set; }

        public required global::System.Func<int, object?, global::System.Threading.Tasks.Task> loadingLevel { get; set; }

        public required global::System.Action<global::UIType> initingZFSharpUICallbacks { get; set; }

        public required uint MAX_FPS_RUNTIME { get; init; }
    }
    public class DailyBenefitsUIController : global::UnityEngine.MonoBehaviour
    {
        [global::UnityEngine.SerializeField]
        private string tips = "";

        public static global::System.Func<global::Tsonic.Runtime.Union<double, string>, global::System.Threading.Tasks.Task> clickingWatchAd;

        private static bool isWatchingAd;

        public void OnClickRecieve()
            {
            if (DailyBenefitsUIController.isWatchingAd)
                return;
            var task = async () =>
            {
            DailyBenefitsUIController.isWatchingAd = true;
            await DailyBenefitsUIController.clickingWatchAd("");
            DailyBenefitsUIController.isWatchingAd = false;
            };
            task();
            }

        public void Start()
            {
            var task = async () =>
            {
            await global::System.Threading.Tasks.Task.Delay(1000);
            await SystemUIManager.PopUpTips(this.tips, ((global::UnityEngine.MonoBehaviour)SystemUIManager.Instance).transform);
            await global::System.Threading.Tasks.Task.Delay(1000);
            await SystemUIManager.PopUpTips(global::Tsonic.Runtime.Operators.@typeof(DailyBenefitsUIController.clickingWatchAd));
            };
            task();
            }

        public void PlayClickingAudio()
            {
            (((global::IAudioTrigger)this).PlayClickingAudio)(null);
            }
    }
}