using UnityEngine;

public class TestScript : MonoBehaviour, IWalkable
{
    public void Walk()
    {
        Debug.Log("Im walking");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
    }
}
