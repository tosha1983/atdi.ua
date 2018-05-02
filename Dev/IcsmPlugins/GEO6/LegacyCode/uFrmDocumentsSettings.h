//---------------------------------------------------------------------------

#ifndef uFrmDocumentsSettingsH
#define uFrmDocumentsSettingsH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include "uFrmTxBase.h"
#include <ExtCtrls.hpp>
#include <Dialogs.hpp>
#include <Mask.hpp>
#include <Buttons.hpp>
#include <OleCtnrs.hpp>
#include "uBaseObjForm.h"
#include <ActnList.hpp>
#include <DB.hpp>
#include <IBDatabase.hpp>
#include "xlcClasses.hpp"
#include "xlEngine.hpp"
#include "xlReport.hpp"
#include <DBCtrls.hpp>
//---------------------------------------------------------------------------
class TfrmDocumentsSettings : public TfrmBaseObjForm
{
__published:	// IDE-managed Components
    TLabel *lblNum;
    TLabel *lblData;
    TLabel *lblAccStat;
    TLabel *ldlDocType;
    TRadioGroup *rgDoctype;
    TEdit *edtNum;
    TDateTimePicker *dtpDocDate;
    TComboBox *cbxAccountState;
    TComboBox *cbxDocType;
    TBitBtn *btnInDocImage;
    TLabel *lblDocName;
    TLabel *lblDN;
    TOpenDialog *OpenDialog1;
    TOleContainer *tmpContainer;
//    TxlReport *xlReport1;
    TButton *btOpenDoc;
    TxlReport *xlReport1;
    TRadioGroup *rbInOut;
    void __fastcall rgDoctypeClick(TObject *Sender);
    void __fastcall btnInDocImageClick(TObject *Sender);
    void __fastcall OpenDialog1SelectionChange(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall btOpenDocClick(TObject *Sender);
    void __fastcall FormKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall edtNumChange(TObject *Sender);
    void __fastcall dtpDocDateChange(TObject *Sender);
    void __fastcall cbxAccountStateChange(TObject *Sender);
    void __fastcall cbxDocTypeChange(TObject *Sender);
    void __fastcall rbInOutClick(TObject *Sender);
private:	// User declarations
    typedef TfrmBaseObjForm Inherited;
    void __fastcall DownloadTemplate(AnsiString& fileName, int id);
    void __fastcall CreatePwrDocument(AnsiString& documentName, AnsiString templateName, TIBQuery* query);
    void __fastcall UploadDocument(TField* field, TDataSet *dataSet, AnsiString file);
    void __fastcall ShowWordDocumentAndWait(AnsiString& path, bool& modified);
    void __fastcall DownloadDocument(AnsiString& fileName, int id);
    void __fastcall ExcelDocumentFill(TOleContainer* oleContainer, TDataSet *DataSet);
    AnsiString __fastcall GetDocumentType(int documentId);
    void __fastcall UpdateDocument(TField* field, TDataSet *dataSet, AnsiString file);
    void WordDocumentFill(TOleContainer* oleContainer, TDataSet *DataSet);
    void __fastcall LoadUpContainer();
    void __fastcall CreateNewContainer();
    void __fastcall SaveContainer();
public:		// User declarations
    __fastcall TfrmDocumentsSettings(TComponent* Owner);
    void __fastcall LoadData();
    void __fastcall SaveData();
};
//---------------------------------------------------------------------------
#endif
