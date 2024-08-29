using GinjaGaming.FinalCharacterController;
using UnityEngine;


public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    private IMenu currentMenu;
    private PlayerController playerController;
    private PlayerActionsInput playerActionsInput;
    private object[] args;

    private void Awake()
    {
        instance = this;
        playerController = FindObjectOfType<PlayerController>();
        playerActionsInput = FindObjectOfType<PlayerActionsInput>();
    }

    public void OpenMenu(GameObject menu, params object[] args)
    {
        if (currentMenu == null)
        {
            if (menu.GetComponent<IMenu>().ToggleMenu(args))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.enabled = false;
                playerActionsInput.enabled = false;
                currentMenu = menu.GetComponent<IMenu>();
                this.args = args;
            }
            return;
        }

        if (currentMenu == menu.GetComponent<IMenu>())
        {
            if (!menu.GetComponent<IMenu>().ToggleMenu(args))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.enabled = true;
                playerActionsInput.enabled = true;
                currentMenu = null;
            }
            return;
        }
    }

    public void PlayerClickedBuild()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.enabled = true;
        playerActionsInput.enabled = true;
    }
}
