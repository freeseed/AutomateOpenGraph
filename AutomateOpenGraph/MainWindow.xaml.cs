﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
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
        private List<StockInfo> stockDataListS100 = new List<StockInfo>();
        private List<StockInfo> stockDataListS50 = new List<StockInfo>();
        private List<StockInfo> stockDataListExcludeS100 = new List<StockInfo>();
        private List<StockInfo> stockDataListWar = new List<StockInfo>();
        private List<StockInfo> stockDataListMarket = new List<StockInfo>();
        private List<StockInfo> stockDataListS50DW = new List<StockInfo>();
        private List<StockInfo> stockDataListAllDW = new List<StockInfo>();
        private List<StockInfo> stockDataListCustom = new List<StockInfo>();
        private List<StockInfo> curStockDataList;

        private List<StockInfo> ipoList = new List<StockInfo>();
        private List<StockInfo> ipoWarList = new List<StockInfo>();
        private List<StockInfo> sectorList = new List<StockInfo>();

        private string mode = "";

        private int refreshInt = 6;
        // data ignore list as of 16-May-2019
        private string[] ignoreArr = { };
        private string[] set100Arr = { };
        private string[] set50Arr = { };
        private string[] customArr = { };
        private string[] marketArr = { };



        private void RemoveSpace(string[] x)
        {
            for (int i=0; i < x.Length; i++)
                x[i] = x[i].Trim();
        }

        public MainWindow()
        {
            InitializeComponent();

            //this.Title = this.Title + " - Debug";
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;

            lbMsg.Content = "Open file .xls to see data and Send Keys";
            lbDataInfo.Content = "Data is empty";
            lbStatus.Content = "Last Sent : -";
            lbFileName.Content = "File Name : -";

            

            gridTable.ItemsSource = stockDataList;

            string strSET50Setting = Properties.Settings.Default.set50;
            string strSET100Setting = Properties.Settings.Default.set100;
            string strDelaySecond = Properties.Settings.Default.delaysec;
            string strCustomList = Properties.Settings.Default.customlist;
            string strMarketSetting = Properties.Settings.Default.market.Replace("TFEX", TfexSeriesCode);


            char[] sep = new char[] { ',' };
            set50Arr = strSET50Setting.Split(sep);
            RemoveSpace(set50Arr);

            set100Arr = strSET100Setting.Split(sep);
            RemoveSpace(set100Arr);

            customArr = strCustomList.Split(sep);
            RemoveSpace(customArr);

            marketArr = strMarketSetting.Split(sep);
            RemoveSpace(marketArr);

            refreshInt = int.TryParse(strDelaySecond, out int tmpresult) ? tmpresult : 6;  //int.Parse(strDelaySecond);

            txtDelay.Text = refreshInt.ToString();

            set100Arr = Array.FindAll(set100Arr, x => !set50Arr.Contains(x));

            set50Arr = Array.FindAll(set50Arr, x => x != "X-X");

            Array.Sort(ignoreArr);

            Array.Sort(set50Arr);

            Array.Sort(set100Arr);

            Array.Sort(customArr);

            Console.WriteLine(set50Arr);

            AddMarketSymbol(marketArr);

            string text;
            IComparer<StockInfo> sortbyDate = new SortByDate();

            try
            {
                text = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ipo.json");
                ipoList = JsonConvert.DeserializeObject<List<StockInfo>>(text);
                ipoList.Sort(sortbyDate);
            }
            catch (Exception e)
            {

                lbMsg.Content = lbMsg.Content + e.Message;
            }




            try
            {
                text = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "ipowar.json");
                ipoWarList = JsonConvert.DeserializeObject<List<StockInfo>>(text);
                ipoWarList.Sort(sortbyDate);
            }
            catch (Exception e)
            {

                lbMsg.Content = lbMsg.Content + e.Message;
            }

            try
            {
                text = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "sector.json");
                sectorList = JsonConvert.DeserializeObject<List<StockInfo>>(text);
                //ipoWarList.Sort(sortbyDate);
            }
            catch (Exception e)
            {

                lbMsg.Content = lbMsg.Content + e.Message;
            }


            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory + " " +  System.Reflection.Assembly.GetEntryAssembly().Location);


        }

        private void CreateIPOList()
        {
           /* ipoList.Add(new StockInfo("ETC", new DateTime(2020, 8, 18), 5824, "mai", 2.6));  // มุกเดิม pe 85 แต่ทำ eps มา เอาแบบสูงสุดที่เคยมี 

            //string json = JsonConvert.SerializeObject((new StockInfo("ETC", new DateTime(2020, 8, 18), 5824, "mai", 2.6)));
            //Console.WriteLine(json);

            ipoList.Add(new StockInfo("IIG", new DateTime(2020, 8, 6), 552, "mai", 6.6));  // มุกเดิม pe 12 แต่ทำ eps มา พึคมาก 
            ipoList.Add(new StockInfo("SICT", new DateTime(2020, 7, 30), 552, "mai", 1.38));
            ipoList.Add(new StockInfo("STGT", new DateTime(2020, 7, 2), 48578, "SET", 34 ));
            ipoList.Add(new StockInfo("CRC", new DateTime(2020, 2, 20), 253302, "SET", 42));
            ipoList.Add(new StockInfo("SFLEX", new DateTime(2019, 12, 19), 1590, "SET", 3.88 ));
            ipoList.Add(new StockInfo("BAM", new DateTime(2019, 12, 16), 52760, "SET", 17.5 ));
            ipoList.Add(new StockInfo("ACE", new DateTime(2019, 11, 13),44774, "SET", 4.4));

            ipoList.Add(new StockInfo("CPW", new DateTime(2019, 10, 18), 1428, "SET", 2.38));
            ipoList.Add(new StockInfo("DOHOME", new DateTime(2019, 8, 6), 14478, "SET", 7.8));
            ipoList.Add(new StockInfo("ILM", new DateTime(2019, 7, 26), 11110, "SET", 22));
            ipoList.Add(new StockInfo("RBF", new DateTime(2019, 10, 24), 6600, "SET", 3.3));

            ipoList.Add(new StockInfo("AWC", new DateTime(2019, 10, 10), 185742, "SET", 6.0));
            ipoList.Add(new StockInfo("SHR", new DateTime(2019, 11, 12), 18686, "SET", 5.2));
            ipoList.Add(new StockInfo("VRANDA", new DateTime(2019, 5, 3), 3196, "SET", 10));
            ipoList.Add(new StockInfo("ZEN", new DateTime(2019, 2, 20), 3900, "SET", 13));

            ipoList.Add(new StockInfo("ACG", new DateTime(2019, 6, 27), 864, "mai", 1.44));
            ipoList.Add(new StockInfo("ALL", new DateTime(2019, 5, 8), 2744, "mai", 4.9));
            ipoList.Add(new StockInfo("APP", new DateTime(2019,11, 22), 688, "mai", 2.46));
            ipoList.Add(new StockInfo("ARIN", new DateTime(2019, 7, 10), 1080, "mai", 1.8));

            ipoList.Add(new StockInfo("YGG", new DateTime(2020, 1, 7), 900, "mai", 5));
            ipoList.Add(new StockInfo("VL", new DateTime(2019, 5, 21), 1400, "mai", 1.75));
            ipoList.Add(new StockInfo("TPS", new DateTime(2019, 11, 15), 700, "mai", 2.5));
            ipoList.Add(new StockInfo("STC", new DateTime(2019, 11, 29), 568, "mai", 1));

            ipoList.Add(new StockInfo("SAAM", new DateTime(2019, 1, 7), 540, "mai", 1.8));
            ipoList.Add(new StockInfo("MITSIB", new DateTime(2019, 6, 11), 1667, "mai", 2.5));
            ipoList.Add(new StockInfo("KUN", new DateTime(2019, 12, 17), 660, "mai", 1.1));
            ipoList.Add(new StockInfo("KUMWEL", new DateTime(2019, 8, 1), 473, "mai", 1));

            ipoList.Add(new StockInfo("IP", new DateTime(2019, 11, 5), 1442, "mai", 7));
            ipoList.Add(new StockInfo("INSET", new DateTime(2019, 10, 8), 1506, "mai", 2.69));
            ipoList.Add(new StockInfo("IMH", new DateTime(2019, 12, 26), 1290, "mai", 6));
            ipoList.Add(new StockInfo("GSC", new DateTime(2019, 3, 13), 425, "mai", 1.7));

            ipoList.Add(new StockInfo("CAZ", new DateTime(2019, 1, 22), 1092, "mai", 3.9));
            ipoList.Add(new StockInfo("BC", new DateTime(2019, 11, 14), 1450, "mai", 2.86));
            */

        }

        private void CreateIPOWarList()
        {
          /*  ipoWarList.Add(new StockInfo("ALL-W1", new DateTime(2020, 8, 28), 140, "mai", 2.8));
            ipoWarList.Add(new StockInfo("ITEL-W2", new DateTime(2020, 8, 27), 250, "mai", 3.00));
            ipoWarList.Add(new StockInfo("MINT-W7", new DateTime(2020, 8, 26), 235, "SET", 21.6));
            ipoWarList.Add(new StockInfo("UREKA-W2", new DateTime(2020, 7, 10), 295, "SET", 1));
            ipoWarList.Add(new StockInfo("JMART-W3", new DateTime(2020, 7, 3), 100, "SET", 11));
            ipoWarList.Add(new StockInfo("JMART-W4", new DateTime(2020, 7, 3), 100, "SET", 15));
            ipoWarList.Add(new StockInfo("NER-W1", new DateTime(2020, 6, 18), 307, "SET", 1.8));
            ipoWarList.Add(new StockInfo("NEX-W2", new DateTime(2020, 6, 18), 223, "SET", 1));

            //ipoWarList.Add(new StockInfo("CIG-W8", new DateTime(2020, 6, 10), 432, "SET", 1));
            ipoWarList.Add(new StockInfo("CHAYO-W1", new DateTime(2020, 6, 9), 209, "SET", 6.5));
            ipoWarList.Add(new StockInfo("III-W1", new DateTime(2020, 5, 28), 152, "SET", 6));
            ipoWarList.Add(new StockInfo("TAPAC-W4", new DateTime(2020, 4, 13), 205, "SET", 9));

            ipoWarList.Add(new StockInfo("D-W1", new DateTime(2020, 3, 17), 40, "SET", 4));
            ipoWarList.Add(new StockInfo("BTS-W5", new DateTime(2020, 2, 27), 1315, "SET", 14));
            ipoWarList.Add(new StockInfo("SMART-W2", new DateTime(2019, 10, 22), 91, "SET", 1.5));
            ipoWarList.Add(new StockInfo("B-W5", new DateTime(2019, 9, 26), 290, "SET", 0.35));

            */
        }

        private string TfexSeriesCode
        {
            get
            {

                DateTime testDate = DateTime.Parse($"2000-{DateTime.Now.ToString("MM-dd")}");
                DateTime hDate = DateTime.Parse("2000-12-27");
                DateTime mDate = DateTime.Parse("2000-03-27");
                DateTime uDate = DateTime.Parse("2000-06-27");
                DateTime zDate = DateTime.Parse("2000-09-27");
                DateTime yearDate = DateTime.Now;
                string symbolQuater;

                //below logic will give invalid symbol on efin during 28-30 Dec. Coz S50 will 
                //expired around 27-dec but this logic still give
                if (testDate >= hDate)
                {
                    symbolQuater = "H";
                    yearDate = yearDate.AddDays(31);
                }
                else if (testDate >= zDate)
                {
                    symbolQuater = "Z";
                }
                else if (testDate >= uDate)
                {
                    symbolQuater = "U";
                }
                else if (testDate >= mDate)
                {
                    symbolQuater = "M";
                }
                else
                {
                    symbolQuater = "H";
                }
                return $"S50{symbolQuater}{yearDate.ToString("yy")}";
            }
        }

        //public string[] ignoreArr { get => ignoreArr; set => ignoreArr = value; }

        private void Timer_Tick(object sender, EventArgs e)
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

                    if (txtBaseURL.Text.Trim() == "")
                        Send_keys(s.StockName);
                    else
                        Send_keys_withBaseURL(s.StockName);

                    lbMsg.Content = $"{lbMsg.Content} {s.StockName} sent.";
                    lbStatus.Content = $"Last Sent : {s.StockName} ({(gridTable.SelectedIndex+1).ToString() }/{gridTable.Items.Count.ToString()}) View Time : {SecondsToString(secondCount)}" ;
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

        private void Send_keys(string str)
        {
            s.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SPACE);
            System.Threading.Thread.Sleep(500);

            s.Keyboard.TextEntry(str);
            System.Threading.Thread.Sleep(500);
            s.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
        }

        private void Send_keys_withBaseURL(string str)
        {
            s.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_L);
            System.Threading.Thread.Sleep(500);

            s.Keyboard.TextEntry(txtBaseURL.Text.Replace("xxx",str));
            System.Threading.Thread.Sleep(500);
            s.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
        }

        private void Command_Open()
        {
            string[] lines = { "" };
            string strDataPath = Properties.Settings.Default.datapath;
            if(!System.IO.Directory.Exists(strDataPath))
            {
                strDataPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Tab Separated Field Excel (*.xls)|*.xls|Text File (*.txt)|*.txt",
                InitialDirectory = strDataPath
            };

            if (openFileDialog.ShowDialog() == true)
            {
                lines = System.IO.File.ReadAllLines(openFileDialog.FileName);
                lbFileName.Content = "File Name : " + openFileDialog.FileName;
            }
            else
            {
                return;
            }

            char[] charSeparators = new char[] { '\t' };

            stockDataList.Clear();
            stockDataListS100.Clear();
            stockDataListS50.Clear();
            stockDataListExcludeS100.Clear();
            stockDataListWar.Clear();
            stockDataListS50DW.Clear();
            stockDataListAllDW.Clear();
            stockDataListCustom.Clear();

            for (int i = 1; i < lines.Length - 1; i++)
                ProcessTextLine(lines, charSeparators, i);

            //remove 
            //AddTfexSymbol();

            stockDataList = stockDataList.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListS100 = stockDataListS100.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListS50 = stockDataListS50.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListExcludeS100 = stockDataListExcludeS100.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListWar = stockDataListWar.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListS50DW = stockDataListS50DW.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListAllDW = stockDataListAllDW.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListCustom = stockDataListCustom.OrderByDescending(o => o.ChangePercent).ToList();


            curStockDataList = stockDataList;
            gridTable.ItemsSource = curStockDataList;
            mode = "All";

            SetUIAfterRefreshStockList(curStockDataList);

            if (openFileDialog.FileName.Contains("_all"))
            {
                CheckWhatIsMissing();
            }
            


        }

        private void ProcessTextLine(string[] lines, char[] charSeparators, int i)
        {
            string line = lines[i];
            StockInfo s = new StockInfo();
            string[] token = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            token[0] = token[0].Trim();
            token[1] = token[1].Trim();
            token[2] = token[2].Trim();
            if ( !Regex.IsMatch(token[0], @"-F$") && !Regex.IsMatch(token[0], @"\d\d\d")) //!Regex.IsMatch(token[0], @"\d\d\d") &&
            {
                //Array.BinarySearch(ignoreArr, token[0]) < 0
                //if ( !ignoreArr.Contains(token[0]) && !Regex.IsMatch(token[0], @"IF$"))


                s.StockName = token[0];
                s.ChangePercent = decimal.TryParse(token[1], out decimal tmpresult) ? tmpresult : 0;
                s.ClosedPrice = decimal.TryParse(token[2], out tmpresult) ? tmpresult : 0;

                if (!Regex.IsMatch(token[0], @"-W"))
                    stockDataList.Add(s);

                //if (token[0] == "ALL-W1")
                //    Console.WriteLine("Find Advance: " + token[0]);

                //remove search by binarySearch coz array is not sorted 
                //int inList100 = Array.BinarySearch(set100Arr, token[0]);
                //int inList50 = Array.BinarySearch(set50Arr, token[0]);

                if (set100Arr.Contains(token[0])) stockDataListS100.Add(s);
                else if (set50Arr.Contains(token[0])) stockDataListS50.Add(s);
                else if (Regex.IsMatch(token[0], @"-W"))
                {
                    /*StockInfo ss = new StockInfo();
                    ss.StockName = s.StockName.Substring(0, s.StockName.IndexOf('-'));
                    ss.ChangePercent =  s.ChangePercent;
                    ss.ClosedPrice =  s.ClosedPrice;*/
                    stockDataListWar.Add(s);
                    //stockDataListWar.Add(ss);

                }
                else if (Regex.IsMatch(token[0], @"^S50")) stockDataListS50DW.Add(s);
                else if (Regex.IsMatch(token[0], @"\d\d\d")) stockDataListAllDW.Add(s);
                else stockDataListExcludeS100.Add(s);

                if (customArr.Contains(token[0]))
                    stockDataListCustom.Add(s);


            }


        }

        private void CheckWhatIsMissing()
        {
            txtLoadingLog.Inlines.Clear();

            Array.ForEach(set100Arr, (x) =>
            {
                  if (!stockDataListS100.Contains(new StockInfo(x, 0, 0)))
                   {
                    //Console.WriteLine("stockDataListS100 not contains: " + x);
                    txtLoadingLog.Inlines.Add("S100 no: " + x + " ");
                   }
                    

            });

            Array.ForEach(set50Arr, (x) =>
            {

                if (!stockDataListS50.Contains(new StockInfo(x, 0, 0)))
                {
                    //Console.WriteLine("stockDataListS50 not contains: " + x);
                    txtLoadingLog.Inlines.Add("S50 no: " + x + " ");
                }
                    

                // this is not work boz stockDataListS50 order by percentchange then we can not use binarysearch on stockname
                // but now we can call binarysearch success after implement IComparable  
                // Console.WriteLine("Index of: " + x + " is " + stockDataListS50.BinarySearch(new StockInfo(x, 0, 0)).ToString()); 

            });

        }

        private void AddMarketSymbol(string[] marketArr)
        {

            foreach(string s in marketArr)
            {
                StockInfo tmp = new StockInfo
                {
                    StockName = s,
                    ChangePercent = 0,
                    ClosedPrice = 0
                };
                stockDataListMarket.Add(tmp);
            }

        }


        private void SetUIAfterRefreshStockList(List<StockInfo> curStockDataList)
        {
            mode = $"[{mode}]";

            int itemCount = curStockDataList.Count;
            lbMsg.Content = itemCount > 0 ? mode + " " + itemCount.ToString() + " records." : "File has no record. Please select new file";
            timer.Stop();
            secondCount = 0;
            lbDataInfo.Content = $"Mode {mode} : Total Record is {itemCount.ToString()} records  ( {SecondsToString(itemCount * refreshInt)} )to view )";
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
                StockInfo s = curStockDataList.Find(o => o.StockName == txtSearch.Text.ToUpper());
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


        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Command_Find();
            }

        }

        private void SetListToGrid(List<StockInfo> StockList)
        {
            if (timer.IsEnabled) return;

            curStockDataList = StockList;
            gridTable.ItemsSource = StockList;
            SetUIAfterRefreshStockList(StockList);
        }

        private void AllButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "All";
            SetListToGrid(stockDataList);
        }
        private void Set100Button_Click(object sender, RoutedEventArgs e)
        {
            mode = "SET100";
            SetListToGrid(stockDataListS100);
        }
        private void ExcSet100Button_Click(object sender, RoutedEventArgs e)
        {
            mode = "Small";
            SetListToGrid(stockDataListExcludeS100);
        }

        private void WarrantButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "War";
            SetListToGrid(stockDataListWar);
        }

        private void Set50Button_Click(object sender, RoutedEventArgs e)
        {
            mode = "SET50";
            SetListToGrid(stockDataListS50);
        }

        private void S50DWButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "S50DW";
            SetListToGrid(stockDataListS50DW);
        }

        private void AllDWButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "DW";
            SetListToGrid(stockDataListAllDW);
        }

        private void TxtDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshInt = int.TryParse(txtDelay.Text, out int tmpresult) ? tmpresult : 6;
            //Console.WriteLine("refreshInt " + refreshInt.ToString());
        }

        private void MarketButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "Market";
            SetListToGrid(stockDataListMarket);
        }

        private void IPOButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "IPO";
            SetListToGrid(ipoList);
        }

        private void IPOWarButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "IPO War";
            SetListToGrid(ipoWarList);
        }

        //CustomButton_Click
        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "Custom";
            SetListToGrid(stockDataListCustom);
        }

        private void SectorButton_Click(object sender, RoutedEventArgs e)
        {
            mode = "Sector";
            SetListToGrid(sectorList);
        }
    }
}
