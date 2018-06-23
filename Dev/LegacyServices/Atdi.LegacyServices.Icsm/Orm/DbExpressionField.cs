using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class DbExpressionField : DbField
    {
        public  DBMS _dbms = DBMS.Unknown;
        public string m_expression;
        private List<Field> m_items;
        private int m_nItems;
        public string m_fmt;
        public string IdPrefix, IdSuffix;
        public string QuoteColumn(string fld) { return IdPrefix + fld + IdSuffix; }

        public delegate string TableTranslator(string tab);

        public DbExpressionField()
        {
            this.m_items = new List<Field>();
            this.m_nItems = 0;
            
        }

        private string GetTableName(string val)
        {
            return this.m_logTab;
        }

        private static string GetAliasMainTable(string expression, string nameTable)
        {
            int ident = -1;
            string value = expression;
            int idx = value.IndexOf(nameTable);

            if (idx != -1)
            {
                value = value.Substring(idx + nameTable.Length, value.Length - (idx + nameTable.Length));
            }
            var sql = new StringBuilder();
            bool start = false;
            foreach (var symbol in value)
            {
                if ((start == true) && ((symbol == ' ') || (symbol == '\t')))
                    break;

                if ((symbol == ' ') || (symbol == '\t') && (start == false))
                {
                    ++ident;
                }
                else
                {
                    start = true;
                    sql.Append(symbol);
                }
            }

            if (sql.Length == 0)
            {
                throw new InvalidOperationException(Exceptions.NotRecognizeAlias.With(expression));
            }
            return sql.ToString();
        }

        public void AddFldsInExpression(SchemasMetadata schemasMetadata)
        {
            if (schemasMetadata._configDataEngine.Config.Type == Contracts.CoreServices.DataLayer.DataEngineType.Oracle)
                _dbms = DBMS.Oracle;
            if (schemasMetadata._configDataEngine.Config.Type == Contracts.CoreServices.DataLayer.DataEngineType.SqlServer)
                _dbms = DBMS.MsSql;
            this.m_fmt = "";
            this.m_items = new List<Field>();
            this.m_nItems = 0;
            bool needRetrieve = false;
            int i = 0;
            int num = i;
            bool flag = false;
            while (i < this.m_expression.Length)
            {
                if (flag)
                {
                    if (this.m_expression[i] == '\'')
                    {
                        flag = false;
                    }
                    i++;
                }
                else if (this.m_expression[i] == '\'')
                {
                    flag = true;
                    i++;
                }
                else if (this.m_expression[i] == '[' && i + 1 < this.m_expression.Length && !char.IsDigit(this.m_expression[i + 1]))
                {
                    if (i > num)
                    {
                        this.m_fmt += this.m_expression.Substring(num, i - num);
                        num = i;
                    }
                    int num2 = this.m_expression.IndexOf(']', i);
                    if (num2 < i)
                    {
                        i++;
                    }
                    else
                    {
                        var ormTable = schemasMetadata.GetTableByName(this.m_logTab);
                        string fldPath = this.m_expression.Substring(i + 1, num2 - i - 1);
                        Field ormItem = ormTable.Field(fldPath);
                        if (ormItem != null)
                        {
                            this.m_items.Add(ormItem);
                            this.m_nItems++;
                            num = (i = num2 + 1);
                            if (_dbms== DBMS.MsSql) this.m_fmt += "Tcaz_0.["+ ormItem.Name+"]";
                            else if (_dbms == DBMS.Oracle) this.m_fmt += $"Tcaz_0.\"{ormItem.Name}\"";
                        }
                        else
                        {
                            i = num2 + 1;
                        }
                    }
                }
                else
                {
                    i++;
                }
            }
            if (i > num)
            {
                this.m_fmt += this.m_expression.Substring(num);
            }
            this.m_fmt = TranslateExpr(this.m_fmt, new TableTranslator(GetTableName));
        }



        string parseParam(string expr, ref int it)
        {
            //eat comma but not end parenthesis
            int s;
            int prof = 0;
            bool inString = false;
            for (s = it; s < expr.Length; s++)
            {
                if (inString)
                {
                    if (expr[s] == '\'') { if (s + 1 < expr.Length && expr[s + 1] == '\'') ++s; else inString = false; }
                }
                else
                {
                    if (expr[s] == '\'') inString = true;
                    else if (expr[s] == '(') ++prof;
                    else if (expr[s] == ',') { if (prof == 0) { string res = expr.Substring(it, s - it); it = s + 1; return res; } }
                    else if (expr[s] == ')') { if (prof != 0) --prof; else { string res = expr.Substring(it, s - it); it = s; return res; } }
                }
            }
             it = s; return ""; //erreur ) not found
        }
        bool trouve(string expr, int idx, string w)
        {
            int wl = w.Length;
            if (idx + wl > expr.Length) return false;
            for (int i = 0; i < wl; i++) if (expr[idx + i] != w[i]) return false;
            return true;
        }


        public string SqlIdent(string s)
        {
            
            if (_dbms == DBMS.MsSql || _dbms == DBMS.Access)
                return string.Format("[{0}]", s);
            else if (_dbms == DBMS.Oracle || _dbms == DBMS.SQLite)
                return string.Format("\"{0}\"", s);
            else { return null; }
        }
        public string SqlFormatConstDate(DateTime da)
        {
            if (da.Ticks == 0) return "NULL";
            if (_dbms == DBMS.MsSql)
                return string.Format("'{0:0000}{1:00}{2:00}'", da.Year, da.Month, da.Day);
            else if (_dbms == DBMS.Oracle)
                return string.Format("TO_DATE('{0}/{1}/{2}','dd/mm/YYYY')", da.Day, da.Month, da.Year);
            else if (_dbms == DBMS.Access)
                return string.Format("#{0}/{1}/{2}#", da.Month, da.Day, da.Year);
            else if (_dbms == DBMS.MySQL)
                return string.Format("'{0:0000}/{1:00}/{2:00}'", da.Year, da.Month, da.Day);
            else if (_dbms == DBMS.InterBase)  //update station set dt='2004-12-12 15:33:22.45';
                return string.Format("'{0:00}/{1:00}/{2:0000}'", da.Month, da.Day, da.Year);
            else if (_dbms == DBMS.SQLite)
                return string.Format("'{0:0000}-{1:00}-{2:00}'", da.Year, da.Month, da.Day);
            else { return null; }
        }
        public string SqlFormatConstDatetime(DateTime da)
        {
            if (da.Ticks == 0) return "NULL";
            if (_dbms == DBMS.MsSql)
                return string.Format("'{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}.{6:000}Z'", da.Year, da.Month, da.Day, da.Hour, da.Minute, da.Second, da.Millisecond);
            else if (_dbms == DBMS.Oracle)
                return string.Format("TO_DATE('{0}/{1}/{2} {3:00}:{4:00}:{5:00}','dd/mm/YYYY HH24:MI:SS')", da.Day, da.Month, da.Year, da.Hour, da.Minute, da.Second);
            else if (_dbms == DBMS.Access)
                return string.Format("#{0}/{1}/{2} {3}:{4}:{5}#", da.Month, da.Day, da.Year, da.Hour, da.Minute, da.Second);
            else if (_dbms == DBMS.MySQL)
                return string.Format("'{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", da.Year, da.Month, da.Day, da.Hour, da.Minute, da.Second);
            else if (_dbms == DBMS.InterBase)  //update station set dt='2004-12-12 15:33:22.45';
                return string.Format("'{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}'", da.Month, da.Day, da.Year, da.Hour, da.Minute, da.Second);
            else if (_dbms == DBMS.SQLite)
                return string.Format("'{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}'", da.Year, da.Month, da.Day, da.Hour, da.Minute, da.Second, da.Millisecond);
            else { return null; }
        }


        public static string QuoteString(string s, char quote, char endQuote = '\0')
        {
            if (endQuote == 0) endQuote = quote;
            StringBuilder ss = new StringBuilder();
            ss.Append(quote);
            foreach (char c in s)
            {
                ss.Append(c);
                if (c == quote) ss.Append(c); //Double inside quotes
            }
            ss.Append(endQuote);
            return ss.ToString();
        }

        public string SqlValue(object a)
        {
            if (a == null) return "NULL";
            if (a is string)
                return QuoteString((string)a, '\'');
            else if (a is DateTime)
                return SqlFormatConstDate((DateTime)a);
            else if (a is double)
            {
                double d = (double)a;
                if (d == 1e-99) return "NULL";
                return d.ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
            }
            else if (a is int)
            {
                int i = (int)a;
                if (i == 0x7FFFFFFF) return "NULL";
                return i.ToString();
            }
            else { throw new Exception("ANetDb.SqlValue unsupported type"); }
        }
        public string SqlFormatWhereNull(bool enull, string fld)
        {
            string not = enull ? "" : "NOT ";
            return string.Format("({0} IS {1}NULL)", fld, not);
        }
        public string SqlFormatWhereStrNull(bool enull, string fld)
        {
            if (_dbms == DBMS.InterBase || _dbms == DBMS.MsSql)
                return string.Format(enull ? "({0} IS NULL OR {0}='')" : "({0} IS NOT NULL AND {0}!='')", fld);
            else if (_dbms == DBMS.MySQL || _dbms == DBMS.Oracle || _dbms == DBMS.SQLite)
                return string.Format(enull ? "({0} IS NULL OR LENGTH({0})=0)" : "LENGTH({0})>0", fld);
            else if (_dbms == DBMS.Access) //type==tDao || //type==tAdo || 
                return string.Format(enull ? "(ISNULL(%s) OR LEN(%s)=0)" : "LEN(%s)>0", fld);
            else if (_dbms == DBMS.VisualFoxPro)
                return string.Format(enull ? "(ISNULL(%s) OR LEN(ALLTRIM(%s))=0)" : "LEN(ALLTRIM(%s))>0", fld);
            else
            {
                return string.Format(enull ? "({0} IS NULL OR {0}='')" : "({0} IS NOT NULL AND {0}!='')", fld);
            }
        }

        public string CodeLikeWhere(bool like, string name, string pattern, bool crystal)
        {
            StringBuilder b = new StringBuilder();
            string wher = name;
            string patupper = pattern.ToUpper(); //better for NLS
            bool needToUpper = patupper != pattern;
            bool esc = false;
            if (crystal)
            {
                if (needToUpper) { pattern = patupper; wher = "UPPERCASE(" + name + ")"; }
                foreach (char c in pattern) b.Append(char.ToUpper(c));
            }
            //else if (Dbms==DBMS.MsSql && type!=tAdoNet) { //&& type!=tAdo
            //    //not case sensitive; % _ [a-b] ESCAPE 
            //    for(;*pattern;pattern++)
            //        if (*pattern=='_' || *pattern=='%' || *pattern=='[' || *pattern==']')
            //            { *b++='\\'; *b++= *pattern;  } 
            //        else if (*pattern=='\\') { *b++= '\\'; *b++= '\\'; }
            //        else if (*pattern=='*') *b++= '%';
            //        else if (*pattern=='?') *b++= '_';
            //        else *b++= *pattern;
            //    }
            else if (_dbms == DBMS.Oracle || _dbms == DBMS.InterBase)
            {
                if (needToUpper) pattern = patupper;
                wher = "UPPER(" + name + ")";
                foreach (char c in pattern)
                    if (c == '*') b.Append('%');
                    else if (c == '?') b.Append('_');
                    else if (c == '%' || c == '_' || c == '\\') { b.Append('\\'); b.Append(c); esc = true; }
                    else b.Append(c);
            }
            else if (_dbms == DBMS.MySQL)
            { //idem *->% ?->_ escape using \; also C style \->\\\\ \n -> \\n
                foreach (char c in pattern)
                    if (c == '_' || c == '%') { b.Append('\\'); b.Append(c); }
                    else if (c == '\\') { b.Append('\\'); b.Append('\\'); b.Append('\\'); b.Append('\\'); }
                    else if (c == '*') b.Append('%');
                    else if (c == '?') b.Append('_');
                    else b.Append(c);
            }
            else
            { //type==tAdoNet
              //idem *->% ?->_ escape using []
                foreach (char c in pattern)
                    if (c == '[' || c == ']' || c == '_' || c == '%')
                    { b.Append('['); b.Append(c); b.Append(']'); }
                    else if (c == '*') b.Append('%');
                    else if (c == '?') b.Append('_');
                    else b.Append(c);
            }
            //else {
            //    for(;*pattern;pattern++) *b++ = *pattern;
            //    }
            string res = QuoteString(b.ToString(), '\'');
            if (esc) res += " ESCAPE '\\'";
            wher += like ? " LIKE " : " NOT LIKE ";
            wher += res;
            return wher;
        }
        private string Nvl(string p1, string p1N)
        {
            if (_dbms == DBMS.MsSql)
                return string.Format("ISNULL({0},{1})", p1, p1N);
            else if (_dbms == DBMS.Access)
                return string.Format("IIF(IsNull({0}),{1},{2})", p1, p1N, p1);
            else if (_dbms == DBMS.SQLite)
                return string.Format("ifnull({0},{1})", p1, p1N);
            else return string.Format("NVL({0},{1})", p1, p1N);
        }
        private string translateRADIANS(string p1)
        {
            if (_dbms == DBMS.MsSql)
                return string.Format("(({0})*PI()/180)", p1); //RADIANS exists but RADIANS(180.0)=3.14, RADIANS(180)=3 !
            else if (_dbms == DBMS.Access)
                return string.Format("(({0})/45*ATN(1))", p1);
            else if (_dbms == DBMS.SQLite)
                return string.Format("(({0})*0.0174532925199433)", p1);
            else  //if (Oracle...
                return string.Format("(({0})/90*ASIN(1))", p1);
        }
        private string translateACOS(string p1, string long1, string lat1, string long2, string lat2)
        {
            if (_dbms == DBMS.Access)
            {
                string reA = string.Format("IIF(({0})>0.99999999999999,0.0,ATN(SQR(1/({0})/({0})-1)))", p1);
                string xnul = ""; string or = ""; double D;
                if (!double.TryParse(long1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                { xnul += or + string.Format("isnull({0})", long1); or = " OR "; }
                if (!double.TryParse(lat1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                { xnul += or + string.Format("isnull({0})", lat1); or = " OR "; }
                if (!double.TryParse(long2, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                { xnul += or + string.Format("isnull({0})", long2); or = " OR "; }
                if (!double.TryParse(lat2, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                { xnul += or + string.Format("isnull({0})", lat2); or = " OR "; }
                if (xnul != "")
                    reA = string.Format("IIF({0},Null,{1})", xnul, reA);
                return reA;
                //return string.Format("IIF(isnull({0}),Null,IIF(({0})>0.99999999999999,0.0,ATN(SQR(1/({0})/({0})-1))))",p1);
            }
            else  //if (Oracle, SQL Server... 
                return string.Format("ACOS(ROUND({0},13))", p1); //in case rounding error: 
        }

        public string TranslateExpr(string expr, TableTranslator ttrans)
        {
            string res = "";
            int ie = 0;
            while (ie < expr.Length)
            {
                if (trouve(expr, ie, "#t("))
                {
                    string translatedConstDate = null;
                    int it = ie + 3;
                    string p1 = parseParam(expr, ref it);
                    string[] ap = p1.Split('/');
                    int year, month, day;
                    if (ap.Length >= 3 && int.TryParse(ap[0], out day) && int.TryParse(ap[1], out month) && int.TryParse(ap[2], out year) && day >= 1 && day <= 31 && month >= 1 && month <= 12 && year >= 1700 && year <= 3000)
                    {
                        int hour, minute, second, millisecond;
                        if (ap.Length == 7 && int.TryParse(ap[3], out hour) && hour >= 0 && hour < 24 && int.TryParse(ap[4], out minute) && minute >= 0 && minute < 60 && int.TryParse(ap[5], out second) && second >= 0 && second < 60 && int.TryParse(ap[6], out millisecond) && millisecond >= 0 && millisecond < 1000)
                            translatedConstDate = SqlFormatConstDatetime(new DateTime(year, month, day, hour, minute, second, millisecond));
                        else translatedConstDate = SqlFormatConstDate(new DateTime(year, month, day));
                    }
                    if (translatedConstDate != null) { res += translatedConstDate; ie = it + 1; continue; }
                }
                if (trouve(expr, ie, "NVL("))
                {
                    int it = ie + 4;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    res += Nvl(p1, p2);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "CASE(") || trouve(expr, ie, "IIF("))
                {
                    int it = expr.IndexOf('(', ie) + 1;
                    //Forme généralisée CASE(cond1,val1,cond2,val2,...,condn,valn,valdef)
                    //   traduit en Access par IIF(cond1,val1,IIF(cond2,val2, ... IIF(condn,valn,valdef)...))
                    //   traduit sinon par  (CASE WHEN cond1 THEN val1 WHEN cond2 THEN val2 ... WHEN condn THEN valn ELSE valdef END)
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p3 = TranslateExpr(parseParam(expr, ref it), ttrans);

                    if (_dbms == DBMS.Access) res += string.Format("IIF({0},{1},", p1, p2);
                    else res += string.Format("(CASE WHEN {0} THEN {1}", p1, p2);
                    int prof = 1;

                    while (it < expr.Length && expr[it] != ')')
                    {
                        p1 = p3;
                        p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                        p3 = TranslateExpr(parseParam(expr, ref it), ttrans);
                        if (_dbms == DBMS.Access) { res += string.Format("IIF({0},{1},", p1, p2); ++prof; }
                        else res += string.Format(" WHEN {0} THEN {1}", p1, p2);
                    }
                    if (_dbms == DBMS.Access) res += p3 + new string(')', prof);
                    else res += string.Format(" ELSE {0} END)", p3);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "GREATEST("))
                {
                    int it = ie + 9;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Oracle)
                        res += string.Format("GREATEST({0},{1})", p1, p2);
                    if (_dbms == DBMS.Access)
                        res += string.Format("IIF(({0})>({1}),{0},{1})", p1, p2);
                    else res += string.Format("(CASE WHEN ({0})>({1}) THEN {0} ELSE {1} END)", p1, p2);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "LOG10("))
                {
                    int it = ie + 6;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Access)
                        res += string.Format("(LOG({0})/LOG(10))", p1);
                    else if (_dbms == DBMS.Oracle)
                        res += string.Format("LOG(10,{0})", p1);
                    else //sure works for if (m_oServer==isSqlServer) +sqlite extension
                        res += string.Format("LOG10({0})", p1);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "GEODISTKM("))
                {
                    int it = ie + 10;
                    string long1 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    string lat1 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    string long2 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    string lat2 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();

                    //"6373*ACOS(sin(RADIANS(lat1))*sin(RADIANS(lat2))+cos(RADIANS(lat1))*cos(RADIANS(lat2))*cos(RADIANS(long2-long1)))"
                    double D;
                    string sinLat1, cosLat1;
                    if (double.TryParse(lat1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                    {
                        sinLat1 = Math.Sin(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                        cosLat1 = Math.Cos(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                    }
                    else
                    {
                        string rt = translateRADIANS(lat1);
                        sinLat1 = "sin" + rt;
                        cosLat1 = "cos" + rt;
                    }
                    string sinLat2, cosLat2;
                    if (double.TryParse(lat2, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                    {
                        sinLat2 = Math.Sin(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                        cosLat2 = Math.Cos(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                    }
                    else
                    {
                        string rt = translateRADIANS(lat2);
                        sinLat2 = "sin" + rt;
                        cosLat2 = "cos" + rt;
                    }
                    string rll = translateRADIANS(string.Format("({0})-({1})", long2, long1));
                    string ar = string.Format("{0}*{1}+{2}*{3}*cos{4}", sinLat1, sinLat2, cosLat1, cosLat2, rll);
                    res += string.Format("(6373*{0})", translateACOS(ar, long1, lat1, long2, lat2));
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "GEODISTKM_ORDER("))
                {
                    int it = ie + 16;
                    string long1 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    string lat1 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    string long2 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    string lat2 = TranslateExpr(parseParam(expr, ref it), ttrans).Trim();
                    //"6373*ACOS(sin(RADIANS(lat1))*sin(RADIANS(lat2))+cos(RADIANS(lat1))*cos(RADIANS(lat2))*cos(RADIANS(long2-long1)))"
                    double D;
                    string sinLat1, cosLat1;
                    if (double.TryParse(lat1, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                    {
                        sinLat1 = Math.Sin(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                        cosLat1 = Math.Cos(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                    }
                    else
                    {
                        string rt = translateRADIANS(lat1);
                        sinLat1 = "sin" + rt;
                        cosLat1 = "cos" + rt;
                    }
                    string msinLat2, cosLat2;
                    if (double.TryParse(lat2, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"), out D))
                    {
                        msinLat2 = (-Math.Sin(D * Math.PI / 180.0)).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                        cosLat2 = Math.Cos(D * Math.PI / 180.0).ToString("G15", System.Globalization.CultureInfo.CreateSpecificCulture("CultureUS"));
                    }
                    else
                    {
                        string rt = translateRADIANS(lat2);
                        msinLat2 = "sin(-" + rt + ")";
                        cosLat2 = "cos" + rt;
                    }
                    string rll = translateRADIANS(string.Format("({0})-({1})", long2, long1));
                    string ar = string.Format("{0}*{1}-{2}*{3}*cos{4}", sinLat1, msinLat2, cosLat1, cosLat2, rll);
                    res += ar;
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "POWER("))
                {
                    int it = ie + 6;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Access)
                        res += string.Format("(({0})^({1}))", p1, p2);
                    else if (_dbms == DBMS.MsSql)
                        res += string.Format("POWER(cast({0} as float),{1})", p1, p2);
                    else // (m_oServer==isOracle),(m_oServer==isSqlServer)+sqlite extension
                        res += string.Format("POWER({0},{1})", p1, p2);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "EQ("))
                {
                    int it = ie + 3;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Oracle || _dbms == DBMS.MsSql || _dbms == DBMS.SQLite)
                    {
                        res += string.Format("(CASE WHEN {0}={1} THEN 'Y' ELSE 'N' END)", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.Access)
                    {
                        res += string.Format("IIF({0}={1},'Y','N')", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else
                    {
                        res += string.Format("EQ({0},{1})", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    //if (_dbms==DBMS.MySQL) ???
                    //m_oServer==isInterBase ?????
                }
                if (trouve(expr, ie, "BYTESIZE("))
                {
                    int it = ie + 9;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.MsSql)
                    {
                        res += string.Format("DATALENGTH({0})", p1);
                        ie = it + 1;
                        continue;
                    } //for datatype image
                    else if (_dbms == DBMS.Oracle)
                    {
                        res += string.Format("DBMS_LOB.GETLENGTH({0})", p1);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.Access)
                    {
                        res += string.Format("2*Len({0})", p1);
                        ie = it + 1;
                        continue;
                    }
                    else
                    { //if (_dbms==DBMS.MySQL),SQLite
                        res += string.Format("LENGTH({0})", p1);
                        ie = it + 1;
                        continue;
                    }
                    //m_oServer==isInterBase ?????
                }
                if (trouve(expr, ie, "INSTR("))
                {
                    int it = ie + 6;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.MsSql)
                        res += string.Format("CHARINDEX({0},{1})", p2, p1); //Reverse order !!!
                    else if (_dbms == DBMS.Access)
                        res += string.Format("Instr(1,{0},{1})", p1, p2);
                    else //Oracle, SQLite
                        res += string.Format("INSTR({0},{1})", p1, p2);
                    continue;
                    //if (m_oServer==isMySQL) ???
                    //m_oServer==isInterBase ?????
                }
                if (trouve(expr, ie, "RADIANS("))
                {
                    int it = ie + 8;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    res += translateRADIANS(p1);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "DEGREES("))
                {
                    int it = ie + 8;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.MsSql)
                        res += string.Format("(({0})*180/PI())", p1); //RADIANS exists but RADIANS(180.0)=3.14, RADIANS(180)=3 !
                    else if (_dbms == DBMS.Access)
                        res += string.Format("(({0})*45/ATN(1))", p1);
                    else if (_dbms == DBMS.SQLite)
                        res += string.Format("(({0})*57.29577951308232)", p1);
                    else  //if (Oracle...
                        res += string.Format("(({0})*90/ASIN(1))", p1);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "SQRT("))
                {
                    int it = ie + 5;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Access)
                        res += string.Format("IIF(IsNull({0}),Null,Sqr({0}))", p1);
                    else  //if (Oracle, SQL Server...
                        res += string.Format("SQRT({0})", p1);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "ACOS("))
                {
                    int it = ie + 5;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    res += translateACOS(p1, p1, "0", "0", "0");
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "LENGTH("))
                {
                    int it = ie + 7;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.MsSql || _dbms == DBMS.Access)
                    {
                        res += string.Format("Len({0})", p1);
                        ie = it + 1;
                        continue;
                    }
                    else
                    { //if (Oracle...
                        res += string.Format("LENGTH({0})", p1);
                        ie = it + 1;
                        continue;
                    }
                    //m_oServer==isInterBase ?????
                }
                if (trouve(expr, ie, "SUBSTRING(") || trouve(expr, ie, "SUBSTR(") || trouve(expr, ie, "MID("))
                {
                    int iop = expr.IndexOf('(', ie);
                    string func = expr.Substring(ie, iop - ie);
                    int it = iop + 1;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p3 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.MsSql) func = "SUBSTRING";
                    if (_dbms == DBMS.Access) func = "MID"; //access!
                    if (_dbms == DBMS.Oracle || _dbms == DBMS.SQLite) func = "SUBSTR"; //Also for MySQL
                    res += string.Format("{0}({1},{2},{3})", func, p1, p2, p3);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "CONCATNVL("))
                {
                    int it = ie + 10;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p1N = TranslateExpr(parseParam(expr, ref it), ttrans);
                    res += Nvl(p1, p1N);
                    while (it < expr.Length && expr[it] != ')')
                    {
                        string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                        string p2N = TranslateExpr(parseParam(expr, ref it), ttrans);
                        if (_dbms == DBMS.Oracle || _dbms == DBMS.SQLite)
                            res += "||" + Nvl(p2, p2N);
                        else res += "+" + Nvl(p2, p2N);
                    }
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "CONCAT("))
                {
                    int it = ie + 7;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Oracle)
                    {
                        res += string.Format("(NVL({0},'')||NVL({1},''))", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.SQLite)
                    {
                        res += string.Format("(ifnull({0},'')||ifnull({1},''))", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.Access)
                    {
                        res += string.Format("(IIF(IsNull({0}),{1},{2}+{3}))", p2, p1, p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.MsSql)
                    {
                        res += string.Format("(ISNULL({0},'')+ISNULL({1},''))", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else
                    {
                        res += string.Format("({0}||{1})", p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    //if (m_oServer==isInterBase) ; //no NVL or IsNull...
                }
                if (trouve(expr, ie, "STARTDATE("))
                {
                    int it = ie + 10;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Oracle)
                        res += string.Format("NVL({0},TO_DATE('1/1/1900','dd/mm/YYYY'))", p1);
                    else if (_dbms == DBMS.Access)
                        res += string.Format("IIF(ISNULL({0}),#1/1/1900#,{1})", p1, p1);
                    else if (_dbms == DBMS.MsSql)
                        res += string.Format("ISNULL({0},19000101)", p1);
                    else if (_dbms == DBMS.SQLite)
                        res += string.Format("ifnull({0},'1900-01-01')", p1);
                    else res += p1;
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "STOPDATE("))
                {
                    int it = ie + 9;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Oracle)
                        res += string.Format("NVL({0},TO_DATE('31/12/2100','dd/mm/YYYY'))", p1);
                    else if (_dbms == DBMS.Access)
                        res += string.Format("IIF(ISNULL({0}),#31/12/2100#,{1})", p1, p1);
                    else if (_dbms == DBMS.MsSql)
                        res += string.Format("ISNULL({0},21001231)", p1);
                    else if (_dbms == DBMS.SQLite)
                        res += string.Format("ifnull({0},'2100-12-31')", p1);
                    else res += p1;
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "ELAPSED_DAYS("))
                {
                    int it = ie + 13;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = null; if (it < expr.Length && expr[it] != ')') p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Oracle)
                    {
                        if (p2 == null) p2 = "SYSDATE";
                        res += string.Format("(({0})-({1}))", p2, p1);
                    }
                    else if (_dbms == DBMS.Access)
                    {
                        if (p2 == null) p2 = "NOW()";
                        res += string.Format("(({0})-({1}))", p2, p1);
                    }
                    else if (_dbms == DBMS.MsSql)
                    {
                        if (p2 == null) p2 = "GETDATE()";
                        res += string.Format("DATEDIFF(day,{0},{1})", p2, p1);
                    }
                    else if (_dbms == DBMS.SQLite)
                    {
                        if (p2 == null) p2 = "datetime('now','localtime')";
                        res += string.Format("(({0})-({1}))", p2, p1);
                    }
                    else res += p1;
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "ADD_DAYS("))
                {
                    int it = ie + 9;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.SQLite)
                        res += string.Format("date({0},'+'||({1})||' days')", p1, p2);
                    else res += string.Format("(({0})+({1}))", p1, p2);
                    ie = it + 1;
                    continue;
                }
                if (trouve(expr, ie, "GETDATE()"))
                {
                    ie += 9;
                    if (_dbms == DBMS.Oracle)
                        res += "SYSDATE ";
                    else if (_dbms == DBMS.Access)
                        res += "NOW()";
                    else if (_dbms == DBMS.MsSql)
                        res += "GETDATE()";
                    else if (_dbms == DBMS.SQLite)
                        res += "datetime('now','localtime')";
                    else res += "GETDATE()"; //????
                    continue;
                }
                if (trouve(expr, ie, "CONCAT_LIST("))
                {
                    int it = ie + 12;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p3 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    //CONCAT_LIST(c.ROLE,SELECT DISTINCT c.ROLE,from %USERS_CNTCT c WHERE c.OPER_ID=[ID])
                    if (_dbms == DBMS.MsSql)
                    {
                        //SQL Server: (select DISTINCT c.ROLE AS 'data()' from %USERS_CNTCT c WHERE c.OPER_ID=[ID] FOR XML PATH(''))
                        res += "(" + p2 + " AS 'data()' " + p3 + " FOR XML PATH(''))";
                        ie = it + 1; continue;
                    }
                    if (_dbms == DBMS.SQLite)
                    {
                        //SQL Server: (select group_concat(xx) from (select distinct c.ROLE AS xx from %USERS_CNTCT c WHERE c.OPER_ID=[ID]))
                        res += "(select group_contact(xx) from (" + p2 + " AS xx " + p3 + "))";
                        ie = it + 1; continue;
                    }
                    //else if (m_oServer==isOracle) { Marche pas!!!!!, LISTAGG marche pas non plus
                    //	//Oracle: (select wm_concat(c.ROLE) from (SELECT DISTINCT c.ROLE from %USERS_CNTCT c WHERE c.OPER_ID=[ID]))
                    //	res+= "(SELECT wm_concat("+p1+") from ("+p2 +" "+p3+"))";
                    //	expr= t+1;
                    //	}
                    else
                    {
                        //Default: concatenate min and max... (SELECT MIN(c.ROLE)+' '+MAX(c.ROLE) FROM %USERS_CNTCT c WHERE c.OPER_ID=[ID])
                        string concatOp = _dbms == DBMS.Access ? "+" : "||";
                        res += "(SELECT MIN(" + p1 + ")" + concatOp + "' '" + concatOp + "MAX(" + p1 + ") " + p3 + ")";
                        ie = it + 1; continue;
                    }
                }
                if (trouve(expr, ie, "LPAD("))
                {
                    int it = ie + 5;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p3 = "' '"; if (it < expr.Length && expr[it] != ')') p3 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Access || _dbms == DBMS.MsSql)
                    {
                        //Access: "LPAD(%1,4)" -> "Right('    '+%1,4)"
                        int npad; if (!int.TryParse(p2, out npad) || npad == 0 || npad > 64) npad = 64;
                        res += string.Format("Right('{0}'+{1},{2})", new string(p3.Length == 3 && p3[0] == '\'' && p3[2] == '\'' ? p3[1] : ' ', npad), p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.SQLite)
                    {
                        //SQLite: "LPAD(%1,4)" -> "substr('    '+%1,-4)"
                        int npad; if (!int.TryParse(p2, out npad) || npad == 0 || npad > 64) npad = 64;
                        res += string.Format("substr('{0}'+{1},-{2})", new string(p3.Length == 3 && p3[0] == '\'' && p3[2] == '\'' ? p3[1] : ' ', npad), p1, p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.MySQL)
                    {
                        //MySQL: "LPAD(%1,4)" -> "LPAD(%1,4,' ')"
                        //int npad= tatoi(expr+8); ASSERT(npad<64); //16+npad<nchbuff && 
                        res += string.Format("LPAD({0},{1},{2})", p1, p2, p3);
                        ie = it + 1;
                        continue;
                    }
                    else
                    {
                        //OK for Oracle
                        //no such function for InterBase
                        res += p3 == "' '" ? string.Format("LPAD({0},{1})", p1, p2) : string.Format("LPAD({0},{1},{2})", p1, p2, p3);
                        ie = it + 1;
                        continue;
                    }
                }
                if (trouve(expr, ie, "RPAD("))
                {
                    int it = ie + 5;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p2 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    string p3 = "' '"; if (it < expr.Length && expr[it] != ')') p3 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.Access || _dbms == DBMS.MsSql)
                    {
                        //Access: "RPAD(%1,4)" -> "Left(%1+'    ',4)"
                        int npad; if (!int.TryParse(p2, out npad) || npad == 0 || npad > 64) npad = 64;
                        res += string.Format("Left({0}+'{1}',{2})", p1, new string(p3.Length == 3 && p3[0] == '\'' && p3[2] == '\'' ? p3[1] : ' ', npad), p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.SQLite)
                    {
                        //SQLite: "RPAD(%1,4)" -> "substr(%1+'    ',4)"
                        int npad; if (!int.TryParse(p2, out npad) || npad == 0 || npad > 64) npad = 64;
                        res += string.Format("substr({0}+'{1}',{2})", p1, new string(p3.Length == 3 && p3[0] == '\'' && p3[2] == '\'' ? p3[1] : ' ', npad), p2);
                        ie = it + 1;
                        continue;
                    }
                    else if (_dbms == DBMS.MySQL)
                    {
                        //MySQL: "LPAD(%1,4)" -> "LPAD(%1,4,' ')"
                        //int npad= tatoi(expr+8); ASSERT(npad<64); //16+npad<nchbuff && 
                        res += string.Format("RPAD({0},{1},{2})", p1, p2, p3);
                        ie = it + 1;
                        continue;
                    }
                    else
                    {
                        //OK for Oracle
                        //no such function for InterBase
                        res += p3 == "' '" ? string.Format("RPAD({0},{1})", p1, p2) : string.Format("RPAD({0},{1},{2})", p1, p2, p3);
                        ie = it + 1;
                        continue;
                    }
                }
                if (trouve(expr, ie, "TOSTRING("))
                {
                    int it = ie + 9;
                    string p1 = TranslateExpr(parseParam(expr, ref it), ttrans);
                    if (_dbms == DBMS.MsSql)
                    {
                        res += string.Format("CONVERT(VARCHAR(50),{0})", p1);
                        ie = it + 1;
                        continue;
                    }
                    if (_dbms == DBMS.Access)
                    {
                        res += string.Format("Ltrim(Str({0}))", p1);
                        ie = it + 1;
                        continue;
                    }
                    if (_dbms == DBMS.SQLite)
                    {
                        res += p1;
                        ie = it + 1;
                        continue;
                    }
                    else
                    {
                        //OK for Oracle
                        res += string.Format("to_char({0})", p1);
                        ie = it + 1;
                        continue;
                    }
                }
                if (expr[ie] == '\'')
                { //string...
                    int ie2 = ie + 1;
                    while (ie2 < expr.Length && expr[ie2] != '\'') ++ie2;
                    if (ie2 < expr.Length) ++ie2;
                    res += expr.Substring(ie, ie2 - ie);
                    ie = ie2;
                    continue;
                }
                if (expr[ie] == '%' && ie + 1 < expr.Length && char.IsLetter(expr[ie + 1]))
                {
                    //%TABLENAME!
                    ++ie;
                    int u0 = ie;
                    do ie++; while (ie < expr.Length && (char.IsLetterOrDigit(expr[ie]) || expr[ie] == '_'));
                    string tbName = expr.Substring(u0, ie - u0);
                    string tt = ttrans(tbName);
                    if (tt != null) res += tt;
                    else { res += "%" + tbName; }
                    continue;
                }
                if (expr[ie] == '{')
                {
                    //{columnName}
                    int ie1 = ie + 1;
                    int ie2 = expr.IndexOf('}', ie1);
                    if (ie2 > ie1)
                    {
                        res += QuoteColumn(expr.Substring(ie1, ie2 - ie1));
                        ie = ie2 + 1;
                        continue;
                    }
                    else {  res += "{"; }
                    continue;
                }
                /*		        if (*expr=='$') {
                                    //%PARAM! (custom expression inside a report...!)
                                    int vlen;
                                    GlobParam *pr= GlobalParam(expr,&vlen);
                                    if (vlen && pr) {
                                        res+= pr->GetSqlValue(this);
                                        expr+= vlen;
                                        }
                                    else {
                                        res+= *expr;
                                        ++expr;
                                        }
                                    }*/
                bool firstAlpha = char.IsLetter(expr[ie]);
                do { res += expr[ie]; ++ie; } while (firstAlpha && ie < expr.Length && char.IsLetter(expr[ie]));
            }
            return res;
        }



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
