using UnityEngine;

public class DogManager : MonoBehaviour
{
    public GameObject dog;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhenButtonClicked()
    {
        if (dog.activeInHierarchy == true)
            dog.SetActive(false);
        else
            dog.SetActive(true);
        
    }

}
