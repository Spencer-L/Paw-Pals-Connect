using System;
using System.Collections;
using System.Collections.Generic;
using PolySpatial.Samples;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

// #if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// #endif

public class NavManager : MonoBehaviour
{
    // Dictionary of the GameObject meshes and their NavMeshSurfaces that are attached as components
    public Dictionary<GameObject, NavMeshSurface> navigableObjects = new();
    private bool isUpdating = false;
    private string DEBUG_TAG = "[NAV MANAGER]: ";
    public ApplicationReferences appRef;
    [SerializeField] private Transform groundDetectorOrigin;
    [SerializeField] public NavMeshAgent navMeshAgent;
    public List<Animator> agentAnimators;
    public GameObject navNode;
    public GameObject debugNode;
    public GameObject classifiedDebugNode;
    private static readonly int IsWalking = Animator.StringToHash("isRunning");
    private static readonly int DoJump = Animator.StringToHash("doJump");

    private Vector3 lastFloorPos;
    private bool wantToSit = false;
    private Vector3 seatPos = Vector3.zero;
    
    private static readonly int AnimationID = Animator.StringToHash("AnimationID");

    enum NavState
    {
        Standing,       // idling on floor
        Walking,        // walking on floor
        IsJumping,      // jumping to or from seat
        Seated          // idling on chair
    }

    NavState navState = NavState.Standing;


    private bool DEBUG_SEAT = false;
    private int DEBUG_COUNT = 0;
    private bool DEBUG_MESH = false;
    private bool DEBUG_DISPLAY_MESH = false;

    private bool IVA_DEMO = false;
    private int IVA_STEP = 0;

