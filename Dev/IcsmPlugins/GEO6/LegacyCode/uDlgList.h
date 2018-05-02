//----------------------------------------------------------------------------
#ifndef uDlgListH
#define uDlgListH
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
#include <vector>
//----------------------------------------------------------------------------
class TdlgList : public TForm
{
__published:
    TButton *btnOk;
    TButton *btnCancel;
	TBevel *Bevel1;
    TListBox *lb;
    TLabel *lblComment;
    void __fastcall lbDblClick(TObject *Sender);
    void __fastcall btnOkClick(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
private:
    int lastIdx;
public:
	virtual __fastcall TdlgList(TComponent* AOwner);
    std::vector<int> ids;
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgList *dlgList;
//----------------------------------------------------------------------------
#endif    
