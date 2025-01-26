using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DogAnimController : MonoBehaviour
{
    private static readonly int AnimationID = Animator.StringToHash("AnimationID");
    public List<Animator> _animators;
    
    private int currentDogState = 0;
    
    private string currentBotResponse;

    private void Update()
    {
        foreach (var animator in _animators)
        {
            animator.SetInteger(AnimationID, currentDogState);
        }
    }

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
        Debug.Log("[DOG ANIM CONTROLLER]: Parsing command: " + command);
        if (command == "angry")
        {
            Debug.Log("[DOG ANIM CONTROLLER]: Angry command detected");
            currentDogState = 6;
        }
        else if (command == "running")
        {
            currentDogState = 4;
        }
        else if (command == "come")
        {
            currentDogState = 4;
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
            currentDogState = 4;
        }
        Debug.Log("wheeeee");
    }
    
    // Set current bot response
    public void SetCurrentBotResponse(string input)
    {
        currentBotResponse = input;
        ParseCommand();
    }
}