using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogAnimController : MonoBehaviour
{
    private static readonly int AnimationID = Animator.StringToHash("AnimationID");
<<<<<<< Updated upstream
    public List<Animator> _animators;
    
=======
    [SerializeField] private List<Animator> _animators;
    [SerializeField] private Animator pugAnimator;

>>>>>>> Stashed changes
    private int currentDogState = 0;

    private string currentBotResponse;

    private int xcount;
    private int counterAngry;

    //if current dog state not changing

    private void Start()
    {
        xcount = Random.Range(1, 6);
        Debug.Log("random: " + xcount);
        currentDogState = 0;
        counterAngry = 0;
    }
} 
 /*
private void Update()
    {
        foreach (var animator in _animators)
        {
            animator.SetInteger(AnimationID, currentDogState);
        }
    }*/
    int  counterAngry,counterRun=0;
    public void ParseCommand()
    {
        // Use regex to parse command ie. ["sit down"]
        string pattern = @"\[\s*\""(.*?)\""\s*\]";
        MatchCollection matches = Regex.Matches(currentBotResponse, pattern);
        string command = "";
        foreach (Match match in matches)
        {
            // Extract the content inside ["..."]
            command = match.Groups[1].Value;
        }
        if (command == "angry"&&counterAngry<2)
        {
            currentDogState = 6;
            counterAngry++;
        }
        else if (command == "running"&& counterRun<5)
        {
            currentDogState = 4;
        }
        else if (command == "come")
        {
            currentDogState = 2;
        }
        else if (command == "sit down")
        {
            currentDogState = 7;
        }
        else if (command == "get ball")
        {
            currentDogState = 4;
        }
        else if (command == "wag tail")
        {
            currentDogState = 1;
        }
        else
        {
            int xcount = Random.Range(1, 6);
            Debug.Log("random"+xcount);
            currentDogState = 0;
            counterAngry = 0;   
            }
        }
   
    
    // Set current bot response
    public void SetCurrentBotResponse(string input)
    {
        currentBotResponse = input;
        ParseCommand();
    }
}