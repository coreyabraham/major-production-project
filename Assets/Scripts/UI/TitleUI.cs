using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public enum MenuCategory
    {
        Main = 0,
        Play,
        Settings,
        Credits,
        Exit
    }

    [System.Serializable]
    public class Container
    {
        public GameObject Frame;
        public MenuCategory Category;
    }

    [System.Serializable]
    public class Navigator
    {
        public Button Button;
        public MenuCategory Category;
    }

    [field: SerializeField] private Container[] Containers;
    [field: SerializeField] private Navigator[] Navigations;

    public void TestGame() => SceneManager.LoadScene("Framework");

    private void ChangeFrame(MenuCategory Category = MenuCategory.Main)
    {
        foreach (Container container in Containers)
        {
            container.Frame.SetActive(container.Category == Category);
        }
    }

    private void Start()
    {
        foreach (Navigator navigator in Navigations)
        {
            navigator.Button.onClick.AddListener(() => ChangeFrame(navigator.Category));
        }
    }
}
