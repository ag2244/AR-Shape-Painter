/*======================================*/
// The tutorial I followed:
// www.youtube.com/watch?v=Ml2UakwRxjkjj
/*======================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARTapToPlace : MonoBehaviour
{

    //Sliders to get a color by setting r, g and b values
    public Slider redSlider, greenSlider, blueSlider;

    //Label object within a shape to detect what shape it is (makes things readable for me!)
    public GameObject shapeDropdownLabel;

    //The material on each object. Editing this mat allows me to change how objects appear to some extent when spawned.
    public Material thisMat;

    //Toggle to destroy all objects
    public Toggle destroyObjs;

    //Shape of the painting objects
    private PrimitiveType shapeType;

    //Color of the painting objects
    public Color thisColor;

    //[Range(1f, 30f)] [SerializeField] private float distance = 10f;

    //public float rotationAmount = 0f;
    //public float rotationDelta = 5f;

    //float posX = -1f, posY = -1f, posZ = -1f;

    public GameObject placementIndicator;
    private ARSessionOrigin arOrigin;
    private ARRaycastManager raycaster;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    public GameObject objectToPlace;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        raycaster = FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {

        //Change the shape being placed based on the dropdown menu
        changeShape();
        updateColor();

        updatePlacementPose();
        updatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }

    }

    private void PlaceObject()
    {

        GameObject thing = GameObject.CreatePrimitive(shapeType);

        thing.transform.position = placementPose.position;
        thing.transform.rotation = placementPose.rotation;

        thing.GetComponent<Renderer>().material = thisMat;
        thing.GetComponent<Renderer>().material.color = thisColor;

        thing.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        //rotationAmount += rotationDelta;
        //thing.transform.Rotate(new Vector3(0f, 0f, rotationAmount));

        thing.transform.parent = this.transform;

        if (destroyObjs.isOn) { Destroy(thing, 5f); }

    }

    private void updatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }

        else { placementIndicator.SetActive(false); }
    }

    private void updatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        raycaster.Raycast(screenCenter, hits, TrackableType.Planes);
        // arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }

    }

    private void updateColor()
    {
        thisColor = new Color(redSlider.value * 2 / 255, greenSlider.value * 2 / 255, blueSlider.value * 2 / 255);
    }

    //Changes the shape based on the dropdown menu option selected.
    void changeShape()
    {

        Text dropdownText = shapeDropdownLabel.GetComponent<Text>();
        string dropdownValue = dropdownText.text;

        if (dropdownValue == "Cube") { shapeType = PrimitiveType.Cube; }

        else if (dropdownValue == "Sphere") { shapeType = PrimitiveType.Sphere; }

        else if (dropdownValue == "Cylinder") { shapeType = PrimitiveType.Cylinder; }

    }
}
