using System;
using System.Windows;

namespace Google_Music_Desktop
{
    public class FormSizeSaver
    {
        private readonly Window window;
        private readonly Func<FormSizeSaverSettings> getSetting;
        private readonly Action<FormSizeSaverSettings> saveSetting;
        private FormSizeSaver(Window window, Func<string> getSetting, Action<string> saveSetting)
        {
            this.window = window;
            this.getSetting = () => FormSizeSaverSettings.FromString(getSetting());
            this.saveSetting = s => saveSetting(s.ToString());

            window.Initialized += InitializedHandler;
            window.StateChanged += StateChangedHandler;
            window.SizeChanged += SizeChangedHandler;
            window.LocationChanged += LocationChangedHandler;
        }

        public static FormSizeSaver RegisterForm(Window window, Func<string> getSetting, Action<string> saveSetting)
        {
            return new FormSizeSaver(window, getSetting, saveSetting);
        }


        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            var s = getSetting();
            s.Height = e.NewSize.Height;
            s.Width = e.NewSize.Width;
            saveSetting(s);
        }

        private void StateChangedHandler(object sender, EventArgs e)
        {
            var s = getSetting();
            if (window.WindowState == WindowState.Maximized)
            {
                if (!s.Maximized)
                {
                    s.Maximized = true;
                    saveSetting(s);
                }
            }
            else if (window.WindowState == WindowState.Normal)
            {
                if (s.Maximized)
                {
                    s.Maximized = false;
                    saveSetting(s);
                }
            }
        }

        private void InitializedHandler(object sender, EventArgs e)
        {
            var s = getSetting();
            window.WindowState = s.Maximized ? WindowState.Maximized : WindowState.Normal;

            if (s.Height != 0 && s.Width != 0)
            {
                window.Height = s.Height;
                window.Width = s.Width;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Left = s.XLoc;
                window.Top = s.YLoc;
            }
        }

        private void LocationChangedHandler(object sender, EventArgs e)
        {
            var s = getSetting();
            s.XLoc = window.Left;
            s.YLoc = window.Top;
            saveSetting(s);
        }
    }
}