//----------------------------------------------------------------------------
#ifndef uOffsetRangeDlgH
#define uOffsetRangeDlgH
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
#include "uSelection.h"
#include <Mask.hpp>
//----------------------------------------------------------------------------
class TOffsetRangeDlg : public TForm
{
__published:
	TButton *OKBtn;
	TButton *CancelBtn;
	TBevel *Bevel1;
    TLabel *Label1;
    TLabel *Label2;
    TEdit *edtDownRange;
    TUpDown *udDown;
    TEdit *edtUpRange;
    TUpDown *udUp;
    void __fastcall OKBtnClick(TObject *Sender);
private:
    TfrmSelection *frmSelection; 
public:
	virtual __fastcall TOffsetRangeDlg(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TOffsetRangeDlg *OffsetRangeDlg;
//----------------------------------------------------------------------------
#endif    
