using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This class is a lightly modified version of the one shipped with AR Foundations to behave the way I want
/// </summary>
[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator2 : MonoBehaviour
{
    [SerializeField] GameObject m_Prefab;
    [SerializeField] GameObject hitpointPreview;
    private Transform _cameraTransform;

    public GameObject prefab
    {
        get => m_Prefab;
        set => m_Prefab = value;
    }

    private bool placementMode = false;
    private bool placeOnHit = false;

    public void EnterPlacementMode()
    {
        placementMode = true;
        placeOnHit = false;
    }

    public void PlaceThereWhenNextHit()
    {
        placeOnHit = true;
    }

    public void RemoveAllAnchors()
    {
        print($"Removing all anchors ({m_Anchors.Count})");
        foreach (var anchor in m_Anchors)
        {
            if (anchor != null)
                Destroy(anchor.gameObject);
        }

        m_Anchors.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        _cameraTransform = Camera.main.transform;
    }

    void SetAnchorText(ARAnchor anchor, string text)
    {
        var canvasTextManager = anchor.GetComponent<CanvasTextManager>();
        if (canvasTextManager)
        {
            canvasTextManager.text = text;
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor = null;

        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane)
        {
            var planeManager = GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                print("Creating anchor attachment.");
                var oldPrefab = m_AnchorManager.anchorPrefab;
                m_AnchorManager.anchorPrefab = prefab;
                anchor = m_AnchorManager.AttachAnchor(plane, hit.pose);
                m_AnchorManager.anchorPrefab = oldPrefab;
                SetAnchorText(anchor, $"Attached to plane {plane.trackableId}");
                return anchor;
            }
        }

        // Otherwise, just create a regular anchor at the hit pose
        print("Creating regular anchor.");

        // Note: the anchor can be anywhere in the scene hierarchy
        var gameObject = Instantiate(prefab, hit.pose.position, hit.pose.rotation);

        // Make sure the new GameObject has an ARAnchor component
        anchor = gameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = gameObject.AddComponent<ARAnchor>();
        }

        SetAnchorText(anchor, $"Anchor (from {hit.hitType})");

        return anchor;
    }

    void Update()
    {
        hitpointPreview.SetActive(false);
        if (!placementMode)
            return;

        hitpointPreview.SetActive(true);

        Touch touch = new Touch();

        // Raycast against planes and feature points
        const TrackableType trackableTypes =
            TrackableType.FeaturePoint |
            TrackableType.PlaneWithinPolygon;

        touch.position = new Vector2(Screen.width / 2, Screen.height / 2);
        // Perform the raycast
        if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes))
        {
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            var hit = s_Hits[0];
            hitpointPreview.transform.position = Vector3.Lerp(hitpointPreview.transform.position, hit.pose.position, .05f);

            if (placeOnHit)
            {
                // Create a new anchor
                var anchor = CreateAnchor(hit);
                if (anchor)
                {
                    m_Anchors.Add(anchor);
                }

                placeOnHit = false;
                placementMode = false;
            }
        }
        else
        {
            var inFrontOfCamera = _cameraTransform.position + _cameraTransform.forward * .5f;
            hitpointPreview.transform.position = Vector3.Lerp(hitpointPreview.transform.position, inFrontOfCamera, .05f);
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_Anchors = new List<ARAnchor>();

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}