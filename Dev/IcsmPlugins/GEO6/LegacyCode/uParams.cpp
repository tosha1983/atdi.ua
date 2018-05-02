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

    std::auto_ptr<TStringList> SubKeyStrings(new TStringList);
    std::auto_ptr<TRegistry> reg(new TRegistry);

    reg->Access = KEY_READ;
    reg->RootKey = HKEY_CURRENT_USER;

    if (reg->OpenKeyReadOnly(ParamPath + "\\" + Path)) {
        reg->GetKeyNames(SubKeyStrings.get());
        DefaultName = reg->ReadString("default");
        reg->CloseKey();

        ServParams buf;
        for (int sub = 0 ; sub < SubKeyStrings->Count; sub++) {
            if (reg->OpenKeyReadOnly(ParamPath + "\\" + Path + "\\" + SubKeyStrings->Strings[sub])) {
                AnsiString ValueGUID = reg->ReadString("GUID");
                buf.name = SubKeyStrings->Strings[sub];
                buf.guid = ValueGUID;
                if (Path == "ReliefServerArray") {
                    AnsiString ValuePath = reg->ReadString("Path");
                    buf.params = ValuePath;
                    if (SubKeyStrings->Strings[sub] == DefaultName)
                        ReliefPath = ValuePath;
                }
                if (SubKeyStrings->Strings[sub] == DefaultName) {
                    rezGUID = ValueGUID;
                    if (Path == "ReliefServerArray")
                        ReliefServerName = SubKeyStrings->Strings[sub];
                    else if (Path == "CalcServerArray")
                        CalcServerName = SubKeyStrings->Strings[sub];
                    else if (Path == "PropagServerArray")
                        PropagServerName = SubKeyStrings->Strings[sub];
                }
                VariantArray.push_back(buf);
                reg->CloseKey();
            }
        }
    }

    reg->CloseKey();

    return rezGUID;
}

