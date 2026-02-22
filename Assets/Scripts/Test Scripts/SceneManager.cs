using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
                if (!allowKeyboardStart) return;
        if (loading) return;
        if (SceneManager.GetActiveScene().buildIndex != menuSceneIndex) return;


        if (Keyboard.current.anyKey.wasPressedThisFrame)
            StartGame();
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
}
