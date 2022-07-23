using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;

namespace System.Printing;

public class Printer : IEquatable<Printer>
{
    // these are the defaults for a 3x5 label using the Seagull driver
    public const int DefaultPrintWidth = 800;
    public const int DefaultPrintHeight = 1200;
    public const int DefaultPrintTop = 10;
    public const int DefaultPrintLeft = 10;

    // used to track if we need to send mode to the printer - we only send changes once
    private PrintMode LastModeSent { get; set; } = PrintMode.Unknown;

    public int PrintTop { get; set; } = DefaultPrintTop;
    public int PrintLeft { get; set; } = DefaultPrintLeft;
    public int PrintWidth { get; set; } = DefaultPrintWidth;
    public int PrintHeight { get; set; } = DefaultPrintHeight;

    public string Name { get; private set; }
    public string PortName { get; private set; }
    public string Driver { get; private set; }
    public PrintMode PrintMode { get; private set; }
    public string IPAddress { get; private set; }
    public int NetworkPort { get; private set; }

    public string CacheFolderPath { get; set; }

    public Printer(ManagementObject printerDescriptor)
    {
        Name = printerDescriptor.Properties["Name"].Value.ToString();
        PortName = printerDescriptor.Properties["PortName"].Value.ToString();
        Driver = printerDescriptor.Properties["DriverName"].Value.ToString();
        DPI = new Point(int.Parse(printerDescriptor.Properties["HorizontalResolution"].Value.ToString()),
            int.Parse(printerDescriptor.Properties["VerticalResolution"].Value.ToString()));
        var searcher2 = new ManagementObjectSearcher($"SELECT * FROM Win32_TCPIPPrinterPort where Name LIKE '{PortName}'");
        var results2 = searcher2.Get();

        foreach (var printer2 in results2)
        {
            IPAddress = printer2.Properties["HostAddress"].Value.ToString();
            if (int.TryParse(printer2.Properties["PortNumber"].Value.ToString(), out int p))
            {
                NetworkPort = p;
            }
        }

        if (string.IsNullOrEmpty(IPAddress))
        {
            // couldn't get it through WMI.  Going to have to go another route
            // Thanks, Zebra!
            // HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Print\Monitors\ZDesigner Port Monitor\Ports
            var key = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Control\\Print\\Monitors\\ZDesigner Port Monitor\\Ports\\{PortName}");
            if (key != null)
            {
                IPAddress = key.GetValue("IPAddress", null).ToString();
                NetworkPort = (int)key.GetValue("PortNumber", 0);
                key.Close();
            }
        }


        CacheFolderPath = Path.GetTempPath();
//            CacheFolderPath = "C:\\temp";

        // TODO: query this from the printer?
        PrintMode = PrintMode.Unknown;
    }

    public Point DPI { get; set; }

    public PrintJob CreateJob(string jobName = null)
    {
        var job = new PrintJobTallComponents(this, jobName);

        return job;
    }

    public void SendRawData(string data)
    {
        RawPrinterHelper.SendStringToPrinter(this.Name, data);
    }
    public void SendRawData(FileInfo file)
    {
        var data = File.ReadAllText(file.FullName);
        SendRawData(data);
    }

    public Printer SetPrintMode(PrintMode mode)
    {
        string command = null;

        if (LastModeSent != mode)
        {
            switch (mode)
            {
                case PrintMode.TearOff:        // T
                    command = string.Format(ZplConstants.PRINT_MODE_FMT, "T");
                    break;
                case PrintMode.PeelOff:        // P
                    command = string.Format(ZplConstants.PRINT_MODE_FMT, "P");
                    break;
                case PrintMode.Rewind:         // R
                    command = string.Format(ZplConstants.PRINT_MODE_FMT, "R");
                    break;
                case PrintMode.DelayedCut:     // D
                    command = string.Format(ZplConstants.PRINT_MODE_FMT, "D");
                    break;
                case PrintMode.Cut:            // C
                    command = string.Format(ZplConstants.PRINT_MODE_FMT, "C");
                    break;
            }

            LastModeSent = PrintMode;
        }

        if (!string.IsNullOrEmpty(command))
        {
            RawPrinterHelper.SendStringToPrinter(this.Name, command);
        }

        PrintMode = mode;


        return this;
    }

    public class PrinterStatus
    {
        internal PrinterStatus(bool isPaperOut, bool isPaused, int formatsInReceiveBuffer)
        {
            IsPaperOut = isPaperOut;
            IsPaused = isPaused;
            FormatsInReceiveBuffer = formatsInReceiveBuffer;
        }

        public bool IsPaperOut { get; private set; }
        public bool IsPaused { get; private set; }
        public int FormatsInReceiveBuffer { get; private set; }
    }

    public PrinterStatus GetStatusDirect()
    {
        var cmd = GetSgdGetCommand("device.host_status");
        var buffer = Encoding.GetEncoding(850).GetBytes(cmd);
        var status = SendAndReceiveToPrinter(IPAddress, NetworkPort, buffer, 0, buffer.Length);

        if (!string.IsNullOrEmpty(status))
        {
            status = status.Trim('\"');
            var lines = status.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            var tokens = lines[0].Split(',');

            var paperOut = tokens[1] == "1";
            var paused = tokens[2] == "1";
            int.TryParse(tokens[4], out int formatsInReceiveBuffer);
            return new PrinterStatus(paperOut, paused, formatsInReceiveBuffer);
        }
        Debug.WriteLine("Failed to get printer status");
        return null;
    }

    private string GetSgdGetCommand(string command, string parameter = "")
    {
        return $"! U1 getvar \"{command}\" \"{parameter}\"\r\n";
    }

    private string SendAndReceiveToPrinter(string hostname, int port, byte[] buffer, int offset, int count)
    {
        var response = new byte[4096];

        using (var tcpClient = new TcpClient(hostname, port))
        using (var networkStream = tcpClient.GetStream())
        {

            networkStream.Write(buffer, offset, count);
            var i = 0;

            // give the printer time to generate a response
            Thread.Sleep(250);

            while (networkStream.DataAvailable)
            {
                var b = networkStream.ReadByte();
                if (b == -1) break;
                response[i++] = (byte)b;
            }

            return Encoding.GetEncoding(850).GetString(response, 0, i);
        }
    }

    internal NativePrintJob[] GetNativePrintJobs()
    {
        var jobs = PrintManager.GetCurrentPrintJobsForPrinter(this.Driver);
        return jobs;
    }

    public bool Equals(Printer other)
    {
        if (other == null) return false;
        if (this.PortName == other.PortName) return true;
        return false;
    }
}