//----------------------------------------------------------------------------
#ifndef uSectorDlgH
#define uSectorDlgH
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
#include "CSPIN.h"
//----------------------------------------------------------------------------
class TdlgSector : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
    TPanel *panSector;
    TLabel *Label1;
    TGroupBox *GroupBox1;
    TLabel *Label2;
    TCSpinEdit *cseAzBeg;
    TCSpinEdit *cseAzEnd;
    TLabel *Label3;
    TLabel *Label5;
    TCSpinEdit *cseRadEnd;
    TCSpinEdit *cseRadBeg;
    TLabel *Label4;
    TLabel *Label6;
    TEdit *edtRadStep;
    TEdit *edtAzStep;
    TLabel *Label7;
    TLabel *lblTxName;
    TCheckBox *chbGradient;
    TLabel *Label8;
    TLabel *Label9;
    TEdit *edtEmin;
    void __fastcall FormShow(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
private:
public:
	virtual __fastcall TdlgSector(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgSector *dlgSector;
//----------------------------------------------------------------------------
#endif    
