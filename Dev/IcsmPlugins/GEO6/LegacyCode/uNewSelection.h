//----------------------------------------------------------------------------
#ifndef uNewSelectionH
#define uNewSelectionH
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
#include "uMainDM.h"
#include <IBSQL.hpp>
#include <Menus.hpp>
#include <ComCtrls.hpp>
#include <Grids.hpp>
#include <CheckLst.hpp>
#include "uWhere.h"
#include "NumericEdit.hpp"
#include <memory>
//----------------------------------------------------------------------------

extern char* szNewSelectionQuestion;

typedef struct { int id; double dist; }  TxInfo;
typedef std::vector<TxInfo> TxInfoList;

class TdlgNewSelection : public TForm
{
__published:
    TButton *btnOk;
    TButton *btnCancel;
    TPanel *panParams;
    TLabel *lblTxName;
    TIBSQL *sqlGetSelection;
    TIBSQL *sqlAddState;
    TIBSQL *sqlAddRegion;
    TPageControl *pcSelectionCriteria;
    TTabSheet *tshCommon;
    TTabSheet *tshAddition;
    TGroupBox *gbxCond;
    TListBox *lbxCond;
    TButton *btnCondAdd;
    TButton *btnCondRm;
    TButton *btnCondRmAll;
    TGroupBox *gbxRegions;
    TListBox *lbxRegions;
    TButton *btnRegionsAdd;
    TButton *btnRegionsRm;
    TButton *btnRegionsRmAll;
    TCheckBox *chbAdjanced;
    TCheckBox *chbImage;
    TCheckBox *chbOnlyRoot;
    TEdit *edtLon;
    TEdit *edtLat;
    TCheckBox *chbMaxRadius;
    TNumericEdit *edtMaxRadius;
    TfmWhereCriteria *fmWhereCriteria1;
    TComboBox *cbxProcName;
    TLabel *Label1;
    TPanel *panFreqGrid;
    TLabel *lblFmDiapason;
    TEdit *edtDiapason;
    TPanel *panChBlockGrid;
    TLabel *lblChFrom;
    TComboBox *cbxChFrom;
    TLabel *lblChTo;
    TComboBox *cbxChTo;
    TComboBox *cbxChannelGrid;
    TLabel *lblChannelGrid;
    TRadioGroup *rgrGrid;
    TCheckBox *chbSelectBrIfic;
    TCheckListBox *lbDbSection;
    TIBSQL *sqlDbSection;
    TRadioGroup *rgBand;
    void __fastcall edtMaxRadiusExit(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall btnCondAddClick(TObject *Sender);
    void __fastcall btnRegionsAddClick(TObject *Sender);
    void __fastcall btnCondRmClick(TObject *Sender);
    void __fastcall btnRegionsRmClick(TObject *Sender);
    void __fastcall btnCondRmAllClick(TObject *Sender);
    void __fastcall btnRegionsRmAllClick(TObject *Sender);
    void __fastcall btnOkClick(TObject *Sender);
    void __fastcall btnCancelClick(TObject *Sender);
    void __fastcall chbMaxRadiusClick(TObject *Sender);
    void __fastcall edtLonExit(TObject *Sender);
    void __fastcall edtLatExit(TObject *Sender);
    void __fastcall cbxChannelGridChange(TObject *Sender);
    void __fastcall rgrGridClick(TObject *Sender);
    void __fastcall rgBandClick(TObject *Sender);
private:
    void __fastcall AcceptChoice(Messages::TMessage& Message);
public:
	virtual __fastcall TdlgNewSelection(TComponent* AOwner);
    int TxId;
    double lat, lon;
protected:
    std::auto_ptr<TIBSQL> sqlSelect;
    std::auto_ptr<TIBSQL> sqlInsert;
    std::vector<int> dbSectionIds;
    void __fastcall clearAuxTabes(int selId);
    TxInfoList __fastcall GetFxmSelection();
    TxInfoList __fastcall GetDigAllotSelection();     

    bool __fastcall CheckTX(double freq1, double bandwith1, double freq2, double bandwith2);

    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, AcceptChoice)
    END_MESSAGE_MAP(TForm)
};
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
#endif    
