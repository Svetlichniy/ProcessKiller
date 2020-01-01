using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Threading;

namespace ProcessKiller
{
    public static class ProcessHelper
    {
        public static ToolStripStatusLabel StatusLabel { get; set; }
        public static Form1 _Form { get; set; }
        public static List<string> GetProcesses()
        {
            var ProcessesList = new List<string>();
            var Processes = System.Diagnostics.Process.GetProcesses();

            foreach (var process in Processes.OrderBy(o => o.ToString()))
            {
                try
                {
                    ProcessesList.Add(string.Format("{0}\t{1}", 
                        process.MainModule.ModuleName, "Процесс"));
                }
                catch { }
            }

            return ProcessesList;
        }

        public static List<string> GetTasks()
        {
            var TasksList = new List<string>();
            var Tasks = ServiceController.GetServices();
            foreach (var task in Tasks.Where(e=>e.Status == ServiceControllerStatus.Running).OrderBy(o => o.ToString()))
            {
                TasksList.Add(string.Format("{0}\t{1}", task.DisplayName, "Служба"));
            }

            return TasksList;
        }

        public static void KillProcess(string ModuleName)
        {            
            var Processes = System.Diagnostics.Process.GetProcesses();
            foreach (var proc in Processes)
            {
                try
                {
                    if (proc.MainModule.ModuleName.Equals(ModuleName))
                        proc.Kill();
                }
                catch { }
            }            
        }

        public static void KillTask(string DispalayName)
        {
            var Tasks = ServiceController.GetServices().Where(e => e.Status == ServiceControllerStatus.Running);
            foreach (var task in Tasks)
            {
                try
                {
                    if (task.DisplayName.Equals(DispalayName))
                        task.Stop();
                }
                catch { }
            }
        }

        public static void KillAll(string[] BlackList)
        {
            foreach (var item in BlackList)
            {
                var moduleName = item.Split('\t');
                if (moduleName[1].Equals("Процесс"))
                    KillProcess(moduleName[0]);
                else if (moduleName[1].Equals("Служба"))
                    KillTask(moduleName[0]);
            }
        }
    }
}
