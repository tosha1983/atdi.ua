//----------------------------------------------------------------------------
#ifndef uDlgEminAndNoteH
#define uDlgEminAndNoteH
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
//----------------------------------------------------------------------------
class TdlgEminAndNote : public TForm
{
__published:
    TButton *btnOk;
    TButton *btnCancel;
    TLabel *Label1;
    TLabel *Label2;
    TEdit *edtEmin;
    TEdit *edtNote;
    void __fastcall btnOkClick(TObject *Sender);
private:
public:
	virtual __fastcall TdlgEminAndNote(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgEminAndNote *dlgEminAndNote;
//----------------------------------------------------------------------------
#endif    
