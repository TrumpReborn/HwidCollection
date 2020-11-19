using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace HwidEnvironment
{
    public class HwidCollection
    {
        private ManagementObjectSearcher baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
   

    
        // Processor ID
        public string PID;
        // UUID
        public string UID;
        // BaseBoard ID
        public string BID;
        // Mac Address
        public string Mac;
        // C Drive Serials
        public string CID;
        // D Drive serial
        public string DID;
        // Motherboard Name
        public string MoboName;
        // Monitor Resolution
        public string MonitorSize;

        public bool SpooferDetect = false;

        protected void Constant()
        {
           

            // ProcessorID, it isn't unique | use it to try see if a user is lying to you, dont use it as hwid
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
            mbsList = mos.Get();
            string processorId = string.Empty;
            foreach (ManagementBaseObject mo in mbsList)
            {
                processorId = mo["ProcessorID"] as string;
            }
            PID = processorId;

            // UUID, Is Unique
            mos = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
            mbsList = mos.Get();
            string systemId = string.Empty;
            foreach (ManagementBaseObject mo in mbsList)
            {
                systemId = mo["UUID"] as string;
            }

            UID = systemId;




            //base board serial, Is Unique
            mos = new ManagementObjectSearcher("Select SerialNumber from Win32_BaseBoard");
            mbsList = mos.Get();
            string BaseId = string.Empty;
            foreach (ManagementBaseObject mo in mbsList)
            {
                BaseId = mo["SerialNumber"] as string;
            }
            BID = BaseId;




            // sets values from methods we have made, if this wont work in your application make sure you call the method before trying to get the id
            CID = GetC();
            Mac = GetMACAddress();
            DID = GetD();
            MonitorSize = GetMonitor();
            MoboName = GetMobo;


            if (CID == DID && DID != "0" && DID !="" && DID !=" ")
            {
                SpooferDetect = true;
                // This is a bad check for bad hwid spoofers that change all serials to the same value
                // Dont use this to ban people since raiding drives or something of the sorts may mess it up but it could indicate if someone is possibly using a hwid spoofer
                // also make sure this check is done after initializing the methods.

            }
        }
        // gets macaddress, should be unique
        protected string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            string MACAddress = String.Empty;

            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                { // only return MAC Address from first card
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            return MACAddress;
        }
        // this gets the C drive serial, it is unique
        protected string GetC()
        {

            var logicalDiskId = "C:";
            var deviceId = string.Empty;

            var query = "ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + logicalDiskId + "'} WHERE AssocClass = Win32_LogicalDiskToPartition";
            var queryResults = new ManagementObjectSearcher(query);
            var partitions = queryResults.Get();

            foreach (var partition in partitions)
            {
                query = "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                queryResults = new ManagementObjectSearcher(query);
                var drives = queryResults.Get();

                foreach (var drive in drives)
                    deviceId = drive["SerialNumber"].ToString();
            }
            return deviceId;

        }
        // gets d drive, this is just to do our hwid spoofer check to try catch out people using poorly made hwid spoofers, you could bind hwid to this but it will just be 0 HOPEFULLY
        protected string GetD()
        {
            try
            {
                var logicalDiskId = "D:";
                var deviceId = string.Empty;

                var query = "ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + logicalDiskId + "'} WHERE AssocClass = Win32_LogicalDiskToPartition";
                var queryResults = new ManagementObjectSearcher(query);
                var partitions = queryResults.Get();

                foreach (var partition in partitions)
                {
                    query = "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                    queryResults = new ManagementObjectSearcher(query);
                    var drives = queryResults.Get();

                    foreach (var drive in drives)
                        deviceId = drive["SerialNumber"].ToString();
                }
                return deviceId;
            }
            catch (Exception ex)
            {
                return "0";

            }

        }
        // DONT USE THIS FOR HWID | This and the mobo checks are just ways to try and see if someone is telling you the truth since you can still see their mobo and monitor dimensions to see if stuff adds up
        // to add to this you can easily take screenshots and send it to your server.
        protected string GetMonitor()
        {
            // this is the size and dimensions of the monitor
          
                try
                {
                    string monitorsize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.ToString();
                    return monitorsize;


                }
                catch (Exception ex)
                {
                    return "";


                }



            



        }
        // DONT USE THIS FOR HWID
        protected string GetMobo
        {

            get
            {
                try
                {
                    foreach (ManagementObject queryObj in baseboardSearcher.Get())
                    {
                        return queryObj["Manufacturer"].ToString();
                    }
                    return "";
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            
        }

        public void HwidCall()
        {
            // makes sure the methods are executed
        
            GetMonitor();
            GetD();
            GetC();
            GetMACAddress();
            Constant();
        }

    }
}
