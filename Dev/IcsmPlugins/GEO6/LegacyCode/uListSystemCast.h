//---------------------------------------------------------------------------

#ifndef uListSystemCastH
#define uListSystemCastH
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
class TfrmListSystemCast : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCODE;
    TIBStringField *dstListDESCRIPTION;
    TSmallintField *dstListTYPESYSTEM;
    TIBStringField *dstListCLASSWAVE;
    TFloatField *dstListFREQFROM;
    TFloatField *dstListFREQTO;
    TSmallintField *dstListNUMDIAPASON;
    TIBStringField *dstListRELATIONNAME;
    TSmallintField *dstListENUMVAL;
    TSmallintField *dstListIS_USED;
    void __fastcall dstListTYPESYSTEMGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListIS_USEDGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall dstListIS_USEDSetText(TField *Sender,
          const AnsiString Text);
private:	// User declarations
    __fastcall TfrmListSystemCast(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListSystemCast(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListSystemCast *frmListSystemCast;
//---------------------------------------------------------------------------
#endif
