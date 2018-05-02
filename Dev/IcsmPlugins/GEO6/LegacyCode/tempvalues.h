#ifndef TempvaluesH
#define TempvaluesH

#include <Forms.hpp>
#include <ComCtrls.hpp>

class TempCursor {
private:
    TCursor oldCursor;
    bool loaded;
public:
    void SetCursor(TCursor newOne)
    {
        if (!loaded)
            oldCursor = Screen->Cursor;
        loaded = true;
        Screen->Cursor = newOne;
    }
    void Reset ()
    {
        if (loaded)
            Screen->Cursor = oldCursor;
        loaded = false;
    }

    TempCursor(): loaded(false) {}
    TempCursor(TCursor c): loaded(false)
    {
        SetCursor(c);
    }

    ~TempCursor()
    {
        Reset();
    }
};

/*
  class TempVal
  шаблон, используется для автоматического восстановления значения любого типа
  запоминает ссылку на объект/переменную и его/её старое значение

  пример использования:
  TempVal<char> tempDecSep(DecimalSeparator, '.');
  
*/

template <class T>
class TempVal {
private:
    T& cref;
    T oldVal;
    bool loaded;
public:
    void SetVal(T newOne)
    {
        if (!loaded)
            oldVal = cref;
        loaded = true;
        cref = newOne;
    }
    void Reset ()
    {
        if (loaded)
            cref = oldVal;
        loaded = false;
    }

    TempVal(T& c): loaded(false), cref(c) {};
    TempVal(T& c, T cVal): loaded(false), cref(c)
    {
        SetVal(cVal);
    }

    ~TempVal()
    {
        Reset();
    }
};

class TempStatusString {
private:
    TStatusBar *sb;
    bool oldsp;
    AnsiString oldtext;
    bool loaded;
public:
    void SetString(TStatusBar* b, AnsiString s)
    {
        if (!loaded)
        {
            sb = b;
            oldsp = sb->SimplePanel;
            sb->SimplePanel = true;
            oldtext = sb->SimpleText;
            sb->SimpleText = s;
            loaded = true;
        }
    }
    void Reset ()
    {
        if (loaded)
        {
            sb->SimpleText = oldtext;
            sb->SimplePanel = oldsp;
            loaded = false;
        }
    }

    TempStatusString(): loaded(false) {}
    TempStatusString(TStatusBar* b, AnsiString s): loaded(false)
    {
        SetString(b, s);
    }

    ~TempStatusString()
    {
        Reset();
    }
};


#endif
