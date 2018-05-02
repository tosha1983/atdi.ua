//---------------------------------------------------------------------------

#ifndef uListDocTemplateH
#define uListDocTemplateH
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
#include <OleCtnrs.hpp>
#include <ExtCtrls.hpp>
#include <IBQuery.hpp>
#include <Dialogs.hpp>
#include "DlgSelectTypeDoc.h"
//---------------------------------------------------------------------------
class TfrmListDocTemplate : public TfrmBaseList
{
__published:	// IDE-managed Components
    void __fastcall actListEditExecute(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall dstListNewRecord(TDataSet *DataSet);
    void __fastcall dstListAfterScroll(TDataSet *DataSet);
    void __fastcall dstListBeforePost(TDataSet *DataSet);
    void __fastcall dgrListKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
 //   void __fastcall dgrListDblClick(TObject *Sender);
private:
protected:
    void __fastcall createNewContainer();
    void __fastcall DownloadTemplate(AnsiString& fileName, TDataSet *dataSet, Db::TField* field);
    void __fastcall loadUpContainer();
    void __fastcall saveContainer();
    void __fastcall ShowWordDocumentAndWait(AnsiString& path, bool& modified);    
    void __fastcall UpdateDocument(TField* field, TDataSet *dataSet, AnsiString file);

    __fastcall TfrmListDocTemplate(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListDocTemplate(TComponent* Owner, HWND callerWin, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListDocTemplate *frmListDocTemplate;
//---------------------------------------------------------------------------
#endif
