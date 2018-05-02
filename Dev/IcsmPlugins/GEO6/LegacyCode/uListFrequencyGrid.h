//---------------------------------------------------------------------------

#ifndef uListFrequencyGridH
#define uListFrequencyGridH
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
class TfrmListFrequencyGrid : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TIBStringField *dstListDESCRIPTION;
    void __fastcall actListInsertExecute(TObject *Sender);
private:	// User declarations
    __fastcall TfrmListFrequencyGrid(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListFrequencyGrid(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListFrequencyGrid *frmListFrequencyGrid;
//---------------------------------------------------------------------------
#endif
