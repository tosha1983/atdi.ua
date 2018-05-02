//----------------------------------------------------------------------------
#ifndef uExportDlgH
#define uExportDlgH
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
#include <vcl\DBGrids.hpp>
#include <vcl\IBCustomDataset.hpp>
#include <Dialogs.hpp>
#include "ExpServer_TLB.h"
//----------------------------------------------------------------------------
class TdlgExport : public TForm
{
__published:
	TButton *OKBtn;
	TButton *CancelBtn;
    TPanel *Panel1;
    TRadioGroup *rgList;
    TRadioGroup *rgFormat;
    TLabel *Label1;
    TEdit *edtFilename;
    TButton *btnFilename;
    TOpenDialog *opdFile;
    void __fastcall btnFilenameClick(TObject *Sender);
    void __fastcall rgFormatClick(TObject *Sender);
    void __fastcall OKBtnClick(TObject *Sender);
private:
public:
	virtual __fastcall TdlgExport(TComponent* AOwner);
};

enum TListKind {
    lkAll,
    lkSelected
};

class TxExporter {
private:
    IExpPtr expPtr;
public:
    void __fastcall exportTxGrid(TExpParam mode, TListKind listKind, TDBGrid* grid, AnsiString filename);
    void __fastcall exportTxList(TExpParam mode, TListKind listKind, ILISBCTxList * txList, AnsiString filename);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgExport *dlgExport;
extern TxExporter txExporter;
//----------------------------------------------------------------------------
#endif
