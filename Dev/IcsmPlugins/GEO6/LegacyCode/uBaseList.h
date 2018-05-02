//---------------------------------------------------------------------------

#ifndef uBaseListH
#define uBaseListH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <ToolWin.hpp>
#include <ActnList.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <ImgList.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <ExtCtrls.hpp>
#include <IBQuery.hpp>
#include <vector>

#define WM_LIST_ELEMENT_SELECTED (WM_USER + 1)
//---------------------------------------------------------------------------
class TfrmBaseList : public TForm
{
__published:	// IDE-managed Components
    TToolBar *tbrList;
    TToolButton *tbtListIns;
    TToolButton *tbtListCpy;
    TToolButton *tbtListEdt;
    TToolButton *tbtListDel;
    TToolButton *tbtListApl;
    TToolButton *tbtListCnc;
    TImageList *imlList;
    TDBGrid *dgrList;
    TActionList *aclList;
    TAction *actListInsert;
    TAction *actListCopy;
    TAction *actListEdit;
    TAction *actListDelete;
    TAction *actListCancel;
    TAction *actOk;
    TAction *actListApply;
    TAction *actClose;
    TIBTransaction *trList;
    TIBDataSet *dstList;
    TDataSource *dsrList;
    TAction *actListSearch;
    TToolButton *tbtSearch;
    TEdit *edtIncSearch;
    TPanel *panList;
    TPanel *panSearch;
    TIBQuery *ibqSelectForCopy;
    TIBQuery *ibqInsertCopy;
    TLabel *lblSearchParam;
    TAction *actSearchCancel;
    TToolButton *tbtSearchCancel;
    TAction *actRefresh;
    TToolButton *tbtRefresh;
    void __fastcall actOkExecute(TObject *Sender);
    void __fastcall actCloseExecute(TObject *Sender);
    void __fastcall actListApplyExecute(TObject *Sender);
    void __fastcall actListInsertExecute(TObject *Sender);
    void __fastcall actListCopyExecute(TObject *Sender);
    void __fastcall actListEditExecute(TObject *Sender);
    void __fastcall actListCancelExecute(TObject *Sender);
    void __fastcall actListDeleteExecute(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall FormDestroy(TObject *Sender);
    void __fastcall actListSearchExecute(TObject *Sender);
    void __fastcall actListApplyUpdate(TObject *Sender);
    void __fastcall dstListBeforePost(TDataSet *DataSet);
    void __fastcall dstListBeforeDelete(TDataSet *DataSet);
    void __fastcall dstListAfterPost(TDataSet *DataSet);
    void __fastcall dstListAfterDelete(TDataSet *DataSet);
    void __fastcall FormKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall edtIncSearchExit(TObject *Sender);
    void __fastcall dgrListEditButtonClick(TObject *Sender);
    void __fastcall dgrListDblClick(TObject *Sender);
    void __fastcall dgrListKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall dstListAfterClose(TDataSet *DataSet);
    void __fastcall edtIncSearchKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall edtIncSearchChange(TObject *Sender);
    void __fastcall dstListFilterRecord(TDataSet *DataSet, bool &Accept);
    void __fastcall actSearchCancelExecute(TObject *Sender);
    void __fastcall actRefreshExecute(TObject *Sender);
protected:	// User declarations
    //  массив последних идентификаторов
    static int last_id[1000];
    //  расположение окна
 
    struct {
        int left, top, width, height;
        int reserve[100];
    } layout;
    //  вызывающее окно
    HWND m_caller;
    //  элемент по умолчанию
    int m_elementId;
    __fastcall TfrmBaseList(TComponent* Owner);
    //  перечитать Lookup из таблицы ассоциации
    virtual void __fastcall updateLookups();
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, setElement)
    END_MESSAGE_MAP(TForm)
    //  установить ассоциацию
    void __fastcall setElement(Messages::TMessage &Message);
    //  сохранить расположение окна
    virtual void __fastcall Initialize();
    virtual void __fastcall setCaption();
    virtual __fastcall changeQueryCopy();
    std::vector<AnsiString> ListFilterField;
    std::vector<AnsiString> ListFilterValue;
    int NewElementId;
    bool _MDIChild;

    bool isInitialized;
public:		// User declarations
    __fastcall TfrmBaseList(TComponent* Owner, HWND callerWin, int elementId);
    __fastcall TfrmBaseList(bool MDIChild);
    virtual void __fastcall EditElement();
    virtual void __fastcall NewElement();
    __property HWND Caller = { read = m_caller };
    __property int ElementId = { read = m_elementId };
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmBaseList *frmBaseList;
//---------------------------------------------------------------------------
#endif
