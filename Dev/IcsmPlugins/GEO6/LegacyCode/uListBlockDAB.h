//---------------------------------------------------------------------------

#ifndef uListBlockDABH
#define uListBlockDABH
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
class TfrmListBlockDAB : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TFloatField *dstListCENTREFREQ;
    TFloatField *dstListFREQFROM;
    TFloatField *dstListFREQTO;
    TFloatField *dstListLOWERGUARDBAND;
    TFloatField *dstListUPPERGUARDBAND;
private:	// User declarations
protected:
    __fastcall TfrmListBlockDAB(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListBlockDAB(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListBlockDAB *frmListBlockDAB;
//---------------------------------------------------------------------------
#endif
