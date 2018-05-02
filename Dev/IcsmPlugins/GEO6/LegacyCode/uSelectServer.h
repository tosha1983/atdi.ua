//----------------------------------------------------------------------------
#ifndef uSelectServerH
#define uSelectServerH
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
#include <Dialogs.hpp>
#include "uParams.h"
#include "uCalcParamsDlg.h"
#include <vector>
//----------------------------------------------------------------------------
class TdlgSelectServer : public TForm
{
__published:        
	TButton *OKBtn;
	TButton *CancelBtn;
	TBevel *Bevel1;
    TEdit *edFileName;
    TButton *btFile;
    TLabel *Label1;
    TListBox *ServerList;
    TOpenDialog *OpenDialog1;
    TButton *AddNewServ;
    void __fastcall btFileClick(TObject *Sender);
    void __fastcall OKBtnClick(TObject *Sender);
    void __fastcall AddNewServClick(TObject *Sender);
private:
public:
    void __fastcall Clear();
    std::vector<String> guidList;
    std::vector<String> nameList;
	virtual __fastcall TdlgSelectServer(TComponent* AOwner);
    GUID iid;
    ServParamsArray arr;
    short addCount;
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgSelectServer *dlgSelectServer;
//----------------------------------------------------------------------------
#endif    
