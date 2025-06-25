using UnityEngine;

public class Timer : MonoBehaviour
{

    public float duration = 1.0f;
    public bool running = false;

    private float startTime;

    // Start is called before the first frame update.
    protected void Start()
    {
        startTime = Time.time;
    }

    // Update is called every frame.
    protected void Update()
    {
        if (running)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime >= duration)
            {
                running = false;
                // Do something when the timer is finished.
            }
        }
    }

    public void StartTimer()
    {
        running = true;
        startTime = Time.time;
    }

    public void StopTimer()
    {
        running = false;
    }
}
