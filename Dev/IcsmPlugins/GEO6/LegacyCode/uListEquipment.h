//---------------------------------------------------------------------------

#ifndef uListEquipmentH
#define uListEquipmentH
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
class TfrmListEquipment : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListTYPEEQUIPMENT;
    TIBStringField *dstListNAME;
    TIBStringField *dstListMANUFACTURE;
private:	// User declarations
    __fastcall TfrmListEquipment(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListEquipment(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListEquipment *frmListEquipment;
//---------------------------------------------------------------------------
#endif
