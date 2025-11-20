using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Linq;

public class LevelConfigLoader : MonoBehaviour
{
    //获取基础表的json名称
    public string levelConfigFileName = "levels.json";
    public string wordsConfigFileName = "words.json";
    public string languageConfigFileName = "language.json";



    //将levelConfigLoader立为单例，便于全局访问
    public static LevelConfigLoader Instance { get; private set; }

    //
    private LevelConfig levelConfig;
    private LanguageDataConfig languageConfig;
    private WordsConfig wordsConfig;

    private Dictionary<string, string> languageDict;


    private void Awake()
    {
        //确保单例唯一
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //加载level.json
        LoadLevelConfig();
        LoadLanguageConfig();
        LoadWordsConfig();
    }
    //加载关卡配置文件

    #region 读表方法组
    private void LoadLevelConfig()
    {
        //定义文件查找路径，统一的json安置在StreamingAssetsPath这个路径下，打包的时候的特殊文件夹，不会被压缩
        string path = Path.Combine(Application.streamingAssetsPath, levelConfigFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            levelConfig = JsonUtility.FromJson<LevelConfig>(json);//通过这个方法来获得json的内容赋值给levelConfig
            Debug.Log(" 成功加载关卡配置，共有关卡：" + levelConfig.levels.Count);
            Debug.Log(levelConfig.levels[1].steps);
        }
        else
        {
            Debug.LogError($"Config file not found at : {path}");
        }
    }
    private void LoadWordsConfig()
    {
        //定义文件查找路径，统一的json安置在StreamingAssetsPath这个路径下，打包的时候的特殊文件夹，不会被压缩
        string path = Path.Combine(Application.streamingAssetsPath, wordsConfigFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            wordsConfig = JsonUtility.FromJson<WordsConfig>(json);//通过这个方法来获得json的内容赋值给levelConfig
            Debug.Log(" 成功加载词语配置，共有词语：" + wordsConfig.words.Count);

        }
        else
        {
            Debug.LogError($"Config file not found at : {path}");
        }
    }
    private void LoadLanguageConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, languageConfigFileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            languageConfig = JsonUtility.FromJson<LanguageDataConfig>(json);
            languageDict = languageConfig.languages.ToDictionary(x => x.id, x => x.text);
            Debug.Log("成功加载多语言表json，共有多语言" + languageConfig.languages.Count);
        }
        else
        {
            Debug.LogError($"Config file not found at : {path}");
        }
    }
    #endregion


    #region 读表后的获取操作
    //获得level的数据
    public LevelData GetLevelData(int levelID)
    {
        return levelConfig.levels.Find(I => I.id == levelID);
    }
    //给levelManager用的数据读取
    public WordsData GetWordsData(int wordsID)
    {
        return wordsConfig.words.Find(I => I.id == wordsID);
    }
    //获得当前关卡的总数
    public int GetTotalLevelCount()
    {
        Debug.Log(levelConfig.levels[0].mainGroup.Length);
        return levelConfig.levels.Count;
    }
    //获取多语言的对应文本
    public string GetTextById(string id)
    {
        if (languageDict == null)
        {
            Debug.LogError("languageDict 未初始化！");
            return "语言数据未加载";
        }

        if (languageDict.TryGetValue(id, out string text))
        {
            return text;
        }
        else
        {
            Debug.LogWarning($"未找到对应文本 ID：{id}");
            return "未找到对应文本";

        }

    }
}
    #endregion