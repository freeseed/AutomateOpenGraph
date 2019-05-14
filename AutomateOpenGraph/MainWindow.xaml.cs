﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            lbStatus.Content = "Last Sent : -";

            gridTable.ItemsSource = stockDataList;


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
                    lbStatus.Content = "Last Sent : " + s.StockName + " (" + (gridTable.SelectedIndex+1).ToString() + "/" + gridTable.Items.Count.ToString() + ") View Time : " + SecondsToString(secondCount) ;
                    if (gridTable.SelectedIndex == gridTable.Items.Count-1)
                    {
                        timer.Stop();
                        lbMsg.Content = "Completed";
                        secondCount = 0;
                    }
                    gridTable.ScrollIntoView(s);
                    

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


        private void Command_Open()
        {
            string[] lines = { "" };
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Tab Separated Field Excel (*.xls)|*.xls|Text File (*.txt)|*.txt",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //openFileDialog.InitialDirectory = @"C:\Users\nevada\source\repos\AutomateOpenGraph";
            if (openFileDialog.ShowDialog() == true)
            {
                lines = System.IO.File.ReadAllLines(openFileDialog.FileName);
            }
            else
            {
                return;
            }

            char[] charSeparators = new char[] { '\t' };

            stockDataList.Clear();
            decimal tmpresult;
            int Id = 0;

            for (int i=1; i<lines.Length-1;i++)
            {
                string line = lines[i];
                StockInfo s = new StockInfo();
                string[] token = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                token[0] = token[0].Trim();
                token[1] = token[1].Trim();
                token[2] = token[2].Trim();
                if (!Regex.IsMatch(token[0], @"-[Ff]") && !Regex.IsMatch(token[0], @"REIT$") && !Regex.IsMatch(token[0], @"\d\d\d") && !Regex.IsMatch(token[0], @"IF$") && !Regex.IsMatch(token[0], @"PF$") && !Regex.IsMatch(token[0], @"GF$") && !Regex.IsMatch(token[0], @"RT$"))
                {
                    Id++;
                    s.Id = Id;
                    s.StockName = token[0];
                    s.ChangePercent = decimal.TryParse(token[1], out tmpresult) ? tmpresult : 0;
                    s.ClosedPrice = decimal.TryParse(token[2], out tmpresult) ? tmpresult : 0;
                    stockDataList.Add(s);
                }

            }

            stockDataList = stockDataList.OrderByDescending(o => o.ChangePercent).ToList();

            // to notify stockDataList is change.
            gridTable.ItemsSource = stockDataList;

            int itemCount = gridTable.Items.Count;
            if (itemCount > 0)
            {
                lbMsg.Content = itemCount.ToString() + " records. Next Send Keys";
            }
            else
            {
                lbMsg.Content = "File has no record. Please select new file";
            }
            timer.Stop();
            secondCount = 0;
            lbDataInfo.Content = "Total Record is " + itemCount.ToString() + " records  ( " + SecondsToString(itemCount * refreshInt) + " to view )";
            lbStatus.Content = "Last Sent : -";


        }

        private string SecondsToString(int sec)
        {
            int minutes = sec / 60;
            int seconds = sec % 60;
            return minutes.ToString() + " minutes and " + seconds.ToString() + " seconds";
        }

        private void Command_Start()
        {
            if (gridTable.Items.Count > 0)
            {
                lbMsg.Content = "0";
                timer.Start();
                gridTable.SelectedIndex = -1;
                secondCount = 0;
            }
            else
            {
                lbMsg.Content = "Please select data file to send keys.";
            }
        }


        private void Command_Stop()
        {
            timer.Stop();
            lbMsg.Content = lbMsg.Content + " Stoped";
        }

        private void Command_Resume()
        {
            timer.Start();
        }

        private void Command_Find()
        {
            if (txtSearch.Text.Trim() != "" && gridTable.Items.Count > 0)
            {
                StockInfo s = stockDataList.Find(o => o.StockName == txtSearch.Text.ToUpper());
                if (s != null)
                {
                   
                    lbMsg.Content = "Found '" + txtSearch.Text.ToUpper() + "'";
                    gridTable.ScrollIntoView(s);
                    gridTable.SelectedItem = s;

                } else
                {
                    lbMsg.Content = "Cannot found '" + txtSearch.Text.ToUpper() + "'";
                }
                
            }
            

        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Command_Open();
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (timer != null)
            {
                e.CanExecute = (!timer.IsEnabled) ? true : false;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void StartCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Command_Start();
        }

        private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (timer != null && gridTable != null)
            {
                e.CanExecute = (!timer.IsEnabled  && gridTable.Items.Count > 0) ? true : false;
            }else
            {
                e.CanExecute = false;
            }
            
        }

        private void StopCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Command_Stop();
        }

        private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (timer != null)
            {
                e.CanExecute = (timer.IsEnabled) ? true : false;
            }else
            {
                e.CanExecute = false;
            }
            
        }

        private void ResumeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Command_Resume();
        }

        private void ResumeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (timer != null && gridTable != null)
            {
                e.CanExecute = (!timer.IsEnabled && gridTable.Items.Count > 0) ? true : false;
            }else
            {
                e.CanExecute = false;
            }
            
        }

        private void FindCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Command_Find();
        }

        private void FindCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (timer != null && gridTable != null)
            {
                e.CanExecute = (!timer.IsEnabled && gridTable.Items.Count > 0) ? true : false;
            }
            else
            {
                e.CanExecute = false;
            }

        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Command_Find();
            }

        }
    }
}
