using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceholderSceneManager : MonoBehaviour
{
    [SerializeField] GameObject _options, mainMenu;
    public void MainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        _options.gameObject.SetActive(false);
    }
    public void Options()
    {
        _options.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
