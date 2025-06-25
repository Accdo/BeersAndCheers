using UnityEngine;

public class FurniturePlacement : MonoBehaviour
{
    [SerializeField] private GameObject furniturePrefab;
    [SerializeField] private float sphereRadius = 1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask placeableLayer; // New field for placeable layer
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float minRayDistance = 1f;
    [SerializeField] private float maxRayDistance = 3f;
    [SerializeField] private Quaternion rotationOffset = Quaternion.identity;

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
            currentFurniture = Instantiate(furniturePrefab, Vector3.zero, furniturePrefab.transform.rotation);
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

            // Tapping 컴포넌트에 BeerMiniGame 설정
            Tapping tapping = currentFurniture.GetComponent<Tapping>();
            if (tapping != null)
            {
                tapping.beerMiniGame = Object.FindFirstObjectByType<BeerMiniGame>(FindObjectsInactive.Include); // 씬에서 BeerMiniGame 찾기
                if (tapping.beerMiniGame == null)
                {
                    Debug.LogError("No BeerMiniGame found in the scene!");
                }
            }
        }


    }

    void UpdatePlacement()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, placeableLayer))
        {
            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
            if (distance >= minRayDistance)
            {
                currentFurniture.transform.position = hit.point + offset;
                Quaternion playerYRotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                currentFurniture.transform.rotation = playerYRotation * furniturePrefab.transform.rotation * rotationOffset;
                bool canPlace = !Physics.CheckSphere(hit.point, sphereRadius, obstacleLayer);
                UpdateFurnitureMaterial(canPlace);
            }
            else
            {
                currentFurniture.transform.position = ray.origin + ray.direction * minRayDistance + offset;
                Quaternion playerYRotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
                currentFurniture.transform.rotation = playerYRotation * furniturePrefab.transform.rotation * rotationOffset;
                UpdateFurnitureMaterial(false);
            }
        }
        else
        {
            currentFurniture.transform.position = ray.origin + ray.direction * maxRayDistance + offset;
            Quaternion playerYRotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
            currentFurniture.transform.rotation = playerYRotation * furniturePrefab.transform.rotation * rotationOffset;
            UpdateFurnitureMaterial(false);
        }
    }

    bool CanPlace()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Check if ray hits placeable layer and no obstacles in sphere
        return Physics.Raycast(ray, out hit, maxRayDistance, placeableLayer) &&
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
        Quaternion playerYRotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        currentFurniture.transform.rotation = playerYRotation * furniturePrefab.transform.rotation * rotationOffset;
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