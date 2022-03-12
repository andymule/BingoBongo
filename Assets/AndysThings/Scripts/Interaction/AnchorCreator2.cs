using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator2 : MonoBehaviour
{
    [SerializeField] GameObject m_Prefab;
    [SerializeField] GameObject hitpointPreview;

    public GameObject prefab
    {
        get => m_Prefab;
        set => m_Prefab = value;
    }

    public void RemoveAllAnchors()
    {
        print($"Removing all anchors ({m_Anchors.Count})");
        foreach (var anchor in m_Anchors)
        {
            Destroy(anchor.gameObject);
        }

        m_Anchors.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
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
        // if (Input.touchCount == 0)
            // return;

            Touch touch = new Touch();
            
        // if (touch.phase != TouchPhase.Began)
            // return;

        // Raycast against planes and feature points
        const TrackableType trackableTypes =
            TrackableType.FeaturePoint |
            TrackableType.PlaneWithinPolygon;

        touch.position = new Vector2(Screen.width / 2, Screen.height / 2);
        // Perform the raycast
        if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes))
        {
            
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            hitpointPreview.SetActive(true);
            var hit = s_Hits[0];
            
            
            
            hitpointPreview.transform.position = Vector3.Lerp(hitpointPreview.transform.position, hit.pose.position, .05f);

            // Create a new anchor
            // var anchor = CreateAnchor(hit);
            // if (anchor)
            // {
            // Remember the anchor so we can remove it later.
            // m_Anchors.Add(anchor);
            // }
            // else
            // {
            // print("Error creating anchor");
            // }
        }
        else
        {
            hitpointPreview.SetActive(false);
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_Anchors = new List<ARAnchor>();

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}