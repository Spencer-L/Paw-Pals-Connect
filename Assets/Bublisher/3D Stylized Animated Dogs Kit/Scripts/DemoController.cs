using Mangrove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoKitStylizedAnimatedDogs 
{
public class DemoController : MonoBehaviour
{
        public GameObject thing;
    [SerializeField] private List<AnimationButton> _buttons;
    [SerializeField] private List<Animator> _animators;
        AIClient aiClient;
        private void Start()
    {
            if (thing.GetComponent<AIClient>())
            {
                if(thing.GetComponent<AIClient>().currState=="")
                Debug.Log("yes breath!!!");
            }
       foreach (var button in _buttons)
       {
          button.Click += OnAnimationButtonClick;
       }
    }
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