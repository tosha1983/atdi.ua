//---------------------------------------------------------------------------

#ifndef uListTypeReceiveH
#define uListTypeReceiveH
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
class TfrmListTypeReceive : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
private:	// User declarations
    __fastcall TfrmListTypeReceive(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListTypeReceive(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListTypeReceive *frmListTypeReceive;
//---------------------------------------------------------------------------
#endif
