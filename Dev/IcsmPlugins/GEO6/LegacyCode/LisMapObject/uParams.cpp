//---------------------------------------------------------------------------

#include <ComObj.hpp>
#include <memory>
#include <Registry.hpp>
#include <vector>
#include <utilcls.h>
#include <p1546_TLB.h>
#pragma hdrstop

#include "uParams.h"

const char* sAppRegPath = "SOFTWARE\\LIS\\BC";

//---------------------------------------------------------------------------

#pragma package(smart_init)

TLISBCCalcParams BCCalcParams;

void HrCheck(HRESULT hr, AnsiString errMsg)
{
    TComInterface<IErrorInfo, &IID_IErrorInfo> pErrorInfo;
    WideString desc;

    if (FAILED(hr) && (GetErrorInfo(0, &pErrorInfo) == S_OK))
    {
        if  (pErrorInfo.IsBound())
        {
            pErrorInfo->GetDescription(&desc);
            if (!desc.IsEmpty())
            {
                if (errMsg.Length() > 0)
                    errMsg += ":\n";
                errMsg += desc;
            }
        }
    throw *(new Exception(errMsg.c_str()));

    } else {
        try {
            OleCheck(hr);
        } catch (Exception &e) {
            if (errMsg.Length() > 0)
                errMsg += ":\n";
            throw *(new Exception(errMsg + e.Message));
        }
    }
}

AnsiString TLISBCCalcParams::loadServerArrays(ServParamsArray &VariantArray, AnsiString Path)
{
    AnsiString rezSvr, rezGUID="", DefaultName;


    ServParams buf;
    AnsiString ValueGUID = "{36211782-F8D1-11D6-A029-C6C3D4859862}";
    buf.name = "RSAGTOPO";//SubKeyStrings->Strings[sub];
    buf.guid = ValueGUID;
    AnsiString ValuePath = "D:\Projects\_UCRF\BC\relief\COCOTReliefData";
    buf.params = ValuePath;
    DefaultName = "RSAGTOPO - instal";
    ReliefPath = ValuePath;
    rezGUID = ValueGUID;
    ReliefServerName = "RSAGTOPO - instal";
    VariantArray.push_back(buf);
   /* std::auto_ptr<TStringList> SubKeyStrings(new TStringList);
    std::auto_ptr<TRegistry> reg(new TRegistry);

    reg->Access = KEY_READ;
    reg->RootKey = HKEY_CURRENT_USER;

    if (reg->OpenKeyReadOnly(ParamPath + "\\" + Path)) {
        reg->GetKeyNames(SubKeyStrings.get());
        DefaultName = "RSAGTOPO - instal";reg->ReadString("default");
        reg->CloseKey();

        ServParams buf;
        for (int sub = 0 ; sub < SubKeyStrings->Count; sub++) {
            if (reg->OpenKeyReadOnly(ParamPath + "\\" + Path + "\\" + SubKeyStrings->Strings[sub])) {
                AnsiString ValueGUID = "{36211782-F8D1-11D6-A029-C6C3D4859862}"//reg->ReadString("GUID");
                buf.name = "RSAGTOPO";//SubKeyStrings->Strings[sub];
                buf.guid = ValueGUID;
                if (Path == "ReliefServerArray") {
                    AnsiString ValuePath = "D:\Projects\_UCRF\BC\relief\COCOTReliefData"//reg->ReadString("Path");
                    buf.params = ValuePath;
                    if (SubKeyStrings->Strings[sub] == DefaultName)
                        ReliefPath = ValuePath;
                }
                if (SubKeyStrings->Strings[sub] == DefaultName) {
                    rezGUID = ValueGUID;
                    if (Path == "ReliefServerArray")
                        ReliefServerName = "RSAGTOPO - instal";//SubKeyStrings->Strings[sub];
                }
                VariantArray.push_back(buf);
                reg->CloseKey();
            }
        }
    }

    reg->CloseKey();
     */
    return rezGUID;
}

