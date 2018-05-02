//----------------------------------------------------------------------------
#ifndef uNewChPlanDlgH
#define uNewChPlanDlgH
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
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <DB.hpp>

//----------------------------------------------------------------------------
class TdgCreateChannelPlan : public TForm
{
__published:
    TButton *btOk;
    TButton *btCancel;
	TBevel *Bevel1;
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TLabel *Label4;
    TLabel *Label5;
    TLabel *Label6;
    TLabel *Label7;
    TLabel *Label8;
    TLabel *Label9;
    TEdit *FirstFreq;
    TEdit *LastFreq;
    TLabel *Label10;
    TEdit *FreqGridName;
    TEdit *Spacing;
    TEdit *ChanWidth;
    TEdit *FirstChanNum;
    TEdit *VideoRem;
    TEdit *SoundRem;
    TEdit *NamePref;
    TEdit *NameSuf;
    TEdit *NumQuant;
    TComboBox *LastOrNumb;

    void __fastcall btOkClick(TObject *Sender);
    void __fastcall CheckFreqOnExit(TObject *Sender);
private:
public:
	virtual __fastcall TdgCreateChannelPlan(TComponent* AOwner);
    bool __fastcall CreatePlan();
};
//----------------------------------------------------------------------------
extern PACKAGE TdgCreateChannelPlan *dgCreateChannelPlan;
//----------------------------------------------------------------------------
#endif    
