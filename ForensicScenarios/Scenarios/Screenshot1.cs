using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Caliburn.Micro;
using ForensicScenarios.Tools;

namespace ForensicScenarios.Scenarios
{
    internal class Screenshot1 : PropertyChangedBase, IScenario
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

        public int StatusValue
        {
            get => statusValue;
            private set
            {
                statusValue = value;
                NotifyOfPropertyChange(nameof(StatusValue));
            }
        }

        private const string NAME = "Screenshot 1"; //Used to control the text displayed in the listbox
        private string status;
        private int statusValue;

        public Screenshot1()
        {
            Description = "The Screenshot Scenario 1\n\nThis Screenshot scenario will take a number of screenshots of the active window and the whole screen and momentarily save the files to the desktop.\n\nThese files will then be renamed, have their extensions changed and be moved to various folders on your hard drive.";
        }

        public void Run()
        {
            string srcpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

            string dstpath1 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\";
            string dstpath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\";

            string str1 = "Not a suspicious file.txt";
            string str2 = "Daphne and Celeste - Ooh Stick You.mp3";

            ClrPrevious(dstpath1, str1);
            CreateFile(srcpath, dstpath1, str1);

            ClrPrevious(dstpath2, str2);
            CreateFile(srcpath, dstpath2, str2);

            StatusValue = 2000;
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
                    StatusValue = 2000;
                }
                else
                    StatusValue = 2000;
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

                StatusValue = 2000;
            }
            catch(Exception)
            {
                Status = "Capturing screenshot and saving to desktop...✖\n";
                Status = "Moving file to a new location...✖\n";
            }
        }
    }
}
