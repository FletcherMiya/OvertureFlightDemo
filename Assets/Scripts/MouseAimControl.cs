using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimControl : MonoBehaviour
{

    [Header("Components")]

    [SerializeField]
    [Tooltip("Transform of the aircraft the rig follows and references")]
    private Transform aircraft = null;
    [SerializeField]
    [Tooltip("Transform of the object the mouse rotates to generate MouseAim position")]
    private Transform mouseAim = null;

    [SerializeField]
    [Tooltip("How far the boresight and mouse flight are from the aircraft")]
    private float aimDistance = 500f;

    [Space]
    [SerializeField]
    [Tooltip("How far the boresight and mouse flight are from the aircraft")]
    private bool showDebugInfo = false;

    private Vector3 frozenDirection = Vector3.forward;
    private bool isMouseAimFrozen = false;

    /// <summary>
    /// Get a point along the aircraft's boresight projected out to aimDistance meters.
    /// Useful for drawing a crosshair to aim fixed forward guns with, or to indicate what
    /// direction the aircraft is pointed.
    /// </summary>
    public Vector3 BoresightPos
    {
        get
        {
            return aircraft == null
                 ? transform.forward * aimDistance
                 : (aircraft.transform.forward * aimDistance) + aircraft.transform.position;
        }
    }

    /// <summary>
    /// Get the position that the mouse is indicating the aircraft should fly, projected
    /// out to aimDistance meters. Also meant to be used to draw a mouse cursor.
    /// </summary>
    public Vector3 MouseAimPos
    {
        get
        {
            if (mouseAim != null)
            {
                return isMouseAimFrozen
                    ? GetFrozenMouseAimPos()
                    : mouseAim.position;
            }
            else
            {
                return transform.forward * aimDistance;
            }
        }
    }

    private void Awake()
    {
        if (aircraft == null)
            Debug.LogError(name + "MouseFlightController - No aircraft transform assigned!");
        if (mouseAim == null)
            Debug.LogError(name + "MouseFlightController - No mouse aim transform assigned!");

        // To work correctly, the entire rig must not be parented to anything.
        // When parented to something (such as an aircraft) it will inherit those
        // rotations causing unintended rotations as it gets dragged around.
        transform.parent = null;
    }

    private void Update()
    {

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 36f);
        aimDistance = Vector3.Distance(aircraft.transform.position, mouseWorldPosition);

        mouseAim.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 36f);

        RotateRig();
    }

    private void FixedUpdate()
    {

    }

    private void RotateRig()
    {
        // Freeze the mouse aim direction when the free look key is pressed.
        if (Input.GetKeyDown(KeyCode.C))
        {
            isMouseAimFrozen = true;
            frozenDirection = mouseAim.forward;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            isMouseAimFrozen = false;
            mouseAim.forward = frozenDirection;
        }

        // Mouse input.
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 36f);

        float angle = angleBetweenTwoPoints(aircraft.transform.position, mouseWorldPosition);

        mouseAim.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
    }

    private float angleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(b.x - a.x, b.z - a.z) * Mathf.Rad2Deg;
    }

    private Vector3 GetFrozenMouseAimPos()
    {
        if (mouseAim != null)
            return mouseAim.position + (frozenDirection * aimDistance);
        else
            return transform.forward * aimDistance;
    }

    private void UpdateCameraPos()
    {
        if (aircraft != null)
        {
            // Move the whole rig to follow the aircraft.
            transform.position = aircraft.position;
        }
    }

}