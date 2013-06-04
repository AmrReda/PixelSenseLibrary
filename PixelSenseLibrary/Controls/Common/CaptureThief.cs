using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Surface.Presentation.Input;
using PixelSenseLibrary.Enums;
using Microsoft.Windows.Design;
using PixelSenseLibrary.Extensions;

namespace PixelSenseLibrary.Controls.Common
{
    [ToolboxBrowsable(false)]
    public class CaptureThief : IDisposable
    {
        // Fields
        public static readonly DependencyProperty AllowsParentToStealCaptureProperty = DependencyProperty.RegisterAttached("AllowsParentToStealCapture", typeof(bool), typeof(CaptureThief), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HasCaptureThiefProperty = DependencyProperty.RegisterAttached("HasCaptureThief", typeof(bool), typeof(CaptureThief), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty InputToStealThresholdProperty = DependencyProperty.RegisterAttached("InputToStealThreshold", typeof(int), typeof(CaptureThief), new FrameworkPropertyMetadata(0x7fffffff));
        protected UIElement m_refOwner;
        protected string m_refThiefID = Guid.NewGuid().ToString();
        public static readonly DependencyProperty MaxInputToStealProperty = DependencyProperty.RegisterAttached("MaxInputToSteal", typeof(int), typeof(CaptureThief), new FrameworkPropertyMetadata(0x7fffffff, new PropertyChangedCallback(CaptureThief.OnMaxInputToStealChanged)));
        public static readonly DependencyProperty ParentStealCaptureOrientationProperty = DependencyProperty.RegisterAttached("ParentStealCaptureOrientation", typeof(Orientation), typeof(CaptureThief), new FrameworkPropertyMetadata((Orientation)0x400));
        public static readonly DependencyProperty ParentStealCaptureRadiusThresholdProperty = DependencyProperty.RegisterAttached("ParentStealCaptureRadiusThreshold", typeof(double), typeof(CaptureThief), new FrameworkPropertyMetadata(15.0));
        public static readonly DependencyProperty ParentStealCaptureWrongDirectionThresholdProperty = DependencyProperty.RegisterAttached("ParentStealCaptureWrongDirectionThreshold", typeof(double), typeof(CaptureThief), new FrameworkPropertyMetadata(12.0));
        protected static readonly string s_strThiefDataKey = Guid.NewGuid().ToString();

        // Methods
        public CaptureThief(UIElement element)
        {
            this.Owner = element;
            SetHasCaptureThief(this.Owner, true);
            element.AddHandler(UIElement.GotMouseCaptureEvent, new MouseEventHandler(this.OnGotMouseCapture), true);
            element.AddHandler(UIElement.GotTouchCaptureEvent, new EventHandler<TouchEventArgs>(this.OnGotTouchCapture), true);
            element.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(this.OnMouseMove), true);
            element.AddHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(this.OnTouchMove), true);
            element.AddHandler(UIElement.LostMouseCaptureEvent, new MouseEventHandler(this.OnLostMouseCapture), true);
            element.AddHandler(UIElement.LostTouchCaptureEvent, new EventHandler<TouchEventArgs>(this.OnLostTouchCapture), true);
        }

