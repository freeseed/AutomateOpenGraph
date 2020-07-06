using Microsoft.Win32;
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
        private List<StockInfo> curStockDataList;
        private List<StockInfo> ipoList = new List<StockInfo>();

        private string mode = "";

        private int refreshInt = 6;
        // data ignore list as of 16-May-2019
        private string[] ignoreArr = { "AIMIRT", "AMATAR", "B-WORK", "BKKCP", "BOFFICE", "CPNCG", "CPNREIT", "CPTGF", "CRYSTAL", "CTARAF", "DREIT", "ERWPF", "FTREIT", "FUTUREPF", "GAHREIT", "GLANDRT", "GOLDPF", "GVREIT", "HPF", "HREIT", "IMPACT", "KPNPF", "LHHOTEL", "LHPF", "LHSC", "LUXF", "M-II", "M-PAT", "M-STOR", "MIPF", "MIT", "MJLF", "MNIT", "MNIT2", "MNRF", "MONTRI", "POPF", "PPF", "QHHR", "QHOP", "QHPF", "SBPF", "SHREIT"
                                        , "SIRIP", "SPF", "SPRIME", "SRIPANWA", "SSPF", "SSTRT", "TIF1", "TLGF", "TLHPF", "TNPF", "TPRIME", "TTLPF", "TU-PF", "URBNPF", "WHABT", "WHART", "AIMCG", "GOLD","LHFG","THE","EVER","AJA","NWR","DTC","PLE","TRITN","PACE","PREB","BA","BLAND","ESTAR","TRC","GENCO","NDR","X-X" };

        private string[] set100Arr = { "ADVANC", "AEONTS", "AMATA", "AOT", "AP", "AWC", "BANPU", "BBL", "BCH", "BCP", "BCPG", "BDMS", "BEC", "BEM", "BGRIM", "BH", "BJC", "BPP", "BTS", "CBG", "CENTEL", "CHG", "CK", "CKP", "COM7", "CPALL", "CPF", "CPN", "CRC", "DELTA", "DTAC", "EA", "EGCO", "EPG", "ERW", "ESSO", "GFPT", "GLOBAL", "GPSC", "GULF", "GUNKUL", "HANA", "HMPRO", "INTUCH", "IRPC", "IVL", "JAS", "JMT"
                                        , "KBANK", "KCE", "KKP", "KTB", "KTC", "LH", "MAJOR", "MEGA", "MINT", "MTC", "ORI", "OSP", "PLANB", "PRM", "PSH", "PTG", "PTT", "PTTEP", "PTTGC", "QH", "RATCH", "RS", "SAWAD", "SCB", "SCC", "SGP", "SPALI", "SPRC", "STA", "STEC", "SUPER", "TASCO", "TCAP", "THANI", "TISCO", "TKN", "TMB", "TOA", "TOP", "TPIPP", "TQM", "TRUE", "TTW", "TU", "VGI", "WHA", "X-X", "DOHOME", "AAV","ACE","RBF","PSL","TVO"  };

        private string[] set50Arr = { "ADVANC", "AOT", "AWC", "BANPU", "BBL", "BDMS", "BEM", "BGRIM", "BH", "BJC", "BTS", "CBG", "CPALL", "CPF", "CPN", "CRC", "DTAC", "EA", "EGCO", "GLOBAL", "GPSC", "GULF", "HMPRO", "INTUCH", "IRPC", "IVL", "KBANK", "KTB", "KTC", "LH", "MINT", "MTC", "OSP", "PTT", "PTTEP", "PTTGC", "RATCH", "SAWAD", "SCB", "SCC", "TCAP", "TISCO", "TMB", "TOA", "TOP", "TRUE", "TU", "VGI", "WHA", "X-X", "BAM","MAKRO","BANPU"};
        //remove banpu delta 4-jul-2020

        //for ipo 1 year setup
        //private string[] ipoArr = { "STGT","CRC","SFLEX","BAM","ACE" };

        //Begin 1July2019 announce 18June2019
        //SET50 remove CENTEL SPRC in OSP SAWAD 
        //SET100 remove GOLD WHAUP WORK in JAS JMT OSP CENTEL SPRC
        //manual add to ignore GOLD GLOW THE LHFG
        //Adhoc add AWC remove KKP 16-Oct-2019
        //manual add DOHOME, RBF, VGI, TQM, AU  to set100 30-Oct-2019


        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;

            lbMsg.Content = "Open file .xls to see data and Send Keys";
            lbDataInfo.Content = "Data is empty";
            lbStatus.Content = "Last Sent : -";

            txtDelay.Text = refreshInt.ToString();

            gridTable.ItemsSource = stockDataList;
            


            set100Arr = Array.FindAll(set100Arr, x => !set50Arr.Contains(x));

            set50Arr = Array.FindAll(set50Arr, x => x != "X-X");

            Array.Sort(ignoreArr);

            Array.Sort(set50Arr);

            Array.Sort(set100Arr);

            Console.WriteLine(set50Arr);

            AddMarketSymbol();

            //set100Arr.Where(x => set50Arr.Contains(x) );
            CreateIPOList();
            IComparer<StockInfo> sortbyDate = new SortByDate();

            ipoList.Sort(sortbyDate);



        }

        private void CreateIPOList()
        {
            ipoList.Add(new StockInfo("STGT", new DateTime(2020, 7, 2), 48578, "SET", 34 ));
            ipoList.Add(new StockInfo("CRC", new DateTime(2020, 2, 20), 253302, "SET", 42));
            ipoList.Add(new StockInfo("SFLEX", new DateTime(2019, 12, 19), 1590, "SET", 3.88 ));
            ipoList.Add(new StockInfo("BAM", new DateTime(2019, 12, 16), 52760, "SET", 17.5 ));
            ipoList.Add(new StockInfo("ACE", new DateTime(2019, 12, 13),44774, "SET", 4.4));

            ipoList.Add(new StockInfo("CPW", new DateTime(2019, 10, 18), 1428, "SET", 2.38));
            ipoList.Add(new StockInfo("DOHOME", new DateTime(2019, 8, 6), 14478, "SET", 7.8));
            ipoList.Add(new StockInfo("ILM", new DateTime(2019, 7, 26), 11110, "SET", 22));
            ipoList.Add(new StockInfo("RBF", new DateTime(2019, 10, 24), 6600, "SET", 3.3));

            ipoList.Add(new StockInfo("SFLEX", new DateTime(2019, 1, 23), 1590, "SET", 3.88));
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

            ipoList.Add(new StockInfo("SAAM", new DateTime(2020, 1, 7), 540, "mai", 1.8));
            ipoList.Add(new StockInfo("MITSIB", new DateTime(2019, 6, 11), 1667, "mai", 2.5));
            ipoList.Add(new StockInfo("KUN", new DateTime(2019, 12, 17), 660, "mai", 1.1));
            ipoList.Add(new StockInfo("KUMWEL", new DateTime(2019, 8, 1), 473, "mai", 1));

            ipoList.Add(new StockInfo("IP", new DateTime(2019, 11, 5), 1442, "mai", 7));
            ipoList.Add(new StockInfo("INSET", new DateTime(2019, 10, 8), 1506, "mai", 2.69));
            ipoList.Add(new StockInfo("IMH", new DateTime(2019, 12, 26), 1290, "mai", 6));
            ipoList.Add(new StockInfo("GSC", new DateTime(2019, 3, 13), 425, "mai", 1.7));

            ipoList.Add(new StockInfo("CAZ", new DateTime(2019, 1, 2), 1092, "mai", 3.9));
            ipoList.Add(new StockInfo("BC", new DateTime(2019, 11, 14), 1450, "mai", 2.86));
            //ipoList.Add(new StockInfo("ARIN", new DateTime(2019, 7, 10), 1080, "mai", 1.8));
            //ipoList.Add(new StockInfo("APP", new DateTime(2019, 3, 13), 425, "mai", 1.7));


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
            stockDataListS100.Clear();
            stockDataListS50.Clear();
            stockDataListExcludeS100.Clear();
            stockDataListWar.Clear();
            stockDataListS50DW.Clear();
            stockDataListAllDW.Clear();

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


            curStockDataList = stockDataList;
            gridTable.ItemsSource = curStockDataList;
            mode = "All";

            SetUIAfterRefreshStockList(curStockDataList);

            CheckWhatIsMissing();


        }

        private void ProcessTextLine(string[] lines, char[] charSeparators, int i)
        {
            string line = lines[i];
            StockInfo s = new StockInfo();
            string[] token = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            token[0] = token[0].Trim();
            token[1] = token[1].Trim();
            token[2] = token[2].Trim();
            if ( !Regex.IsMatch(token[0], @"-F$")) //!Regex.IsMatch(token[0], @"\d\d\d") &&
            {
                //Array.BinarySearch(ignoreArr, token[0]) < 0
                if ( !ignoreArr.Contains(token[0]) && !Regex.IsMatch(token[0], @"IF$"))
                {

                    s.StockName = token[0];
                    s.ChangePercent = decimal.TryParse(token[1], out decimal tmpresult) ? tmpresult : 0;
                    s.ClosedPrice = decimal.TryParse(token[2], out tmpresult) ? tmpresult : 0;

                    stockDataList.Add(s);

                    //if (token[0] == "TTW")
                    //    Console.WriteLine("Find Advance: " + token[0]);

                    //remove search by binarySearch coz array is not sorted 
                    //int inList100 = Array.BinarySearch(set100Arr, token[0]);
                    //int inList50 = Array.BinarySearch(set50Arr, token[0]);

                    if (set100Arr.Contains(token[0])) stockDataListS100.Add(s);
                    else if (set50Arr.Contains(token[0])) stockDataListS50.Add(s);
                    else if (Regex.IsMatch(token[0], @"-W")) stockDataListWar.Add(s);
                    else if (Regex.IsMatch(token[0], @"^S50")) stockDataListS50DW.Add(s);
                    else if (Regex.IsMatch(token[0], @"\d\d\d")) stockDataListAllDW.Add(s);
                    else stockDataListExcludeS100.Add(s);


                }
                else
                {
                    //Console.WriteLine("Rejectd not to include in ALL_LIST: " + token[0]);
                }

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

        private void AddMarketSymbol()
        {
            StockInfo SET = new StockInfo
            {
                StockName = "SET",
                ChangePercent = 1600,
                ClosedPrice = 1600
            };

            stockDataListMarket.Add(SET);

            StockInfo SET50 = new StockInfo
            {
                StockName = "SET50",
                ChangePercent = 1050,
                ClosedPrice = 1050
            };

            stockDataListMarket.Add(SET50);

            StockInfo tfex = CreateTfexStockInfo();
            stockDataListMarket.Add(tfex);



        }

        private StockInfo CreateTfexStockInfo()
        {
            string symbolTfex = TfexSeriesCode;
            StockInfo tfex = new StockInfo
            {
                StockName = symbolTfex,
                ChangePercent = 1000,
                ClosedPrice = 1000
            };
            return tfex;

        }

        private void SetUIAfterRefreshStockList(List<StockInfo> curStockDataList)
        {
            //string mode = (curStockDataList == stockDataList) ? "All" : (curStockDataList == stockDataListS100) ? "Set 100" : (curStockDataList == stockDataListS50) ? "Set 50"  : (curStockDataList == stockDataListExcludeS100) ? "Exc Set 100" : (curStockDataList == stockDataListS50DW ) ? "S50DW" : (curStockDataList == stockDataListAllDW) ? "AllDW" : "Warrant";
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
            refreshInt = int.TryParse(txtDelay.Text, out int tmpresult) ? tmpresult : 5;
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
    }
}