    public void FixedUpdate()
    {
        // // find all game objects that use the Navigable tag
        //     // mesh will be generated from prefab with "Navigable" tag
        // navigableObjects = GameObject.FindGameObjectsWithTag("Navigable");

        // perform a raycast from groundDectorOrigin to the ground and add navigable
        RaycastHit hit;
        if (Physics.Raycast(groundDetectorOrigin.position, Vector3.down, out hit, 100f))
        {
            if (hit.collider.gameObject.CompareTag("Navigable"))
            {
                if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "Navigable object found: " + hit.collider.gameObject.name);
                AddNavigable(hit.collider.gameObject);
            }
            else
            {
                if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "Collider hit, but no navigable object tag: " + hit.collider.gameObject.tag);
            }
        }
        else
        {
            if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "No navigable object found");
        }

        if (navigableObjects.Count <= 0)
        {
            if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "No navigable objects have been added.");
        }

        foreach (KeyValuePair<GameObject, NavMeshSurface> pair in navigableObjects)
        {
            if (isUpdating == false)
            {
                StartCoroutine(UpdateNavMesh(pair.Value));
            }
        }



        if(navState == NavState.Walking && Vector3.Distance(navMeshAgent.gameObject.transform.position, navMeshAgent.destination) <= 0.5f)
        {
            Debug.Log(DEBUG_TAG + "Position: " + navMeshAgent.gameObject.transform.position);
            Debug.Log(DEBUG_TAG + "Destination: " + navMeshAgent.destination);
            Debug.Log(DEBUG_TAG + "Reached destination");
            navMeshAgent.ResetPath();
            
            foreach (var animator in agentAnimators)
            {
                animator.SetInteger(AnimationID, 0);
            }
            
            if (wantToSit && Vector3.Distance(navMeshAgent.gameObject.transform.position, seatPos) <= 1.0f)
            {
                wantToSit = false;
                IVA_DEMO = false;

                Debug.Log(DEBUG_TAG + "Seating animation to position: " + seatPos);
                StartCoroutine(MoveAgentUpToSeat(seatPos));
            }
            else if (wantToSit)
            {
                IVA_STEP = 0;
                // want to sit, but not at seat yet... keep trying!
                Debug.Log(DEBUG_TAG + "Want to sit, but not at seat yet :( Not trying again.");
                Debug.Log(DEBUG_TAG + "Position: " + navMeshAgent.gameObject.transform.position);
                Debug.Log(DEBUG_TAG + "Seat Pos: " + seatPos);
                Debug.Log(DEBUG_TAG + "Distance: " + Vector3.Distance(navMeshAgent.gameObject.transform.position, seatPos));
                // MoveAgentToNearestSeatMesh();
                wantToSit = false;
                navState = NavState.Standing;
            }
            else
            {
                navState = NavState.Standing;

                StartCoroutine(TurnToCamera());
            }
        }

    }

    /**
     * Add a navigable object to the list of navigable objects
     * Used in InputDebug.cs for adding meshes that are tapped (user selects the floor meshes)
     */
    public void AddNavigable(GameObject n)
    {
        if (navigableObjects.ContainsKey(n) == false)
        {
            if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "Adding navigable object: " + n.ToString());
            NavMeshSurface navMeshSurface = n.AddComponent<NavMeshSurface>();
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            navigableObjects.Add(n, navMeshSurface);
        } else {
            if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "Navigable object already exists: " + n.ToString());
        }
    }

    private IEnumerator UpdateNavMesh(NavMeshSurface navMeshSurface)
    {
        if (navMeshSurface != null)
        {
            isUpdating = true;
            if (DEBUG_MESH) Debug.Log(DEBUG_TAG + "Updating NavMeshSurface");
            navMeshSurface.BuildNavMesh();
            yield return new WaitForSeconds(1f);
            isUpdating = false;
        } else {
            Debug.Log(DEBUG_TAG + "NavMeshSurface is null");
        }
    }

    public void MakeNavigate(GameObject destNode) {
        // TODO: assumes that navagent is on floor!!!

        if (destNode != null)
        {
            Vector3 destPos = destNode.transform.position;
            Debug.Log(DEBUG_TAG + "Navigating to: " + destPos);
            // move the user to the selected position
            navMeshAgent.SetDestination(destPos);
            navNode = destNode;

            // animations
            foreach (var animator in agentAnimators)
            {
                animator.SetInteger(AnimationID, 4);
            }
            navState = NavState.Walking;

            if (navMeshAgent.gameObject.GetComponent<Animation>() != null) {
                navMeshAgent.gameObject.GetComponent<Animation>().Play("Walking");
            }
        }
        else
        {
            Debug.Log(DEBUG_TAG + "No navigation position found");
        }
    }

    public void MakeNavigate(Vector3 destCoords) {
        // TODO: assumes that navagent is on the floor!!!!
        // if (navState == NavState.Seated)
        // {
        //     StartCoroutine(MoveAgentDownFromSeatThenMove(destCoords));
        //     return;
        // }

        Vector3 destPos = new Vector3(destCoords.x, 0, destCoords.z);
        if (wantToSit)
        {
            if (DEBUG_SEAT) Debug.Log(DEBUG_TAG + "Remembering seat height to: " + destCoords.y);
            seatPos = destCoords;
        }
        Debug.Log(DEBUG_TAG + "Navigating to: " + destPos);
        // move the user to the selected position
        navMeshAgent.SetDestination(destPos);
        if(DEBUG_DISPLAY_MESH && debugNode) Instantiate(debugNode, destPos, Quaternion.identity);

        // animations
        foreach (var animator in agentAnimators)
        {
            animator.SetInteger(AnimationID, 4);
        }
        navState = NavState.Walking;

        if (DEBUG_SEAT) Debug.Log(DEBUG_TAG + "Puppy has set destination: " + navMeshAgent.destination + " and is pending path?: " + navMeshAgent.pathPending);
    }

    public void CallPuppy() {
        if (navState == NavState.IsJumping)
        {
            Debug.Log(DEBUG_TAG + "Ignoring call to puppy while jumping");
            return;
        }
        Debug.Log(DEBUG_TAG + "Calling the puppy to you");
        GameObject yourPos = new GameObject();
        yourPos.transform.position = appRef.camTrans.position;
        Debug.Log(DEBUG_TAG + "Your position: " + yourPos.transform.position);
        if (navState == NavState.Seated)
        {
            Debug.Log(DEBUG_TAG + "Puppy is seated, delaying navigation until after finishing jumping, then headed to position: " + yourPos.transform.position);
            StartCoroutine(MoveAgentDownFromSeatThenMove(yourPos.transform.position));
            return;
        }
        else
        {
            MakeNavigate(yourPos.transform.position);
            Debug.Log(DEBUG_TAG + "Puppy is on the way!");
        }
    }
    public void MoveAgentToNearestSeatMesh() {
        Debug.Log(DEBUG_TAG + "Calling the puppy to seat mesh");
        var seats = GameObject.FindGameObjectsWithTag("Seat");
        Debug.Log(DEBUG_TAG + "Number of seats: " + seats.Length);
        // find the nearest seat in seats
        GameObject nearestSeat = null;
        float minDistance = Mathf.Infinity;
        Vector3 targetPos = Vector3.zero;
        foreach (GameObject seat in seats)
        {
            Vector3 seatPos = seat.transform.GetComponent<MeshRenderer>().bounds.center;
            Debug.Log(DEBUG_TAG + "Seat position: " + seatPos);
            float distance = Vector3.Distance(seatPos, appRef.camTrans.position);
            if (distance < minDistance)
            {
                nearestSeat = seat;
                targetPos = seatPos;
                minDistance = distance;
            }
        }
        if(nearestSeat) Debug.Log(DEBUG_TAG + "Navigating to seat at position " + targetPos);
        Debug.Log(DEBUG_TAG + "Your position: " + appRef.camTrans.position);
        MakeNavigate(targetPos);
        Debug.Log(DEBUG_TAG + "Puppy is on the way!");
    }

    public void MoveAgentToNearestSeatPlane() {
        if (navState == NavState.IsJumping)
        {
            Debug.Log(DEBUG_TAG + "Ignoring call to puppy while jumping");
            return;
        }
        Debug.Log(DEBUG_TAG + "Calling the puppy to seat plane");
        var planes = GameObject.FindGameObjectsWithTag("Plane");
        Debug.Log(DEBUG_TAG + "Number of planes: " + planes.Length);
        // find the nearest seat in seats
        GameObject nearestSeat = null;
        float minDistance = Mathf.Infinity;
        Vector3 targetPos = Vector3.zero;
        foreach (GameObject plane in planes)
        {
            // Debug.Log("Plane classification: " + plane.GetComponent<ARPlane>().classifications);
            if (plane.GetComponent<ARPlane>().classifications == PlaneClassifications.SeatOfAnyType ||
                plane.GetComponent<ARPlane>().classifications == PlaneClassifications.Seat ||
                plane.GetComponent<ARPlane>().classifications == PlaneClassifications.Couch)
            {
                // Debug.Log("Seat Alignment: " + plane.GetComponent<ARPlane>().alignment);
                // Debug.Log("Seat is Horizontal Up");
                Vector3 seatPos = plane.transform.position;
                Debug.Log(DEBUG_TAG + "Seat plane position: " + seatPos);
                float distance = Vector3.Distance(seatPos, appRef.camTrans.position);
                if (distance < minDistance)
                {
                    nearestSeat = plane;
                    targetPos = seatPos;
                    minDistance = distance;
                }
            }
        }

        if (nearestSeat || DEBUG_SEAT)
        {
            Debug.Log(DEBUG_TAG + "Navigating to seat plane at position " + targetPos);
            wantToSit = true;
        }
        else
        {
            Debug.Log(DEBUG_TAG + "No seat plane found. Calling puppy to you instead.");
            CallPuppy();
            if (IVA_DEMO) IVA_STEP = 0;
            return;
        }

        if (DEBUG_SEAT)
        {
            Debug.Log(DEBUG_TAG + "DEBUG MODE: Using default seat position 1");
            if (DEBUG_COUNT == 0)
            {
                targetPos = new Vector3(0.5f, 0.6f, 0.5f);
            }
            else
            {
                targetPos = new Vector3(-0.5f, 0.6f, -0.5f);
            }
        }
        Debug.Log(DEBUG_TAG + "Your position: " + appRef.camTrans.position);
        if (navState == NavState.Seated)
        {
            Debug.Log(DEBUG_TAG + "Puppy is seated, delaying navigation until after finishing jumping, then headed to pos: " + targetPos);
            StartCoroutine(MoveAgentDownFromSeatThenMove(targetPos));
        }
        else
        {
            Debug.Log(DEBUG_TAG + "Puppy is not seated, calling MakeNavigate to seat plane at position " + targetPos);
            MakeNavigate(targetPos);
            Debug.Log(DEBUG_TAG + "Puppy is on the way!");
        }
    }

    private IEnumerator MoveAgentUpToSeat(Vector3 seatPos)
    {
        // agentAnimator.SetTrigger(DoJump);
        navState = NavState.IsJumping;

        lastFloorPos = navMeshAgent.gameObject.transform.position;
        navMeshAgent.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        yield return new WaitForSeconds(0.5f);

        if (DEBUG_SEAT)
        {
            Debug.Log(DEBUG_TAG + "DEBUG MODE: Using default seat position 2");
            if (DEBUG_COUNT == 0)
            {
                seatPos = new Vector3(0.5f, 0.6f, 0.5f);
                DEBUG_COUNT++;
            }
            else
            {
                seatPos = new Vector3(-0.5f, 0.6f, -0.5f);
                DEBUG_COUNT = 0;
            }
        }


        Vector3 startPosition = navMeshAgent.gameObject.transform.position;
        Debug.Log("Start position: " + startPosition);
        Vector3 targetPosition = seatPos;

        var stepSize = 0.9f;
        var arcHeight = 0.5f;
        arcHeight = arcHeight * Vector3.Distance(startPosition, targetPosition);

        var objectToMove = navMeshAgent.gameObject.transform;

        var progress = 0f;
        var arrived = false;
        while (!arrived)
        {
            // Increment our progress from 0 at the start, to 1 when we arrive.
            progress = Mathf.Min(progress + Time.fixedDeltaTime * stepSize, 1.0f);

            // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
            float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);

            // Travel in a straight line from our start position to the target.
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, progress);

            // Then add a vertical arc in excess of this.
            nextPos.y += parabola * arcHeight;

            // Continue as before.
            objectToMove.LookAt(new Vector3(nextPos.x, objectToMove.position.y, nextPos.z), Vector3.up);
            objectToMove.position = nextPos;

            // yield return new WaitForSeconds(0.005f);
            yield return new WaitForFixedUpdate();

            if (progress == 1.0f)
            {
                arrived = true;
            }
        }

        Debug.Log(DEBUG_TAG + "Done?");

        navState = NavState.Seated;

        // turn around to face the camera
        float dur = 0.8f;
        Quaternion start = navMeshAgent.gameObject.transform.rotation;
        Quaternion end = Quaternion.LookRotation(new Vector3(appRef.camTrans.position.x, navMeshAgent.gameObject.transform.position.y, appRef.camTrans.position.z) - navMeshAgent.gameObject.transform.position);
        float rotationTime = 0f;
        while (rotationTime < dur)
        {
            navMeshAgent.gameObject.transform.rotation = Quaternion.Slerp(start, end, rotationTime / dur);
            yield return null;
            rotationTime += Time.fixedDeltaTime;
        }


        // Vector3 startPos = navMeshAgent.gameObject.transform.position;
        // Vector3 targetPos = new Vector3(appRef.camTrans.position.x, navMeshAgent.gameObject.transform.position.y, appRef.camTrans.position.z);
        //
        // float temp = 0.0f;
        // float speed = 0.01f;
        // while (navMeshAgent.gameObject.transform.rotation.eulerAngles.y < appRef.camTrans.rotation.eulerAngles.y - 180.0f + 0.01f &&
        //        navMeshAgent.gameObject.transform.rotation.eulerAngles.y > appRef.camTrans.rotation.eulerAngles.y - 180.0f - 0.01f)
        // {
        //     navMeshAgent.gameObject.transform.LookAt(Vector3.Lerp(startPos, targetPos, speed * temp));
        //     temp += Time.fixedDeltaTime;
        //     // navMeshAgent.gameObject.transform.LookAt(new Vector3(appRef.camTrans.position.x, navMeshAgent.gameObject.transform.position.y, appRef.camTrans.position.z));
        //     yield return new WaitForFixedUpdate();
        // }


        yield return null;
    }

    private IEnumerator MoveAgentDownFromSeatThenMove(Vector3 destPos)
    {
        // agentAnimator.SetTrigger(DoJump);
        navState = NavState.IsJumping;

        // jump down to lastFloorPos
        var startPosition = navMeshAgent.gameObject.transform.position;
        var targetPosition = lastFloorPos;

        var stepSize = 0.9f;
        var arcHeight = 0.5f;
        arcHeight = arcHeight * Vector3.Distance(startPosition, targetPosition);

        var objectToMove = navMeshAgent.gameObject.transform;

        var progress = 0f;
        var arrived = false;
        while (!arrived)
        {
            // Increment our progress from 0 at the start, to 1 when we arrive.
            progress = Mathf.Min(progress + Time.fixedDeltaTime * stepSize, 1.0f);

            // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
            float parabola = 1.0f - 4.0f * (progress - 0.5f) * (progress - 0.5f);

            // Travel in a straight line from our start position to the target.
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, progress);

            // Then add a vertical arc in excess of this.
            nextPos.y += parabola * arcHeight;

            // Continue as before.
            objectToMove.LookAt(new Vector3(nextPos.x, objectToMove.position.y, nextPos.z), Vector3.up);
            objectToMove.position = nextPos;

            // yield return new WaitForSeconds(0.005f);
            yield return new WaitForFixedUpdate();

            if (progress == 1.0f)
            {
                arrived = true;
            }
        }

        navMeshAgent.enabled = true;

        // skelly boy needs rest
        yield return new WaitForSeconds(0.5f);

        MakeNavigate(destPos);

        yield return null;
    }

    public void ResetIVA()
    {
        IVA_DEMO = true;
        IVA_STEP = 0;
        StartCoroutine(IVACoroutine());
        if (!DEBUG_DISPLAY_MESH)
        {
            // get game object by name
            GameObject panel = GameObject.Find("Spatial Panel Scroll");
            // disable panel
            panel.SetActive(false);
        }
    }

    private IEnumerator IVACoroutine()
    {
        yield return new WaitForSecondsRealtime(8.0f);
        while (IVA_DEMO)
        {
            Debug.Log(DEBUG_TAG + "IVA Demo: " + IVA_STEP);
            if (navState == NavState.Standing)
            {
                if (IVA_STEP == 0)
                {
                    // continuously call puppy until it's near camera

                    if (Vector3.Distance(navMeshAgent.gameObject.transform.position, new Vector3(appRef.camTrans.position.x, 0, appRef.camTrans.position.z)) <= 1.0f)
                    {
                        Debug.Log(DEBUG_TAG + "IVA DEMO: Puppy is near camera, moving to seat");
                        IVA_STEP++;
                    }
                    else
                    {
                        Debug.Log(DEBUG_TAG + "IVA DEMO: Puppy is not near camera, calling puppy to camera. Distance: " + Vector3.Distance(navMeshAgent.gameObject.transform.position, new Vector3(appRef.camTrans.position.x, 0, appRef.camTrans.position.z)));
                        CallPuppy();
                    }
                } else if (IVA_STEP == 1)
                {
                    // call puppy to seat
                    MoveAgentToNearestSeatPlane();
                    IVA_STEP++;
                }
            }

            yield return new WaitForSecondsRealtime(3.0f);
        }
        yield return null;
    }

    private IEnumerator TurnToCamera()
    {
        // turn around to face the camera
        float dur = 0.8f;
        Quaternion start = navMeshAgent.gameObject.transform.rotation;
        Quaternion end = Quaternion.LookRotation(new Vector3(appRef.camTrans.position.x, navMeshAgent.gameObject.transform.position.y, appRef.camTrans.position.z) - navMeshAgent.gameObject.transform.position);
        float rotationTime = 0f;
        while (rotationTime < dur)
        {
            navMeshAgent.gameObject.transform.rotation = Quaternion.Slerp(start, end, rotationTime / dur);
            yield return null;
            rotationTime += Time.fixedDeltaTime;
        }
    }

    // public void StopCurrentNavigation()
    // {
    //     Debug.Log("Stopping current navigation");
    //     navMeshAgent.ResetPath();
    // }
}
