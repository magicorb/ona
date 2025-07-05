namespace Ona.Main.Model;

public class DatesChangedMessage
{
    public DatesChangedMessage(object sender)
    {
        Sender = sender;
    }

    public object Sender { get; }
}
