//----------------------------------------------------------------------------
#ifndef uDlgConstrSetH
#define uDlgConstrSetH
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
#include "CoordConv.hpp"
//----------------------------------------------------------------------------
class TdlgConstrSet : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
	TBevel *Bevel1;
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TLabel *Label4;
    TEdit *edtMinLat;
    TEdit *edtMinLon;
    TEdit *edtMaxLat;
    TEdit *edtMaxLon;
    TCheckBox *chbOnlyIfContExist;
    void __fastcall edtMinLatExit(TObject *Sender);
    void __fastcall edtMinLonExit(TObject *Sender);
    void __fastcall edtMaxLatExit(TObject *Sender);
    void __fastcall edtMaxLonExit(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
private:
    void __fastcall ShowValues();
    void __fastcall GetValues();
public:
    double minLon;
    double minLat;
    double maxLon;
    double maxLat;
    TCoordinateConvertor* cc;
	virtual __fastcall TdlgConstrSet(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgConstrSet *dlgConstrSet;
//----------------------------------------------------------------------------
#endif    
