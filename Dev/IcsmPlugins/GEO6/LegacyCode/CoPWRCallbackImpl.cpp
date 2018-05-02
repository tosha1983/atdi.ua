// COPWRCALLBACKIMPL : Implementation of TCoPWRCallbackImpl (CoClass: CoPWRCallback, Interface: PWRCallback)

#include <vcl.h>
#include <Classes.hpp>
#include <Math.hpp>

#include <memory>
#pragma hdrstop

#include "COPWRCALLBACKIMPL.H"
#include "uMainForm.h"
#include "uMainDm.h"
#pragma link "PiFolio_OCX"

#ifdef StrToInt
#undef StrToInt
#endif

extern TCOMIWordReporter4 WordReporter;

//------------------------------------------------------------------------------

TCoPWRCallbackImpl::TCoPWRCallbackImpl()
{
    status = 0;
}
//------------------------------------------------------------------------------

TCoPWRCallbackImpl::~TCoPWRCallbackImpl()
{
    if ( !arrays.empty() )
        for ( std::map<AnsiString, double*>::iterator array = arrays.begin(); array != arrays.end(); array++ )
            if ( array->second )
                delete[] (array->second);
}
//------------------------------------------------------------------------------

void __fastcall TCoPWRCallbackImpl::FillArra(double** _array, int transmitterId, AnsiString fieldName)
{
    double *array = new double[36]; ZeroMemory(array, 36);

try
{
    TIBQuery *query = new TIBQuery(NULL);
      query->Database = dmMain->dbMain;
      query->SQL->Text = "SELECT " + fieldName + " FROM Transmitters WHERE ( id = :id )";
      query->ParamByName("id")->Value = transmitterId;

    query->Open();

    if ( !query->Eof )
    {
        TStream *stream = query->CreateBlobStream(query->Fields->FieldByName(fieldName), bmRead);

        stream->ReadBuffer(array, stream->Size);

    }
}
catch(...)
{
}

    *_array = array;
}
//------------------------------------------------------------------------------

AnsiString __fastcall TCoPWRCallbackImpl::NumToStr(double num, int numCount, int afterPoint)
{
    if(afterPoint > 0)
        ++numCount;
    AnsiString res,parStr = "%" + IntToStr(numCount ) + "." + IntToStr(afterPoint) + "f";
    res.sprintf(parStr.c_str(), num);
    if(afterPoint > 0)
    {
        int n = res.Pos(".");
        res.Delete(n,1);
        res.Insert(" ",n);
    }
    return res;
}

//------------------------------------------------------------------------------