__fastcall TLISBCCalcParams::TLISBCCalcParams():
    minSelInterf(0.0), higherIntNum(6), degreeStep(10),
    showCp(false), treshVideo(0.3), treshAudio(0.5),
    calcMethod(0),
    standRadius(30), defArea(-1), defCity(-1), SelectionAutotruncation(true), mapAutoFit(true),
    duelAutoRecalc(false), GetCoordinatesFromBase(false), DisableReliefAtPlanning(true),
    Emin_dvb_200(38.5), Emin_dvb_500(42.6), Emin_dvb_700(46.6),
    Dvb_antenna_discrimination(true), Quick_calc_duel_interf(true), Quick_calc_max_dist(false), RequestForCoordDist(true),
    earthCurveInRelief(false),
    ShowTxNames(true),
    rpcRxModeLink(true),
    Coord_dist_ini_file("C:\\Program Files\\Lis\\BC\\dist.ini"),
    QueryOnMainormClose(true),
    filesNum(90),
    backLobeFmMono(-6.0),
    backLobeFmStereo(-12.0),
    backLobeTvBand2(-6.0),
    polarCorrectFm(-10.0),
    tvSoundStereo(false),
    stepCalcMaxDist(1),
    lineThicknessZoneCover(1),
    lineThicknessZoneNoise(1),
    lineThicknessZoneInterfere(1),
    lineColorZoneCover(clGreen),
    lineColorZoneNoise(clBlue),
    lineColorZoneInterfere(clRed),
    lineColorZoneInterfere2(clYellow),
    coordinationPointsInZoneColor(clBlue),
    coordinationPointsOutZoneColor(clRed),
    changedTxColor(0x008000FF)
    , duelType(dtTxLocation)
{
    AnsiString StringVal;   
    ParamPath = (AnsiString(sAppRegPath) + "\\CalcParams");

   /* TRegistry *reg =  new TRegistry;
    try {
        reg->Access = KEY_READ;
        reg->RootKey = HKEY_CURRENT_USER;
        if (reg->OpenKeyReadOnly(ParamPath))  {

            try {   */
                earthCurveInRelief = true;//reg->ReadBool("earthCurveInRelief");
           /* } catch (...) { ;}

        }
        reg->CloseKey();
                           

    } __finally {
        reg->Free();
    }                    */

  //  CalcServerArrayGUID = loadServerArrays(CalcServerArray, "CalcServerArray");
  //  PropagServerArrayGUID = loadServerArrays(PropagServerArray,  "PropagServerArray");
  ReliefServerArrayGUID = loadServerArrays(ReliefServerArray,  "ReliefServerArray");
}

void TLISBCCalcParams::load()
{

    if (!ReliefServerArrayGUID.IsEmpty() && ReliefServerArrayGUID != "") {

        try {
            OLECHECK(FTerrInfoSrv.CreateInstance(Comobj::StringToGUID(ReliefServerArrayGUID),  0, CLSCTX_INPROC_SERVER));
        } catch (Exception &e) {
            AnsiString msg = AnsiString("Помилка завантаження серверу рельєфу\n") + e.Message;
            Application->MessageBox(msg.c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

        try {

            if (FTerrInfoSrv.IsBound()) {
                Variant Params;

                std::vector<AnsiString> paramArray;
                AnsiString asAux(ReliefPath);
                int delimiterPos;
                while (asAux.Length() > 0) {
                    delimiterPos = asAux.Pos(";");
                    if (delimiterPos == 0) {
                        paramArray.push_back(asAux);
                        asAux = "";
                    } else {
                        paramArray.push_back(asAux.SubString(1, delimiterPos - 1));
                        asAux.Delete(1, delimiterPos);
                    }
                }

                if (paramArray.size() > 0) {
                    Params = VarArrayCreate(OPENARRAY(int, (0, paramArray.size() - 1)), varVariant);
                    for (unsigned i = 0; i < paramArray.size(); i++)
                        Params.PutElement(paramArray[i], i);
                }

                HrCheck(FTerrInfoSrv->Set_PatchCount(filesNum));
                HrCheck(FTerrInfoSrv->Init(Params));
            }



        } catch (Exception &e) {
            AnsiString msg = AnsiString("Помилка ініциалізації серверу рельєфу\n") + e.Message;
            Application->MessageBox(msg.c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

    } else {
        FTerrInfoSrv.Unbind();
       // FPathSrv.Unbind();
    }

}



__fastcall TLISBCCalcParams::~TLISBCCalcParams()
{
    //
}


