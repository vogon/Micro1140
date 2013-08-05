using System;
using Microsoft.SPOT;

namespace Micro1140.Cpu
{
    public class Cpu
    {
        private readonly byte[] phys;
        private readonly ushort[] regs = new ushort[8];
        private ushort psw;
        private bool n, z, v, c;

        // opcode fields
        private const ushort BOP        = 0xF000;       // binary opcode format         : 0b 1111 0000 0000 0000
        private const ushort UOP_LO     = 0x0FC0;       // low part of unary opcode     : 0b 0000 1111 1100 0000
        private const ushort SMODE      = 0x0E00;       // src addressing mode          : 0b 0000 1110 0000 0000
        private const ushort RS         = 0x01C0;       // src reg                      : 0b 0000 0001 1100 0000
        private const ushort DMODE      = 0x0038;       // dest addressing mode         : 0b 0000 0000 0011 1000
        private const ushort RD         = 0x0007;       // dest reg                     : 0b 0000 0000 0000 0111

        // opcode field shifts
        private const int BOP_SHIFT = 12;
        private const int UOP_LO_SHIFT = 6;
        private const int SMODE_SHIFT = 9;
        private const int RS_SHIFT = 6;
        private const int DMODE_SHIFT = 3;

        public Cpu(uint physSize)
        {
            phys = new byte[physSize];
            regs[0] = regs[1] = regs[2] = regs[3] = regs[4] = regs[5] = regs[6] = regs[7] = 0;
            psw = 0;
            n = z = v = c = false;
        }

        public void Next()
        {
            ushort opcode = ReadMem2(regs[7]);
            // advance PC past opcode
            regs[7] += 2;

            int bop = (opcode & BOP);

            switch (bop)
            {
                case 0x0000:
                    UnaryOpExecuteLow(opcode);
                    break;
                case 0x8000:
                    UnaryOpExecuteHigh(opcode);
                    break;
                default:
                    BinaryOpExecute(opcode);
                    break;
            }
        }

        private void UnaryOpExecuteLow(ushort opcode)
        {
            int rd = (opcode & RD);
            int dmode = (opcode & DMODE) >> DMODE_SHIFT;

            ushort imm = ReadMem2(regs[7] + 2);

            switch ((opcode & UOP_LO) >> UOP_LO_SHIFT)
            {
                case 0x00:
                    // HALT/WAIT/RTI/BPT/IOT/RESET/RTT
                case 0x01:
                    // JMP
                case 0x02:
                    // RTS/SPL/NOP
                case 0x03:
                    // SWAB
                case 0x04:
                    // BR
                case 0x08:
                    // BNE
                case 0x0C:
                    // BEQ
                case 0x10:
                    // BGE
                case 0x14:
                    // BLT
                case 0x18:
                    // BGT
                case 0x1C:
                    // BLE
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25:
                case 0x26:
                case 0x27:
                    // JSR
                    throw new NotImplementedException();
                case 0x28:
                    {
                        // CLR
                        WriteOperand2(mode: dmode, rn: rd, value: 0, imm: imm);

                        n = false;
                        z = true;
                        v = false;
                        c = false;

                        break;
                    }
                case 0x29:
                // COM
                    throw new NotImplementedException();
                case 0x2A:
                    {
                        // INC
                        ushort value = ReadOperand2(mode: dmode, rn: rd, imm: imm, pendingWriteback: true);
                        ushort result = (ushort)(value + 1);

                        WriteOperand2(mode: dmode, rn: rd, value: result, imm: imm, afterRead: true);

                        n = (result & 0x8000) != 0;
                        z = (result == 0);
                        v = (value == 0xffff);
                        /* C flag unchanged */

                        break;
                    }
                case 0x2B:
                    // DEC
                case 0x2C:
                    // NEG
                case 0x2D:
                    // ADC
                case 0x2E:
                    // SBC
                case 0x2F:
                    // TST
                case 0x30:
                    // ROR
                case 0x31:
                    // ROL
                case 0x32:
                    // ASR
                case 0x33:
                    // ASL
                case 0x34:
                    // MARK
                case 0x35:
                    // MFPI
                case 0x36:
                    // MTPI
                case 0x37:
                    // SXT
                default:
                    throw new NotImplementedException();
            }
        }

