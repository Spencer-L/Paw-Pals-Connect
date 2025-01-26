using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DogAnimController : MonoBehaviour
{
    [SerializeField] private NavManager navManager;
    
    private static readonly int AnimationID = Animator.StringToHash("AnimationID");
    public List<Animator> _animators;
    public AudioSource audioSource;
    
    [SerializeField] private List<AudioClip> audioClips;
    
    private int currentDogState = 0;
    
    private string currentBotResponse;
    private bool needUpdate;

    private void Update()
    {
        foreach (var animator in _animators)
        {
            // Debug.Log($"Setting AnimationID to {currentDogState} for animator {animator.name}");
            animator.SetInteger(AnimationID, currentDogState);
        }

        if (needUpdate)
        {
            if (currentDogState == 6)
            {
                // navManager.StopCurrentNavigation();
            }
            else if (currentDogState == 4)
            {
                navManager.CallPuppy();
            }
            else if (currentDogState == 2)
            {
                navManager.CallPuppy();
            }

            needUpdate = false;
        }
    }
    int  counterAngry=0;
    public void ParseCommand()
    {
        needUpdate = true;
        // Use regex to parse command ie. ["sit down"]
        string pattern = @"\[\s*\""(.*?)\""\s*\]";
        MatchCollection matches = Regex.Matches(currentBotResponse, pattern);
        string command = "";
        foreach (Match match in matches)
        {
            // Extract the content inside ["..."]
            command = match.Groups[1].Value;
        }
        if (command == "angry")
        {
            currentDogState = 6;
            Debug.Log("[Dog Anim Controller]: angry");
            // navManager.StopCurrentNavigation();
            audioSource.PlayOneShot(audioClips[0]);
        }
        else if (command == "running")
        {
            currentDogState = 4;
            int random = UnityEngine.Random.Range(1, 3);
            if(random == 1) audioSource.PlayOneShot(audioClips[1]);
            else if(random == 2) audioSource.PlayOneShot(audioClips[4]);
            else if(random == 3) audioSource.PlayOneShot(audioClips[5]);
            navManager.CallPuppy();
        }
        else if (command == "come")
        {
            currentDogState = 4;
            // play a random sound from the list (either 2, 4, or 5)
            int random = UnityEngine.Random.Range(1, 3);
            if(random == 1) audioSource.PlayOneShot(audioClips[1]);
            else if(random == 2) audioSource.PlayOneShot(audioClips[4]);
            else if(random == 3) audioSource.PlayOneShot(audioClips[5]);
            navManager.MoveAgentToNearestSeatPlane();
        }
        else if (command == "sit down")
        {
            currentDogState = 7;
            int random = UnityEngine.Random.Range(1, 3);
            if(random == 1) audioSource.PlayOneShot(audioClips[1]);
            else if(random == 2) audioSource.PlayOneShot(audioClips[4]);
            else if(random == 3) audioSource.PlayOneShot(audioClips[5]);
            // navManager.StopCurrentNavigation();
        }
        else if (command == "get ball")
        {
            currentDogState = 4;
        }
        else if (command == "wag tail")
        {
            int random = UnityEngine.Random.Range(1, 3);
            if(random == 1) audioSource.PlayOneShot(audioClips[1]);
            else if(random == 2) audioSource.PlayOneShot(audioClips[4]);
            else if(random == 3) audioSource.PlayOneShot(audioClips[5]);
            // navManager.StopCurrentNavigation();
            currentDogState = 1;
        }
    }
    
    // Set current bot response
    public void SetCurrentBotResponse(string input)
    {
        currentBotResponse = input;
        ParseCommand();
    }
}