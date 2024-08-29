using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildManager : MonoBehaviour, IMenu
{
    private BuildSO[] buildSOs;
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject buildButtonPrefab;
    [SerializeField] private Transform buildButtonParent;
    public static BuildManager instance;
    private InventoryController inventoryController;
    PlayerInput playerInput;
    private BuildSO currentBuildSO;
    private bool isBuilding = false;
    private bool isMenuOpen = false;

    #region Static Methods
    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Build.performed += ctx => MenuButtonPressed();
        playerInput.Player.Interact.performed += ctx => Build();
        inventoryController = InventoryController.instance;
    }
    void Start()
    {
        instance = this;
        buildSOs = Resources.LoadAll<BuildSO>("Builds");
        buildMenu.SetActive(false);
    }
    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
    private void MenuButtonPressed()
    {
        MenuManager.instance.OpenMenu(gameObject, "Inventory");
    }
    #endregion

    public bool ToggleMenu(params object[] args)
    {
        isMenuOpen = !isMenuOpen;

        // Menu button pressed while building
        if (isBuilding)
        {
            BuildRayCast.instance.StopBuilding();
            isBuilding = false;
        }

        buildMenu.SetActive(isMenuOpen);
        if (buildMenu.activeSelf)
        {
            UpdateBuildMenu();
        }

        return isMenuOpen;
    }

    void UpdateBuildMenu()
    {
        int buildListHeight = 275;
        int buildListCounter = -1;
        foreach (Transform child in buildButtonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (BuildSO buildSO in buildSOs)
        {
            GameObject buildButton = Instantiate(buildButtonPrefab, buildButtonParent);
            buildButton.GetComponent<BuildSlot>().SetBuildSlot(buildSO);
            buildListCounter++;
            if (buildListCounter == 4)
            {
                buildListHeight += 275;
                buildListCounter = 0;
            }
        }
        buildButtonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(1750, buildListHeight);
    }

    public void CanBuild(BuildSO buildSO)
    {
        if (buildSO.requiredItems.Length != buildSO.requiredAmounts.Length)
        {
            Debug.LogError("BuildSO " + buildSO.name + " has different required items and amounts lengths");
            return;
        }
        for (int i = 0; i < buildSO.requiredItems.Length; i++)
        {
            if (inventoryController.HaveItems(buildSO.requiredItems[i], buildSO.requiredAmounts[i]))
            {
                continue;
            }
            else
            {
                return;
            }
        }
        Debug.Log("Can build " + buildSO.name);

        // Disables the menu and allow the player to move and build
        currentBuildSO = buildSO;
        isBuilding = true;
        buildMenu.SetActive(false);
        MenuManager.instance.PlayerClickedBuild();
        BuildRayCast.instance.StartBuilding(buildSO.buildPrefab);
    }

    public bool Build()
    {
        if (!isBuilding)
        {
            return false;
        }
        BuildSO buildSO = currentBuildSO;
        print("Building " + buildSO.buildName);
        if (buildSO.requiredItems.Length != buildSO.requiredAmounts.Length)
        {
            Debug.LogError("BuildSO " + buildSO.name + " has different required items and amounts lengths");
            return false;
        }
        for (int i = 0; i < buildSO.requiredItems.Length; i++)
        {
            if (inventoryController.HaveItems(buildSO.requiredItems[i], buildSO.requiredAmounts[i]))
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        for (int i = 0; i < buildSO.requiredItems.Length; i++)
        {
            inventoryController.RemoveItem(buildSO.requiredItems[i], buildSO.requiredAmounts[i]);
            print("Removed " + buildSO.requiredItems[i].name + " x" + buildSO.requiredAmounts[i]);
        }
        print("Built " + buildSO.buildName);
        Instantiate(buildSO.buildPrefab, BuildRayCast.instance.GetCurrentBuildingPosition(), Quaternion.identity);
        return true;
    }
}
