//---------------------------------------------------------------------------

#ifndef uListTypeSFNH
#define uListTypeSFNH
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
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
//---------------------------------------------------------------------------
class TfrmListTypeSFN : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListTYPENAME;
    TIBStringField *dstListNETGEOMETRY;
    TSmallintField *dstListNUMTRANSMITTERS;
    TFloatField *dstListPOWER_CENTRANS;
    TFloatField *dstListPOWER_PERTRANS;
    TFloatField *dstListCOVERAGEAREA;
    TFloatField *dstListDIRECTIVITY_CENTRANS;
    TFloatField *dstListDIRECTIVITY_PERTRANS;
    TFloatField *dstListDISTANCE_ADJACENT_TRANS;
    TIntegerField *dstListEFFHEIGHTANT_CENTRANS;
    TIntegerField *dstListEFFHEIGHTANT_PERTRANS;
private:	// User declarations
    __fastcall TfrmListTypeSFN(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListTypeSFN(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListTypeSFN *frmListTypeSFN;
//---------------------------------------------------------------------------
#endif
