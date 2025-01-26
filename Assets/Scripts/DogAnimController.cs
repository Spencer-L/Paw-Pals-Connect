using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DogAnimController : MonoBehaviour
{
    private static readonly int AnimationID = Animator.StringToHash("AnimationID");
    [SerializeField] private List<Animator> _animators;
    [SerializeField] private Animator pugAnimator;
    
    private int currentDogState = 0;
    
    private string currentBotResponse;

    private void Update()
    {
        foreach (var animator in _animators)
        {
            animator.SetInteger(AnimationID, currentDogState);
        }
    }
    int  counterAngry=0;
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
        else if (command == "running")
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