using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;

namespace ServiceStatus.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Services s = null;// = new Services();
        private ObservableCollection<ServiceAdd.ListViewBindingItem> items = new ObservableCollection<ServiceAdd.ListViewBindingItem>();
        private bool _terminate = false;
     //   private List<ServiceCell> cells = new List<ServiceCell>();

        public MainWindow()
        {
            InitializeComponent();

            stackPanel.Children.Clear();

            try
            {
                s = new Services("servicestatus.ini");
                s.Read();

                Dictionary<String, GroupBox> boxes = new Dictionary<string, GroupBox>();

                // create a dictionary of group boxes, each containing a stackpanel
                //
                foreach (ServiceCell cell in s.Items)
                {
                    String server = cell.serverName.ToUpper();
                    if (boxes.ContainsKey(server) == false)
                    {
                        boxes[server] = CreateGroupBox(server);
                        boxes[server].Content = new StackPanel();
                    }

                    StackPanel sp = (StackPanel)boxes[server].Content;
                    AddButton(cell);
                    sp.Children.Add(cell.button);
                }

                // add the dictionary to the master stackpanel
                //
                foreach (GroupBox gb in boxes.Values)
                {
                    stackPanel.Children.Add(gb);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Service(String server, String service)
        {
            /*
            // PerformanceCounter myAppCpu =new PerformanceCounter ( "Process", "% Processor Time", "sqlserver", server);

            ServiceHelper.ServiceStatus sc = ServiceHelper.Status(service, server);

            ServiceItem si = s.GetInfo(server, service);
            if (si == null)
                return;

            Control[] ctrls = Controls.Find(si.StatusField, true);

            if (ctrls.Length == 1)
            {
                LabelGradient.LabelGradient lbl = (LabelGradient.LabelGradient)ctrls[0];

                if (sc == ServiceHelper.ServiceStatus.Stopped)
                {
                    lbl.GradientColorOne = System.Drawing.Color.LightSalmon;
                    lbl.GradientColorTwo = System.Drawing.Color.Red;

                    // lbl.BackColor = Color.Tomato;
                    int startStatus = ReadRegistry(server, service);
                    if (startStatus == -1)
                        lbl.Text = "Unknown";
                    if (startStatus == 2)
                        lbl.Text = "Automatic";
                    if (startStatus == 3)
                        lbl.Text = "Manual";
                    if (startStatus == 4)
                        lbl.Text = "Disabled";

                }
                else
                    if (sc == ServiceHelper.ServiceStatus.Running)
                    {
                        lbl.GradientColorOne = System.Drawing.Color.Lime;
                        lbl.GradientColorTwo = System.Drawing.Color.SeaGreen;
                        lbl.Text = sc.ToString();
                        //   lbl.Text = myAppCpu.NextValue().ToString();
                    }
                    else
                        if (sc == ServiceHelper.ServiceStatus.Transitional)
                        {
                            lbl.GradientColorOne = System.Drawing.Color.Bisque;
                            lbl.GradientColorTwo = System.Drawing.Color.DarkOrange;
                            lbl.Text = sc.ToString();
                        }
                        else
                        {

                            lbl.GradientColorOne = System.Drawing.Color.White;//.Lime;
                            lbl.GradientColorTwo = System.Drawing.Color.LightGray; // SeaGreen;
                            lbl.Text = "???";
                        }
            }
             */
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         //   stackPanel.Children.Clear();
        }

        private void AddButton(ServiceCell cell)
        {
            cell.button = new Button();
            cell.button.Style = Resources["StartedService2"] as Style;
            //  cell.button.Content = item.Name;
            cell.button.Height = 40;
            cell.button.MinWidth = 235;
            cell.button.Width = 235;
            cell.button.MouseDoubleClick += DoubleClick_Cell;
            LinearGradientBrush grad2 = new LinearGradientBrush(Colors.DarkSlateGray, Colors.DarkSlateGray, 0);//Colors.Yellow, 90);
            cell.button.Background = grad2;
            cell.button.DataContext = cell;
            StackPanel sp = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Text = cell.serviceName;//service;//item.Name;
            tb.FontSize = 12;
            tb.HorizontalAlignment = HorizontalAlignment.Right;

            SolidColorBrush fore = new SolidColorBrush(Colors.Gainsboro);

            tb.Foreground = fore;
            TextBlock tb2 = new TextBlock();
            tb2.Text = cell.serverName;//server;//add.Server;
            tb2.FontSize = 10;
            tb2.Foreground = fore;
            tb.HorizontalAlignment = HorizontalAlignment.Right;

            sp.Children.Add(tb);
            sp.Children.Add(tb2);
            cell.button.Content = sp;

            //    parent.Children.Add(cell.button);

            //cell.serverName = server;//add.Server;
            //cell.serviceName = service;//item.Name;

            Thread thread = new Thread(() => Monitor(cell));//lbl, item.Name, add.Server));
            thread.Start();

          //  return cell;            
        }

        private void RemoveButton(ServiceCell cell)
        {
            foreach (GroupBox gb in stackPanel.Children)
            {
                //GroupBox gb = (GroupBox)sp.Children[0];

                StackPanel sp2 = (StackPanel)gb.Content;
                for ( int i = 0; i < sp2.Children.Count; i++)
                //foreach (Control c in sp2.Children)
                {
                    Control c = (Control)sp2.Children[i];
                    if (c.GetType() == typeof (Button))
                    {
                        Button b = (Button)c;
                        if (cell.button == b)
                        {
                            sp2.Children.Remove(b);
                            i--;
                        }
                    }
                }
            }
        }

        private void RemoveCell( String server, String service)
        {
            for ( int i = 0; i < s.Items.Count; i++)
            //foreach (ServiceCell cell in s.Items)
            {
                ServiceCell cell = s.Items[i];

                if (cell.serverName == server && cell.serviceName == service)
                {
                    RemoveButton(cell);
                    s.Items.Remove(cell);
                    i--;
                }
            }
        }

        private ServiceCell AddCell( String server, String service)//ServiceAdd.ListViewBindingItem item)
        {
            foreach (ServiceCell cell in s.Items)
            {
                //ServiceCell cell = s.Items[i];

                if (cell.serverName == server && cell.serviceName == service)
                {
                    // already exists
                    return null;
                }
            }

            ServiceCell newCell = new ServiceCell();
            newCell.serverName = server;
            newCell.serviceName = service;
            AddButton(newCell);

        //    cells.Add(cell);

/*            cell.button = new Button();
            cell.button.Style = Resources["StartedService2"] as Style;
            //  cell.button.Content = item.Name;
            cell.button.Height = 40;
            cell.button.MinWidth = 235;
            cell.button.Width = 235;
            cell.button.MouseDoubleClick += DoubleClick_Cell;
            LinearGradientBrush grad2 = new LinearGradientBrush(Colors.DarkSlateGray, Colors.DarkSlateGray, 0);//Colors.Yellow, 90);
            cell.button.Background = grad2;
            cell.button.DataContext = cell;
            StackPanel sp = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Text = service;//item.Name;
            tb.FontSize = 12;
            tb.HorizontalAlignment = HorizontalAlignment.Right;

            SolidColorBrush fore = new SolidColorBrush(Colors.Gainsboro);

            tb.Foreground = fore;
            TextBlock tb2 = new TextBlock();
            tb2.Text = server;//add.Server;
            tb2.FontSize = 10;
            tb2.Foreground = fore;
            tb.HorizontalAlignment = HorizontalAlignment.Right;

            sp.Children.Add(tb);
            sp.Children.Add(tb2);
            cell.button.Content = sp;

        //    parent.Children.Add(cell.button);

            cell.serverName = server;//add.Server;
            cell.serviceName = service;//item.Name;

            Thread thread = new Thread(() => Monitor(cell));//lbl, item.Name, add.Server));
            thread.Start();
            */
            return newCell;
        }

        private void DoubleClick_Cell(object sender, RoutedEventArgs e)
        {
            Button lbl = (Button)sender;
            ServiceCell cell = (ServiceCell)lbl.DataContext;

            if (cell.status == ServiceHelper.ServiceStatus.Running)
            {
                MessageBoxResult result = MessageBox.Show("Please confirm stopping " + Environment.NewLine + cell.serviceName + " on " + cell.serverName, "Warning", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Thread thread = new Thread(() => Stop(cell));
                    thread.Start();
                }
            }

            if (cell.status == ServiceHelper.ServiceStatus.Stopped)
            {
                MessageBoxResult result = MessageBox.Show("Please confirm starting " + Environment.NewLine + cell.serviceName + " on " + cell.serverName, "Warning", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Thread thread = new Thread(() => Start(cell));
                    thread.Start();
                }
            }
        }

        private void Start(ServiceCell cell) //Label lbl, String serviceName, String serverName)
        {
            if ( ServiceHelper.StartService(cell.serverName, cell.serviceName, 10000) == false )
                cell.status = ServiceHelper.ServiceStatus.Unknown;
            
        }

        private void Stop(ServiceCell cell) //Label lbl, String serviceName, String serverName)
        {
            if ( ServiceHelper.StopService(cell.serverName, cell.serviceName, 10000) == false)
                cell.status = ServiceHelper.ServiceStatus.Unknown;
        }

        private void Monitor(ServiceCell cell)//Label lbl, String serviceName, String serverName)
        {
            while (_terminate == false)
            {
                cell.status = ServiceHelper.Status(cell.serviceName, cell.serverName);
                if (cell.status == ServiceHelper.ServiceStatus.Running)
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                     //   cell.label.Background = new LinearGradientBrush(Colors.AntiqueWhite, Colors.Green, 90);
//                        cell.button.Background = new LinearGradientBrush(Colors.LightSeaGreen, Colors.Green, 90);
                        cell.button.Style = Resources["StartedService2"] as Style;

                    }));
                if (cell.status == ServiceHelper.ServiceStatus.Stopped)
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                    //    cell.label.Background = new LinearGradientBrush(Colors.AntiqueWhite, Colors.Red, 90);
  //                      cell.button.Background = new LinearGradientBrush(Colors.LightPink, Colors.Red, 90);
                        cell.button.Style = Resources["StoppedService2"] as Style;

                    }));
                if (cell.status == ServiceHelper.ServiceStatus.Transitional)
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                    //    cell.label.Background = new LinearGradientBrush(Colors.AntiqueWhite, Colors.Orange, 90);
//                        cell.button.Background = new LinearGradientBrush(Colors.AntiqueWhite, Colors.Orange, 90);
                        cell.button.Style = Resources["StoppedService2"] as Style;
                    }));
                if (cell.status == ServiceHelper.ServiceStatus.Unknown)
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                     //   cell.label.Background = new LinearGradientBrush(Colors.DarkSlateGray, Colors.DarkSlateGray, 90);
                        cell.button.Style = Resources["UnknownService2"] as Style;
                  //      cell.button.Background = new LinearGradientBrush(Colors.DarkSlateGray, Colors.DarkSlateGray, 90);
                    }));

                Thread.Sleep(10000);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            s.Write();//cells);
            _terminate = true;
        }

        private void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            ServiceAdd add = new ServiceAdd(s);

            // populate combobox
            foreach (String cell in s.Servers) //cells)
            {
                if (!add.comboServers.Items.Contains(cell))
                    add.comboServers.Items.Add(cell);
            }

            add.ShowDialog();

            items = add.Items;

            StackPanel _stackPanel = new StackPanel();
            _stackPanel.HorizontalAlignment = HorizontalAlignment.Left;

            foreach (ServiceAdd.ListViewBindingItem item in items)
            {
                if (item.Checked == false)
                {
                    // check if was previously being monitored
                    RemoveCell(add.Server, item.Name);
                }

                if (item.Checked)
                {
                    ServiceCell cell = AddCell(add.Server, item.Name);

                    if (cell == null)
                        // cell already being monitored
                        continue;

                    _stackPanel.Children.Add(cell.button);
                    s.Items.Add(cell);
                }
            }

            try
            {
                Process[] proc = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension("notepad"), "VSEMPSYSAPP05");
                if (proc.Count() > 0)
                    Debug.WriteLine("proc");
                //String s = proc[0].StartInfo.Arguments.ToString();
            }
            catch ( Exception ex )
            {

            }

            if (_stackPanel.Children.Count > 0)
            {
                GroupBox gb = CreateGroupBox(add.Server);
                gb.Content = _stackPanel;
                stackPanel.Children.Add(gb);
            }
        }

        private GroupBox CreateGroupBox(String name)
        {
            SolidColorBrush gbgrad = new SolidColorBrush(Colors.Transparent);
            GroupBox gb = new GroupBox();
            gb.Width = 250;
            gb.Header = name;
            gb.Foreground = new SolidColorBrush(Colors.White);
            gb.Background = gbgrad;
            gb.BorderThickness = new System.Windows.Thickness(0);
            gb.BorderBrush = gbgrad;
            gb.HorizontalAlignment = HorizontalAlignment.Left;

            return gb;
        }
    }

    public class MyDependencyClass : DependencyObject
    {
        public static readonly DependencyProperty CurrentDurationProperty;

        public static void SetCurrentDuration(DependencyObject DepObject, string value)
        {
            DepObject.SetValue(CurrentDurationProperty, value);
        }

        public static string GetCurrentDuration(DependencyObject DepObject)
        {
            return "This is some text";
            return (string)DepObject.GetValue(CurrentDurationProperty);
        }

        static MyDependencyClass()
        {
            PropertyMetadata MyPropertyMetadata = new PropertyMetadata("Duration: 0m");

            CurrentDurationProperty = DependencyProperty.RegisterAttached("CurrentDuration",
                                                                typeof(string),
                                                                typeof(MyDependencyClass),
                                                                MyPropertyMetadata);
        }
    }
}
