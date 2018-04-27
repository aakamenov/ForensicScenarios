using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Forms = System.Windows.Forms;
using Caliburn.Micro;
using ForensicScenarios.Tools;
using ForensicScenarios.Events;

namespace ForensicScenarios.Scenarios
{
    public class Screenshot1 : PropertyChangedBase, IScenario
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

        public Screenshot1(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            Description = "This Screenshot scenario will take a number of screenshots of the active window and the whole screen and momentarily save the files to the desktop.\n\nThese files will then be renamed, have their extensions changed and be moved to various folders on your hard drive.";
        }

        public void Run()
        {
            string srcpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

            string dstpath1 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\";
            string dstpath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\";

            string str1 = "Not a suspicious file.txt";
            string str2 = "Daphne and Celeste - Ooh Stick You.mp3";

            string keysent1 = "%{PRTSC}";
            string keysent2 = "{PRTSC}";

            ClrPrevious(dstpath1, str1);
            CreateFile(srcpath, dstpath1, str1, keysent1);

            ClrPrevious(dstpath2, str2);
            CreateFile(srcpath, dstpath2, str2, keysent2);

            eventAggregator.SendStatusInfo(this, "Screenshot 1 complete...✔");
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

            eventAggregator.SendStatusInfo(this, msg);
        }

        private void CreateFile(string srcpath, string dstpath, string newfilename, string keysent)
        {
            var msg = string.Empty;

            try
            {
                var str1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                var str2 = srcpath + "image" + str1 + ".jpeg";

                Forms.SendKeys.SendWait(keysent);
                Image data = (Image)Forms.Clipboard.GetDataObject().GetData(Forms.DataFormats.Bitmap);

                data.Save(str2, ImageFormat.Jpeg);
                msg = "Capturing screenshot and saving to desktop...✔\n";

                File.Move(str2, dstpath + newfilename);
                msg += "Moving file to a new location...✔";
            }
            catch(Exception)
            {
                msg = "Capturing screenshot and saving to desktop...✖\n";
                msg += "Moving file to a new location...✖";
            }

            eventAggregator.SendStatusInfo(this, msg);
        }
    }
}
