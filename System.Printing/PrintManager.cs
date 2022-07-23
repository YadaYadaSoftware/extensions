using System.Diagnostics;
using System.Management;

namespace System.Printing;

public static class PrintManager
{
    private static List<Printer> m_knownList = new List<Printer>();

    private const string ZebraTokensCommaDelimitedList = "ZDesigner,Zebra";

    public static Printer[] GetPrinters()
    {
        return GetPrinters(ZebraTokensCommaDelimitedList.Split(','));
    }

    public static Printer[] GetPrinters(params string[] tokens)
    {
        var list = new List<Printer>();

        string query = "SELECT * from Win32_Printer";

        using (ManagementObjectSearcher searcher = new(query))
        using (ManagementObjectCollection coll = searcher.Get())
        {
            foreach (ManagementObject printer in coll)
            {
                Debug.WriteLine(printer.Properties["DriverName"].Value);
                Debug.WriteLine(printer.Properties["Name"].Value);
                Debug.WriteLine(printer.Properties["PortName"].Value);

                if (tokens.Any(t => printer.Properties["DriverName"].Value.ToString().IndexOf(t, StringComparison.InvariantCultureIgnoreCase) >= 0))
                {
                    list.Add(new Printer(printer));
                }
            }
        }

        m_knownList = list;
        return m_knownList.ToArray();
    }

    internal static NativePrintJob[] GetCurrentPrintJobsForPrinter(string driverName)
    {
        var list = new List<NativePrintJob>();

        string query = string.Format("SELECT * from Win32_PrintJob WHERE DriverName = '{0}'", driverName);

        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
        using (ManagementObjectCollection coll = searcher.Get())
        {
            foreach (ManagementObject job in coll)
            {
                list.Add(new NativePrintJob(job));
            }
        }

        return list.ToArray();
    }

    internal static NativePrintJob GetNativePrintJobByID(int jobID)
    {
        var list = new List<NativePrintJob>();

        string query = string.Format("SELECT * from Win32_PrintJob WHERE JobID = {0}", jobID);

        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
        using (ManagementObjectCollection coll = searcher.Get())
        {
            foreach (ManagementObject job in coll)
            {
                var j = new NativePrintJob(job);
                return j;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a known printer by caseless matching either the printer or the printer port name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A printer object, if one can be found, otherwise null</returns>
    public static Printer GetPrinter(string name)
    {
        return GetPrinter(name, true);
    }

    private static Printer GetPrinter(string name, bool refreshIfNotFound)
    {
        var existing = m_knownList.FirstOrDefault(p => string.Compare(p.Name, name, true) == 0 || string.Compare(p.PortName, name, true) == 0);
        if (existing == null && refreshIfNotFound)
        {
            GetPrinters();
            return GetPrinter(name, false);
        }
        return existing;
    }
}