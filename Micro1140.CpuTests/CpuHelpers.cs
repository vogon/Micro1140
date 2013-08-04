using System;
using Microsoft.SPOT;
using Micro1140.Cpu;

namespace Micro1140.CpuTests
{
    public static partial class Assert
    {
        internal static void ByteAt(Cpu.Cpu cpu, int ofs, byte value)
        {
            Assert.AreEqual(cpu.PhysicalMemory[ofs], value);
        }

        internal static void WordAt(Cpu.Cpu cpu, int ofs, ushort value)
        {
            byte lo = (byte)(value & 0xff), hi = (byte)(value >> 8);
            Assert.AreEqual(cpu.PhysicalMemory[ofs], lo);
            Assert.AreEqual(cpu.PhysicalMemory[ofs + 1], hi);
        }

        internal static void Reg(Cpu.Cpu cpu, int rn, ushort value)
        {
            Assert.AreEqual(cpu.Registers[rn], value);
        }
    }
}
