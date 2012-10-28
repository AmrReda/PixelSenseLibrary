namespace PixelSenseLibrary.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    /// <summary>
    /// StoryboardHelper class
    /// </summary>
    public static  class StoryboardHelper
    {
        /// <summary>
        /// Clones the Storyboard.
        /// </summary>
        /// <param name="clonable">Target Storyboard.</param>
        /// <returns>Storyboard.</returns>
        public static Storyboard CloneStoryboard(Storyboard clonable)
        {
            if (clonable != null)
                return (Storyboard)XamlReader.Parse(XamlWriter.Save((object)clonable));
            else
                return (Storyboard)null;
        }


        public static void BeginElementStoryboard(UIElement target, string StoryboardName, PropertyPath propertyPath)
        {
            BeginElementStoryboard(target, StoryboardName, propertyPath, null);
        }

        public static void BeginElementStoryboard(UIElement target, string StoryboardName, PropertyPath propertyPath, EventHandler completedHandler)
        {
            BeginElementStoryboard(Application.Current.Resources, target, StoryboardName, propertyPath, completedHandler);
        }

        public static void BeginElementStoryboard(UIElement target, string subTargetName, string StoryboardName, PropertyPath propertyPath)
        {
            BeginElementStoryboard(target, subTargetName, StoryboardName, propertyPath, null);
        }

        public static void BeginElementStoryboard(UIElement target, string subTargetName, string StoryboardName, PropertyPath propertyPath, EventHandler completedHandler)
        {
            BeginElementStoryboard(Application.Current.Resources, target, subTargetName, StoryboardName, propertyPath, completedHandler);
        }

        public static void BeginElementStoryboard(ResourceDictionary resources, UIElement target, string StoryboardName, PropertyPath propertyPath)
        {
            BeginElementStoryboard(resources, target, StoryboardName, propertyPath, null);
        }

        public static void BeginElementStoryboard(ResourceDictionary resources, UIElement target, string StoryboardName, PropertyPath propertyPath, EventHandler completedHandler)
        {
            BeginElementStoryboard(resources, target, null, StoryboardName, propertyPath, completedHandler);
        }

        public static void BeginElementStoryboard(ResourceDictionary resources, UIElement target, string subTargetName, string StoryboardName, PropertyPath propertyPath, EventHandler completedHandler)
        {
            if (!resources.Contains(StoryboardName))
            {
                throw new ArgumentException(string.Format("Resource with name {0} not found!", StoryboardName));
            }
            if (!string.IsNullOrEmpty(subTargetName))
            {
                target = (target as FrameworkElement).FindName(subTargetName) as UIElement;
            }
            Storyboard sbAlpha = resources[StoryboardName] as Storyboard;
            Storyboard sb = CloneStoryboard(sbAlpha);
            Storyboard.SetTarget(sb, target);
            Storyboard.SetTargetProperty(sb, propertyPath);
            if (completedHandler != null)
            {
                sb.Completed += completedHandler;
            }
            sb.Begin();
        }
    }
}