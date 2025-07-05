using System.Globalization;

namespace Ona.Main.Environment;

public class CultureInfoProvider : ICultureInfoProvider
{
    public CultureInfo CurrentUICulture
        => CultureInfo.CurrentUICulture;
}
