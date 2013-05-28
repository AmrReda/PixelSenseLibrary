using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PixelSenseLibrary.Extensions
{
    public static class ItemsControlExtensions
    {
        public static IEnumerable GetItems(this ItemsControl itemsControl)
        {
            if (itemsControl.ItemsSource == null)
                return (IEnumerable)itemsControl.Items;
            ICollectionView defaultView = CollectionViewSource.GetDefaultView((object)itemsControl.ItemsSource);
            if (defaultView != null && defaultView.SourceCollection != null)
                return defaultView.SourceCollection;
            else
                return itemsControl.ItemsSource;
        }

        public static bool IsItemsINotifyCollectionChanged(this ItemsControl itemsControl)
        {
            return GetItems(itemsControl) is INotifyCollectionChanged;
        }

        public static bool IsItemsReadOnly(this ItemsControl itemsControl)
        {
            var list = GetItems(itemsControl) as IList;
            if (list != null && !list.IsReadOnly)
                return list.IsFixedSize;
            else
                return true;
        }

        public static T FindItemsPanel<T>(this ItemsControl itemsControl, Visual visual)
        {
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(visual); ++childIndex)
            {
                Visual visual1 = VisualTreeHelper.GetChild(visual, childIndex) as Visual;
                if (visual1 != null)
                {
                    if (visual1 is T && VisualTreeHelper.GetParent(visual1) is ItemsPresenter)
                        return (T)(object)visual1;
                    T itemsPanel = FindItemsPanel<T>(itemsControl, visual1);
                    if ((object)itemsPanel != null)
                        return itemsPanel;
                }
            }
            return default(T);
        } 
    }
}