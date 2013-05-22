namespace Nolte.UI
{
    public interface IMainMenuCapable
    {
        System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Windows.Forms.ToolStripMenuItem>> MenuItems { get; }
        bool MenuVisible { get; set; }
    }
}
