#if NORMCORE

using UnityEngine;
using UnityEngine.AI;

namespace Normal.Realtime.Examples {
    public class NormcoreManager : MonoBehaviour {
        [SerializeField] private GameObject _prefab;

        [SerializeField] private Transform _userLocation;
        [SerializeField] private NavManager _navManager;
        [SerializeField] private DogAnimController _dogAnimController;
        [SerializeField] private DogManager _dogManager;
        
        private Realtime _realtime;

        private void Awake() {
            // Get the Realtime component on this game object
            _realtime = GetComponent<Realtime>();

            // Notify us when Realtime successfully connects to the room
            _realtime.didConnectToRoom += DidConnectToRoom;
        }

        private void DidConnectToRoom(Realtime realtime) {
            // Instantiate the CubePlayer for this client once we've successfully connected to the room. Position it 1 meter in the air.
            var options = new Realtime.InstantiateOptions {
                ownedByClient            = true,    // Make sure the RealtimeView on this prefab is owned by this client.
                preventOwnershipTakeover = true,    // Prevent other clients from calling RequestOwnership() on the root RealtimeView.
                useInstance              = realtime // Use the instance of Realtime that fired the didConnectToRoom event.
            };
            // set spawn position to 0.4 meters in front of user
            Vector3 spawnPosition = _userLocation.position + _userLocation.forward * 0.4f;
            GameObject newPuppy = Realtime.Instantiate(_prefab.name, spawnPosition, Quaternion.identity, options);
            _navManager.navMeshAgent = newPuppy.GetComponent<NavMeshAgent>();
            _dogAnimController.audioSource = newPuppy.GetComponent<AudioSource>();
            foreach (Transform child in newPuppy.transform)
            {
                // Check if the child has the desired tag
                if (child.CompareTag("Pet"))
                {
                    _navManager.agentAnimators.Add(child.GetComponent<Animator>());
                    _dogAnimController._animators.Add(child.GetComponent<Animator>());
                    _dogManager.dogs.Add(child.gameObject);
                    if(child.gameObject.activeSelf) _dogManager.currentActiveDog = child.gameObject;
                }
            }
        }
    }
}

#endif