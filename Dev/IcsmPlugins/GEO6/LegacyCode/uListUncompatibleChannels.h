//---------------------------------------------------------------------------

#ifndef uListUncompatibleChannelsH
#define uListUncompatibleChannelsH
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
class TfrmListUncompatibleChannels : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCOCHANNEL;
    TIBStringField *dstListLOWERADJACENT;
    TIBStringField *dstListUPPERADJACENT;
    TIBStringField *dstListHETERODYNEHARMONIC1;
    TIBStringField *dstListHETERODYNEHARMONIC2;
    TIBStringField *dstListHETERODYNEHARMONIC3;
    TIBStringField *dstListLOWERIMAGE1;
    TIBStringField *dstListLOWERIMAGE2;
    TIBStringField *dstListUPPERIMAGE1;
    TIBStringField *dstListUPPERIMAGE2;
private:	// User declarations
    __fastcall TfrmListUncompatibleChannels(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListUncompatibleChannels(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListUncompatibleChannels *frmListUncompatibleChannels;
//---------------------------------------------------------------------------
#endif
