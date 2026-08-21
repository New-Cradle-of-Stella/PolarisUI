using nel;

namespace Polaris.UI
{
    public interface IPUI
    {
        string Name { get; }

        UiBoxDesigner GetUIWindow(UiBoxDesignerFamily source);

        void BuildUI(UiBoxDesigner designer);
    }
}
