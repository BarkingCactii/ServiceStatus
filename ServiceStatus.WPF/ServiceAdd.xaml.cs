using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ServiceProcess;

namespace ServiceStatus.WPF
{
    /// <summary>
    /// Interaction logic for ServiceAdd.xaml
    /// </summary>
    public partial class ServiceAdd : Window
    {
        private Services _services;
        private String server;

        private ObservableCollection<ListViewBindingItem> items = new ObservableCollection<ListViewBindingItem>();

        public ObservableCollection<ListViewBindingItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        public String Server { get { return server;  } }

        public ServiceAdd(Services s)
        {
            _services = s;
            InitializeComponent();

            /*
            GridView view = new GridView();

            GridViewColumn col1 = new GridViewColumn();
            col1.Header = "Name";
            col1.DisplayMemberBinding = new Binding("Name");
            view.Columns.Add(col1);

            GridViewColumn col2 = new GridViewColumn();
            col2.Header = "Can Stop";
            col2.DisplayMemberBinding = new Binding("CanStop");
            view.Columns.Add(col2);

            GridViewColumn col3 = new GridViewColumn();
            col3.Header = "Can Stop";
            col3.DisplayMemberBinding = new Binding("CanStop");
            view.Columns.Add(col3);
            
            listServices.View = view;
            */
            // comboServers.Items.Add("localhost");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         //   foreach (string s in _services.Servers)
          //  {
          //      comboServers.Items.Add(s);
          //  }
            listServices.ItemsSource = items;
            //listServices.DataContext = items;
        }

        
        /*
        private void btnGetServices_Click(object sender, EventArgs e)
        {
            try
            {
                listServices.Items.Clear();

                ServiceController[] services = ServiceController.GetServices(comboServers.Text);

                services = services.OrderBy(c => c.DisplayName).ToArray();

                foreach (ServiceController service in services)
                {
                    Items.Add(new ListViewBindingItem(service.DisplayName, service.CanStop));
//                    listServices.Items.Add(new ListViewBindingItem(service.DisplayName, service.CanStop));
                }
                //listServices.Items;
                //listServices.Select();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
         */

        public class ListViewBindingItem
        {
            public ListViewBindingItem(string _name, bool _canStop)
            {
                name = _name;
                canStop = _canStop;
                ischecked = false;
                // ischecked = new CheckBox();
                // ischecked.IsChecked = false;
            }

            bool ischecked;
            public bool Checked
            {
                get { return ischecked; }
                set { ischecked = value; }
            }

            string name;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            bool canStop;
            public bool CanStop
            {
                get { return canStop; }
                set { canStop = value; }
            }
        }

        private void comboServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            server = (String)comboServers.SelectedValue;
        }

        private void btnGetServices_Click(object sender, RoutedEventArgs e)
        {
            if (comboServers.Text == String.Empty)
                return;

            // new server entered by user
            if (comboServers.SelectedIndex == -1)
            {
                if (comboServers.Items.Contains(comboServers.Text) == false)
                {
                    // add it to the combolist if it doesn't exist
                    comboServers.Items.Add(comboServers.Text);
                    comboServers.SelectedIndex = comboServers.Items.Count - 1;
                }
            }

            try
            {
                //listServices.Items.Clear();

                ServiceController[] services = ServiceController.GetServices(comboServers.Text);

                services = services.OrderBy(c => c.DisplayName).ToArray();

                Items = new ObservableCollection<ListViewBindingItem>();
                foreach (ServiceController service in services)
                {
                    bool check = false;

                    foreach ( ServiceCell si in _services.Items)//...Servers)
                    {
                        if (si.serverName == service.MachineName && si.serviceName == service.DisplayName)//ServiceName)
                        {
                            check = true;
                            //items[1].Checked = true;
                            break;
                        }
                    }
                    Items.Add(new ListViewBindingItem(service.DisplayName, service.CanStop));
                    items[items.Count - 1].Checked = check;
                    //listServices.Items.Add(new ListViewBindingItem(service.DisplayName, service.CanStop));
                }

                listServices.ItemsSource = Items;

                //listServices.Select();
                /*
                                for (int i = 0; i < listServices.Items.Count; i++)
                                {
                                    foreach (ServiceItem item in this.services.Items)
                                    {
                                        if (item.Server.ToLower() == comboServers.Text.ToLower())
                                        {
                                            if (item.Service.ToLower() == listServices.Items[i].Text.ToLower())
                                            {
                                                listServices.Items[i].Checked = true;
                                            }
                                        }
                                    }
                                }
                 */
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            GridView view  = (GridView)listServices.View;

      //      ObservableCollection<ListViewBindingItem> items2 = (ObservableCollection<ListViewBindingItem>)listServices.ItemsSource;
       //     foreach (ListViewBindingItem item in items2 )//listServices.Items)
       //     {
        //        if (item.Checked)
         //           MessageBox.Show("Checked " + item.Name);
                    
        //        String name = item.Name;
         //   }
            Close();
            //view.Columns[0];
            for (int i = 0; i < listServices.Items.Count; i++)
            {
//                ListViewItem item = listServices.Items[i];
             //   ListViewBindingItem item = (ListViewBindingItem)listServices.Items[i];
            //    if (item.Checked.IsChecked == true)
            //    {
             //       MessageBox.Show("Checked " + item.Name);
              //  }
            }
        }

        private void btnAddServer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
