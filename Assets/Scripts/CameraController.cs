using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float cameraMoveSpeed;
    [SerializeField]
    private float cameraRotationSpeed;
    [SerializeField]
    private float cameraZoomSpeed;

    public float moveSpeed;

    protected bool isMoving = false;
    protected Vector3 newLocation;
    protected Vector3 newRotation;
    protected Vector3 initialRotation;
    protected Vector3 focusOffset;

    private void Start()
    {
        initialRotation = this.gameObject.transform.eulerAngles;
        focusOffset = -this.gameObject.transform.forward * 20;
    }

    public void FixedUpdate()
    {
        Vector3 forward = Vector3.Cross(Vector3.up, transform.right);
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.position += transform.right * cameraMoveSpeed * Time.deltaTime * x;
        transform.position += forward * cameraMoveSpeed * Time.deltaTime * -y;

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.up * -cameraRotationSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.up * cameraRotationSpeed * Time.deltaTime, Space.World);
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * cameraZoomSpeed;
        }

        if (isMoving)
        {
            float step = cameraMoveSpeed * Time.deltaTime;
            this.gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, newLocation, step);
            this.gameObject.transform.eulerAngles = Vector3.RotateTowards(gameObject.transform.eulerAngles, newRotation, step, 0.0f);

            if (Vector3.Distance(gameObject.transform.position, newLocation) < 0.5 &&
                Vector3.Distance(gameObject.transform.eulerAngles, newRotation) < 0.5)
            {
                isMoving = false;
            }
        }
    }
    
    public void FocusLocation(Vector3 pos)
    {
        this.newRotation = initialRotation;
        this.newLocation = pos + focusOffset;
        isMoving = true;
    }
}
