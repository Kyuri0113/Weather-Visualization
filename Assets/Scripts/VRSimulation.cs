using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSimulation : MonoBehaviour
{
    private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    private RotationAxes axes = RotationAxes.MouseXAndY;
    private float sensitivityX = 15F;
    private float sensitivityY = 15F;
    private float minimumX = -360F;
    private float maximumX = 360F;
    private float minimumY = -60F;
    private float maximumY = 60F;
    private float rotationX = 0F;
    private float rotationY = 0F;
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        UpdateMovement();
        UpdateMouseLook();
    }

    private void UpdateMovement()
    {
        float speed = 50.0f * Time.deltaTime;

        if (Input.GetKey(KeyCode.W)) {
            Camera.main.transform.Translate(0.0f, 0.0f, speed);
        } else if (Input.GetKey(KeyCode.S)) {
            Camera.main.transform.Translate(0.0f, 0.0f, -speed);
        }
        if (Input.GetKey(KeyCode.A)) {
            Camera.main.transform.Translate(-speed, 0.0f, 0.0f);
        } else if (Input.GetKey(KeyCode.D)) {
            Camera.main.transform.Translate(speed, 0.0f, 0.0f);
        }
    }

    private void UpdateMouseLook()
    {
        if (axes == RotationAxes.MouseXAndY) {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        } else if (axes == RotationAxes.MouseX) {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        } else {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) {
            angle += 360F;
        }
        if (angle > 360F) {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, min, max);
    }
}
