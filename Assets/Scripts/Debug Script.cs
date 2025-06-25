using UnityEngine;
using TMPro;

public class DebugScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _fpsText;

    private float pollingTime = 0.5f;
    private float time;
    private int frameCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / pollingTime);
            _fpsText.text = frameRate.ToString() + " FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }
}
