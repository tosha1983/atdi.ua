//---------------------------------------------------------------------------

#ifndef uBaseObjFormH
#define uBaseObjFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <ExtCtrls.hpp>
#include <IBDatabase.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
//---------------------------------------------------------------------------
class TfrmBaseObjForm : public TForm
{
__published:	// IDE-managed Components
    TPanel *pnButtons;
    TBitBtn *btOk;
    TBitBtn *btApply;
    TBitBtn *btRefresh;
    TBitBtn *btClose;
    TActionList *ActionList1;
    TAction *actOk;
    TAction *actApply;
    TAction *actClose;
    TAction *actRefresh;
    TIBTransaction *tr;
    TDataSource *dscObj;
    void __fastcall actOkExecute(TObject *Sender);
    void __fastcall actApplyExecute(TObject *Sender);
    void __fastcall actRefreshExecute(TObject *Sender);
    void __fastcall actCloseExecute(TObject *Sender);
    void __fastcall actApplyUpdate(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
private:	// User declarations
public:		// User declarations
    unsigned objId;
    bool dataChanged;
    String m_Caption;
    __fastcall TfrmBaseObjForm(TComponent* Owner);
    virtual void __fastcall LoadData();
    virtual void __fastcall SaveData();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmBaseObjForm *frmBaseObjForm;
//---------------------------------------------------------------------------
#endif