STDMETHODIMP TCoPWRCallbackImpl::ReplaceBookmark(BSTR Bookmark, OLE_CANCELBOOL* Result)
{
    AnsiString bookmark;
    {
        // извращенец бл€. нельз€ было просто WideString использовать? - sk

        int size = ::WideCharToMultiByte(CP_ACP, 0, Bookmark, -1, 0, 0, 0, 0);
        bookmark.SetLength(size);
        size = ::WideCharToMultiByte(CP_ACP, 0, Bookmark, -1, &(bookmark[1]), size, 0, 0);

        bookmark = bookmark.Trim().LowerCase();
    }

    if ( bookmark.Pos('}') != 0 )
    {
        if ( bookmark[bookmark.Length()] != '}' )
            bookmark += '}';

        AnsiString str;

        int pos = 0;
        while ( ( pos = bookmark.Pos('}') ) != 0 )
        {
            int pos2 = pos;
            while ( (pos2 > 1) && (bookmark[pos2] != ' ') )
                pos2--;
            if ( bookmark[pos2] == ' ' )
                pos2++;

            str += bookmark.SubString(pos2, pos - pos2);
            bookmark.Delete(1, pos);
        }

        bookmark = str;

        if ( ( pos = bookmark.Pos('{') ) != 0 )
            bookmark.Delete(pos, bookmark.Length() - pos + 1);
    }

    if ( bookmark.IsEmpty() )
    {
        *Result = TVariant(true);
        return S_OK;
    }

    bool found = false;
    AnsiString value;

    bool to3 = false;

    if ( bookmark[1] == '_' )
    {
        to3 = true;
        bookmark.Delete(1, 1);
        if (bookmark == "createdate")
        {
            AnsiString temp = (Date()).DateString();
            temp.Delete(3,1);
            temp.Delete(5,1);
            value = temp;
            found = true;
        }
    }

    if ( bookmark[1] == '#' )
    {
        int pos = bookmark.Pos("-");
        if ( pos > 0 )
        {
            AnsiString arrayName = bookmark.SubString(2, pos - 2);
            int arrayIndex = StrToInt(bookmark.SubString(pos + 1, bookmark.Length() - pos));

            if ( !arrays[arrayName] )
                FillArra(&(arrays[arrayName]), query->Fields->FieldByName("id")->AsInteger, arrayName);

            value = FormatFloat("0.###", RoundTo(arrays[arrayName][arrayIndex], -3));

            found = true;
        }
    }
    else if(bookmark[1] == '^')
    {
        bookmark.Delete(1, 1);
        value = GetDNParam(bookmark);
        found = true;
    }
    else
    {

        for ( int i = 0; i < query->FieldCount ; i++ )
            if ( bookmark == query->Fields->Fields[i]->FieldName.LowerCase() )
            {
                if ( bookmark == "direction" )
                {
                    if ( query->Fields->Fields[i]->AsString == "D" )
                        value = "—";
                    else
                        value = "Ќ—";
                }
                if ( bookmark == "direction2" )
                {
                    value = query->Fields->Fields[i]->AsString.SubString(1, 1);
                }
                else if ( bookmark == "latitude" )
                {
                    if(to3)
                    {
                        AnsiString val, temp = dmMain->coordToStr(query->Fields->Fields[i]->AsFloat, 'Y');
                        val = temp.SubString(1,2) + temp.SubString(6,2) + temp.SubString(10,2) + temp.SubString(4,1);
                        value = val;
                    }
                    else
                        value = dmMain->coordToStr(query->Fields->Fields[i]->AsFloat, 'Y');
                }
                else if( bookmark == "identifiersfn")
                {       
                    if(to3)
                    {
                        TIBQuery *query_sfnid = new TIBQuery(NULL);
                        query_sfnid->Database = dmMain->dbMain;
                        query_sfnid->SQL->Text = "SELECT SYNHRONETID FROM SYNHROFREQNET WHERE id = :id ";
                        query_sfnid->ParamByName("id")->Value = query->Fields->Fields[i]->AsInteger;
                        query_sfnid->Open();
                        value = query_sfnid->FieldByName("SYNHRONETID")->AsString;
                        delete query_sfnid;
                    }
                }
                else if ( bookmark == "longitude" )
                {
                    if(to3)
                    {
                        AnsiString val, temp = dmMain->coordToStr(query->Fields->Fields[i]->AsFloat, 'X');
                        if(isdigit(temp[3]))
                            val = temp.SubString(1,3) + temp.SubString(7,2) + temp.SubString(11,2) + temp.SubString(5,1);
                        else
                            val = " " + temp.SubString(1,2) + temp.SubString(6,2) + temp.SubString(10,2) + temp.SubString(4,1);
                        value = val;
                    }
                    else
                        value = dmMain->coordToStr(query->Fields->Fields[i]->AsFloat, 'X');
                }
                else if ( bookmark == "monostereo_primary" )
                {
                    if ( query->Fields->Fields[i]->AsInteger )
                        value = "стерео";
                    else
                        value = "моно";
                }
                else if ( bookmark == "polarization" )
                {
                    if ( query->Fields->Fields[i]->AsString == "H" )
                          value = "√";
                    else if ( query->Fields->Fields[i]->AsString == "V" )
                          value = "¬";
                    else
                        value = "«";
                }
                else if ( bookmark == "typeoffset2" )
                {
                    value = query->Fields->Fields[i]->AsString.SubString(1, 1);
                }
                else if ( bookmark == "status" )
                {
                    if ( query->Fields->Fields[i]->AsInteger == 0 )
                        value = "Ѕаза";
                    else if ( query->Fields->Fields[i]->AsInteger == 1 )
                        value = "ѕередбаза";
                    else if ( query->Fields->Fields[i]->AsInteger == -1 )
                        value = "¬идалений";
                }
                else if ( bookmark == "summatorpowers" )
                {
                    if ( query->Fields->Fields[i]->AsInteger )
                        value = "“ак";
                    else
                        value = "Ќ≥";
                }
                else if ( bookmark.UpperCase() == "POWER_VIDEO" )
                    //выводитс€ в к¬т, а надо в ¬т
                    value = FormatFloat("0.###", query->Fields->Fields[i]->AsFloat * 1000.0);
                else if ( bookmark.UpperCase() == "POWER_SOUND_PRIMARY" )
                    //выводитс€ в к¬т, а надо в ¬т
                    value = FormatFloat("0.###", query->Fields->Fields[i]->AsFloat * 1000.0);
                else if ( bookmark.UpperCase() == "VIDEO_OFFSET_HERZ" )
                    //выводитс€ в √ц, а надо в к√ц
                    value = FormatFloat("0.###", query->Fields->Fields[i]->AsFloat / 1000.0);
                else if ( bookmark.UpperCase() == "EPR_VIDEO_MAX" )
                    //выводитс€ в дЅк¬т, а надо в дЅ¬т
                    value = FormatFloat("0.###", query->Fields->Fields[i]->AsFloat + 3);
                else if (to3 && bookmark == "dateintenduse")
                {
                    AnsiString temp = query->Fields->Fields[i]->AsString;
                    temp.Delete(3,1);
                    temp.Delete(5,1);
                    value = temp;
                }
                else if (to3 && bookmark == "sound_carrier_primary")
                {
                    AnsiString val = NumToStr(query->Fields->Fields[i]->AsFloat * 1000., 5, 1);
                    value = val;
                }
                else if (to3 && bookmark == "gnd_cond")
                {
                    AnsiString val = NumToStr(query->Fields->Fields[i]->AsFloat, 6, 2);
                    value = val;
                }
                else if ( query->Fields->Fields[i]->DataType == ftFloat )
                {
                    if ( query->Fields->Fields[i]->AsFloat < -300 )
                    {
                        if ( (bookmark == "EPR_SOUND_HOR_PRIMARY_DBW") || (bookmark == "EPR_SOUND_VERT_PRIMARY_DBW") )
                            value = "0";
                        else
                           value = "";
                    }
                    else if ( bookmark.SubString(1,3)=="EPR" )
                        value = FormatFloat("0.###", query->Fields->Fields[i]->AsFloat);
                    else
                        value = FormatFloat("0.###", query->Fields->Fields[i]->AsFloat);
                }
                else
                {
                    value = query->Fields->Fields[i]->AsString;
                }

                found = true;
                break;
            }
    }
    
    if(!found && to3 && (bookmark.SubString(1,2) == "hj" || bookmark.SubString(1,2) == "hn"))
    {

        AnsiString lfmf = bookmark.SubString(1,2);
        bookmark.Delete(1,2);
        int mas_index;
        if(bookmark == "")
            bookmark = "daynight";
        else if(bookmark.SubString(1,8) == "gain_azm")
        {
            mas_index = StrToInt(bookmark.SubString(9,2));
            bookmark = bookmark.SubString(1,8);
        }
        TIBQuery *query_lfmf = new TIBQuery(NULL);
        query_lfmf->Database = dmMain->dbMain;
        query_lfmf->SQL->Text = "SELECT * FROM LFMF_OPER WHERE ( sta_id = :id ) and (DAYNIGHT = :daynight)";
        query_lfmf->ParamByName("id")->Value = query->Fields->FieldByName("id")->AsInteger;
        query_lfmf->ParamByName("daynight")->Value = lfmf.UpperCase();
        query_lfmf->Open();
        if ( !query_lfmf->Eof )
        {
            for ( int i = 0; i < query_lfmf->FieldCount ; i++ )
                if ( bookmark == query_lfmf->Fields->Fields[i]->FieldName.LowerCase() )
                {
                    if(bookmark == "daynight")
                    {
                        value = lfmf;
                    }
                    else if(bookmark == "pwr_kw")
                    {
                        AnsiString val = NumToStr(query_lfmf->Fields->Fields[i]->AsFloat, 6, 2);
                        value = val;
                    }
                    else if(bookmark == "e_max")
                    {
                        AnsiString val = NumToStr(query_lfmf->Fields->Fields[i]->AsFloat, 4, 1);
                        value = val;
                    }
                    else if(bookmark == "bdwdth")
                    {
                        value = NumToStr(query_lfmf->Fields->Fields[i]->AsFloat, 2, 0);
                    }
                    else if(bookmark == "agl")
                    {
                        value = NumToStr(query_lfmf->Fields->Fields[i]->AsFloat, 3, 0);
                    }
                    else if(bookmark == "gain_azm")
                    {
                        double *array = new double[36];
                        ZeroMemory(array, 36);
                        try
                        {
                            TStream *stream = query_lfmf->CreateBlobStream(query_lfmf->Fields->Fields[i], bmRead);
                            stream->ReadBuffer(array, stream->Size);
                        }
                        catch(...)
                        {
                        }
                        value = NumToStr(array[mas_index], 3, 1);
                        found = true;
                        delete []array;
                        break;
                    }
                    else
                    {
                        value = query_lfmf->Fields->Fields[i]->AsString;
                    }
                    found = true;
                    break;
                }
            if(!found && bookmark == "time")
            {
                AnsiString temp = FormatFloat("00.00", query_lfmf->FieldByName("START_TIME")->AsFloat) + FormatFloat("00.00", query_lfmf->FieldByName("STOP_TIME")->AsFloat);
                temp.Delete(3,1);
                temp.Delete(7,1);
                value = temp;
                found = true;
            }

        }
        else
        {
            value = "";
            found = true;
        }
        delete query_lfmf;
    }

    if ( found )
    {
        int size = ::MultiByteToWideChar(CP_ACP, 0, value.c_str(), -1, 0, 0);
        LPWSTR dst = new wchar_t[size];
        size = ::MultiByteToWideChar(CP_ACP, 0, value.c_str(), -1, dst, size);

        WordReporter.WriteField(dst, PWR_VALUE_Text, NULL);

        *Result = TVariant(false);
    }
    else
        *Result = TVariant(true);

    return S_OK;
}
//------------------------------------------------------------------------------

