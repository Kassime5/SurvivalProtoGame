public interface IMenu
{
    // Returns true if the menu is open, false if it's closed
    bool ToggleMenu(params object[] args);
}