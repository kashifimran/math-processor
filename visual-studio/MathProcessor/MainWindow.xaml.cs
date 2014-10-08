using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using MathProcessorLib;
using System.Reflection;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Windows.Interop;

namespace MathProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fileVersion = "1.0";
        string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        string currentFile = "";
        static string mpExtension = "mp";
        static string mpFileFilter = "Math Processor File (*." + mpExtension + ")|*." + mpExtension;
        public bool Dirty { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(MainWindow_MouseDown), true);
            commandControl.Focus();
            SetTitle();
            CreateSamplesMenues();
            Task.Factory.StartNew(CheckForUpdate);
        }

        void CheckForUpdate()
        {
            if (ConfigManager.GetConfigurationValue("checkUpdates") == "false")
            {
                return;
            }
            try
            {
                string newVersion = version;
                using (WebClient client = new WebClient())
                {
                    newVersion = client.DownloadString("http://www.mathiversity.com/mathprocessor/version");
                }
                string[] newParts = newVersion.Split('.');
                string[] currentParts = version.Split('.');
                for (int i = 0; i < newParts.Count(); i++)
                {
                    if (int.Parse(newParts[i]) > int.Parse(currentParts[i]))
                    {
                        if (System.Windows.MessageBox.Show("A new version of Math Processor is available.\r\nWould you like to download the new version?",
                                            "New version available",
                                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Process.Start("http://www.mathiversity.com/Downloads");
                        }
                        break;
                    }
                    else if (int.Parse(newParts[i]) < int.Parse(currentParts[i]))
                    {
                        break;
                    }
                }
            }
            catch { } // hopeless..
        }

        void CreateSamplesMenues()
        {
            try
            {
                string[] sampleFiles = Directory.GetFiles("Examples");
                foreach (string s in sampleFiles)
                {
                    System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem();
                    item.Header = System.IO.Path.GetFileNameWithoutExtension(s);
                    item.Click += new RoutedEventHandler(item_Click);
                    examplesMenuItem.Items.Add(item);
                }
            }
            catch
            {
            }
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            string name = (sender as System.Windows.Controls.MenuItem).Header as string;
            RunCodeFile(new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Examples", name + ".txt")).LocalPath);
        }

        void SetTitle()
        {
            if (currentFile.Length > 0)
            {
                Title = currentFile + " - Math Processor v." + version;
            }
            else
            {
                Title = "Math Processor v." + version;
            }
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver != null)
            {
                if (commandControl.IsMouseOver)
                {
                    commandControl.Focus();
                }
            }
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        void LoadFile(Stream stream)
        {
            ZipInputStream zipInputStream = new ZipInputStream(stream);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            MemoryStream outputStream = new MemoryStream();
            if (zipEntry != null)
            {
                byte[] buffer = new byte[4096];
                StreamUtils.Copy(zipInputStream, outputStream, buffer);
            }
            outputStream.Position = 0;
            using (outputStream)
            {
                XDocument xDoc = XDocument.Load(outputStream);
                commandControl.OpenFile(xDoc.Root);
            }
            Dirty = false;
        }
        
        private void OpenCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckPathExists = true;
            ofd.Filter = mpFileFilter;
            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                try
                {
                    using (Stream stream = File.OpenRead(ofd.FileName))
                    {
                        LoadFile(stream);
                    }
                    currentFile = ofd.FileName;
                }
                catch
                {
                    currentFile = "";
                    System.Windows.MessageBox.Show("File could not be opened. It is either corrupt or permission was denied.", "Error");
                }
            }
            SetTitle();
            commandControl.Focus();
        }

        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessFileSave();
            commandControl.Focus();
        }

        private bool ProcessFileSave()
        {
            if (!File.Exists(currentFile))
            {
                string result = ShowSaveFileDialog(mpExtension, mpFileFilter);
                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }
                else
                {
                    currentFile = result;
                }
            }
            return SaveFile();
        }

        private void SaveAsCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            string result = ShowSaveFileDialog(mpExtension, mpFileFilter);
            if (!string.IsNullOrEmpty(result))
            {
                currentFile = result;
                SaveFile();
            }
            commandControl.Focus();
        }

        string ShowSaveFileDialog(string extension, string filter)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.DefaultExt = "." + extension;
            sfd.Filter = filter;
            bool? result = sfd.ShowDialog(this);
            if (result == true)
            {
                return System.IO.Path.GetExtension(sfd.FileName) == "." + extension ? sfd.FileName : sfd.FileName + "." + extension;
            }
            else
            {
                return null;
            }
        }

        private bool SaveFile()
        {
            try
            {
                XDocument xDoc = new XDocument();
                XElement root = new XElement("MathProcessor");
                root.Add(new XAttribute("fileVersion", fileVersion));
                root.Add(new XAttribute("appVersion", Assembly.GetEntryAssembly().GetName().Version));
                commandControl.SaveXML(root);
                xDoc.Add(root);
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        xDoc.Save(memoryStream);
                        memoryStream.Position = 0;
                        using (Stream stream = File.Open(currentFile, FileMode.Create))
                        {
                            ZipStream(memoryStream, stream, System.IO.Path.GetFileNameWithoutExtension(currentFile) + ".xml");
                            SetTitle();
                        }
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("Could not save file. Make sure the specified path is correct.", "Error");
                }
                Dirty = false;
                return true;
            }
            catch
            {
                System.Windows.MessageBox.Show("File could not be opened for writing. Make sure the file exists and is not already open elsewhere", "Error");
            }
            return false;
        }

        public void ZipStream(MemoryStream memStreamIn, Stream outputStream, string zipEntryName)
        {
            ZipOutputStream zipStream = new ZipOutputStream(outputStream);
            zipStream.SetLevel(5); //0-9, 9 being the highest level of compression
            ZipEntry newEntry = new ZipEntry(zipEntryName);
            newEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(newEntry);
            StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;	// False stops the Close also Closing the underlying stream.
            zipStream.Close();			// Must finish the ZipOutputStream before using outputMemStream.            
        }

        private void CutCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            commandControl.Cut();
        }

        private void CopyCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            commandControl.Copy();
        }

        private void PasteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            commandControl.Paste();
        }

        private void PrintCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void UndoCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void RedoCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void DecreaseZoomCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void IncreaseZoomCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void RunFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "Examples");
            ofd.Title = "Open comamnd file to execute";
            ofd.Filter = "File (.txt;*.*)|*.txt;*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RunCodeFile(ofd.FileName);
                //DisplayResult(result);
            }
        }

        private void RunCodeFile(string path)
        {
            try
            {
                FileStream textFile = File.Open(path, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(textFile);
                String data = reader.ReadToEnd();
                reader.Close();
                commandControl.RunFileCommand(data);
            }
            catch
            {
                System.Windows.MessageBox.Show("The file could not be opened for reading. Make sure the file is availabe.", "Error");
            }
        }

        private void DocumentationMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://MathiVersity.com/MathProcessor/Documentation");
        }

        private void TutorialsMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://MathiVersity.com/MathProcessor/Tutorials");
        }

        private void KitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            System.Windows.Forms.Form form = null;
            if (item != null)
            {
                switch (item.Header.ToString())
                {
                    case "_Basic Math":
                        form = new BasicKit(this);
                        break;
                    case "_Matrices":
                        form = new MatrixKit(this);
                        break;
                    case "_Truth Tables":
                        form = new BooleanKit(this);
                        break;
                }
                form.Show();
            }
        }

        private void clearAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            commandControl.Clear();
        }

        private void mainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0 || e.HorizontalChange != 0)
            {
                commandControl.InvalidateVisual();
            }
        }

        private void DeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            commandControl.DeleteSelected();
        }

        private void SelectAllCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            commandControl.SelectAll();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void fbMenu_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.facebook.com/mathiversity");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfigManager.ShowAd)
            {
                GamentryAd adForm = new GamentryAd();
                adForm.Owner = this;
                adForm.ShowDialog();
            }
        }

        private void SolveNow_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.Gamentry.com");
        }
    }
}
