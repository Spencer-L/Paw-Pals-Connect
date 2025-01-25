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
        public GameObject thing;
    [SerializeField] private List<AnimationButton> _buttons;
    [SerializeField] private List<Animator> _animators;
        AIClient aiClient;
      //  public InputField textInput_;

        private void Start()
    {
            thing.GetComponent<AIClient>().textInput_.onValueChanged.AddListener(delegate { myTextChanged(); });

            if (thing.GetComponent<AIClient>())
            {
              //  if(thing.GetComponent<AIClient>().textInput_.text== "")
                    if (thing.GetComponent<AIClient>().currState == "")
                        Debug.Log("yes breath!!!");
            }
       foreach (var button in _buttons)
       {
          button.Click += OnAnimationButtonClick;
       }
            //thing.onValueChanged.AddListener(myTextChanged);
        }
        public void myTextChanged()
        {
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