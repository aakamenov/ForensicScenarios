using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class Screenshot2 : PropertyChangedBase, IScenario
    {
        public string Name => "Screenshot 1";

        public string Description { get; set; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));
            }
        }

        private bool isSelected;

        private readonly IEventAggregator eventAggregator;

        public Screenshot2(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "The Screenshot Scenario 2\n\nThis Screenshot scenario will take number of screenshots of the active window and the whole screen with the bot minimised and momentarily save the file to the desktop.\n\nThese files will then be renamed, have their extensions changed and be moved to various folders on your hard drive.";
        }

        public void Run()
        {
            string srcpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

            string dstpath1 = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\";
            string dstpath2 = Environment.GetFolderPath(Environment.SpecialFolder.Favorites) + "\\";

            string str1 = "Sir Rick Astley - Never Gonna Give You Up.mpeg";
            string str2 = "Tweenies.html";

            ClrPrevious(dstpath1, str1);
            CreateFile(srcpath, dstpath1, str1);

            ClrPrevious(dstpath2, str2);
            CreateFile(srcpath, dstpath2, str2);

            eventAggregator.BeginPublishOnUIThread(new ScenarioCompleted(this));
        }

        private void ClrPrevious(string dstpath, string renmefle)
        {
            string path = dstpath + renmefle;
            var msg = string.Empty;

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    msg = "Removing previous files...✔";
                }
            }
            catch
            {
                msg = "Removing previous files...✖";
            }

            eventAggregator.BeginPublishOnUIThread(new ScenarioStatusUpdated(this, msg));
        }

        private void CreateFile(string srcpath, string dstpath, string newfilename)
        {
            var msg = string.Empty;

            try
            {
                string str1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                string str2 = srcpath + "image" + str1 + ".jpeg";

                Image data = ScreenCapture.CaptureScreen();
                data.Save(str2, ImageFormat.Jpeg);
                msg = "Capturing screenshot and saving to desktop...✔\n";

                File.Move(str2, dstpath + newfilename);
                msg += "Moving file to a new location...✔";
            }
            catch (Exception)
            {
                msg = "Capturing screenshot and saving to desktop...✖\n";
                msg += "Moving file to a new location...✖";
            }

            eventAggregator.BeginPublishOnUIThread(new ScenarioStatusUpdated(this, msg));
        }
    }
}
