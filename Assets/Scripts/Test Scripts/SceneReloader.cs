using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SceneReloader : MonoBehaviour
{
    [SerializeField] int menuSceneIndex;
    [SerializeField] int gameSceneIndex;
    [SerializeField] private bool allowKeyboardStart = true;

    private bool loading;



    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
                    Debug.Log("Quit Game Button Pressed");
            Quit();
        }
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