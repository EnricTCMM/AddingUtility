using UnityEngine;

public class StartStopController : MonoBehaviour
{
    float timeScale;

    // Use this for initialization
    void Awake()
    {
        Debug.Log("Press space bar to (re)start/pause");
        timeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = timeScale;
            }
            else
            {
                Time.timeScale = 0f;
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = timeScale;
    }
}
