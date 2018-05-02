//---------------------------------------------------------------------------

#ifndef uLoggerH
#define uLoggerH

#include <Classes.hpp>
#include <memory>
//---------------------------------------------------------------------------

class Logger
{
private:
    std::auto_ptr<TStringList> sl;
protected:

public:
    bool log_time;
  __fastcall Logger();
    void __fastcall LogEvent(String msg);
    void __fastcall Clear();
    void __fastcall SaveToFile(String filename);
    void __fastcall CopyTo(TStrings* strings);
    String __fastcall GetText();
};


#endif
