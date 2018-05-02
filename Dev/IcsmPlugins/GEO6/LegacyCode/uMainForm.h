//---------------------------------------------------------------------------

#ifndef uMainFormH
#define uMainFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <Menus.hpp>
#include <StdActns.hpp>
#include <ActnMan.hpp>
#include <ToolWin.hpp>
#include <CustomizeDlg.hpp>
#include <ActnCtrls.hpp>
#include <BandActn.hpp>
#include <AppEvnts.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TfrmMain : public TForm
{
__published:	// IDE-managed Components
    TMainMenu *MainMenu1;
    TActionList *ActionList1;
    TFileExit *actFileExit;
    TStatusBar *StatusBar1;
    TAction *actListAnalogRadioSystem;
    TAction *actListAnalogTeleSystem;
    TAction *actListDigitalSystem;
    TAction *actListChannel;
    TAction *actListSystemCast;
    TAction *actListCarrierGuardInterval;
    TAction *actListOffsetCarryFreqTVA;
    TAction *actListDistrict;
    TAction *actListMinFieldStrength;
    TAction *actListRadioService;
    TAction *actListTypeReceive;
    TAction *actListUncompatibleChannels;
    TMenuItem *miFile;
    TMenuItem *N2;
    TMenuItem *N3;
    TAction *actAbout;
    TMenuItem *N4;
    TAction *actListEquipment;
    TAction *actListCountry;
    TMenuItem *miDictDb;
    TMenuItem *actDictListRadioSyst1;
    TMenuItem *actListAnalTeleSyst1;
    TMenuItem *actDictDigitSyst1;
    TMenuItem *actDictChannels1;
    TMenuItem *actListCountries1;
    TMenuItem *actDictEquipment1;
    TWindowClose *WindowClose1;
    TWindowCascade *WindowCascade1;
    TWindowMinimizeAll *WindowMinimizeAll1;
    TWindowArrange *WindowArrange1;
    TMenuItem *mniForms;
    TMenuItem *N7;
    TMenuItem *N8;
    TMenuItem *N9;
    TMenuItem *N10;
    TAction *actListArea;
    TMenuItem *N11;
    TMenuItem *N12;
    TMenuItem *N14;
    TAction *actListCitiy;
    TAction *actListStreet;
    TMenuItem *N15;
    TAction *actListAccountCondition;
    TMenuItem *N18;
    TMenuItem *miTools;
    TMenuItem *N20;
    TMenuItem *N21;
    TMenuItem *N22;
    TMenuItem *N23;
    TMenuItem *N24;
    TMenuItem *N25;
    TMenuItem *N26;
    TMenuItem *N27;
    TMenuItem *N28;
    TAction *actListStand;
    TMenuItem *miTxDb;
    TMenuItem *N30;
    TMenuItem *N13;
    TAction *actListOwner;
    TMenuItem *N31;
    TAction *actListBank;
    TMenuItem *N33;
    TAction *actListTelecomOrganization;
    TMenuItem *N34;
    TAction *actNewTx;
    TMenuItem *N35;
    TAction *actListBlockDAB;
    TMenuItem *N37;
    TCustomizeDlg *CustomizeDlg1;
    TAction *actListTPOnBorder;
    TMenuItem *N60;
    TAction *actListLicense;
    TMenuItem *actListLicense1;
    TAction *actListSynchroNetTypes;
    TAction *actListSynchroNets;
    TMenuItem *N61;
    TMenuItem *N62;
    TMenuItem *N63;
    TMenuItem *N64;
    TMenuItem *N65;
    TAction *actExplorer;
    TMenuItem *N67;
    TCustomizeActionBars *actCustomizeActionBars;
    TApplicationEvents *ApplicationEvents1;
    TMenuItem *miView;
    TAction *actMap;
    TMenuItem *N66;
    TAction *actParams;
    TMenuItem *N69;
    TWindowTileHorizontal *WindowTileHorizontal1;
    TWindowTileVertical *WindowTileVertical1;
    TMenuItem *N70;
    TMenuItem *N71;
    TAction *actUserActivityLog;
    TMenuItem *N72;
    TMenuItem *N73;
    TToolBar *tbrShortcut;
    TAction *actSearch;
    TMenuItem *N75;
    TAction *actExport;
    TAction *actImport;
    TAction *actPrint;
    TMenuItem *actPrint1;
    TAction *actListFrequencyGrid;
    TMenuItem *N32;
    TAction *actShowPlanning;
    TMenuItem *mniFormsSeparator;
    TMenuItem *N6;
    TMenuItem *miDocs;
    TAction *actDocuments;
    TMenuItem *miDocuments;
        TSplitter *leftSplitter;
        TSplitter *rightSplitter;
        TPanel *pnlLeftDockPanel;
        TPanel *pnlRightDockPanel;
    TProgressBar *pb;
    TMenuItem *miRrc06;
    TAction *actListDigAllocations;
    TMenuItem *N29;
    TAction *actListDigSubareas;
    TMenuItem *N76;
    TAction *actImpRrc06;
    TMenuItem *miMemInfo;
    TMenuItem *miUnloadAll;
    TMenuItem *miSep;
    TMenuItem *miUnloadAllForced;
    TAction *actMemInfo;
    TAction *actUnloadAll;
    TAction *actUnloadAllForced;
    TAction *actList;
    TAction *actTxList;
    TMenuItem *N1;
    void __fastcall actAboutExecute(TObject *Sender);
    void __fastcall actListExecute(TObject *Sender);
    void __fastcall actNewTxExecute(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall actExplorerExecute(TObject *Sender);
    void __fastcall actParamsExecute(TObject *Sender);
    void __fastcall actSearchExecute(TObject *Sender);
    void __fastcall FormDestroy(TObject *Sender);
    void __fastcall mniFormsClick(TObject *Sender);
    void __fastcall mniChildWindowClick(TObject *Sender);
    void __fastcall actShowPlanningExecute(TObject *Sender);
    void __fastcall actDocumentsExecute(TObject *Sender);
        void __fastcall DockPanelDockDrop(TObject *Sender,
          TDragDockObject *Source, int X, int Y);
        void __fastcall DockPanelDockOver(TObject *Sender,
          TDragDockObject *Source, int X, int Y, TDragState State,
          bool &Accept);
        void __fastcall DockPanelGetSiteInfo(TObject *Sender,
          TControl *DockClient, TRect &InfluenceRect, TPoint &MousePos,
          bool &CanDock);
        void __fastcall DockPanelUnDock(TObject *Sender, TControl *Client,
          TWinControl *NewTarget, bool &Allow);
        void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
    void __fastcall StatusBar1Resize(TObject *Sender);
    void __fastcall actImpRrc06Execute(TObject *Sender);
    void __fastcall MemoryInfoDblClick(TObject *Sender);
    void __fastcall actMemInfoExecute(TObject *Sender);
    void __fastcall actUnloadAllExecute(TObject *Sender);
    void __fastcall actUnloadAllForcedExecute(TObject *Sender);
    void __fastcall actUnloadAllForcedUpdate(TObject *Sender);
    void __fastcall actTxListExecute(TObject *Sender);
private:	// User declarations
public:		// User declarations
    void ShowDockPanel(TPanel* APanel, bool MakeVisible, TControl* Client);
    __fastcall TfrmMain(TComponent* Owner);
    HANDLE __fastcall showExplorer();
    void __fastcall FormListTxRefresh();
    void __fastcall FormListStandRefresh();
    void __fastcall DisplayCalcParams();
    TMenuItem* __fastcall AddMenu(TMenuItem* parent, String caption, TAction* action, int tag, int index = -1);
    TMenuItem* __fastcall AddListMenu(TMenuItem* parent, int tag, int index = -1);
    __fastcall AddTxListMenu(TMenuItem* parent, int sc, int index = -1);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmMain *frmMain;
//---------------------------------------------------------------------------
#endif
