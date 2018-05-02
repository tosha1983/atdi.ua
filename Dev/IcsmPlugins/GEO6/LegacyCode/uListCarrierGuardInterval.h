//---------------------------------------------------------------------------

#ifndef uListCarrierGuardIntervalH
#define uListCarrierGuardIntervalH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseList.h"
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TfrmListCarrierGuardInterval : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCODE;
    TSmallintField *dstListNUMBERCARRIER;
    TIntegerField *dstListTIMEUSEFULINTERVAL;
    TIntegerField *dstListFREQINTERVAL;
    TFloatField *dstListFREQBOUNDINTERVAL;
    TIntegerField *dstListTIMECURRIERGUARD;
    TIBStringField *dstListNAMECURRIERGUARD;
private:	// User declarations
    __fastcall TfrmListCarrierGuardInterval(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListCarrierGuardInterval(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListCarrierGuardInterval *frmListCarrierGuardInterval;
//---------------------------------------------------------------------------
#endif
