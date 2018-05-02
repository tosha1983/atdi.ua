//---------------------------------------------------------------------------

#ifndef uListCountryH
#define uListCountryH
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
class TfrmListCountry : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TIBStringField *dstListCODE;
    TIBStringField *dstListDESCRIPTION;
    TIBStringField *dstListDEF_COLOR;
    TIntegerField *dstListDEF_FM_SYS;
    TIntegerField *dstListDEF_DVB_SYS;
    TIBDataSet *dstTvaLookup;
    TIBDataSet *dstFmLookup;
    TIBDataSet *dstDvbLookup;
    TIBStringField *dstListDEF_TVA_NAME;
    TIntegerField *dstTvaLookupID;
    TIBStringField *dstTvaLookupNAMESYSTEM;
    TIntegerField *dstFmLookupID;
    TIBStringField *dstFmLookupCODSYSTEM;
    TIntegerField *dstDvbLookupID;
    TIBStringField *dstDvbLookupNAMESYSTEM;
    TIBStringField *dstListDEF_FM_NAME;
    TIBStringField *dstListDEF_DVB_NAME;
    TIntegerField *dstListDEF_TVA_SYS;
private:	// User declarations
protected:
    __fastcall TfrmListCountry(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListCountry(TComponent* Owner, HWND callerWin, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListCountry *frmListCountry;
//---------------------------------------------------------------------------
#endif
