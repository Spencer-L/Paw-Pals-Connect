using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DogAnimController : MonoBehaviour
{
    [SerializeField] private List<Animator> _animators;
    [SerializeField] private Animator pugAnimator;
    
    private string currentBotResponse;
    
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
            // iterate through all animators
            // for(int i = 0; i < _animators.Count; i++)
            // {
            //     Debug.Log("[DOG ANIM CONTROLLER]: iterating animators");
            //     _animators[i].SetInteger("AnimationID", 6);
            //     Debug.Log("[DOG ANIM CONTROLLER] animator integer: " + _animators[i].GetInteger("AnimationID"));
            // }
            pugAnimator.SetTrigger("Bark");
            pugAnimator.SetInteger("AnimationID", 6);
            pugAnimator.Update(0f);
            Debug.Log("[DOG ANIM CONTROLLER] animator integer: " + pugAnimator.GetInteger("AnimationID"));
            var parameters = pugAnimator.parameters;
            foreach (var param in parameters)
            {   
                Debug.Log($"Parameter: {param.name}, Type: {param.type}");
            }
        }
        else if (command == "running")
        {
            foreach (var animator in _animators)
            {
                animator.SetInteger("AnimationID", 4);
            }
    
        }
        else if (command == "come")
        {
            foreach (var animator in _animators)
            {
                animator.SetInteger("AnimationID", 4);
            }
    
        }
        else if (command == "sit down")
        {
            foreach (var animator in _animators)
            {
                animator.SetInteger("AnimationID", 7);
            }
    
        }
        else if (command == "get ball")
        {
            foreach (var animator in _animators)
            {
                animator.SetInteger("AnimationID", 4);
            }
    
        }
        else if (command == "wag tail")
        {
            foreach (var animator in _animators)
            {
                animator.SetInteger("AnimationID", 4);
            }
    
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