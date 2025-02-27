using Mangrove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DemoKitStylizedAnimatedDogs 
{
    public class DemoController : MonoBehaviour
    {
        public GameObject newGOActionController;
        [SerializeField] private List<AnimationButton> _buttons;
        [SerializeField] private List<Animator> _animators;
        private AIClient aiClient;
        private void Start()
        {
            aiClient = newGOActionController.GetComponent<AIClient>();
            aiClient.textInput_.onValueChanged.AddListener(delegate { myTextChanged(); });
            if (aiClient != null)
            {
                Debug.Log("yes breath!!!");
            }
            foreach (var button in _buttons)
            {
                button.Click += OnAnimationButtonClick;
                //  newGOActionController.GetComponent<AIClient>().textInput_.text = "hi";
            }
        }
        public void myTextChanged()
        {
            Debug.Log("Reached");
            string command = aiClient.textInput_.text;
            if (command.Contains("angry"))
            {
                foreach (var animator in _animators)
                {
                    animator.SetInteger("AnimationID", 6);
                }
            }
            else if (command.Contains("running"))
            {
                foreach (var animator in _animators)
                {
                    animator.SetInteger("AnimationID", 4);
                }

            }
            else if (command.Contains("come"))
            {
                foreach (var animator in _animators)
                {
                    animator.SetInteger("AnimationID", 4);
                }

            }
            else if (command.Contains("sit down"))
            {
                foreach (var animator in _animators)
                {
                    animator.SetInteger("AnimationID", 7);
                }

            }
            else if (command.Contains("get ball"))
            {
                foreach (var animator in _animators)
                {
                    animator.SetInteger("AnimationID", 4);
                }

            }
            else if (command.Contains("wag tail"))
            {
                foreach (var animator in _animators)
                {
                    animator.SetInteger("AnimationID", 4);
                }

            }

            Debug.Log("wheeeee");
        }
        // We need this signature in order to directly add and remove this method as 
        // a listener for the onValueChanged event
        public void SwitchAnim()
        {
            _animators[1].SetInteger("AnimationID", 1);

        }

        private void OnAnimationButtonClick(int id)
        {
            foreach(var animator in _animators)
            {
                animator.SetInteger("AnimationID",id);
                Debug.Log("id"+id);

            }
        }
    }
}