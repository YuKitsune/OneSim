using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneSim.Uwp
{
    public interface IXamlRenderListener
    {
        void OnXamlRendered(FrameworkElement control);
    }
}
