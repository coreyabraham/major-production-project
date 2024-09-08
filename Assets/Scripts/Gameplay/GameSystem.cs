using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSystem : Singleton<GameSystem>
{
    [System.Serializable]
    public class GameEvents
    {
        public UnityEvent PlayerDied;

        public UnityEvent RequestLoadingUI;
        [HideInInspector] public UnityEvent LoadingUIFinished;

        public UnityEvent LoadingStarted;
        public UnityEvent LoadingFinished;
        public UnityEvent<float> LoadingProgress;

        public UnityEvent<Scene> SceneUnloaded;
        public UnityEvent<Scene, LoadSceneMode> SceneLoaded;
        public UnityEvent<Scene, Scene> SceneChanged;
    }

    [field: Header("Tags")]
    public string PlayerTag = "Player";
    public string CameraTag = "Camera";

    [field: Header("Externals")]
    public PlayerSystem Player;
    public CameraSystem Camera;

    [field: Header("Miscellaneous")]
    public bool GameplayPaused = false;
    public string[] LevelNames;
    public GameEvents Events;

    private string TargetSceneName;
    private float ElapsedPlaytime;

    public float GetElapsedPlaytime() => ElapsedPlaytime;
    public void SetPausedState(bool State) => GameplayPaused = State; 

    public void PlayerDiedScenario()
    {
        // TODO: Reset all interactable / moveable objects the Player interacts with in the level they're currently in during the death transition!
    }

    public void RefreshCachedExternals()
    {
        if (Player == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(PlayerTag))
            {
                obj.TryGetComponent(out Player);
                if (Player != null) break;
            }
        }

        if (Camera == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(CameraTag))
            {
                obj.TryGetComponent(out Camera);
                if (Camera != null) break;
            }
        }
    }

    public void RequestLoadScene(string SceneName)
    {
        TargetSceneName = SceneName;
        Events.RequestLoadingUI?.Invoke();
    }

    public void LoadScene()
    {
        if (string.IsNullOrWhiteSpace(TargetSceneName))
        {
            Debug.LogWarning(name + " | Could not Load Scene with Name: " + TargetSceneName + "\nMake sure the target Scene you're attempting to load is in the `Build Settings` 'Scenes in Build' list!");
            return;
        }

        Events.LoadingStarted?.Invoke();

        StartCoroutine(LoadSceneInBackground(TargetSceneName));
        StartCoroutine(UnloadSceneInBackground());

        TargetSceneName = string.Empty;
    }

    // Could possibly be done in an Update loop instead? If so, then try adding Lerping to it!
    IEnumerator LoadSceneInBackground(string SceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);

        while (!operation.isDone)
        {
            Events.LoadingProgress?.Invoke(Mathf.Clamp01(operation.progress / 0.9f));
            yield return null;
        }

        Events.LoadingFinished?.Invoke();
    }

    IEnumerator UnloadSceneInBackground()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!operation.isDone) yield return null;
    }

    private void SceneLoaded(Scene Scene, LoadSceneMode Mode) => Events.SceneLoaded?.Invoke(Scene, Mode);
    private void SceneUnloaded(Scene Scene) => Events.SceneUnloaded?.Invoke(Scene);
    private void ActiveSceneChanged(Scene Old, Scene New) => Events.SceneChanged?.Invoke(Old, New);

    private void Update()
    {
        if (GameplayPaused) return;
        ElapsedPlaytime += Time.deltaTime;
    }

    protected override void Initialize()
    {
        RefreshCachedExternals();

        SceneManager.activeSceneChanged += ActiveSceneChanged;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        Events.LoadingUIFinished.AddListener(LoadScene);
    }
}
