//---------------------------------------------------------------------------

#ifndef uSearchH
#define uSearchH
//---------------------------------------------------------------------------
#include <ActnList.hpp>
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <ComObj.hpp>
#include <Controls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <Forms.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include "uWhere.h"
#include <StdCtrls.hpp>
#include <ToolWin.hpp>
#include <ExtCtrls.hpp>
#include <Menus.hpp>
#include "CustomMap.h"
#include "NumericEdit.hpp"
#include <memory>
#include <Menus.hpp>
#include <StdCtrls.hpp>
#include <system.hpp>
#include <vcl.h>

#include "uParams.h"
#include "FormProvider.h"
#include "TxBroker.h"

#include "uMainDm.h"

class Cfields
{
public:
     AnsiString         alias;
     AnsiString         displayLabel;
     AnsiString         fieldName;
     bool		        parsed;
     bool               selected;
     bool               visible;
     unsigned int       width;

     Cfields();
     Cfields(const Cfields &value);
    ~Cfields();
};

//---------------------------------------------------------------------------
class TfrmSearch : public TForm
{
__published:	// IDE-managed Components
    TToolBar *tbrSearch;
    TPageControl *pcSearch;
    TStatusBar *sbSearch;
    TTabSheet *tshCriteria;
    TTabSheet *tshSQL;
    TTabSheet *tshResults;
    TfmWhereCriteria *fmWhereCriteria1;
    TGroupBox *gbTerr;
    TLabel *Label1;
    TEdit *edtLon;
    TLabel *Label2;
    TEdit *edtLat;
    TLabel *Label3;
    TEdit *edtMaxRadius;
    TGroupBox *gbAdmin;
    TCheckBox *chbCity;
    TComboBox *cbxCountry;
    TComboBox *cbxArea;
    TComboBox *cbxCity;
    TLabel *Label4;
    TLabel *Label5;
    TLabel *Label6;
    TMemo *memSQL;
        TDBGrid *dbgRsults;
    TImageList *ImageList1;
    TActionList *ActionList1;
    TToolButton *tbtRun;
    TToolButton *tbtColumns;
    TToolButton *btnToExcel;
    TAction *actRun;
    TAction *actToExcel;
    TAction *actColumns;
    TIBSQL *sqlCountry;
    TIBSQL *sqlArea;
    TIBSQL *sqlCity;
        TIBQuery *ibqQuery;
        TDataSource *DataSource1;
    TToolButton *tbtSortOrder;
    TGroupBox *gbTxNo;
        TCheckBox *chbNumber;
        TEdit *edtRegion;
        TEdit *edtTransmitter;
        TLabel *Label7;
        TLabel *Label8;
        TCheckBox *chbTerritory;
    TAction *actExport;
    TToolButton *tbtSep1;
    TToolButton *tbtExport;
    TAction *actSortOrder;
    TRadioGroup *rgrChFB;
    TComboBox *cbxBlock;
    TEdit *edtFrequency;
    TComboBox *cbxChannel;
    TButton *btChannelClear;
    TButton *btFrequencyClear;
    TButton *btBlockClear;
    TTabSheet *tshMap;
    TImageList *imlMap;
    TActionList *alMap;
    TAction *actClear;
    TAction *actSelect;
    TAction *actPan;
    TAction *actLayers;
    TAction *actReload;
    TAction *actZoomIn;
    TAction *actZoomOut;
    TAction *actDistance;
    TAction *actNone;
    TAction *actSaveBmp;
    TAction *actCalcCoverSector;
    TAction *actSetTP;
    TAction *actOffset;
    TAction *actERP;
    TAction *actGetRelief;
    TAction *actZoomFit;
    TAction *actShowTx;
    TProgressBar *ProgressBar1;
    TAction *actEdit;
    TCustomMapFrame *cmf;
    TPopupMenu *pmnTx;
    TPanel *pnTx;
    TPanel *pnLocation;
    TPanel *pnCommon;
    TPanel *pnSite;
    TGroupBox *gbSite;
    TLabel *Label9;
    TComboBox *cbHgtSea;
    TNumericEdit *edHgtSea;
    TLabel *Label10;
    TLabel *Label11;
    TComboBox *cbHgtGnd;
    TNumericEdit *edHgtGnd;
    TLabel *Label12;
    TLabel *Label13;
    TEdit *edSiteName;
    TRadioButton *rbPoint;
    TRadioButton *rbRegion;
    TLabel *Label14;
    TLabel *Label15;
    TEdit *edLonLeft;
    TEdit *edLatTop;
    TLabel *Label16;
    TEdit *edLonRight;
    TEdit *edLatBottom;
    TLabel *Label17;
    TToolButton *tb7;
    void __fastcall btnToExcelClick(TObject *Sender);
    void __fastcall btnCloseClick(TObject *Sender);
    void __fastcall ExcelFormClose(TObject *Sender, TCloseAction &Action);
    AnsiString __fastcall nToAz(int n);
    void __fastcall edtLonExit(TObject *Sender);
    void __fastcall edtLatExit(TObject *Sender);
    void __fastcall edtMaxRadiusExit(TObject *Sender);
    void __fastcall chbTerritoryClick(TObject *Sender);
    void __fastcall chbCityClick(TObject *Sender);
    void __fastcall actRunExecute(TObject *Sender);
    void __fastcall actColumnsExecute(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall cbxCountryChange(TObject *Sender);
    void __fastcall cbxAreaChange(TObject *Sender);
        void __fastcall tshSQLShow(TObject *Sender);
        void __fastcall ibqQueryAfterOpen(TDataSet *DataSet);
        void __fastcall edtRegionChange(TObject *Sender);
        void __fastcall chbNumberClick(TObject *Sender);
        void __fastcall edtTransmitterChange(TObject *Sender);
        void __fastcall dbgRsultsMouseMove(TObject *Sender,
          TShiftState Shift, int X, int Y);
        void __fastcall dbgRsultsDblClick(TObject *Sender);
    void __fastcall actExportExecute(TObject *Sender);
    void __fastcall actSortOrderExecute(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall FormDestroy(TObject *Sender);
    void __fastcall btChannelClearClick(TObject *Sender);
    void __fastcall cbxChannelDropDown(TObject *Sender);
    void __fastcall rgrChFBClick(TObject *Sender);
    void __fastcall btFrequencyClearClick(TObject *Sender);
    void __fastcall btBlockClearClick(TObject *Sender);
    void __fastcall cbxBlockDropDown(TObject *Sender);
    void __fastcall actNoneExecute(TObject *Sender);
    void __fastcall actDistanceExecute(TObject *Sender);
    void __fastcall actPanExecute(TObject *Sender);
    void __fastcall actZoomInExecute(TObject *Sender);
    void __fastcall actZoomOutExecute(TObject *Sender);
    void __fastcall actZoomFitExecute(TObject *Sender);
    void __fastcall actSaveBmpExecute(TObject *Sender);
    void __fastcall pcSearchChange(TObject *Sender);
    void __fastcall actEditExecute(TObject *Sender);
    void __fastcall dbgRsultsKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall OnObjectSelection(TObject * Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift);
    void __fastcall cmftb3Click(TObject *Sender);
    void __fastcall cmftb4Click(TObject *Sender);
    void __fastcall edRegCoordEnter(TObject *Sender);
    void __fastcall edPntCoordEnter(TObject *Sender);
    void __fastcall MapToolUsed(TObject *Sender,
      short ToolNum, double X1, double Y1, double X2, double Y2,
      double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
      VARIANT_BOOL *EnableDefault);
    void __fastcall tbtSelectionClick(TObject *Sender);
private:	// User declarations
    bool stopExcel;
    std::vector<Cfields> baseFields;
    std::vector<Cfields> sortedFields;
    TCOMILISBCTxList txList;
    std::map<int, LPSAFEARRAY> coverage;
    void ParseIni(AnsiString source, std::vector<Cfields> &where);
    void __fastcall fillCombo(TComboBox* cbx, TIBSQL* sql, int parentId, int elementId);
    void __fastcall ActivateMapSheet();
    void __fastcall getCoverage(bool recalcAll);
    void __fastcall drawCoverage();
    void drawTxs();
public:		// User declarations
    AnsiString iniFieldsFileName;
    __fastcall TfrmSearch(TComponent* Owner);
    __fastcall ~TfrmSearch();
    void __fastcall LoadFields();
    AnsiString __fastcall getClause();
    void __fastcall query_lat_lon_OnGetText(TField* Sender, AnsiString &Text, bool DisplayText);
    void __fastcall query_K_CH_B_OnGetText(TField* Sender, AnsiString &Text, bool DisplayText);
    void __fastcall query_SystemType_OnGetText(TField* Sender, AnsiString &Text, bool DisplayText);
    AnsiString __fastcall getTxName(ILISBCTx* iTx);

    double lat, lon;
    std::map<int,int> shapeIdx;  
};
//---------------------------------------------------------------------------
#endif
