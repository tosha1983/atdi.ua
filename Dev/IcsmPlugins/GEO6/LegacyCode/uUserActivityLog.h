//---------------------------------------------------------------------------

#ifndef uUserActivityLogH
#define uUserActivityLogH
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
class TfrmUserActivityLog : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TDateTimeField *dstListDATECHANGE;
    TIBStringField *dstListLOGIN;
    TIBStringField *dstListNAME_TABLE;
    TIBStringField *dstListNAME_FIELD;
    TIntegerField *dstListNUM_CHANGE;
    TIBStringField *dstListTYPECHANGE;
private:	// User declarations
public:		// User declarations
    __fastcall TfrmUserActivityLog(TComponent* Owner);
    __fastcall TfrmUserActivityLog(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmUserActivityLog *frmUserActivityLog;
//---------------------------------------------------------------------------
#endif
