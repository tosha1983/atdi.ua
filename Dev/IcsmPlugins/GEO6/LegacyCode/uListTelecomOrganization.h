//---------------------------------------------------------------------------

#ifndef uListTelecomOrganizationH
#define uListTelecomOrganizationH
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
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmListTelecomOrganization : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCODE;
    TIBStringField *dstListNAME;
    TIBStringField *dstListPHONE;
    TIBStringField *dstListMAIL;
    TIBStringField *dstListADDRESS;
    TIntegerField *dstListCOUNTRY_ID;
    TIBStringField *dstListC_CODE;
    TIBDataSet *IBDataSet1;
    TIntegerField *IBDataSet1ID;
    TIBStringField *IBDataSet1CODE;
    TSmallintField *dstListCOORDDOCUMENT;
    void __fastcall dstListCOORDDOCUMENTGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListCOORDDOCUMENTSetText(TField *Sender,
          const AnsiString Text);
private:	// User declarations
protected:
    __fastcall TfrmListTelecomOrganization(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListTelecomOrganization(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListTelecomOrganization *frmListTelecomOrganization;
//---------------------------------------------------------------------------
#endif
