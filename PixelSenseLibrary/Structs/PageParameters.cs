using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace PixelSenseLibrary.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PageParameters
    {
        private double _page0ShadowOpacity;
        private double _page1RotateAngle;
        private double _page1RotateCenterX;
        private double _page1RotateCenterY;
        private double _page1TranslateX;
        private double _page1TranslateY;
        private PathFigure _page1ClippingFigure;
        private PathFigure _page2ClippingFigure;
        private Point _page1ReflectionStartPoint;
        private Point _page1ReflectionEndPoint;
        private Point _page0ShadowStartPoint;
        private Point _page0ShadowEndPoint;
        private Size _renderSize;

        public PageParameters(Size renderSize)
        {
            this._page0ShadowOpacity = 0.0;
            this._page0ShadowEndPoint = new Point();
            this._page0ShadowStartPoint = new Point();
            this._page1ClippingFigure = new PathFigure();
            this._page1ReflectionEndPoint = new Point();
            this._page1ReflectionStartPoint = new Point();
            this._page1RotateAngle = 0.0;
            this._page1RotateCenterX = 0.0;
            this._page1RotateCenterY = 0.0;
            this._page1TranslateX = 0.0;
            this._page1TranslateY = 0.0;
            this._page2ClippingFigure = new PathFigure();
            this._renderSize = renderSize;
        }

        public double Page0ShadowOpacity
        {
            get
            {
                return this._page0ShadowOpacity;
            }
            set
            {
                this._page0ShadowOpacity = value;
            }
        }
        public double Page1RotateAngle
        {
            get
            {
                return this._page1RotateAngle;
            }
            set
            {
                this._page1RotateAngle = value;
            }
        }
        public double Page1RotateCenterX
        {
            get
            {
                return this._page1RotateCenterX;
            }
            set
            {
                this._page1RotateCenterX = value;
            }
        }
        public double Page1RotateCenterY
        {
            get
            {
                return this._page1RotateCenterY;
            }
            set
            {
                this._page1RotateCenterY = value;
            }
        }
        public double Page1TranslateX
        {
            get
            {
                return this._page1TranslateX;
            }
            set
            {
                this._page1TranslateX = value;
            }
        }
        public double Page1TranslateY
        {
            get
            {
                return this._page1TranslateY;
            }
            set
            {
                this._page1TranslateY = value;
            }
        }
        public PathFigure Page1ClippingFigure
        {
            get
            {
                return this._page1ClippingFigure;
            }
            set
            {
                this._page1ClippingFigure = value;
            }
        }
        public PathFigure Page2ClippingFigure
        {
            get
            {
                return this._page2ClippingFigure;
            }
            set
            {
                this._page2ClippingFigure = value;
            }
        }
        public Point Page1ReflectionStartPoint
        {
            get
            {
                return this._page1ReflectionStartPoint;
            }
            set
            {
                this._page1ReflectionStartPoint = value;
            }
        }
        public Point Page1ReflectionEndPoint
        {
            get
            {
                return this._page1ReflectionEndPoint;
            }
            set
            {
                this._page1ReflectionEndPoint = value;
            }
        }
        public Point Page0ShadowStartPoint
        {
            get
            {
                return this._page0ShadowStartPoint;
            }
            set
            {
                this._page0ShadowStartPoint = value;
            }
        }
        public Point Page0ShadowEndPoint
        {
            get
            {
                return this._page0ShadowEndPoint;
            }
            set
            {
                this._page0ShadowEndPoint = value;
            }
        }
        public Size RenderSize
        {
            get
            {
                return this._renderSize;
            }
            set
            {
                this._renderSize = value;
            }
        }

    }
}