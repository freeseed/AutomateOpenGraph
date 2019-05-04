using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsInput;

namespace AutomateOpenGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private InputSimulator s = new InputSimulator();
        private int secondCount = 0;
        private List<StockInfo> stockDataList = new List<StockInfo>();
        private const int refreshInt = 5;


        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            lbMsg.Content = "Open file .xls to see data and Send Keys";
            lbDataInfo.Content = "Data is empty";

            gridTable.DataContext = stockDataList;

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            secondCount = secondCount + 1 ;
            int modResult = secondCount % refreshInt;

            if (modResult ==1) {
                lbMsg.Content = modResult.ToString();
            }

            if (modResult > 1) {
                lbMsg.Content = lbMsg.Content + " " + modResult.ToString();
            }
            

            if (modResult == 0)
            {
                if (gridTable.SelectedIndex >= -1 && gridTable.SelectedIndex < gridTable.Items.Count)
                {
                    gridTable.SelectedIndex = gridTable.SelectedIndex + 1;
                    StockInfo s = (StockInfo)gridTable.SelectedItem;
                    send_keys(s.StockName);
                    lbMsg.Content = lbMsg.Content + " " + s.StockName + " sent.";
                    if (gridTable.SelectedIndex == gridTable.Items.Count-1)
                    {
                        timer.Stop();
                        lbMsg.Content = "Completed";
                        secondCount = 0;
                    }

                }

            }

        }

        private void send_keys(string str)
        {
            s.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
            System.Threading.Thread.Sleep(500);

            s.Keyboard.TextEntry(str);
            System.Threading.Thread.Sleep(500);
            s.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
        }


        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = { "" };
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Tab Separated Fields (*.xls)|*.xls|Text File (*.txt)|*.txt";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.InitialDirectory = @"C:\Users\nevada\source\repos\AutomateOpenGraph";
            if (openFileDialog.ShowDialog() == true)
            {
                lines = System.IO.File.ReadAllLines(openFileDialog.FileName);
            }

            char[] charSeparators = new char[] { '\t' };

            stockDataList.Clear();

            for (int i=1; i<lines.Length-1;i++)
            {
                string line = lines[i];
                StockInfo s = new StockInfo();
                string[] token = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                s.StockName = token[0].Trim();
                s.ChangePercent = Convert.ToDecimal(token[1]);
                s.ClosedPrice = Convert.ToDecimal(token[2]);

                stockDataList.Add(s);
            }

            stockDataList = stockDataList.OrderBy(o => o.ChangePercent).ToList();

            gridTable.DataContext = stockDataList;
            int itemCount = gridTable.Items.Count;
            int minutes = (itemCount * refreshInt) / 60;
            int seconds = (itemCount * refreshInt) % 60;
            if (itemCount > 0)
            {
                lbMsg.Content = itemCount.ToString() + " records. Next Send Keys";
            }
            else
            {
                lbMsg.Content = "File has no record. Please select new file";
            }
            lbDataInfo.Content = "Total Record is " + itemCount.ToString() + "  ( " + minutes.ToString() + " minutes and " + seconds.ToString() + " seconds to view )";


        }

        private void MenuSendKey_Click(object sender, RoutedEventArgs e)
        {
            if (gridTable.Items.Count > 0)
            {
                lbMsg.Content = "0";
                timer.Start();
                gridTable.SelectedIndex = -1;

            }
            else
            {
                lbMsg.Content = "Please select data file to send keys.";
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
