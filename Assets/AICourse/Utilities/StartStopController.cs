using UnityEngine;

public class StartStopController : MonoBehaviour
{
    float timeScale;

    // Use this for initialization
    void Awake()
    {
        if (!this.enabled) return; 
        timeScale = Time.timeScale;
        Time.timeScale = 0;
        Debug.Log("NOW PAUSED. Press space bar to (re)start/pause");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = timeScale;
                Debug.Log("NOW RUNNING. Press space bar to (re)start/pause");
            }
            else
            {
                Time.timeScale = 0f;
                Debug.Log("NOW PAUSED. Press space bar to (re)start/pause");
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Debug.Log("NOW PAUSED. Press space bar to (re)start/pause");
    }

    public void Resume()
    {
        Time.timeScale = timeScale;
        Debug.Log("NOW RUNNING. Press space bar to (re)start/pause");
    }
}
