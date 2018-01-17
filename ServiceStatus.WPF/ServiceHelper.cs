using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Windows.Forms;

namespace ServiceStatus.WPF
{
    public class ServiceHelper
    {
        public enum ServiceStatus { Unknown, Running, Stopped, Transitional };

        public static bool StartService(string server, string serviceName, int timeoutMilliseconds)
        {
            bool status = true;

            ServiceController service = new ServiceController(serviceName, server);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

//                if (service.CanStop)
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                status = false;
                //  MessageBox.Show(ex.Message);
            }
            return status;
        }

        public static bool StopService(string server, string serviceName, int timeoutMilliseconds)
        {
            bool status = true;

            ServiceController service = new ServiceController(serviceName, server);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                status = false;
                //  MessageBox.Show(ex.Message);
            }
            return status;
        }

        public static ServiceStatus Status(string serviceName, string server)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName, server);

                if (sc.Status == ServiceControllerStatus.Running)
                    return ServiceStatus.Running;

                if (sc.Status == ServiceControllerStatus.Stopped)
                    return ServiceStatus.Stopped;

                return ServiceStatus.Transitional;
            }
            catch
            {
                return ServiceStatus.Unknown;
            }
        }
    }
}