STDMETHODIMP TCoPWRCallbackImpl::SetCallback(Pifolio_tlb::PWRCallBackAction CurrentPoint, OLE_CANCELBOOL* Result)
{
    *Result = TVariant(false);
    return S_OK;
}
//------------------------------------------------------------------------------

STDMETHODIMP TCoPWRCallbackImpl::SetStatus(BSTR StatusMsg)
{
    if ( status == 0 )
    {
        query = (TIBQuery*)StatusMsg;
        status++;
    }

    frmMain->StatusBar1->SimpleText = WideString(StatusMsg);
    frmMain->StatusBar1->Repaint();

    return S_OK;
}
//------------------------------------------------------------------------------

AnsiString __fastcall TCoPWRCallbackImpl::GetDNParam(AnsiString bookmark)
{
     AnsiString lfmf = bookmark.SubString(1,2);
     int mas_index;
     if(bookmark.SubString(1,8) == "gain_azm")
     {
        mas_index = StrToInt(bookmark.SubString(9,2));
        bookmark = bookmark.SubString(1,8);
     }
     TIBQuery *query_lfmf = new TIBQuery(NULL);
     query_lfmf->Database = dmMain->dbMain;

     query_lfmf->SQL->Text = "SELECT * FROM LFMF_OPER WHERE ( sta_id = :id ) and (DAYNIGHT = :DN)";
     query_lfmf->ParamByName("id")->Value = query->Fields->FieldByName("id")->AsInteger;
     query_lfmf->ParamByName("DN")->AsString = "HJ";
     query_lfmf->Open();
     if(query_lfmf->Eof)
     {
        query_lfmf->Close();
        query_lfmf->ParamByName("DN")->AsString = "HN";
        query_lfmf->Open();
        if(query_lfmf->Eof)
        {
            throw *(new Exception("” передавача немаЇ жодного режиму роботи"));
        }
     }
     AnsiString value;
     bool found = false;
     for ( int i = 0; i < query_lfmf->FieldCount ; i++ )
        if ( bookmark == query_lfmf->Fields->Fields[i]->FieldName.LowerCase() )
        {
            if(bookmark == "daynight")
            {
                value = lfmf;
            }
            else if(bookmark == "pwr_kw")
            {
                float temp = query_lfmf->Fields->Fields[i]->AsFloat;
                if(temp == 0)
                    value = "0";
                else
                    value = FloatToStr(temp);
            }
            else if(bookmark == "e_max")
            {
                
                float temp = query_lfmf->Fields->Fields[i]->AsFloat;
                if(temp == 0)
                    value = "0";
                else
                    value = FloatToStr(temp);
            }
            else if(bookmark == "bdwdth")
            {
                float temp = query_lfmf->Fields->Fields[i]->AsFloat;
                if(temp == 0)
                    value = "0";
                else
                    value = FloatToStr(temp);
            }
            else if(bookmark == "agl")
            {
                float temp = query_lfmf->Fields->Fields[i]->AsFloat;
                if(temp == 0)
                    value = "0";
                else
                    value = FloatToStr(temp);
            }
            else if(bookmark == "gain_azm")
            {
                double *array = new double[36];
                ZeroMemory(array, 36);
                try
                {
                    TStream *stream = query_lfmf->CreateBlobStream(query_lfmf->Fields->Fields[i], bmRead);
                    stream->ReadBuffer(array, stream->Size);
                }
                catch(...)
                {
                }
                float temp = array[mas_index];
                if(temp == 0)
                    value = "0";
                else
                    value = FloatToStr(temp);
                found = true;
                delete []array;
                break;
            }
            else
            {
                value = query_lfmf->Fields->Fields[i]->AsString;
            }
            found = true;
            break;
        }
     if(!found && bookmark == "time")
     {
        AnsiString temp = FormatFloat("0.00", query_lfmf->FieldByName("START_TIME")->AsFloat) + FormatFloat("00.00", query_lfmf->FieldByName("STOP_TIME")->AsFloat);
        temp.Delete(3,1);
        temp.Delete(7,1);
        value = temp;
        found = true;
     }
     delete query_lfmf;
     if ( found )
        return value;
     else
        return "";
}
//------------------------------------------------------------------------------
