using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace cpl
{

    public class Compiler
    {
        #region Properties
        private StringBuilder result;
        public StringBuilder Result
        {
            get
            {
                return result;
            }
        }
        private StringBuilder rowString;
        public StringBuilder RowString
        {
            get
            {
                return rowString;
            }
        }
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
            result = new StringBuilder(100);
            rowString = new StringBuilder(10);
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
            optable.Add("NOP", "0000");

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
            if (index < len && (soure[index] >= '0' && soure[index] <= '3'))
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

            while (index < len)
            {
                if ((soure[index] >= '0' && soure[index] <= '9') || (soure[index] >= 'A' && soure[index] <= 'F'))
                    result += soure[index];
                else
                    if (soure[index] == 'H')
                {
                    index++;
                    return result;
                }
                else
                    return null;
                index++;
            }

            if (result.Length > 0 && result[result.Length - 1] != 'H')
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
                string.Equals(op, "STOP") || string.Equals(op, "NOP"))
            {
                return 0;
            }
            else
                return 2;
        }

        public string Work(string soureCode)
        {
            result.Clear();
            rowString.Clear();
            int counter = -1, offset = 0;
            int rdi = 0, rsi = 0;
            StringBuilder op = new StringBuilder(10);

            if (string.IsNullOrWhiteSpace(soureCode))
                return result.Append(International.GetString("WFYC")).ToString();
            string soure = string.Copy(soureCode).ToUpper();
            int index = 0, len = soure.Length;
            while (index < len)
            {
                if (IsAlpha(soure[index]))
                {
                    counter++;
                    op.Clear();
                    while (index < len && IsAlpha(soure[index]))
                    {
                        op.Append(soure[index]);
                        index++;
                    }
                    string opStr = (string)optable[op.ToString()];
                    if (opStr != null)
                        result.Append(opStr);
                    else
                        return result.Append(International.GetString("CFTOP")).Append('"').Append(op).Append("\" !!!").ToString();
                    int opCount = GetOperandsCount(op.ToString());
                    if (opCount == 0)
                    {
                        result.Append("XXXX");
                        continue;
                    }

                    while (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == ';' || soure[index] == '\n'))
                    {
                        if (soure[index] == ';')
                        {
                            while (index < len && soure[index] != '\n')
                            {
                                result.Append(soureCode[index]);
                                index++;
                            }
                        }
                        else
                        {
                            result.Append(soureCode[index]);
                            index++;
                        }
                    }

                    if (opCount == 2)
                    {
                        string rd = GetRegr(soure, ref index);
                        if (rd == null)
                        {
                            return result.Append(International.GetString("CFTFO")).Append('"').Append(op).Append( "\" !!!").ToString();
                        }
                        string orStr = (string)rstable[rd];
                        if (orStr != null)
                            result.Append(orStr);
                        else
                            return result.Append(International.GetString("CFTFO")).Append('"').Append(op).Append( "\" !!!").ToString();

                        rdi = result.Length - 2;

                        while (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == ';' || soure[index] == '\n'))
                        {
                            if (soure[index] == ';')
                            {
                                while (index < len && soure[index] != '\n')
                                {
                                    result.Append(soureCode[index]);
                                    index++;
                                }
                            }
                            else
                            {
                                result.Append(soureCode[index]);
                                index++;
                            }
                        }
                        if (index >= len || soure[index++] != ',')
                            return result.Append(International.GetString("CFTDO")).Append(" !!!").ToString();
                        if ((string.Equals(op.ToString(), "LD") || string.Equals(op.ToString(), "ST")) && (index >= len || soure[index++] != '['))
                            return result.Append(International.GetString("CFTLO")).Append(" !!!").ToString();
                        string rs = GetRegr(soure, ref index);
                        if (rs == null)
                        {
                            return result.Append(International.GetString("CFTSO")).Append('"').Append(op).Append("\" !!!").ToString();
                        }
                        orStr = (string)rstable[rs];
                        if (orStr != null)
                            result.Append(orStr);
                        else
                            return result.Append(International.GetString("CFTSO")).Append('"').Append(op).Append("\" !!!").ToString();

                        rsi = result.Length - 2;

                        if ((string.Equals(op.ToString(), "LD") || string.Equals(op.ToString(), "ST")) && (index >= len || soure[index++] != ']'))
                            return result.Append(International.GetString("CFTRO")).Append(" !!!").ToString();
                        if (string.Equals(op.ToString(), "ST"))
                        {
                            char ch0 = result[rdi];
                            char ch1 = result[rdi + 1];
                            result[rdi] = result[rsi];
                            result[rdi + 1] = result[rsi + 1];
                            result[rsi] = ch0;
                            result[rsi + 1] = ch1;
                            //char[] tmpResult = result.ToCharArray();
                            //tmp = result[rdi];
                            //tmpResult[rdi] = tmpResult[rsi];
                            //tmpResult[rsi] = tmp;
                            //tmp = result[rdi + 1];
                            //tmpResult[rdi + 1] = tmpResult[rsi + 1];
                            //tmpResult[rsi + 1] = tmp;
                            //result = new string(tmpResult);
                        }
                    }
                    else
                        if (opCount == 1)
                    {
                        if (string.Equals(op.ToString(), "JC") || string.Equals(op.ToString(), "JZ"))
                        {
                            string num = GetOffset(soure, ref index);
                            if (num == null)
                            {
                                return result.Append(International.GetString("TOOP")).Append('"').Append(op).Append('"').Append(International.GetString("MBAHN")).Append(" !!!").ToString();
                            }

                            if (Int32.TryParse(num, System.Globalization.NumberStyles.HexNumber, null, out offset))
                            {
                                offset = offset - counter - 1;
                                if (offset < 0)
                                {
                                    if (offset < -8)
                                        return result.Append(International.GetString("TVLIUR")).Append(" .").ToString();
                                    offset += 16;
                                }
                                else
                                {
                                    if (offset > 7)
                                        return result.Append(International.GetString("TVLIUR")).Append(" .").ToString();
                                }
                            }
                            else
                                return result.Append(International.GetString("TOOP")).Append('"').Append(op).Append('"').Append(International.GetString("MBAHN")).Append(" !!!").ToString();
                            result.Append(hexcode[offset]);
                            continue;
                        }
                        else
                            if (string.Equals(op.ToString(), "OUT"))
                        {
                            result.Append("XX");
                        }
                        if (string.Equals(op.ToString(), "JMP") && (index >= len || soure[index++] != '['))
                            return result.Append(International.GetString("CFTLO")).Append(" !!!").ToString();
                        string rd = GetRegr(soure, ref index);
                        if (rd == null)
                        {
                            return result.Append(International.GetString("CFTFO")).Append('"').Append(op).Append("\" !!!").ToString();
                        }
                        string orStr = (string)rstable[rd];
                        if (optable != null)
                            result.Append(orStr);
                        else
                            return result.Append(International.GetString("CFTFO")).Append('"').Append(op).Append("\" !!!").ToString();
                        if (string.Equals(op.ToString(), "INC") || string.Equals(op.ToString(), "JMP"))
                        {
                            result.Append("XX");
                        }
                        if (string.Equals(op.ToString(), "JMP") && (index >= len || soure[index++] != ']'))
                            return result.Append(International.GetString("CFTRO")).Append(" !!!").ToString();
                    }

                }
                if (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == '\t' || soure[index] == ';' || soure[index] == '\n'))
                {
                    while (index < len && (soure[index] == ' ' || soure[index] == '\r' || soure[index] == '\t' || soure[index] == ';' || soure[index] == '\n'))
                    {
                        if (soure[index] == '\r')
                            rowString.AppendFormat("{0:X2}H\r\n", counter == -1 ? 0 : counter);
                        if (soure[index] == ';')
                        {
                            while (index < len && soure[index] != '\n')
                            {
                                if (soure[index] == '\r')
                                    rowString.AppendFormat("{0:X2}H\r\n", counter == -1 ? 0 : counter);
                                result.Append(soureCode[index]);
                                index++;
                            }
                        }
                        else
                        {
                            result.Append(soureCode[index]);
                            index++;
                        }
                    }
                }
                else
                    if (index == len)
                    break;
                else
                    if (index < len)
                    return result.Append(International.GetString("BI")).Append(" !!!").ToString();
            }
            return result.ToString();
        }

        public Task<string> WorkAsync(string soureCode)
        {
            var task = new Task<string>(()=>Work(soureCode));
            task.Start();
            return task;
        }
    }
}
