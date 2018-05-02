//---------------------------------------------------------------------------

#ifndef uListOffsetCarryFreqTVAH
#define uListOffsetCarryFreqTVAH
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
class TfrmListOffsetCarryFreqTVA : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCODEOFFSET;
    TIntegerField *dstListOFFSET;
    TSmallintField *dstListOFFSETLINES;
private:	// User declarations
    __fastcall TfrmListOffsetCarryFreqTVA(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListOffsetCarryFreqTVA(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListOffsetCarryFreqTVA *frmListOffsetCarryFreqTVA;
//---------------------------------------------------------------------------
#endif
