using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Awesomium.Core;
using GlobalHotKey;

namespace Google_Music_Desktop
{
    public partial class MainWindow : Window
    {
        private const uint WM_SYSTEMMENU = 0xa4;
        private const uint WP_SYSTEMMENU = 0x02;
        private HotKeyManager _hotkeyManager;

        private const string playPauseCommand = "location.assign('javascript:SJBpost(\"playPause\");void 0');";
        private const string prevSongCommand = "location.assign('javascript:SJBpost(\"prevSong\");void 0');";
        private const string nextSongCommand = "location.assign('javascript:SJBpost(\"nextSong\");void 0');";
        private const string likeSongCommand = "location.assign('javascript:SJBpost(\"thumbsUpPlayer\");void 0');";
        private const string dislikeSongCommand = "location.assign('javascript:SJBpost(\"thumbsDownPlayer\");void 0');";

        public MainWindow()
        {
            FormSizeSaver.RegisterForm(this, () => Properties.Settings.Default.MainWindowSettings,
                                   s =>
                                   {
                                       Properties.Settings.Default.MainWindowSettings = s;
                                       Properties.Settings.Default.Save();
                                   });

            InitializeComponent();
            CreateSession();
            Loaded += OnLoaded;
            SetupHotkeys();
        }

        private void CreateSession()
        {
            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Chromium\\User Data";
            var prefs = SetupWebPreferences();

            Browser.WebSession  = WebCore.Sessions[dataPath] ?? WebCore.CreateWebSession(dataPath, prefs);
            Browser.LoadingFrame += BrowserOnLoadingFrame;
            Browser.Source = new Uri("https://play.google.com/music/listen");
        }

        private void BrowserOnLoadingFrame(object sender, LoadingFrameEventArgs loadingFrameEventArgs)
        {
            if (loadingFrameEventArgs.IsMainFrame)
            {
                Browser.ZoomOut();
            }
        }

        private WebPreferences SetupWebPreferences()
        {
            return new WebPreferences
                {
                    CustomCSS = @"
                                    .gb_9.gb_wb.gb_f.gb_vb { display: none !important; } 
                                    .explicit { display: none !important; }
                                    #extra-links-container { display: none !important; }
                                "
                };
        }

//        public void InjectJQuery()
//        {
//            var injectCode =
//                @"function InjectJquery(urlJQuery)
//                {
//                  if (typeof jQuery != 'undefined')
//                    return;
//
//                  var scriptTag = document.createElement('script');
//                  scriptTag.setAttribute('type', 'text/javascript');
//                  scriptTag.setAttribute('src', urlJQuery);
//                  scriptTag.onload = scriptTag.onreadystatechange = function (){};
//                  document.head.appendChild(scriptTag);
//                }
//                InjectJquery('//ajax.aspnetcdn.com/ajax/jquery/jquery-1.8.0.js');";
//
//            Browser.ExecuteJavascript(injectCode);
//
//            Browser.ExecuteJavascript("$('#gb .gb_9.gb_wb.gb_f.gb_vb').hide();");
//        }

        private void SetupHotkeys()
        {
            _hotkeyManager = new HotKeyManager();
            _hotkeyManager.KeyPressed += HotKeyManagerPressed;
            _hotkeyManager.Register(Key.Up, ModifierKeys.Control | ModifierKeys.Alt);
            _hotkeyManager.Register(Key.Down, ModifierKeys.Control | ModifierKeys.Alt);
            _hotkeyManager.Register(Key.Right, ModifierKeys.Control | ModifierKeys.Alt);
            _hotkeyManager.Register(Key.Left, ModifierKeys.Control | ModifierKeys.Alt);
            _hotkeyManager.Register(Key.PageUp, ModifierKeys.Control | ModifierKeys.Alt);
            _hotkeyManager.Register(Key.PageDown, ModifierKeys.Control | ModifierKeys.Alt);

        }

        void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.HotKey.Key)
            {
                case Key.Up:
                case Key.Down: Browser.ExecuteJavascript(playPauseCommand);
                    break;
                case Key.Left: Browser.ExecuteJavascript(prevSongCommand);
                    break;
                case Key.Right: Browser.ExecuteJavascript(nextSongCommand);
                    break;
                case Key.PageUp: Browser.ExecuteJavascript(likeSongCommand);
                    break;
                case Key.PageDown: Browser.ExecuteJavascript(dislikeSongCommand);
                    break;
            }
        }

        public ContextMenu SystemMenu
        {
            get { return Resources["systemMenu"] as ContextMenu; }
        }

        private void ShowContextMenu()
        {
            if (SystemMenu != null) SystemMenu.IsOpen = true;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == WM_SYSTEMMENU) && (wParam.ToInt32() == WP_SYSTEMMENU))
            {
                ShowContextMenu();
                handled = true;
            }

            return IntPtr.Zero;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var windowhandle = new WindowInteropHelper(this).Handle;
            var hwndSource = HwndSource.FromHwnd(windowhandle);
            hwndSource.AddHook(WndProc);
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }

        private void Maxamise(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
        private void Minimise(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Restore(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Show();
        }
    }
}
