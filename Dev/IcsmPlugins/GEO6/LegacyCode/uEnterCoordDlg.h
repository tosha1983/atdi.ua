//----------------------------------------------------------------------------
#ifndef uEnterCoordDlgH
#define uEnterCoordDlgH
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
class TdlgEnterCoord : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
    TLabel *Label1;
    TEdit *edtLon;
    TEdit *edtLat;
    TLabel *Label2;
    TLabel *Label3;
    void __fastcall edtLonExit(TObject *Sender);
    void __fastcall edtLatExit(TObject *Sender);
private:
public:
	virtual __fastcall TdlgEnterCoord(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgEnterCoord *dlgEnterCoord;
//----------------------------------------------------------------------------
#endif    
