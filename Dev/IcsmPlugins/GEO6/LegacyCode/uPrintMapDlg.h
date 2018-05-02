//----------------------------------------------------------------------------
#ifndef uPrintMapDlgH
#define uPrintMapDlgH
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
#include <ComCtrls.hpp>
#include <Dialogs.hpp>
//----------------------------------------------------------------------------
class TdlgPrintMap : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
	TBevel *Bevel1;
    TSaveDialog *SaveDialog1;
    TLabel *lblFileName;
    TEdit *edtFileName;
    TButton *btnGetFileName;
    TLabel *lblQuality;
    TUpDown *udQuality;
    TEdit *edtQuality;
    TLabel *lblRange;
    TEdit *edtRange;
    TUpDown *upRange;
    TLabel *lblX;
    void __fastcall btnGetFileNameClick(TObject *Sender);
    void __fastcall CancelBtnClick(TObject *Sender);
    void __fastcall OKBtnClick(TObject *Sender);
    void __fastcall SaveDialog1SelectionChange(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
private:
    //TfmMap *frmMap;
public:
	virtual __fastcall TdlgPrintMap(TComponent* AOwner);

};
//----------------------------------------------------------------------------
extern PACKAGE TdlgPrintMap *dlgPrintMap;
//----------------------------------------------------------------------------
#endif    
