//---------------------------------------------------------------------------

#ifndef uListRadioServiceH
#define uListRadioServiceH
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
class TfrmListRadioService : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCODE;
    TIBStringField *dstListNAME;
    TFloatField *dstListFREQFROM;
    TFloatField *dstListFREQTO;
    TIBStringField *dstListDESCRIPTION;
private:	// User declarations
    __fastcall TfrmListRadioService(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListRadioService(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListRadioService *frmListRadioService;
//---------------------------------------------------------------------------
#endif
