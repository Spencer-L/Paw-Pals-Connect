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
        AIClient aiClient;
        private void Start()
    {
             newGOActionController.GetComponent<AIClient>().textInput_.onValueChanged.AddListener(delegate { myTextChanged(); });

            if (newGOActionController.GetComponent<AIClient>())
            {
                if (newGOActionController.GetComponent<AIClient>().textInput_.text == "")
                {
                    Debug.Log("yes breath!!!");
                }
            }
                foreach (var button in _buttons)
       {
          button.Click += OnAnimationButtonClick;
              //  newGOActionController.GetComponent<AIClient>().textInput_.text = "hi";
       }
        }
        public void myTextChanged()
        {
            string command = newGOActionController.GetComponent<AIClient>().textInput_.text;
            if (command == "angry")
            {
                _animators[1].SetInteger("AnimationID", 6);

            }
            else if (command == "running")
            {
                _animators[1].SetInteger("AnimationID", 4);

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
       }
    }
}
}