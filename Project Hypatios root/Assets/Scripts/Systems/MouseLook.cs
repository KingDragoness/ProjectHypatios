using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
    public bool useDeltaTime = false;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";


    void Start()
    {

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Update ()
	{
		if (Input.GetKey(KeyCode.F)) return;
		if (axes == RotationAxes.MouseXAndY)
		{
            float _strengthX = sensitivityX;
            float _strengthY = sensitivityY;

            if (useDeltaTime)
            {
                _strengthX = Time.deltaTime * _strengthX * 20;
                _strengthY = Time.deltaTime * _strengthY * 20;
            }

            float rotationX = transform.localEulerAngles.y + GetInputAxisRight() * _strengthX;
			rotationY += GetInputAxisUp() * _strengthY;

            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, GetInputAxisRight() * sensitivityX, 0);
		}
		else
		{
			rotationY += GetInputAxisUp() * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
	
    private float GetInputAxisRight()
    {
        float totalLookAxisRight;

        float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
        //float gamepadLookAxisRight = Input.GetAxisRaw("Look X") * 0.5f;
        //gamepadLookAxisRight = Mathf.Clamp(gamepadLookAxisRight, -1, 1);
        totalLookAxisRight = mouseLookAxisRight;

        return totalLookAxisRight;
    }

    private float GetInputAxisUp()
    {
        float totalLookAxisUp;

        float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
        //float gamepadLookAxisUp = player.GetAxisRaw("Look Y") * 0.5f;
        //gamepadLookAxisUp = Mathf.Clamp(gamepadLookAxisUp, -1, 1);
        totalLookAxisUp = mouseLookAxisUp;

        return totalLookAxisUp;
    }

}