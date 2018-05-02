//---------------------------------------------------------------------------

#ifndef uListDigitalSystemH
#define uListDigitalSystemH
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
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmListDigitalSystem : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAMESYSTEM;
    TIBStringField *dstListMODULATION;
    TIBStringField *dstListCODERATE;
    TFloatField *dstListGAUSSIANCHANNEL;
    TFloatField *dstListRICEANCHANNEL;
    TFloatField *dstListRAYLEIGHCHANNEL;
    TFloatField *dstListNETBITRATEGUARD4;
    TFloatField *dstListNETBITRATEGUARD8;
    TFloatField *dstListNETBITRATEGUARD16;
    TFloatField *dstListNETBITRATEGUARD32;
    TIntegerField *dstListENUMVAL;
    TIBStringField *dstListDESCR;
private:	// User declarations
    __fastcall TfrmListDigitalSystem(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListDigitalSystem(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListDigitalSystem *frmListDigitalSystem;
//---------------------------------------------------------------------------
#endif
