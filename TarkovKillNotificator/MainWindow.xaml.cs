using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using IniParser;
using IniParser.Model;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace TarkovKillNotificator
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private delegate void PrintLogDelegate(string message);
        private delegate void UpdateKillCountDelegate();

        private Config config;
        private SoundPlayer soundPlayer;
        private FileSystemWatcher watcher;
        private KillPopup killPopup;

        private bool isWatch = false;

        private int killCount = 0;

        public MainWindow()
        {
            this.Icon = IconToImageSource(Properties.Resources.InsideIcon);

            InitializeComponent();

            var parser = new FileIniDataParser();
            config = new Config();

            // 설정 값으로 UI 초기화
            HighlightsPath.Text = config.Get("HighlightsPath");
            CustomSoundPath.Text = config.Get("CustomSoundPath");

            CustomSound.IsChecked = (config.Get("CustomSound") == "True" ? true : false);
            AlwaysTop.IsChecked = (config.Get("AlwaysTop") == "True" ? true : false);
            PlaySound.IsChecked = (config.Get("PlaySound") == "True" ? true : false);

            // 항상위 처리.
            if (AlwaysTop.IsChecked == true) this.Topmost = true;
            else this.Topmost = false;

            killPopup = new KillPopup();
            killPopup.Topmost = true;
        }
        public BitmapFrame IconToImageSource(System.Drawing.Icon ico)
        {
            MemoryStream iconStream = new MemoryStream();
            ico.Save(iconStream);
            iconStream.Seek(0, SeekOrigin.Begin);
            return BitmapFrame.Create(iconStream);
        }

        private void PrintLog(string message)
        {
            if (!Log.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new PrintLogDelegate(PrintLog), message);
                return;
            }

            Log.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + message);


            try
            {
                ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(Log);

                IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;

                if (scrollInterface.VerticallyScrollable) scrollInterface.Scroll(scrollHorizontal, scrollVertical);
            }
            catch
            {
                // 하단 내리기 실패.
            }
        }

        private void UpdateKillCount()
        {
            if (!KillCount.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(new UpdateKillCountDelegate(UpdateKillCount));
                return;
            }

            KillCount.Content = "Kill: " + killCount;
            killPopup.KillCount.Content = "Kill: " + killCount;
        }

        // 체크박스 클릭 이벤트
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var obj = (CheckBox)sender;
            config.Set(obj.Name, obj.IsChecked.ToString());
        }

        private void AlwaysTop_Click(object sender, RoutedEventArgs e)
        {
            var obj = (CheckBox)sender;
            config.Set(obj.Name, obj.IsChecked.ToString());

            if (obj.IsChecked == true) this.Topmost = true;
            else this.Topmost = false;
        }

        // 버튼 클릭 이벤트
        private void FindHighlights_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            if (config.Get("HighlightsPath") != "" && Directory.Exists(config.Get("HighlightsPath")))
            {
                dialog.InitialDirectory = config.Get("HighlightsPath");
            }
            dialog.Title = "하이라이트 폴더를 선택해주세요."; // instead of default "Save As"
            dialog.Filter = "Directory|*.선택합니다"; // Prevents displaying files
            dialog.FileName = "이 폴더를 "; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\이 폴더를 .선택합니다", "");
                path = path.Replace(".선택", "");
                // If user has changed the filename, create the new directory
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                // Our final value is in path
                config.Set("HighlightsPath", path);
                HighlightsPath.Text = path;
            }
        }

        private void FindCustomSound_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (config.Get("CustomSoundPath") != "" && File.Exists(config.Get("CustomSoundPath")))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(config.Get("CustomSoundPath"));
            }
            dialog.Title = "사운드 파일을 선택해주세요."; // instead of default "Save As"
            dialog.Filter = "wav|*.wav"; // Prevents displaying files
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                if (File.Exists(path))
                {
                    // Our final value is in path
                    config.Set("CustomSoundPath", path);
                    CustomSoundPath.Text = path;
                }
            }
        }

        private void OpenPopup_Click(object sender, RoutedEventArgs e)
        {
            if (killPopup.IsVisible == true)
            {
                killPopup.Hide();
                OpenPopup.Content = "팝업 열기";
            }
            else
            {
                killPopup.Show();
                OpenPopup.Content = "팝업 닫기";
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (isWatch)
            {
                // 종료
                soundPlayer.Dispose();
                watcher.EnableRaisingEvents = false;
                watcher.Created -= new FileSystemEventHandler(WatcherCreated);
                watcher.Deleted -= new FileSystemEventHandler(WatcherDeleted);
                watcher.Dispose();

                isWatch = false;
                CustomSound.IsEnabled = true;
                Start.Content = "모니터링 시작";
                killCount = 0;
                UpdateKillCount();
                PrintLog("모니터링이 종료되었습니다.");
                return;
            }

            if (!Directory.Exists(config.Get("HighlightsPath")))
            {
                PrintLog("하이라이트 폴더를 선택해주세요.");
                return;
            }

            Stream soundStream;

            if (CustomSound.IsChecked == true)
            {
                if (File.Exists(config.Get("CustomSoundPath")))
                {
                    soundStream = File.OpenRead(config.Get("CustomSoundPath"));
                }
                else
                {
                    PrintLog("커스텀 사운드 파일을 찾을 수 없습니다.");
                    return;
                }
            }
            else
            {
                soundStream = Properties.Resources.sound;
            }

            try
            {
                soundPlayer = new SoundPlayer(soundStream);
                soundPlayer.Load();
            }
            catch
            {
                PrintLog("커스텀 사운드 파일을 로딩할 수 없습니다.");
                return;
            }

            soundPlayer.Play();

            watcher = new FileSystemWatcher { Path = config.Get("HighlightsPath"), IncludeSubdirectories = true, Filter = "*.mp4" };
            watcher.EnableRaisingEvents = true;
            watcher.Created += new FileSystemEventHandler(WatcherCreated);
            watcher.Deleted += new FileSystemEventHandler(WatcherDeleted);

            isWatch = true;
            CustomSound.IsEnabled = false;
            Start.Content = "모니터링 종료";
            PrintLog("모니터링이 시작되었습니다.");
        }

        private void WatcherCreated(object source, FileSystemEventArgs e)
        {
            // 킬 증가
            PrintLog("킬이 감지되었습니다. " + e.Name);
            killCount += 1;
            UpdateKillCount();
            if(config.Get("PlaySound") == "True") soundPlayer.Play();
        }

        private void WatcherDeleted(object source, FileSystemEventArgs e)
        {
            // 게임 종료
            if (killCount > 0)
            {
                PrintLog("게임이 종료되었습니다. 킬을 초기화합니다.");
                killCount = 0;
                UpdateKillCount();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            killPopup.Close();
        }
    }
}
