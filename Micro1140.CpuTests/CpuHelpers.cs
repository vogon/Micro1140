using System;
using Microsoft.SPOT;
using Micro1140.Cpu;

namespace Micro1140.CpuTests
{
    internal static class ExtensionMethods
    {
        internal static void WriteMem2Oct(this Cpu.Cpu cpu, string ofsOctal, string valueOctal)
        {
            cpu.WriteMem2(ofsOctal.AsOctal(), valueOctal.AsOctal());
        }

        internal static void WriteMem2Oct(this Cpu.Cpu cpu, int ofs, string octal)
        {
            cpu.WriteMem2(ofs, octal.AsOctal());
        }

        internal static void WriteRegOct(this Cpu.Cpu cpu, int rn, string octal)
        {
            cpu.Registers[rn] = octal.AsOctal();
        }

        internal static ushort AsOctal(this string s)
        {
            if (s == null) throw new ArgumentNullException();
            if (s.Length > 6) throw new ArgumentOutOfRangeException();

            ushort acc = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < '0' || s[i] > '9') throw new ArgumentException();
                
                acc <<= 3;
                acc += (ushort)(s[i] - '0');
            }

            return acc;
        }
    }

    public static partial class Assert
    {
        internal static void ByteAt(Cpu.Cpu cpu, int ofs, byte expected)
        {
            byte actual = cpu.PhysicalMemory[ofs];
            string message = "Assert.ByteAt: M[" + ofs.ToString() + "] == " + actual.ToString() + " != " + expected.ToString();

            Assert.AreEqual(actual, expected, message: message);
        }

        internal static void WordAt(Cpu.Cpu cpu, int ofs, ushort expected)
        {
            ushort actual = (ushort)((cpu.PhysicalMemory[ofs + 1] << 8) | cpu.PhysicalMemory[ofs]);
            string message = "Assert.WordAt: M[" + ofs.ToString() + "] == " + actual.ToString() + " != " + expected.ToString();

            Assert.AreEqual(actual, expected, message: message);
        }

        internal static void Reg(Cpu.Cpu cpu, int rn, ushort expected)
        {
            ushort actual = cpu.Registers[rn];
            string message = "Assert.Reg: r" + rn.ToString() + " == " + actual.ToString() + " != " + expected.ToString();

            Assert.AreEqual(actual, expected, message: message);
        }

        internal static void PC(Cpu.Cpu cpu, ushort value)
        {
            Assert.Reg(cpu, 7, value);
        }

        internal static void Reg(Cpu.Cpu cpu, int rn, string octal)
        {
            Assert.Reg(cpu, rn, octal.AsOctal());
        }

        internal static void WordAt(Cpu.Cpu cpu, string ofs, string octal)
        {
            Assert.WordAt(cpu, ofs.AsOctal(), octal.AsOctal());
        }

        internal static void PC(Cpu.Cpu cpu, string value)
        {
            Assert.PC(cpu, value.AsOctal());
        }
    }
}
