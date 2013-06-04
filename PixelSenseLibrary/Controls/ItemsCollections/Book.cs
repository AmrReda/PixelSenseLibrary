using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using PixelSenseLibrary.Commands;
using PixelSenseLibrary.Enums;
using PixelSenseLibrary.Events;
using PixelSenseLibrary.Extensions;
using PixelSenseLibrary.Helpers;

namespace PixelSenseLibrary.Controls.ItemsCollections
{
    [TemplatePart(Name = "PART_LeftPage", Type = typeof(BookPage)),
     TemplatePart(Name = "PART_RightPage", Type = typeof(BookPage))]
    public class Book : ItemsControl, IDisposable
    {
        private int? m_nNextSheet = new int?();
        protected DispatcherTimer m_refIdleAnimationTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds((double)Book.s_nIDLE_ANIMATION_INTERVAL)
        };
        public static int s_nDEFAULT_TURN_ANIMATION_DURATION = 500;
        public static int s_nIDLE_ANIMATION_INTERVAL = 1000;
        private static readonly DependencyPropertyKey CurrentSheetIndexPropertyKey = DependencyProperty.RegisterReadOnly("CurrentSheetIndex", typeof(int), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)0, new PropertyChangedCallback(Book.OnCurrentSheetIndexChanged), new CoerceValueCallback(Book.CoerceCurrentSheetIndexValue)));
        public static readonly DependencyProperty CurrentSheetIndexProperty = CurrentSheetIndexPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsPageDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsPageDragging", typeof(bool), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(Book.OnIsPageDraggingChanged)));
        public static readonly DependencyProperty IsPageDraggingProperty = IsPageDraggingPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsReadingFromRightToLeftProperty = DependencyProperty.Register("IsReadingFromRightToLeft", typeof(bool), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(Book.OnIsReadingFromRightToLeftChanged)));
        public static readonly DependencyProperty IsTurningFromTopProperty = DependencyProperty.Register("IsTurningFromTop", typeof(bool), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
        public static readonly DependencyProperty TurnAnimationDurationProperty = DependencyProperty.Register("TurnAnimationDuration", typeof(int), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)500, new PropertyChangedCallback(Book.OnTurnAnimationDurationChanged)));
        public static readonly DependencyProperty ReferencePageSizeProperty = DependencyProperty.Register("ReferencePageSize", typeof(Size), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Size(double.NaN, double.NaN)));
        public static readonly DependencyProperty DisplayPageNumbersProperty = DependencyProperty.Register("DisplayPageNumbers", typeof(bool), typeof(Book), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(Book.OnDisplayPageNumbersChanged)));
        public static readonly RoutedEvent PageTurnedEvent = EventManager.RegisterRoutedEvent("PageTurned", RoutingStrategy.Bubble, typeof(EventHandler<GenericRoutedEventArgs<int>>), typeof(Book));
        public static readonly RoutedEvent PageDroppedEvent = EventManager.RegisterRoutedEvent("PageDropped", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Book));
        public static readonly RoutedEvent DraggingStartedEvent = EventManager.RegisterRoutedEvent("DraggingStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Book));
        protected BookPage m_refPARTLeftPage;
        protected BookPage m_refPARTRightPage;

        public int CurrentSheetIndex
        {
            get
            {
                return (int)GetValue(CurrentSheetIndexProperty);
            }
        }

        public bool IsPageDragging
        {
            get
            {
                return (bool)GetValue(IsPageDraggingProperty);
            }
        }

        public bool IsReadingFromRightToLeft
        {
            get
            {
                return (bool)GetValue(IsReadingFromRightToLeftProperty);
            }
            set
            {
                SetValue(IsReadingFromRightToLeftProperty, value);
            }
        }

        public bool IsTurningFromTop
        {
            get
            {
                return (bool)GetValue(IsTurningFromTopProperty);
            }
            set
            {
                this.SetValue(IsTurningFromTopProperty, value);
            }
        }

        public int TurnAnimationDuration
        {
            get
            {
                return (int)GetValue(TurnAnimationDurationProperty);
            }
            set
            {
                SetValue(TurnAnimationDurationProperty, value);
            }
        }

        public Size ReferencePageSize
        {
            get
            {
                return (Size)GetValue(ReferencePageSizeProperty);
            }
            set
            {
                SetValue(ReferencePageSizeProperty, value);
            }
        }

        public bool DisplayPageNumbers
        {
            get
            {
                return (bool)GetValue(DisplayPageNumbersProperty);
            }
            set
            {
                SetValue(DisplayPageNumbersProperty, value);
            }
        }

        public event EventHandler<GenericRoutedEventArgs<int>> PageTurned
        {
            add
            {
                AddHandler(PageTurnedEvent, value);
            }
            remove
            {
                RemoveHandler(PageTurnedEvent, value);
            }
        }

        public event RoutedEventHandler PageDropped
        {
            add
            {
                AddHandler(PageDroppedEvent, value);
            }
            remove
            {
                RemoveHandler(PageDroppedEvent, value);
            }
        }

        public event RoutedEventHandler DraggingStarted
        {
            add
            {
                AddHandler(DraggingStartedEvent, value);
            }
            remove
            {
                RemoveHandler(DraggingStartedEvent, value);
            }
        }

        static Book()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Book), new FrameworkPropertyMetadata(typeof(Book)));
        }

        public Book()
        {
            CommandBindings.Add(new CommandBinding(BookCommands.TurnLeftPage, ExecuteTurnLeftPage, CanTurnLeftPage));
            CommandBindings.Add(new CommandBinding(BookCommands.TurnRightPage, ExecuteTurnRightPage, CanTurnRightPage));
            CommandBindings.Add(new CommandBinding(BookCommands.NavigateToPage, ExecuteNavigateToPage, CanNavigateToPage));
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected void SetCurrentSheetIndex(int value)
        {
            SetValue(CurrentSheetIndexPropertyKey, value);
        }

        private static void OnCurrentSheetIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Book)d).OnCurrentSheetIndexChanged(e);
        }

        protected virtual void OnCurrentSheetIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshSheetsContent();
            CommandManager.InvalidateRequerySuggested();
        }

        private static object CoerceCurrentSheetIndexValue(DependencyObject d, object value)
        {
            int num = (int)value;
            Book Book = (Book)d;
            if (Book != null)
            {
                if (num < 0)
                    return 0;
                int count = Book.Items.Count;
                if (num > (count + 1) / 2)
                    return ((count + 1) / 2);
            }
            return num;
        }

        protected void SetIsPageDragging(bool value)
        {
            SetValue(IsPageDraggingPropertyKey, value);
        }

        private static void OnIsPageDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Book)d).OnIsPageDraggingChanged(e);
        }

        protected virtual void OnIsPageDraggingChanged(DependencyPropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private static void OnIsReadingFromRightToLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Book Book = (Book)d;
            bool oldIsReadingFromRightToLeft = (bool)e.OldValue;
            bool readingFromRightToLeft = Book.IsReadingFromRightToLeft;
            Book.OnIsReadingFromRightToLeftChanged(oldIsReadingFromRightToLeft, readingFromRightToLeft);
        }

        protected virtual void OnIsReadingFromRightToLeftChanged(bool oldIsReadingFromRightToLeft, bool newIsReadingFromRightToLeft)
        {
            this.RefreshSheetsContent();
        }

        private static void OnTurnAnimationDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Book Book = (Book)d;
            int oldTurnAnimationDuration = (int)e.OldValue;
            int animationDuration = Book.TurnAnimationDuration;
            Book.OnTurnAnimationDurationChanged(oldTurnAnimationDuration, animationDuration);
        }

        protected virtual void OnTurnAnimationDurationChanged(int oldTurnAnimationDuration, int newTurnAnimationDuration)
        {
            if (this.m_refPARTLeftPage != null)
                this.m_refPARTLeftPage.TurnAnimationDuration = this.TurnAnimationDuration;
            if (this.m_refPARTRightPage == null)
                return;
            this.m_refPARTRightPage.TurnAnimationDuration = this.TurnAnimationDuration;
        }

        private static void OnDisplayPageNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Book Book = (Book)d;
            bool oldDisplayPageNumbers = (bool)e.OldValue;
            bool displayPageNumbers = Book.DisplayPageNumbers;
            Book.OnDisplayPageNumbersChanged(oldDisplayPageNumbers, displayPageNumbers);
        }

        protected virtual void OnDisplayPageNumbersChanged(bool oldDisplayPageNumbers, bool newDisplayPageNumbers)
        {
            if (newDisplayPageNumbers)
            {
                if (this.m_refPARTLeftPage != null)
                    this.m_refPARTLeftPage.ShowPageNumbers();
                if (this.m_refPARTRightPage == null)
                    return;
                this.m_refPARTRightPage.ShowPageNumbers();
            }
            else
            {
                if (this.m_refPARTLeftPage != null)
                    this.m_refPARTLeftPage.HidePageNumbers();
                if (this.m_refPARTRightPage == null)
                    return;
                this.m_refPARTRightPage.HidePageNumbers();
            }
        }

        public static void AddPageTurnedHandler(DependencyObject element, EventHandler<GenericRoutedEventArgs<int>> handler)
        {
            RoutedEventHelper.AddHandler(element, Book.PageTurnedEvent, (Delegate)handler);
        }

        public static void RemovePageTurnedHandler(DependencyObject element, EventHandler<GenericRoutedEventArgs<int>> handler)
        {
            RoutedEventHelper.RemoveHandler(element, Book.PageTurnedEvent, (Delegate)handler);
        }

        protected GenericRoutedEventArgs<int> RaisePageTurnedEvent(int nNumberOfPage)
        {
            return Book.RaisePageTurnedEvent((DependencyObject)this, nNumberOfPage);
        }

        internal static GenericRoutedEventArgs<int> RaisePageTurnedEvent(DependencyObject target, int nNumberOfPage)
        {
            if (target == null)
                return (GenericRoutedEventArgs<int>)null;
            GenericRoutedEventArgs<int> genericRoutedEventArgs = new GenericRoutedEventArgs<int>(nNumberOfPage);
            genericRoutedEventArgs.RoutedEvent = Book.PageTurnedEvent;
            RoutedEventHelper.RaiseEvent(target, (RoutedEventArgs)genericRoutedEventArgs);
            return genericRoutedEventArgs;
        }

        protected RoutedEventArgs RaisePageDroppedEvent()
        {
            return Book.RaisePageDroppedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaisePageDroppedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = Book.PageDroppedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaiseDraggingStartedEvent()
        {
            return Book.RaiseDraggingStartedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaiseDraggingStartedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = Book.DraggingStartedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshSheetsContent();
            this.m_refIdleAnimationTimer.Tick += new EventHandler(this.OnIdleAnimationTimerTick);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.m_refIdleAnimationTimer == null)
                return;
            this.m_refIdleAnimationTimer.Tick -= new EventHandler(this.OnIdleAnimationTimerTick);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool))
                return;
            if (!(bool)e.NewValue)
            {
                this.m_refIdleAnimationTimer.Stop();
                if (this.m_refPARTLeftPage.Status == PageStatus.Idle)
                    this.m_refPARTLeftPage.ResetCornerAnimation();
                if (this.m_refPARTRightPage.Status == PageStatus.Idle)
                    this.m_refPARTRightPage.ResetCornerAnimation();
                if (this.m_refPARTLeftPage.Status == PageStatus.Dragging)
                    this.m_refPARTLeftPage.DropPage(BookPage.s_nANIMATION_DURATION);
                if (this.m_refPARTRightPage.Status != PageStatus.Dragging)
                    return;
                this.m_refPARTRightPage.DropPage(BookPage.s_nANIMATION_DURATION);
            }
            else
                this.m_refIdleAnimationTimer.Start();
        }

        private void OnIdleAnimationEnded(object sender, RoutedEventArgs e)
        {
            if (!this.IsEnabled)
                return;
            this.m_refIdleAnimationTimer.Start();
        }

        private void OnIdleAnimationTimerTick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).IsEnabled = false;
            if (this.m_refPARTRightPage == null || this.m_refPARTLeftPage == null || (!this.IsEnabled || this.m_refPARTLeftPage.Status != PageStatus.Idle) || this.m_refPARTRightPage.Status != PageStatus.Idle)
                return;
            int num = this.Items.Count / 2;
            Random random = new Random();
            BookPage BookPage = random.Next(1, 3) != 1 ? (this.CurrentSheetIndex != num && !this.IsReadingFromRightToLeft || this.CurrentSheetIndex != 0 && this.IsReadingFromRightToLeft ? this.m_refPARTRightPage : this.m_refPARTLeftPage) : (this.CurrentSheetIndex != 0 && !this.IsReadingFromRightToLeft || this.CurrentSheetIndex != num && this.IsReadingFromRightToLeft ? this.m_refPARTLeftPage : this.m_refPARTRightPage);
            if (BookPage == null)
                return;
            BookPage.ExecuteIdleAnimation(random.Next(1, 3));
        }

        private void ExecuteTurnLeftPage(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is CornerOrigin)
                this.TurnLeftPage(new bool?((CornerOrigin)e.Parameter == CornerOrigin.TopLeft));
            else
                this.TurnLeftPage(new bool?());
        }

        private void CanTurnLeftPage(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!this.IsReadingFromRightToLeft)
                e.CanExecute = this.CurrentSheetIndex > 0 && !this.IsPageDragging;
            else
                e.CanExecute = this.CurrentSheetIndex < this.Items.Count / 2 && !this.IsPageDragging || this.Items.Count % 2 == 1 && this.CurrentSheetIndex == this.Items.Count / 2;
        }

        private void ExecuteTurnRightPage(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is CornerOrigin)
                this.TurnRightPage(new bool?((CornerOrigin)e.Parameter == CornerOrigin.TopRight));
            else
                this.TurnRightPage(new bool?());
        }

        private void CanTurnRightPage(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!this.IsReadingFromRightToLeft)
                e.CanExecute = this.CurrentSheetIndex < this.Items.Count / 2 && !this.IsPageDragging || this.Items.Count % 2 == 1 && this.CurrentSheetIndex == this.Items.Count / 2;
            else
                e.CanExecute = this.CurrentSheetIndex > 0 && !this.IsPageDragging;
        }

        private void ExecuteNavigateToPage(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is int)
            {
                this.NavigateToPage((int)e.Parameter, true);
            }
            else
            {
                if (!(e.Parameter is string))
                    return;
                this.NavigateToPage(int.Parse(e.Parameter as string), true);
            }
        }

        private void CanNavigateToPage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.m_refPARTLeftPage != null)
            {
                this.m_refPARTLeftPage.DraggingStarted -= new RoutedEventHandler(this.OnLeftPageDraggingStarted);
                this.m_refPARTLeftPage.DraggingEnded -= new RoutedEventHandler(this.OnLeftPageDraggingEnded);
                this.m_refPARTLeftPage.PageTurned -= new RoutedEventHandler(this.OnLeftPageTurned);
                this.m_refPARTLeftPage.IdleAnimationEnded -= new RoutedEventHandler(this.OnIdleAnimationEnded);
                this.m_refPARTLeftPage.PageDropped -= new RoutedEventHandler(this.OnPageDropped);
            }
            if (this.m_refPARTRightPage != null)
            {
                this.m_refPARTRightPage.DraggingStarted -= new RoutedEventHandler(this.OnRightPageDraggingStarted);
                this.m_refPARTRightPage.DraggingEnded -= new RoutedEventHandler(this.OnRightPageDraggingEnded);
                this.m_refPARTRightPage.PageTurned -= new RoutedEventHandler(this.OnRightPageTurned);
                this.m_refPARTRightPage.IdleAnimationEnded -= new RoutedEventHandler(this.OnIdleAnimationEnded);
                this.m_refPARTRightPage.PageDropped -= new RoutedEventHandler(this.OnPageDropped);
            }
            this.m_refPARTLeftPage = (BookPage)this.GetTemplateChild("PART_LeftPage");
            this.m_refPARTRightPage = (BookPage)this.GetTemplateChild("PART_RightPage");
            if (this.m_refPARTLeftPage != null)
            {
                this.m_refPARTLeftPage.DraggingStarted += new RoutedEventHandler(this.OnLeftPageDraggingStarted);
                this.m_refPARTLeftPage.DraggingEnded += new RoutedEventHandler(this.OnLeftPageDraggingEnded);
                this.m_refPARTLeftPage.PageTurned += new RoutedEventHandler(this.OnLeftPageTurned);
                this.m_refPARTLeftPage.IdleAnimationEnded += new RoutedEventHandler(this.OnIdleAnimationEnded);
                this.m_refPARTLeftPage.PageDropped += new RoutedEventHandler(this.OnPageDropped);
                if (this.m_refPARTLeftPage.IsLoaded)
                {
                    if (this.DisplayPageNumbers)
                        this.m_refPARTLeftPage.ShowPageNumbers();
                    else
                        this.m_refPARTLeftPage.HidePageNumbers();
                }
                else
                    this.m_refPARTLeftPage.Loaded += (RoutedEventHandler)((s, e) =>
                    {
                        if (this.DisplayPageNumbers)
                            this.m_refPARTLeftPage.ShowPageNumbers();
                        else
                            this.m_refPARTLeftPage.HidePageNumbers();
                    });
            }
            if (this.m_refPARTRightPage != null)
            {
                this.m_refPARTRightPage.DraggingStarted += new RoutedEventHandler(this.OnRightPageDraggingStarted);
                this.m_refPARTRightPage.DraggingEnded += new RoutedEventHandler(this.OnRightPageDraggingEnded);
                this.m_refPARTRightPage.PageTurned += new RoutedEventHandler(this.OnRightPageTurned);
                this.m_refPARTRightPage.IdleAnimationEnded += new RoutedEventHandler(this.OnIdleAnimationEnded);
                this.m_refPARTRightPage.PageDropped += new RoutedEventHandler(this.OnPageDropped);
                if (this.m_refPARTRightPage.IsLoaded)
                {
                    if (this.DisplayPageNumbers)
                        this.m_refPARTRightPage.ShowPageNumbers();
                    else
                        this.m_refPARTRightPage.HidePageNumbers();
                }
                else
                    this.m_refPARTRightPage.Loaded += (RoutedEventHandler)((s, e) =>
                    {
                        if (this.DisplayPageNumbers)
                            this.m_refPARTRightPage.ShowPageNumbers();
                        else
                            this.m_refPARTRightPage.HidePageNumbers();
                    });
            }
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            this.RefreshSheetsContent();
            if (this.IsEnabled)
                this.m_refIdleAnimationTimer.Start();
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (this.m_refPARTLeftPage != null)
                this.m_refPARTLeftPage.ResetCornerAnimation();
            if (this.m_refPARTRightPage != null)
                this.m_refPARTRightPage.ResetCornerAnimation();
            this.m_refIdleAnimationTimer.Start();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            this.RefreshSheetsContent();
            CommandManager.InvalidateRequerySuggested();
        }

        protected virtual object GetPage(int index)
        {
            if (index >= 0 && index < this.Items.Count)
                return this.Items[index];
            else
                return (object)new Canvas();
        }

        protected virtual void RefreshSheetsContent()
        {
            if (this.m_refPARTLeftPage == null || this.m_refPARTRightPage == null)
                return;
            this.m_refPARTLeftPage.IsEnabled = true;
            this.m_refPARTRightPage.IsEnabled = true;
            if (this.m_refPARTLeftPage != null)
                this.m_refPARTLeftPage.TurnAnimationDuration = this.TurnAnimationDuration;
            if (this.m_refPARTRightPage != null)
                this.m_refPARTRightPage.TurnAnimationDuration = this.TurnAnimationDuration;
            bool flag = this.Items.Count % 2 == 1;
            int val1 = this.Items.Count / 2;
            int num = !flag ? Math.Min(val1, this.CurrentSheetIndex) : Math.Min(val1 + 1, this.CurrentSheetIndex);
            if (num != this.CurrentSheetIndex)
            {
                this.SetCurrentSheetIndex(num);
            }
            else
            {
                ContentPresenter frontPage1 = this.m_refPARTLeftPage.FrontPage;
                ContentPresenter backPage1 = this.m_refPARTLeftPage.BackPage;
                ContentPresenter futurePage1 = this.m_refPARTLeftPage.FuturePage;
                if (frontPage1 == null || backPage1 == null || futurePage1 == null)
                    return;
                ContentPresenter frontPage2 = this.m_refPARTRightPage.FrontPage;
                ContentPresenter backPage2 = this.m_refPARTRightPage.BackPage;
                ContentPresenter futurePage2 = this.m_refPARTRightPage.FuturePage;
                if (frontPage2 == null || backPage2 == null || futurePage2 == null)
                    return;
                Visibility visibility1 = Visibility.Visible;
                Visibility visibility2 = Visibility.Visible;
                Visibility visibility3 = Visibility.Visible;
                Visibility visibility4 = Visibility.Visible;
                Visibility visibility5 = Visibility.Visible;
                Visibility visibility6 = Visibility.Visible;
                Visibility visibility7 = Visibility.Visible;
                Visibility visibility8 = Visibility.Visible;
                if (this.ItemTemplateSelector != null)
                {
                    if (frontPage1.ContentTemplateSelector == null)
                        frontPage1.ContentTemplateSelector = this.ItemTemplateSelector;
                    if (backPage1.ContentTemplateSelector == null)
                        backPage1.ContentTemplateSelector = this.ItemTemplateSelector;
                    if (futurePage1.ContentTemplateSelector == null)
                        futurePage1.ContentTemplateSelector = this.ItemTemplateSelector;
                    if (frontPage2.ContentTemplateSelector == null)
                        frontPage2.ContentTemplateSelector = this.ItemTemplateSelector;
                    if (backPage2.ContentTemplateSelector == null)
                        backPage2.ContentTemplateSelector = this.ItemTemplateSelector;
                    if (futurePage2.ContentTemplateSelector == null)
                        futurePage2.ContentTemplateSelector = this.ItemTemplateSelector;
                }
                else if (this.ItemTemplate != null)
                {
                    if (frontPage1.ContentTemplateSelector == null)
                        frontPage1.ContentTemplate = this.ItemTemplate;
                    if (backPage1.ContentTemplateSelector == null)
                        backPage1.ContentTemplate = this.ItemTemplate;
                    if (futurePage1.ContentTemplateSelector == null)
                        futurePage1.ContentTemplate = this.ItemTemplate;
                    if (frontPage2.ContentTemplateSelector == null)
                        frontPage2.ContentTemplate = this.ItemTemplate;
                    if (backPage2.ContentTemplateSelector == null)
                        backPage2.ContentTemplate = this.ItemTemplate;
                    if (futurePage2.ContentTemplateSelector == null)
                        futurePage2.ContentTemplate = this.ItemTemplate;
                }
                if (!this.IsReadingFromRightToLeft)
                {
                    Visibility visibility9 = this.CurrentSheetIndex == 1 ? Visibility.Hidden : Visibility.Visible;
                    this.m_refPARTRightPage.EnableDragging();
                    if (this.CurrentSheetIndex == val1)
                    {
                        if (!flag)
                            visibility2 = Visibility.Hidden;
                        visibility8 = Visibility.Hidden;
                        this.m_refPARTRightPage.SetIsLastPage(true);
                    }
                    else if (this.CurrentSheetIndex == val1 + 1)
                    {
                        visibility2 = Visibility.Hidden;
                        this.m_refPARTRightPage.SetIsLastPage(true);
                    }
                    else
                        this.m_refPARTRightPage.SetIsLastPage(false);
                    if (this.CurrentSheetIndex == val1 - 1 && !flag)
                        visibility8 = Visibility.Hidden;
                    if (this.CurrentSheetIndex == 0)
                    {
                        frontPage1.Content = (object)null;
                        backPage1.Content = (object)null;
                        futurePage1.Content = (object)null;
                        this.m_refPARTLeftPage.SetBackPageNumber("");
                        this.m_refPARTLeftPage.SetFrontPageNumber("");
                        this.m_refPARTLeftPage.SetFuturePageNumber("");
                        this.m_refPARTLeftPage.IsEnabled = false;
                        visibility1 = Visibility.Hidden;
                    }
                    else
                    {
                        frontPage1.Content = GetPage(2 * (CurrentSheetIndex - 1) + 1);
                        backPage1.Content = GetPage(2 * (CurrentSheetIndex - 1));
                        futurePage1.Content = GetPage(2 * (CurrentSheetIndex - 1) - 1);
                        m_refPARTLeftPage.SetFrontPageNumber((2 * (CurrentSheetIndex - 1) + 1 + 1).ToString() + "/" + Items.Count.ToString());
                        m_refPARTLeftPage.SetBackPageNumber((2 * (CurrentSheetIndex - 1) + 1).ToString() + "/" + Items.Count.ToString());
                        m_refPARTLeftPage.SetFuturePageNumber((2 * (CurrentSheetIndex - 1) - 1 + 1).ToString() + "/" + Items.Count.ToString());
                        m_refPARTLeftPage.IsEnabled = true;
                    }
                    frontPage2.Content = GetPage(2 * CurrentSheetIndex);
                    backPage2.Content = GetPage(2 * CurrentSheetIndex + 1);
                    futurePage2.Content = this.GetPage(2 * CurrentSheetIndex + 2);
                    m_refPARTRightPage.SetFrontPageNumber((2 * CurrentSheetIndex + 1).ToString() + "/" + Items.Count.ToString());
                    m_refPARTRightPage.SetBackPageNumber((2 * CurrentSheetIndex + 1 + 1).ToString() + "/" + Items.Count.ToString());
                    m_refPARTRightPage.SetFuturePageNumber((2 * CurrentSheetIndex + 2 + 1).ToString() + "/" + Items.Count.ToString());
                    m_refPARTLeftPage.Visibility = visibility1;
                    m_refPARTRightPage.Visibility = visibility2;
                    frontPage1.Visibility = visibility3;
                    backPage1.Visibility = visibility4;
                    futurePage1.Visibility = visibility9;
                    frontPage2.Visibility = visibility6;
                    backPage2.Visibility = visibility7;
                    futurePage2.Visibility = visibility8;
                }
                else
                {
                    Visibility visibility9 = this.CurrentSheetIndex == 1 ? Visibility.Hidden : Visibility.Visible;
                    this.m_refPARTLeftPage.EnableDragging();
                    if (this.CurrentSheetIndex == val1)
                    {
                        if (!flag)
                            visibility1 = Visibility.Hidden;
                        visibility5 = Visibility.Hidden;
                        this.m_refPARTLeftPage.SetIsLastPage(true);
                    }
                    else if (this.CurrentSheetIndex == val1 + 1)
                    {
                        visibility1 = Visibility.Hidden;
                        this.m_refPARTLeftPage.SetIsLastPage(true);
                    }
                    else
                        this.m_refPARTLeftPage.SetIsLastPage(false);
                    if (this.CurrentSheetIndex == val1 - 1 && !flag)
                        visibility5 = Visibility.Hidden;
                    if (this.CurrentSheetIndex == 0)
                    {
                        frontPage2.Content = (object)null;
                        backPage2.Content = (object)null;
                        futurePage2.Content = (object)null;
                        this.m_refPARTLeftPage.SetBackPageNumber("");
                        this.m_refPARTLeftPage.SetFrontPageNumber("");
                        this.m_refPARTLeftPage.SetFuturePageNumber("");
                        this.m_refPARTRightPage.IsEnabled = false;
                        visibility2 = Visibility.Hidden;
                    }
                    else
                    {
                        frontPage2.Content = this.GetPage(2 * (this.CurrentSheetIndex - 1) + 1);
                        backPage2.Content = this.GetPage(2 * (this.CurrentSheetIndex - 1));
                        futurePage2.Content = this.GetPage(2 * (this.CurrentSheetIndex - 1) - 1);
                        this.m_refPARTLeftPage.SetFrontPageNumber((string)(object)(2 * (this.CurrentSheetIndex - 1) + 1 + 1) + (object)"/" + (string)(object)this.Items.Count);
                        this.m_refPARTLeftPage.SetBackPageNumber((string)(object)(2 * (this.CurrentSheetIndex - 1) + 1) + (object)"/" + (string)(object)this.Items.Count);
                        this.m_refPARTLeftPage.SetFuturePageNumber((string)(object)(2 * (this.CurrentSheetIndex - 1) - 1 + 1) + (object)"/" + (string)(object)this.Items.Count);
                        this.m_refPARTRightPage.IsEnabled = true;
                    }
                    frontPage1.Content = this.GetPage(2 * this.CurrentSheetIndex);
                    backPage1.Content = this.GetPage(2 * this.CurrentSheetIndex + 1);
                    futurePage1.Content = this.GetPage(2 * this.CurrentSheetIndex + 2);
                    this.m_refPARTRightPage.SetFrontPageNumber((string)(object)(2 * this.CurrentSheetIndex + 1) + (object)"/" + (string)(object)this.Items.Count);
                    this.m_refPARTRightPage.SetBackPageNumber((string)(object)(2 * this.CurrentSheetIndex + 1 + 1) + (object)"/" + (string)(object)this.Items.Count);
                    this.m_refPARTRightPage.SetFuturePageNumber((string)(object)(2 * this.CurrentSheetIndex + 2 + 1) + (object)"/" + (string)(object)this.Items.Count);
                    this.m_refPARTRightPage.Visibility = visibility2;
                    this.m_refPARTLeftPage.Visibility = visibility1;
                    frontPage2.Visibility = visibility6;
                    backPage2.Visibility = visibility7;
                    futurePage2.Visibility = visibility9;
                    frontPage1.Visibility = visibility3;
                    backPage1.Visibility = visibility4;
                    futurePage1.Visibility = visibility5;
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this) || !MathHelper.IsValidNumeric(constraint.Width) || !MathHelper.IsValidNumeric(constraint.Height))
                return base.MeasureOverride(constraint);
            Size availableSize = constraint;
            if (!double.IsNaN(this.ReferencePageSize.Width) && !double.IsNaN(this.ReferencePageSize.Height))
            {
                double width = this.ReferencePageSize.Width * 2.0;
                double height = this.ReferencePageSize.Height;
                Size uniformRatio = VisualHelper.ComputeUniformRatio(new Size(width, height), constraint);
                availableSize = new Size(width * uniformRatio.Width, height * uniformRatio.Height);
            }
            if (this.VisualChildrenCount > 0)
            {
                UIElement uiElement = this.GetVisualChild(0) as UIElement;
                if (uiElement != null)
                    uiElement.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return base.ArrangeOverride(arrangeBounds);
            if (this.VisualChildrenCount > 0)
            {
                UIElement uiElement = this.GetVisualChild(0) as UIElement;
                if (uiElement != null)
                {
                    uiElement.Arrange(new Rect(0.0, 0.0, arrangeBounds.Width, arrangeBounds.Height));
                    return arrangeBounds;
                }
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        private void OnPageDropped(object sender, RoutedEventArgs e)
        {
            this.RaisePageDroppedEvent();
            if (this.m_refPARTLeftPage != null)
                this.m_refPARTLeftPage.EnableManipulation();
            if (this.m_refPARTRightPage == null)
                return;
            this.m_refPARTRightPage.EnableManipulation();
        }

        private void OnLeftPageTurned(object sender, RoutedEventArgs e)
        {
            if (this.m_nNextSheet.HasValue)
            {
                int num = this.m_nNextSheet.Value;
                this.SetValue(Book.CurrentSheetIndexPropertyKey, (object)num);
                this.RaisePageTurnedEvent(num * 2);
                this.m_nNextSheet = new int?();
            }
            else
            {
                int num = this.IsReadingFromRightToLeft ? this.CurrentSheetIndex + 1 : this.CurrentSheetIndex - 1;
                this.SetValue(Book.CurrentSheetIndexPropertyKey, (object)num);
                this.RaisePageTurnedEvent(this.CurrentSheetIndex * 2);
            }
            if (this.IsEnabled)
                this.m_refIdleAnimationTimer.Start();
            this.m_refPARTRightPage.EnableDragging();
            this.m_refPARTLeftPage.EnableManipulation();
        }

        private void OnLeftPageDraggingStarted(object sender, RoutedEventArgs e)
        {
            if (this.m_refPARTLeftPage == null || this.m_refPARTRightPage == null)
                return;
            Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 1);
            Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 0);
            this.RaiseDraggingStartedEvent();
            if (!this.IsPageDragging)
            {
                this.m_refIdleAnimationTimer.Stop();
                this.SetValue(Book.IsPageDraggingPropertyKey, (object)true);
                this.m_refPARTRightPage.DisableDragging();
                this.m_refPARTRightPage.ResetCornerAnimation();
                if (this.IsReadingFromRightToLeft && this.CurrentSheetIndex == (this.Items.Count + 1) / 2 - 1 || !this.IsReadingFromRightToLeft && this.CurrentSheetIndex == 1)
                    this.m_refPARTLeftPage.DisableManipulation();
            }
            e.Handled = true;
        }

        private void OnLeftPageDraggingEnded(object sender, RoutedEventArgs e)
        {
            if (this.IsPageDragging)
            {
                this.SetValue(Book.IsPageDraggingPropertyKey, (object)false);
                this.m_refPARTRightPage.EnableDragging();
                this.RefreshSheetsContent();
                if (this.IsEnabled)
                    this.m_refIdleAnimationTimer.Start();
            }
            e.Handled = true;
        }

        private void OnRightPageTurned(object sender, RoutedEventArgs e)
        {
            if (this.m_nNextSheet.HasValue)
            {
                int num = this.m_nNextSheet.Value;
                this.SetValue(Book.CurrentSheetIndexPropertyKey, (object)num);
                this.RaisePageTurnedEvent(num * 2 + 1);
                this.m_nNextSheet = new int?();
            }
            else
            {
                int num = this.IsReadingFromRightToLeft ? this.CurrentSheetIndex - 1 : this.CurrentSheetIndex + 1;
                this.SetValue(Book.CurrentSheetIndexPropertyKey, (object)num);
                this.RaisePageTurnedEvent(this.CurrentSheetIndex * 2 + 1);
            }
            if (this.IsEnabled)
                this.m_refIdleAnimationTimer.Start();
            this.m_refPARTLeftPage.EnableDragging();
            this.m_refPARTRightPage.EnableManipulation();
        }

        private void OnRightPageDraggingStarted(object sender, RoutedEventArgs e)
        {
            if (this.m_refPARTLeftPage == null || this.m_refPARTRightPage == null)
                return;
            Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 0);
            Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 1);
            this.RaiseDraggingStartedEvent();
            if (!this.IsPageDragging)
            {
                this.m_refIdleAnimationTimer.Stop();
                this.SetValue(Book.IsPageDraggingPropertyKey, (object)true);
                this.m_refPARTLeftPage.DisableDragging();
                this.m_refPARTLeftPage.ResetCornerAnimation();
                if (!this.IsReadingFromRightToLeft && this.CurrentSheetIndex == (this.Items.Count + 1) / 2 - 1 || this.IsReadingFromRightToLeft && this.CurrentSheetIndex == 1)
                    this.m_refPARTRightPage.DisableManipulation();
            }
            e.Handled = true;
        }

        private void OnRightPageDraggingEnded(object sender, RoutedEventArgs e)
        {
            if (this.IsPageDragging)
            {
                this.SetValue(Book.IsPageDraggingPropertyKey, (object)false);
                this.m_refPARTLeftPage.EnableDragging();
                this.RefreshSheetsContent();
                if (this.IsEnabled)
                    this.m_refIdleAnimationTimer.Start();
            }
            e.Handled = true;
        }

        public void TurnLeftPage(bool? turnFromTop = null)
        {
            if ((this.CurrentSheetIndex <= 0 || this.IsReadingFromRightToLeft) && (this.CurrentSheetIndex + 1 > (this.Items.Count + 1) / 2 || !this.IsReadingFromRightToLeft) || (this.m_refPARTLeftPage == null || this.m_refPARTRightPage == null || this.IsPageDragging))
                return;
            this.m_refIdleAnimationTimer.Stop();
            this.m_refPARTRightPage.ResetCornerAnimation();
            Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 1);
            Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 0);
            if (!turnFromTop.HasValue)
            {
                this.m_refPARTLeftPage.TurnPageFromCorner(this.IsTurningFromTop ? CornerOrigin.TopLeft : CornerOrigin.BottomLeft, this.TurnAnimationDuration);
            }
            else
            {
                BookPage BookPage = this.m_refPARTLeftPage;
                bool? nullable = turnFromTop;
                int num = (!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0 ? 0 : 2;
                int animationDuration = this.TurnAnimationDuration;
                BookPage.TurnPageFromCorner((CornerOrigin)num, animationDuration);
            }
        }

        public void TurnRightPage(bool? turnFromTop = null)
        {
            if ((this.CurrentSheetIndex <= 0 || !this.IsReadingFromRightToLeft) && (this.CurrentSheetIndex + 1 > (this.Items.Count + 1) / 2 || this.IsReadingFromRightToLeft) || (this.m_refPARTLeftPage == null || this.m_refPARTRightPage == null || this.IsPageDragging))
                return;
            this.m_refIdleAnimationTimer.Stop();
            this.m_refPARTLeftPage.ResetCornerAnimation();
            Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 0);
            Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 1);
            if (!turnFromTop.HasValue)
            {
                this.m_refPARTRightPage.TurnPageFromCorner(this.IsTurningFromTop ? CornerOrigin.TopRight : CornerOrigin.BottomRight, this.TurnAnimationDuration);
            }
            else
            {
                BookPage BookPage = this.m_refPARTRightPage;
                bool? nullable = turnFromTop;
                int num = (!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0 ? 1 : 3;
                int animationDuration = this.TurnAnimationDuration;
                BookPage.TurnPageFromCorner((CornerOrigin)num, animationDuration);
            }
        }

        public PageKind GetPageKindContainingPoint(Point p)
        {
            if (VisualHelper.IsWithinElementBounds((UIElement)this.m_refPARTLeftPage, this.TranslatePoint(p, (UIElement)this.m_refPARTLeftPage)))
                return PageKind.Left;
            if (VisualHelper.IsWithinElementBounds((UIElement)this.m_refPARTRightPage, this.TranslatePoint(p, (UIElement)this.m_refPARTRightPage)))
                return PageKind.Right;
            else
                return PageKind.None;
        }

        public int GetLeftPageIndex()
        {
            return 2 * (this.CurrentSheetIndex - 1) + 1;
        }

        public int GetRightPageIndex()
        {
            return 2 * this.CurrentSheetIndex;
        }

        public void NavigateToPage(int pageNum, bool useAnimation = false)
        {
            if (this.m_refPARTLeftPage == null || this.m_refPARTRightPage == null)
                return;
            if (pageNum > this.Items.Count)
                pageNum = this.Items.Count;
            if (pageNum < 0)
                pageNum = 0;
            int num1 = (pageNum + 1) / 2;
            this.m_refPARTLeftPage.ResetCornerAnimation();
            this.m_refPARTRightPage.ResetCornerAnimation();
            if (useAnimation)
            {
                this.m_refIdleAnimationTimer.Stop();
                int num2 = this.Items.Count / 2;
                ContentPresenter backPage1 = this.m_refPARTLeftPage.BackPage;
                ContentPresenter futurePage1 = this.m_refPARTLeftPage.FuturePage;
                if (backPage1 == null || futurePage1 == null)
                    return;
                ContentPresenter backPage2 = this.m_refPARTRightPage.BackPage;
                ContentPresenter futurePage2 = this.m_refPARTRightPage.FuturePage;
                if (backPage2 == null || futurePage2 == null)
                    return;
                if (num1 > this.CurrentSheetIndex && !this.IsReadingFromRightToLeft || num1 < this.CurrentSheetIndex && this.IsReadingFromRightToLeft)
                {
                    this.m_refPARTLeftPage.DisableDragging();
                    if (!this.IsReadingFromRightToLeft)
                    {
                        if (num1 == num2 && this.Items.Count % 2 != 1)
                            futurePage2.Visibility = Visibility.Hidden;
                        Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 0);
                        Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 1);
                        if (num1 != this.CurrentSheetIndex + 1)
                        {
                            this.m_nNextSheet = new int?(num1);
                            futurePage2.Content = this.GetPage(num1 * 2);
                            backPage2.Content = this.GetPage(num1 * 2 - 1);
                        }
                    }
                    else
                    {
                        if (num1 == 0)
                            futurePage2.Visibility = Visibility.Hidden;
                        Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 0);
                        Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 1);
                        if (num1 != this.CurrentSheetIndex - 1)
                        {
                            this.m_nNextSheet = new int?(num1);
                            backPage2.Content = this.GetPage(num1 * 2);
                            futurePage2.Content = this.GetPage(num1 * 2 - 1);
                        }
                    }
                    this.m_refPARTRightPage.TurnPageFromCorner(this.IsTurningFromTop ? CornerOrigin.TopRight : CornerOrigin.BottomRight, this.TurnAnimationDuration);
                }
                else if (num1 > this.CurrentSheetIndex && this.IsReadingFromRightToLeft || num1 < this.CurrentSheetIndex && !this.IsReadingFromRightToLeft)
                {
                    if (!this.IsReadingFromRightToLeft)
                    {
                        this.m_refPARTRightPage.DisableDragging();
                        if (num1 == 0)
                            futurePage1.Visibility = Visibility.Hidden;
                        Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 1);
                        Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 0);
                        if (num1 != this.CurrentSheetIndex - 1)
                        {
                            this.m_nNextSheet = new int?(num1);
                            backPage1.Content = this.GetPage(num1 * 2);
                            futurePage1.Content = this.GetPage(num1 * 2 - 1);
                        }
                    }
                    else
                    {
                        if (num1 == num2 && this.Items.Count % 2 != 1)
                            futurePage1.Visibility = Visibility.Hidden;
                        Panel.SetZIndex((UIElement)this.m_refPARTLeftPage, 1);
                        Panel.SetZIndex((UIElement)this.m_refPARTRightPage, 0);
                        if (num1 != this.CurrentSheetIndex + 1)
                        {
                            this.m_nNextSheet = new int?(num1);
                            futurePage1.Content = this.GetPage(num1 * 2);
                            backPage1.Content = this.GetPage(num1 * 2 - 1);
                        }
                    }
                    this.m_refPARTLeftPage.TurnPageFromCorner(this.IsTurningFromTop ? CornerOrigin.TopLeft : CornerOrigin.BottomLeft, this.TurnAnimationDuration);
                }
                else
                {
                    if (!this.IsEnabled)
                        return;
                    this.m_refIdleAnimationTimer.Start();
                }
            }
            else
                this.SetCurrentSheetIndex(num1);
        }

        public void Dispose()
        {
            CommandBindings.Clear();
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            IsEnabledChanged -= OnIsEnabledChanged;
            if (m_refPARTLeftPage != null)
            {
                m_refPARTLeftPage.DraggingStarted -= OnLeftPageDraggingStarted;
                m_refPARTLeftPage.DraggingEnded -= OnLeftPageDraggingEnded;
                m_refPARTLeftPage.PageTurned -= OnLeftPageTurned;
                m_refPARTLeftPage.IdleAnimationEnded -= OnIdleAnimationEnded;
                m_refPARTLeftPage.PageDropped -= OnPageDropped;
                m_refPARTLeftPage.Dispose();
                m_refPARTLeftPage = null;
            }
            if (this.m_refPARTRightPage != null)
            {
                this.m_refPARTRightPage.DraggingStarted -= new RoutedEventHandler(this.OnRightPageDraggingStarted);
                this.m_refPARTRightPage.DraggingEnded -= new RoutedEventHandler(this.OnRightPageDraggingEnded);
                this.m_refPARTRightPage.PageTurned -= new RoutedEventHandler(this.OnRightPageTurned);
                this.m_refPARTRightPage.IdleAnimationEnded -= new RoutedEventHandler(this.OnIdleAnimationEnded);
                this.m_refPARTRightPage.PageDropped -= new RoutedEventHandler(this.OnPageDropped);
                this.m_refPARTRightPage.Dispose();
                this.m_refPARTRightPage = (BookPage)null;
            }
            if (this.m_refIdleAnimationTimer == null)
                return;
            this.m_refIdleAnimationTimer.Tick -= new EventHandler(this.OnIdleAnimationTimerTick);
            this.m_refIdleAnimationTimer.Stop();
        }
    }
}