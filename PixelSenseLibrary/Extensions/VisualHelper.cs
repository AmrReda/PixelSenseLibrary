using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PixelSenseLibrary.Extensions
{
    public static class VisualHelper
    {
        public static bool IsWithinElementBounds(UIElement element, Point relativePosition)
        {
            return (FindHitTestableDescendant(element, relativePosition) != null);
        }

        public static UIElement FindHitTestableDescendant(UIElement element, Point relativePosition)
        {
            HitTestResult result = null;
            VisualTreeHelper.HitTest(element, null, delegate(HitTestResult candidate)
            {
                if (candidate != null)
                {
                    UIElement visualHit = candidate.VisualHit as UIElement;
                    if ((visualHit != null) && visualHit.IsHitTestVisible)
                    {
                        result = candidate;
                        return HitTestResultBehavior.Stop;
                    }
                }
                return HitTestResultBehavior.Continue;
            }, new PointHitTestParameters(relativePosition));
            if (result != null)
            {
                return (result.VisualHit as UIElement);
            }
            return null;
        }

        public static bool IsWithinElementBounds(Visual visual, Geometry geometry)
        {
            bool result = false;
            GeometryHitTestParameters hitTestParameters = new GeometryHitTestParameters(geometry);
            VisualTreeHelper.HitTest(visual, null, delegate(HitTestResult candidate)
            {
                switch (((GeometryHitTestResult)candidate).IntersectionDetail)
                {
                    case IntersectionDetail.FullyInside:
                    case IntersectionDetail.FullyContains:
                    case IntersectionDetail.Intersects:
                        result = true;
                        break;
                }
                return HitTestResultBehavior.Stop;
            }, hitTestParameters);
            return result;
        }
        /// <summary>
        /// Gets the Visual Tree for a DependencyObject with that DependencyObject as the root.
        /// </summary>
        /// <param name="element">The root element.</param>
        /// <returns>The matching elements.</returns>
        public static IEnumerable<DependencyObject> GetVisualTree(this DependencyObject element)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < childrenCount; i++)
            {
                var visualChild = VisualTreeHelper.GetChild(element, i);

                yield return visualChild;

                foreach (var visualChildren in GetVisualTree(visualChild))
                {
                    yield return visualChildren;
                }
            }
        }
        public static Window GetWindow(this FrameworkElement element)
        {
            if (element is Window)
                return (Window)element;
            else
                return FindAncestor<Window>((DependencyObject)element);
        }

        public static T FindAncestor<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child);
            if (parentObject == null)
                return default(T);
            T obj = parentObject as T;
            if ((object)obj != null)
                return obj;
            else
                return FindAncestor<T>(parentObject);
        }

        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null)
                return (DependencyObject)null;
            ContentElement reference = child as ContentElement;
            if (reference != null)
            {
                DependencyObject parent = ContentOperations.GetParent(reference);
                if (parent != null)
                    return parent;
                FrameworkContentElement frameworkContentElement = reference as FrameworkContentElement;
                if (frameworkContentElement == null)
                    return (DependencyObject)null;
                else
                    return frameworkContentElement.Parent;
            }
            else
            {
                if (child is FrameworkElement)
                {
                    DependencyObject parent = VisualTreeHelper.GetParent(child);
                    if (parent != null)
                        return parent;
                }
                return VisualTreeHelper.GetParent(child);
            }
        }

        public static void DisposeAllDescendants(DependencyObject refObject)
        {
            if (refObject == null)
                return;
            int childrenCount = VisualTreeHelper.GetChildrenCount(refObject);
            for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
            {
                DependencyObject child = VisualTreeHelper.GetChild(refObject, childIndex);
                if (child != null)
                {
                    if (child is IDisposable)
                        (child as IDisposable).Dispose();
                    else
                        DisposeAllDescendants(child);
                }
            }
        }

        public static T FindDescendants<T>(Visual visual) where T : DependencyObject
        {
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount((DependencyObject)visual); ++childIndex)
            {
                Visual visual1 = VisualTreeHelper.GetChild((DependencyObject)visual, childIndex) as Visual;
                if (visual1 != null)
                {
                    if (visual1 is T)
                        return (T)(object)visual1;
                    T descendants = FindDescendants<T>(visual1);
                    if ((object)descendants != null)
                        return descendants;
                }
            }
            return default(T);
        }

        public static T FindVisualChild<T>(this DependencyObject obj)
           where T : DependencyObject
        {
            if (obj == null)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }
        public static double GetOrientation(FrameworkElement refElement, FrameworkElement refTargetLandmark)
        {
            if (refElement == null || refTargetLandmark == null)
                return 0.0;
            Matrix matrix = (refElement.TransformToAncestor((Visual)refTargetLandmark) as Transform).Value;
            double num1 = Math.Sqrt(matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21);
            double num2 = Math.Acos(matrix.M11 / num1);
            if (matrix.M12 < 0.0)
                num2 *= -1.0;
            return num2 * 180.0 / 3.14159265358979;
        }

        public static Size ComputeUniformRatio(Size refOriginalSize, Size refConstraintSize)
        {
            double width = 1.0;
            double height = 1.0;
            bool flag1 = !double.IsPositiveInfinity(refConstraintSize.Width);
            bool flag2 = !double.IsPositiveInfinity(refConstraintSize.Height);
            if (flag1 || flag2)
            {
                width = refOriginalSize.Width == 0.0 ? 0.0 : refConstraintSize.Width / refOriginalSize.Width;
                height = refOriginalSize.Height == 0.0 ? 0.0 : refConstraintSize.Height / refOriginalSize.Height;
                if (!flag1)
                    width = height;
                else if (!flag2)
                {
                    height = width;
                }
                else
                {
                    double num = width < height ? width : height;
                    width = height = num;
                }
            }
            return new Size(width, height);
        }

        public static Size ComputeUniformToFillRatio(Size refOriginalSize, Size refConstraintSize)
        {
            double width = 1.0;
            double height = 1.0;
            bool flag1 = !double.IsPositiveInfinity(refConstraintSize.Width);
            bool flag2 = !double.IsPositiveInfinity(refConstraintSize.Height);
            if (flag1 || flag2)
            {
                width = refOriginalSize.Width == 0.0 ? 0.0 : refConstraintSize.Width / refOriginalSize.Width;
                height = refOriginalSize.Height == 0.0 ? 0.0 : refConstraintSize.Height / refOriginalSize.Height;
                if (!flag1)
                    width = height;
                else if (!flag2)
                {
                    height = width;
                }
                else
                {
                    double num = width < height ? height : width;
                    width = height = num;
                }
            }
            return new Size(width, height);
        }
    }
}