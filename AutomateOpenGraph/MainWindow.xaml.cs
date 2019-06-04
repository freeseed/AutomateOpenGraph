using Microsoft.Win32;
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
        private List<StockInfo> stockDataListS100 = new List<StockInfo>();
        private List<StockInfo> stockDataListExcludeS100 = new List<StockInfo>();
        private List<StockInfo> stockDataListWar = new List<StockInfo>();
        private List<StockInfo> curStockDataList;

        private const int refreshInt = 5;
        // data ignore list as of 16-May-2019
        private string[] ignoreArr = { "AIMIRT", "AMATAR", "B-WORK", "BKKCP", "BOFFICE", "CPNCG", "CPNREIT", "CPTGF", "CRYSTAL", "CTARAF", "DREIT", "ERWPF", "FTREIT", "FUTUREPF", "GAHREIT", "GLANDRT", "GOLDPF", "GVREIT", "HPF", "HREIT", "IMPACT", "KPNPF", "LHHOTEL", "LHPF", "LHSC", "LUXF", "M-II", "M-PAT", "M-STOR", "MIPF", "MIT", "MJLF", "MNIT", "MNIT2", "MNRF", "MONTRI", "POPF", "PPF", "QHHR", "QHOP", "QHPF", "SBPF", "SHREIT", "SIRIP", "SPF", "SPRIME", "SRIPANWA", "SSPF", "SSTPF", "SSTRT", "TIF1", "TLGF", "TLHPF", "TNPF", "TPRIME", "TTLPF", "TU-PF", "URBNPF", "WHABT", "WHART" };
        private string[] set100Arr = { "AAV", "ADVANC", "AEONTS", "AMATA", "ANAN", "AOT", "AP", "BANPU", "BBL", "BCH", "BCP", "BCPG", "BDMS", "BEAUTY", "BEM", "BGRIM", "BH", "BJC", "BLAND", "BPP", "BTS", "CBG", "CENTEL", "CHG", "CK", "CKP", "COM7", "CPALL", "CPF", "CPN", "DELTA", "DTAC", "EA", "EGCO", "EPG", "ERW", "ESSO", "GFPT", "GLOBAL", "GLOW", "GOLD", "GPSC", "GULF", "GUNKUL", "HANA", "HMPRO", "INTUCH", "IRPC", "IVL", "KBANK", "KCE", "KKP", "KTB", "KTC", "LH", "MAJOR", "MBK", "MEGA", "MINT", "MTC", "ORI", "PLANB", "PRM", "PSH", "PSL", "PTG", "PTT", "PTTEP", "PTTGC", "QH", "RATCH", "ROBINS", "RS", "SAWAD", "SCB", "SCC", "SGP", "SIRI", "SPALI", "SPRC", "STA", "STEC", "SUPER", "TASCO", "TCAP", "THAI", "THANI", "TISCO", "TKN", "TMB", "TOA", "TOP", "TPIPP", "TRUE", "TTW", "TU", "TVO", "WHA", "WHAUP", "WORK" };

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

            gridTable.ItemsSource = stockDataList;
            Array.Sort(ignoreArr);
            Array.Sort(set100Arr);
            

        }
        private string TfexSeriesCode
        {
            get
            {

                DateTime testDate = DateTime.Parse($"2000-{DateTime.Now.ToString("MM-dd")}");
                DateTime hDate = DateTime.Parse("2000-01-01");
                DateTime mDate = DateTime.Parse("2000-03-27");
                DateTime uDate = DateTime.Parse("2000-06-27");
                DateTime zDate = DateTime.Parse("2000-09-27");
                string symbolQuater;

                if (testDate >= zDate)
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
                return $"S50{symbolQuater}{DateTime.Now.ToString("yy")}";
            }
        }

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
                    Send_keys(s.StockName);
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
            stockDataListExcludeS100.Clear();
            stockDataListWar.Clear();

            for (int i = 1; i < lines.Length - 1; i++)
                ProcessTextLine(lines, charSeparators, i);

            AddTfexSymbol();

            stockDataList = stockDataList.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListS100 = stockDataListS100.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListExcludeS100 = stockDataListExcludeS100.OrderByDescending(o => o.ChangePercent).ToList();
            stockDataListWar = stockDataListWar.OrderByDescending(o => o.ChangePercent).ToList();


            curStockDataList = stockDataList;
            gridTable.ItemsSource = curStockDataList;

            SetUIAfterRefreshStockList(curStockDataList);




        }

        private void ProcessTextLine(string[] lines, char[] charSeparators, int i)
        {
            string line = lines[i];
            StockInfo s = new StockInfo();
            string[] token = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            token[0] = token[0].Trim();
            token[1] = token[1].Trim();
            token[2] = token[2].Trim();
            if (!Regex.IsMatch(token[0], @"\d\d\d") && !Regex.IsMatch(token[0], @"-F$"))
            {

                if (Array.BinarySearch(ignoreArr, token[0]) < 0 && !Regex.IsMatch(token[0], @"IF$"))
                {

                    s.StockName = token[0];
                    s.ChangePercent = decimal.TryParse(token[1], out decimal tmpresult) ? tmpresult : 0;
                    s.ClosedPrice = decimal.TryParse(token[2], out tmpresult) ? tmpresult : 0;

                    stockDataList.Add(s);
                    
                    int inList = Array.BinarySearch(set100Arr, token[0]);
                    if (inList >= 0) stockDataListS100.Add(s);
                    else if (Regex.IsMatch(token[0], @"-W")) stockDataListWar.Add(s);
                    else stockDataListExcludeS100.Add(s);


                }
                else
                {
                    Console.WriteLine("Rejectd: " + token[0]);
                }

            }

        }

        private void AddTfexSymbol()
        {
            StockInfo tfex = CreateTfexStockInfo();
            if (stockDataList.Count > 0) stockDataList.Add(tfex);
            if (stockDataListS100.Count > 0) stockDataListS100.Add(tfex);
            if (stockDataListExcludeS100.Count > 0) stockDataListExcludeS100.Add(tfex);
            if (stockDataListWar.Count > 0) stockDataListWar.Add(tfex);
            
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
            string mode = (curStockDataList == stockDataList) ? "All" : (curStockDataList == stockDataListS100) ? "Set 100" : (curStockDataList == stockDataListExcludeS100) ? "Exc Set 100" : "Warrant";
            mode = $"[{mode}]";

            int itemCount = curStockDataList.Count;
            lbMsg.Content = itemCount > 0 ? itemCount.ToString() + " records. Next Send Keys" : "File has no record. Please select new file";
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
            SetListToGrid(stockDataList);
        }
        private void Set100Button_Click(object sender, RoutedEventArgs e)
        {
            SetListToGrid(stockDataListS100);
        }
        private void ExcSet100Button_Click(object sender, RoutedEventArgs e)
        {
            SetListToGrid(stockDataListExcludeS100);
        }

        private void WarrantButton_Click(object sender, RoutedEventArgs e)
        {
            SetListToGrid(stockDataListWar);
        }
    }
}
