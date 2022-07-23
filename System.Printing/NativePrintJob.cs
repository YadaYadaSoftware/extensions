using System.Management;

namespace System.Printing;

public class NativePrintJob
{
    public string DriverName { get; private set; }
    public DateTime TimeSubmitted { get; private set; }
    public int ID { get; private set; }
    public int Size { get; private set; }
    public int Pages { get; private set; }

    public NativePrintJob(ManagementObject job)
    {
        DriverName = job.Properties["DriverName"].Value.ToString();
        TimeSubmitted = ParseWMIDateTime(job.Properties["TimeSubmitted"].Value.ToString());
        ID = Convert.ToInt32(job.Properties["JobId"].Value);
        Size = Convert.ToInt32(job.Properties["Size"].Value);
        Pages = Convert.ToInt32(job.Properties["TotalPages"].Value);

    }

    private DateTime ParseWMIDateTime(string wmiDateTime)
    {
        //                                       20180822143730.012000-300
        return DateTime.ParseExact(wmiDateTime.Substring(0, 21), "yyyyMMddHHmmss.ffffff", null);
    }
}