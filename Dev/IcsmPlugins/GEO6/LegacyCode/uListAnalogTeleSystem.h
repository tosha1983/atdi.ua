//---------------------------------------------------------------------------

#ifndef uListAnalogTeleSystemH
#define uListAnalogTeleSystemH
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
class TfrmListAnalogTeleSystem : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAMESYSTEM;
    TSmallintField *dstListNUMBERLINES;
    TFloatField *dstListCHANNELBAND;
    TFloatField *dstListVIDEOBAND;
    TFloatField *dstListSEPARATEVIDEOSOUND1;
    TFloatField *dstListVESTIGIALBAND;
    TIBStringField *dstListVIDEOMODULATION;
    TIBStringField *dstListSOUND1MODULATION;
    TIBStringField *dstListSOUND2SYSTEM;
    TFloatField *dstListSEPARATEVIDEOSOUND2;
    TSmallintField *dstListENUMVAL;
    TDataSource *dsGrids;
    TIBDataSet *dstGrids;
    TIntegerField *dstGridsID;
    TIBStringField *dstGridsNAME;
    TIntegerField *dstListFREQUENCYGRID_ID;
    TStringField *dstListGRIDNAME;
    TIBStringField *dstListDESCR;
private:	// User declarations
    __fastcall TfrmListAnalogTeleSystem(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListAnalogTeleSystem(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListAnalogTeleSystem *frmListAnalogTeleSystem;
//---------------------------------------------------------------------------
#endif
