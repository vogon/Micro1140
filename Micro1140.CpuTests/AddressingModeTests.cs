using System;
using Microsoft.SPOT;
using Micro1140.Cpu;

namespace Micro1140.CpuTests
{
    class AddressingModeTests : ITestSuite
    {
        private readonly Cpu.Cpu cpu;

        public AddressingModeTests()
        {
            cpu = new Cpu.Cpu(4096);
        }

        #region Addressing mode 0 (register)
        public void Test_Mode0_Read1()
        {
            cpu.Registers[0] = 0x0123;
            byte r0 = cpu.ReadOperand1(mode: 0, rn: 0);
            Assert.AreEqual(r0, 0x23);
        }

        public void Test_Mode0_Read2()
        {
            cpu.Registers[0] = 0x4567;
            ushort r0 = cpu.ReadOperand2(mode: 0, rn: 0);
            Assert.AreEqual(r0, 0x4567);
        }

        public void Test_Mode0_Write1()
        {
            cpu.Registers[0] = 0xDEAD;
            cpu.WriteOperand1(mode: 0, rn: 0, value: 0x12);
            Assert.Reg(cpu, 0, 0xDE12);
        }

        public void Test_Mode0_Write2()
        {
            cpu.Registers[0] = 0xDEAD;
            cpu.WriteOperand2(mode: 0, rn: 0, value: 0x1234);
            Assert.Reg(cpu, 0, 0x1234);
        }
        #endregion Addressing mode 0 (register)

        #region Addressing mode 2 (autoincrement)
        public void Test_Mode2_Read1()
        {
            cpu.Registers[0] = 0x0122;
            cpu.PhysicalMemory[0x0122] = 0xCD;
            cpu.PhysicalMemory[0x0122 + 1] = 0xAB;
            byte r0 = cpu.ReadOperand1(mode: 2, rn: 0);
            Assert.AreEqual(r0, 0xCD);
            Assert.Reg(cpu, 0, 0x0123);
        }

        public void Test_Mode2_Read2()
        {
            cpu.Registers[0] = 0x0244;
            cpu.PhysicalMemory[0x0244] = 0x89;
            cpu.PhysicalMemory[0x0244 + 1] = 0x67;
            ushort r0 = cpu.ReadOperand2(mode: 2, rn: 0);
            Assert.AreEqual(r0, 0x6789);
            Assert.Reg(cpu, 0, 0x0246);
        }

        public void Test_Mode2_Write1()
        {
            cpu.Registers[0] = 0x0366;
            cpu.PhysicalMemory[0x0366] = 0xAD;
            cpu.PhysicalMemory[0x0366 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 2, rn: 0, value: 0x12);
            Assert.WordAt(cpu, 0x0366, 0xDE12);
            Assert.Reg(cpu, 0, 0x0367);
        }

        public void Test_Mode2_Write2()
        {
            cpu.Registers[0] = 0x0488;
            cpu.PhysicalMemory[0x0488] = 0xAD;
            cpu.PhysicalMemory[0x0488 + 1] = 0xDE;
            cpu.WriteOperand2(mode: 2, rn: 0, value: 0x1234);
            Assert.WordAt(cpu, 0x0488, 0x01234);
            Assert.Reg(cpu, 0, 0x048A);
        }
        #endregion Addressing mode 2 (autoincrement)

        #region Addressing mode 4 (autodecrement)
        public void Test_Mode4_Read1()
        {
            cpu.Registers[0] = 0x0132;
            cpu.PhysicalMemory[0x0130] = 0xCD;
            cpu.PhysicalMemory[0x0130 + 1] = 0xAB;
            byte r0 = cpu.ReadOperand1(mode: 4, rn: 0);
            Assert.AreEqual(r0, 0xAB);
            Assert.Reg(cpu, 0, 0x0131);
        }

        public void Test_Mode4_Read2()
        {
            cpu.Registers[0] = 0x0264;
            cpu.PhysicalMemory[0x0262] = 0x34;
            cpu.PhysicalMemory[0x0262 + 1] = 0x12;
            ushort r0 = cpu.ReadOperand2(mode: 4, rn: 0);
            Assert.AreEqual(r0, 0x1234);
            Assert.Reg(cpu, 0, 0x0262);            
        }

        public void Test_Mode4_Write1()
        {
            cpu.Registers[0] = 0x0396;
            cpu.PhysicalMemory[0x0394] = 0xAD;
            cpu.PhysicalMemory[0x0394 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 4, rn: 0, value: 0x12);
            Assert.WordAt(cpu, 0x0394, 0x12AD);
            Assert.Reg(cpu, 0, 0x0395);
        }

        public void Test_Mode4_Write2()
        {
            cpu.Registers[0] = 0x04C8;
            cpu.PhysicalMemory[0x04C6] = 0xAD;
            cpu.PhysicalMemory[0x04C7 + 1] = 0xDE;
            cpu.WriteOperand2(mode: 4, rn: 0, value: 0x1234);
            Assert.WordAt(cpu, 0x04C6, 0x1234);
            Assert.Reg(cpu, 0, 0x04C6);
        }
        #endregion Addressing mode 4 (autodecrement)

        #region Addressing mode 6 (index)
        public void Test_Mode6_Read1()
        {
            cpu.Registers[0] = 0x0112;
            cpu.PhysicalMemory[0x0114] = 0x34;
            cpu.PhysicalMemory[0x0114 + 1] = 0x12;
            byte r0 = cpu.ReadOperand1(mode: 6, rn: 0, imm: 0x02);
            Assert.AreEqual(r0, 0x34);
        }

        public void Test_Mode6_Read2()
        {
            cpu.Registers[0] = 0x0224;
            cpu.PhysicalMemory[0x0226] = 0x34;
            cpu.PhysicalMemory[0x0226 + 1] = 0x12;
            ushort r0 = cpu.ReadOperand2(mode: 6, rn: 0, imm: 0x02);
            Assert.AreEqual(r0, 0x1234);
        }

        public void Test_Mode6_Write1()
        {
            cpu.Registers[0] = 0x0336;
            cpu.PhysicalMemory[0x0338] = 0xAD;
            cpu.PhysicalMemory[0x0338 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 6, rn: 0, value: 0x12, imm: 0x02);
            Assert.WordAt(cpu, 0x0338, 0xDE12);
        }

        public void Test_Mode6_Write2()
        {
            cpu.Registers[0] = 0x0448;
            cpu.PhysicalMemory[0x044A] = 0xAD;
            cpu.PhysicalMemory[0x044A + 1] = 0xDE;
            cpu.WriteOperand2(mode: 6, rn: 0, value: 0x1234, imm: 0x02);
            Assert.WordAt(cpu, 0x044A, 0x1234);
        }
        #endregion Addressing mode 6 (index)
    }
}
