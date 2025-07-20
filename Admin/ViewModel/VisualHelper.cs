using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FNS.Admin.ViewModel
{
    public static class VisualHelper
    {
        public static T FindAncestorOrSelf<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                if (obj is T objTyped)
                {
                    return objTyped;
                }

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }
    }
}