        private void UnaryOpExecuteHigh(ushort opcode)
        {
            int rd = (opcode & RD);
            int dmode = (opcode & DMODE) >> DMODE_SHIFT;

            ushort imm = ReadMem2(regs[7] + 2);

            switch ((opcode & UOP_LO) >> UOP_LO_SHIFT)
            {
                case 0x00:
                // BPL
                case 0x04:
                // BMI
                case 0x08:
                // BHI
                case 0x0C:
                // BLOS
                case 0x10:
                // BVC
                case 0x14:
                // BVS
                case 0x18:
                // BCC, BHIS
                case 0x1C:
                // BCS, BLO
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                // EMT
                case 0x24:
                case 0x25:
                case 0x26:
                case 0x27:
                // TRAP
                    throw new NotImplementedException();
                case 0x28:
                    {
                        // CLRB
                        WriteOperand1(mode: dmode, rn: rd, value: 0, imm: imm);
                        
                        n = false;
                        z = true;
                        v = false;
                        c = false;

                        break;
                    }
                case 0x29:
                    {
                        // COMB
                        byte value = ReadOperand1(mode: dmode, rn: rd, imm: imm, pendingWriteback: true);
                        byte result = (byte)~value;

                        WriteOperand1(mode: dmode, rn: rd, value: result, imm: imm, afterRead: true);

                        n = (result & 0x80) != 0;
                        z = (result == 0);
                        v = false;
                        c = true;

                        break;
                    }
                case 0x2A:
                    // INCB
                case 0x2B:
                    // DECB
                case 0x2C:
                    // NEGB
                case 0x2D:
                    // ADCB
                case 0x2E:
                    // SBCB
                case 0x2F:
                    // TSTB
                case 0x30:
                    // RORB
                case 0x31:
                    // ROLB
                case 0x32:
                    // ASRB
                case 0x33:
                    // ASLB
                case 0x35:
                    // MFPD
                case 0x36:
                    // MTPD
                default:
                    throw new NotImplementedException();
            }
        }

