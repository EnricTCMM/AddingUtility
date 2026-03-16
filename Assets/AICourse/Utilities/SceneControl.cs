
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class SceneControl : MonoBehaviour
{

    public bool enableSpacebar = true;

    private float timeScale;
    private TextMeshProUGUI btGoPauseText;

    public void Update ()
    {
        if (enableSpacebar)
            if (Input.GetKeyDown("space"))
                GoAndPause();
    }

    void Awake()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 0f;

        btGoPauseText = GameObject.Find("Canvas/GoButton/Text").GetComponent<TextMeshProUGUI>();
    }

    public void RestartScene ()
    {
        if (Time.timeScale == 0f) Time.timeScale = timeScale;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoAndPause ()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = timeScale;
            btGoPauseText.text = "Pause";
        }
        else
        {
            Time.timeScale = 0f;
            btGoPauseText.text = "Go";
        }
    }

    public void Recall ()
    {
        // find all recallable objects
        RecallPosition[] recallables = FindObjectsOfType<RecallPosition>();
        foreach (RecallPosition r in recallables)
            r.Invoke("Recall",0);
    }

} 
