using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlayground
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestMemoryLayout tsl = new TestMemoryLayout();
            tsl.TestLayoutExplicit();
            tsl.TestStructMembers();

            Console.ReadLine();
        }
    }
}
