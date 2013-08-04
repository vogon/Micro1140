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

        #region Addressing mode 1 (deferred register)
        public void Test_Mode1_Read1()
        {
            cpu.Registers[0] = 0x0110;
            cpu.PhysicalMemory[0x0110] = 0x34;
            cpu.PhysicalMemory[0x0110 + 1] = 0x12;
            byte r0 = cpu.ReadOperand1(mode: 1, rn: 0);
            Assert.AreEqual(r0, 0x34);
            Assert.Reg(cpu, 0, 0x0110);
        }

        public void Test_Mode1_Read2()
        {
            cpu.Registers[0] = 0x0120;
            cpu.PhysicalMemory[0x0120] = 0x34;
            cpu.PhysicalMemory[0x0120 + 1] = 0x12;
            ushort r0 = cpu.ReadOperand2(mode: 1, rn: 0);
            Assert.AreEqual(r0, 0x1234);
            Assert.Reg(cpu, 0, 0x0120);
        }

        public void Test_Mode1_Write1()
        {
            cpu.Registers[0] = 0x0130;
            cpu.PhysicalMemory[0x0130] = 0xAD;
            cpu.PhysicalMemory[0x0130 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 1, rn: 0, value: 0x12);
            Assert.WordAt(cpu, 0x0130, 0xDE12);
            Assert.Reg(cpu, 0, 0x0130);
        }

        public void Test_Mode1_Write2()
        {
            cpu.Registers[0] = 0x0140;
            cpu.PhysicalMemory[0x0140] = 0xAD;
            cpu.PhysicalMemory[0x0140 + 1] = 0xDE;
            cpu.WriteOperand2(mode: 1, rn: 0, value: 0x1234);
            Assert.WordAt(cpu, 0x0140, 0x1234);
            Assert.Reg(cpu, 0, 0x0140);
        }
        #endregion Addressing mode 1 (deferred register)

        #region Addressing mode 2 (autoincrement)
        public void Test_Mode2_Read1()
        {
            cpu.Registers[0] = 0x0210;
            cpu.PhysicalMemory[0x0210] = 0xCD;
            cpu.PhysicalMemory[0x0210 + 1] = 0xAB;
            byte r0 = cpu.ReadOperand1(mode: 2, rn: 0);
            Assert.AreEqual(r0, 0xCD);
            Assert.Reg(cpu, 0, 0x0211);
        }

        public void Test_Mode2_Read2()
        {
            cpu.Registers[0] = 0x0220;
            cpu.PhysicalMemory[0x0220] = 0x89;
            cpu.PhysicalMemory[0x0220 + 1] = 0x67;
            ushort r0 = cpu.ReadOperand2(mode: 2, rn: 0);
            Assert.AreEqual(r0, 0x6789);
            Assert.Reg(cpu, 0, 0x0222);
        }

        public void Test_Mode2_Write1()
        {
            cpu.Registers[0] = 0x0230;
            cpu.PhysicalMemory[0x0230] = 0xAD;
            cpu.PhysicalMemory[0x0230 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 2, rn: 0, value: 0x12);
            Assert.WordAt(cpu, 0x0230, 0xDE12);
            Assert.Reg(cpu, 0, 0x0231);
        }

        public void Test_Mode2_Write2()
        {
            cpu.Registers[0] = 0x0240;
            cpu.PhysicalMemory[0x0240] = 0xAD;
            cpu.PhysicalMemory[0x0240 + 1] = 0xDE;
            cpu.WriteOperand2(mode: 2, rn: 0, value: 0x1234);
            Assert.WordAt(cpu, 0x0240, 0x01234);
            Assert.Reg(cpu, 0, 0x0242);
        }
        #endregion Addressing mode 2 (autoincrement)

        #region Addressing mode 3 (deferred autoincrement)
        public void Test_Mode3_Read1()
        {
            cpu.Registers[0] = 0x0310;
            cpu.PhysicalMemory[0x0310] = 0x18;
            cpu.PhysicalMemory[0x0310 + 1] = 0x03;
            cpu.PhysicalMemory[0x0318] = 0x34;
            cpu.PhysicalMemory[0x0318 + 1] = 0x12;
            byte r0 = cpu.ReadOperand1(mode: 3, rn: 0);
            Assert.AreEqual(r0, 0x34);
            Assert.WordAt(cpu, 0x0310, 0x0318);
            Assert.Reg(cpu, 0, 0x0312);
        }

        public void Test_Mode3_Read2()
        {
            cpu.Registers[0] = 0x0320;
            cpu.PhysicalMemory[0x0320] = 0x28;
            cpu.PhysicalMemory[0x0320 + 1] = 0x03;
            cpu.PhysicalMemory[0x0328] = 0x34;
            cpu.PhysicalMemory[0x0328 + 1] = 0x12;
            ushort r0 = cpu.ReadOperand2(mode: 3, rn: 0);
            Assert.AreEqual(r0, 0x1234);
            Assert.WordAt(cpu, 0x0320, 0x0328);
            Assert.Reg(cpu, 0, 0x0322);
        }

        public void Test_Mode3_Write1()
        {
            cpu.Registers[0] = 0x0330;
            cpu.PhysicalMemory[0x0330] = 0x38;
            cpu.PhysicalMemory[0x0330 + 1] = 0x03;
            cpu.PhysicalMemory[0x0338] = 0xAD;
            cpu.PhysicalMemory[0x0338 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 3, rn: 0, value: 0x12);
            Assert.WordAt(cpu, 0x0330, 0x0338);
            Assert.WordAt(cpu, 0x0338, 0xDE12);
            Assert.Reg(cpu, 0, 0x0332);
        }

        public void Test_Mode3_Write2()
        {
            cpu.Registers[0] = 0x0340;
            cpu.PhysicalMemory[0x0340] = 0x48;
            cpu.PhysicalMemory[0x0340 + 1] = 0x03;
            cpu.PhysicalMemory[0x0348] = 0xAD;
            cpu.PhysicalMemory[0x0348 + 1] = 0xDE;
            cpu.WriteOperand2(mode: 3, rn: 0, value: 0x1234);
            Assert.WordAt(cpu, 0x0340, 0x0348);
            Assert.WordAt(cpu, 0x0348, 0x1234);
            Assert.Reg(cpu, 0, 0x0342);
        }
        #endregion Addressing mode 3 (deferred autoincrement)

        #region Addressing mode 4 (autodecrement)
        public void Test_Mode4_Read1()
        {
            cpu.Registers[0] = 0x0410;
            cpu.PhysicalMemory[0x040E] = 0xCD;
            cpu.PhysicalMemory[0x040E + 1] = 0xAB;
            byte r0 = cpu.ReadOperand1(mode: 4, rn: 0);
            Assert.AreEqual(r0, 0xAB);
            Assert.Reg(cpu, 0, 0x040F);
        }

        public void Test_Mode4_Read2()
        {
            cpu.Registers[0] = 0x0420;
            cpu.PhysicalMemory[0x041E] = 0x34;
            cpu.PhysicalMemory[0x041E + 1] = 0x12;
            ushort r0 = cpu.ReadOperand2(mode: 4, rn: 0);
            Assert.AreEqual(r0, 0x1234);
            Assert.Reg(cpu, 0, 0x041E);            
        }

        public void Test_Mode4_Write1()
        {
            cpu.Registers[0] = 0x0430;
            cpu.PhysicalMemory[0x042E] = 0xAD;
            cpu.PhysicalMemory[0x042E + 1] = 0xDE;
            cpu.WriteOperand1(mode: 4, rn: 0, value: 0x12);
            Assert.WordAt(cpu, 0x042E, 0x12AD);
            Assert.Reg(cpu, 0, 0x042F);
        }

        public void Test_Mode4_Write2()
        {
            cpu.Registers[0] = 0x0440;
            cpu.PhysicalMemory[0x043E] = 0xAD;
            cpu.PhysicalMemory[0x043E + 1] = 0xDE;
            cpu.WriteOperand2(mode: 4, rn: 0, value: 0x1234);
            Assert.WordAt(cpu, 0x043E, 0x1234);
            Assert.Reg(cpu, 0, 0x043E);
        }
        #endregion Addressing mode 4 (autodecrement)

        #region Addressing mode 6 (index)
        public void Test_Mode6_Read1()
        {
            cpu.Registers[0] = 0x0610;
            cpu.PhysicalMemory[0x0612] = 0x34;
            cpu.PhysicalMemory[0x0612 + 1] = 0x12;
            byte r0 = cpu.ReadOperand1(mode: 6, rn: 0, imm: 0x02);
            Assert.AreEqual(r0, 0x34);
        }

        public void Test_Mode6_Read2()
        {
            cpu.Registers[0] = 0x0620;
            cpu.PhysicalMemory[0x0622] = 0x34;
            cpu.PhysicalMemory[0x0622 + 1] = 0x12;
            ushort r0 = cpu.ReadOperand2(mode: 6, rn: 0, imm: 0x02);
            Assert.AreEqual(r0, 0x1234);
        }

        public void Test_Mode6_Write1()
        {
            cpu.Registers[0] = 0x0630;
            cpu.PhysicalMemory[0x0632] = 0xAD;
            cpu.PhysicalMemory[0x0632 + 1] = 0xDE;
            cpu.WriteOperand1(mode: 6, rn: 0, value: 0x12, imm: 0x02);
            Assert.WordAt(cpu, 0x0632, 0xDE12);
        }

        public void Test_Mode6_Write2()
        {
            cpu.Registers[0] = 0x0640;
            cpu.PhysicalMemory[0x0642] = 0xAD;
            cpu.PhysicalMemory[0x0642 + 1] = 0xDE;
            cpu.WriteOperand2(mode: 6, rn: 0, value: 0x1234, imm: 0x02);
            Assert.WordAt(cpu, 0x0642, 0x1234);
        }
        #endregion Addressing mode 6 (index)
    }
}
