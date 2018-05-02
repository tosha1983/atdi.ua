//----------------------------------------------------------------------------
#ifndef uSelectStationsH
#define uSelectStationsH
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
#include <CheckLst.hpp>
//----------------------------------------------------------------------------
class TdgSelectStations : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
    TCheckListBox *lbList;
    TButton *btAll;
    TButton *btNone;
    TLabel *Label1;
    void __fastcall btAllClick(TObject *Sender);
    void __fastcall btNoneClick(TObject *Sender);
private:
public:
	virtual __fastcall TdgSelectStations(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdgSelectStations *dgSelectStations;
//----------------------------------------------------------------------------
#endif    
