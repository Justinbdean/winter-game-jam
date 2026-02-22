using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] int menuSceneIndex;
    [SerializeField] int gameSceneIndex;
    [SerializeField] private bool allowKeyboardStart = true;

    private bool loading;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != menuSceneIndex)
            SceneManager.LoadScene(menuSceneIndex);
    }

    void Update()
    {
        if (!loading &&
            allowKeyboardStart &&
            SceneManager.GetActiveScene().buildIndex == menuSceneIndex &&
            Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartGame();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
                    Debug.Log("Quit Game Button Pressed");
            Quit();
        }
    }

    public void StartGame()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void OnDeath()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(gameSceneIndex);
    }

    void Quit()
    {
        Debug.Log("Quit Game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}