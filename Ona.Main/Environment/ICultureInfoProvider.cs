using System.Globalization;

namespace Ona.Main.Environment;

public interface ICultureInfoProvider
{
    CultureInfo CurrentUICulture { get; }
}
