using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace ServiceStatus.WPF
{
    public class Services
    {
        String file;
        private List<ServiceCell> services = new List<ServiceCell>();
        //ArrayList services;
        public List<ServiceCell> Items { get { return services; } } 
        //public ArrayList Items { get { return services; } }

        public Services(String filename)
        {
            file = filename;
        //    services = new ArrayList();

            //Read();
        }


        public ArrayList Servers
        {
            get
            {
                ArrayList servers = new ArrayList();
                foreach (ServiceCell item in services)
                {
                    if (servers.Contains(item.serverName) == false)
                        servers.Add(item.serverName);
                }
                return servers;
            }
        }

     //   public string Server(int idx)
     //   {
        //    foreach (ServiceItem item in services)
        //    {
          //      if (item.ID == idx)
          //          return item.Server;
          //  }
        //
          //  return null;
       // }

        public bool Remove(string server, string service)
        {
            for (int i = 0; i < services.Count; i++)
            {
                ServiceCell item = services[i];
                if (item.serverName == server && item.serviceName == service)
                {
                    services.Remove(item);
                    return true;
                }
            }
            return false;
        }

        public bool Write()///List<ServiceCell> services)
        {
            TextWriter tw = null;

            try
            {
                tw = new StreamWriter(file);

                foreach ( ServiceCell item in services)
                //foreach (ServiceItem item in services)
                {
                    tw.WriteLine(item.serverName + ", " + item.serviceName);
                }
                tw.Close();
                return true;
            }
            catch
            {
                if (tw != null)
                    tw.Close();

                return false;
            }
        }

        public bool Read()
        {
            //services = new ArrayList();
            TextReader tr = null;
            bool allRead = true;

            try
            {
                tr = new StreamReader(file);
                String line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    String[] options = line.Split(',');

                    if (options.Length == 1)
                        ;
                 //       services.Add(new ServiceItem(options[0].Trim(), "", services.Count + 1));
                    else if (options.Length == 2)
                    {
                        ServiceCell cell = new ServiceCell();
                        cell.serverName = options[0].ToUpper().Trim();
                        if (cell.serverName == "")
                            cell.serverName = "???";
                        cell.serviceName = options[1].Trim();
                        if (cell.serviceName == "")
                            cell.serviceName = "???";
                        services.Add(cell);
                        //       services.Add(new ServiceItem(options[0].Trim(), options[1].Trim(), services.Count + 1));
                    }
                    else
                    {
                        allRead = false;
                        continue;
                    }
                }
            }
            catch
            {
                allRead = false;
            }
            finally
            {
                tr.Close();
                services = services.OrderBy(e => e.serverName).ThenBy(e => e.serviceName).ToList();
              //  services.Sort();
            }

            return allRead;
        }
    }

        public class ServiceCell
        {
            public String serviceName;
            public String serverName;
        //    public Label label;
            public ServiceHelper.ServiceStatus status;
            public Button button;
        }

}
