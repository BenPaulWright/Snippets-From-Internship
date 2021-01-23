using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using OpenCvSharp;
using Path = System.IO.Path;
using SpectroVision_Setup_Wizard.OtherClasses;
using SpectroVision_Setup_Wizard.Properties;
using System.Threading.Tasks;

namespace SpectroVision_Setup_Wizard.Pages
{
    /// <summary>
    ///     Interaction logic for ConfigureDalsaCam.xaml
    /// </summary>
    public partial class ConfigureDalsaCam
    {
        public readonly double Es5000FrameToApertureXmm = 7.366;
        public readonly double Es5000FrameToApertureYmm = 195.843;

        public ConfigureDalsaCam()
        {
            InitializeComponent();
        }

        public bool DalsaCamUiAvailable
        {
            get { return (bool)GetValue(DalsaCamUiAvailableProperty); }
            set { SetValue(DalsaCamUiAvailableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DalsaCamUiAvailable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DalsaCamUiAvailableProperty =
            DependencyProperty.Register("DalsaCamUiAvailable", typeof(bool), typeof(ConfigureDalsaCam), new PropertyMetadata(false));

        private void HandleDalsaConnectionChanged(bool connected)
        {
            DalsaCamUiAvailable = connected;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Hs.Instance.CameraConnectionChanged += HandleDalsaConnectionChanged;
            OnLoad(Requirements.DalsaCam);
            Hs.Instance.Camera.CaptureSingleFrame();
        }

        private void ProcessWithOpenCv(object sender, RoutedEventArgs e)
        {
            DCamDisp.CVDrawing.Children.Clear();

            if (Hs.Instance.Camera.FrameBitmap == null)
            {
                Hs.Instance._log.LogError("ConfigureDalsaCam", "No frame available to process");
                return;
            }

            //Save Image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName() + ".bmp");
            Hs.Instance.Camera.FrameBitmap.Save(imagePath);

            //Load image
            var rgbImage = new Mat(imagePath);

            //Delete saved image
            File.Delete(imagePath);

            //Convert to gray
            var grayImage = rgbImage.CvtColor(ColorConversionCodes.BGR2GRAY);

            //Find Circles
            var cr = grayImage.HoughCircles(HoughMethods.Gradient, 1, grayImage.Rows / 8.0, 200, 50);

            if (!cr.Any())
            {
                Hs.Instance._log.LogError("ConfigureDalsaCam", "Calibration tool not detected");
                return;
            }

            //Grab relevant parameters
            var circleX = cr[0].Center.X;
            var circleY = cr[0].Center.Y;
            var circleRadius = cr[0].Radius;
            var circleDiameter = cr[0].Radius * 2;
            var frameWidth = Hs.Instance.Camera.FrameBitmap.Width;
            var frameHeight = Hs.Instance.Camera.FrameBitmap.Height;


            //Draw on canvas
            var el = new Ellipse
            { Width = circleDiameter, Height = circleDiameter, Stroke = Brushes.Green, StrokeThickness = 3 };
            DCamDisp.CVDrawing.Children.Add(el);
            Canvas.SetTop(el, circleY - circleRadius);
            Canvas.SetLeft(el, circleX - circleRadius);

            //Scale Factors

            //pixels -> mm
            var pxToMm = 25.4 / circleDiameter;

            //Scale Factor
            ScaleFactor = (600 / circleDiameter);//.ToString(CultureInfo.InvariantCulture);

            //X Offset
            var mRectCenterToCamCenterXMm = (frameWidth / 2.0 - circleX) * pxToMm;

            XOffset = (Es5000FrameToApertureXmm - mRectCenterToCamCenterXMm);//.ToString(CultureInfo.InvariantCulture);

            //Y Offset
            var mRectCenterToCamCenterYMm = (frameHeight / 2.0 - circleY) * pxToMm;

            YOffset = (mRectCenterToCamCenterYMm + Es5000FrameToApertureYmm);//.ToString(CultureInfo.InvariantCulture);

            SaveCfg();
        }

        #region Dependency Properties

        public double YOffset
        {
            get { return (double)GetValue(YOffsetProperty); }
            set { SetValue(YOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YOffsetProperty =
            DependencyProperty.Register("YOffset", typeof(double), typeof(ConfigureDalsaCam), new PropertyMetadata(0.0));

        public double XOffset
        {
            get { return (double)GetValue(XOffsetProperty); }
            set { SetValue(XOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XOffsetProperty =
            DependencyProperty.Register("XOffset", typeof(double), typeof(ConfigureDalsaCam), new PropertyMetadata(0.0));

        public double ScaleFactor
        {
            get { return (double)GetValue(ScaleFactorProperty); }
            set { SetValue(ScaleFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleFactorProperty =
            DependencyProperty.Register("ScaleFactor", typeof(double), typeof(ConfigureDalsaCam), new PropertyMetadata(0.0));

        #endregion

        #region Button Functions

        private void SnapImage(object sender, RoutedEventArgs e)
        {
            if (!Hs.Instance.Camera.IsConnected)
            {
                Hs.Instance._log.LogError("ConfigureDalsaCam", "Camera not connected");
                return;
            }

            DCamDisp.CVDrawing.Children.Clear();
            Hs.Instance.Camera.CaptureSingleFrame();
        }

        private void SaveCfg()
        {
            Hs.Instance.Camera.CameraToSpectroDistanceYmm = YOffset;
            Hs.Instance.Camera.CameraToSpectroDistanceXmm = XOffset;
            Hs.Instance.Camera.CameraPrintScaleFactor = ScaleFactor;

            if (!ConfigRetriever.Instance.SaveSettings())
            {
                Hs.Instance._log.LogError("ConfigureDalsaCam", "Could not save settings");
                return;
            }
        }

        #endregion
    }
}