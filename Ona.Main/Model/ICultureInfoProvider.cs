using System.Globalization;

namespace Ona.Main.Model;

public interface ICultureInfoProvider
{
    CultureInfo CurrentUICulture { get; }
}
