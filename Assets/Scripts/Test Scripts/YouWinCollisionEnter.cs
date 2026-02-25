using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinOnCollision : MonoBehaviour
{
    [SerializeField] private string winTag = "Goal";
    [SerializeField] private int winSceneIndex = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(winTag))
        {
            Win();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(winTag))
        {
            Win();
        }
    }

    void Win()
    {
        Debug.Log("YOU WIN!");
        SceneManager.LoadScene(winSceneIndex);
    }
}