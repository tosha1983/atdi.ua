using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class DbExpressionField : DbField
    {
        public string m_expression;
        private List<DbField> m_items;
        private int m_nItems;
        public string m_fmt;

        public DbExpressionField()
        {
            this.m_items = new List<DbField>();
            this.m_nItems = 0;
        }
        //public void AddFldsInExpression()
        //{
        //    this.m_fmt = "";
        //    this.m_items = new List<DbField>();
        //    this.m_nItems = 0;
        //    bool needRetrieve = false;
        //    int i = 0;
        //    int num = i;
        //    bool flag = false;
        //    while (i < this.m_expression.Length)
        //    {
        //        if (flag)
        //        {
        //            if (this.m_expression[i] == '\'')
        //            {
        //                flag = false;
        //            }
        //            i++;
        //        }
        //        else if (this.m_expression[i] == '\'')
        //        {
        //            flag = true;
        //            i++;
        //        }
        //        else if (this.m_expression[i] == '[' && i + 1 < this.m_expression.Length && !char.IsDigit(this.m_expression[i + 1]))
        //        {
        //            if (i > num)
        //            {
        //                this.m_fmt += this.m_expression.Substring(num, i - num);
        //                num = i;
        //            }
        //            int num2 = this.m_expression.IndexOf(']', i);
        //            if (num2 < i)
        //            {
        //                i++;
        //            }
        //            else
        //            {
        //                string fldPath = this.m_expression.Substring(i + 1, num2 - i - 1);
        //                DbField ormItem = rs.AddFld(this.m_logTab, fldPath, null, needRetrieve);
        //                if (ormItem != null)
        //                {
        //                    this.m_items.Add(ormItem);
        //                    this.m_nItems++;
        //                    num = (i = num2 + 1);
        //                    this.m_fmt += string.Format("%{0}", this.m_nItems);
        //                }
        //                else
        //                {
        //                    i = num2 + 1;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }
        //    if (i > num)
        //    {
        //        this.m_fmt += this.m_expression.Substring(num);
        //    }
        //    ANetDb dB = rs.GetDB();
        //    if (dB != null)
        //    {
        //        this.m_fmt = dB.TranslateExpr(this.m_fmt, new ANetDb.TableTranslator(rs.TableTranslate));
        //    }
        //}
        public override string GetDataName()
        {
            string str = "";
            int num = 0;
            while (true)
            {
                int i = num;
                bool flag = false;
                while (i < this.m_fmt.Length)
                {
                    if (flag)
                    {
                        if (this.m_fmt[i] == '\'')
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        if (this.m_fmt[i] == '%')
                        {
                            break;
                        }
                        if (this.m_fmt[i] == '\'')
                        {
                            flag = true;
                        }
                    }
                    i++;
                }
                if (i == this.m_fmt.Length)
                {
                    break;
                }
                if (i > num)
                {
                    str += this.m_fmt.Substring(num, i - num);
                }
                if (i + 1 < this.m_fmt.Length)
                {
                    if (this.m_fmt[i + 1] == '%')
                    {
                        str += "%";
                        num = i + 2;
                        continue;
                    }
                    if (char.IsDigit(this.m_fmt[i + 1]))
                    {
                        int num2 = (int)(this.m_fmt[i + 1] - '0');
                        i += 2;
                        while (i < this.m_fmt.Length && char.IsDigit(this.m_fmt[i]))
                        {
                            num2 = num2 * 10 + (int)(this.m_fmt[i] - '0');
                            i++;
                        }
                        if (num2 <= this.m_nItems)
                        {
                            str += this.m_items[num2 - 1].GetDataName();
                        }
                        num = i;
                        continue;
                    }
                }
                str += "%";
                num = i + 1;
            }
            return str + this.m_fmt.Substring(num);
        }
    }
}
