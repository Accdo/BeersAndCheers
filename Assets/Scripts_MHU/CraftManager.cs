using UnityEngine;

public class FurniturePlacement : MonoBehaviour
{
    [SerializeField] private GameObject furniturePrefab;
    [SerializeField] private float sphereRadius = 1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float minRayDistance = 1f;
    [SerializeField] private float maxRayDistance = 5f;

    private GameObject currentFurniture;
    private bool isPlacing = false;
    private Camera mainCamera;
    private Material originalMaterial;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartPlacement();
        }

        if (Input.GetMouseButtonDown(1)) // Right-click to cancel placement
        {
            CancelPlacement();
        }

        if (isPlacing && currentFurniture != null)
        {
            UpdatePlacement();
            if (Input.GetMouseButtonDown(0) && CanPlace())
            {
                FinalizePlacement();
            }
        }
    }

    void StartPlacement()
    {
        if (!isPlacing)
        {
            isPlacing = true;
            currentFurniture = Instantiate(furniturePrefab, Vector3.zero, Quaternion.identity);
            BoxCollider collider = currentFurniture.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            Renderer renderer = currentFurniture.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterial = renderer.material;
            }
        }
    }

    void UpdatePlacement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
            if (distance >= minRayDistance)
            {
                currentFurniture.transform.position = hit.point + offset;
                bool canPlace = !Physics.CheckSphere(hit.point, sphereRadius, obstacleLayer);
                UpdateFurnitureMaterial(canPlace);
            }
            else
            {
                // If too close, position at min distance and mark as unplaceable
                currentFurniture.transform.position = ray.origin + ray.direction * minRayDistance + offset;
                UpdateFurnitureMaterial(false);
            }
        }
        else
        {
            // If ray doesn't hit within max distance, position at max distance
            currentFurniture.transform.position = ray.origin + ray.direction * maxRayDistance + offset;
            UpdateFurnitureMaterial(false); // Assume cannot place if out of range
        }
    }

    bool CanPlace()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        return Physics.Raycast(ray, out hit, maxRayDistance) &&
               !Physics.CheckSphere(hit.point, sphereRadius, obstacleLayer);
    }

    void UpdateFurnitureMaterial(bool canPlace)
    {
        Renderer renderer = currentFurniture.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = canPlace ? greenMaterial : redMaterial;
        }
    }

    void FinalizePlacement()
    {
        BoxCollider collider = currentFurniture.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
        Renderer renderer = currentFurniture.GetComponent<Renderer>();
        if (renderer != null && originalMaterial != null)
        {
            renderer.material = originalMaterial;
        }
        currentFurniture = null;
        isPlacing = false;
    }

    void CancelPlacement()
    {
        if (isPlacing && currentFurniture != null)
        {
            Destroy(currentFurniture);
            currentFurniture = null;
            isPlacing = false;
        }
    }

    void OnDrawGizmos()
    {
        if (isPlacing && currentFurniture != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentFurniture.transform.position - offset, sphereRadius);
        }
    }
}