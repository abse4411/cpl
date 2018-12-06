using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace cpl
{

    public class Compiler
    {
        #region Properties
        private Hashtable optable;
        protected Hashtable OpTable
        {
            private set
            {
                optable = value;
            }
            get
            {
                return optable;
            }
        }
        private Hashtable rstable;
        protected Hashtable RsTable
        {
            private set
            {
                rstable = value;
            }
            get
            {
                return rstable;
            }
        }
        #endregion
        static private string[] hexcode = new string[16]{ "0000", "0001", "0010", "0011",
                                                                    "0100", "0101", "0110", "0111",
                                                                    "1000", "1001", "1010", "1011",
                                                                    "1100", "1101", "1110", "1111" };
        public Compiler()
        {
            optable = new Hashtable();
            optable.Add("ADD", "0001");
            optable.Add("SUB", "0010");
            optable.Add("AND", "0011");
            optable.Add("INC", "0100");
            optable.Add("LD", "0101");
            optable.Add("ST", "0110");
            optable.Add("JC", "0111");
            optable.Add("JZ", "1000");
            optable.Add("JMP", "1001");
            optable.Add("OUT", "1010");
            optable.Add("IRET", "1011");
            optable.Add("DI", "1100");
            optable.Add("EI", "1101");
            optable.Add("STOP", "1110");

            rstable = new Hashtable();
            rstable.Add("R0", "00");
            rstable.Add("R1", "01");
            rstable.Add("R2", "10");
            rstable.Add("R3", "11");
        }

        private bool IsNumer(char ch) => (ch >= '0' && ch <= '9') ? true : false;
        private bool IsAlpha(char ch) => (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') ? true : false;

        virtual protected string GetRegr(string soure, ref int index)
        {
            string result = "";
            int len = soure.Length;

            if (index < len && soure[index++] == 'R')
                result += 'R';
            else
                return null;
            if (index < len && (soure[index] >= '0' || soure[index] <= '3'))
            {
                result += soure[index];
                index++;
            }
            else
                return null;

            return result;
        }

        virtual protected string GetOffset(string soure, ref int index)
        {
            string result = "";
            int len = soure.Length;

            if (index < len && ((soure[index] >= '0' && soure[index] <= '9') || (soure[index] >= 'A' && soure[index] <= 'F')))
            {
                result += soure[index];
                index++;
            }
            else
                return null;

            return result;
        }

        virtual protected int GetOperandsCount(string op)
        {
            if (string.Equals(op, "JC") || string.Equals(op, "JZ") || string.Equals(op, "INC") ||
                string.Equals(op, "OUT") || string.Equals(op, "JMP"))
            {
                return 1;
            }
            else
                if (string.Equals(op, "IRET") || string.Equals(op, "DI") || string.Equals(op, "EI") ||
                string.Equals(op, "STOP"))
            {
                return 0;
            }
            else
                return 2;
        }

        public string Work(string soureCode)
        {
            string result = "";
            string op;

            if (string.IsNullOrWhiteSpace(soureCode))
                return result + "Bad Input!!!";
            string soure = string.Copy(soureCode).ToUpper();
            int index = 0, len = soure.Length;
            while (index < len)
            {
                if (IsAlpha(soure[index]))
                {
                    op = "";
                    while (index < len && IsAlpha(soure[index]))
                    {
                        op += soure[index];
                        index++;
                    }
                    string opStr = (string)optable[op];
                    if (opStr != null)
                        result += opStr;
                    else
                        return result + "Cannot find the OP:\"" + op + "\" !!!";

                    int opCount = GetOperandsCount(op);
                    if (opCount == 0)
                        result += "????";
                    while (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == ';' || soure[index] == '\n'))
                    {
                        if (soure[index] == ';')
                        {
                            while (index < len && soure[index] != '\n')
                            {
                                result += soureCode[index];
                                index++;
                            }
                        }
                        else
                        {
                            result += soureCode[index];
                            index++;
                        }
                    }

                    if (opCount == 2)
                    {
                        string rd = GetRegr(soure, ref index);
                        if (rd == null)
                        {
                            return result + "Cannot find the first operand of OP\"" + op + "\" !!!";
                        }
                        string orStr = (string)rstable[rd];
                        if (orStr != null)
                            result += orStr;
                        else
                            return result + "Cannot find the first operand of OP\"" + op + "\" !!!";
                        while (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == ';' || soure[index] == '\n'))
                        {
                            if (soure[index] == ';')
                            {
                                while (index < len && soure[index] != '\n')
                                {
                                    result += soureCode[index];
                                    index++;
                                }
                            }
                            else
                            {
                                result += soureCode[index];
                                index++;
                            }
                        }
                        if (index >= len || soure[index++] != '[')
                            return result + "Cannot find the \" [\" before operand !!!";
                        string rs = GetRegr(soure, ref index);
                        if (rs == null)
                        {
                            return result + "Cannot find the second operand of OP\"" + op + "\" !!!";
                        }
                        orStr = (string)rstable[rs];
                        if (orStr != null)
                            result += orStr;
                        else
                            return result + "Cannot find the second operand of OP\"" + op + "\" !!!";
                        if (index >= len || soure[index++] != ']')
                            return result + "Cannot find the \" ]\" after operand !!!";
                    }
                    else
                        if (opCount == 1)
                    {
                        if (string.Equals(op, "JC") || string.Equals(op, "JZ"))
                        {
                            string num = GetOffset(soure, ref index);
                            if (num == null)
                            {
                                return result + "The  operand of OP\"" + op + "\"must a be hex number!!!";
                            }
                            try
                            {
                                result += hexcode[IsNumer(num[0]) ? num[0] - '0' : num[0] - 'A' + 10];
                            }
                            catch
                            {
                                return result += $"Exception at: hexcode[{num[0]}]";
                            }
                            continue;
                        }
                        else
                            if (string.Equals(op, "OUT"))
                        {
                            result += "XX";
                        }
                        string rd = GetRegr(soure, ref index);
                        if (rd == null)
                        {
                            return result + "Cannot find the first operand of OP\"" + op + "\" !!!";
                        }
                        string orStr = (string)rstable[rd];
                        if (optable != null)
                            result += orStr;
                        else
                            return result + "Cannot find the first operand of OP\"" + op + "\" !!!";
                        if (string.Equals(op, "INC") || string.Equals(op, "JMP"))
                        {
                            result += "XX";
                        }
                    }

                }
                if (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == ';' || soure[index] == '\n'))
                {
                    while (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == ';' || soure[index] == '\n'))
                    {
                        if (soure[index] == ';')
                        {
                            while (index < len && soure[index] != '\n')
                            {
                                result += soureCode[index];
                                index++;
                            }
                        }
                        else
                        {
                            result += soureCode[index];
                            index++;
                        }
                    }
                }
                else
                    if (index == len)
                    break;
                else
                    if (index < len)
                    return result + "Bad Input!!!";
            }
            return result;
        }

    }
}
