//---------------------------------------------------------------------------

#ifndef uListChannelH
#define uListChannelH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseListTree.h"
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
#include <IBSQL.hpp>
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmListChannel : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAMECHANNEL;
    TFloatField *dstListFMSOUNDCARRIERSECOND;
    TFloatField *dstListFREQCARRIERNICAM;
    TFloatField *dstListFREQCARRIERSOUND;
    TFloatField *dstListFREQCARRIERVISION;
    TFloatField *dstListFREQFROM;
    TFloatField *dstListFREQTO;
    TIntegerField *dstListFREQUENCYGRID_ID;
private:	// User declarations
    __fastcall TfrmListChannel(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListChannel(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListChannel *frmListChannel;
//---------------------------------------------------------------------------
#endif