        private void BinaryOpExecute(ushort opcode)
        {
            int rs = (opcode & RS) >> RS_SHIFT, rd = (opcode & RD);
            int smode = (opcode & SMODE) >> SMODE_SHIFT, dmode = (opcode & DMODE) >> DMODE_SHIFT;
            
            ushort imm = ReadMem2(regs[7] + 2);

            switch (opcode & BOP)
            {
                case 0x1000:
                    {
                        // MOV
                        ushort value = ReadOperand2(mode: smode, rn: rs, imm: imm);
                        WriteOperand2(mode: dmode, rn: rd, value: value, imm: imm);
                        
                        n = (value & 0x8000) != 0;
                        z = (value == 0);
                        v = false;
                        /* C flag is left unchanged */
                        
                        break;
                    }
                case 0x2000:
                    {
                        // CMP
                        ushort src = ReadOperand2(mode: smode, rn: rs, imm: imm);
                        ushort dst = ReadOperand2(mode: dmode, rn: rd, imm: imm);
                        int result = src - dst;

                        n = (result & 0x8000) != 0;
                        z = (result & 0xFFFF) == 0;
                        v = ((src ^ dst) & 0x8000) != 0 && ((dst ^ result) & 0x8000) == 0;
                        c = (result & 0x10000) == 0;

                        break;
                    }
                case 0x3000:
                case 0x4000:
                case 0x5000:
                    throw new NotImplementedException();
                case 0x6000:
                    {
                        // ADD
                        ushort src = ReadOperand2(mode: smode, rn: rs, imm: imm);
                        ushort dst = ReadOperand2(mode: dmode, rn: rd, imm: imm, pendingWriteback: true);
                        int result = src + dst;

                        WriteOperand2(mode: dmode, rn: rd, value: (ushort)result, imm: imm, afterRead: true);

                        n = (result & 0x8000) != 0;
                        z = (result & 0xFFFF) == 0;
                        v = ((src ^ dst) & 0x8000) == 0 && ((dst ^ result) & 0x8000) != 0;
                        c = (result & 0x10000) != 0;

                        break;
                    }
                case 0x7000:
                case 0x8000:
                    throw new NotImplementedException();
                case 0x9000:
                    {
                        // MOVB
                        byte value = ReadOperand1(mode: smode, rn: rs, imm: imm);
                        if (dmode != 0)
                        {
                            WriteOperand1(mode: dmode, rn: rd, value: value, imm: imm);
                        }
                        else
                        {
                            // special case: MOVB to a register sign-extends
                            ushort sextVal = (ushort)(((value & 0x80) != 0) ? (0xF0 | value) : value);
                            WriteOperand2(mode: 0, rn: rd, value: sextVal, imm: imm);
                        }

                        n = (value & 0x80) != 0;
                        z = (value == 0);
                        v = false;
                        /* C flag is unchanged */

                        break;
                    }
                case 0xA000:
                case 0xB000:
                case 0xC000:
                case 0xD000:
                    throw new NotImplementedException();
                case 0xE000:
                    {
                        // SUB
                        ushort src = ReadOperand2(mode: smode, rn: rs, imm: imm);
                        ushort dst = ReadOperand2(mode: dmode, rn: rd, imm: imm, pendingWriteback: true);
                        int result = dst - src;

                        WriteOperand2(mode: dmode, rn: rd, value: (ushort)result, imm: imm, afterRead: true);

                        n = (result & 0x8000) != 0;
                        z = (result & 0xFFFF) == 0;
                        v = ((src ^ dst) & 0x8000) != 0 && ((dst ^ result) & 0x8000) == 0;
                        c = (result & 0x10000) == 0;

                        break;
                    }
                case 0xF000:
                default:
                    throw new NotImplementedException();
            }
        }

        #region access to processor state for tests
        internal ushort PC
        {
            get
            {
                return regs[7];
            }
            set
            {
                regs[7] = value;
            }
        }
        internal ushort[] Registers { get { return regs; } }
        internal byte[] PhysicalMemory { get { return phys; } }
        #endregion access to processor state for tests

        #region read operands
        internal byte ReadOperand1(int mode, int rn, ushort imm = 0, bool pendingWriteback = false)
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
                        if (!pendingWriteback) regs[rn] += 1;
                        return value;
                    }
                case 3:
                    {
                        // deferred autoincrement mode
                        ushort ofs = ReadMem2(regs[rn]);
                        byte value = ReadMem1(ofs);
                        if (!pendingWriteback) regs[rn] += 2;
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

        internal ushort ReadOperand2(int mode, int rn, ushort imm = 0, bool pendingWriteback = false)
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
                        if (!pendingWriteback) regs[rn] += 2;
                        return value;
                    }
                case 3:
                    {
                        // deferred autoincrement mode
                        ushort ofs = ReadMem2(regs[rn]);
                        ushort value = ReadMem2(ofs);
                        if (!pendingWriteback) regs[rn] += 2;
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
        internal void WriteOperand1(int mode, int rn, byte value, ushort imm = 0, bool afterRead = false)
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
                    if (!afterRead) regs[rn] -= 1;
                    WriteMem1(regs[rn], value);
                    break;
                case 5:
                    {
                        // deferred autodecrement mode
                        if (!afterRead) regs[rn] -= 2;
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

        internal void WriteOperand2(int mode, int rn, ushort value, ushort imm = 0, bool afterRead = false)
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
                    if (!afterRead) regs[rn] -= 2;
                    WriteMem2(regs[rn], value);
                    break;
                case 5:
                    {
                        // deferred autodecrement mode
                        if (!afterRead) regs[rn] -= 2;
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
