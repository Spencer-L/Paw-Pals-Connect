using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DogManager : MonoBehaviour
{
    public GameObject[] dogs;  // Array to hold different dog GameObjects
    public Dropdown dogDropdown;  // Reference to the UI Dropdown component

    // Start is called before the first frame update
    void Start()
    {
        if (dogs.Length > 0)
        {
            // Populate the dropdown with the names of the dog objects
            dogDropdown.ClearOptions();
            var dogNames = new List<string>();
            foreach (var dog in dogs)
            {
                dogNames.Add(dog.name);
            }
            dogDropdown.AddOptions(dogNames);
        }
        else
        {
            Debug.LogWarning("No dogs assigned in the array.");
        }
    }

    // When the button is clicked, toggle the selected dog
    public void WhenButtonClicked()
    {
        if (dogs.Length == 0) return;

        int selectedIndex = dogDropdown.value;  // Get the selected dog index from the dropdown
        GameObject selectedDog = dogs[selectedIndex];  // Get the dog based on dropdown selection

        // Toggle the active state of the selected dog
        if (selectedDog != null)
        {
            selectedDog.SetActive(!selectedDog.activeSelf);
        }
    }
}
