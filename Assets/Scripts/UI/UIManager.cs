using System;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Transform dragLayer;
    public Transform rowLayer;

    public static Func<Task> loadingLevel;

    [SerializeField]
    private DefeatUIController defeatUIPrefab;
    private DefeatUIController defeatUI;
    [SerializeField]
    private EnergyUIController energyUIPrefab;
    private EnergyUIController energyUI;
    [SerializeField]
    private HomeUIController homeUIPrefab;
    private HomeUIController homeUI;
    [SerializeField]
    private ShopUIController shopUIPrefab;
    private ShopUIController shopUI;
    [SerializeField]
    private VictoryUIController victoryUIPrefab;
    private VictoryUIController victoryUI;

    public void Awake()
    {
        Instance = this;
    }

    private void CreateUI(ref MonoBehaviour ui, MonoBehaviour uiPrefab )
    {
        Destroy(ui.gameObject);
        ui = Instantiate(uiPrefab, transform);
    }

    private void OnSpendEnergy()
    {
        // TODO: 更新各界面体力值显示
    }

    private void InitDefeatUI(int progress)
    {
        Action destroy = () => Destroy(defeatUI.gameObject);

        DefeatUIController.progress = progress;

        DefeatUIController.clickingHome += () =>
        {
            CreateUI(ref homeUI, homeUIPrefab);
            destroy();
        }

        DefeatUIController.clickingContinue += async () =>
        {
            await Task.Delay(3000); // 播放3秒广告
            _ = loadingLevel?.Invoke(); // TODO: 这一行加载关卡
            destroy();
        };

        DefeatUIController.clickingReplay += () =>
        {
            if (PlayerEnergy.TrySpendEnergy(1)) {
                OnUpdateEnergy();
                _ = loadingLevel?.Invoke(); // TODO: 这一行加载关卡
                destroy();
                return;
            }
            // TODO: 显示体力不足警告及体力补充界面
        }
    }
}
