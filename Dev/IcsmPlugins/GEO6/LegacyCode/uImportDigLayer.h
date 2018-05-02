//----------------------------------------------------------------------------
#ifndef uImportDigLayerH
#define uImportDigLayerH
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
class TdlgImportDigLayer : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
	TBevel *Bevel1;
    TCheckListBox *lbxLayers;
    TRadioButton *rbDab;
    TRadioButton *rbDvb;
    TComboBox *cbxCountry;
    void __fastcall OKBtnClick(TObject *Sender);
private:
public:
	virtual __fastcall TdlgImportDigLayer(TComponent* AOwner);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgImportDigLayer *dlgImportDigLayer;
//----------------------------------------------------------------------------
#endif    
