using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlayground
{
    class TestStructMemberClass
    {
        float a;
        float b;
    }

    struct TestEmptyStruct
    { }

    [StructLayout(LayoutKind.Sequential)]
    struct TestStruct
    {
        public int a;
        long b;
        public StringBuilder sb;
    }

    internal class TestMemoryLayout
    {
        public void TestLayoutExplicit()
        {
            unsafe
            {
                Console.WriteLine($"Boolean size: {sizeof(bool)}");
                Console.WriteLine($"Float size: {sizeof(float)}");
                Console.WriteLine($"Double size: {sizeof(double)}");
                Console.WriteLine($"Empty struct size: {sizeof(TestEmptyStruct)}");
                Console.WriteLine($"Struct size: {sizeof(TestStruct)}");
                Console.WriteLine($"Class size: {sizeof(StringBuilder)}");
            }
        }

        public void TestStructMembers()
        {
            TestStruct ts = new TestStruct();
            ts.a = -1;
            Console.WriteLine($"ts.a (before) = {ts.a}");
            TestAnotherStructMembers(ts);
            Console.WriteLine($"ts.a (after) = {ts.a}");
            Console.WriteLine($"ts.sb is null: {ts.sb == null}");

        }

        private void TestAnotherStructMembers(TestStruct ts)
        {
            ts.a = 1;
            ts.sb = new StringBuilder();
            Console.WriteLine($"ts.a = {ts.a}");
            Console.WriteLine($"ts.sb is null: {ts.sb == null}");
        }
    }
}