void TLISBCCalcParams::saveServerArrays(ServParamsArray &VariantArray, AnsiString Path, AnsiString DefServName)
{
    std::auto_ptr<TRegistry> reg(new TRegistry);
    std::auto_ptr<TRegistry> subreg(new TRegistry);
    std::auto_ptr<TStringList> SubKeyStrings(new TStringList);

    reg->Access = KEY_ALL_ACCESS;
    reg->RootKey = HKEY_CURRENT_USER;
    subreg->RootKey = HKEY_CURRENT_USER;

    if (reg->OpenKey(ParamPath + "\\" + Path, true)) {

        reg->WriteString("default", DefServName);

        reg->GetKeyNames(SubKeyStrings.get());
        for (int sub = 0 ; sub < SubKeyStrings->Count; sub++)
            reg->DeleteKey(SubKeyStrings->Strings[sub]);

        for (unsigned sub = 0 ; sub < VariantArray.size(); sub++)
        {
            if (subreg->OpenKey(ParamPath + "\\" + Path + "\\" + VariantArray[sub].name, true))
            {
                subreg->WriteString("GUID", VariantArray[sub].guid);
                if (Path == "ReliefServerArray")
                    subreg->WriteString("Path", VariantArray[sub].params);
                subreg->CloseKey();
            }
        }
        reg->CloseKey();
    }
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
    , doMapInitDelay(true)
    , mapInitDelay(1)
    , doMapInitInfo(true)
{
    AnsiString StringVal;   
    ParamPath = (AnsiString(sAppRegPath) + "\\CalcParams");

    TRegistry *reg =  new TRegistry;
    try {
        reg->Access = KEY_READ;
        reg->RootKey = HKEY_CURRENT_USER;
        if (reg->OpenKeyReadOnly(ParamPath))  {
            try {
                UseHeff = reg->ReadInteger("UseHeff");
            } catch (...) {;}
            try {
                UseTxClearence = reg->ReadInteger("UseTxClearance");
            } catch (...) { ;}
            try {
                UseRxClearence = reg->ReadInteger("UseRxClearance");
            } catch (...) { ;}
            try {
                UseMorfology = reg->ReadInteger("UseMorfology");
            } catch (...) { ;}
            try {
                Step = reg->ReadFloat("Step");
            } catch (...) { Step = 1;}
            try {
                UseHeffTheo = reg->ReadInteger("UseHeffTheo");
            } catch (...) {;}
            try {
                UseTxClearenceTheo = reg->ReadInteger("UseTxClearanceTheo");
            } catch (...) { ;}
            try {
                UseRxClearenceTheo = reg->ReadInteger("UseRxClearanceTheo");
            } catch (...) { ;}
            try {
                UseMorfologyTheo = reg->ReadInteger("UseMorfologyTheo");
            } catch (...) { ;}
            try {
                StepTheo = reg->ReadFloat("StepTheo");
            } catch (...) { StepTheo = 1;}
            try {
                TheoPathTheSame = reg->ReadBool("TheoPathTheSame");
            } catch (...) { ;}
            try {
                minSelInterf = reg->ReadFloat("minSelInterf");
            } catch (...) { ;}
            try {
                higherIntNum = reg->ReadInteger("higherIntNum");
            } catch (...) { ;}
            try {
                calcMethod = reg->ReadInteger("calcMethod");
            } catch (...) { ;}
            try {
                standRadius = reg->ReadInteger("standRadius");
            } catch (...) { ;}
            try {
                defArea = reg->ReadInteger("defArea");
            } catch (...) { ;}
            try {
                defCity = reg->ReadInteger("defCity");
            } catch (...) { ;}
            try {
                SelectionAutotruncation = reg->ReadBool("SelectionAutotruncation");
            } catch (...) { ;}
            try {
                mapAutoFit = reg->ReadBool("mapAutoFit");
            } catch (...) { ;}
            try {
                rpcRxModeLink = reg->ReadBool("rpcRxModeLink");
            } catch (...) { ;}
            try {
                duelAutoRecalc = reg->ReadBool("duelAutoRecalc");
            } catch (...) { ;}
            try {
                duelType = reg->ReadInteger("duelType");
            } catch (...) { ;}
            try {
                //from 0.1.25.57 use only Coordination Distancies calculated in LisBcCalc
                //GetCoordinatesFromBase = reg->ReadBool("GetCoordinatesFromBase");
            } catch (...) { ;}
            try {
                DisableReliefAtPlanning = reg->ReadBool("DisableReliefAtPlanning");
            } catch (...) { ;}
            try {
                earthCurveInRelief = reg->ReadBool("earthCurveInRelief");
            } catch (...) { ;}
            try {
                ShowTxNames = reg->ReadBool("ShowTxNames");
            } catch (...) { ;}
            try {
                filesNum = reg->ReadInteger("filesNum");
            } catch (...) { ;}
            try {
                lineThicknessZoneCover = reg->ReadInteger("lineThicknessZoneCover");
            } catch (...) { ;}
            try {
                lineThicknessZoneNoise = reg->ReadInteger("lineThicknessZoneNoise");
            } catch (...) { ;}
            try {
                lineThicknessZoneInterfere = reg->ReadInteger("lineThicknessZoneInterfere");
            } catch (...) { ;}
            try {
                lineColorZoneCover = reg->ReadInteger("lineColorZoneCover");
            } catch (...) { ;}
            try {
                lineColorZoneNoise = reg->ReadInteger("lineColorZoneNoise");
            } catch (...) { ;}
            try {
                lineColorZoneInterfere = reg->ReadInteger("lineColorZoneInterfere");
            } catch (...) { ;}
            try {
                lineColorZoneInterfere2 = reg->ReadInteger("lineColorZoneInterfere2");
            } catch (...) { ;}
            try {
                coordinationPointsInZoneColor = reg->ReadInteger("coordinationPointsInZoneColor");
            } catch (...) { ;}
            try {
                coordinationPointsOutZoneColor = reg->ReadInteger("coordinationPointsOutZoneColor");
            } catch (...) { ;}
            try {
                changedTxColor = reg->ReadInteger("changedTxColor");
            } catch (...) { ;}
            try {
                QueryOnMainormClose = reg->ReadBool("QueryOnMainormClose");
            } catch (...) { ;}

            try {
                doMapInitDelay = reg->ReadBool("doMapInitDelay");
            } catch (...) { ;}
            try {
                mapInitDelay = reg->ReadInteger("mapInitDelay");
                if (mapInitDelay < 0) mapInitDelay = 0;
            } catch (...) { ;}
            try {
                doMapInitInfo = reg->ReadBool("doMapInitInfo");
            } catch (...) { ;}


        }
        reg->CloseKey();

        reg->Access = KEY_READ;
        reg->RootKey = HKEY_CURRENT_USER;

        if (reg->OpenKeyReadOnly("Software\\LIS\\LISBCCalc"))  {
            try {
                Emin_dvb_200 = reg->ReadFloat("_emin_dvb_200");
            } catch (...) { ;}
            try {
                Emin_dvb_500 = reg->ReadFloat("_emin_dvb_500");
            } catch (...) { ;}
            try {
                Emin_dvb_700 = reg->ReadFloat("_emin_dvb_700");
            } catch (...) { ;}
            /*try {
                Dvb_antenna_discrimination = reg->ReadBool("_dvb_antenna_discrimination");
            } catch (...) { ;} */
            try {
                Quick_calc_duel_interf = reg->ReadBool("_quick_calc_duel_interf");
            } catch (...) { ;}
            try {
                Quick_calc_max_dist = reg->ReadBool("_quick_calc_max_dist");
            } catch (...) { ;}
            try {
                RequestForCoordDist = reg->ReadBool("_request_for_coord_dist");
            } catch (...) { ;}
            try {
                Coord_dist_ini_file = reg->ReadString("_coord_dist_ini_file");
            } catch (...) { ;}
            try {
                backLobeFmMono = reg->ReadFloat("_back_lobe_fm_mono");
            } catch (...) { ;}
            try {
                backLobeFmStereo = reg->ReadFloat("_back_lobe_fm_stereo");
            } catch (...) { ;}
            try {
                backLobeTvBand2 = reg->ReadFloat("_back_lobe_tv_band2");
            } catch (...) { ;}
            try {
                polarCorrectFm = reg->ReadFloat("_polar_correct_fm");
            } catch (...) { ;}
            try {
                tvSoundStereo = reg->ReadBool("_tv_sound_stereo");
            } catch (...) { ;}
            try {
                stepCalcMaxDist = reg->ReadFloat("_step_calc_max_dist");
            } catch (...) { ;}

        }
        reg->CloseKey();
    } __finally {
        reg->Free();
    }

    CalcServerArrayGUID = loadServerArrays(CalcServerArray, "CalcServerArray");
    PropagServerArrayGUID = loadServerArrays(PropagServerArray,  "PropagServerArray");
    ReliefServerArrayGUID = loadServerArrays(ReliefServerArray,  "ReliefServerArray");
}

void TLISBCCalcParams::load()
{
    try {
        if (CalcServerArrayGUID != "")
            OLECHECK(FCalcSrv.CreateInstance(Comobj::StringToGUID(CalcServerArrayGUID),  0, CLSCTX_INPROC_SERVER));
    } catch (Exception &e) {
        AnsiString msg = AnsiString("Помилка завантаження серверу розрахунку\n") + e.Message;
        Application->MessageBox(msg.c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
    }
    try {
        if (!PropagServerArrayGUID.IsEmpty() && PropagServerArrayGUID != "") {
            OLECHECK(FPropSrv.CreateInstance(Comobj::StringToGUID(PropagServerArrayGUID),  0, CLSCTX_INPROC_SERVER));
            if (FPropSrv)
                FPropSrv->Init();
        } else {
            FPropSrv.Unbind();
        }
    } catch (Exception &e) {
        AnsiString msg = AnsiString("Помилка завантаження серверу моделі росповсюдження\n") + e.Message;
        Application->MessageBox(msg.c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
    }

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

            OLECHECK(FPathSrv.CreateInstance(CLSID_RSAGeoPath,  0, CLSCTX_INPROC_SERVER));
            if (FPathSrv) {
                HrCheck(FPathSrv->Init(FTerrInfoSrv));

                TRSAPathParams param;

                param.CalcHEff = UseHeff;
                param.CalcTxClearance = UseTxClearence;
                param.CalcRxClearance = UseRxClearence;
                param.CalcSeaPercent =  UseMorfology ;
                param.Step = Step;

                HrCheck(FPathSrv->Set_Params(param));
            }

        } catch (Exception &e) {
            AnsiString msg = AnsiString("Помилка ініциалізації серверу рельєфу\n") + e.Message;
            Application->MessageBox(msg.c_str(), Application->Title.c_str(), MB_ICONERROR | MB_OK);
        }

    } else {
        FTerrInfoSrv.Unbind();
        FPathSrv.Unbind();
    }

    try {
        if (FCalcSrv.IsBound())
        {
            HrCheck(FCalcSrv->Init());
            HrCheck(FCalcSrv->SetPropagServer(FPropSrv));
            HrCheck(FCalcSrv->SetReliefServer(FPathSrv));
            FCalcSrv->CalcMethod = (TCalcMethod)calcMethod;
        }
    } catch (...) {
        Application->MessageBox("Помилка ініциалізації серверу розрахунку", Application->Title.c_str(), MB_ICONERROR | MB_OK);
    }
}

void TLISBCCalcParams::save()
{
    saveServerArrays(CalcServerArray, "CalcServerArray", CalcServerName);
    saveServerArrays(PropagServerArray, "PropagServerArray", PropagServerName);
    saveServerArrays(ReliefServerArray, "ReliefServerArray", ReliefServerName);

    TRegistry *reg = new TRegistry;
    try {
        reg->Access = KEY_WRITE;
        reg->RootKey = HKEY_CURRENT_USER;
        if (reg->OpenKey(ParamPath, true)) {
            reg->WriteInteger("UseHeff", UseHeff);
            reg->WriteInteger("UseTxClearance", UseTxClearence);
            reg->WriteInteger("UseRxClearance", UseRxClearence);
            reg->WriteInteger("UseMorfology", UseMorfology);
            reg->WriteFloat("Step", Step);
            reg->WriteInteger("UseHeffTheo", UseHeffTheo);
            reg->WriteInteger("UseTxClearanceTheo", UseTxClearenceTheo);
            reg->WriteInteger("UseRxClearanceTheo", UseRxClearenceTheo);
            reg->WriteInteger("UseMorfologyTheo", UseMorfologyTheo);
            reg->WriteFloat("StepTheo", StepTheo);
            reg->WriteBool("TheoPathTheSame", TheoPathTheSame);
            reg->WriteFloat("minSelInterf", minSelInterf);
            reg->WriteInteger("higherIntNum", higherIntNum);
            reg->WriteInteger("calcMethod", calcMethod);
            reg->WriteInteger("standRadius", standRadius);
            reg->WriteInteger("defArea", defArea);
            reg->WriteInteger("defCity", defCity);
            reg->WriteBool("SelectionAutotruncation", SelectionAutotruncation);
            reg->WriteBool("mapAutoFit", mapAutoFit);
            reg->WriteBool("rpcRxModeLink", rpcRxModeLink);
            reg->WriteBool("duelAutoRecalc", duelAutoRecalc);
            reg->WriteInteger("duelType", duelType);
            reg->WriteBool("GetCoordinatesFromBase", GetCoordinatesFromBase);
            reg->WriteBool("DisableReliefAtPlanning", DisableReliefAtPlanning);
            reg->WriteBool("earthCurveInRelief", earthCurveInRelief);
            reg->WriteBool("ShowTxNames", ShowTxNames);
            reg->WriteInteger("filesNum", filesNum);
            reg->WriteInteger("lineThicknessZoneCover", lineThicknessZoneCover);
            reg->WriteInteger("lineThicknessZoneNoise", lineThicknessZoneNoise);
            reg->WriteInteger("lineThicknessZoneInterfere", lineThicknessZoneInterfere);

            reg->WriteInteger("lineColorZoneCover", lineColorZoneCover);
            reg->WriteInteger("lineColorZoneNoise", lineColorZoneNoise);
            reg->WriteInteger("lineColorZoneInterfere", lineColorZoneInterfere);
            reg->WriteInteger("lineColorZoneInterfere2", lineColorZoneInterfere2);

            reg->WriteInteger("coordinationPointsInZoneColor", coordinationPointsInZoneColor);
            reg->WriteInteger("coordinationPointsOutZoneColor", coordinationPointsOutZoneColor);

            reg->WriteInteger("changedTxColor", changedTxColor);

            reg->WriteBool("QueryOnMainormClose", QueryOnMainormClose);

            reg->WriteBool("doMapInitDelay", doMapInitDelay);
            reg->WriteInteger("mapInitDelay", mapInitDelay);
            reg->WriteBool("doMapInitInfo", doMapInitInfo);

            reg->CloseKey();
        }

        reg->Access = KEY_WRITE;
        reg->RootKey = HKEY_CURRENT_USER;
        if (reg->OpenKey("Software\\LIS\\LISBCCalc", true)) {
            reg->WriteFloat("_emin_dvb_200", Emin_dvb_200);
            reg->WriteFloat("_emin_dvb_500", Emin_dvb_500);
            reg->WriteFloat("_emin_dvb_700", Emin_dvb_700);
            //reg->WriteBool("_dvb_antenna_discrimination", Dvb_antenna_discrimination);
            reg->WriteBool("_quick_calc_duel_interf", Quick_calc_duel_interf);
            reg->WriteBool("_quick_calc_max_dist", Quick_calc_max_dist);
            reg->WriteBool("_request_for_coord_dist", RequestForCoordDist);
            reg->WriteString("_coord_dist_ini_file", Coord_dist_ini_file);
            reg->WriteFloat("_back_lobe_fm_mono", backLobeFmMono);
            reg->WriteFloat("_back_lobe_fm_stereo", backLobeFmStereo);
            reg->WriteFloat("_back_lobe_tv_band2", backLobeTvBand2);
            reg->WriteFloat("_polar_correct_fm", polarCorrectFm);
            reg->WriteBool("_tv_sound_stereo", tvSoundStereo);
            reg->WriteFloat("_step_calc_max_dist", stepCalcMaxDist);

            reg->CloseKey();
        }
    } __finally {
        reg->Free();
    }
}

__fastcall TLISBCCalcParams::~TLISBCCalcParams()
{
    //
}

void __fastcall TLISBCCalcParams::CheckLibVersion(AnsiString guid, DWORD* version)
{
    std::auto_ptr<TRegistry> reg(new TRegistry);
    reg->Access = KEY_READ;
    reg->RootKey = HKEY_CLASSES_ROOT;
    if (!reg->OpenKeyReadOnly("CLSID\\" + guid + "\\InProcServer32"))
        return;

    AnsiString moduleName = reg->ReadString("");
    unsigned long buffsize = 0;
    int viSize = GetFileVersionInfoSize(moduleName.c_str(), &buffsize);
    if (viSize > 0)
    {
        void* viBuff = malloc(viSize);

        GetFileVersionInfo(moduleName.c_str(), 0, viSize, viBuff);
        VS_FIXEDFILEINFO *ffi;
        VerQueryValue(viBuff, "\\", (void**)&ffi, (PUINT)&buffsize);
        __int64 ver = ffi->dwFileVersionMS;
        ver = (ver << 32) | ffi->dwFileVersionLS;
        __int64 req = version[0];
        req = (req << 32) | version[1];

        if (ver == req)
        {
            free(viBuff);
            return;
        }
        else {
            AnsiString reqVer = AnsiString().sprintf("%d.%d.%d.%d",
                                                    version[0] >> 16,
                                                    version[0] & 0xFFFF,
                                                    version[1] >> 16,
                                                    version[1] & 0xFFFF);
            AnsiString presVer = AnsiString().sprintf("%d.%d.%d.%d",
                                                    ffi->dwFileVersionMS >> 16,
                                                    ffi->dwFileVersionMS & 0xFFFF,
                                                    ffi->dwFileVersionLS >> 16,
                                                    ffi->dwFileVersionLS & 0xFFFF);
            free(viBuff);

            if (ver < req)
                throw *(new Exception("Версия '"+moduleName+"'\n"+presVer+"\nниже требуемой\n"+reqVer));
            else
                throw *(new EUpgradeHost("Версия '"+moduleName+"'\n"+presVer+"\nвыше требуемой\n"+reqVer));
        }
    } else
        throw *(new Exception(AnsiString().sprintf("'%s':\nНет информации о версии файла", moduleName.c_str())));
}


void __fastcall TLISBCCalcParams::CheckCalcServVersion()
{
    // 0.0.6.41 required
    DWORD ver[2] = { (0 << 16) | 1, (7 << 16) | 46 };       //{ (0 << 16) | 0, (5 << 16) | 39 };
    CheckLibVersion(CalcServerArrayGUID, ver);
}
