using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Splines;

public class UFOBehavior : NetworkBehaviour
{
    new public Camera camera;
    public GameObject glint;

    public float vertOffset;

    Spline path;
    Transform target;
    GameObject mothership;

    private float startTime = 0;
    private float abductionStateChangeTime = 0;

    public float abductionPauseDuration = 4;
    public float durationMult = 10;

    private BaseRpcTarget clientTarget;
    private ulong targetClientId;
    private enum State
    {
        Outbound, Abducting, Returning, Idle
    }

    State state = State.Idle;

    void Start()
    {
        camera = Camera.main;

        //StartCoroutine(TestingSendUFO());
    }

    IEnumerator TestingSendUFO()
    {
        yield return new WaitForSeconds(5);
        AcquireTarget(GuyBehavior.activeGuys.RandomEntry().transform);      
    }

    public void AcquireTarget(Transform newTarget)
    {
        if(state == State.Idle)
        {
            ChangeState(State.Outbound);
            startTime = Time.time;
            target = newTarget;
            mothership = Leaderboard.singleton.Orbs.RandomEntry();
        }
    }

    void Update()
    {
        if(mothership == null) mothership = Leaderboard.singleton.Orbs.RandomEntry();
        Vector3 camRelative = camera.transform.position - transform.position;
        glint.transform.parent.rotation = Quaternion.LookRotation(-camRelative);
        glint.transform.localScale = Vector3.one * Mathf.Clamp((camRelative.sqrMagnitude/2000000f) * Mathf.PerlinNoise1D(Time.time * 100f), 0, Mathf.Infinity);

        glint.SetActive(camRelative.sqrMagnitude > 90000);

        if (IsServer)
        {
            if (target != null) 
            {
                if (clientTarget == null) 
                {
                    targetClientId = target.GetComponent<GuyBehavior>().OwnerClientId;
                    clientTarget = RpcTarget.Single(targetClientId, RpcTargetUse.Persistent);
                }
            }

            switch (state)
            {
                case State.Idle:
                    transform.position = mothership.transform.position;
                    break;

                case State.Outbound:

                    if (target != null)
                    {
                        path = GenerateSpline();

                        float timeIndexOut = (Time.time - startTime) / durationMult;
                        float pathProgressOut = -Mathf.Pow(timeIndexOut - 1, 2f) + 1;

                        if (timeIndexOut >= 1)
                        {
                            transform.position = target.position + (Vector3.up * vertOffset);
                            abductionStateChangeTime = Time.time;
                            ChangeState(State.Abducting);
                        }
                        else
                        {
                            transform.position = path.EvaluatePosition(pathProgressOut);
                        }
                    }
                    else
                    {
                        ChangeState(State.Idle);
                    }
                    break;

                case State.Abducting:

                    //target.gameObject.GetComponent<Unity.Multiplayer.Samples.Utilities.ClientAuthority.ClientNetworkTransform>().serverAuth = true;

                    transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);

                    ClientInteractionAbductionProcessRpc(clientTarget);

                    if (Time.time - abductionStateChangeTime >= abductionPauseDuration)
                    {
                        target.parent = transform;
                        ClientInteractionAbductionToReturnRpc(clientTarget);
                        startTime = Time.time;
                        ChangeState(State.Returning);
                        
                        path = GenerateReturnSpline();
                    }

                    break;

                case State.Returning:

                    if (target != null)
                    {
                        ClientInteractionReturnProcessRpc(clientTarget);
                    }

                    float timeIndex = (Time.time - startTime) / durationMult;
                    float pathProgress = -Mathf.Pow((1 - timeIndex) - 1, 2f) + 1;

                    if (timeIndex >= 1.1f)
                    {
                        //target.gameObject.GetComponent<Unity.Multiplayer.Samples.Utilities.ClientAuthority.ClientNetworkTransform>().serverAuth = false;
                        if (target != null)
                        {
                            ClientInteractionFinishsRpc(clientTarget);
                        }

                        NetworkObject.Despawn();
                    }
                    else
                    {
                        transform.position = path.EvaluatePosition(pathProgress);
                    }
                    break;
            }
        }
    }

    private void ChangeState(State newState)
    {
        state = newState;
    }

    private Spline GenerateSpline()
    {
        Vector3 shadowVector = (mothership.transform.position - camera.transform.position).normalized;
        Vector3 startPos = mothership.transform.position + (shadowVector * 500);
        Vector3 endPos = target.position + (Vector3.up * vertOffset);
        Vector3 midPos = ((startPos + endPos) / 2f) + (-Vector3.right * startPos.magnitude / 5f);
        Vector3 approachPos = endPos + (midPos - endPos).normalized * 20;
        approachPos.y = endPos.y;

        BezierKnot startKnot = new BezierKnot(startPos, Quaternion.AngleAxis(90, Vector3.up) * shadowVector, Quaternion.AngleAxis(90, Vector3.up) * shadowVector);
        BezierKnot midKnot = new BezierKnot(midPos, shadowVector * 2000, -shadowVector * 2000);
        BezierKnot endKnot = new BezierKnot(endPos, Quaternion.AngleAxis(-90, Vector3.up) * shadowVector, Quaternion.AngleAxis(-90, Vector3.up) * shadowVector);

        Spline toReturn = new Spline(4, false);
        toReturn.Add(startKnot);
        toReturn.Add(midKnot);
        toReturn.Add(new BezierKnot(approachPos), TangentMode.AutoSmooth);
        toReturn.Add(endKnot);
        return toReturn;
    }

    private Spline GenerateReturnSpline()
    {
        Vector3 shadowVector = (mothership.transform.position - camera.transform.position).normalized;
        Vector3 startPos = mothership.transform.position + (shadowVector * 500);
        Vector3 endPos = transform.position;
        Vector3 midPos = ((startPos + endPos) / 2f) + (-Vector3.right * startPos.magnitude / 5f);
        Vector3 approachPos = endPos + (midPos - endPos).normalized * 20;
        approachPos.y = endPos.y;

        BezierKnot startKnot = new BezierKnot(startPos, Quaternion.AngleAxis(90, Vector3.up) * shadowVector, Quaternion.AngleAxis(90, Vector3.up) * shadowVector);
        BezierKnot midKnot = new BezierKnot(midPos, shadowVector * 2000, -shadowVector * 2000);
        BezierKnot endKnot = new BezierKnot(endPos, Quaternion.AngleAxis(-90, Vector3.up) * shadowVector, Quaternion.AngleAxis(-90, Vector3.up) * shadowVector);

        Spline toReturn = new Spline(4, false);
        toReturn.Add(startKnot);
        toReturn.Add(midKnot);
        toReturn.Add(new BezierKnot(approachPos), TangentMode.AutoSmooth);
        toReturn.Add(endKnot);
        return toReturn;
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void ClientInteractionAbductionProcessRpc(RpcParams rpcParams = default)
    {
        Transform targetPass = NetworkManager.LocalClient.PlayerObject.transform;
        targetPass.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - targetPass.position), ForceMode.Acceleration);
        targetPass.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ClientInteractionAbductionToReturnRpc(RpcParams rpcParams = default)
    {
        Transform targetPass = NetworkManager.LocalClient.PlayerObject.transform;
        targetPass.GetComponent<Rigidbody>().isKinematic = true;
        targetPass.gameObject.GetComponent<GuyBehavior>().enabled = false;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ClientInteractionReturnProcessRpc(RpcParams rpcParams = default)
    {
        Transform targetPass = NetworkManager.LocalClient.PlayerObject.transform;
        //targetPass.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - target.position) * 2, ForceMode.VelocityChange);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ClientInteractionFinishsRpc(RpcParams rpcParams = default)
    {
        Transform targetPass = NetworkManager.LocalClient.PlayerObject.transform;
        targetPass.parent = null;
        targetPass.gameObject.GetComponent<GuyBehavior>().enabled = true;
        targetPass.gameObject.GetComponent<GuyBehavior>().KillMeRpc();
    }
}
