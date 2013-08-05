using System;
using Microsoft.SPOT;
using Micro1140.Cpu;

namespace Micro1140.CpuTests
{
    class Ch3CodeTests : ITestSuite
    {
        private Cpu.Cpu cpu;

        public Ch3CodeTests()
        {
            cpu = new Cpu.Cpu(262144);
        }

        public void Test_Ch331_2()
        {
            cpu.WriteRegOct(2, "000002");
            cpu.WriteRegOct(4, "000004");
            
            cpu.WriteMem2Oct(0, "060204");
            cpu.PC = 0;
            cpu.Next();
            
            Assert.Reg(cpu, 4, "000006");
            Assert.PC(cpu, 2);
        }

        public void Test_Ch331_3()
        {
            cpu.WriteRegOct(4, "022222");
            
            cpu.WriteMem2Oct(0, "105104");
            cpu.PC = 0;
            cpu.Next();
            
            Assert.Reg(cpu, 4, "022155");
            Assert.PC(cpu, 2);
        }

        public void Test_Ch332_1()
        {
            cpu.WriteMem2Oct("030000", "111116");
            cpu.WriteRegOct(5, "030000");
            
            cpu.WriteMem2Oct("020000", "005025");
            cpu.WriteRegOct(7, "020000");
            cpu.Next();

            Assert.WordAt(cpu, "030000", "000000");
            Assert.Reg(cpu, 5, "030002");
            Assert.PC(cpu, "020002");
        }

        public void Test_Ch332_2()
        {
            cpu.WriteMem2Oct("030000", "111116");
            cpu.WriteRegOct(5, "030000");

            cpu.WriteMem2Oct("020000", "105025");
            cpu.WriteRegOct(7, "020000");
            cpu.Next();

            Assert.WordAt(cpu, "030000", "111000");
            Assert.Reg(cpu, 5, "030001");
            Assert.PC(cpu, "020002");
        }

        public void Test_Ch332_3()
        {
            cpu.WriteMem2Oct("100002", "010000");
            cpu.WriteRegOct(2, "100002");
            cpu.WriteRegOct(4, "010000");

            cpu.WriteMem2Oct("010000", "062204");
            cpu.WriteRegOct(7, "010000");
            cpu.Next();

            Assert.WordAt(cpu, "100002", "010000");
            Assert.Reg(cpu, 2, "100004");
            Assert.Reg(cpu, 4, "020000");
            Assert.PC(cpu, "010002");
        }
    }
}
