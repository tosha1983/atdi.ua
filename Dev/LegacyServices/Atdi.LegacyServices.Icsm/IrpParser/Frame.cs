using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Atdi.LegacyServices.Icsm
{
    public class Frame
    {
        public Frame() { fRoot = fLast = null; }
        public Frame(frameobject f) { fRoot = fLast = f; }
        public static implicit operator frameobject(Frame ff) { return ff.fRoot; }

        public void Add(string prop, string value) { add(prop, new frameString(value)); }
        public void Add(string prop, int value) { add(prop, new frameInt(value)); }
        public void Add(string prop, double value) { add(prop, new frameDouble(value)); }
        public void Add(string prop, frameobject value) { add(prop, new frameFrame(value)); }
        public void Add(string prop, DateTime value) { add(prop, new frameDate(value)); }

        public void Set(string prop, string value) { set(prop, new frameString(value)); }
        public void Set(string prop, int value) { set(prop, new frameInt(value)); }
        public void Set(string prop, double value) { set(prop, new frameDouble(value)); }
        public void Set(string prop, frameobject value) { set(prop, new frameFrame(value)); }
        public void Set(string prop, DateTime value) { set(prop, new frameDate(value)); }

        public bool Get(string prop, out string value)
        {
            value = null;
            frameobject p = get(prop);
            if (p == null) return false;
            if (p is frameString) { value = ((frameString)p).value; return true; }
            return false;
        }
        public bool Get(string prop, out bool value)
        {
            int val; bool found = Get(prop, out val);
            value = val != 0 && val != 0x7FFFFFFF;
            return found;
        }
        public bool Get(string prop, out int value)
        {
            value = 0x7FFFFFFF;
            frameobject p = get(prop);
            if (p == null) return false;
            if (p is frameInt) { value = ((frameInt)p).value; return true; }
            return false;
        }
        public bool Get(string prop, out double value)
        {
            value = 1e-99;
            frameobject p = get(prop);
            if (p == null) return false;
            if (p is frameDouble) { value = ((frameDouble)p).value; return true; }
            if (p is frameInt) { int n = ((frameInt)p).value; if (n != 0x7FFFFFFF) value = n; return true; }
            return false;
        }
        public bool Get(string prop, out Frame value)
        {
            value = null;
            frameobject p = get(prop);
            if (p == null) return false;
            if (p is frameFrame) { value = new Frame(((frameFrame)p).value); return true; }
            return false;
        }
        public bool Get(string prop, out DateTime value)
        {
            value = new DateTime();
            frameobject p = get(prop);
            if (p == null) return false;
            if (p is frameDate) { value = ((frameDate)p).value; return true; }
            return false;
        }

        private frameobject get(string prop)
        {
            frameobject e;
            for (e = fLast; e != null; e = e.next) if (e.prop == prop) return fLast = e;
            for (e = fRoot; e != null && e != fLast; e = e.next) if (e.prop == prop) return fLast = e;
            return null;
        }

        private void add(string prop, frameobject p)
        {
            p.next = null;
            p.prop = prop;
            if (fRoot == null) fRoot = p;
            else
            {
                while (fLast.next != null) fLast = fLast.next;
                fLast.next = p;
            }
            fLast = p;
        }
        private void set(string prop, frameobject p)
        {
            p.prop = prop;
            if (fLast != null)
            {
                frameobject e = fLast;
                while (e.next != null)
                {
                    frameobject ee = e.next;
                    if (ee.prop == prop) { p.next = ee.next; e.next = p; fLast = e; return; }
                    e = ee;
                }
            }
            if (fRoot != null)
            {
                frameobject e = fRoot;
                if (e.prop == prop) { p.next = e.next; fLast = fRoot = p; return; }
                while (e.next != null && e != fLast)
                {
                    frameobject ee = e.next;
                    if (ee.prop == prop) { p.next = ee.next; e.next = p; fLast = e; return; }
                    e = ee;
                }
            }
            //have to add it:
            if (fRoot == null) fLast = fRoot = p;
            else
            {
                while (fLast.next != null) fLast = fLast.next;
                fLast.next = p;
            }
        }

        private frameobject fRoot;
        private frameobject fLast;

        public void Load(InChannel c)
        {
            string id = null;
            c.ReadToken();
            if (c.tokType != type.tChr || c.tokC != '{') c.Wanted("'{'");
            for (; ; )
            {
                c.ReadToken();
                if (c.tokType == type.tInt) { id = c.tokI.ToString(); if (c.tokI < 0) c.Wanted("positive int"); }
                else if (c.tokType == type.tSym) { id = c.tokB; }
                else if (c.tokType == type.tChr && c.tokC == '}') break;
                else c.Wanted("property or '}'");
                c.ReadToken();
                if (c.tokType != type.tChr || c.tokC != '=') c.Wanted("=");
                c.ReadToken();
                switch (c.tokType)
                {
                    case type.tSym: Add(id, c.tokB); break;
                    case type.tStr: Add(id, c.tokS); break;
                    case type.tInt: Add(id, c.tokI); break;
                    case type.tTim: Add(id, c.tokT); break;
                    case type.tDou: Add(id, c.tokD); break;
                    case type.tChr:
                        {
                            if (c.tokC != '{') c.Wanted("value");
                            c.UnreadToken();
                            Frame f2 = new Frame();
                            f2.Load(c);
                            Add(id, f2);
                        }
                        break;
                    default: Debug.Assert(false); break;
                }
            }
        }

    }
}
