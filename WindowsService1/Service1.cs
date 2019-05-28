using Cjwdev.WindowsApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private System.Threading.Timer timer = null;

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            timer = new System.Threading.Timer(Timer, null, 0, 1000);
        }

        protected override void OnStop()
        {

        }

        private void Timer(object o)
        {
            try
            {
                DoWork();
            }
            catch (Exception ex)
            {
                Write(DateTime.Now.ToString() + ":" + ex.Message);
            }
        }

        private void DoWork()
        {
            //检查进程，杀死，并提示返回
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                //Console.WriteLine("Process Name: {0}, Responding: {1}", process.ProcessName, process.Responding);
                if(process.ProcessName == "MultiCharts64")
                {
                    Process[] p = Process.GetProcessesByName("MultiCharts64");
                    p[0].Kill();

                    AppStart(@"D:\tws\logexe\WindowsFormsAppMessage.exe");

                    Write("The tws is not legitimate landing,please update tws Exe.lof and excel.dde to latest version,and update multicharts tcp_ip connect" +
                        "the 0x_00040340RAM is crash,please reload again!");
                    Write(DateTime.Now.ToString() + ":" + "MultiCharts64 update");

                }

                if (process.ProcessName == "tws")
                {
                    Process[] p = Process.GetProcessesByName("tws");
                    p[0].Kill();

                    AppStart(@"D:\tws\logexe\WindowsFormsAppMessage.exe");
                    
                    Write("The tws is not legitimate landing,please update tws Exe.lof and excel.dde to latest version,and update multicharts tcp_ip connect" +
                        "the 0x_00040340RAM is crash,please reload again!");
                    Write(DateTime.Now.ToString() + ":" + "Trader Workstation");

                }
            }
        }

        public void AppStart(string appPath)
        {
            try
            {

                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);

                ApiDefinitions.CreateProcessAsUser(
                    userTokenHandle,
                    appStartPath,
                    "",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startInfo,
                    out procInfo);

                if (userTokenHandle != IntPtr.Zero)
                    ApiDefinitions.CloseHandle(userTokenHandle);

                int _currentAquariusProcessId = (int)procInfo.dwProcessId;
            }
            catch (Exception ex)
            {
            }
        }

        private void Write(string str)
        {

            using (StreamWriter sw = new StreamWriter("D:\\lof.txt", true))
            {
                sw.WriteLine(str);
            }
        }
    }
}
