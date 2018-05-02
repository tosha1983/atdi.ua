//----------------------------------------------------------------------------
#ifndef uSaveServerH
#define uSaveServerH
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
class TdlgSaveServer : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
	TBevel *Bevel1;
    TLabel *Label1;
    TEdit *edGuid;
    TLabel *Label2;
    TEdit *edServName;
    TLabel *Label3;
    TMemo *memDescr;
private:
public:
	virtual __fastcall TdlgSaveServer(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgSaveServer *dlgSaveServer;
//----------------------------------------------------------------------------
#endif    
