using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PixelSenseLibrary.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static IList<DependencyProperty> GetAttachedProperties(this DependencyObject refDependencyObject)
        {
            List<DependencyProperty> list = new List<DependencyProperty>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties((object)refDependencyObject, new Attribute[1]
      {
        (Attribute) new PropertyFilterAttribute(PropertyFilterOptions.Invalid | PropertyFilterOptions.SetValues | PropertyFilterOptions.UnsetValues | PropertyFilterOptions.Valid)
      }))
            {
                DependencyPropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(property);
                if (propertyDescriptor != null && propertyDescriptor.IsAttached)
                    list.Add(propertyDescriptor.DependencyProperty);
            }
            return (IList<DependencyProperty>)list;
        }

        public static void DisposeGeneric(this FrameworkElement refElement)
        {
            foreach (DependencyProperty dp in (IEnumerable<DependencyProperty>)GetAttachedProperties((DependencyObject)refElement))
            {
                if (!dp.ReadOnly && dp.IsValidValue((object)null))
                    refElement.SetValue(dp, (object)null);
            }
            refElement.CommandBindings.Clear();
            foreach (InputBinding inputBinding in refElement.InputBindings)
            {
                inputBinding.CommandParameter = (object)null;
                inputBinding.CommandTarget = (IInputElement)null;
            }
            refElement.InputBindings.Clear();
        } 
    }
}