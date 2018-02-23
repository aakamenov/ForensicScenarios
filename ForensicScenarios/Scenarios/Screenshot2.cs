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
        public string Description { get; set; }

        public string Status
        {
            get => status;
            private set
            {
                status = value;
                NotifyOfPropertyChange(nameof(Status));
            }
        }

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
        private string status;

        private const string NAME = "Screenshot 2"; //Used to control the text displayed in the listbox
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

        public override string ToString()
        {
            return NAME;
        }

        private void ClrPrevious(string dstpath, string renmefle)
        {
            string path = dstpath + renmefle;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Status = "Removing previous files...✔\n";
                }
            }
            catch
            {
                Status = "Removing previous files...✖\n";
            }
        }

        private void CreateFile(string srcpath, string dstpath, string newfilename)
        {
            try
            {
                string str1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                string str2 = srcpath + "image" + str1 + ".jpeg";

                Status = "Capturing screenshot and saving to desktop...✔\n";
                Image data = ScreenCapture.CaptureScreen();
                data.Save(str2, ImageFormat.Jpeg);

                Status = "Moving file to a new location...✔\n";
                File.Move(str2, dstpath + newfilename);
            }
            catch (Exception)
            {
                Status = "Capturing screenshot and saving to desktop...✖\n";
                Status = "Moving file to a new location...✖\n";
            }
        }
    }
}
