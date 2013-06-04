using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PixelSenseLibrary.Commands;
using PixelSenseLibrary.Helpers;

namespace PixelSenseLibrary.Controls.ItemsCollections
{
    [TemplatePart(Name = "PART_NewItem", Type = typeof(ContentPresenter)),
    TemplatePart(Name = "PART_Transitions", Type = typeof(Panel)),
    TemplatePart(Name = "RP_TransitionPreviousStoryboard", Type = typeof(Storyboard)),
    TemplatePart(Name = "RP_TransitionNextStoryboard", Type = typeof(Storyboard)),
    TemplatePart(Name = "PART_ContentPresenter", Type = typeof(UIElement)),
    TemplatePart(Name = "PART_OldItem", Type = typeof(ContentPresenter))]
    public class TransitionPresenter : ListBox
    {
        //Dependency Properties
        public static readonly DependencyProperty IsSelectTransitionRunningProperty;
        private static readonly DependencyPropertyKey IsSelectTransitionRunningPropertyKey;

        
        protected ContentPresenter _newItem;
        protected ContentPresenter _oldItem;
        protected UIElement _contentPresenter;
        protected Panel _Transitions;
        protected Storyboard _transitionNextStoryboard;
        protected Storyboard _transitionPreviousStoryboard;

        //RoutedEvents
        public static readonly RoutedEvent NextTransitionEndedEvent;
        public static readonly RoutedEvent NextTransitionStartedEvent;
        public static readonly RoutedEvent PreviousTransitionEndedEvent;
        public static readonly RoutedEvent PreviousTransitionStartedEvent;


        #region Event Setters/Getters
        
        public event RoutedEventHandler NextTransitionEnded
        {
            add
            {
                AddHandler(NextTransitionEndedEvent, value);
            }
            remove
            {
                RemoveHandler(NextTransitionEndedEvent, value);
            }
        }

        public event RoutedEventHandler NextTransitionStarted
        {
            add
            {
                AddHandler(NextTransitionStartedEvent, value);
            }
            remove
            {
                RemoveHandler(NextTransitionStartedEvent, value);
            }
        }

        public event RoutedEventHandler PreviousTransitionEnded
        {
            add
            {
                AddHandler(PreviousTransitionEndedEvent, value);
            }
            remove
            {
                RemoveHandler(PreviousTransitionEndedEvent, value);
            }
        }

        public event RoutedEventHandler PreviousTransitionStarted
        {
            add
            {
                AddHandler(PreviousTransitionStartedEvent, value);
            }
            remove
            {
                RemoveHandler(PreviousTransitionStartedEvent, value);
            }
        }

        #endregion

