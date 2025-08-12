using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTargetHandler : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (!spawnedObjects.ContainsKey(trackedImage.referenceImage.name))
            {
                GameObject newObject = Instantiate(prefabToSpawn, trackedImage.transform);
                spawnedObjects.Add(trackedImage.referenceImage.name, newObject);
            }
            UpdateObjectPosition(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateObjectPosition(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject obj))
            {
                Destroy(obj);
                spawnedObjects.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    private void UpdateObjectPosition(ARTrackedImage trackedImage)
    {
        if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out GameObject obj))
        {
            obj.transform.position = trackedImage.transform.position;
            obj.transform.rotation = trackedImage.transform.rotation;
            obj.SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }
    }
}