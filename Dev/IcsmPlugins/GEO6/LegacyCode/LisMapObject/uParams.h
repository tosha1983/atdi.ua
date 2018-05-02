//---------------------------------------------------------------------------

#ifndef uParamsH
#define uParamsH

#include <vcl.h>
#pragma hdrstop
#include <LISBCCalc_TLB.h>
#include <vector>
//---------------------------------------------------------------------------

enum BCCursors {
    crGetE = 100,
    crGetTx = 101,
    crGetRelief = 102,
};

enum DuelType {dtTxLocation, dtMiddlePoint, dtZoneContraction};

void HrCheck(HRESULT hr, AnsiString errMsg = String());
struct ServParams {
    String name;
    String guid;
    Variant params;
    ServParams() {};
    ServParams(const ServParams& src)
    {
        name = src.name;
        guid = src.guid;
        params = src.params;
    }
    ServParams& operator= (const ServParams& src)
    {
        name = src.name;
        guid = src.guid;
        params = src.params;
        return *this;
    }
};

typedef std::vector<ServParams> ServParamsArray;

class EUpgradeHost: public Exception
{
  public:
    EUpgradeHost(AnsiString msg): Exception(msg) {};
};

class TLISBCCalcParams {
public:
    TCOMIRSATerrainInfo  FTerrInfoSrv;

    ServParamsArray ReliefServerArray;
        AnsiString ReliefServerArrayGUID;
        AnsiString ReliefServerName;
        AnsiString ReliefPath;  


    VARIANT_BOOL UseHeff, UseTxClearence, UseRxClearence, UseMorfology;
    VARIANT_BOOL UseHeffTheo, UseTxClearenceTheo, UseRxClearenceTheo, UseMorfologyTheo;
    int calcMethod;
    float Step;
    float StepTheo;
    bool TheoPathTheSame;
    int filesNum;

    //  экспертиза
    bool SelectionAutotruncation;
    double minSelInterf;    //  минимальная помеха, по значению которой обрезается выборка
    int higherIntNum;       //  количество старших помех, отбираемых для лконтрольной точки
    bool mapAutoFit;
    bool duelAutoRecalc;
    int degreeStep;         //  шаг, с которым считаются зоны
    bool showCp;            //  показывать КТ при расчёте зон
    float treshVideo;       //  критическое изменение Епомехи для ТВ
    float treshAudio;       //  критическое изменение Епомехи для радио

    //   общие
    bool QueryOnMainormClose;
    bool GetCoordinatesFromBase;
    bool DisableReliefAtPlanning;
    bool earthCurveInRelief;
    bool ShowTxNames;
    bool rpcRxModeLink;


    //  новый передатчик
    int standRadius; //  радиус выборки существующих опор в минутах
    int defArea;     //  регион по умолчанию
    int defCity;     //  город по умолчанию

    float Emin_dvb_200;
    float Emin_dvb_500;
    float Emin_dvb_700;
    bool Dvb_antenna_discrimination;
    bool Quick_calc_duel_interf;
    bool Quick_calc_max_dist;
    bool RequestForCoordDist;
    AnsiString Coord_dist_ini_file;
    float backLobeFmMono;
    float backLobeFmStereo;
    float backLobeTvBand2;
    float polarCorrectFm;
    bool tvSoundStereo;
    float stepCalcMaxDist;

    int lineThicknessZoneCover;
    int lineThicknessZoneNoise;
    int lineThicknessZoneInterfere;

    int lineColorZoneCover;
    int lineColorZoneNoise;
    int lineColorZoneInterfere;
    int lineColorZoneInterfere2;

    int coordinationPointsInZoneColor;
    int coordinationPointsOutZoneColor;

    int changedTxColor;

    DuelType duelType;

    __fastcall TLISBCCalcParams();
    __fastcall ~TLISBCCalcParams();
    AnsiString loadServerArrays(ServParamsArray &VariantArray, AnsiString Path);
    void saveServerArrays(ServParamsArray &VariantArray, AnsiString Path, AnsiString DegaultGUID);

    void save();
    void load();
    AnsiString ParamPath;
private:
    //Variant FCalcSrvVar;
};

extern TLISBCCalcParams BCCalcParams;
extern const char* sAppRegPath;

#endif
