//---------------------------------------------------------------------------

#ifndef uListLicenseH
#define uListLicenseH
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
#include <IBQuery.hpp>
#include "uBaseList.h"
#include <IBSQL.hpp>
//---------------------------------------------------------------------------
class TfrmListLicense : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TDateField *dstListDATEFROM;
    TDateField *dstListDATETO;
    TIBStringField *dstListNUMLICENSE;
    TIntegerField *dstListOWNER_ID;
    TTimeField *dstListTIMEFROM;
    TTimeField *dstListTIMETO;
    TIBQuery *ibqOwner;
    TDataSource *dsOwner;
    TIntegerField *ibqOwnerID;
    TIBStringField *ibqOwnerNAMEORGANIZATION;
    TIBStringField *dstListNAMEORGANIZATION;
    TIntegerField *dstListCODE;
    TIBStringField *dstListCALLSIGN;
    TSmallintField *dstListANNUL;
    void __fastcall dstListBeforePost(TDataSet *DataSet);
    void __fastcall dstListANNULGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall dstListANNULSetText(TField *Sender,
          const AnsiString Text);
private:	// User declarations
    __fastcall TfrmListLicense(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListLicense::TfrmListLicense(TComponent* Owner, HWND caller, int elementId);
protected:
    virtual void __fastcall fillTree();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListLicense *frmListLicense;
//---------------------------------------------------------------------------
#endif
