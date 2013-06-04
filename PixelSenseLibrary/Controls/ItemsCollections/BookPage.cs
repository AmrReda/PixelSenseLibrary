using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Converters;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using PixelSenseLibrary.Commands;
using PixelSenseLibrary.Controls.Common;
using PixelSenseLibrary.Enums;
using PixelSenseLibrary.Extensions;
using PixelSenseLibrary.Helpers;
using PixelSenseLibrary.Structs;


namespace PixelSenseLibrary.Controls.ItemsCollections
{
    [TemplatePart(Name = "PART_IdleAnimationPathBottomLeft", Type = typeof(Path)),
     TemplatePart(Name = "PART_SideLeft", Type = typeof(Rectangle)),
     TemplatePart(Name = "PART_SideRight", Type = typeof(Rectangle)),
     TemplatePart(Name = "PART_CornerTopLeft", Type = typeof(Rectangle)),
     TemplatePart(Name = "PART_CornerTopRight", Type = typeof(Rectangle)),
     TemplatePart(Name = "PART_ClippingFrontPage", Type = typeof(CombinedGeometry)),
     TemplatePart(Name = "PART_CornerBottomRight", Type = typeof(Rectangle)),
     TemplatePart(Name = "PART_FrontPage", Type = typeof(ContentPresenter)),
     TemplatePart(Name = "PART_FuturePage", Type = typeof(ContentPresenter)),
     TemplatePart(Name = "PART_TransformTranslate", Type = typeof(TranslateTransform)),
     TemplatePart(Name = "PART_TransformRotate", Type = typeof(RotateTransform)),
     TemplatePart(Name = "PART_ReflectionBackground", Type = typeof(LinearGradientBrush)),
     TemplatePart(Name = "PART_ReflectionCanvas", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_ShadowBackground", Type = typeof(LinearGradientBrush)),
     TemplatePart(Name = "PART_ShadowCanvas", Type = typeof(Canvas)),
     TemplatePart(Name = "PART_IdleAnimationPathTopRight", Type = typeof(Path)),
     TemplatePart(Name = "PART_IdleAnimationPathBottomRight", Type = typeof(Path)),
     TemplatePart(Name = "PART_IdleAnimationPathTopLeft", Type = typeof(Path)),
     TemplatePart(Name = "PART_CornerBottomLeft", Type = typeof(Rectangle)),
     TemplatePart(Name = "PART_FrontPageNumber", Type = typeof(TextBlock)),
     TemplatePart(Name = "PART_ClippingBackPage", Type = typeof(PathGeometry)),
     TemplatePart(Name = "PART_BackPageNumber", Type = typeof(TextBlock)),
     TemplatePart(Name = "PART_FuturePageNumber", Type = typeof(TextBlock)),
     TemplatePart(Name = "PART_BackPage", Type = typeof(ContentPresenter))]
    public class BookPage : ContentControl, IDisposable
    {
        protected CornerOrigin m_refCornerOrigin = CornerOrigin.BottomRight;
        protected bool m_bDraggingEnabled = true;
        protected DispatcherTimer m_refTapTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds((double)BookPage.s_nTAP_TIME_THRESHOLD)
        };
        protected DispatcherTimer m_refFlickTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds((double)BookPage.s_nFLICK_TIME_THRESHOLD)
        };
        protected Point m_ptTapPoint = new Point();
        protected DateTime m_dateStartFlick = DateTime.Now;
        protected bool m_bCanStarNanipulation = true;
        public static int s_nANIMATION_DURATION = 500;
        public static int s_nTAP_TIME_THRESHOLD = 500;
        public static int s_nTAP_DISTANCE_THRESHOLD = 12;
        public static int s_nIDLE_ANIMATION_DURATION = 2500;
        public static int s_nFLICK_TIME_THRESHOLD = 500;
        public static int s_nFLICK_DISTANCE_THRESHOLD = 12;
        public static double s_dOFFSET_TO_AVOID_GLITCH = 0.1;
        private static readonly DependencyPropertyKey StatusPropertyKey = DependencyProperty.RegisterReadOnly("Status", typeof(PageStatus), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)PageStatus.Idle, new PropertyChangedCallback(BookPage.OnStatusChanged)));
        public static readonly DependencyProperty StatusProperty = BookPage.StatusPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsLeftPagePropertyKey = DependencyProperty.RegisterReadOnly("IsLeftPage", typeof(bool), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
        public static readonly DependencyProperty IsLeftPageProperty = BookPage.IsLeftPagePropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsRightPagePropertyKey = DependencyProperty.RegisterReadOnly("IsRightPage", typeof(bool), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)true));
        public static readonly DependencyProperty IsRightPageProperty = BookPage.IsRightPagePropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(BookPage.OnIsDraggingChanged)));
        public static readonly DependencyProperty IsDraggingProperty = BookPage.IsDraggingPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsLastPagePropertyKey = DependencyProperty.RegisterReadOnly("IsLastPage", typeof(bool), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(BookPage.OnIsLastPageChanged)));
        public static readonly DependencyProperty IsLastPageProperty = BookPage.IsLastPagePropertyKey.DependencyProperty;
        public static DependencyProperty CornerPointProperty = DependencyProperty.Register("CornerPoint", typeof(Point), typeof(BookPage));
        public static readonly DependencyProperty PageKindProperty = DependencyProperty.Register("PageKind", typeof(PageKind), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)PageKind.Right, new PropertyChangedCallback(BookPage.OPageKindChanged)));
        public static readonly DependencyProperty TurnAnimationDurationProperty = DependencyProperty.Register("TurnAnimationDuration", typeof(int), typeof(BookPage), (PropertyMetadata)new FrameworkPropertyMetadata((object)500));
        public static readonly RoutedEvent PageTurnedEvent = EventManager.RegisterRoutedEvent("PageTurned", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BookPage));
        public static readonly RoutedEvent PageDroppedEvent = EventManager.RegisterRoutedEvent("PageDropped", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BookPage));
        public static readonly RoutedEvent DraggingStartedEvent = EventManager.RegisterRoutedEvent("DraggingStarted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BookPage));
        public static readonly RoutedEvent DraggingEndedEvent = EventManager.RegisterRoutedEvent("DraggingEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BookPage));
        public static readonly RoutedEvent IdleAnimationEndedEvent = EventManager.RegisterRoutedEvent("IdleAnimationEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BookPage));
        protected Point m_ptCornerPoint;
        protected bool m_bContactAlreadyInsidePage;
        protected PointAnimationBase m_refAnimation;
        protected PathGeometry m_refPART_ClippingBackPage;
        protected CombinedGeometry m_refPART_ClippingFrontPage;
        protected Rectangle m_rctSideLeft;
        protected Rectangle m_rctSideRight;
        protected Rectangle m_rctCornerTopLeft;
        protected Rectangle m_rctCornerTopRight;
        protected Rectangle m_rctCornerBottomLeft;
        protected Rectangle m_rctCornerBottomRight;
        protected Canvas m_refPART_ReflectionCanvas;
        protected LinearGradientBrush m_refPART_ReflectionBackground;
        protected Canvas m_refPART_ShadowCanvas;
        protected LinearGradientBrush m_refPART_ShadowBackground;
        protected ContentPresenter m_refPART_FrontPage;
        protected ContentPresenter m_refPART_BackPage;
        protected ContentPresenter m_refPART_FuturePage;
        protected RotateTransform m_refPART_TransformRotate;
        protected TranslateTransform m_refPART_TransformTranslate;
        protected Path m_refPART_IdleAnimationPathTopRight;
        protected Path m_refPART_IdleAnimationPathBottomRight;
        protected Path m_refPART_IdleAnimationPathTopLeft;
        protected Path m_refPART_IdleAnimationPathBottomLeft;
        protected TextBlock m_refPARTFrontPageNumber;
        protected TextBlock m_refPARTBackPageNumber;
        protected TextBlock m_refPARTFuturePageNumber;

        public PageStatus Status
        {
            get
            {
                return (PageStatus)this.GetValue(BookPage.StatusProperty);
            }
        }

        public bool IsLeftPage
        {
            get
            {
                return (bool)this.GetValue(BookPage.IsLeftPageProperty);
            }
        }

        public bool IsRightPage
        {
            get
            {
                return (bool)this.GetValue(BookPage.IsRightPageProperty);
            }
        }

        public bool IsDragging
        {
            get
            {
                return (bool)this.GetValue(BookPage.IsDraggingProperty);
            }
        }

        public bool IsLastPage
        {
            get
            {
                return (bool)this.GetValue(BookPage.IsLastPageProperty);
            }
        }

        private Point CornerPoint
        {
            get
            {
                return (Point)this.GetValue(BookPage.CornerPointProperty);
            }
            set
            {
                this.SetValue(BookPage.CornerPointProperty, (object)value);
            }
        }

        public PageKind PageKind
        {
            get
            {
                return (PageKind)this.GetValue(BookPage.PageKindProperty);
            }
            set
            {
                this.SetValue(BookPage.PageKindProperty, (object)value);
            }
        }

        public int TurnAnimationDuration
        {
            get
            {
                return (int)this.GetValue(BookPage.TurnAnimationDurationProperty);
            }
            set
            {
                this.SetValue(BookPage.TurnAnimationDurationProperty, (object)value);
            }
        }

        public ContentPresenter FrontPage
        {
            get
            {
                return this.m_refPART_FrontPage;
            }
        }

        public ContentPresenter BackPage
        {
            get
            {
                return this.m_refPART_BackPage;
            }
        }

        public ContentPresenter FuturePage
        {
            get
            {
                return this.m_refPART_FuturePage;
            }
        }

        public event RoutedEventHandler PageTurned
        {
            add
            {
                this.AddHandler(BookPage.PageTurnedEvent, (Delegate)value);
            }
            remove
            {
                this.RemoveHandler(BookPage.PageTurnedEvent, (Delegate)value);
            }
        }

        public event RoutedEventHandler PageDropped
        {
            add
            {
                this.AddHandler(BookPage.PageDroppedEvent, (Delegate)value);
            }
            remove
            {
                this.RemoveHandler(BookPage.PageDroppedEvent, (Delegate)value);
            }
        }

        public event RoutedEventHandler DraggingStarted
        {
            add
            {
                this.AddHandler(BookPage.DraggingStartedEvent, (Delegate)value);
            }
            remove
            {
                this.RemoveHandler(BookPage.DraggingStartedEvent, (Delegate)value);
            }
        }

        public event RoutedEventHandler DraggingEnded
        {
            add
            {
                this.AddHandler(BookPage.DraggingEndedEvent, (Delegate)value);
            }
            remove
            {
                this.RemoveHandler(BookPage.DraggingEndedEvent, (Delegate)value);
            }
        }

        public event RoutedEventHandler IdleAnimationEnded
        {
            add
            {
                this.AddHandler(BookPage.IdleAnimationEndedEvent, (Delegate)value);
            }
            remove
            {
                this.RemoveHandler(BookPage.IdleAnimationEndedEvent, (Delegate)value);
            }
        }

        static BookPage()
        {
            Type forType = typeof(BookPage);
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(forType, (PropertyMetadata)new FrameworkPropertyMetadata((object)forType));
        }

        public BookPage()
        {
            this.CommandBindings.Add(new CommandBinding((ICommand)BookPageCommands.EnableDragging, new ExecutedRoutedEventHandler(this.ExecuteEnableDragging), new CanExecuteRoutedEventHandler(this.CanEnableDragging)));
            this.CommandBindings.Add(new CommandBinding((ICommand)BookPageCommands.DisableDragging, new ExecutedRoutedEventHandler(this.ExecuteDisableDragging), new CanExecuteRoutedEventHandler(this.CanDisableDragging)));
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            this.Loaded += new RoutedEventHandler(this.OnLoaded);
            this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
            this.m_refTapTimer.Tick += new EventHandler(this.OnTimerTick);
            this.m_refFlickTimer.Tick += new EventHandler(this.OnTimerTick);
        }

        protected void SetStatus(Enums.PageStatus value)
        {
            this.SetValue(BookPage.StatusPropertyKey, (object)value);
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BookPage)d).OnStatusChanged(e);
        }

        protected virtual void OnStatusChanged(DependencyPropertyChangedEventArgs e)
        {
            Enums.PageStatus PageStatus = (Enums.PageStatus)e.NewValue;
            if (PageStatus == PageStatus.Dragging)
                this.SetValue(BookPage.IsDraggingPropertyKey, (object)true);
            else
                this.SetValue(BookPage.IsDraggingPropertyKey, (object)false);
            if (this.m_refPART_ShadowCanvas != null)
                this.m_refPART_ShadowCanvas.Visibility = PageStatus == PageStatus.Idle ? Visibility.Hidden : Visibility.Visible;
            if (this.m_refPART_ReflectionCanvas == null)
                return;
            this.m_refPART_ReflectionCanvas.Visibility = PageStatus == PageStatus.Idle ? Visibility.Hidden : Visibility.Visible;
        }

        protected void SetIsLeftPage(bool value)
        {
            this.SetValue(BookPage.IsLeftPagePropertyKey, value);
        }

        protected void SetIsRightPage(bool value)
        {
            this.SetValue(BookPage.IsRightPagePropertyKey, value);
        }

        protected void SetIsDragging(bool value)
        {
            this.SetValue(BookPage.IsDraggingPropertyKey, value);
        }

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BookPage)d).OnIsDraggingChanged(e);
        }

        protected virtual void OnIsDraggingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                this.RaiseDraggingEndedEvent();
            else
                this.RaiseDraggingStartedEvent();
        }

        internal void SetIsLastPage(bool value)
        {
            this.SetValue(BookPage.IsLastPagePropertyKey, value);
        }

        private static void OnIsLastPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BookPage)d).OnIsLastPageChanged(e);
        }

        protected virtual void OnIsLastPageChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OPageKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BookPage)d).OPageKindChanged(e);
        }

        protected virtual void OPageKindChanged(DependencyPropertyChangedEventArgs e)
        {
            switch ((PageKind)e.NewValue)
            {
                case PageKind.Left:
                    if (this.m_refPARTFrontPageNumber != null)
                        this.m_refPARTFrontPageNumber.HorizontalAlignment = HorizontalAlignment.Left;
                    if (this.m_refPARTBackPageNumber != null)
                        this.m_refPARTBackPageNumber.HorizontalAlignment = HorizontalAlignment.Right;
                    if (this.m_refPARTFuturePageNumber != null)
                        this.m_refPARTFuturePageNumber.HorizontalAlignment = HorizontalAlignment.Left;
                    this.SetValue(BookPage.IsLeftPagePropertyKey, (object)true);
                    this.SetValue(BookPage.IsRightPagePropertyKey, (object)false);
                    break;
                case PageKind.Right:
                    if (this.m_refPARTFrontPageNumber != null)
                        this.m_refPARTFrontPageNumber.HorizontalAlignment = HorizontalAlignment.Right;
                    if (this.m_refPARTBackPageNumber != null)
                        this.m_refPARTBackPageNumber.HorizontalAlignment = HorizontalAlignment.Left;
                    if (this.m_refPARTFuturePageNumber != null)
                        this.m_refPARTFuturePageNumber.HorizontalAlignment = HorizontalAlignment.Right;
                    this.SetValue(BookPage.IsLeftPagePropertyKey, (object)false);
                    this.SetValue(BookPage.IsRightPagePropertyKey, (object)true);
                    break;
            }
        }

        protected RoutedEventArgs RaisePageTurnedEvent()
        {
            return BookPage.RaisePageTurnedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaisePageTurnedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = BookPage.PageTurnedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaisePageDroppedEvent()
        {
            return BookPage.RaisePageDroppedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaisePageDroppedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = BookPage.PageDroppedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaiseDraggingStartedEvent()
        {
            return BookPage.RaiseDraggingStartedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaiseDraggingStartedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = BookPage.DraggingStartedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaiseDraggingEndedEvent()
        {
            return BookPage.RaiseDraggingEndedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaiseDraggingEndedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = BookPage.DraggingEndedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        protected RoutedEventArgs RaiseIdleAnimationEndedEvent()
        {
            return BookPage.RaiseIdleAnimationEndedEvent((DependencyObject)this);
        }

        internal static RoutedEventArgs RaiseIdleAnimationEndedEvent(DependencyObject target)
        {
            if (target == null)
                return (RoutedEventArgs)null;
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = BookPage.IdleAnimationEndedEvent;
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DesignerProperties.GetIsInDesignMode((DependencyObject)this))
                return;
            this.m_refPART_ClippingBackPage = (PathGeometry)this.GetTemplateChild("PART_ClippingBackPage");
            this.m_refPART_ClippingFrontPage = (CombinedGeometry)this.GetTemplateChild("PART_ClippingFrontPage");
            this.m_rctSideLeft = (Rectangle)this.GetTemplateChild("PART_SideLeft");
            CaptureThief.SetParentStealCaptureOrientation((DependencyObject)this.m_rctSideLeft, PixelSenseLibrary.Enums.Orientation.Left);
            this.m_rctSideRight = (Rectangle)this.GetTemplateChild("PART_SideRight");
            CaptureThief.SetParentStealCaptureOrientation((DependencyObject)this.m_rctSideRight, PixelSenseLibrary.Enums.Orientation.Right);
            this.m_rctCornerTopLeft = (Rectangle)this.GetTemplateChild("PART_CornerTopLeft");
            CaptureThief.SetParentStealCaptureOrientation((DependencyObject)this.m_rctCornerTopLeft, (PixelSenseLibrary.Enums.Orientation)19);
            this.m_rctCornerTopRight = (Rectangle)this.GetTemplateChild("PART_CornerTopRight");
            CaptureThief.SetParentStealCaptureOrientation((DependencyObject)this.m_rctCornerTopRight, (PixelSenseLibrary.Enums.Orientation)41);
            this.m_rctCornerBottomLeft = (Rectangle)this.GetTemplateChild("PART_CornerBottomLeft");
            CaptureThief.SetParentStealCaptureOrientation((DependencyObject)this.m_rctCornerBottomLeft, (PixelSenseLibrary.Enums.Orientation)70);
            this.m_rctCornerBottomRight = (Rectangle)this.GetTemplateChild("PART_CornerBottomRight");
            CaptureThief.SetParentStealCaptureOrientation((DependencyObject)this.m_rctCornerBottomRight, (PixelSenseLibrary.Enums.Orientation)140);
            this.m_refPART_ReflectionCanvas = (Canvas)this.GetTemplateChild("PART_ReflectionCanvas");
            this.m_refPART_ReflectionBackground = (LinearGradientBrush)this.GetTemplateChild("PART_ReflectionBackground");
            this.m_refPART_ShadowCanvas = (Canvas)this.GetTemplateChild("PART_ShadowCanvas");
            this.m_refPART_ShadowBackground = (LinearGradientBrush)this.GetTemplateChild("PART_ShadowBackground");
            this.m_refPART_FrontPage = (ContentPresenter)this.GetTemplateChild("PART_FrontPage");
            this.m_refPART_BackPage = (ContentPresenter)this.GetTemplateChild("PART_BackPage");
            this.m_refPART_FuturePage = (ContentPresenter)this.GetTemplateChild("PART_FuturePage");
            this.m_refPART_TransformTranslate = (TranslateTransform)this.GetTemplateChild("PART_TransformTranslate");
            this.m_refPART_TransformRotate = (RotateTransform)this.GetTemplateChild("PART_TransformRotate");
            this.m_refPART_IdleAnimationPathTopRight = (Path)this.GetTemplateChild("PART_IdleAnimationPathTopRight");
            this.m_refPART_IdleAnimationPathTopLeft = (Path)this.GetTemplateChild("PART_IdleAnimationPathTopLeft");
            this.m_refPART_IdleAnimationPathBottomRight = (Path)this.GetTemplateChild("PART_IdleAnimationPathBottomRight");
            this.m_refPART_IdleAnimationPathBottomLeft = (Path)this.GetTemplateChild("PART_IdleAnimationPathBottomLeft");
            this.m_refPARTFrontPageNumber = (TextBlock)this.GetTemplateChild("PART_FrontPageNumber");
            this.m_refPARTBackPageNumber = (TextBlock)this.GetTemplateChild("PART_BackPageNumber");
            this.m_refPARTFuturePageNumber = (TextBlock)this.GetTemplateChild("PART_FuturePageNumber");
            if (this.PageKind == PageKind.Left)
            {
                if (this.m_refPARTFrontPageNumber != null)
                    this.m_refPARTFrontPageNumber.HorizontalAlignment = HorizontalAlignment.Left;
                if (this.m_refPARTBackPageNumber != null)
                    this.m_refPARTBackPageNumber.HorizontalAlignment = HorizontalAlignment.Right;
                if (this.m_refPARTFuturePageNumber != null)
                    this.m_refPARTFuturePageNumber.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (this.PageKind == PageKind.Right)
            {
                if (this.m_refPARTFrontPageNumber != null)
                    this.m_refPARTFrontPageNumber.HorizontalAlignment = HorizontalAlignment.Right;
                if (this.m_refPARTBackPageNumber != null)
                    this.m_refPARTBackPageNumber.HorizontalAlignment = HorizontalAlignment.Left;
                if (this.m_refPARTFuturePageNumber != null)
                    this.m_refPARTFuturePageNumber.HorizontalAlignment = HorizontalAlignment.Right;
            }
            foreach (UIElement uiElement in Enumerable.Where<UIElement>((IEnumerable<UIElement>)new UIElement[6]
      {
        (UIElement) this.m_rctSideLeft,
        (UIElement) this.m_rctSideRight,
        (UIElement) this.m_rctCornerTopLeft,
        (UIElement) this.m_rctCornerBottomLeft,
        (UIElement) this.m_rctCornerTopRight,
        (UIElement) this.m_rctCornerBottomRight
      }, (Func<UIElement, bool>)(r => r != null)))
            {
                uiElement.TouchDown += new EventHandler<TouchEventArgs>(this.OnCornerTouchDown);
                uiElement.TouchMove += new EventHandler<TouchEventArgs>(this.OnCornerTouchMove);
                uiElement.TouchUp += new EventHandler<TouchEventArgs>(this.OnCornerTouchUp);
                uiElement.GotTouchCapture += new EventHandler<TouchEventArgs>(this.OnCornerGotTouchCapture);
                uiElement.LostTouchCapture += new EventHandler<TouchEventArgs>(this.OnCornerLostTouchCapture);
                uiElement.TouchEnter += new EventHandler<TouchEventArgs>(this.OnCornerTouchEnter);
                uiElement.TouchLeave += new EventHandler<TouchEventArgs>(this.OnCornerTouchLeave);
                uiElement.MouseDown += new MouseButtonEventHandler(this.OnCornerMouseDown);
                uiElement.MouseMove += new MouseEventHandler(this.OnCornerMouseMove);
                uiElement.MouseUp += new MouseButtonEventHandler(this.OnCornerMouseUp);
                uiElement.GotMouseCapture += new MouseEventHandler(this.OnCornerGoNouseCapture);
                uiElement.LostMouseCapture += new MouseEventHandler(this.OnCornerLosNouseCapture);
                uiElement.MouseEnter += new MouseEventHandler(this.OnCornerMouseEnter);
                uiElement.MouseLeave += new MouseEventHandler(this.OnCornerMouseLeave);
                CaptureThief.SetAllowsParentToStealCapture((DependencyObject)uiElement, true);
                CaptureThief.SetParentStealCaptureRadiusThreshold((DependencyObject)uiElement, 20.0);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.ApplyParameters(new PageParameters(this.RenderSize));
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ApplyParameters(new PageParameters(this.RenderSize));
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).IsEnabled = false;
        }

        private void OnAnimationCurrentStateInvalidated(object sender, EventArgs e)
        {
            if ((sender as Clock).CurrentState == ClockState.Active)
                return;
            this.ApplyParameters(new PageParameters(this.RenderSize));
            if (this.Status == PageStatus.TurnAnimation)
                this.RaisePageTurnedEvent();
            else if (this.Status == PageStatus.DropAnimation)
                this.RaisePageDroppedEvent();
            this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.Idle);
        }

        private void OnAnimationCurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (this.Status == PageStatus.Dragging)
            {
                (sender as Clock).Controller.Stop();
            }
            else
            {
                PageParameters? page = BookPageComputeHelper.ComputePage(this, this.CornerPoint, this.m_refCornerOrigin);
                this.m_ptCornerPoint = this.CornerPoint;
                if (!page.HasValue)
                    return;
                this.ApplyParameters(page.Value);
            }
        }

        private void OnIdleAnimationCurrentStateInvalidated(object sender, EventArgs e)
        {
            if ((sender as Clock).CurrentState == ClockState.Active)
                return;
            if (this.Status != PageStatus.Idle)
                (sender as Clock).Controller.Stop();
            else
                this.RaiseIdleAnimationEndedEvent();
        }

        private void OnIdleAnimationCurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (this.Status != PageStatus.Idle)
            {
                (sender as Clock).Controller.Stop();
            }
            else
            {
                PageParameters? page = BookPageComputeHelper.ComputePage(this, this.CornerPoint, this.m_refCornerOrigin);
                this.m_ptCornerPoint = this.CornerPoint;
                if (!page.HasValue)
                    return;
                this.ApplyParameters(page.Value);
            }
        }

        private void ExecuteEnableDragging(object sender, ExecutedRoutedEventArgs e)
        {
            this.EnableDragging();
        }

        private void CanEnableDragging(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExecuteDisableDragging(object sender, ExecutedRoutedEventArgs e)
        {
            this.DisableDragging();
        }

        private void CanDisableDragging(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected virtual void ApplyParameters(PageParameters parameters)
        {
            if (this.m_refPART_ReflectionBackground != null)
                this.m_refPART_ReflectionBackground.Opacity = parameters.Page0ShadowOpacity;
            if (this.m_refPART_TransformRotate != null)
            {
                this.m_refPART_TransformRotate.Angle = parameters.Page1RotateAngle;
                this.m_refPART_TransformRotate.CenterX = parameters.Page1RotateCenterX;
                this.m_refPART_TransformRotate.CenterY = parameters.Page1RotateCenterY;
            }
            if (this.m_refPART_TransformTranslate != null)
            {
                this.m_refPART_TransformTranslate.X = parameters.Page1TranslateX;
                this.m_refPART_TransformTranslate.Y = parameters.Page1TranslateY;
            }
            if (this.m_refPART_ClippingBackPage != null)
            {
                this.m_refPART_ClippingBackPage.Figures.Clear();
                this.m_refPART_ClippingBackPage.Figures.Add(parameters.Page1ClippingFigure);
            }
            if (this.m_refPART_ClippingFrontPage != null)
            {
                ((RectangleGeometry)this.m_refPART_ClippingFrontPage.Geometry1).Rect = new Rect(parameters.RenderSize);
                PathGeometry pathGeometry = (PathGeometry)this.m_refPART_ClippingFrontPage.Geometry2;
                pathGeometry.Figures.Clear();
                pathGeometry.Figures.Add(parameters.Page2ClippingFigure);
            }
            if (this.m_refPART_ReflectionBackground != null)
            {
                this.m_refPART_ReflectionBackground.StartPoint = parameters.Page1ReflectionStartPoint;
                this.m_refPART_ReflectionBackground.EndPoint = parameters.Page1ReflectionEndPoint;
            }
            if (this.m_refPART_ShadowBackground == null)
                return;
            this.m_refPART_ShadowBackground.StartPoint = parameters.Page0ShadowStartPoint;
            this.m_refPART_ShadowBackground.EndPoint = parameters.Page0ShadowEndPoint;
        }

        protected virtual CornerOrigin GetCornerOrigin(Rectangle rctCorner, Point ptControl)
        {
            CornerOrigin CornerOrigin = CornerOrigin.TopLeft;
            if (this.m_rctCornerTopLeft != null && this.m_rctCornerTopLeft == rctCorner)
                CornerOrigin = CornerOrigin.TopLeft;
            else if (this.m_rctCornerTopRight != null && this.m_rctCornerTopRight == rctCorner)
                CornerOrigin = CornerOrigin.TopRight;
            else if (this.m_rctCornerBottomLeft != null && this.m_rctCornerBottomLeft == rctCorner)
                CornerOrigin = CornerOrigin.BottomLeft;
            else if (this.m_rctCornerBottomRight != null && this.m_rctCornerBottomRight == rctCorner)
                CornerOrigin = CornerOrigin.BottomRight;
            else if (this.m_rctSideLeft != null && this.m_rctSideLeft == rctCorner)
                CornerOrigin = ptControl.Y > this.ActualHeight / 2.0 ? CornerOrigin.BottomLeft : CornerOrigin.TopLeft;
            else if (this.m_rctSideRight != null && this.m_rctSideRight == rctCorner)
                CornerOrigin = ptControl.Y > this.ActualHeight / 2.0 ? CornerOrigin.BottomRight : CornerOrigin.TopRight;
            return CornerOrigin;
        }

        protected virtual bool IsInsideBoundingBox(Point ptPositionRelativeToControl)
        {
            if (ptPositionRelativeToControl.X >= 0.0 && ptPositionRelativeToControl.X <= this.ActualWidth && ptPositionRelativeToControl.Y >= 0.0)
                return ptPositionRelativeToControl.Y <= this.ActualHeight;
            else
                return false;
        }

        protected virtual Point OriginToPoint(CornerOrigin refCornerOrigin)
        {
            switch (refCornerOrigin)
            {
                case CornerOrigin.TopRight:
                    return new Point(this.RenderSize.Width, 0.0);
                case CornerOrigin.BottomLeft:
                    return new Point(0.0, this.RenderSize.Height);
                case CornerOrigin.BottomRight:
                    return new Point(this.RenderSize.Width, this.RenderSize.Height);
                default:
                    return new Point(0.0, 0.0);
            }
        }

        protected virtual Point OriginToOppositePoint(CornerOrigin refCornerOrigin)
        {
            switch (refCornerOrigin)
            {
                case CornerOrigin.TopRight:
                    return new Point(-this.RenderSize.Width, 0.0);
                case CornerOrigin.BottomLeft:
                    return new Point(this.RenderSize.Width * 2.0, this.RenderSize.Height);
                case CornerOrigin.BottomRight:
                    return new Point(-this.RenderSize.Width, this.RenderSize.Height);
                default:
                    return new Point(this.RenderSize.Width * 2.0, 0.0);
            }
        }

        protected virtual bool IsOnNextPage(Point refPoint, CornerOrigin refCornerOrigin)
        {
            switch (refCornerOrigin)
            {
                case CornerOrigin.TopLeft:
                case CornerOrigin.BottomLeft:
                    return refPoint.X > this.RenderSize.Width;
                default:
                    return refPoint.X < 0.0;
            }
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            this.OnInputDeviceDown((InputEventArgs)e);
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            this.OnInputDeviceMove((InputEventArgs)e);
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            this.OnInputDeviceUp((InputEventArgs)e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || e.ChangedButton != MouseButton.Left)
                return;
            this.OnInputDeviceDown((InputEventArgs)e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            this.OnInputDeviceMove((InputEventArgs)e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released || e.ChangedButton != MouseButton.Left)
                return;
            this.OnInputDeviceUp((InputEventArgs)e);
        }

        protected virtual void OnInputDeviceDown(InputEventArgs e)
        {
            if (!this.IsDragging && this.m_bDraggingEnabled)
                return;
            e.Handled = true;
        }

        protected virtual void OnInputDeviceMove(InputEventArgs e)
        {
            if (!this.IsDragging && this.m_bDraggingEnabled)
                return;
            e.Handled = true;
        }

        protected virtual void OnInputDeviceUp(InputEventArgs e)
        {
            if (!this.IsDragging && this.m_bDraggingEnabled)
                return;
            e.Handled = true;
        }

        private void OnCornerTouchDown(object sender, TouchEventArgs e)
        {
            this.OnCornerInputDeviceDown((InputEventArgs)e);
        }

        private void OnCornerTouchMove(object sender, TouchEventArgs e)
        {
            this.OnCornerInputDeviceMove((InputEventArgs)e);
        }

        private void OnCornerTouchUp(object sender, TouchEventArgs e)
        {
            this.OnCornerInputDeviceUp((InputEventArgs)e);
        }

        private void OnCornerGotTouchCapture(object sender, TouchEventArgs e)
        {
            this.OnCornerGotInputDeviceCapture((InputEventArgs)e);
        }

        private void OnCornerLostTouchCapture(object sender, TouchEventArgs e)
        {
            this.OnCornerLostInputDeviceCapture((InputEventArgs)e);
        }

        private void OnCornerTouchEnter(object sender, TouchEventArgs e)
        {
            this.OnCornerInputDeviceEnter((InputEventArgs)e);
        }

        private void OnCornerTouchLeave(object sender, TouchEventArgs e)
        {
            this.OnCornerInputDeviceLeave((InputEventArgs)e);
        }

        private void OnCornerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || e.ChangedButton != MouseButton.Left)
                return;
            this.OnCornerInputDeviceDown((InputEventArgs)e);
        }

        private void OnCornerGoNouseCapture(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            this.OnCornerGotInputDeviceCapture((InputEventArgs)e);
        }

        private void OnCornerMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            this.OnCornerInputDeviceMove((InputEventArgs)e);
        }

        private void OnCornerMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released || e.ChangedButton != MouseButton.Left)
                return;
            this.OnCornerInputDeviceUp((InputEventArgs)e);
        }

        private void OnCornerLosNouseCapture(object sender, MouseEventArgs e)
        {
            this.OnCornerLostInputDeviceCapture((InputEventArgs)e);
        }

        private void OnCornerMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            this.OnCornerInputDeviceEnter((InputEventArgs)e);
        }

        private void OnCornerMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            this.OnCornerInputDeviceLeave((InputEventArgs)e);
        }

        protected virtual void OnCornerInputDeviceDown(InputEventArgs e)
        {
            Rectangle rctCorner = e.Source as Rectangle;
            if (rctCorner == null || rctCorner.AreAnyTouchesCaptured || (!this.m_bCanStarNanipulation || !this.m_bDraggingEnabled))
                return;
            e.Handled = true;
            if (this.Status == Enums.PageStatus.DropAnimation || this.Status == Enums.PageStatus.Dragging)
                return;
            if (this.Status == Enums.PageStatus.TurnAnimation)
            {
                this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)null);
                this.RaisePageTurnedEvent();
            }
            TouchExtensions.Capture(e.Device, (IInputElement)rctCorner);
            this.m_ptTapPoint = TouchExtensions.GetPosition(e.Device, (IInputElement)this);
            this.m_refTapTimer.Start();
            this.m_refFlickTimer.Start();
            this.m_dateStartFlick = DateTime.Now;
            Point position = TouchExtensions.GetPosition(e.Device, (IInputElement)this);
            CornerOrigin cornerOrigin = this.GetCornerOrigin(rctCorner, position);
            this.m_refCornerOrigin = cornerOrigin;
            if (this.IsInsideBoundingBox(position))
            {
                this.m_bContactAlreadyInsidePage = true;
                Point p = position;
                if (this.m_rctSideLeft != null && rctCorner == this.m_rctSideLeft || this.m_rctSideRight != null && rctCorner == this.m_rctSideRight)
                    p.Y = this.OriginToPoint(cornerOrigin).Y;
                PageParameters? page = BookPageComputeHelper.ComputePage(this, p, cornerOrigin);
                this.CornerPoint = this.m_ptCornerPoint = p;
                if (page.HasValue)
                    this.ApplyParameters(page.Value);
                this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.Dragging);
            }
            else
            {
                this.m_ptCornerPoint = position;
                if (this.IsRightPage && this.m_ptCornerPoint.X > this.ActualWidth)
                    this.m_ptCornerPoint.X = this.ActualWidth;
                if (this.IsLeftPage && this.m_ptCornerPoint.X < 0.0)
                    this.m_ptCornerPoint.X = 0.0;
                if (this.m_ptCornerPoint.Y > this.ActualHeight)
                    this.m_ptCornerPoint.Y = this.ActualHeight;
                if (this.m_ptCornerPoint.Y < 0.0)
                    this.m_ptCornerPoint.Y = 0.0;
                this.CornerPoint = this.m_ptCornerPoint;
            }
        }

        protected virtual void OnCornerGotInputDeviceCapture(InputEventArgs e)
        {
            e.Handled = true;
        }

        protected virtual void OnCornerInputDeviceMove(InputEventArgs e)
        {
            Rectangle rectangle = e.Source as Rectangle;
            if (rectangle == null || TouchExtensions.GetCaptured(e.Device) != rectangle || !this.m_bDraggingEnabled)
                return;
            e.Handled = true;
            if (this.Status == PageStatus.DropAnimation || this.Status == PageStatus.TurnAnimation)
                return;
            Point position = TouchExtensions.GetPosition(e.Device, (IInputElement)this);
            if (this.m_refTapTimer.IsEnabled && (position.X - this.m_ptTapPoint.X) * (position.X - this.m_ptTapPoint.X) + (position.Y - this.m_ptTapPoint.Y) * (position.Y - this.m_ptTapPoint.Y) > (double)(BookPage.s_nTAP_DISTANCE_THRESHOLD * BookPage.s_nTAP_DISTANCE_THRESHOLD))
                this.m_refTapTimer.IsEnabled = false;
            if (!this.IsInsideBoundingBox(position) && !this.m_bContactAlreadyInsidePage)
                return;
            this.m_bContactAlreadyInsidePage = true;
            Point p = position;
            if (this.m_rctSideLeft != null && rectangle == this.m_rctSideLeft || this.m_rctSideRight != null && rectangle == this.m_rctSideRight)
                p.Y = this.OriginToPoint(this.m_refCornerOrigin).Y;
            PageParameters? page = BookPageComputeHelper.ComputePage(this, p, this.m_refCornerOrigin);
            this.m_ptCornerPoint = p;
            if (page.HasValue)
                this.ApplyParameters(page.Value);
            if (this.Status == PageStatus.Dragging)
                return;
            this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.Dragging);
        }

        protected virtual void OnCornerInputDeviceUp(InputEventArgs e)
        {
            Rectangle rectangle = e.Source as Rectangle;
            if (rectangle == null || TouchExtensions.GetCaptured(e.Device) != rectangle)
                return;
            e.Handled = true;
            TouchExtensions.Capture(e.Device, (IInputElement)null);
            if (Enumerable.Count<TouchDevice>(rectangle.TouchesCaptured) != 0)
                return;
            this.m_bContactAlreadyInsidePage = false;
            if (this.m_bDraggingEnabled)
            {
                Point position = TouchExtensions.GetPosition(e.Device, (IInputElement)this);
                double num1 = 0.0;
                if (this.IsRightPage)
                    num1 = this.m_ptTapPoint.X - position.X;
                else if (this.IsLeftPage)
                    num1 = position.X - this.m_ptTapPoint.X;
                if (this.m_refTapTimer.IsEnabled)
                {
                    if (position.X < 0.0 || position.X > this.ActualWidth || (position.Y < 0.0 || position.Y > this.ActualHeight))
                    {
                        this.m_ptCornerPoint = position;
                        if (this.IsRightPage && this.m_ptCornerPoint.X > this.ActualWidth)
                            this.m_ptCornerPoint.X = this.ActualWidth;
                        if (this.IsLeftPage && this.m_ptCornerPoint.X < 0.0)
                            this.m_ptCornerPoint.X = 0.0;
                        this.m_ptCornerPoint.Y = this.m_ptCornerPoint.Y <= this.ActualHeight / 2.0 ? 0.0 : this.ActualHeight;
                        PageParameters? page = BookPageComputeHelper.ComputePage(this, this.m_ptCornerPoint, this.m_refCornerOrigin);
                        if (page.HasValue)
                            this.ApplyParameters(page.Value);
                        this.SetIsDragging(true);
                        this.TurnPage(this.TurnAnimationDuration);
                    }
                    else
                        this.TurnPage(this.TurnAnimationDuration);
                }
                else if (this.m_refFlickTimer.IsEnabled && num1 > (double)BookPage.s_nFLICK_DISTANCE_THRESHOLD)
                {
                    TimeSpan timeSpan = DateTime.Now - this.m_dateStartFlick;
                    double num2 = Math.Abs(this.m_ptTapPoint.X - position.X) / (this.ActualWidth * 2.0);
                    double num3 = num2 / timeSpan.TotalMilliseconds;
                    int nAnimationDuration = (int)((1.0 - num2) / num3);
                    if (nAnimationDuration < 0)
                        nAnimationDuration = 0;
                    if (nAnimationDuration > BookPage.s_nANIMATION_DURATION)
                        nAnimationDuration = BookPage.s_nANIMATION_DURATION;
                    this.SetIsDragging(true);
                    this.TurnPage(nAnimationDuration);
                }
                else if (this.IsOnNextPage(position, this.m_refCornerOrigin))
                {
                    this.TurnPage(BookPage.s_nANIMATION_DURATION);
                }
                else
                {
                    int animationDuration = BookPageComputeHelper.ComputeAnimationDuration(this, position, this.m_refCornerOrigin);
                    this.SetIsDragging(true);
                    this.DropPage(animationDuration);
                }
            }
            else
                this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.Idle);
        }

        protected virtual void OnCornerLostInputDeviceCapture(InputEventArgs e)
        {
            e.Handled = true;
            if (!(TouchExtensions.GetCaptured(e.Device) is ScatterViewItem))
                return;
            this.DropPage(BookPage.s_nANIMATION_DURATION);
        }

        protected virtual void OnCornerInputDeviceEnter(InputEventArgs e)
        {
        }

        protected virtual void OnCornerInputDeviceLeave(InputEventArgs e)
        {
        }

        internal void DisableManipulation()
        {
            this.m_bCanStarNanipulation = false;
        }

        internal void EnableManipulation()
        {
            this.m_bCanStarNanipulation = true;
        }

        public void TurnPage(int nAnimationDuration)
        {
            this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.TurnAnimation);
            this.CornerPoint = this.m_ptCornerPoint;
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)null);
            Point toValue = this.OriginToOppositePoint(this.m_refCornerOrigin);
            if (this.m_refAnimation != null)
            {
                this.m_refAnimation.CurrentTimeInvalidated -= new EventHandler(this.OnAnimationCurrentTimeInvalidated);
                this.m_refAnimation.CurrentStateInvalidated -= new EventHandler(this.OnAnimationCurrentStateInvalidated);
            }
            this.m_refAnimation = (PointAnimationBase)new PointAnimation(toValue, new Duration(TimeSpan.FromMilliseconds((double)nAnimationDuration)));
            this.m_refAnimation.AccelerationRatio = 0.6;
            this.m_refAnimation.CurrentTimeInvalidated += new EventHandler(this.OnAnimationCurrentTimeInvalidated);
            this.m_refAnimation.CurrentStateInvalidated += new EventHandler(this.OnAnimationCurrentStateInvalidated);
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)this.m_refAnimation);
        }

        public void TurnPageFromCorner(CornerOrigin refStarterCornerOrigin, int nAnimationDuration)
        {
            if (this.Status != PageStatus.Idle)
                return;
            this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.TurnAnimation);
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)null);
            this.DisableManipulation();
            Point point1 = this.OriginToPoint(refStarterCornerOrigin);
            if (refStarterCornerOrigin == CornerOrigin.BottomRight || refStarterCornerOrigin == CornerOrigin.BottomLeft)
                point1.Y -= BookPage.s_dOFFSET_TO_AVOID_GLITCH;
            Point point3 = this.OriginToOppositePoint(refStarterCornerOrigin);
            this.m_ptCornerPoint = this.CornerPoint = point1;
            this.m_refCornerOrigin = refStarterCornerOrigin;
            BezierSegment bezierSegment = new BezierSegment(point1, new Point(point3.X + (point1.X - point3.X) / 3.0, 250.0), point3, true);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(new PathFigure()
            {
                StartPoint = point1,
                Segments = {
          (PathSegment) bezierSegment
        },
                IsClosed = false
            });
            if (this.m_refAnimation != null)
            {
                this.m_refAnimation.CurrentTimeInvalidated -= new EventHandler(this.OnAnimationCurrentTimeInvalidated);
                this.m_refAnimation.CurrentStateInvalidated -= new EventHandler(this.OnAnimationCurrentStateInvalidated);
            }
            this.m_refAnimation = (PointAnimationBase)new PointAnimationUsingPath();
            (this.m_refAnimation as PointAnimationUsingPath).PathGeometry = pathGeometry;
            this.m_refAnimation.Duration = new Duration(TimeSpan.FromMilliseconds((double)nAnimationDuration));
            this.m_refAnimation.AccelerationRatio = 0.6;
            this.m_refAnimation.CurrentTimeInvalidated += new EventHandler(this.OnAnimationCurrentTimeInvalidated);
            this.m_refAnimation.CurrentStateInvalidated += new EventHandler(this.OnAnimationCurrentStateInvalidated);
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)this.m_refAnimation);
        }

        public void DropPage(int nAnimationDuration)
        {
            this.SetValue(BookPage.StatusPropertyKey, (object)PageStatus.DropAnimation);
            this.CornerPoint = this.m_ptCornerPoint;
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)null);
            Point toValue = this.OriginToPoint(this.m_refCornerOrigin);
            if (this.m_refAnimation != null)
            {
                this.m_refAnimation.CurrentTimeInvalidated -= new EventHandler(this.OnAnimationCurrentTimeInvalidated);
                this.m_refAnimation.CurrentStateInvalidated -= new EventHandler(this.OnAnimationCurrentStateInvalidated);
            }
            this.m_refAnimation = (PointAnimationBase)new PointAnimation(toValue, new Duration(TimeSpan.FromMilliseconds((double)nAnimationDuration)));
            this.m_refAnimation.AccelerationRatio = 0.6;
            this.m_refAnimation.CurrentTimeInvalidated += new EventHandler(this.OnAnimationCurrentTimeInvalidated);
            this.m_refAnimation.CurrentStateInvalidated += new EventHandler(this.OnAnimationCurrentStateInvalidated);
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)this.m_refAnimation);
        }

        public void ExecuteIdleAnimation(int corner)
        {
            Path path = (Path)null;
            switch (corner)
            {
                case 1:
                    if (this.IsRightPage)
                    {
                        path = this.m_refPART_IdleAnimationPathTopRight;
                        this.m_refCornerOrigin = CornerOrigin.TopRight;
                        break;
                    }
                    else
                    {
                        path = this.m_refPART_IdleAnimationPathTopLeft;
                        this.m_refCornerOrigin = CornerOrigin.TopLeft;
                        break;
                    }
                case 2:
                    if (this.IsRightPage)
                    {
                        path = this.m_refPART_IdleAnimationPathBottomRight;
                        this.m_refCornerOrigin = CornerOrigin.BottomRight;
                        break;
                    }
                    else
                    {
                        path = this.m_refPART_IdleAnimationPathBottomLeft;
                        this.m_refCornerOrigin = CornerOrigin.BottomLeft;
                        break;
                    }
            }
            if (this.Status != PageStatus.Idle || path == null)
                return;
            Geometry renderedGeometry = path.RenderedGeometry;
            if (renderedGeometry == null)
                return;
            if (this.m_refPART_ShadowCanvas != null)
                this.m_refPART_ShadowCanvas.Visibility = Visibility.Visible;
            if (this.m_refPART_ReflectionCanvas != null)
                this.m_refPART_ReflectionCanvas.Visibility = Visibility.Visible;
            this.m_ptCornerPoint = this.CornerPoint = this.OriginToPoint(this.m_refCornerOrigin);
            PageParameters? page = BookPageComputeHelper.ComputePage(this, this.m_ptCornerPoint, this.m_refCornerOrigin);
            if (page.HasValue)
                this.ApplyParameters(page.Value);
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)null);
            if (this.m_refAnimation != null)
            {
                this.m_refAnimation.CurrentTimeInvalidated -= new EventHandler(this.OnAnimationCurrentTimeInvalidated);
                this.m_refAnimation.CurrentStateInvalidated -= new EventHandler(this.OnAnimationCurrentStateInvalidated);
            }
            this.m_refAnimation = (PointAnimationBase)new PointAnimationUsingPath();
            this.m_refAnimation.AccelerationRatio = 0.1;
            this.m_refAnimation.DecelerationRatio = 0.9;
            this.m_refAnimation.Duration = (Duration)TimeSpan.FromMilliseconds((double)BookPage.s_nIDLE_ANIMATION_DURATION);
            PathGeometry pathGeometry = new PathGeometry((IEnumerable<PathFigure>)PathFigureCollection.Parse(new GeometryValueSerializer().ConvertToString((object)renderedGeometry, (IValueSerializerContext)null)));
            pathGeometry.Transform = (Transform)this.TransformToVisual((Visual)path).Inverse;
            pathGeometry.Freeze();
            (this.m_refAnimation as PointAnimationUsingPath).PathGeometry = pathGeometry;
            this.m_refAnimation.CurrentTimeInvalidated += new EventHandler(this.OnIdleAnimationCurrentTimeInvalidated);
            this.m_refAnimation.CurrentStateInvalidated += new EventHandler(this.OnIdleAnimationCurrentStateInvalidated);
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)this.m_refAnimation);
        }

        public void EnableDragging()
        {
            this.m_bDraggingEnabled = true;
        }

        public void DisableDragging()
        {
            this.m_bDraggingEnabled = false;
        }

        public void ResetCornerAnimation()
        {
            this.BeginAnimation(BookPage.CornerPointProperty, (AnimationTimeline)null);
            this.SetStatus(PageStatus.Idle);
        }

        public void SetFrontPageNumber(string pageNumber)
        {
            if (this.m_refPARTFrontPageNumber == null)
                return;
            this.m_refPARTFrontPageNumber.Text = pageNumber;
        }

        public void SetBackPageNumber(string pageNumber)
        {
            if (this.m_refPARTBackPageNumber == null)
                return;
            this.m_refPARTBackPageNumber.Text = pageNumber;
        }

        public void SetFuturePageNumber(string pageNumber)
        {
            if (this.m_refPARTFuturePageNumber == null)
                return;
            this.m_refPARTFuturePageNumber.Text = pageNumber;
        }

        public void Dispose()
        {
            this.Loaded -= new RoutedEventHandler(this.OnLoaded);
            this.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);
            if (this.m_refTapTimer != null)
            {
                this.m_refTapTimer.Tick -= new EventHandler(this.OnTimerTick);
                this.m_refTapTimer.Stop();
            }
            if (this.m_refFlickTimer != null)
            {
                this.m_refFlickTimer.Tick -= new EventHandler(this.OnTimerTick);
                this.m_refFlickTimer.Stop();
            }
            if (this.m_refAnimation == null)
                return;
            this.m_refAnimation.CurrentTimeInvalidated -= new EventHandler(this.OnAnimationCurrentTimeInvalidated);
            this.m_refAnimation.CurrentStateInvalidated -= new EventHandler(this.OnAnimationCurrentStateInvalidated);
        }

        public void ShowPageNumbers()
        {
            if (this.m_refPARTFrontPageNumber != null)
                this.m_refPARTFrontPageNumber.Visibility = Visibility.Visible;
            if (this.m_refPARTBackPageNumber != null)
                this.m_refPARTBackPageNumber.Visibility = Visibility.Visible;
            if (this.m_refPARTFuturePageNumber == null)
                return;
            this.m_refPARTFuturePageNumber.Visibility = Visibility.Visible;
        }

        public void HidePageNumbers()
        {
            if (this.m_refPARTFrontPageNumber != null)
                this.m_refPARTFrontPageNumber.Visibility = Visibility.Collapsed;
            if (this.m_refPARTBackPageNumber != null)
                this.m_refPARTBackPageNumber.Visibility = Visibility.Collapsed;
            if (this.m_refPARTFuturePageNumber == null)
                return;
            this.m_refPARTFuturePageNumber.Visibility = Visibility.Collapsed;
        }
    }

}