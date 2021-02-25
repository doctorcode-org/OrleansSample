using Orleans.Statistics;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SiloHost
{
    //https://gunnarpeipman.com/dotnet-core-system-memory/
    public class CrossPlatformHostEnvironmentStatistics : IHostEnvironmentStatistics
    {
        public long? TotalPhysicalMemory => 0;

        public float? CpuUsage => 0;

        public long? AvailableMemory => 0;

        private bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isUnix;
        }

        //private void GetCpuUsage()
        //{
        //    var cpuCounter = new PerformanceCounter(
        //        categoryName: "Processor",
        //        counterName: "% Processor Time",
        //        instanceName: "_Total",
        //        readOnly: true);

        //    foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
        //    {
        //        cores = cores + int.Parse(item["NumberOfCores"].ToString());
        //    }

        //}
    }
}
