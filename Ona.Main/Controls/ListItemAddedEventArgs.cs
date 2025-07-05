namespace Ona.Main.Controls;

public class ListItemAddedEventArgs
{
    public ListItemAddedEventArgs(int index, object item)
    {
        Index = index;
        Item = item;
    }

    public int Index { get; }

    public object Item { get; }
}
