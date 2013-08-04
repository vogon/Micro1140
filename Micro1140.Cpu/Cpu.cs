using System;
using Microsoft.SPOT;

namespace Micro1140.Cpu
{
    public class Cpu
    {
        private readonly byte[] phys;
        private readonly ushort[] regs = new ushort[8];
        private ushort psw;

        public Cpu(uint physSize)
        {
            phys = new byte[physSize];
            regs[0] = regs[1] = regs[2] = regs[3] = regs[4] = regs[5] = regs[6] = regs[7] = 0;
            psw = 0;
        }

        public void Next()
        {

        }

        #region access to processor state for tests
        internal ushort[] Registers { get { return regs; } }
        internal byte[] PhysicalMemory { get { return phys; } }
        #endregion access to processor state for tests

        #region read operands
        internal byte ReadOperand1(int mode, int rn, ushort imm = 0)
        {
            switch (mode)
            {
                case 0:
                    // register mode
                    return (byte)(regs[rn] & 0xff);
                case 1:
                    // deferred register mode
                    return ReadMem1(regs[rn]);
                case 2:
                    {
                        // autoincrement mode
                        byte value = ReadMem1(regs[rn]);
                        regs[rn] += 1;
                        return value;
                    }
                case 3:
                    {
                        // deferred autoincrement mode
                        ushort ofs = ReadMem2(regs[rn]);
                        byte value = ReadMem1(ofs);
                        regs[rn] += 2;
                        return value;
                    }
                case 4:
                    // autodecrement mode
                    regs[rn] -= 1;
                    return ReadMem1(regs[rn]);
                case 5:
                    {
                        // deferred autodecrement mode
                        regs[rn] -= 2;
                        ushort ofs = ReadMem2(regs[rn]);
                        return ReadMem1(ofs);
                    }
                case 6:
                    // index mode
                    return ReadMem1(regs[rn] + imm);
                case 7:
                    {
                        // deferred index mode
                        ushort ofs = ReadMem2(regs[rn]);
                        return ReadMem1(ofs + imm);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        internal ushort ReadOperand2(int mode, int rn, ushort imm = 0)
        {
            switch (mode)
            {
                case 0:
                    // register mode
                    return regs[rn];
                case 1:
                    // deferred register mode
                    return ReadMem2(regs[rn]);
                case 2:
                    {
                        // autoincrement mode
                        ushort value = ReadMem2(regs[rn]);
                        regs[rn] += 2;
                        return value;
                    }
                case 3:
                    {
                        // deferred autoincrement mode
                        ushort ofs = ReadMem2(regs[rn]);
                        ushort value = ReadMem2(ofs);
                        regs[rn] += 2;
                        return value;
                    }
                case 4:
                    // autodecrement mode
                    regs[rn] -= 2;
                    return ReadMem2(regs[rn]);
                case 5:
                    {
                        // deferred autodecrement mode
                        regs[rn] -= 2;
                        ushort ofs = ReadMem2(regs[rn]);
                        return ReadMem2(ofs);
                    }
                case 6:
                    // index mode
                    return ReadMem2(regs[rn] + imm);
                case 7:
                    {
                        // deferred index mode
                        ushort ofs = ReadMem2(regs[rn]);
                        return ReadMem2(ofs + imm);
                    }
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion read operands

        #region write operands
        internal void WriteOperand1(int mode, int rn, byte value, ushort imm = 0)
        {
            switch (mode)
            {
                case 0:
                    // register mode
                    regs[rn] = (ushort)((regs[rn] & 0xff00) | value);
                    break;
                case 1:
                    // deferred register mode
                    WriteMem1(regs[rn], value);
                    break;
                case 2:
                    // autoincrement mode
                    WriteMem1(regs[rn], value);
                    regs[rn] += 1;
                    break;
                case 3:
                    {
                        // deferred autoincrement mode
                        ushort ofs = ReadMem2(regs[rn]);
                        WriteMem1(ofs, value);
                        regs[rn] += 2;
                        break;
                    }
                case 4:
                    // autodecrement mode
                    regs[rn] -= 1;
                    WriteMem1(regs[rn], value);
                    break;
                case 5:
                    {
                        // deferred autodecrement mode
                        regs[rn] -= 2;
                        ushort ofs = ReadMem2(regs[rn]);
                        WriteMem1(ofs, value);
                        break;
                    }
                case 6:
                    // index mode
                    WriteMem1(regs[rn] + imm, value);
                    break;
                case 7:
                    {
                        // deferred index mode
                        ushort ofs = ReadMem2(regs[rn]);
                        WriteMem1(ofs + imm, value);
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        internal void WriteOperand2(int mode, int rn, ushort value, ushort imm = 0)
        {
            switch (mode)
            {
                case 0:
                    // register mode
                    regs[rn] = value;
                    break;
                case 1:
                    // deferred register mode
                    WriteMem2(regs[rn], value);
                    break;
                case 2:
                    // autoincrement mode
                    WriteMem2(regs[rn], value);
                    regs[rn] += 2;
                    break;
                case 3:
                    {
                        // deferred autoincrement mode
                        ushort ofs = ReadMem2(regs[rn]);
                        WriteMem2(ofs, value);
                        regs[rn] += 2;
                        break;
                    }
                case 4:
                    // autodecrement mode
                    regs[rn] -= 2;
                    WriteMem2(regs[rn], value);
                    break;
                case 5:
                    {
                        // deferred autodecrement mode
                        regs[rn] -= 2;
                        ushort ofs = ReadMem2(regs[rn]);
                        WriteMem2(ofs, value);
                        break;
                    }
                case 6:
                    // index mode
                    WriteMem2(regs[rn] + imm, value);
                    break;
                case 7:
                    {
                        // deferred index mode
                        ushort ofs = ReadMem2(regs[rn]);
                        WriteMem2(ofs + imm, value);
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion write operands

        #region memory read primitives
        internal byte ReadMem1(int ofs)
        {
            return phys[ofs];
        }

        internal ushort ReadMem2(int ofs)
        {
            return (ushort)((phys[ofs + 1] << 8) | phys[ofs]);
        }
        #endregion memory read primitives

        #region memory write primitives
        internal void WriteMem1(int ofs, byte value)
        {
            phys[ofs] = value;
        }

        internal void WriteMem2(int ofs, ushort value)
        {
            phys[ofs] = (byte)(value & 0xFF);
            phys[ofs + 1] = (byte)(value >> 8);
        }
        #endregion memory write primitives
    }
}
