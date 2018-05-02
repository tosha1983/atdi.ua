//----------------------------------------------------------------------------
#ifndef DlgSelectTypeDocH
#define DlgSelectTypeDocH
//----------------------------------------------------------------------------
#include <vcl\System.hpp>
#include <vcl\Windows.hpp>
#include <vcl\SysUtils.hpp>
#include <vcl\Classes.hpp>
#include <vcl\Graphics.hpp>
#include <vcl\StdCtrls.hpp>
#include <vcl\Forms.hpp>
#include <vcl\Controls.hpp>
#include <vcl\Buttons.hpp>
#include <vcl\ExtCtrls.hpp>
#include <Dialogs.hpp>
#include "uBaseObjForm.h"
#include <ActnList.hpp>
#include <DB.hpp>
#include <IBDatabase.hpp>
#include <OleCtnrs.hpp>
#include "xlcClasses.hpp"
#include "xlEngine.hpp"
#include "xlReport.hpp"

//----------------------------------------------------------------------------

enum TTypeDoc { tdWord , tdExcel, tdFromFile };
enum TSpecDocType { tsdGsh = 0, tsdNr, tsdSz, tsdEmc, tsdDne, tsdOthr};
enum TRadTech { trtNone = -1, trtFM, trtDAB, trtTV, trtDVB};

class TSelectTypeDoc : public TfrmBaseObjForm
{
__published:
    TRadioGroup *RadioGroup1;
    TEdit *edtFileName;
    TButton *btnLoadDoc;
    TOpenDialog *OpenDialog1;
    TLabel *Label1;
    TEdit *edTempCode;
    TEdit *edTempName;
    TLabel *Label2;
    TLabel *Label3;
    TButton *btEditTempl;
    TOleContainer *tmpContainer;
        TRadioGroup *rgDT;
        TRadioGroup *rgRadtech;
    void __fastcall RadioGroup1Click(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall btEditTemplClick(TObject *Sender);
    void __fastcall btnLoadDocClick(TObject *Sender);
    void __fastcall edChange(TObject *Sender);
private:
    TComponent *frm;
protected:
    void __fastcall createNewContainer();
    void __fastcall DownloadTemplate(AnsiString& fileName, TDataSet *dataSet, Db::TField* field);
    void __fastcall loadUpContainer();
    void __fastcall saveContainer();
    void __fastcall ShowWordDocumentAndWait(AnsiString& path, bool& modified);
    void __fastcall UpdateDocument(TField* field, TDataSet *dataSet, AnsiString file);
    void __fastcall DataChange();
public:
	virtual __fastcall TSelectTypeDoc(TComponent* AOwner);
    void __fastcall LoadData();
    void __fastcall SaveData();
    bool newTemp;
    int TypeDoc;
    AnsiString FileName;
};
//----------------------------------------------------------------------------
//extern PACKAGE TSelectTypeDoc *SelectTypeDoc;
//----------------------------------------------------------------------------
#endif