        static TransitionPresenter()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionPresenter), new FrameworkPropertyMetadata(typeof(TransitionPresenter)));


            //Registering RoutedEvents and Dependency Properties
            PreviousTransitionStartedEvent = EventManager.RegisterRoutedEvent("PreviousTransitionStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TransitionPresenter));
            PreviousTransitionEndedEvent = EventManager.RegisterRoutedEvent("PreviousTransitionEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TransitionPresenter));
            NextTransitionStartedEvent = EventManager.RegisterRoutedEvent("NextTransitionStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TransitionPresenter));
            NextTransitionEndedEvent = EventManager.RegisterRoutedEvent("NextTransitionEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TransitionPresenter));
            IsSelectTransitionRunningPropertyKey = DependencyProperty.RegisterReadOnly("IsSelectTransitionRunning", typeof(bool), typeof(TransitionPresenter), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(TransitionPresenter.OnIsSelectTransitionRunningChanged)));
            IsSelectTransitionRunningProperty = IsSelectTransitionRunningPropertyKey.DependencyProperty;
        }

        public TransitionPresenter()
        {
            CommandBinding commandBinding = new CommandBinding(ItemsListCommands.SelectPrevious, this.ExecuteSelectPrevious,
                            this.CanExecuteSelectPrevious);
            base.CommandBindings.Add(commandBinding);
            CommandBinding binding2 = new CommandBinding(ItemsListCommands.SelectNext, this.ExecuteSelectNext,
                this.CanExecuteSelectNext);
            base.CommandBindings.Add(binding2);
        }


        public override void OnApplyTemplate()
        {
            OnApplyTemplate();
            
            //Assign Controls
            _newItem = (ContentPresenter)GetTemplateChild("PART_NewItem");
            _oldItem = (ContentPresenter)GetTemplateChild("PART_OldItem");
            _contentPresenter = (UIElement)GetTemplateChild("PART_ContentPresenter");
            this._Transitions= (Panel)base.GetTemplateChild("PART_Transitions");
            
            //Assign Storyboard
            var previousStoryboard = (Storyboard)base.Template.Resources["RP_TransitionPreviousStoryboard"];
            var nextStoryboard = (Storyboard)base.Template.Resources["RP_TransitionNextStoryboard"];

            
            if (previousStoryboard != null)
            {
                this._transitionPreviousStoryboard = StoryboardHelper.CloneStoryboard(previousStoryboard);
                
                //Delegate Completed Event
                if (this._transitionPreviousStoryboard != null)
                {
                    this._transitionPreviousStoryboard.Completed += new EventHandler(this.OnPreviousStoryboardCompleted);
                }
            }

            if (nextStoryboard != null)
            {
                this._transitionNextStoryboard = StoryboardHelper.CloneStoryboard(nextStoryboard);

                if (this._transitionNextStoryboard != null)
                {
                    this._transitionNextStoryboard.Completed += new EventHandler(this.OnNextStoryboardCompleted);
                }
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return null;
        }

        private void CanExecuteSelectNext(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanExecuteSelectPrevious(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ExecuteSelectNext(object sender, ExecutedRoutedEventArgs e)
        {
            bool bWrapValue = false;
            if (e.Parameter != null)
            {
                if (e.Parameter is string)
                {
                    bWrapValue = bool.Parse((string)e.Parameter);
                }
                else
                {
                    bWrapValue = (bool)e.Parameter;
                }
            }
            this.SelectNext(bWrapValue);
        }

        private void ExecuteSelectPrevious(object sender, ExecutedRoutedEventArgs e)
        {
            bool bWrapValue = false;
            if (e.Parameter != null)
            {
                if (e.Parameter is string)
                {
                    bWrapValue = bool.Parse((string)e.Parameter);
                }
                else
                {
                    bWrapValue = (bool)e.Parameter;
                }
            }
            this.SelectPrevious(bWrapValue);
        }
        protected virtual void OnIsSelectTransitionRunningChanged(DependencyPropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private static void OnIsSelectTransitionRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TransitionPresenter)d).OnIsSelectTransitionRunningChanged(e);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    if ((e.OldItems != null) && ((base.SelectedIndex == -1) || ((e.OldStartingIndex <= base.SelectedIndex) && ((e.OldStartingIndex + e.OldItems.Count) >= base.SelectedIndex))))
                    {
                        base.SelectedIndex = Math.Min(e.OldStartingIndex, base.Items.Count - 1);
                    }
                    if ((e.NewItems == null) || (base.SelectedIndex != -1))
                    {
                        break;
                    }
                    base.SelectedIndex = e.NewStartingIndex;
                    return;

                case NotifyCollectionChangedAction.Reset:
                    base.SelectedIndex = 0;
                    break;

                default:
                    return;
            }
        }

        private void OnNextStoryboardCompleted(object sender, EventArgs e)
        {
            if (base.SelectedIndex < (base.Items.Count - 1))
            {
                base.SelectedIndex++;
            }
            else
            {
                base.SelectedIndex = 0;
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.Visibility = Visibility.Visible;
            }
            if (this._contentPresenter != null)
            {
                this._Transitions.Visibility = Visibility.Hidden;
            }
            base.SetValue(IsSelectTransitionRunningPropertyKey, false);
            this.RaiseNextTransitionEndedEvent();
        }
        
        private void OnPreviousStoryboardCompleted(object sender, EventArgs e)
        {
            if (base.SelectedIndex > 0)
            {
                base.SelectedIndex--;
            }
            else
            {
                base.SelectedIndex = base.Items.Count - 1;
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.Visibility = Visibility.Visible;
            }
            if (this._Transitions != null)
            {
                this._Transitions.Visibility = Visibility.Hidden;
            }
            base.SetValue(IsSelectTransitionRunningPropertyKey, false);
            this.RaisePreviousTransitionEndedEvent();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            foreach (object obj2 in e.AddedItems)
            {
                ContentControl control = (ContentControl)base.ItemContainerGenerator.ContainerFromItem(obj2);
                if (control != null)
                {
                    control.Visibility = Visibility.Visible;
                }
            }
            foreach (object obj3 in e.RemovedItems)
            {
                ContentControl control2 = (ContentControl)base.ItemContainerGenerator.ContainerFromItem(obj3);
                if (control2 != null)
                {
                    control2.Visibility = Visibility.Hidden;
                }
            }
            e.Handled = true;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ContentControl control = (ContentControl)element;
            if (control != null)
            {
                control.Visibility = (base.SelectedItem == item) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void PrepareContents(FrameworkElement eltOldSelectedItem, FrameworkElement eltNewSelectedItem)
        {
            VisualBrush brush = new VisualBrush(eltOldSelectedItem)
            {
                Stretch = Stretch.Uniform
            };
            this._oldItem.Content = brush;
            VisualBrush brush2 = new VisualBrush(eltNewSelectedItem)
            {
                Stretch = Stretch.Uniform
            };
            this._newItem.Content = brush2;
        }

        protected RoutedEventArgs RaiseNextTransitionEndedEvent()
        {
            return RaiseNextTransitionEndedEvent(this);
        }

        internal static RoutedEventArgs RaiseNextTransitionEndedEvent(DependencyObject target)
        {
            if (target == null)
            {
                return null;
            }
            RoutedEventArgs args = new RoutedEventArgs
            {
                RoutedEvent = NextTransitionEndedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaiseNextTransitionStartedEvent()
        {
            return RaiseNextTransitionStartedEvent(this);
        }

        internal static RoutedEventArgs RaiseNextTransitionStartedEvent(DependencyObject target)
        {
            if (target == null)
            {
                return null;
            }
            RoutedEventArgs args = new RoutedEventArgs
            {
                RoutedEvent = NextTransitionStartedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaisePreviousTransitionEndedEvent()
        {
            return RaisePreviousTransitionEndedEvent(this);
        }

        internal static RoutedEventArgs RaisePreviousTransitionEndedEvent(DependencyObject target)
        {
            if (target == null)
            {
                return null;
            }
            RoutedEventArgs args = new RoutedEventArgs
            {
                RoutedEvent = PreviousTransitionEndedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaisePreviousTransitionStartedEvent()
        {
            return RaisePreviousTransitionStartedEvent(this);
        }

        internal static RoutedEventArgs RaisePreviousTransitionStartedEvent(DependencyObject target)
        {
            if (target == null)
            {
                return null;
            }
            RoutedEventArgs args = new RoutedEventArgs
            {
                RoutedEvent = PreviousTransitionStartedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        public static void RemoveNextTransitionEndedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            RoutedEventHelper.RemoveHandler(element, NextTransitionEndedEvent, handler);
        }

        public static void RemoveNextTransitionStartedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            RoutedEventHelper.RemoveHandler(element, NextTransitionStartedEvent, handler);
        }

        public static void RemovePreviousTransitionEndedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            RoutedEventHelper.RemoveHandler(element, PreviousTransitionEndedEvent, handler);
        }

        public static void RemovePreviousTransitionStartedHandler(DependencyObject element, RoutedEventHandler handler)
        {
            RoutedEventHelper.RemoveHandler(element, PreviousTransitionStartedEvent, handler);
        }

        public void SelectNext(bool bWrapValue)
        {
            int selectedIndex = base.SelectedIndex;
            int index = base.SelectedIndex + 1;
            int count = base.Items.Count;
            if (index >= count)
            {
                if (!bWrapValue)
                {
                    return;
                }
                index = 0;
            }
            if (this._contentPresenter!= null)
            {
                this._contentPresenter.Visibility = Visibility.Collapsed;
            }
            if (((index >= 0) && (index < count)) && ((selectedIndex >= 0) && (selectedIndex < count)))
            {
                ContentControl eltOldSelectedItem =
                    (ContentControl)base.ItemContainerGenerator.ContainerFromItem(base.Items.GetItemAt(selectedIndex));
                ContentControl eltNewSelectedItem =
                    (ContentControl)base.ItemContainerGenerator.ContainerFromItem(base.Items.GetItemAt(index));
                if (eltNewSelectedItem != null)
                {
                    eltNewSelectedItem.Visibility = Visibility.Visible;
                }
                if ((eltOldSelectedItem != null) && (eltNewSelectedItem != null))
                {
                    this.PrepareContents(eltOldSelectedItem, eltNewSelectedItem);
                }
                if (this._Transitions!= null)
                {
                    this._Transitions.Visibility = Visibility.Visible;
                }
                if ((this._transitionNextStoryboard != null) && (base.Template != null))
                {
                    this._transitionNextStoryboard.Begin(this, base.Template);
                    base.SetValue(IsSelectTransitionRunningPropertyKey, true);
                    this.RaiseNextTransitionStartedEvent();
                }
            }
        }

        public void SelectPrevious(bool bWrapValue)
        {
            int selectedIndex = base.SelectedIndex;
            int index = base.SelectedIndex - 1;
            int count = base.Items.Count;
            if (index < 0)
            {
                if (!bWrapValue)
                {
                    return;
                }
                index = count - 1;
            }
            if (this._contentPresenter != null)
            {
                this._contentPresenter.Visibility = Visibility.Collapsed;
            }
            if (((index >= 0) && (index < count)) && ((selectedIndex >= 0) && (selectedIndex < count)))
            {
                ContentControl eltOldSelectedItem = (ContentControl)base.ItemContainerGenerator.ContainerFromItem(base.Items.GetItemAt(selectedIndex));
                ContentControl eltNewSelectedItem = (ContentControl)base.ItemContainerGenerator.ContainerFromItem(base.Items.GetItemAt(index));
                if (eltOldSelectedItem != null)
                {
                    eltNewSelectedItem.Visibility = Visibility.Visible;
                }
                if ((eltOldSelectedItem != null) && (eltNewSelectedItem != null))
                {
                    this.PrepareContents(eltOldSelectedItem, eltNewSelectedItem);
                }
                if (this._Transitions != null)
                {
                    this._Transitions.Visibility = Visibility.Visible;
                }
                if ((this._transitionPreviousStoryboard != null) && (base.Template != null))
                {
                    this._transitionPreviousStoryboard.Begin(this, base.Template);
                    base.SetValue(IsSelectTransitionRunningPropertyKey, true);
                    this.RaisePreviousTransitionStartedEvent();
                }
            }
        }

        protected void SetIsSelectTransitionRunning(bool value)
        {
            base.SetValue(IsSelectTransitionRunningPropertyKey, value);
        }


        // Properties
        public bool IsSelectTransitionRunning
        {
            get
            {
                return (bool)base.GetValue(IsSelectTransitionRunningProperty);
            }
        } 
    }
}