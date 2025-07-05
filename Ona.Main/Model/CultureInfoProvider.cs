using System.Globalization;

namespace Ona.Main.Model;

public class CultureInfoProvider : ICultureInfoProvider
{
    public CultureInfo CurrentUICulture
        => CultureInfo.CurrentUICulture;
}
