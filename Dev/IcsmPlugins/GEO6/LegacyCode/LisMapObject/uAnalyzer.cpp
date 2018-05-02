//---------------------------------------------------------------------------
#include <math>
#include <memory>
#include <values.h>
#pragma hdrstop

//#include "FormProvider.h"
#include "uAnalyzer.h"
//#include "uDuelResult.h"
//#include "uNewSelection.h"
//#include "uMainDm.h"
//#include "uMainForm.h"
#include "uParams.h"
//#include "uSelection.h"
//#include "TxBroker.h"
//#include "tempvalues.h"
//#include "COCALCPROGRESSIMPL.H"
#include <safearry.h>
#include <Clipbrd.hpp>
#include "uReliefFrm.h"
#include <LisIdwm_TLB.h>
#include "BaseMap.h"
//#include <LisLfMfEmin_TLB.h>
//#include <LisLFMFFieldStrength_TLB.h>
//#include <LisLfMfZone_TLB.h>
//#include <p1147_2_TLB.h>
//#include <p368_7_TLB.h>
//#include "uCoordZoneFieldStr.h"

//---------------------------------------------------------------------------
char* szCalcDuelInterfError = "Помилка '%s' при розрахунку дуельних завад: %s";
char* szCalcZoneError = "Помилка '%s' при розрахунку зони: %s";
//---------------------------------------------------------------------------

#pragma package(smart_init)

TxAnalyzer txAnalyzer;
//TCOMIRSASpherics TxAnalyzer::FSpherics;
//TCOMILISProgress TxAnalyzer::FProgress;
const ILisGeoSpherePtr geoSphServ;
//const ILisGndInfoPtr gndInfoServ;
//const ILisIdwmParamPtr idwmParamServ;
/*
const ILisLFMFEminCalcPtr lfmfEmin;
const ILisLFMFFieldStrengthPtr lfmfFs;
const ILisLFMFZoneCalcPtr lfmfCalc;
 
const IP1147Ptr p1147_2;
const IP368Ptr p368_7;

char *szTxNotDefined = "Не визначений передавач";

struct ZoneKey
{
    long txid; long flag;
    ZoneKey() {}; ZoneKey(long id, long fl): txid(id), flag(fl) {};
    bool operator < (const ZoneKey&) const;
};
bool ZoneKey::operator < (const ZoneKey& z) const
{
    if (this->txid < z.txid)
        return true;
    else if (this->txid > z.txid)
        return false;
    else
        return this->flag < z.flag;
}

typedef std::map<ZoneKey, LPSAFEARRAY> EtalonMap;
EtalonMap etalonMap;
       */
//---------------------------------------------------------------------------
__fastcall TxAnalyzer::TxAnalyzer()
// : isNewPlan(false), wasChanges(false), time0(Now())
{
}
//---------------------------------------------------------------------------

void __fastcall TxAnalyzer::MapToolUsed(TObject *Sender,
      short ToolNum, double X1, double Y1, double X2, double Y2,
      double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
      VARIANT_BOOL *EnableDefault)
{
  /*  static TfrmRelief* frmRelief = NULL;
    switch (ToolNum) {
        case miReliefTool:
            if (!frmRelief)
                frmRelief = new TfrmRelief(Application);
            frmRelief->fmProfileView1->RetreiveProfile(X1, Y1, X2, Y2);
            frmRelief->Show();
        break;
        case miDistanceTool:
        break;
    }    */
}
//---------------------------------------------------------------------------

void __fastcall TxAnalyzer::CheckIdwm()
{
    if (!geoSphServ.IsBound())
        HrCheck(geoSphServ.CreateInstance(CLSID_CoLisIdwm), __FUNC__"(): инициализация CoLisIdwm");
}

void __fastcall TxAnalyzer::GetPoint(double lon1, double lat1, double az, double dist, double* lon2, double* lat2)
{
    CheckIdwm();
    float lat, lon;
    geoSphServ->GetPoint(lon1, lat1, az, dist, &lon, &lat);
    *lon2 = lon;
    *lat2 = lat;
}


