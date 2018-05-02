//---------------------------------------------------------------------------
#ifndef uSelectionH
#define uSelectionH
//---------------------------------------------------------------------------
#include "LISBCCalc_TLB.h"
#include "uCoordinationPoint.h"
#include "uMainDm.h"
#include "uWhere.h"

#include <ActnList.hpp>
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <DB.hpp>
#include <DBTables.hpp>
#include <Dialogs.hpp>
#include <ExtCtrls.hpp>
#include <Forms.hpp>
#include <Graphics.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <Menus.hpp>
#include <StdCtrls.hpp>
#include <ToolWin.hpp>
#include "uLisObjectGrid.h"
#include "CustomMap.h"
#include <map>
#include <memory>
#include <Menus.hpp>
#include <StdCtrls.hpp>
#include <string>
#include <ToolWin.hpp>
//---------------------------------------------------------------------------

extern TColor ccTx;
extern TColor ccTxSelected;
extern TColor ccTxZero;
extern TColor ccZoneCover;
extern TColor ccZoneCoverNotUsed;
extern TColor ccZoneNoise;
extern TColor ccECover;
extern TColor ccEPoint;
extern TColor ccZoneCoord;

enum CheckPointDataIndex {
    cpdiStandName = 0,
    cpdiNumReg,
    cpdiTxNum,
    cpdiLongitude,
    cpdiLatitude,
    cpdiAzimuth,
    cpdiDist,
    cpdiE01,
    cpdiE10,
    cpdiE50,
    cpdiEu1,
    cpdiEu2,
    cpdiPrC,
    cpdiPrT,
    cpdiPol,
    cpdiDa,
    cpdiDaO,
    cpdiHeff,
    cpdiLast
};

class TPolarDiagramPanel;
class TBCCalcResult;

enum TBCCalcResultType {rtOffset, rtERP};

enum Sorting {sortNone = 0, sortFrom, sortTo};
struct AdditionalParams{
    Sorting sorting;
    bool NewSelection;
};

