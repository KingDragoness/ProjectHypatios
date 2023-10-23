using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TriviaMapCamera : MonoBehaviour
{
    public Camera cam;
    public Vector3 maximumExtend;
    public Vector3 center;
    public MouseLook mouselook;
    public float damping = 5f;
    public float x, y;
    public float PanSpeed = 20f; public float PanSpeedKey = 20f;
    [FoldoutGroup("Raycast")] public LayerMask triviaMapLayermask;

    public Transform baitTransformCam;
    public TriviaMapUI triviaScript;

    private float xMovement;
    private float yMovement;
    private bool enableLook = false;
    private bool isMouse = false;
    private bool isKeyboard = false;
    private Vector3 lastPanPosition;
    private Vector3 _cameraTargetPos = Vector3.zero;

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.5f);
        Gizmos.DrawWireCube(center, maximumExtend);

    }

    public void OverrideCameraTargetPos(Vector3 _newPos)
    {
        _cameraTargetPos = _newPos;
    }

    private void Update()
    {
        isMouse = false;
        isKeyboard = false;
        var step = damping * Time.unscaledDeltaTime;

        HandleKeyBoard();
        HandleMouse();
        CorrectPosition();
        RaycastTrivia();

        float distVector = Vector3.Distance(transform.position, _cameraTargetPos);
        distVector /= 5f;
        distVector = Mathf.Clamp(distVector, 0.1f, 10f);
        transform.position = Vector3.MoveTowards(transform.position, _cameraTargetPos, step * distVector); //damping

    }


    private void RaycastTrivia()
    {
        bool noBall = true;
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 88f, triviaMapLayermask))
        {
            var interactable = hit.collider.GetComponent<TriviaBallButton>();

            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<TriviaBallButton>();
            }

            if (interactable != null)
            {
                triviaScript.currentHoveredBall = interactable;
                noBall = false;
            }
        }

        if (noBall)
        {
            triviaScript.currentHoveredBall = null;
        }
    }

    private void HandleMouse()
    {
        //Handle mouselook
        {
            if (Input.GetMouseButton(1))
            {
                enableLook = true;
            }
            else
            {
                enableLook = false;
            }

            if (enableLook)
            {
                mouselook.disableInput = false;
            }
            else
            {
                mouselook.disableInput = true;
            }
        }


        if (isKeyboard == true) return; //prevent bug
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            isMouse = true;
            PanCamera(Input.mousePosition);
        }


    }

    public void ManualIntervention_Mouse()
    {
        lastPanPosition = Input.mousePosition;
        //PanCamera(Input.mousePosition);
        mouselook.ExecuteFunction();

    }

    private bool safetyBoolKeyboard = false;

    private void HandleKeyBoard()
    {
        Vector3 move3DVector = new Vector3();
        var moveVector = Hypatios.Input.Move.ReadValue<Vector2>();
        xMovement = moveVector.x;
        yMovement = moveVector.y;
        move3DVector.x = xMovement;
        move3DVector.y = yMovement * 0.5f;
        move3DVector.z = yMovement;

        if (Mathf.Abs(xMovement) > 0f | Mathf.Abs(yMovement) > 0f)
        {
            if (safetyBoolKeyboard == false)
            {
                lastPanPosition = move3DVector * PanSpeedKey;
                safetyBoolKeyboard = true;
            }

            PanCameraByKeyboard(move3DVector * PanSpeedKey);
            isKeyboard = true;
        }
        else
        {
            safetyBoolKeyboard = false;

        }

    }

    void CorrectPosition()
    {
        Vector3 correctedPosition = _cameraTargetPos;

        correctedPosition.x = Mathf.Clamp(correctedPosition.x, center.x - maximumExtend.x, center.x + maximumExtend.x);
        correctedPosition.y = Mathf.Clamp(correctedPosition.y, center.y - maximumExtend.y, center.y + maximumExtend.y);
        correctedPosition.z = Mathf.Clamp(correctedPosition.z, center.z - maximumExtend.z, center.z + maximumExtend.z);


        _cameraTargetPos = correctedPosition;
    }

    void PanCamera(Vector3 newPanPosition)
    {
        float panSpeedAdjusted = PanSpeed;

        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * panSpeedAdjusted, offset.y * panSpeedAdjusted, offset.y * panSpeedAdjusted);

        Vector3 eulerCam;
        eulerCam.x = 0;
        eulerCam.y = transform.eulerAngles.y;
        eulerCam.z = 0;
        baitTransformCam.eulerAngles = eulerCam;

        // Perform the movement
        //transform.Translate(move, baitTransformCam);
        _cameraTargetPos += baitTransformCam.TransformDirection(move);

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void PanCameraByKeyboard(Vector3 newPanPosition)
    {
        float panSpeedAdjusted = PanSpeed;

        // Determine how much to move the camera
        Vector3 move = lastPanPosition + newPanPosition;

        Vector3 eulerCam;
        eulerCam.x = 0;
        eulerCam.y = transform.eulerAngles.y;
        eulerCam.z = 0;
        baitTransformCam.eulerAngles = eulerCam;

        // Perform the movement
        //transform.Translate(move, baitTransformCam);
        _cameraTargetPos += baitTransformCam.TransformDirection(move);

        // Cache the position
        lastPanPosition = newPanPosition;
    }
}
