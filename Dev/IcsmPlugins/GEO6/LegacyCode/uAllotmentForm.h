//---------------------------------------------------------------------------

#ifndef uAllotmentFormH
#define uAllotmentFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <ExtCtrls.hpp>
#include <ComCtrls.hpp>
#include "NumericEdit.hpp"
#include <Grids.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include "LISBCTxServer_TLB.h"
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <IBSQL.hpp>
#include <Menus.hpp>
#include <vector>
#include "uMainDm.h"
#include "BcDrawer.h"

//---------------------------------------------------------------------------
class TfrmAllotment : public TForm
{
__published:	// IDE-managed Components
    TPanel *panData;
    TPanel *panBottom;
    TBitBtn *btnOk;
    TBitBtn *btnSave;
    TBitBtn *btnLoad;
    TBitBtn *btnCancel;
    TActionList *ActionList1;
    TAction *actOk;
    TAction *actSave;
    TAction *actLoad;
    TAction *actClose;
    TPageControl *pcRemark;
    TTabSheet *tshRemark1;
    TTabSheet *tshRemark2;
    TTabSheet *tshRemark3;
    TMemo *memRemark1;
    TMemo *memRemark2;
    TMemo *memRemark3;
    TLabel *lblDabDvb;
    TLabel *lblItuId;
    TEdit *edtItuId;
    TLabel *lblRrc06Code;
    TLabel *lblName;
    TLabel *lblAdmin;
    TLabel *lblCountry;
    TLabel *lblUniqCntry;
    TLabel *lblRpc;
    TLabel *lblRn;
    TLabel *lblFreq;
    TLabel *lblChanBlock;
    TLabel *lblFreqOffset;
    TLabel *lblSfn;
    TLabel *lblPol;
    TLabel *lblSm;
    TEdit *edtName;
    TEdit *edtAdmin;
    TComboBox *cbxCountry;
    TComboBox *cbxGeoArea;
    TComboBox *cbxRpc;
    TComboBox *cbxRn;
    TNumericEdit *edtFreq;
    TNumericEdit *edtFreqOffset;
    TComboBox *cbxPol;
    TComboBox *cbxSm;
    TComboBox *cbxChanBlock;
    TDBEdit *edtSfn;
    TButton *btnSfn;
    TComboBox *cbxRrc06Code;
    TButton *btnDropSfn;
    TButton *btnGeoAreaClear;
    TPanel *panContours;
    TTabControl *tcContours;
    TStringGrid *grdContour;
    TPanel *panMap;
    TToolBar *tbContours;
    TToolButton *tbtAddCntr;
    TToolButton *tbtDelCntr;
    TToolButton *ToolButton5;
    TToolButton *tbtZoomIn;
    TToolButton *tbtZoomOut;
    TToolButton *tbtLeft;
    TToolButton *tbtRight;
    TImageList *imageList;
    TLabel *lblId;
    TStaticText *txtId;
    TToolButton *tbtDown;
    TToolButton *tbtUp;
    TToolButton *ToolButton1;
    TButton *btnChanFilter;
    TToolButton *ToolButton2;
    TToolButton *tbtSelCountry;
    TIBDataSet *dstSfn;
    TDataSource *dsSfn;
    TToolButton *tbtFit;
    TLabel *lblCoord;
    TMemo *memCoord;
    TListBox *lbxTxList;
    TIBSQL *sqlTxList;
    TPanel *panAdditional;
    TPanel *panBotRight;
    TSplitter *spl;
    TLabel *Label1;
    TSpeedButton *sbExamination;
    TPopupMenu *PopupMenu1;
    TSpeedButton *sbIntoProject;
    void __fastcall actOkExecute(TObject *Sender);
    void __fastcall actSaveExecute(TObject *Sender);
    void __fastcall actLoadExecute(TObject *Sender);
    void __fastcall actCloseExecute(TObject *Sender);
    void __fastcall actSaveUpdate(TObject *Sender);
    void __fastcall edtFreqValueChange(TObject *Sender);
    void __fastcall edtFreqOffsetValueChange(TObject *Sender);
    void __fastcall edtItuIdValueChange(TObject *Sender);
    void __fastcall edtNameExit(TObject *Sender);
    void __fastcall cbxRrc06CodeChange(TObject *Sender);
    void __fastcall cbxRpcChange(TObject *Sender);
    void __fastcall cbxRnChange(TObject *Sender);
    void __fastcall cbxChanBlockChange(TObject *Sender);
    void __fastcall cbxPolChange(TObject *Sender);
    void __fastcall cbxSmChange(TObject *Sender);
    void __fastcall edtAdminExit(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall tcContoursChange(TObject *Sender);
    void __fastcall tbtAddCntrClick(TObject *Sender);
    void __fastcall tbtDelCntrClick(TObject *Sender);
    void __fastcall tbtAddPointClick(TObject *Sender);
    void __fastcall tbtDelPointClick(TObject *Sender);
    void __fastcall grdContourSetEditText(TObject *Sender, int ACol,
          int ARow, const AnsiString Value);
    void __fastcall FormPaint(TObject *Sender);
    void __fastcall tbtSelCountryClick(TObject *Sender);
    void __fastcall btnDropSfnClick(TObject *Sender);
    void __fastcall btnGeoAreaClearClick(TObject *Sender);
    void __fastcall edtItuIdExit(TObject *Sender);
    void __fastcall cbxCountryChange(TObject *Sender);
    void __fastcall cbxGeoAreaChange(TObject *Sender);
    void __fastcall btnSfnClick(TObject *Sender);
    void __fastcall FormResize(TObject *Sender);
    void __fastcall tbtFitClick(TObject *Sender);
    void __fastcall tbtZoomInClick(TObject *Sender);
    void __fastcall tbtZoomOutClick(TObject *Sender);
    void __fastcall tbtLeftClick(TObject *Sender);
    void __fastcall tbtRightClick(TObject *Sender);
    void __fastcall tbtDownClick(TObject *Sender);
    void __fastcall tbtUpClick(TObject *Sender);
    void __fastcall memCoordExit(TObject *Sender);
    void __fastcall lbxTxListDblClick(TObject *Sender);
    void __fastcall lbxTxListKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall memRemark1Exit(TObject *Sender);
    void __fastcall memRemark2Exit(TObject *Sender);
    void __fastcall memRemark3Exit(TObject *Sender);
    void __fastcall sbExaminationClick(TObject *Sender);
    void __fastcall sbIntoProjectClick(TObject *Sender);
private:	// User declarations
    TCOMILISBCTx tx;
    TCOMILisBcDigAllot allot;
    WideString dab_dvb;
    std::vector<int> chn_blk_ids;
    std::vector<double> freqs;
    TForm *mapForm;

    ContourDrawer cd;
    std::vector<DrawContourData> contoursData;
    int oldIndex;
    void __fastcall CacheContours();
    
    int __fastcall GetDbSection();
    void __fastcall CheckPlanEntry();

    bool addSubArea;

    TPopupMenu *popupMenu;
    void __fastcall MenuItem_OnClick(TObject *Sender);
protected:
    __fastcall TfrmAllotment(TComponent* Owner);
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
public:		// User declarations
    __fastcall TfrmAllotment(TComponent* Owner, ILISBCTx *iTx);
    void __fastcall ShowData();
    ILISBCTx* __fastcall GetTx() { return (ILISBCTx *)tx; }
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmAllotment *frmAllotment;
//---------------------------------------------------------------------------
#endif
