//---------------------------------------------------------------------------

#ifndef uListOwnerH
#define uListOwnerH
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
//#include "uFormOwner.h"
//---------------------------------------------------------------------------
class TfrmListOwner : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAMEORGANIZATION;
    TIBStringField *dstListADDRESSJURE;
    TIBStringField *dstListADDRESSPHYSICAL;
    TIBStringField *dstListNUMIDENTYCOD;
    TIBStringField *dstListNUMNDS;
    TSmallintField *dstListTYPEFINANCE;
    TIBStringField *dstListNAMEBOSS;
    TIBStringField *dstListPHONE;
    TIBStringField *dstListFAX;
    TIBStringField *dstListMAIL;
    TIBStringField *dstListNUMSETTLEMENTACCOUNT;
    TIBStringField *dstListNUMIDENTYCODACCOUNT;
    TIntegerField *dstListBANK_ID;
    TIBStringField *dstListB_NAME;
    TSmallintField *dstListAVB;
    TSmallintField *dstListAAB;
    TSmallintField *dstListDVB;
    TSmallintField *dstListDAB;
    TStringField *dstListSERV_LIST;
    void __fastcall dstListAfterOpen(TDataSet *DataSet);
    void __fastcall dgrListDblClick(TObject *Sender);
    void __fastcall actListEditExecute(TObject *Sender);
    void __fastcall dstListBeforePost(TDataSet *DataSet);
    void __fastcall dstListCalcFields(TDataSet *DataSet);
    void __fastcall actListInsertExecute(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmListOwner(TComponent* Owner, HWND caller, int elementId);
    virtual void __fastcall updateLookups();
protected:
    __fastcall TfrmListOwner(TComponent* Owner);
};
//---------------------------------------------------------------------------
#endif