class TfrmSelection : public TForm
{
__published:	// IDE-managed Components
    TIBSQL *sqlRefresh;
    TIBSQL *sqlAddTx;
    TIBSQL *sqlDeleteTx;
    TActionList *alSelection;
    TPopupMenu *pmnSelection;
    TAction *actAddTx;
    TAction *actRemoveTx;
    TAction *actRefresh;
    TAction *actUsedInCalc;
    TAction *actSortFromUs;
    TAction *actSortToUs;
    TMenuItem *N1;
    TMenuItem *N2;
    TMenuItem *N3;
    TMenuItem *N4;
    TMenuItem *N5;
    TMenuItem *N6;
    TMenuItem *N7;
    TMenuItem *N8;
    TIBSQL *sqlUpdateUsed;
    TAction *actEdit;
    TPageControl *pcSelection;
    TTabSheet *tshSelection;
    TAction *actCalcDuel;
    TAction *actCalcCoverSector;
    TAction *actSaveList;
    TAction *actSaveTx;
    TAction *actSaveRes;
    TAction *actSetTP;
    TMenuItem *N9;
    TMenuItem *N10;
    TMenuItem *N11;
    TMenuItem *N12;
    TMenuItem *N13;
    TMenuItem *N14;
    TAction *actOffset;
    TAction *actERP;
    TMenuItem *N15;
    TMenuItem *N16;
    TMenuItem *N17;
    TMenuItem *N18;
    TSaveDialog *SaveDialog1;
    TIBSQL *sqlTxId;
    TTabSheet *tshChannels;
    TStatusBar *StBr;
    TStringGrid *sgrChannelList;
    TAction *actDuelInterfere;
    TImageList *imlSelection;
    TPopupMenu *pmnSave;
    TMenuItem *N19;
    TIBDataSet *dstResults;
    TIntegerField *dstResultsID;
    TBlobField *dstResultsRESULT;
    TPopupMenu *pmnResults;
    TMenuItem *N21;
    TMenuItem *N22;
    TMenuItem *N23;
    TMenuItem *N25;
    TMenuItem *N26;
    TAction *actSaveObject;
    TMenuItem *N29;
    TMenuItem *N30;
    TMenuItem *N31;
    TIBSQL *sqlChannelList;
    TMenuItem *N35;
    TMenuItem *N36;
    TMenuItem *N37;
    TMenuItem *N39;
    TMenuItem *N43;
    TMenuItem *N44;
    TMenuItem *N45;
    TAction *actSelectAll;
    TAction *actDeselectAll;
    TAction *actRevertAll;
    TAction *actSelect20;
    TProgressBar *pb;
    TAction *actPureCoverage;
    TAction *actRemoveAllUnused;
    TMenuItem *N46;
    TMenuItem *N49;
    TMenuItem *N50;
    TMenuItem *N51;
    TMenuItem *N52;
    TMenuItem *N24;
    TTabSheet *tshMap;
    TPanel *panCalcResult;
    TPanel *panGraph;
    TSplitter *Splitter1;
    TAction *actGetTxZones;
    TPageControl *pcCalcResult;
    TTabSheet *tshZone;
    TTabSheet *tshPoint;
    TTabSheet *tshCoordination;
    TComboBox *cbxWantedTx;
    TIBSQL *sqlTxNameList;
    TStaticText *txtNo;
    TStaticText *txtDesc;
    TButton *btnGetZones;
    TStringGrid *grdZones;
    TPanel *panData;
    TLabel *lblDataParam1;
    TLabel *lblDataParam2;
    TLabel *lblDataParam3;
    TLabel *lblDataVal1;
    TLabel *lblDataVal2;
    TLabel *lblDataVal3;
    TLabel *lblDataParam4;
    TLabel *lblDataParam5;
    TLabel *lblDataParam6;
    TLabel *lblDataVal4;
    TLabel *lblDataVal5;
    TLabel *lblDataVal6;
    TIBSQL *sqlUpdateSort;
    TButton *btnDelZones;
    TLabel *lblPointParam1;
    TLabel *lblPointParam2;
    TLabel *lblPointParam3;
    TLabel *lblPointData1;
    TLabel *lblPointData2;
    TLabel *lblPointData3;
    TStringGrid *grdPoint;
    TLabel *lblPointParam8;
    TLabel *lblPointData8;
    TLabel *lblPointParam9;
    TLabel *lblPointData9;
    TPanel *panCoordination;
    TMemo *memCountryList;
    TLabel *lblCoordination;
    TTabSheet *tshDuel;
    TMenuItem *N32;
    TAction *actRemoveLessThanZero;
    TStringGrid *grdDuelPoints;
    TLabel *lblAData;
    TLabel *lblBData;
    TLabel *lblEminA;
    TLabel *lblEminB;
    TLabel *lblEa;
    TLabel *lblEb;
    TAction *actAnalyze;
    TLabel *lblA;
    TLabel *lblB;
    TLabel *lblUnwantedTx;
    TComboBox *cbxUnwantedTx;
    TLabel *lblEminAData;
    TLabel *lblEminBData;
    TAction *actShowTestPoints;
    TTabSheet *tshOffsetPick;
    TTabSheet *tshErpPick;
    TMemo *memErp;
    TMemo *memOffset;
    TIBDataSet *ibdsRefresh;
    TIntegerField *ibdsRefreshTRANSMITTERS_ID;
    TSmallintField *ibdsRefreshUSED_IN_CALC;
    TFloatField *ibdsRefreshDISTANCE;
    TIntegerField *ibdsRefreshSORTINDEX;
    TBlobField *ibdsRefreshRESULT;
    TToolBar *tbrSelection;
    TToolButton *tbtAdd;
    TToolButton *tbtRemove;
    TToolButton *tbtSep1;
    TToolButton *tbtDuelInterfere;
    TToolButton *tbtSortFrom;
    TToolButton *tbtSortTo;
    TToolButton *tbtSep2;
    TToolButton *tbtFirst20;
    TToolButton *tbtSelectAll;
    TToolButton *tbtDeselectAll;
    TToolButton *tbtRevertSelection;
    TToolButton *tbtSep5;
    TToolButton *tbtDuel;
    TToolButton *tbtSector;
    TToolButton *tbtOffset;
    TToolButton *tbtERP;
    TToolButton *tbtSep3;
    TToolButton *tbtSetTP;
    TToolButton *tbtSaveResTo;
    TToolButton *tbtSep4;
    TImageList *imlMap;
    TActionList *alMap;
    TAction *actClear;
    TAction *actPan;
    TAction *actLayers;
    TAction *actZoomIn;
    TAction *actZoomOut;
    TAction *actDistance;
    TAction *actNone;
    TAction *actSaveBmp;
    TToolBar *tbrMap;
    TToolButton *tbtClear;
    TToolButton *tbtSep6;
    TToolButton *tbtNone;
    TToolButton *tbtDistance;
    TToolButton *tbtPan;
    TToolButton *tbtZoomIn;
    TToolButton *tbtZoomOut;
    TToolButton *tbtSep7;
    TToolButton *tbtLayers;
    TToolButton *tbtSavePicture;
    TToolButton *tbtSep8;
    TCheckBox *chbSaveZone;
    TPanel *panMap;
    TAction *actGetRelief;
    TToolButton *tbtGetRelief;
    TToolButton *tbtZoomFit;
    TAction *actZoomFit;
    TFloatField *ibdsRefreshE_UNWANT;
    TFloatField *ibdsRefreshE_WANT;
    TAction *actExport;
    TSmallintField *ibdsRefreshZONE_OVERLAPPING;
    TToolButton *tbtShowTx;
    TAction *actShowTx;
    TAction *actTruncateSelection;
    TAction *actTruncateSelectionFromCurrentTx;
    TMenuItem *N20;
    TMenuItem *N27;
    TMenuItem *N28;
    TFloatField *ibdsRefreshAZIMUTH;
    TMenuItem *N33;
    TMenuItem *N34;
    TLabel *lblPoint2Param2;
    TLabel *lblPoint2Param8;
    TLabel *lblPoint2Data2;
    TLabel *lblPoint2Data8;
    TLabel *lblPoint2Param3;
    TLabel *lblPoint2Param9;
    TLabel *lblPoint2Data3;
    TLabel *lblPoint2Data9;
    TLabel *lblPoint2Data1;
    TCheckBox *chbShowInDetail;
    TButton *btnCoordinates;
    TToolButton *ToolButton1;
    TAction *actExportSelectionToExcel;
    TToolButton *tbtnDegradationSectorSelection;
    TAction *actDegradationSectorSelection;
    TToolButton *tbtnCoordinationPointsShow;
    TAction *actCoordinationPointsShow;
    TTimer *tmrInvalidate;
    TLabel *lblUnwantedTx2;
    TComboBox *cbxUnwantedTx2;
    TCheckBox *chbTwoUnwantedTxs;
    TIntegerField *ibdsRefreshENUMVAL;
    TLisObjectGrid *grid;
    TCustomMapFrame *cmf;
    TPopupMenu *pmnTx;
    TMenuItem *mniTxEdit;
    TMenuItem *mniAnalyze;
    TMenuItem *mniUseInCalc;
    TMenuItem *mniShowTestPoints;
    TAction *actAllotInterfZone;
    TPopupMenu *pmnAllotZones;
    TToolButton *tbtHideBtns;
    TButton *btUnwantedList;
    TLabel *lbUnwantedList;
    TAction *actSetAsInterfere;
    TAction *actSetAsObject;
    TPanel *pnProjection;
    TButton *btProjection;
    TPanel *pnTime;
    TDateTimePicker *dpTime;
    TLabel *lbUniTime;
    TDateTimePicker *tpTime;
    TAction *actDayNight;
    TMenuItem *miDay;
    TMenuItem *miNight;
    TMenuItem *N38;
    TMenuItem *N40;
    TAction *actDropEtalonZones;
    TMenuItem *i2;
    TMenuItem *N41;
    TButton *btNow;
    TButton *btNoon;
    TButton *btMidnight;
    TButton *btOpMode;
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall sgrSelectionDragOver(TObject *Sender, TObject *Source, int X, int Y, TDragState State, bool &Accept);
    void __fastcall sgrSelectionDragDrop(TObject *Sender, TObject *Source, int X, int Y);
    void __fastcall actAddTxExecute(TObject *Sender);
    void __fastcall actRemoveTxExecute(TObject *Sender);
    void __fastcall actRefreshExecute(TObject *Sender);
    void __fastcall actUsedInCalcExecute(TObject *Sender);
    void __fastcall actSortFromUsExecute(TObject *Sender);
    void __fastcall actSortToUsExecute(TObject *Sender);
    void __fastcall actUsedInCalcUpdate(TObject *Sender);
    void __fastcall sgrSelectionKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
    void __fastcall actEditExecute(TObject *Sender);
    void __fastcall sgrSelectionDblClick(TObject *Sender);
    void __fastcall actCalcDuelExecute(TObject *Sender);
    void __fastcall actCalcCoverSectorExecute(TObject *Sender);
    void __fastcall actSaveListExecute(TObject *Sender);
    void __fastcall actSaveTxExecute(TObject *Sender);
    void __fastcall actSaveResExecute(TObject *Sender);
    void __fastcall actSetTPExecute(TObject *Sender);
    void __fastcall actOffsetExecute(TObject *Sender);
    void __fastcall actERPExecute(TObject *Sender);
    void __fastcall actDuelInterfereExecute(TObject *Sender);
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
    void __fastcall pcSelectionChange(TObject *Sender);
    void __fastcall actSelectAllExecute(TObject *Sender);
    void __fastcall actDeselectAllExecute(TObject *Sender);
    void __fastcall actRevertAllExecute(TObject *Sender);
    void __fastcall actSelect20Execute(TObject *Sender);
    void __fastcall actPureCoverageExecute(TObject *Sender);
    void __fastcall actRemoveAllUnusedExecute(TObject *Sender);
    void __fastcall sgrSelectionKeyUp(TObject *Sender, WORD &Key, TShiftState Shift);
    void __fastcall sgrSelectionMouseUp(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall actGetTxZonesExecute(TObject *Sender);
    void __fastcall panCalcResultResize(TObject *Sender);
    void __fastcall cbxWantedTxChange(TObject *Sender);
    void __fastcall btnDelZonesClick(TObject *Sender);
    void __fastcall pcCalcResultChange(TObject *Sender);
    void __fastcall actRemoveLessThanZeroExecute(TObject *Sender);
    void __fastcall grdDuelPointsDrawCell(TObject *Sender, int ACol, int ARow, TRect &Rect, TGridDrawState State);
    void __fastcall actAnalyzeExecute(TObject *Sender);
    void __fastcall cbxUnwantedTxChange(TObject *Sender);
    void __fastcall actShowTestPointsExecute(TObject *Sender);
    void __fastcall actReloadUpdate(TObject *Sender);
    void __fastcall actClearExecute(TObject *Sender);
    void __fastcall actPanExecute(TObject *Sender);
    void __fastcall actLayersExecute(TObject *Sender);
    void __fastcall actZoomInExecute(TObject *Sender);
    void __fastcall actZoomOutExecute(TObject *Sender);
    void __fastcall actDistanceExecute(TObject *Sender);
    void __fastcall actNoneExecute(TObject *Sender);
    void __fastcall actSaveBmpExecute(TObject *Sender);
    void __fastcall actGetReliefExecute(TObject *Sender);
    void __fastcall actZoomFitExecute(TObject *Sender);
    void __fastcall actExportExecute(TObject *Sender);
    void __fastcall actShowTxExecute(TObject *Sender);
    void __fastcall actTruncateSelectionExecute(TObject *Sender);
    void __fastcall actTruncateSelectionFromCurrentTxExecute(TObject *Sender);
    void __fastcall N34Click(TObject *Sender);
    void __fastcall btnCoordinatesClick(TObject *Sender);
    void __fastcall grdPointDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall chbShowInDetailClick(TObject *Sender);
    void __fastcall actExportSelectionToExcelExecute(TObject *Sender);
    void __fastcall actDegradationSectorSelectionExecute(TObject *Sender);
    void __fastcall actCoordinationPointsShowExecute(TObject *Sender);
    void __fastcall tmrInvalidateTimer(TObject *Sender);
    void __fastcall chbTwoUnwantedTxsClick(TObject *Sender);
    void __fastcall grdZonesDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall griddgDblClick(TObject *Sender);
    void __fastcall griddgDragDrop(TObject *Sender, TObject *Source, int X,
          int Y);
    void __fastcall griddgDragOver(TObject *Sender, TObject *Source, int X,
          int Y, TDragState State, bool &Accept);
    void __fastcall griddgDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall griddgKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall griddgKeyUp(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall griddgMouseUp(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall OnObjectSelection(TObject * Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift);
    void __fastcall MapMouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall MapMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
    void __fastcall MapMouseUp(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall SelectTxClick(TObject *Sender);
    void __fastcall SelectCpClick(TObject *Sender);
    void __fastcall mniDelAllAllotZonesClick(TObject *Sender);
    void __fastcall MapToolUsed(TObject *Sender,
          short ToolNum, double X1, double Y1, double X2, double Y2,
          double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
          VARIANT_BOOL *EnableDefault);
    void __fastcall btUnwantedListClick(TObject *Sender);
    void __fastcall cbxWantedTxDrawItem(TWinControl *Control, int Index,
          TRect &Rect, TOwnerDrawState State);
    void __fastcall actSetAsInterfereExecute(TObject *Sender);
    void __fastcall actSetAsObjectExecute(TObject *Sender);
    void __fastcall btProjectionClick(TObject *Sender);
    void __fastcall dpTimeChange(TObject *Sender);
    void __fastcall tpTimeChange(TObject *Sender);
    void __fastcall FormResize(TObject *Sender);
    void __fastcall actDayNightExecute(TObject *Sender);
    void __fastcall actDayNightUpdate(TObject *Sender);
    void __fastcall actDropEtalonZonesExecute(TObject *Sender);
    void __fastcall btNowClick(TObject *Sender);
    void __fastcall btNoonClick(TObject *Sender);
    void __fastcall btMidnightClick(TObject *Sender);
    void __fastcall btOpModeClick(TObject *Sender);
private:
    bool coordinationPointsShow;
    bool doSelectionStarted;
    AdditionalParams parameters;

    std::vector<CoordinationPoint> cordinationPoints;
    std::vector<MapPoint*> mapCPoints;
    typedef std::map<int, MapShape*> MapShapeMap;
    typedef std::multimap<int, MapShape*> MapShapeMulti;
    MapShapeMulti txs;
    MapShapeMap coverZones;
    MapShapeMap noiseZones;
    MapShapeMap interfZones;
    MapShapeMap interfZones2;

    TMouseMoveEvent oldMouseMove;
    TMouseEvent oldMouseDown;
    TMouseEvent oldMouseUp;

    int checkPointsLayer;
    bool ttAMType;

    void __fastcall ActivateMapSheet();
    void __fastcall addResult(TBCCalcResult* result);
    void __fastcall clearPointData();
    void __fastcall clearPoint2Data();
    void __fastcall CoordinationPointsFillCountryName(std::vector<CoordinationPoint>& cordinationPoints);
    void __fastcall CoordinationPointsGet(std::vector<CoordinationPoint>& cordinationPoints);
    void __fastcall CoordinationPointsHide();
    void __fastcall CoordinationPointsShow();
    void __fastcall fillChannelGrid();
    LPSAFEARRAY __fastcall getCheckPoints(ILISBCTxList*, /*ILISBCTx*, ILISBCTx*, */LPSAFEARRAY zone);	// User declarations
    AnsiString __fastcall nToAz(int n);
    void __fastcall saveResults();
    void __fastcall showTestPoints(int txId);
    void __fastcall checkIndex(int idx);
    void __fastcall fillCalcTxList();
    void __fastcall showCurTxZones();
    void __fastcall SetTxColor(int tx_id, TColor color);
    void __fastcall CreateTxSubMenu(TMenuItem* mni, MapShape* sp);
protected:
    bool interferenceIsFromBase;
    __fastcall TfrmSelection(TComponent* Owner);
    int FId;
    int FTxId;
    TPolarDiagramPanel *polarDiagramPanel;
    int currentTxIndex;
    int zeroTxIndex;
    TIBBlobStream *resultStream;

    double lastCpLon;
    double lastCpLat;

    TCOMIRSASpherics sphereCalc;

    void __fastcall addTx(int TxId);
    void __fastcall acceptChoice(Messages::TMessage& Message);
    void __fastcall ClearZonesCache(std::map<int, LPSAFEARRAY>& cacheMap);
    void __fastcall deleteTx(int TxId);
    AnsiString __fastcall GetTxName(ILISBCTx* tx);
    AnsiString __fastcall GetTxIdx(int i);
    void __fastcall NotImplemented();
    void __fastcall OffsetFill(TBCCalcResultType BCCalcResultType);
    virtual __fastcall ~TfrmSelection();
    void __fastcall DropCalculatedData(bool refreshMap = false);

    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptChoice)
    END_MESSAGE_MAP(TForm)
public:
    int CheckPointsCount;
    bool wasChanges;
    __fastcall TfrmSelection(TComponent* Owner, void* Id);
    void __fastcall ShowCheckPoint(double X, double Y);
    void __fastcall refresh();
    void __fastcall reload();
    void __fastcall refresh_row(int row);
    void __fastcall updateStatus();
    TCOMILISBCTxList txList;
    TCOMILISBCTxList curTxList;
    TCOMILISBCTxList curSfn; // assignments that belong to current allotment
    TCOMILISBCTxList curSfnUw1; // assignments that belong to unwanted allotment1
    TCOMILISBCTxList curSfnUw2; // assignments that belong to unwanted allotment2
    TCOMILISBCTxList selectedUnwanted;
    void GetCoverage(bool recalcAll);
    void __fastcall DrawTxs();
    void __fastcall CalcDuelInterfere();
    std::map<int, LPSAFEARRAY> coverage;
    std::map<int, LPSAFEARRAY> noiseLimited;
    std::map<int, LPSAFEARRAY> interfereLimited;
    std::map<int, LPSAFEARRAY> checkPoints;
    std::map<int, bool> modified;
    std::map<int, LPSAFEARRAY> interfereLimited2;
    std::map<int, StandRecord> standRecords;

    std::map<int, int> currentAllotZones;

    // allotments cache
    typedef std::map<int, AnsiString> IntToStrMap;      
    typedef std::map<AnsiString, std::set<int> > GroupsMap;     
    typedef std::map<AnsiString, int> StrToIntMap;
    typedef std::vector<AnsiString> TagsVector;

    IntToStrMap allots;                           // tx allotments
    GroupsMap txGroups;                           // txs grouped by allotment name
    GroupsMap allotGroups;                        // allots grouped by allotment name
    StrToIntMap curAllot;                         // actual allotment selected (from ones with the same name)
    TagsVector tags;                              // tags (two-part labels in column "¹")

    bool mapInitialized;

    bool sortingChanged;
    void __fastcall copyTxToBeforeBase(long TxId);    
    void __fastcall DrawCoverage();
    void __fastcall selectAnalyzeTx(int idx);
    void __fastcall drawResultZones();
    void __fastcall GetE(double lon, double lat);
    void __fastcall GetCoordination();
    std::auto_ptr<Graphics::TBitmap> useInCalcBitmap;
    std::auto_ptr<Graphics::TBitmap> showOnMapBitmap;
    std::auto_ptr<Graphics::TBitmap> zoneOverlapBitmap;
    std::auto_ptr<Graphics::TBitmap> isDayBitmap;
    static void __fastcall GetCoordZone (ILISBCTx* ptrTx, double* zone, int num = 36, TDateTime* stageTimes = NULL, int timesNum = 0);
    void __fastcall selectTxByID(int id);
    void __fastcall DoSelection(double X1, double Y1, double X2, double Y2);
    void __fastcall UpdateLines();
    void __fastcall UpdateTxNames();
    void __fastcall ArrangeAllotGroups();
    void __fastcall DelZones();
    int __fastcall GetId() { return FId; }
    void __fastcall GetSfn(ILISBCTxList* sfn, int allotPos);
    void __fastcall SetTime();
    void __fastcall CalcAllotRes(long txId_, TRSAGeoPoint chPoint, std::vector<double> *params, TControlPointCalcResult *res);
};

//---------------------------------------------------------------------------


class TBCCalcResult{
    TBCCalcResultType resultType;
    void *data;
    unsigned dataSize;
public:
    AnsiString desc;

    TBCCalcResult(TBCCalcResultType rt, unsigned ds = 0): resultType(rt), dataSize(ds) {
        switch(rt) {
            case rtOffset: data = new char[dataSize]; break;
            case rtERP: data = new char[dataSize]; break;
            default: data = NULL; break;
        }
    }
    ~TBCCalcResult() {
        switch(resultType) {
            case rtOffset: delete[] (char*)data; break;
            case rtERP: delete[] (char*)data; break;
            default: break;
        }
    }

    void *getData() { return data; };
    void exportToMemo(TMemo*);
};


//---------------------------------------------------------------------------
#endif
