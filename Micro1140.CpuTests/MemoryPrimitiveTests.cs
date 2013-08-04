using System;
using Microsoft.SPOT;

namespace Micro1140.CpuTests
{
    public class MemoryPrimitiveTests : ITestSuite
    {
        private readonly Cpu.Cpu cpu;

        public MemoryPrimitiveTests()
        {
            cpu = new Cpu.Cpu(4096);
        }

        public void Test_ReadMem1()
        {
            cpu.PhysicalMemory[0] = 0x01;
            cpu.PhysicalMemory[1] = 0x02;
            byte b = cpu.ReadMem1(0);
            Assert.AreEqual(b, 0x01);
        }

        public void Test_ReadMem2()
        {
            cpu.PhysicalMemory[0] = 0x01;
            cpu.PhysicalMemory[1] = 0x02;
            ushort w = cpu.ReadMem2(0);
            Assert.AreEqual(w, 0x0201);
        }

        public void Test_WriteMem1()
        {
            cpu.WriteMem1(2, 0x03);
            Assert.ByteAt(cpu, 2, 0x03);
        }

        public void Test_WriteMem2()
        {
            cpu.WriteMem2(2, 0x0403);
            Assert.WordAt(cpu, 2, 0x0403);
        }
    }
}
