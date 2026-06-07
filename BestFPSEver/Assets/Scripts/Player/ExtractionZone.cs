using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionZone : MonoBehaviour
{
    public float extractTime = 5f;

    private float timer;

    private bool playerInside;

    private string mainMenuScene = "MainMenu";

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player entered extraction zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0;
            Debug.Log("Player exited extraction zone");
        }
    }

    private void Update()
    {
        if (!playerInside)
            return;

        timer += Time.deltaTime;
        Debug.Log("Time in the zone:" + timer);

        if (timer >= extractTime)
        {
            FinishLevel();
        }
    }

    private void FinishLevel()
    {
        Debug.Log("Extraction Complete");

        var scoreManager = FindAnyObjectByType<ScoreManager>();
        int runCoins = scoreManager != null ? scoreManager.GetScore() : 0;

        if (SaveLoadManager.Instance != null)
        {
            int total = SaveLoadManager.Instance.LoadCoins();
            total += runCoins;
            SaveLoadManager.Instance.SaveCoins(total);
            Debug.Log($"Saved {runCoins} run coins. Total coins now: {total}");
        }
        else
        {
            Debug.LogWarning("SaveLoadManager instance not found - cannot save coins.");
        }
        SceneManager.LoadScene(mainMenuScene);

    }
}