        public void Dispose()
        {
            if (this.Owner != null)
            {
                SetHasCaptureThief(this.Owner, false);
                this.Owner.RemoveHandler(UIElement.GotMouseCaptureEvent, new MouseEventHandler(this.OnGotMouseCapture));
                this.Owner.RemoveHandler(UIElement.GotTouchCaptureEvent, new EventHandler<TouchEventArgs>(this.OnGotTouchCapture));
                this.Owner.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(this.OnMouseMove));
                this.Owner.RemoveHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(this.OnTouchMove));
                this.Owner.RemoveHandler(UIElement.LostMouseCaptureEvent, new MouseEventHandler(this.OnLostMouseCapture));
                this.Owner.RemoveHandler(UIElement.LostTouchCaptureEvent, new EventHandler<TouchEventArgs>(this.OnLostTouchCapture));
            }
        }

        public static bool GetAllowsParentToStealCapture(DependencyObject d)
        {
            return (bool)d.GetValue(AllowsParentToStealCaptureProperty);
        }

        public static bool GetHasCaptureThief(DependencyObject d)
        {
            return (bool)d.GetValue(HasCaptureThiefProperty);
        }

        public static int GetInputToStealThreshold(DependencyObject d)
        {
            return (int)d.GetValue(InputToStealThresholdProperty);
        }

        public static int GetMaxInputToSteal(DependencyObject d)
        {
            return (int)d.GetValue(MaxInputToStealProperty);
        }

        public static Orientation GetParentStealCaptureOrientation(DependencyObject d)
        {
            return (Orientation)d.GetValue(ParentStealCaptureOrientationProperty);
        }

        public static double GetParentStealCaptureRadiusThreshold(DependencyObject d)
        {
            return (double)d.GetValue(ParentStealCaptureRadiusThresholdProperty);
        }

        public static double GetParentStealCaptureWrongDirectionThreshold(DependencyObject d)
        {
            return (double)d.GetValue(ParentStealCaptureWrongDirectionThresholdProperty);
        }

        private void OnGotInputCapture(InputEventArgs e)
        {
            InputDevice inputDevice = e.Device;
            DependencyObject captured = inputDevice.GetCaptured() as DependencyObject;
            if (this.Owner.IsManipulationEnabled && GetAllowsParentToStealCapture(captured))
            {
                NThiefData userData = inputDevice.GetUserData(s_strThiefDataKey) as NThiefData;
                if ((this.Owner.TouchesCapturedWithin.Count<TouchDevice>() >= GetInputToStealThreshold(this.m_refOwner)) && (inputDevice is TouchDevice))
                {
                    if (userData == null)
                    {
                        userData = new NThiefData(this.m_refThiefID, inputDevice.GetPosition(this.Owner));
                        inputDevice.SetUserData(s_strThiefDataKey, userData);
                        foreach (TouchDevice device2 in this.Owner.TouchesCapturedWithin.ToList<TouchDevice>())
                        {
                            if (device2.Captured != this.Owner)
                            {
                                NThiefData data2 = device2.GetUserData(s_strThiefDataKey) as NThiefData;
                                if (data2 != null)
                                {
                                    data2.MustBeStealed = true;
                                }
                            }
                        }
                    }
                }
                else if (((userData == null) && (captured != this.Owner)) && (captured != null))
                {
                    inputDevice.SetUserData(s_strThiefDataKey, new NThiefData(this.m_refThiefID, inputDevice.GetPosition(this.Owner)));
                }
            }
        }

        private void OnGotMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.OnGotInputCapture(e);
            }
        }

        private void OnGotTouchCapture(object sender, TouchEventArgs e)
        {
            this.OnGotInputCapture(e);
        }

        private void OnInputMove(InputEventArgs e)
        {
            InputDevice inputDevice = e.Device;
            DependencyObject captured = inputDevice.GetCaptured() as DependencyObject;
            NThiefData userData = inputDevice.GetUserData(s_strThiefDataKey) as NThiefData;
            if (((userData != null) && ((captured == null) || (userData.ThiefID == this.m_refThiefID))) && (userData.MustBeStealed && (inputDevice.GetCaptured() != this.Owner)))
            {
                inputDevice.Capture(this.Owner);
            }
            else if (((captured != null) && (userData != null)) && (userData.ThiefID == this.m_refThiefID))
            {
                int maxInputToSteal = GetMaxInputToSteal(captured);
                if (captured is FrameworkElement)
                {
                    FrameworkElement element = captured as FrameworkElement;
                    if (element.TouchesCaptured.Count<TouchDevice>() > maxInputToSteal)
                    {
                        return;
                    }
                }
                bool? nullable = GetParentStealCaptureOrientation(captured).CheckValidMovementForOrientation(userData.OriginPosition, inputDevice.GetPosition(this.Owner), GetParentStealCaptureRadiusThreshold(captured), GetParentStealCaptureWrongDirectionThreshold(captured));
                if (nullable.HasValue && nullable.Value)
                {
                    inputDevice.SetUserData(s_strThiefDataKey, null);
                    inputDevice.Capture(this.Owner);
                }
                else if (!nullable.HasValue)
                {
                    inputDevice.SetUserData(s_strThiefDataKey, null);
                }
            }
        }

        private void OnLostInputCapture(InputEventArgs e)
        {
            InputDevice device = e.Device;
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.OnLostInputCapture(e);
        }

        private void OnLostTouchCapture(object sender, TouchEventArgs e)
        {
            this.OnLostInputCapture(e);
        }

        private static void OnMaxInputToStealChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int oldValue = (int)e.OldValue;
            int num2 = (int)d.GetValue(MaxInputToStealProperty);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.OnInputMove(e);
            }
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            this.OnInputMove(e);
        }

        public static void SetAllowsParentToStealCapture(DependencyObject d, bool value)
        {
            d.SetValue(AllowsParentToStealCaptureProperty, value);
        }

        public static void SetHasCaptureThief(DependencyObject d, bool value)
        {
            d.SetValue(HasCaptureThiefProperty, value);
        }

        public static void SetInputToStealThreshold(DependencyObject d, int value)
        {
            d.SetValue(InputToStealThresholdProperty, value);
        }

        public static void SetMaxInputToSteal(DependencyObject d, int value)
        {
            d.SetValue(MaxInputToStealProperty, value);
        }

        public static void SetParentStealCaptureOrientation(DependencyObject d, Orientation value)
        {
            d.SetValue(ParentStealCaptureOrientationProperty, value);
        }

        public static void SetParentStealCaptureRadiusThreshold(DependencyObject d, double value)
        {
            d.SetValue(ParentStealCaptureRadiusThresholdProperty, value);
        }

        public static void SetParentStealCaptureWrongDirectionThreshold(DependencyObject d, double value)
        {
            d.SetValue(ParentStealCaptureWrongDirectionThresholdProperty, value);
        }

        // Properties
        public UIElement Owner
        {
            get
            {
                return this.m_refOwner;
            }
            protected set
            {
                this.m_refOwner = value;
            }
        }

        // Nested Types
        protected class NThiefData
        {
            // Methods
            public NThiefData(string ownerID, Point originPosition)
            {
                this.ThiefID = ownerID;
                this.OriginPosition = originPosition;
            }

            // Properties
            public bool MustBeStealed { get; set; }

            public Point OriginPosition { get; protected set; }

            public string ThiefID { get; protected set; }
        }

    }
}