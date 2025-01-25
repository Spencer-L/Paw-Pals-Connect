using System;
using UnityEngine;
using UnityEngine.Events;

namespace PolySpatial.Samples
{
    public class SpatialUIButton : SpatialUI
    {
        public Action<string, MeshRenderer> WasPressed;

        public string ButtonText => m_ButtonText;

        [SerializeField] private UnityEvent toInvoke;
        public MeshRenderer MeshRenderer => m_MeshRenderer;

        [SerializeField]
        string m_ButtonText;

        MeshRenderer m_MeshRenderer;

        void OnEnable()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        public override void Press(Vector3 position)
        {
            base.Press(position);

            if (WasPressed != null)
            {
                WasPressed.Invoke(m_ButtonText, m_MeshRenderer);
            
            }
        }
    }
}
