//---------------------------------------------------------------------------


#pragma hdrstop

#include "uLogger.h"
#include <fstream>
//---------------------------------------------------------------------------

#pragma package(smart_init)
__fastcall Logger::Logger():
    sl(new TStringList), log_time(true)
{
}

void __fastcall Logger::LogEvent(String msg)
{
    sl->Add((log_time ? FormatDateTime("hh:mm:ss:zzz' - '", Now()) : String()) + msg);
}

void __fastcall Logger::Clear()
{
    sl->Clear();
}

void __fastcall Logger::SaveToFile(String filename)
{
    std::ofstream os(filename.c_str(), std::ios_base::ate);
    if (os.bad())
        throw *(new Exception("Проблема записи журнала в файл '" + filename + '\''));

    os << sl->Text.c_str() << '\n';

    if (os.fail())
        throw *(new Exception("Проблема записи журнала в файл '" + filename + '\''));
}

void __fastcall Logger::CopyTo(TStrings* strings)
{
    strings->Clear();
    strings->AddStrings(sl.get());
}

String __fastcall Logger::GetText()
{
    return sl->Text;
}
