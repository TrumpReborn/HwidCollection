using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HwidEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            HwidCollection hwid = new HwidCollection();
            // you need to call the method to get the hwid!
            hwid.HwidCall();
        Console.WriteLine($"BaseBoard: {hwid.BID}\n ProcessorID: {hwid.PID}\n UUID: {hwid.UID} \n C Drive: {hwid.CID} \n D Drive: {hwid.DID} \n Motherboard Name: {hwid.MoboName} \n Monitor Size: {hwid.MonitorSize}\n MacAdress: {hwid.Mac} \n Spoofer Detected: {hwid.SpooferDetect}");
            Console.ReadLine();
        }
    }
}
