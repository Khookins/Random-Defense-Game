using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    // Inspector
    [Header("General Settings")]
    [SerializeField] private float ghostLerpSpeed = 1.0f;
    [SerializeField] private LayerMask enemyPathLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject rangeVisualPrefab;
    [SerializeField] private GameObject towersParent;
    [Header("Ghost Materials")]
    [SerializeField] private Material rangeMaterial;
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Material invalidMaterial;
    // uhh not inspector I guess
    private Tower PendingTowerPrefab { get; set; }
    private GameObject towerGhost;
    private GameObject towerGhostModel;
    private GameObject towerRangeVisual;
    private bool isValidPlacement = false;


    private void OnEnable()
    {
        Game.OnTowerPlacementStarted += HandleTowerPlacementStarted;
        Game.OnTowerPlacementEnded += HandleTowerPlacementEnded;
    }

    private void OnDisable()
    {
        Game.OnTowerPlacementStarted -= HandleTowerPlacementStarted;
        Game.OnTowerPlacementEnded -= HandleTowerPlacementEnded;
    }

    private void Update()
    {
        if (Game.Instance.cState != Game.ControlState.Placing) return;  

        if (GetMouseWorldPosition(out Vector3 mousePos))
        {
            MoveGhost(mousePos);
            isValidPlacement = CheckValidPlacement(mousePos);
            UpdateGhostColor();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isValidPlacement) return;
            PlaceTower();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Game.Instance.ExitTowerPlacement();
        }
    }

    /*private void HandleControlStateChanged(Game.ControlState state)
    {
        if (state == Game.ControlState.Placing)
        {
            SpawnGhost(Game.Instance.PendingTowerPrefab);
        }
        else
        {
            DestroyGhost();
        }
    } obsolete */

    private void HandleTowerPlacementStarted(Tower tower)
    {
        if (PendingTowerPrefab) return;
        PendingTowerPrefab = tower;
        SpawnGhost();
    }

    private void HandleTowerPlacementEnded()
    {
        DestroyGhost();
        PendingTowerPrefab = null;
    }

    // Replaces the current ghost prefab with a real tower placed down. Exits tower placement.
    private void PlaceTower()
    {
        if (!towerGhost) return;

        Instantiate(PendingTowerPrefab.gameObject, towerGhost.transform.position,Quaternion.identity,towersParent.transform);
        Game.Instance.ExitTowerPlacement();
    }

    // Spawns a visual of the tower you are pending to place, constantly following your cursor.
    private void SpawnGhost()
    {
        towerGhost = new GameObject();
        towerGhost.name = $"TowerPlacementGhost ({PendingTowerPrefab.name})";
        towerGhostModel = Instantiate(PendingTowerPrefab.gameObject,towerGhost.transform);

        foreach (Collider collider in towerGhostModel.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        foreach (MonoBehaviour monoBehaviour in towerGhostModel.GetComponentsInChildren<MonoBehaviour>())
        {
            monoBehaviour.enabled = false;
        }

        towerRangeVisual = Instantiate(rangeVisualPrefab, towerGhost.transform);
        towerRangeVisual.transform.localScale = new Vector3(PendingTowerPrefab.GetRange() * 2, towerRangeVisual.transform.localScale.y, PendingTowerPrefab.GetRange() * 2);
        towerRangeVisual.transform.position = Vector3.zero + (Vector3.down * (towerGhostModel.GetComponent<MeshRenderer>().bounds.size.y * 0.5f));
        towerGhostModel.transform.position = Vector3.zero;
    }

    // Moves the ghost to your cursors location.
    private void MoveGhost(Vector3 pos)
    {
        if (towerGhost == null) return;

        pos = pos + (Vector3.up * (towerGhostModel.GetComponent<MeshRenderer>().bounds.size.y * 0.5f));

        towerGhost.transform.position = Vector3.Lerp(towerGhost.transform.position,pos, ghostLerpSpeed);
    }

    // Destroys the pending ghost prefab.
    private void DestroyGhost()
    {
        GameObject.Destroy(towerGhost.gameObject);
    }

    // Gets the position of where the mouse is, in world space.
    private bool GetMouseWorldPosition(out Vector3 result)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity))
        {
            result = hit.point;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }

    // Checks if where the cursor currently is is a valid place for a tower.
    private bool CheckValidPlacement(Vector3 pos)
    {
        float radius = PendingTowerPrefab.transform.lossyScale.magnitude / 2;
        return !Physics.CheckSphere(pos, radius, enemyPathLayer) && Physics.CheckSphere(pos, radius, groundLayer);
    }

    // Updates the ghost color to match the current placement validity.
    private void UpdateGhostColor()
    {
        if (!towerGhost) return;
        MeshRenderer ghostRenderer = towerGhostModel.GetComponent<MeshRenderer>();
        MeshRenderer rangeRenderer = towerRangeVisual.GetComponent<MeshRenderer>();

        ghostRenderer.material = isValidPlacement ? ghostMaterial : invalidMaterial;
        rangeRenderer.material = isValidPlacement ? rangeMaterial : invalidMaterial;
    }
}
