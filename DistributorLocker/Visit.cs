using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributorLocker
{
    public class InvistLocker : Locker
    {
        public override void Invisit()
        {
            Thread.Sleep(1000);
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var locker = new InvistLocker();
            locker.ConnectionString = "192.168.119.131:2181";
            locker.OurPath = "seq";
            locker.Parent = "/locker";

            try
            {
                while (!locker.IsGetLock())
                {
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
