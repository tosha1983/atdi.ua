#include <vcl.h>

#include <DBGrids.hpp>
#include <fstream>
#include <iomanip.h>
#include <math>
#include <safearry.h>
#include <set>
#include <values.h>
#include <SysUtils.hpp>
#pragma hdrstop 

#include "RSAGeography_TLB.h"

#include "uSelection.h"
#include "FormProvider.h"
#include "TxBroker.h"
#include "uAnalyzer.h"
#include "uDuelResult.h"
#include "uEnterCoordDlg.h"
#include "uExplorer.h"
#include "uExportDlg.h"
#include "uFrmPoint.h"
#include "uFrmTxBase.h"
#include "uLayoutMngr.h"
#include "uListTransmitters.h"
#include "uMainForm.h"
#include "uOffsetRangeDlg.h"
#include "uParams.h"
#include "uSectorDlg.h"
#include "uTable36.h"
#include "uListDigAllotments.h"
#include "tempvalues.h"
#include "uDlgEminAndNote.h"
#include "uDlgMapConf.h"
#include "uLogger.h"
#include "uSelectStations.h"
#include "uSelectionGridProxi.h"
#include "uCoordZoneFieldStr.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uWhere"
#pragma link "uLisObjectGrid"
#pragma link "CustomMap"
#pragma resource "*.dfm"

#ifdef StrToInt
#undef StrToInt
#endif

char* szETxIndexOut = "Індекс списку передавачів поза межами [%d]";
char* szDeleteTx = "Видалити передавач [%s] із вибірки?";
char* szRemoveAllUnused = "Видалити із вибірки всі передавачі, що не використовуються?";

char* szNoTx = "Неможливо вибрати об'єкт розрахунку";

char* szCalcNoiseArea = "Розрахувати зону, що обмежена шумами?";
char* szCalcInterfereArea = "Розрахувати зону, що обмежена завадами?";
char* szCalcDuel = "Розрахувати умови дуелі?";
char* szCalcCoverSector = "Розрахувати сектор розподілу напруженості поля?";
char* szSetTP = "Сформувати набір контрольних точок зі старшими завадами?";
char* szSaveList = "";
char* szSaveTx = "";
char* szSaveRes = "";
char* szDuelInterfere = "Розрахувати дуельні завади?";
char* szDeleteResultPage = "Видалити сторінку результатів?";

char* szCalcInterf_WantedError = "Помилка '%s' при розрахунку завад від бажаного передавача: %s";
char* szCalcInterf_UnwantedError = "Помилка '%s' при розрахунку завад від небажаних передавачів: %s";

char* szPickResultSheet = "Укажіть спочатку сторінку, з якої потрібно зберігти результати";

char* szSaveResultsFilter = "Результати розрахунку (*.rez)|*.rez|Всі файли (*.*)|*.*";
char* szSaveTxFilter = "Передавачі (*.tx)|*.tx|Всі файли (*.*)|*.*";
char* szSaveListFilter = "Списки передавачів (*.lst)|*.lst|Всі файли (*.*)|*.*";

char* szSaveResultsRequest = "Зберігти поточний набір результатів?";

char *resultNames[] = { "ЗП", "ЗО", "Дуель", "Сектор", "ЗНЧ", "ЕВП", "Дуель З" };

char *szStrengthSectorCaption = "Розподіл напруженості поля передавача [%s] в секторі %.1f-%.1f град, %.1f-%.1fкм";

static char *duelColNamesTtAM[] = {"Точка", "Е зав", "ЗВ", "Е вик", "Е мін"};
static char *duelColNames[] = {"Точка", "Е зав", "ЗВ", "D ант", "Е вик", "Е мін", "Тип зав"};

//  color constants
TColor ccTx = clMaroon;
TColor ccTxSelected = clBlue;
TColor ccTxZero = clRed;
TColor ccZoneCover = clGreen;
//TColor ccIsDay = 0x0000F0F0;
TColor ccIsDay = (TColor)0x0000DFDF;
TColor ccZoneCoverNotUsed = clSilver;
TColor ccZoneNoise = clBlue;
TColor ccECover = clRed;
TColor ccEPoint = clTeal;
TColor ccZoneCoord = clRed;
TColor ccDuelAxe = clMaroon;

enum AnalyzisTools { miGetSectorTool = miLastBmTool };
//
//---------------------------------------------------------------------------
__fastcall TfrmSelection::TfrmSelection(TComponent* Owner)
    : TForm(Owner), mapInitialized(false)
{
    coordinationPointsShow = false;
}

__fastcall TfrmSelection::~TfrmSelection()
{
    ClearZonesCache(coverage);
    ClearZonesCache(noiseLimited);
    ClearZonesCache(interfereLimited);
    ClearZonesCache(interfereLimited2);
    ClearZonesCache(checkPoints);
}

__fastcall TfrmSelection::TfrmSelection(TComponent* Owner, void* SelId)
    : TForm(Owner), FId((int)SelId), mapInitialized(false)
{
    doSelectionStarted = false;
    cmf->Align = alClient;
    cmf->PopupMenu = pmnTx;
    cmf->ColorStationUnH = clMaroon;
    cmf->ColorStationH = clBlue;
    cmf->ColorCoverageZoneUnH = (TColor)BCCalcParams.lineColorZoneCover;
    cmf->ColorLinkUnH = clTeal;
    cmf->ColorRelLineUnH = clRed;
    cmf->ColorCoordZoneUnH = clRed;

    cmf->WidthStation = 5;
    cmf->WidthLink = 1;
    cmf->WidthRelLine = 2;
    cmf->WidthCoordZone = 2;
    cmf->WidthCoverageZone = 1;

    tbtHideBtns->Action = cmf->bmf->actPanButtons;
    tbtHideBtns->ImageIndex = 28;

    LayoutManager.EnsureShortcut(this);

    txtNo->Top = txtDesc->Top - 2;

    sortingChanged = false;

    pb->Top = 2;
    pb->Left = 0;
    pb->Parent = StBr;
    pb->Height = StBr->Height - 2;
    pb->Width = StBr->Width;

    lblDataParam1->Caption = "Потужність";
    lblDataParam2->Caption = "Н підвісу";
    lblDataParam3->Caption = "Поляризація";
    lblDataParam4->Caption = "Канал/Блок";
    lblDataParam5->Caption = "Частота";
    lblDataParam6->Caption = "ЗНЧ";

    AnsiString emptyVal("-/-");

    lblDataVal1->Caption = emptyVal;
    lblDataVal2->Caption = emptyVal;
    lblDataVal3->Caption = emptyVal;
    lblDataVal4->Caption = emptyVal;
    lblDataVal5->Caption = emptyVal;
    lblDataVal6->Caption = emptyVal;

    lblDataVal1->Font->Color = clMaroon;
    lblDataVal2->Font->Color = clMaroon;
    lblDataVal3->Font->Color = clMaroon;
    lblDataVal4->Font->Color = clMaroon;
    lblDataVal5->Font->Color = clMaroon;
    lblDataVal6->Font->Color = clMaroon;

    lblPointData1->Font->Color = clMaroon;
    lblPointData2->Font->Color = clMaroon;
    lblPointData3->Font->Color = clMaroon;
    lblPointData8->Font->Color = clMaroon;
    lblPointData9->Font->Color = clMaroon;

    lblPoint2Data2->Font->Color = clMaroon;
    lblPoint2Data3->Font->Color = clMaroon;
    lblPoint2Data8->Font->Color = clMaroon;
    lblPoint2Data9->Font->Color = clMaroon;


    lblAData->Font->Color = clMaroon;
    lblA->Font->Style = lblA->Font->Style << fsBold;
    lblBData->Font->Color = clMaroon;
    lblB->Font->Style = lblB->Font->Style << fsBold;

    lblEminAData->Font->Color = clMaroon;
    lblEminBData->Font->Color = clMaroon;

    currentTxIndex = -1;
    zeroTxIndex = -1;
    txtNo->Font->Style = txtNo->Font->Style << fsBold;
    txtNo->Font->Size = 10;

    lblUnwantedTx->Font->Color = (TColor)BCCalcParams.lineColorZoneInterfere;
    lblUnwantedTx2->Font->Color = (TColor)BCCalcParams.lineColorZoneInterfere2;
    chbTwoUnwantedTxsClick(this);

    lblCoordination->Caption = "";
    lblCoordination->Font->Size = 10;
    lblCoordination->Font->Color = clNavy;

    sqlTxId->Close();
    sqlTxId->Params->Vars[0]->AsInteger = FId;
    sqlTxId->ExecQuery();
    if (!sqlTxId->Eof)
        FTxId = sqlTxId->Fields[0]->AsInteger;
    else
        throw *(new Exception(szNoTx));
    sqlTxId->Close();

    panCalcResult->Width = panCalcResult->Width + 1;
    //panGraph->Height = panGraph->Width;
    polarDiagramPanel = new TPolarDiagramPanel(panGraph);
    polarDiagramPanel->Parent = panGraph;
    polarDiagramPanel->BevelOuter = bvNone;
    polarDiagramPanel->Left = 20;
    polarDiagramPanel->Width = panGraph->Width - polarDiagramPanel->Left * 2;
    polarDiagramPanel->Top = 10;
    polarDiagramPanel->Height = polarDiagramPanel->Width;
    polarDiagramPanel->Anchors << akRight << akBottom;
    polarDiagramPanel->SendToBack();

    grdZones->Cells[0][0] = "Азим.";
    grdZones->Cells[1][0] = "Теор.";
    grdZones->Cells[2][0] = "без зав.";
    grdZones->Cells[3][0] = "із зав.";
    grdZones->Height = tshZone->Height - grdZones->Top;

    grdPoint->Cells[1][0] = "без завади";
    grdPoint->Cells[2][0] = "із завадою";
    grdPoint->Cells[0][1] = "Ймовірність";
    grdPoint->Cells[0][2] = "R зони, km";
    grdPoint->Cells[0][3] = "Е вик, dB";

    txList.CreateInstance(CLSID_LISBCTxList);
    curTxList.CreateInstance(CLSID_LISBCTxList);
    curSfn.CreateInstance(CLSID_LISBCTxList);
    curSfnUw1.CreateInstance(CLSID_LISBCTxList);
    curSfnUw2.CreateInstance(CLSID_LISBCTxList);
    selectedUnwanted.CreateInstance(CLSID_LISBCTxList);

    grid->SetQuery("select TRANSMITTERS_ID ID, USED_IN_CALC, DISTANCE, SORTINDEX, RESULT "
                    "from SELECTEDTRANSMITTERS "
                    "where SELECTIONS_ID = "+IntToStr(FId)+
                    " and TRANSMITTERS_ID <> "+IntToStr(FTxId)+
                    "order by SORTINDEX");
    TCOMILISBCTx tx (txBroker.GetTx(FTxId, dmMain->GetObjClsid(dmMain->GetSc(FTxId))), true);
    TBCTxType tt = tx.systemcast;
    grid->ClearColumns();
    grid->AddColumn("№",          "","No",              taCenter,        ptString,  40);
    grid->AddColumn("Р",          "","IsUsedInCalc",    taCenter,        ptString,  18);
    grid->AddColumn("К",          "","IsShownOnMap",    taCenter,        ptString,  18);
    if (tt != ttAM)
        grid->AddColumn("З",          "","ZoneOverlapped",  taCenter,        ptString,  18);
    else
        grid->AddColumn("Д",          "","IsDay",  taCenter,        ptString,  18);
    grid->AddColumn("Опора",      "","Name",            taLeftJustify,   ptString,  120);
    if (tt != ttAM)
        grid->AddColumn("К/Ч/Б",      "","Channel",         taCenter,        ptString,  35);
    else
        grid->AddColumn("Ч,кГц",      "","Channel",         taCenter,        ptString,  45);
    grid->AddColumn("Відстань",   "","Dist",            taRightJustify,  ptString,  50);
    grid->AddColumn("Азимут",     "","Azm",             taRightJustify,  ptString,  45);
    if (tt!=ttAM)
    {
        grid->AddColumn("ЕВП",        "","ErpMax",          taRightJustify,  ptString,  45);
        grid->AddColumn("Н эфф",      "","EahMax",          taRightJustify,  ptString,  45);
        grid->AddColumn("Пол",        "","Polar",           taCenter,        ptString,  30);
        grid->AddColumn("ЗНЧ",        "","Offset",          taCenter,        ptString,  30);
    } else {
    }
    grid->AddColumn("Стан",       "","State",           taCenter,        ptString,  45);
    grid->AddColumn("Від нас",    "","IntfWant",        taRightJustify,  ptString,  60);
    grid->AddColumn("Тип",        "","IntfWantKind",    taCenter,        ptString,  18);
    grid->AddColumn("Нам",        "","IntfUnwant",      taRightJustify,  ptString,  60);
    grid->AddColumn("Тип",        "","IntfUnwantKind",  taCenter,        ptString,  18);
    grid->AddColumn("Рег",        "","RegNum",          taLeftJustify,   ptString,  35);
    grid->AddColumn("№прд",       "","AdmId",           taLeftJustify,   ptString,  35);
    grid->AddColumn("Широта",     "","Lat",             taLeftJustify,   ptString,  65);
    grid->AddColumn("Довгота",    "","Lon",             taLeftJustify,   ptString,  65);
    grid->AddColumn("Система",    "","Sys",             taCenter,        ptString,  50);

   // char **duelCol;
    int duelColCount;
    if (tt!=ttAM)
    {
        grid->AddColumn("Стандарт",   "","Color",           taLeftJustify,   ptString,  50);
   //     duelCol = duelColNames;
        grdDuelPoints->ColCount = 7;
        ttAMType = false;
        duelColCount = sizeof(duelColNames)/sizeof(char*);
        for (int i = 0; i < duelColCount; i++)
            grdDuelPoints->Cells[0][i] = duelColNames[i];
    }
    else
    {
       // duelCol = duelColNamesTtAM;
        grdDuelPoints->ColCount = 5;
        ttAMType = true;
        duelColCount = sizeof(duelColNamesTtAM)/sizeof(char*);
        for (int i = 0; i < duelColCount; i++)
            grdDuelPoints->Cells[0][i] = duelColNamesTtAM[i];
    }
    grdDuelPoints->Cells[1][0] = "-1";
    grdDuelPoints->Cells[2][0] = "-2";
    grdDuelPoints->Cells[3][0] = "-3";
    grdDuelPoints->Cells[4][0] = "-4";
    grdDuelPoints->RowCount = duelColCount;
    grdDuelPoints->Height = tshDuel->Height - grdDuelPoints->Top;


    for (unsigned i = 0; i < grid->columnsInfo.size(); i++)
        grid->columnsInfo[i].customSort = true;

    if (!selProxi)
        selProxi = new TSelectionProxi(Application);
    grid->SetProxi(selProxi);

    if (tt == ttAM)
    {
        dpTime->Date = Date();
        tpTime->Time = Time();
        SetTime();
    } else
        pnTime->Visible = false;

    Update();

    wasChanges = false;

    reload();

    dstResults->Close();
    dstResults->Params->Vars[0]->AsInteger = FId;
    dstResults->Transaction->CommitRetaining();
    dstResults->Open();
    resultStream = (TIBBlobStream*)dstResults->CreateBlobStream(dstResultsRESULT, bmReadWrite);

    pcSelection->ActivePage = tshSelection;
    pcCalcResult->ActivePage = tshZone;
    ActiveControl = grid;

    Graphics::TBitmap* bmp = NULL;
    useInCalcBitmap.reset(bmp = new Graphics::TBitmap());
    bmp->Width = 5;
    bmp->Height = bmp->Width;
    bmp->Canvas->Brush->Style = bsSolid;
    bmp->Canvas->Brush->Color = clBlack;
    bmp->Canvas->Ellipse(0, 0, bmp->Height, bmp->Width);
    bmp->Canvas->FloodFill(bmp->Width / 2, bmp->Height / 2, clBlack, fsBorder);

    showOnMapBitmap.reset(bmp = new Graphics::TBitmap());
    bmp->Width = 5;
    bmp->Height = bmp->Width;
    bmp->Canvas->Brush->Style = bsSolid;
    bmp->Canvas->Brush->Color = clBlue;
    bmp->Canvas->Ellipse(0,0,bmp->Height,bmp->Width);
    bmp->Canvas->FloodFill(bmp->Width / 2, bmp->Height / 2, ccTx, fsBorder);

    zoneOverlapBitmap.reset(bmp = new Graphics::TBitmap());
    bmp->Width = 5;
    bmp->Height = bmp->Width;
    bmp->Canvas->Brush->Style = bsSolid;
    bmp->Canvas->Brush->Color = clRed;
    bmp->Canvas->Ellipse(0,0, bmp->Height, bmp->Width);
    bmp->Canvas->FloodFill(bmp->Width / 2, bmp->Height / 2, ccZoneCover, fsBorder);

    isDayBitmap.reset(bmp = new Graphics::TBitmap());
    bmp->Width = 9;
    bmp->Height = bmp->Width;
    bmp->Canvas->Brush->Style = bsSolid;
    bmp->Canvas->Brush->Color = ccIsDay;
    bmp->Canvas->Pen->Color = ccIsDay;
    bmp->Canvas->Ellipse(2,2, bmp->Height-2, bmp->Width-2);
    bmp->Canvas->FloodFill(bmp->Width / 2, bmp->Height / 2, ccIsDay, fsBorder);
    bmp->Canvas->MoveTo(bmp->Width / 2, 0);
    bmp->Canvas->LineTo(bmp->Width / 2, bmp->Height);
    bmp->Canvas->MoveTo(0, bmp->Height / 2);
    bmp->Canvas->LineTo(bmp->Width, bmp->Height / 2);
    bmp->Canvas->MoveTo(1, 1);
    bmp->Canvas->LineTo(bmp->Width - 1, bmp->Height - 1);
    bmp->Canvas->MoveTo(bmp->Width - 1, 1);
    bmp->Canvas->LineTo(1, bmp->Height - 1);

    cmf->omsCallBack = OnObjectSelection;

    lastCpLon = 0.0;
    lastCpLat = 0.0;

    try {
        OLECHECK(sphereCalc.CreateInstance(CLSID_RSASpherics));
    } catch (Exception &e) {
        MessageBox(NULL, ("Ошибка создания объекта CLSID_RSASpherics.\n"
                          "Проверьте, зарегистрирована ли RSAGeography.dll:\n\n" +e.Message).c_str(), "Ошибка", MB_ICONHAND);
    }

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::FormClose(TObject *, TCloseAction &Action)
{
    try
    {
        Action = caFree;
        LayoutManager.DeleteShortcut(this);
    }
    catch(...)
    {
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::reload()
{
    TempCursor oldCursor (crHourGlass);
    bool oldSimplePanel = frmMain->StatusBar1->SimplePanel;
    frmMain->StatusBar1->SimplePanel = true;

    frmMain->StatusBar1->SimpleText = "Завантаження списку...";
    frmMain->StatusBar1->Update();

    try
    {
        txList.Clear();
        TCOMILISBCTx tx(txBroker.GetTx(FTxId, dmMain->GetObjClsid(dmMain->GetSc(FTxId))), true);

        txList.AddTx(tx);
        txList.set_TxUseInCalc(0, true);

        ibdsRefresh->Transaction->CommitRetaining();
        ibdsRefresh->Close();
        ibdsRefresh->Params->Vars[0]->AsInteger = FId;
        ibdsRefresh->Prepare();
        ibdsRefresh->Open();
        ibdsRefresh->FetchAll();

        frmMain->pb->Max = ibdsRefresh->RecordCount;
        frmMain->pb->Min = 0;
        frmMain->pb->Position = 0;
        frmMain->pb->Show();

        ibdsRefresh->First();

        //загрузка дополнительных параметров
        {
            std::auto_ptr<TIBQuery> sql(new TIBQuery(this));
              sql->Database = dmMain->dbMain;
              sql->SQL->Text = "select RESULT from SELECTIONS where ID = " + IntToStr(FId);
              sql->Transaction = dmMain->trMain;
            sql->Open();

            TStream *stream = sql->CreateBlobStream(sql->Fields->FieldByName("RESULT"), bmRead);
            try
            {
                stream->Read(&parameters, sizeof(parameters));

                if ( parameters.sorting == sortFrom )
                    StBr->Panels->Items[2]->Text = "Від нас";
                else if ( parameters.sorting == sortTo)
                    StBr->Panels->Items[2]->Text = "Нам";
            }
            __finally
            {
                delete stream;
            }
        }

        try {

            while(!ibdsRefresh->Eof)
            {//загрузка основных параметров
                TCOMILISBCTx tx;
                int TxId = ibdsRefreshTRANSMITTERS_ID->AsInteger;
                tx.Bind(txBroker.GetTx(TxId, dmMain->GetObjClsid(ibdsRefreshENUMVAL->AsInteger)), true);

                int idx = txList.AddTx(tx);
                if (ibdsRefreshENUMVAL->AsInteger == ttAllot)
                {
                    txList.set_TxUseInCalc(idx, false);
                    txList.set_TxShowOnMap(idx, true);
                } else {
                    txList.set_TxUseInCalc(idx, ibdsRefreshUSED_IN_CALC->AsInteger);
                    txList.set_TxShowOnMap(idx, ibdsRefreshUSED_IN_CALC->AsInteger);
                }
                txList.set_TxUnwantInterfere(idx, ibdsRefreshE_UNWANT->AsFloat);
                txList.set_TxWantInterfere(idx, ibdsRefreshE_WANT->AsFloat);
                txList.set_TxZoneOverlapping(idx, ibdsRefreshZONE_OVERLAPPING->AsInteger);
                txList.set_TxAzimuth(idx, ibdsRefreshAZIMUTH->AsFloat);
                txList.set_TxDistance(idx, ibdsRefreshDISTANCE->AsInteger);

                ibdsRefresh->Next();

                frmMain->pb->StepBy(1);
                frmMain->pb->Update();
            }
        } __finally {
        }

        frmMain->StatusBar1->SimpleText = "Завантаження передавачiв...";
        frmMain->StatusBar1->Update();
        txBroker.EnsureList(txList, frmMain->pb);

        ArrangeAllotGroups();
        // again - allotments could be added
        txBroker.EnsureList(txList, frmMain->pb);

        wasChanges = false;

        if ( parameters.NewSelection )
        {
            CalcDuelInterfere();

            //авто обрезание выборки
            if ( BCCalcParams.SelectionAutotruncation )
            {
                actRemoveLessThanZeroExecute(this);
                refresh();
            }
        }
        else if (BCCalcParams.duelAutoRecalc)
        {
            frmMain->StatusBar1->SimpleText = "Розрахунок дуельних завад...";

            frmMain->pb->Update();

            frmMain->StatusBar1->Update();
            CalcDuelInterfere();
        }

        //  подготовить список Ид опор и извлечь опоры
        //  можно было сразу записывать в строку, однако вероятны повторы,
        //  поэтому сначала использован класс std::set<long>
        frmMain->StatusBar1->SimpleText = "Список опор...";
        frmMain->StatusBar1->Update();

        std::set<long> standIdSet;
        for (int i = 0; i < txList.Size; i++) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            standIdSet.insert(tx.stand_id);

            frmMain->pb->StepBy(1);
            frmMain->pb->Update();
        }

        frmMain->pb->Hide();

        frmMain->StatusBar1->SimpleText = "Загрузка опор...";
        frmMain->StatusBar1->Update();

        try {
            dmMain->cacheSites(standIdSet, standRecords, true);
        } catch (Exception &e) {
            e.Message = AnsiString("Помилка при формуванні списку опор:\n") + e.Message;
            Application->ShowException(&e);
        }
        
    } __finally {
        frmMain->StatusBar1->SimpleText = "";
        frmMain->StatusBar1->SimplePanel = oldSimplePanel;
        frmMain->StatusBar1->Update();
    }

    refresh();

    sortingChanged = false;
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::refresh()
{
    grid->Refresh();
    updateStatus();
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::refresh_row(int row)
{
    if (row > 0)// row is idx of tx in list
    {
        grid->RefreshRow(row-1);
        updateStatus();
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::sgrSelectionDragOver(TObject *Sender,
      TObject *Source, int X, int Y, TDragState State, bool &Accept)
{
    TDBGrid* grd;
    if (
        (grd = dynamic_cast<TDBGrid*>(Source))
        && (grd->Name == "dgrList" && (dynamic_cast<TfrmListTransmitters*>(grd->Owner)
                                        || dynamic_cast<TfrmListDigAllotments*>(grd->Owner))
        || grd->Name == "grdTx")
    )
        Accept = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::sgrSelectionDragDrop(TObject *Sender,
      TObject *Source, int X, int Y)
{
    TDBGrid* grd;
    if (
        (grd = dynamic_cast<TDBGrid*>(Source))
        && (grd->Name == "dgrList" && (dynamic_cast<TfrmListTransmitters*>(grd->Owner)
                                        || dynamic_cast<TfrmListDigAllotments*>(grd->Owner))
        || grd->Name == "grdTx")
        )
    {
        //  grd - сетка с передатчиками. нулевое поле соотв. датасета содержит Ид передатчика.
        TDataSet *ds = grd->DataSource->DataSet;
        TBookmarkList *bml = grd->SelectedRows;
        TBookmark bm = ds->GetBookmark();
        ds->DisableControls();
        try {
            for (int i=0; i < bml->Count; i++)
            {
                ds->GotoBookmark((void *)bml->Items[i].c_str());
                addTx(ds->Fields->Fields[0]->AsInteger);
            //for (ds->First(); !ds->Eof; ds->Next())
            //    if (bml->CurrentRowSelected)
            }
        } __finally {
            ds->GotoBookmark(bm);
            ds->FreeBookmark(bm);
            ds->EnableControls();
        }
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::addTx(int TxId)
{
    sqlAddTx->Close();
    sqlAddTx->ParamByName("SELECTIONS_ID")->AsInteger = FId;
    sqlAddTx->ParamByName("TRANSMITTERS_ID")->AsInteger = TxId;
    sqlAddTx->ParamByName("USED_IN_CALC")->AsInteger = true;
    sqlAddTx->ExecQuery();
    sqlAddTx->Transaction->CommitRetaining();

    TCOMILISBCTx tx(txBroker.GetTx(TxId, dmMain->GetObjClsid(dmMain->GetSc(TxId))), true);

    int pos = txList.AddTx(tx);
    txList.set_TxUseInCalc(pos, true);
    txList.set_TxShowOnMap(pos, true);
    txList.set_TxZoneOverlapping(pos, 0);
    txList.set_TxAzimuth(pos, 0);
    txList.set_TxDistance(pos, 0);
    txList.set_TxUnwantedKind(pos, 'T');
    txList.set_TxUnwantInterfere(pos, -999);
    txList.set_TxWantedKind(pos, 'T');
    txList.set_TxWantInterfere(pos, -999);

    std::set<long> standIdSet;
    standIdSet.insert(tx.stand_id);
    dmMain->cacheSites(standIdSet, standRecords, false);

    // new tx index-tag
    int txIdx = txList.Size-1;
    while (txIdx > tags.size())
    {
        //prev index
        String strIdx = tags.empty() ? String("0") : tags[txIdx-2]; //dangerous but true
        if (strIdx.IsEmpty())
            strIdx = "0";
        //remove SFN subindex
        int commaPos = strIdx.Pos(".");
        if (commaPos > 0)
            strIdx.Delete(commaPos, strIdx.Length() - commaPos + 1);
        //fire!
        try { strIdx = IntToStr(strIdx.ToInt()+1); } catch (...) {};
        tags.push_back(strIdx);
    }

    //refresh_row(txList.Size); было так, но по логике нужно отнимать 1
    refresh_row(txIdx);
    grid->Refresh();
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::deleteTx(int TxId)
{
    sqlDeleteTx->Close();
    sqlDeleteTx->ParamByName("SEL_ID")->AsInteger = FId;
    sqlDeleteTx->ParamByName("TX_ID")->AsInteger = TxId;
    sqlDeleteTx->ExecQuery();
    sqlDeleteTx->Transaction->CommitRetaining();
    txList.RemoveId(TxId);
    curTxList.RemoveId(TxId);
    curSfn.RemoveId(TxId);
    curSfnUw1.RemoveId(TxId);
    curSfnUw2.RemoveId(TxId);

    wasChanges = true;

    refresh();

    selectAnalyzeTx(grid->dg->Row + 1);
}

void __fastcall TfrmSelection::actAddTxExecute(TObject *Sender)
{
    FormProvider.ShowTxList(this->Handle, 0, 0 /* все */);

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actRemoveTxExecute(TObject *Sender)
{
    int txIdx = currentTxIndex;
    if (txIdx < 1)
        txIdx = grid->dg->Row + 1;
    if (txIdx < 1)
        return;
    TCOMILISBCTx tx(txList.get_Tx(txIdx), true);

    if (Application->MessageBox(AnsiString().sprintf(szDeleteTx, AnsiString(tx.station_name).c_str()).c_str(),
                                Application->Title.c_str(),
                                MB_ICONQUESTION | MB_YESNO) == IDYES) {
        tags.erase(&tags[txIdx-1]);
        deleteTx(tx.id);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actRefreshExecute(TObject *Sender)
{
    reload();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actUsedInCalcExecute(TObject *Sender)
{
    bool fromMap = pcSelection->ActivePage == tshMap && cmf->Visible;

    int idx = fromMap ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    checkIndex(idx);

    bool used = txList.get_TxUseInCalc(idx);
    bool show = txList.get_TxShowOnMap(idx);

    TCOMILISBCTx tx(txList.get_Tx(idx), true);
    // in case of Allotment operate with all Txs of it
    typedef std::set<int> IdList;
    IdList idList;
    idList.insert(idx);
    if (tx.systemcast == ttAllot && idx < tags.size())
    {
        String tag = tags[idx-1];
        // just in case when 1 item is allot and the 0 is it's tx, take previous item into account
        if (idx == 1 && tags [0] == "0")
            idList.insert(0);
        for (int i = idx; i < tags.size() && tags[i].Pos(tag) == 1; i++)
            idList.insert(i+1);
    }

    used = !used;
    for (IdList::iterator i = idList.begin(); i != idList.end(); i++)
    {
        if (!fromMap) {
            if (!used && show) {
                txList.set_TxUseInCalc(*i, false);
            } else if (show) {
                txList.set_TxShowOnMap(*i, false);
            } else {
                txList.set_TxUseInCalc(*i, true);
                txList.set_TxShowOnMap(*i, true);
            }
        } else {
            txList.set_TxUseInCalc(*i, used);
            unsigned long txId = txList.get_TxId(*i);
            MapShapeMap::iterator msi = coverZones.find(txId);
            if (msi != coverZones.end())
            {
                if (used)
                {
                    msi->second->color = ccZoneCover;
                    if (tx.systemcast == ttAM)
                    {
                        ILisBcLfMfPtr lfmf;
                        tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
                        if (lfmf.IsBound() && lfmf->is_day)
                            msi->second->color = ccIsDay;
                    }
                } else
                    msi->second->color = ccZoneCoverNotUsed;

                msi->second->width = used ? BCCalcParams.lineThicknessZoneCover : 1;
            }
            // add to calc list and sfn (if necessary) and set flag
            ILISBCTx *itx = txList.get_Tx(*i);
            int calcIdx = curTxList.AddTx(itx);
            curTxList.set_TxUseInCalc(calcIdx, used);
            for (int sfnIdx = 0; sfnIdx < curSfn.Size; sfnIdx++)
                if (curSfn.get_Tx(sfnIdx) == itx)
                    curSfn.set_TxUseInCalc(sfnIdx, used);
            for (int sfnIdx = 0; sfnIdx < curSfnUw1.Size; sfnIdx++)
                if (curSfnUw1.get_Tx(sfnIdx) == itx)
                    curSfnUw1.set_TxUseInCalc(sfnIdx, used);
            for (int sfnIdx = 0; sfnIdx < curSfnUw2.Size; sfnIdx++)
                if (curSfnUw2.get_Tx(sfnIdx) == itx)
                    curSfnUw2.set_TxUseInCalc(sfnIdx, used);

            // add to combo boxes (if necessary)
            if (used) {
                String txName = GetTxIdx(*i) + " " + GetTxName(itx);
                if (cbxWantedTx->Items->IndexOfObject((TObject*)*i) == -1)
                    cbxWantedTx->Items->AddObject(txName, (TObject*)*i);
                if (cbxUnwantedTx->Items->IndexOfObject((TObject*)*i) == -1)
                    cbxUnwantedTx->Items->AddObject(txName, (TObject*)*i);
                if (cbxUnwantedTx2->Items->IndexOfObject((TObject*)*i) == -1)
                    cbxUnwantedTx2->Items->AddObject(txName, (TObject*)*i);
            }
        }
    }
    if (fromMap)
        cmf->bmf->Map->Refresh();

    for (IdList::iterator i = idList.begin(); i != idList.end(); i++)
        refresh_row(*i);

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSortFromUsExecute(TObject *Sender)
{
    SetTime();
    txAnalyzer.SortTxList(txList, "WANTED", "DESC");
    ArrangeAllotGroups();
    pcSelection->ActivePage = tshSelection;
    StBr->Panels->Items[2]->Text = "Від нас";
    refresh();

    parameters.sorting = 1;

    sortingChanged = true;

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSortToUsExecute(TObject *Sender)
{
    SetTime();
    txAnalyzer.SortTxList(txList, "UNWANTED", "DESC");
    ArrangeAllotGroups();
    pcSelection->ActivePage = tshSelection;
    StBr->Panels->Items[2]->Text = "Нам";
    refresh();

    parameters.sorting = 2;

    sortingChanged = true;

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actUsedInCalcUpdate(TObject *Sender)
{
    if (txList.IsBound() && currentTxIndex > 0 && currentTxIndex < txList.Size) {
        if (txList.get_TxUseInCalc(currentTxIndex))
            actUsedInCalc->Checked = true;
        else
            actUsedInCalc->Checked = false;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::acceptChoice(Messages::TMessage& Message)
{
    if (Message.WParam == 39)
        addTx(Message.LParam);
}

void __fastcall TfrmSelection::sgrSelectionKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    switch (Key) {
        case ' ':
            if (Shift == TShiftState() && actUsedInCalc->Enabled) {
                actUsedInCalcUpdate(this);
                actUsedInCalcExecute(this);
            }
            break;
        case 13:
            if (Shift == TShiftState() && actCalcDuel->Enabled)
                actCalcDuelExecute(this);
            break;
        case 106: // grey '*'
            if (actRevertAll->Enabled) actRevertAllExecute(this);
            break;
        case 107: // grey '+'
            if (actSelectAll->Enabled) actSelectAllExecute(this);
            break;
        case 109: // grey '-'
            if (actDeselectAll->Enabled) actDeselectAllExecute(this);
            break;
        case 111: // grey '/'
            if (actSelect20->Enabled) actSelect20Execute(this);
            break;
        case 'a': case 'A': case 'а': case 'А':
            if (Shift == TShiftState() << ssAlt)
            actEditExecute(Sender);
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actEditExecute(TObject *Sender)
{
    int idx = (pcSelection->ActivePage == tshMap && cmf->Visible) ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    checkIndex(idx);
    FormProvider.ShowTx(txList.get_Tx(idx));
    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::sgrSelectionDblClick(TObject *Sender)
{
    if (currentTxIndex <= 0)
        selectAnalyzeTx(grid->dg->Row + 1);

    if (actCalcDuel->Enabled) {
        actCalcDuelExecute(this);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::NotImplemented()
{
    Application->MessageBox("Feature not implemented yet", Application->Title.c_str(), 0);
}


void __fastcall TfrmSelection::actCalcDuelExecute(TObject *Sender)
{
    checkIndex(currentTxIndex);

    TempCursor tc(crHourGlass);

    //wasChanges = true;
    int idxA = 0;
    if (pcSelection->ActivePage == tshMap)
        idxA = (int)cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex];

    int idxB = currentTxIndex;

    AnsiString emptyVal("-/-");

    lblA->Caption = AnsiString(idxA) + ':';
    lblB->Caption = AnsiString(idxB) + ':';
    lblEminA->Caption = AnsiString("E мін ") + AnsiString(idxA) + " = ";
    lblEminB->Caption = AnsiString("E мін ") + AnsiString(idxB) + " = ";
    lblEa->Caption = AnsiString("Сигнал в точці ") + lblA->Caption;
    lblEb->Caption = AnsiString("Сигнал в точці ") + lblB->Caption;

    grdDuelPoints->Cells[1][0] = AnsiString(idxA) + "-1";
    grdDuelPoints->Cells[2][0] = AnsiString(idxA) + "-2";
    grdDuelPoints->Cells[3][0] = AnsiString(idxB) + "-3";
    grdDuelPoints->Cells[4][0] = AnsiString(idxB) + "-4";

    if (idxA == idxB) {
        //  очистить панель на карте, если видима, да свалить
        if (pcSelection->ActivePage == tshMap) {

            lblAData->Caption = emptyVal;
            lblBData->Caption = emptyVal;
            lblEminAData->Caption = emptyVal;
            lblEminBData->Caption = emptyVal;

            for (int col = 0; col < 4; col++) {
                grdDuelPoints->Cells[col + 1][1] = emptyVal;
                grdDuelPoints->Cells[col + 1][2] = emptyVal;
                grdDuelPoints->Cells[col + 1][3] = emptyVal;
                grdDuelPoints->Cells[col + 1][4] = emptyVal;
                if(!ttAMType)
                {
                    grdDuelPoints->Cells[col + 1][5] = emptyVal;
                    grdDuelPoints->Cells[col + 1][6] = emptyVal;
                }
            }
        }
        return;
    }


    TCOMILISBCTx tx1(txList.get_Tx(idxA), true);
    TCOMILISBCTx tx2(txList.get_Tx(idxB), true);

    TDuelResult2 duelResult;
    duelResult.Tx1_NoiseLimited = NULL;
    duelResult.Tx1_InterferenceLimited = NULL;
    duelResult.Tx2_NoiseLimited = NULL;
    duelResult.Tx2_InterferenceLimited = NULL;

    TPointDuelResult duelRes[16];
    memset(duelRes, 0, sizeof(duelRes));

    long num = 0;

    SetTime();
    txAnalyzer.CalcDuel(tx1, tx2, &duelResult, duelRes);

    // нормирование (зоны без помех сделаем не меньше зон с помехами). чтоб юзер не нервничал.
    if (duelResult.Tx1_NoiseLimited && duelResult.Tx1_InterferenceLimited &&
        duelResult.Tx2_NoiseLimited && duelResult.Tx2_InterferenceLimited) {

        double *n1 = (double*)duelResult.Tx1_NoiseLimited->pvData;
        double *i1 = (double*)duelResult.Tx1_InterferenceLimited->pvData;
        double *n2 = (double*)duelResult.Tx2_NoiseLimited->pvData;
        double *i2 = (double*)duelResult.Tx2_InterferenceLimited->pvData;

        if (SafeArrayGetUBound(duelResult.Tx1_NoiseLimited, 1, &num) == S_OK)
            num++;

        for (int i = 0; i < num; i++) {
            if (n1[i] < i1[i])
                n1[i] = i1[i];
            if (n2[i] < i2[i])
                n2[i] = i2[i];
        }
    }

    if (pcSelection->ActivePage == tshMap) {

        TRSAGeoPoint pointA, pointB, point1, point2;
        pointA.H = tx1.latitude;
        pointA.L = tx1.longitude;
        pointB.H = tx2.latitude;
        pointB.L = tx2.longitude;

        sphereCalc.PolarToGeo(duelRes[0].radius, duelRes[0].azimuth, pointA, &point1);
        sphereCalc.PolarToGeo(duelRes[3].radius, duelRes[3].azimuth, pointB, &point2);

        // TODO: maybe it's worth to reengeneer this code to simplify it
        std::vector<double> zone1;
        std::vector<double> zone2;
        std::vector<double> zone3;
        std::vector<double> zone4;
        for (int i = 0; i < num; i++)
        {
            if (duelResult.Tx1_NoiseLimited)
                zone1.push_back(((double*)duelResult.Tx1_NoiseLimited->pvData)[i]);
            if (duelResult.Tx1_InterferenceLimited)
                zone2.push_back(((double*)duelResult.Tx1_InterferenceLimited->pvData)[i]);
            if (duelResult.Tx2_NoiseLimited)
                zone3.push_back(((double*)duelResult.Tx2_NoiseLimited->pvData)[i]);
            if (duelResult.Tx2_InterferenceLimited)
                zone4.push_back(((double*)duelResult.Tx2_InterferenceLimited->pvData)[i]);
        }
        cmf->ShowDuelResult(Lis_map::Point(tx1.longitude, tx1.latitude), Lis_map::Point(tx2.longitude, tx2.latitude),
                        zone1, zone2, zone3, zone4, Lis_map::Point(point1.L, point1.H), Lis_map::Point(point2.L, point2.H));

        pcCalcResult->ActivePage = tshDuel;

        lblAData->Caption = txAnalyzer.GetDuelObjString(tx1, duelRes[1].azimuth) + ", азимут " + (int)duelRes[1].azimuth + "\xB0, радіус зони " + FormatFloat("0.0", duelRes[1].radius);
        lblBData->Caption = txAnalyzer.GetDuelObjString(tx2, duelRes[2].azimuth) + ", азимут " + (int)duelRes[2].azimuth + "\xB0, радіус зони " + FormatFloat("0.0", duelRes[2].radius);
        lblEminAData->Caption = FormatFloat("0.0", duelRes[0].emin);
        lblEminBData->Caption = FormatFloat("0.0", duelRes[2].emin);

        lblAData->Left = lblA->Left + lblA->Width + 2;
        lblBData->Left = lblB->Left + lblB->Width + 2;
        lblEminAData->Left = lblEminA->Left + lblEminA->Width + 2;
        lblEminBData->Left = lblEminB->Left + lblEminB->Width + 2;

        for (int col = 0; col < 4; col++) {
            grdDuelPoints->Cells[col + 1][1] = FormatFloat("0.0", duelRes[col].eInt);
            grdDuelPoints->Cells[col + 1][2] = FormatFloat("0.0", duelRes[col].aPR);
            if(ttAMType)
            {
                grdDuelPoints->Cells[col + 1][3] = FormatFloat("0.0", duelRes[col].eUsable);
                grdDuelPoints->Cells[col + 1][4] = FormatFloat("0.0", duelRes[col].emin);
            }
            else
            {
                grdDuelPoints->Cells[col + 1][3] = FormatFloat("0.0", duelRes[col].aDiscr);
                grdDuelPoints->Cells[col + 1][4] = FormatFloat("0.0", duelRes[col].eUsable);
                grdDuelPoints->Cells[col + 1][5] = FormatFloat("0.0", duelRes[col].emin);
                grdDuelPoints->Cells[col + 1][6] = duelRes[col].intType == 0 ? 'Т' : 'П';
            }
        }
    } else {

        txAnalyzer.ShowDuelResult(tx1, tx2, idxA, idxB, duelResult, duelRes);

    }

    //  удалить массивы
    SafeArrayDestroy(duelResult.Tx1_NoiseLimited);
    SafeArrayDestroy(duelResult.Tx1_InterferenceLimited);
    SafeArrayDestroy(duelResult.Tx2_NoiseLimited);
    SafeArrayDestroy(duelResult.Tx2_InterferenceLimited);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actCalcCoverSectorExecute(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->Map->CurrentTool = miGetSectorTool;
}
//---------------------------------------------------------------------------

void SaveWideStringToFile(WideString& s, AnsiString& filename)
{
    std::auto_ptr<TFileStream> f(new TFileStream(filename, fmCreate));
    int n = s.Length();
    for (int i = 1; i <= n; i++) {
        wchar_t ch = s[i];
        f->Write(&ch, sizeof(ch));
    }
}

void __fastcall TfrmSelection::actSaveListExecute(TObject *Sender)
{
    WideString s;

    SaveDialog1->Filter = szSaveListFilter;
    SaveDialog1->DefaultExt = "lst";

    if (SaveDialog1->Execute()) {
        std::auto_ptr<TStringList>fl(new TStringList());

        for (int i = 0; i <= txList.Size - 1; i++) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            tx->saveToString(&s);
            int id = tx.get_id();
            AnsiString fn = ExtractFilePath(SaveDialog1->FileName) + "tx_" + IntToStr(id) + ".tx";
            SaveWideStringToFile(s, fn);
            fl->Add(ExtractFileName(fn));
        }

        fl->SaveToFile(SaveDialog1->FileName);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSaveTxExecute(TObject *Sender)
{
    SaveDialog1->Filter = szSaveTxFilter;
    SaveDialog1->DefaultExt = "tx";
    if (SaveDialog1->Execute()) {
        int idx = (Sender == actSaveObject) ? 0 : grid->dg->Row + 1;
        TCOMILISBCTx tx(txList.get_Tx(idx), true);
        WideString s;
        tx->saveToString(&s);
        SaveWideStringToFile(s, SaveDialog1->FileName);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSaveResExecute(TObject *Sender)
{
/*
    if (pcSelection->ActivePage == tshResults && pcResults->ActivePageIndex != -1) {
        SaveDialog1->Filter = szSaveResultsFilter;
        SaveDialog1->DefaultExt = "rez";
        if (SaveDialog1->Execute()) {
            ((TMemo*)pcResults->ActivePage->Controls[0])->Lines->SaveToFile(SaveDialog1->FileName);
        }
    } else
        Application->MessageBox(szPickResultSheet, Application->Title.c_str(), MB_ICONINFORMATION);
        */
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSetTPExecute(TObject *Sender)
{
    checkIndex(currentTxIndex);

    TCOMILISBCTx tx(txList.get_Tx(currentTxIndex), true);
    if (interfereLimited.find(tx.id) == interfereLimited.end())
        throw *(new Exception("Для передавача не розрахована зона із завадами"));

    if (Application->MessageBox(szSetTP, Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) != IDYES)
        return;

    std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
    sql->Database = dmMain->dbMain;
    if (sql->Transaction->Active)
        sql->Transaction->CommitRetaining();
    else
        sql->Transaction->StartTransaction();

    try {
        //  очистить таблицу тестовых точек
        sql->SQL->Text = AnsiString("delete from TESTPOINTS where (TESTPOINT_TYPE = 0 or TESTPOINT_TYPE = 2)"
                        " and TRANSMITTERS_ID = ") + FTxId;
        sql->ExecQuery();

        //  очистить таблицу выборок. выбранные передачики удаляются по внешнему ключу
        sql->SQL->Text = AnsiString("delete from SELECTIONS where (SELTYPE = 'T')"
                        " and TRANSMITTERS_ID = ") + FTxId;
        sql->ExecQuery();

        sql->SQL->Text = AnsiString("insert into TESTPOINTS (ID, TRANSMITTERS_ID, NAME, LATITUDE, LONGITUDE, TESTPOINT_TYPE, DISTANCE)"
                                    "  values (:ID, :TRANSMITTERS_ID, :NAME, :LATITUDE, :LONGITUDE, :TESTPOINT_TYPE, :DISTANCE)");
        sql->Prepare();

        double *tp =  (double*)interfereLimited[tx.id]->pvData;
        double lon, lat;
        int maxRadius;
        TCOMIRSASpherics Spherics;
        Spherics.CreateInstance(CLSID_RSASpherics);
        TRSAGeoPoint rezGeoPoint, centreGeoPoint;
        centreGeoPoint.H = tx.latitude; centreGeoPoint.L = tx.longitude;

        TCOMILISBCTxList duelTxList;
        duelTxList.CreateInstance(CLSID_LISBCTxList);
        duelTxList.AddTx(tx);
        duelTxList.set_TxUseInCalc(0, true);

        VoltageVector voltageVector;

        std::auto_ptr<TIBSQL> selSql(new TIBSQL(NULL));
        std::auto_ptr<TIBSQL> selTxSql(new TIBSQL(NULL));
        selSql->Database = sql->Database;
        selSql->Transaction = sql->Transaction;
        selTxSql->Database = sql->Database;
        selTxSql->Transaction = sql->Transaction;

        selSql->SQL->Text = "insert into SELECTIONS (ID, TRANSMITTERS_ID, SELTYPE, NAMEQUERIES, USERID, CREATEDATE) values "
                                                    "(:ID, :TRANSMITTERS_ID, 'T', :NAMEQUERIES, :USERID, 'today')";
        selTxSql->SQL->Text = "insert into SELECTEDTRANSMITTERS (SELECTIONS_ID, TRANSMITTERS_ID, USED_IN_CALC, SORTINDEX) values "
                                                    "(:SELECTIONS_ID, :TRANSMITTERS_ID, 1, :SORTINDEX)";
        selSql->Prepare();
        selTxSql->Prepare();

        int userId = dmMain->UserId;

        SetTime();
        
        int pointNum = interfereLimited[tx.id]->rgsabound[0].cElements;
        for (int azimuth = 0; azimuth < pointNum; azimuth++) {

            Spherics.PolarToGeo(tp[azimuth], azimuth * 360 / pointNum, centreGeoPoint, &rezGeoPoint);
            int tpId = dmMain->getNewId();
            AnsiString tpName;
            tpName.sprintf("TP %.2d", azimuth);

            sql->ParamByName("ID")->AsInteger = tpId;
            sql->ParamByName("TRANSMITTERS_ID")->AsInteger = tx.id;
            sql->ParamByName("NAME")->AsString = tpName;
            sql->ParamByName("LATITUDE")->AsDouble = rezGeoPoint.H;
            sql->ParamByName("LONGITUDE")->AsDouble = rezGeoPoint.L;
            sql->ParamByName("TESTPOINT_TYPE")->AsInteger = 0;
            sql->ParamByName("DISTANCE")->AsDouble = tp[azimuth];
            sql->ExecQuery();

            selSql->ParamByName("ID")->AsInteger = tpId;
            selSql->ParamByName("TRANSMITTERS_ID")->AsInteger = tx.id;
            selSql->ParamByName("NAMEQUERIES")->AsString = tpName;
            selSql->ParamByName("USERID")->AsInteger = userId;
            selSql->ExecQuery();

            //  считаем дуельные помехи, сортируем список и запоминаем первые higherIntNum помех
            voltageVector.clear();

            for (int i = 1; i < txList.Size; i++) {
                if (txList.get_TxUseInCalc(i)) {
                    if (duelTxList.Size == 2)
                        duelTxList.RemoveTx(duelTxList.get_Tx(1));
                    duelTxList.AddTx(txList.get_Tx(i));
                    duelTxList.set_TxUseInCalc(1, true);

                    voltageVector.push_back(VoltagePair(txList.get_TxId(i),
                                txAnalyzer.GetUsableE(duelTxList, rezGeoPoint.L, rezGeoPoint.H)));
                }
            }

            if (voltageVector.size() > 0)
                SortVoltageVector(voltageVector, 0, voltageVector.size() - 1);

            VoltageVector::iterator vvi = voltageVector.begin();
            int intNum = min(BCCalcParams.higherIntNum, voltageVector.size());
            for (int i = 0; i < intNum; i++) {
                selTxSql->ParamByName("SELECTIONS_ID")->AsInteger = tpId;
                selTxSql->ParamByName("TRANSMITTERS_ID")->AsInteger = vvi->first;
                selTxSql->ParamByName("SORTINDEX")->AsInteger = i;
                selTxSql->ExecQuery();
                vvi++;
            }

        }
        sql->Transaction->CommitRetaining();

        showTestPoints(tx.id);

    } __finally {
        sql->Transaction->RollbackRetaining();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::OffsetFill(TBCCalcResultType Type)
{
    if (!OffsetRangeDlg)
        OffsetRangeDlg = new TOffsetRangeDlg(this);

    long OffsetDown = OffsetRangeDlg->edtDownRange->Text.ToInt();
    long OffsetUp = OffsetRangeDlg->edtUpRange->Text.ToInt();

    if (OffsetRangeDlg->ShowModal() != mrOk)
        return;

    if (OffsetDown > OffsetUp) {
        Application->MessageBox("Невірно задані межі", Application->Title.c_str(), MB_ICONERROR | MB_OK);
        return;
    }

    LPSAFEARRAY res;

    TempCursor tc(crHourGlass);

    BCCalcParams.FCalcSrv->SetTxListServer(txList);
    if (Type == rtOffset)
        BCCalcParams.FCalcSrv->OffsetSelection(OffsetDown, OffsetUp, &res);
    else
        BCCalcParams.FCalcSrv->ERPSelection(OffsetDown, OffsetUp, &res);

    long hi, low, idx[2];
    AnsiString s;
    double ss0, ss1;
    low = 0; hi = 0;

    SafeArrayGetLBound(res, 2, &low);
    SafeArrayGetUBound(res, 2, &hi);

    TBCCalcResult *cr = new TBCCalcResult(Type, (2 + 2*(hi-low)) * sizeof(double));
    double *data = (double*)cr->getData();
    //  заголовок
    *data++ = low; //
    *data++ = hi; //
    long bounds[2];
    for (long i = low; i <= hi; i++) {
        bounds[0] = 0;
        bounds[1] = i;
        SafeArrayGetElement(res, bounds, data++);
        bounds[0] = 1;
        SafeArrayGetElement(res, bounds, data++);
    }
    //  мы ответственны за освобождение массива;
    SafeArrayDestroy(res);
    long cm;
    cr->desc =
    BCCalcParams.CalcServerName + ", " +(
        (cm = BCCalcParams.FCalcSrv.CalcMethod) == 0 ? "Сум." :
        cm == 1 ? "Множ." :
        cm == 2 ? "Честер" : "Невід."
        ) + ", " + BCCalcParams.PropagServerName+ ", " +BCCalcParams.ReliefServerName + ", " + frmMain->StatusBar1->Panels->Items[5]->Text;

    //addResult(cr);

    TMemo* mem = NULL;
    if (Type == rtOffset) {
        pcCalcResult->ActivePage = tshOffsetPick;
        mem = memOffset;
    } else if (Type == rtERP) {
        pcCalcResult->ActivePage = tshErpPick;
        mem = memErp;
    } else
        return;

    if (mem)
        cr->exportToMemo(mem);
}


void __fastcall TfrmSelection::actOffsetExecute(TObject *Sender)
{
    OffsetFill(rtOffset);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actERPExecute(TObject *Sender)
{
    OffsetFill(rtERP);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actDuelInterfereExecute(TObject *Sender)
{
    if (Application->MessageBox(szDuelInterfere, Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES) {
        TempCursor tc(crHourGlass);

        CalcDuelInterfere();
        pcSelection->ActivePage = tshSelection;

        wasChanges = true;

        refresh();
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::updateStatus()
{
    //  общее количество
    StBr->Panels->Items[0]->Text = txList.Size;
    //  выбрано для расчёта
    int sel = 0;
    for (int i = 0; i < txList.Size; i++)
        if (txList.get_TxUseInCalc(i))
            sel++;
    StBr->Panels->Items[1]->Text = sel;
}

//---------------------------------------------------------------------------

void __fastcall TfrmSelection::saveResults()
{
    bool oldSimplePanel = frmMain->StatusBar1->SimplePanel;

    frmMain->StatusBar1->SimplePanel = true;
    frmMain->StatusBar1->SimpleText = "Збереження результатів - сортування...";
    frmMain->StatusBar1->Update();

    frmMain->pb->Max = 2 * txList.Size;
    frmMain->pb->Min = 0;
    frmMain->pb->Position = 0;
    frmMain->pb->Show();

    // sorting
    if (sortingChanged)
    {
        sqlUpdateSort->Transaction->CommitRetaining();
        if (!sqlUpdateSort->Prepared)
            sqlUpdateSort->Prepare();
        sqlUpdateSort->ParamByName("SELECTIONS_ID")->AsInteger = FId;
        for (int i = 0; i < txList.Size; i++) {
            sqlUpdateSort->ParamByName("TRANSMITTERS_ID")->AsInteger = txList.get_TxId(i);
            sqlUpdateSort->ParamByName("SORTINDEX")->AsInteger = i;
            sqlUpdateSort->ExecQuery();

            frmMain->pb->StepBy(1);
            frmMain->pb->Update();
        }
        sqlUpdateSort->Transaction->CommitRetaining();
    }
    else
    {
        frmMain->pb->StepBy(txList.Size);
        frmMain->pb->Update();
    }

    sqlUpdateUsed->Transaction->CommitRetaining();

    //остльные параметры
    frmMain->StatusBar1->SimpleText = "Збереження результатів - параметри вибірки...";
    frmMain->StatusBar1->Update();

    std::auto_ptr<TIBSQL> sqlSaveParams(new TIBSQL(this));
      sqlSaveParams->Database = dmMain->dbMain;
      sqlSaveParams->Transaction = dmMain->trMain;
      sqlSaveParams->SQL->Text = "update SELECTEDTRANSMITTERS set AZIMUTH = :AZIMUTH, DISTANCE = :DISTANCE, E_UNWANT = :E_UNWANT, E_WANT = :E_WANT, SELECTIONS_ID = :SELECTIONS_ID, TRANSMITTERS_ID = :TRANSMITTERS_ID, USED_IN_CALC = :USED_IN_CALC, ZONE_OVERLAPPING = :ZONE_OVERLAPPING where ( (SELECTIONS_ID = :SELECTIONS_ID) and (TRANSMITTERS_ID = :TRANSMITTERS_ID) )";
      sqlSaveParams->ParamByName("SELECTIONS_ID")->AsInteger = FId;

    if ( !sqlSaveParams->Prepared )
        sqlSaveParams->Prepare();

    for (int i = 0; i < txList.Size; i++)
    {
        sqlSaveParams->ParamByName("AZIMUTH")->AsDouble = txList.get_TxAzimuth(i);
        sqlSaveParams->ParamByName("DISTANCE")->AsInteger = txList.get_TxDistance(i);
        sqlSaveParams->ParamByName("E_UNWANT")->AsDouble = txList.get_TxUnwantInterfere(i);
        sqlSaveParams->ParamByName("E_WANT")->AsDouble = txList.get_TxWantInterfere(i);
        sqlSaveParams->ParamByName("TRANSMITTERS_ID")->AsInteger = txList.get_TxId(i);
        sqlSaveParams->ParamByName("USED_IN_CALC")->AsInteger = txList.get_TxUseInCalc(i) || txList.get_TxShowOnMap(i);
        sqlSaveParams->ParamByName("ZONE_OVERLAPPING")->AsInteger = txList.get_TxZoneOverlapping(i);
        if ( sqlSaveParams->ParamByName("ZONE_OVERLAPPING")->AsInteger > 0 )
            sqlSaveParams->ParamByName("ZONE_OVERLAPPING")->AsInteger = 1;
        else
            sqlSaveParams->ParamByName("ZONE_OVERLAPPING")->AsInteger = 0;

        sqlSaveParams->ExecQuery();
        sqlSaveParams->Close();

        frmMain->pb->StepIt();
        frmMain->pb->Update();
    }

    sqlSaveParams->Transaction->CommitRetaining();

    //сохранение дополнительных параметров
    frmMain->StatusBar1->SimpleText = "Збереження результатів - додаткові...";
    frmMain->StatusBar1->Update();
    {
        parameters.NewSelection = false;

        std::auto_ptr<TIBQuery> sql(new TIBQuery(this));
          sql->Database = dmMain->dbMain;
          sql->SQL->Text = "update SELECTIONS set RESULT = :RESULT where ID = " + IntToStr(FId);
          sql->Transaction = dmMain->trMain;

        std::auto_ptr<TStream> stream(new TMemoryStream());
        stream->Write(&parameters, sizeof(parameters));

        sql->Params->ParamByName("RESULT")->LoadFromStream(stream.get(), ftBlob);

        sql->ExecSQL();
    }

    frmMain->pb->Visible = false;
    frmMain->StatusBar1->SimplePanel = oldSimplePanel;

    wasChanges = false;
    sortingChanged = false;
}


void __fastcall TfrmSelection::FormCloseQuery(TObject *Sender, bool &CanClose)
{
    if (wasChanges)
        saveResults();
}
//---------------------------------------------------------------------------

void TBCCalcResult::exportToMemo(TMemo* memo)
{
    memo->Lines->Clear();
    memo->Lines->Add(desc);
    memo->Lines->Add("");
    /*
    for (int i = 0; i < 255; i++)
        memo->Lines->Add(AnsiString().sprintf("%3d - %.3c", i, i));
    */

    long alb, aub, rlb, rub;
    double *fHead, *fData;
    AnsiString line;
    long max;

    switch(resultType) {
        case rtOffset:
            fHead = (double*)getData();
            if (abs(fHead[0]) > abs(fHead[1])) max = abs(fHead[0]); else max = abs(fHead[1]);
            for (int i = fHead[0]; i < fHead[1]; i++) {
               line = "Offset = " + AnsiString(i) + "/12";
               line = line + "  ss0 = " + FormatFloat("0.0##",fHead[2+i*2]) + "\t";
               line = line + "  ss1 = " + FormatFloat("0.0##",fHead[2+i*2+1]) + "\t";
               memo->Lines->Add(line);
           }
           break;
        case rtERP:
                    fHead = (double*)getData();
            if (abs(fHead[0]) > abs(fHead[1])) max = abs(fHead[0]); else max = abs(fHead[1]);
            for (int i = fHead[0]; i < fHead[1]; i++) {
               line = "ERP = " + AnsiString(i) + " dBkW";
               line = line + "  ss0 = " + FormatFloat("0.0##",fHead[2+i*2]) + "\t";
               line = line + "  ss1 = " + FormatFloat("0.0##",fHead[2+i*2+1]) + "\t";
               memo->Lines->Add(line);
           }
           break;
        default: break;
    }
    memo->SelStart = 0;
    //memo->SelLength = 0;
}

//---------------------------------------------------------------------------
void __fastcall TfrmSelection::pcSelectionChange(TObject *Sender)
{
    if (pcSelection->ActivePage == tshChannels)
        fillChannelGrid();
    else if (pcSelection->ActivePage == tshMap)
        ActivateMapSheet();
    else
        ;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSelectAllExecute(TObject *Sender)
{
    for (int i = 1; i < txList.Size; i++) {
        txList.set_TxUseInCalc(i, true);
        txList.set_TxShowOnMap(i, true);
        refresh_row(i);
    }
    pcSelection->ActivePage = tshSelection;

    wasChanges = true;    
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actDeselectAllExecute(TObject *Sender)
{
    for (int i = 1; i < txList.Size; i++) {
        txList.set_TxUseInCalc(i, false);
        txList.set_TxShowOnMap(i, false);
        refresh_row(i);
    }
    pcSelection->ActivePage = tshSelection;

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actRevertAllExecute(TObject *Sender)
{
    for (int i = 1; i < txList.Size; i++) {
        bool used = !txList.get_TxShowOnMap(i);
        txList.set_TxUseInCalc(i, used);
        txList.set_TxShowOnMap(i, used);
        refresh_row(i);
    }
    pcSelection->ActivePage = tshSelection;

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSelect20Execute(TObject *Sender)
{
    for (int i = 1; i < txList.Size; i++) {
        txList.set_TxUseInCalc(i, i <= 20);
        txList.set_TxShowOnMap(i, i <= 20);
        refresh_row(i);
    }
    pcSelection->ActivePage = tshSelection;

    wasChanges = true;
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::actPureCoverageExecute(TObject *Sender)
{
    // only coverages
    pcSelection->ActivePage = tshMap;
    cmf->Clear(-1);
    txs.clear();
    coverZones.clear();
    noiseZones.clear();
    interfZones.clear();
    interfZones2.clear();

    DrawTxs();
    GetCoverage(true);

    #ifdef _DEBUG
 //   ShowMessage("Calling Map->Refresh() from "__FILE__":"+IntToStr(__LINE__)+", "__FUNC__);
    #endif // _DEBUG
    cmf->bmf->Map->Refresh();

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actRemoveAllUnusedExecute(TObject *Sender)
{
    if (Application->MessageBox(szRemoveAllUnused, Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES) {
        pb->Position = pb->Min;
        pb->Max = txList.Size;
        pb->Visible = true;
        Screen->Cursor = crHourGlass;
        try {
            int i = 1;
            while (i < txList.Size) {
                if (txList.get_TxUseInCalc(i)) {
                    i++;
                } else {
                    deleteTx(txList.get_TxId(i));
                    tags.erase(&tags[i-1]);
                    grid->Refresh();
                }
                pb->StepIt();
                pb->Refresh();
            }
        } __finally {
            pb->Visible = false;
            Screen->Cursor = crDefault;
            refresh();
        }
    }
}
//---------------------------------------------------------------------------


void TfrmSelection::GetCoverage(bool recalcAll)
{
    // параметры рельефа
    TRSAPathParams param;
    if (!BCCalcParams.TheoPathTheSame && BCCalcParams.FPathSrv.IsBound()) {

        param.CalcHEff = BCCalcParams.UseHeffTheo;
        param.CalcTxClearance = BCCalcParams.UseTxClearenceTheo;
        param.CalcRxClearance = BCCalcParams.UseRxClearenceTheo;
        param.CalcSeaPercent =  BCCalcParams.UseMorfologyTheo;
        param.Step = BCCalcParams.StepTheo;

        BCCalcParams.FPathSrv->Set_Params(param);
    }

    pb->Position = pb->Min;
    pb->Max = txList.Size;
    pb->Visible = true;
    Screen->Cursor = crHourGlass;

    //std::map<int, double36>::iterator vi;
    LPSAFEARRAY zone;

    SetTime();
    
    //ofstream fs("CoverageZones.log", ios_base::trunc);
    try {
        for (int i = 0; i < txList.Size; i++) {
            if (txList.get_TxUseInCalc(i) || i == 0) {

                TCOMILISBCTx tx(txList.get_Tx(i), true);
                if (tx.get_systemcast() < ttCTV)
                {
                    int ID = txList.get_TxId(i);

                    if (coverage.find(ID) == coverage.end()) {
                        // зоны нет, будем создавать
                        zone = NULL;
                    } else {
                        //  зона уже есть
                        if (recalcAll || tx.systemcast == ttAM) {
                            //  удалить
                            zone = coverage[ID];
                            SafeArrayDestroy(zone);
                            zone = NULL;
                        } else
                            // ничего делать не надо
                            continue;
                    }

                    if (tx.systemcast == ttAM)
                    {
                        ILisBcLfMfPtr lfmf;
                        tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
                        if (lfmf.IsBound())
                            zone = txAnalyzer.GetEtalonZone(tx, lfmf->is_day);
                    } else
                        txAnalyzer.GetTxZone(tx, &zone);

                    coverage[ID] = zone;
                }
            }
            pb->StepIt();
            Update();
        }

    } __finally {
        pb->Visible = false;
        Screen->Cursor = crDefault;
        //  восстановим параметры расчёта
        BCCalcParams.load();
        Update();
        //fs << "================================================================================" << endl << endl;
    }
    DrawCoverage();
}

void __fastcall TfrmSelection::DrawTxs()
{
    txs.clear();

    for (int i = 0; i < txList.Size; i++) {
        if (txList.get_TxShowOnMap(i)) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            int tx_id = tx.id;
            AnsiString txName = GetTxIdx(i);
            if (BCCalcParams.ShowTxNames)
                txName = txName + ' ' + GetTxName(tx);

            if (tx.systemcast == ttAllot)
            {
                TCOMILisBcDigAllot allot;
                tx->QueryInterface(IID_ILisBcDigAllot, (void**) &allot);
                if (allot.IsBound())
                {
                    long cn = 0;
                    OleCheck(allot->get_nb_sub_areas(&cn));
                    for (int i = 0; i < cn; i++)
                    {
                        Lis_map::Points points;
                        long pn = 0;
                        long saId = 0;
                        OleCheck(allot->get_subareaId(i, &saId));
                        OleCheck(allot->get_points_num(saId, &pn));
                        for (int j = 0; j < pn; j++)
                        {
                            BcCoord bcCurr = { 0.0, 0.0 };
                            OleCheck(allot->get_point(saId, j, &bcCurr));
                            points.push_back(Lis_map::Point(bcCurr.lon, bcCurr.lat));
                        }
                        WideString name;
                        allot->get_allot_name(&name);
                        //txs.insert(tx_id)->second = cmf->ShowContour(points, txName, name));
                        txs.insert(std::pair<int, MapShape*>(tx_id, cmf->ShowContour(points, txName, name)));
                    }

                }

            } else {

                MapShape* txsh = cmf->ShowStation(tx.longitude, tx.latitude, txName, GetTxName(tx));
                txs.insert(std::pair<int, MapShape*>(tx_id, txsh));

                // find allotment for tx
                // if is, draw line from allot contour center to tx
                IntToStrMap::iterator ai = allots.find(tx.id);
                if (ai != allots.end())
                {
                    StrToIntMap::iterator aci = curAllot.find(ai->second);
                    if (aci != curAllot.end())
                    {
                        // let's draw
                        int allotId = aci->second;
                        TCOMILISBCTx txAllot(txBroker.GetTx(allotId, CLSID_LisBcDigAllot), true);

                        cmf->ShowLink(txAllot.longitude, txAllot.latitude, tx.longitude, tx.latitude, 3, matArrow);
                    }
                }
            }
        }
    }
}

void __fastcall TfrmSelection::CalcDuelInterfere()
{
    SetTime();
    txAnalyzer.CalcDuelInterfere(txList, "Розрахунок дуельних завад...");
    wasChanges = true;
}

void __fastcall TfrmSelection::DrawCoverage()
{
    coverZones.clear();
    for (int i = 0; i < txList.Size; i++) {
        if (txList.get_TxShowOnMap(i) || i == 0) {
            int ID = txList.get_TxId(i);
            std::map<int, LPSAFEARRAY>::iterator vi;
            if ((vi = coverage.find(ID)) != coverage.end()) {
                TCOMILISBCTx tx(txList.get_Tx(i), true);
                std::vector<double> zone;
                for (int i = 0; i < vi->second->rgsabound[0].cElements; i++)
                    zone.push_back(((double*)vi->second->pvData)[i]);
                MapPolygon* pgn = cmf->ShowCoverageZone(tx.longitude, tx.latitude, zone);
                coverZones[tx.id] = pgn;

                if (txList.get_TxUseInCalc(i))
                {
                    pgn->color = ccZoneCover;
                    if (tx.systemcast == ttAM)
                    {
                        ILisBcLfMfPtr lfmf;
                        tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
                        if (lfmf.IsBound() && lfmf->is_day)
                            pgn->color = ccIsDay;
                    }
                } else
                    pgn->color = ccZoneCoverNotUsed;

                pgn->width = txList.get_TxUseInCalc(i) ? BCCalcParams.lineThicknessZoneCover : 1;
            }
        }
    }
}

//---------------------------------------------------------------------------

void __fastcall TfrmSelection::selectAnalyzeTx(int index)
{
    if (index < 0 || txList.Size <= index)
        return;

    cbxWantedTx->ItemIndex = cbxWantedTx->Items->IndexOfObject((TObject*)index);
    currentTxIndex = index;
    txtNo->Caption = GetTxIdx(index);

    TCOMILISBCTx tx(txList.get_Tx(index), true);
    TBCTxType systemcast = tx.systemcast;
    TCOMILisBcDigAllot allot;
    if (systemcast == ttAllot)
        tx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot);
    double freq;
    long chId = 0;
    WideString ws;
    AnsiString emptyVal("-/-");
    AnsiString unknownVal("???");

    if (systemcast > ttUNKNOWN && systemcast < ttAllot)
        lblDataVal2->Caption = AnsiString().sprintf("%d", tx.get_heightantenna());
    else
        lblDataVal2->Caption = emptyVal;

    if (systemcast == ttAllot)
    {
        char p;
        allot->get_polar(&p);
        lblDataVal3->Caption = AnsiString(&p, 1);
    } else
        switch (tx.get_polarization()) {
            /*TBCPolarization*/
            case plVER: lblDataVal3->Caption = "V";
                break;
            case plHOR: lblDataVal3->Caption = "H";
                break;
            case plMIX: lblDataVal3->Caption = "M";
                break;
            case plCIR: lblDataVal3->Caption = "C";
                break;
            default: lblDataVal3->Caption = emptyVal;
                break;
        };

    if (systemcast == ttAllot)
    {
        long o;
        allot->get_offset(&o);
        lblDataVal6->Caption = IntToStr(o);
    } else if (systemcast != ttAM && systemcast != ttFM)
        lblDataVal6->Caption = AnsiString().sprintf("%.1f", tx.get_video_offset_herz() * 1.0 / 1000);
    else
        lblDataVal6->Caption = emptyVal;

    switch (systemcast) {
        case ttTV:
            txtDesc->Caption = "АТБ ";
            lblDataVal4->Caption = dmMain->getChannelName(tx.channel_id);
            lblDataVal5->Caption = AnsiString().sprintf("%.1f", tx.get_video_carrier());
            lblDataVal1->Caption = AnsiString().sprintf("%.1f", tx.get_epr_video_max());
            break;
        case ttAM:
        case ttFM:
            txtDesc->Caption = systemcast == ttFM ? "АРМ " : "СХ/ДХ ";
            lblDataVal4->Caption = emptyVal;
            lblDataVal5->Caption = AnsiString().sprintf("%.3f", tx.get_sound_carrier_primary());
            lblDataVal1->Caption = AnsiString().sprintf("%.1f", tx.get_epr_sound_max_primary());
            break;
        case ttDVB:
            txtDesc->Caption = "ЦТБ ";
            lblDataVal4->Caption = dmMain->getChannelName(tx.channel_id);
            lblDataVal5->Caption = AnsiString().sprintf("%.1f", tx.get_video_carrier());
            lblDataVal1->Caption = AnsiString().sprintf("%.1f", tx.get_epr_video_max());
            break;
        case ttDAB:
            txtDesc->Caption = "ЦРМ ";
            lblDataVal4->Caption = dmMain->getDabBlockName(tx.blockcentrefreq);
            lblDataVal5->Caption = AnsiString().sprintf("%.1f", tx.get_sound_carrier_primary());
            lblDataVal1->Caption = emptyVal;
            break;
        case ttCTV:
            txtDesc->Caption = "СКТ ";
            lblDataVal4->Caption = emptyVal;
            lblDataVal5->Caption = emptyVal;
            lblDataVal1->Caption = emptyVal;
            break;
        case ttAllot:
            allot->get_notice_type(&ws);
            txtDesc->Caption = "Выд " + AnsiString(ws);
            allot->get_channel_id(&chId);
            if (ws.Pos("T") > 0)
                lblDataVal4->Caption = dmMain->getChannelName(chId);
            else
                lblDataVal4->Caption = dmMain->getDabBlockName(chId);
            allot->get_freq(&freq);
            lblDataVal5->Caption = AnsiString().sprintf("%.2f", freq);
            lblDataVal1->Caption = emptyVal;
            break;
        default:
            txtDesc->Caption = unknownVal;
            lblDataVal4->Caption = emptyVal;
            lblDataVal5->Caption = emptyVal;
            lblDataVal1->Caption = emptyVal;
            break;
    }
    txtDesc->Left = cbxWantedTx->Left + cbxWantedTx->Width - txtDesc->Width;

    if (index == 0)
        txtNo->Font->Color = ccTxZero;
    else
        txtNo->Font->Color = ccTxSelected;

    if (index > 0) {
        grid->dg->Row = index - 1;
    } else {
        grid->dg->Row = 0;
    }

    //  fill curTxList
    fillCalcTxList();

    if (pcSelection->ActivePage == tshMap) {

        showCurTxZones();

        if (pcCalcResult->ActivePage == tshCoordination) {
            GetCoordination();
        } else if (pcCalcResult->ActivePage == tshDuel) {
            actCalcDuelExecute(this);
        }

        // раскраска помехи
        cmf->UnHighlihtAll();
        SetTxColor(txList.get_TxId(index), ccTxSelected);
        SetTxColor(txList.get_TxId((int)cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex]), ccTxZero);
        if (chbTwoUnwantedTxs->Checked)
            SetTxColor(txList.get_TxId((int)cbxUnwantedTx2->Items->Objects[cbxUnwantedTx2->ItemIndex]), BCCalcParams.lineColorZoneInterfere2);

        cmf->bmf->Map->Refresh();

        bool itIsTx = systemcast != ttAllot;
        cbxUnwantedTx->Enabled = itIsTx;
        lblUnwantedTx->Enabled = itIsTx;
        if (!itIsTx)
            chbTwoUnwantedTxs->Checked = false;
        chbTwoUnwantedTxs->Enabled = itIsTx;
        chbTwoUnwantedTxsClick(cbxWantedTx);
        if (itIsTx)
        {
            grdZones->Cells[2][0] = "без зав.";
            grdZones->Cells[3][0] = "із зав.";
        } else {
            grdZones->Cells[2][0] = "Allot";
            grdZones->Cells[3][0] = "SFN";
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::sgrSelectionKeyUp(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    selectAnalyzeTx(grid->dg->Row + 1);
}
//--------------------------------------------------------------------------

void __fastcall TfrmSelection::sgrSelectionMouseUp(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
    selectAnalyzeTx(grid->dg->Row + 1);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actGetTxZonesExecute(TObject *Sender)
/*
    расчёт зон покрытия передатчика "с помехой и без" или "с помехой 1 и помехой 2"
    или зоны помех выделения
*/
{
    checkIndex(currentTxIndex);

    LPSAFEARRAY saWout = NULL;
    LPSAFEARRAY saWith = NULL;

    SetTime();
    
    int analyzeID = (int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex];
    TCOMILISBCTx tx(txList.get_Tx(analyzeID), true);

    //AnsiString allotMenuItemPrefix("Emin = ");
    if (tx.systemcast == ttAllot && actGetTxZones == Sender)
    {
        // показать меню из имеющихся или, если ничего в БД нет, идти дальше
        std::auto_ptr<TStringList> sl(new TStringList);
        dmMain->GetList("select ID, 'Emin = ' || cast(Emin as varchar(6)) || ' : ' || NOTE from DIG_ALLOT_ZONE "
                        " where ALLOT_ID = " + IntToStr(tx.id) + "ORDER BY ID ", sl.get());
        if (sl->Count > 0) {
            pmnAllotZones->Items->Clear();
            TMenuItem *mni = NULL;
            for (int i = 0; i < sl->Count; i++)
            {
                mni = new TMenuItem(this);
                mni->Caption = sl->Strings[i];
                mni->OnClick = actGetTxZonesExecute;
                mni->Tag = reinterpret_cast<int>(sl->Objects[i]);
                pmnAllotZones->Items->Add(mni);
            }
            mni = new TMenuItem(this);
            mni->Caption = "-";
            pmnAllotZones->Items->Add(mni);
            mni = new TMenuItem(this);
            mni->Caption = "Новый контур помехи...";
            mni->OnClick = actGetTxZonesExecute;
            pmnAllotZones->Items->Add(mni);
            mni = new TMenuItem(this);
            mni->Caption = "Удалить все контуры помех...";
            mni->OnClick = mniDelAllAllotZonesClick;
            pmnAllotZones->Items->Add(mni);

            TPoint p = btnGetZones->ClientToScreen(TPoint(0, btnGetZones->Height + 1));
            pmnAllotZones->Popup(p.x, p.y);
            // here it goes to recurse actGetTxZonesExecute with allot id (or 0 for new one)
            //or to mniDelAllAllotZonesClick
            return;

        } else
            ; // NOTHING - go further

    }

    TempStatusString(StBr, "Считаем зоны...");
    Update();

    if (tx.systemcast == ttAllot)
    {
        double eMin = 30.0;
        TMenuItem *mni = dynamic_cast<TMenuItem*>(Sender);
        if (mni && mni->Tag > 0)
        {
            // если из БД
            dmMain->GetAllotZone(mni->Tag, &saWout);
            currentAllotZones[tx.id] = mni->Tag;
        } else {
            // считаем
            //todo: remove all of this? (to analyzer, I think)
            String note;
            //set emin as calc server parameter
            eMin = txAnalyzer.GetAllotEmin(tx);
            if (dlgEminAndNote == NULL)
                 dlgEminAndNote = new TdlgEminAndNote(Application);
            dlgEminAndNote->edtEmin->Text = FormatFloat("0.0#", eMin);
            if (dlgEminAndNote->ShowModal() == mrOk)
            {
                //eMin = StrToFloat(dlgEminAndNote->edtEmin->Text);
                note = dlgEminAndNote->edtNote->Text;
                TempCursor tc(crHourGlass);
                txAnalyzer.GetTxZone(curTxList, &saWout, "Розрахунок зони завад видiлення...");
                currentAllotZones[tx.id] = dmMain->SaveAllotZone(tx.id, eMin, note, saWout);
            }
        }
        if (mni && mni->Tag > 0) // allot zone is taken from the DB, so eMin is undefined
            eMin = txAnalyzer.GetAllotEmin(tx);
        txAnalyzer.GetSfnZone(curSfn, &eMin, &saWith);

    } else {

        TempCursor tc(crHourGlass);
        if (curTxList.Size == 0)
            throw *(new Exception("Cписок передатчиков пуст - что считать?"));
        txAnalyzer.GetInterfZones(curTxList, cbxUnwantedTx->ItemIndex > -1 ? 1 : 0,
                                (chbTwoUnwantedTxs->Checked && cbxUnwantedTx2->ItemIndex > -1) ?
                                    (cbxUnwantedTx2->ItemIndex != cbxUnwantedTx->ItemIndex ? 2 : 1) : 0,
                                curSfnUw1, curSfnUw2,
                                &saWout, &saWith);
    }
    

    DelZones();

    wasChanges = true;

    //!!!!!!!!!!!!!!!!!!!!!!!!!
    //  correct data as interfere zone should never be bigger then theoretic
    double temp1, temp2;
    if (!chbTwoUnwantedTxs->Checked && tx.systemcast < ttAllot)
        if (saWout && saWith && saWout->rgsabound[0].cElements == saWith->rgsabound[0].cElements) {
            for (int i = 0; i < saWout->rgsabound[0].cElements; i++) {
                temp1 = ((double*)saWith->pvData)[i];
                temp2 = ((double*)saWout->pvData)[i];
                if (((double*)saWith->pvData)[i] > ((double*)saWout->pvData)[i])
                    ((double*)saWith->pvData)[i] = ((double*)saWout->pvData)[i];
            }
        }

    //  записать и нарисовать на карте
    std::vector<double> zone1;
    std::vector<double> zone2;
    if (saWout)
        for (int i = 0; i < saWout->rgsabound[0].cElements; i++)
        {
            temp1 = ((double*)saWout->pvData)[i];
            zone1.push_back(((double*)saWout->pvData)[i]);

        }
    if (saWith)
        for (int i = 0; i < saWith->rgsabound[0].cElements; i++)
        {
            temp1 = ((double*)saWith->pvData)[i];
            zone2.push_back(((double*)saWith->pvData)[i]);
        }

    bool compareIntfs = chbTwoUnwantedTxs->Checked;
    TColor cl1 = compareIntfs ? BCCalcParams.lineColorZoneInterfere : BCCalcParams.lineColorZoneNoise;
    TColor cl2 = compareIntfs ? BCCalcParams.lineColorZoneInterfere2: BCCalcParams.lineColorZoneInterfere;
    std::map<int, LPSAFEARRAY>& v1 = compareIntfs ? interfereLimited : noiseLimited;
    std::map<int, LPSAFEARRAY>& v2 = compareIntfs ? interfereLimited2 : interfereLimited;
    MapShapeMap& shm1 = compareIntfs ? interfZones : noiseZones;
    MapShapeMap& shm2 = compareIntfs ? interfZones2: interfZones;

    if (saWout) {
        shm1[tx.id] =
        cmf->ShowZone(tx.longitude, tx.latitude, zone1,
                            BCCalcParams.lineThicknessZoneInterfere,
                            cl1, psSolid, 0, layer_default);
        v1[tx.id] = saWout;
    }
    if (saWith) {
        shm2[tx.id] =
        cmf->ShowZone(tx.longitude, tx.latitude, zone2,
                            BCCalcParams.lineThicknessZoneInterfere,
                            cl2, psSolid, 0, layer_default);
        v2[tx.id] = saWith;
    }

    //   массив с контрольными точками
    if (BCCalcParams.showCp && !chbTwoUnwantedTxs->Checked) {
        LPSAFEARRAY cpa = getCheckPoints(curTxList, saWout);
        checkPoints[tx.id] = cpa;
    }

    //  поставить отметку об изменении
    modified[analyzeID] = true;
    //  отобразить
    showCurTxZones();

    //#ifdef _DEBUG
    //ShowMessage("Calling Map->Refresh() from "__FILE__":"+IntToStr(__LINE__)+", "__FUNC__);
    //#endif // _DEBUG
    cmf->bmf->Map->Refresh();

    wasChanges = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::panCalcResultResize(TObject *Sender)
{
    panGraph->Height = panGraph->Width;

    //  выровнять колонки сетки
    grdZones->DefaultColWidth = (grdZones->Width - 20) / 4;
    grdPoint->DefaultColWidth = (grdPoint->Width) / 3 - 1;
    grdDuelPoints->DefaultColWidth = (grdDuelPoints->Width - 20) / grdDuelPoints->ColCount;

    int newLeft = panData->Width / 2;
    lblDataParam4->Left = newLeft;
    lblDataParam5->Left = newLeft;
    lblDataParam6->Left = newLeft;
    newLeft = panData->Width / 4;
    if(newLeft < lblDataParam3->Left+lblDataParam3->Width + 4)
        newLeft = lblDataParam3->Left+lblDataParam3->Width + 4;
    lblDataVal1->Left = newLeft;
    lblDataVal2->Left = newLeft;
    lblDataVal3->Left = newLeft;
    newLeft = panData->Width * 3 / 4;
    if(newLeft < lblDataParam4->Left+lblDataParam4->Width + 4)
        newLeft = lblDataParam4->Left+lblDataParam4->Width + 4;
    lblDataVal4->Left = newLeft;
    lblDataVal5->Left = newLeft;
    lblDataVal6->Left = newLeft;

    newLeft = tshPoint->Width / 2;
    lblPointParam3->Left = newLeft;
    lblPointParam9->Left = newLeft;

    newLeft = tshPoint->Width / 4;
    if(newLeft < tshPoint->Left+lblPointParam1->Width + 4)
        newLeft = tshPoint->Left+lblPointParam1->Width + 4;
    lblPointData1->Left = newLeft;
    lblPointData2->Left = newLeft;
    lblPointData8->Left = newLeft;

    newLeft = tshPoint->Width * 3 / 4;
    if(newLeft < tshPoint->Left+lblPointParam3->Width + 4)
        newLeft = tshPoint->Left+lblPointParam3->Width + 4;
    lblPointData3->Left = newLeft;
    lblPointData9->Left = newLeft;

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::cbxWantedTxChange(TObject *Sender)
{
    selectAnalyzeTx((int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex]);
    clearPointData();
    clearPoint2Data();
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::drawResultZones()
{
    bool compareIntfs = chbTwoUnwantedTxs->Checked;
    TColor cl1 = compareIntfs ? BCCalcParams.lineColorZoneInterfere : BCCalcParams.lineColorZoneNoise;
    TColor cl2 = compareIntfs ? BCCalcParams.lineColorZoneInterfere2 : BCCalcParams.lineColorZoneInterfere;
    std::map<int, LPSAFEARRAY>& v1 = compareIntfs ? interfereLimited : noiseLimited;
    std::map<int, LPSAFEARRAY>& v2 = compareIntfs ? interfereLimited2 : interfereLimited;
    MapShapeMap& shm1 = compareIntfs ? interfZones : noiseZones;
    MapShapeMap& shm2 = compareIntfs ? interfZones2: interfZones;

    for (int i = 0; i < txList.Size; i++) {
        if (txList.get_TxUseInCalc(i)) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            std::map<int, LPSAFEARRAY>::iterator vi1 = v1.find(tx.id);
            std::map<int, LPSAFEARRAY>::iterator vi2 = v2.find(tx.id);
            if (vi1 != v1.end())
            {
                std::vector<double> zone;
                for (int i = 0; i < vi1->second->rgsabound[0].cElements; i++)
                    zone.push_back(((double*)vi1->second->pvData)[i]);
                shm1[tx.id] =
                cmf->ShowZone(tx.longitude, tx.latitude, zone,
                            BCCalcParams.lineThicknessZoneInterfere,
                            cl1, psSolid, 0, layer_default);
            }
            if (vi2 != v2.end())
            {
                std::vector<double> zone;
                for (int i = 0; i < vi2->second->rgsabound[0].cElements; i++)
                    zone.push_back(((double*)vi2->second->pvData)[i]);
                shm2[tx.id] =
                cmf->ShowZone(tx.longitude, tx.latitude, zone,
                            BCCalcParams.lineThicknessZoneInterfere,
                            cl2, psSolid, 0, layer_default);
            }
        }
    }
}

void __fastcall TfrmSelection::fillChannelGrid()
{
    //  заполнить сеткку каналов/частот
    TCOMILISBCTx Tx(txList.get_Tx(0), true);
    TBCTxType txType = Tx.systemcast;
    std::auto_ptr<TIBSQL> sqlChannelListAllot(new TIBSQL(this));
    sqlChannelListAllot->Database = dmMain->dbMain;
    sqlChannelListAllot->Transaction = dmMain->trMain;
    bool cont;
    int iter, temp;
    AnsiString rez;
    switch(txType) {
        case ttTV: case ttDVB: case ttDAB: case ttCTV:
            sgrChannelList->ColCount = 3;
            sgrChannelList->FixedCols = 2;
            sgrChannelList->RowCount = 2;
            sgrChannelList->FixedRows = 1;
            sgrChannelList->Cells[0][0] = "ТВК";
            sgrChannelList->Cells[1][0] = "F, МГц";
            sgrChannelList->Cells[2][0] = "Кільк";

            sqlChannelList->Close();
            sqlChannelList->SQL->Text = "SELECT CH.NAMECHANNEL, CH.FREQCARRIERVISION, count(*) "
                                        "FROM SELECTEDTRANSMITTERS ST "
                                        "   LEFT OUTER JOIN TRANSMITTERS TX ON (ST.TRANSMITTERS_ID = TX.ID) "
                                        "   LEFT OUTER JOIN SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID) "
                                        "   LEFT OUTER JOIN CHANNELS CH ON  (TX.CHANNEL_ID = CH.ID) "
                                        "WHERE ST.SELECTIONS_ID = :SEL_ID "
                                        "and  (SC.ENUMVAL = :SCAST_ID1 or SC.ENUMVAL = :SCAST_ID2) "
                                        "group by CH.NAMECHANNEL, CH.FREQCARRIERVISION "
                                        "order by CH.NAMECHANNEL ";

            sqlChannelList->ParamByName("SEL_ID")->AsInteger = FId;
            sqlChannelList->ParamByName("SCAST_ID1")->AsInteger = ttTV;
            sqlChannelList->ParamByName("SCAST_ID2")->AsInteger = ttDVB;
            sqlChannelList->ExecQuery();
            while (!sqlChannelList->Eof) {
                sgrChannelList->Cells[0][sgrChannelList->RowCount-1] = sqlChannelList->Fields[0]->AsString;
                sgrChannelList->Cells[1][sgrChannelList->RowCount-1] = sqlChannelList->Fields[1]->AsString;
                sgrChannelList->Cells[2][sgrChannelList->RowCount-1] = sqlChannelList->Fields[2]->AsString;
                sqlChannelList->Next();
                if (!sqlChannelList->Eof)
                    sgrChannelList->RowCount = sgrChannelList->RowCount+1;
            }
            break;
        case ttAllot:
            sgrChannelList->ColCount = 3;
            sgrChannelList->FixedCols = 2;
            sgrChannelList->RowCount = 2;
            sgrChannelList->FixedRows = 1;
            sgrChannelList->Cells[0][0] = "ТВК";
            sgrChannelList->Cells[1][0] = "F, МГц";
            sgrChannelList->Cells[2][0] = "Кільк";


            sqlChannelListAllot->Close();
            sqlChannelListAllot->SQL->Text = "SELECT CH.NAMECHANNEL, CH.FREQCARRIERVISION, count(*) "
                                            "FROM SELECTEDTRANSMITTERS ST "
                                            "LEFT OUTER JOIN DIG_ALLOTMENT DA ON (ST.TRANSMITTERS_ID = DA.ID) "
                                            "LEFT OUTER JOIN CHANNELS CH ON  (DA.CHANNEL_ID = CH.ID) "
                                            "LEFT OUTER JOIN TRANSMITTERS TX ON (DA.ID = TX.ID) "
                                            "LEFT OUTER JOIN SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID) "
                                            "WHERE ST.SELECTIONS_ID = :SEL_ID  and da.channel_id is not null "
                                            "and  (SC.ENUMVAL = :SCAST_ID1) "
                                            "group by CH.NAMECHANNEL, CH.FREQCARRIERVISION "
                                            "order by CH.NAMECHANNEL ";
            sqlChannelListAllot->ParamByName("SEL_ID")->AsInteger = FId;
            sqlChannelListAllot->ParamByName("SCAST_ID1")->AsInteger = ttAllot;
            sqlChannelListAllot->ExecQuery();

            sqlChannelList->Close();
            sqlChannelList->SQL->Text = "SELECT CH.NAMECHANNEL, CH.FREQCARRIERVISION, count(*) "
                                        "FROM SELECTEDTRANSMITTERS ST "
                                        "   LEFT OUTER JOIN TRANSMITTERS TX ON (ST.TRANSMITTERS_ID = TX.ID) "
                                        "   LEFT OUTER JOIN SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID) "
                                        "   LEFT OUTER JOIN CHANNELS CH ON  (TX.CHANNEL_ID = CH.ID) "
                                        "WHERE ST.SELECTIONS_ID = :SEL_ID "
                                        "and  (SC.ENUMVAL = :SCAST_ID1 or SC.ENUMVAL = :SCAST_ID2) "
                                        "group by CH.NAMECHANNEL, CH.FREQCARRIERVISION "
                                        "order by CH.NAMECHANNEL ";

            sqlChannelList->ParamByName("SEL_ID")->AsInteger = FId;
            sqlChannelList->ParamByName("SCAST_ID1")->AsInteger = ttTV;
            sqlChannelList->ParamByName("SCAST_ID2")->AsInteger = ttDVB;
            sqlChannelList->ExecQuery();
            while (!sqlChannelList->Eof) {
                sgrChannelList->Cells[0][sgrChannelList->RowCount-1] = sqlChannelList->Fields[0]->AsString;
                sgrChannelList->Cells[1][sgrChannelList->RowCount-1] = sqlChannelList->Fields[1]->AsString;
                sgrChannelList->Cells[2][sgrChannelList->RowCount-1] = sqlChannelList->Fields[2]->AsString;
                sqlChannelList->Next();
                if (!sqlChannelList->Eof)
                    sgrChannelList->RowCount = sgrChannelList->RowCount+1;
            }

            while (!sqlChannelListAllot->Eof) {
                cont = false;
                for(iter = 0; iter < sgrChannelList->RowCount; ++iter)
                {
                    if(sgrChannelList->Cells[0][iter] == sqlChannelListAllot->Fields[0]->AsString &&
                        sgrChannelList->Cells[1][iter] == sqlChannelListAllot->Fields[1]->AsString)
                    {
                        cont = true;
                        rez =  sgrChannelList->Cells[2][iter];
                        temp = StrToInt(rez.c_str()) + sqlChannelListAllot->Fields[2]->AsInteger;
                        sgrChannelList->Cells[2][iter] = IntToStr(temp);
                        break;
                    }
                }
                if(!cont)
                {
                    sgrChannelList->Cells[0][sgrChannelList->RowCount-1] = sqlChannelListAllot->Fields[0]->AsString;
                    sgrChannelList->Cells[1][sgrChannelList->RowCount-1] = sqlChannelListAllot->Fields[1]->AsString;
                    sgrChannelList->Cells[2][sgrChannelList->RowCount-1] = sqlChannelListAllot->Fields[2]->AsString;
                    sqlChannelList->Next();
                    if (!sqlChannelList->Eof)
                        sgrChannelList->RowCount = sgrChannelList->RowCount+1;
                }
                sqlChannelListAllot->Next();
            }

            sqlChannelListAllot->Close();
            sqlChannelListAllot->SQL->Text = "SELECT bd.name, bd.centrefreq, count(*) "
                                            "FROM SELECTEDTRANSMITTERS ST "
                                            "LEFT OUTER JOIN DIG_ALLOTMENT DA ON (ST.TRANSMITTERS_ID = DA.ID) "
                                            "LEFT OUTER JOIN TRANSMITTERS TX ON (DA.ID = TX.ID) "
                                            "LEFT OUTER JOIN SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID) "
                                            "LEFT OUTER JOIN blockdab bd on (da.blockdab_id = bd.id) "
                                            "WHERE ST.SELECTIONS_ID = :SEL_ID and da.channel_id is null "
                                            "and  (SC.ENUMVAL = :SCAST_ID1) "
                                            "group by bd.name, bd.centrefreq "
                                            "order by bd.name";
            sqlChannelListAllot->ParamByName("SEL_ID")->AsInteger = FId;
            sqlChannelListAllot->ParamByName("SCAST_ID1")->AsInteger = ttAllot;
            sqlChannelListAllot->ExecQuery();

            while (!sqlChannelListAllot->Eof) {
                sgrChannelList->Cells[0][sgrChannelList->RowCount-1] = sqlChannelListAllot->Fields[0]->AsString;
                sgrChannelList->Cells[1][sgrChannelList->RowCount-1] = sqlChannelListAllot->Fields[1]->AsString;
                sgrChannelList->Cells[2][sgrChannelList->RowCount-1] = sqlChannelListAllot->Fields[2]->AsString;
                sqlChannelListAllot->Next();
                if (!sqlChannelListAllot->Eof)
                    sgrChannelList->RowCount = sgrChannelList->RowCount+1;
            }
            

            break;
        case ttAM:
        case ttFM:
            sgrChannelList->ColCount = 2;
            sgrChannelList->FixedCols = 1;
            sgrChannelList->RowCount = 2;
            sgrChannelList->FixedRows = 1;
            sgrChannelList->Cells[0][0] = "F, MHz";
            sgrChannelList->Cells[1][0] = "Кільк";

            sqlChannelList->Close();
            sqlChannelList->SQL->Text = "SELECT TX.SOUND_CARRIER_PRIMARY, count(*) "
                                        "FROM SELECTEDTRANSMITTERS ST "
                                        "   LEFT OUTER JOIN TRANSMITTERS TX ON (ST.TRANSMITTERS_ID = TX.ID) "
                                        "   LEFT OUTER JOIN SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID) "
                                        "WHERE ST.SELECTIONS_ID = :SEL_ID "
                                        "and SC.ENUMVAL = :SCAST_ID "
                                        "group by TX.SOUND_CARRIER_PRIMARY "
                                        "order by TX.SOUND_CARRIER_PRIMARY ";

            sqlChannelList->ParamByName("SEL_ID")->AsInteger = FId;
            sqlChannelList->ParamByName("SCAST_ID")->AsInteger = txType;
            sqlChannelList->ExecQuery();
            while (!sqlChannelList->Eof) {
                sgrChannelList->Cells[0][sgrChannelList->RowCount-1] = sqlChannelList->Fields[0]->AsString;
                sgrChannelList->Cells[1][sgrChannelList->RowCount-1] = sqlChannelList->Fields[1]->AsString;
                sqlChannelList->Next();
                if (!sqlChannelList->Eof)
                    sgrChannelList->RowCount = sgrChannelList->RowCount+1;
            }
            break;
    }
}

void __fastcall TfrmSelection::ActivateMapSheet()
{
    bool firstTime = false;
    if (!mapInitialized)
    try {
        cmf->Init();
        // if map is already initialized, an exception will be thrown and code below will not be executed

        try {
            checkPointsLayer = cmf->bmf->GetLayersCount();
            cmf->bmf->SetLayersCount(checkPointsLayer + 1);
            cmf->bmf->SetLayerCaption(checkPointsLayer, "Контрольные точки");
            cmf->bmf->SetLayerSelectable(checkPointsLayer, true);
        } catch(Exception &e) {
            e.Message = "Ошибка добавления слоя контрольных точек:\n" + e.Message;
            Application->ShowException(&e);
        }
        mapInitialized = true;
        firstTime = true;

        oldMouseMove = cmf->bmf->Map->OnMouseMove;
        oldMouseDown = cmf->bmf->Map->OnMouseDown;
        oldMouseUp   = cmf->bmf->Map->OnMouseUp;

        cmf->ColorLinkH = clTeal;
        cmf->ColorLinkUnH = clTeal;

        cmf->bmf->Map->OnMouseMove = MapMouseMove;
        cmf->bmf->Map->OnMouseDown = MapMouseDown;
        cmf->bmf->Map->OnMouseUp   = MapMouseUp;
        cmf->bmf->OnToolUsed = MapToolUsed;

        cmf->bmf->actPanButtons->Checked = false;
        cmf->bmf->actPanButtonsExecute(this);

        cmf->bmf->Map->CreateCustomTool(miGetSectorTool, miToolTypeMarquee, TVariant(miCrossCursor), TVariant(NULL), TVariant(NULL), TVariant(0));

    } catch(Exception &e) {
        e.Message = "Ошибка инициализации карты:\n" + e.Message;
        Application->ShowException(&e);
    }

    ccZoneCover = (TColor)(BCCalcParams.lineColorZoneCover);
    ccZoneNoise = (TColor)(BCCalcParams.lineColorZoneNoise);

    cmf->Clear(-1);
    txs.clear();
    coverZones.clear();
    noiseZones.clear();
    interfZones.clear();
    interfZones2.clear();

    if ( coordinationPointsShow )
        CoordinationPointsShow();

    tbtNone->Down = true;
    actNoneExecute(this);

    TCOMILISBCTx tx(txList.get_Tx(0), true);

    DrawTxs();

    drawResultZones();

    GetCoverage(true);

    if (firstTime || BCCalcParams.mapAutoFit)
        cmf->bmf->FitObjects();

    cbxWantedTx->Clear();
    cbxUnwantedTx->Clear();
    cbxUnwantedTx2->Clear();

    for (int i = 0; i < txList.Size; i++)
        if (i == 0 || txList.get_TxShowOnMap(i)) {
            String txName = GetTxIdx(i) + " " + GetTxName(txList.get_Tx(i));
            cbxWantedTx->Items->AddObject(txName, (TObject*)i);
            cbxUnwantedTx->Items->AddObject(txName, (TObject*)i);
            cbxUnwantedTx2->Items->AddObject(txName, (TObject*)i);
        }

    int minCount = ::Screen->Height / 3 / cbxWantedTx->ItemHeight;
    if (cbxWantedTx->Items->Count < minCount) {
        cbxWantedTx->DropDownCount = cbxWantedTx->Items->Count;
        cbxUnwantedTx->DropDownCount = cbxWantedTx->Items->Count;
        cbxUnwantedTx2->DropDownCount = cbxWantedTx->Items->Count;
    } else {
        cbxWantedTx->DropDownCount = minCount;
        cbxUnwantedTx->DropDownCount = minCount;
        cbxUnwantedTx2->DropDownCount = minCount;
    }

    if (cbxUnwantedTx->Items->Count > 0)
        cbxUnwantedTx->ItemIndex = 0;
    if (cbxUnwantedTx2->Items->Count > 0)
        cbxUnwantedTx2->ItemIndex = 0;

    //подкорректируем currentTxIndex
    if (currentTxIndex == -1)
    {
        for (int i = 1; i < txList.get_Size(); i++)
            if (txList.get_TxUseInCalc(i))
            {
                currentTxIndex = i;
                break;
            }
    }

    //выберем объект анализа
    if (currentTxIndex >=0 && txList.get_TxUseInCalc(currentTxIndex))
        selectAnalyzeTx(currentTxIndex);
    else
        selectAnalyzeTx(0);
}

void __fastcall TfrmSelection::btnDelZonesClick(TObject *Sender)
{
    DelZones();

    int analyzeID = (int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex];
    TCOMILISBCTx tx(txList.get_Tx(analyzeID), true);

    if (tx.systemcast == ttAllot && currentAllotZones[analyzeID] > 0
    && MessageBox(NULL, "Удалить контур зоны помехи этого выделения из БД?", "Вопрос, однако",
                        MB_ICONQUESTION | MB_YESNO) == IDYES)
    {
        dmMain->DelAllotZone(currentAllotZones[analyzeID]);
    }
    currentAllotZones[analyzeID];
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::GetE(double lon, double lat)
{   // анализ в точке
    if (pcCalcResult->ActivePage != tshPoint)
        return;

    SetTime();

    checkIndex(currentTxIndex);
    TCOMILISBCTx Tx(txList.get_Tx(currentTxIndex), true);

    clearPointData();
    lblPointData1->Caption = dmMain->coordToStr(lon, 'X') + " : " + dmMain->coordToStr(lat, 'Y');


    if (BCCalcParams.FPathSrv.IsBound()) {

        if ((bool)Tx) {
            Rsageography_tlb::TRSAGeoPoint A;
            Rsageography_tlb::TRSAGeoPoint B;
            Rsageography_tlb::TRSAGeoPathData Data;
            Rsageography_tlb::TRSAGeoPathResults Results;

            Tx->get_latitude(&A.H);
            Tx->get_longitude(&A.L);
            long ha;
            Tx->get_heightantenna(&ha);
            Data.TxHeight = ha;

            B.L = lon;
            B.H = lat;
            Data.RxHeight = 10;

            BCCalcParams.FPathSrv->RunPointToPoint(A, B, Data, &Results);
            lblPointData2->Caption = FormatFloat("0.#",   Results.Distance) + " km";
            lblPointData3->Caption = FormatFloat("0",   Results.Azimuth) + "\xB0";
        }
    }
    char type;
    lblPointData8->Caption = FormatFloat("0.#", txAnalyzer.GetEmin(Tx)) + " dB";
    lblPointData9->Caption = FormatFloat("0.#", txAnalyzer.GetE(Tx, lon, lat, &type)) + " dB";

    grdPoint->Cells[2][1] = "";
    grdPoint->Cells[2][2] = "";
    grdPoint->Cells[2][3] = "";
    grdPoint->Cells[1][1] = "";
    grdPoint->Cells[1][2] = "";
    grdPoint->Cells[1][3] = "";

    if (curTxList.IsBound()) try {
        double az = txAnalyzer.GetAzimuth(Tx.longitude, Tx.latitude, lon, lat);
        //  с нами
        curTxList.set_TxUseInCalc(1, true);
        grdPoint->Cells[2][2] = FormatFloat("  0.#", txAnalyzer.GetZoneByAzm(curTxList, az));
        grdPoint->Cells[2][3] = FormatFloat("  0.#", txAnalyzer.GetUsableE(curTxList, lon, lat));

        //  без нас
        if (cbxWantedTx->ItemIndex != cbxUnwantedTx->ItemIndex) {
            curTxList.set_TxUseInCalc(1, false);
        }
        //grdPoint->Cells[1][1] =
        grdPoint->Cells[1][2] = FormatFloat("  0.#", txAnalyzer.GetZoneByAzm(curTxList, az));
        grdPoint->Cells[1][3] = FormatFloat("  0.#", txAnalyzer.GetUsableE(curTxList, lon, lat));
    } catch (Exception &e) {
        e.Message = "Ошибка расчёта в точке:\n"+e.Message;
        Application->ShowException(&e);
    }

    static MapPoint* epoint = NULL;

    if (epoint)
        try {
            epoint->CheckIsValid();
            delete epoint;
        } catch (...) {};

    epoint = cmf->ShowPoint(lon, lat, ccEPoint, 6, ptXCross, "", "");

    cmf->bmf->Map->Refresh();

    //для второго передатчика
    {
        clearPoint2Data();

        TCOMILISBCTx Tx(txList.get_Tx((int)cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex]), true);

        if (BCCalcParams.FPathSrv.IsBound()) {

            if ((bool)Tx) {
                Rsageography_tlb::TRSAGeoPoint A;
                Rsageography_tlb::TRSAGeoPoint B;
                Rsageography_tlb::TRSAGeoPathData Data;
                Rsageography_tlb::TRSAGeoPathResults Results;

                Tx->get_latitude(&A.H);
                Tx->get_longitude(&A.L);
                long ha;
                Tx->get_heightantenna(&ha);
                Data.TxHeight = ha;

                B.L = lon;
                B.H = lat;
                Data.RxHeight = 10;

                BCCalcParams.FPathSrv->RunPointToPoint(A, B, Data, &Results);
                lblPoint2Data2->Caption = FormatFloat("0.#",   Results.Distance) + " km";
                lblPoint2Data3->Caption = FormatFloat("0",   Results.Azimuth) + "\xB0";
            }
        }

        lblPoint2Data8->Caption = FormatFloat("0.#", txAnalyzer.GetEmin(Tx)) + " dB";
        lblPoint2Data9->Caption = FormatFloat("0.#", txAnalyzer.GetE(Tx, lon, lat, &type)) + " dB";
    }


    lastCpLon = lon;
    lastCpLat = lat;

    if ( (chbShowInDetail->Checked) && (BCCalcParams.FPathSrv.IsBound()) )
    {
        ShowCheckPoint(lon, lat);
        if (frmPoint)
            frmPoint->pnlCpReq->Visible = false;
    }
}

void __fastcall TfrmSelection::clearPointData()
{
    AnsiString emptyVal("-/-");
    
    lblPointData1->Caption = emptyVal;
    lblPointData2->Caption = emptyVal;
    lblPointData3->Caption = emptyVal;
    lblPointData8->Caption = emptyVal;
    lblPointData9->Caption = emptyVal;

    lastCpLon = 0.0;
    lastCpLat = 0.0;

    for (int x = 1; x < grdPoint->ColCount; x++)
        for (int y = 1; y < grdPoint->RowCount; y++)
            grdPoint->Cells[x][y] = "";

    if (frmPoint)
        frmPoint->Close();
}


void __fastcall TfrmSelection::clearPoint2Data()
{
    AnsiString emptyVal("-/-");
    
    lblPoint2Data2->Caption = emptyVal;
    lblPoint2Data3->Caption = emptyVal;
    lblPoint2Data8->Caption = emptyVal;
    lblPoint2Data9->Caption = emptyVal;

    lastCpLon = 0.0;
    lastCpLat = 0.0;
/*
    for (int x = 1; x < grdPoint->ColCount; x++)
        for (int y = 1; y < grdPoint->RowCount; y++)
            grdPoint->Cells[x][y] = "";
*/
}

void __fastcall TfrmSelection::ShowCheckPoint(double X, double Y)
{
    /*
    отображение Контрольной Точки
    */
    AnsiString emptyVal("-/-");

    TempCursor ttc(crHourGlass);

    SetTime();

    TCOMILISBCTx txWanted(txList.get_Tx((int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex]), true);
    TCOMILISBCTx txUnwanted(txList.get_Tx((int)cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex]), true);

    if (!sphereCalc.IsBound())
        sphereCalc.CreateInstance(CLSID_RSASpherics);

    TRSAGeoPoint chPoint, txPoint;

    txPoint.H = txWanted.latitude;
    txPoint.L = txWanted.longitude;

    chPoint.H = Y;
    chPoint.L = X;

    double azimuth = 0, distance = 0;
    sphereCalc->Azimuth(txPoint, chPoint, &azimuth);
    sphereCalc->Distance(txPoint, chPoint, &distance);

    if (!frmPoint)
        frmPoint = new TfrmPoint(Application);

    if (txWanted.systemcast == ttAM)
    {
        frmPoint->stringGrid->ColCount = 10;
        frmPoint->stringGrid->Cells[0][0] = "Передавач";
        frmPoint->stringGrid->Cells[1][0] = "Азм.,\xB0";
        frmPoint->stringGrid->Cells[2][0] = "Відст.,км";
        frmPoint->stringGrid->Cells[3][0] = "МЕВП,дБВт";
        frmPoint->stringGrid->Cells[4][0] = "ЕВП,дБВт";
        frmPoint->stringGrid->Cells[5][0] = "Тип ант.";
        frmPoint->stringGrid->Cells[6][0] = "Вис. ант.,м";
        frmPoint->stringGrid->Cells[7][0] = "Е, дБмкВ/м";
        frmPoint->stringGrid->Cells[8][0] = "ЗВ, дБ";
        frmPoint->stringGrid->Cells[9][0] = "Тип Зав";
    } else {
        frmPoint->stringGrid->ColCount = 18;
        frmPoint->stringGrid->Cells[0][0] = "Передавач";
        frmPoint->stringGrid->Cells[1][0] = "Азм.,\xB0";
        frmPoint->stringGrid->Cells[2][0] = "Відст.,км";
        frmPoint->stringGrid->Cells[3][0] = "ЭВП,дБВт";
        frmPoint->stringGrid->Cells[4][0] = "Н ефф,м";
        frmPoint->stringGrid->Cells[5][0] = "Кут Tx,\xB0";
        frmPoint->stringGrid->Cells[6][0] = "Кут Rx,\xB0";
        frmPoint->stringGrid->Cells[7][0] = "Море,%";
        frmPoint->stringGrid->Cells[8][0] = "Е50,дБ";
        frmPoint->stringGrid->Cells[9][0] = "Е10,дБ";
        frmPoint->stringGrid->Cells[10][0] = "Е1,дБ";
        frmPoint->stringGrid->Cells[11][0] = "ЗВ п";
        frmPoint->stringGrid->Cells[12][0] = "ЗВ т";
        frmPoint->stringGrid->Cells[13][0] = "Да,дБ";
        frmPoint->stringGrid->Cells[14][0] = "Езав,дБ";
        frmPoint->stringGrid->Cells[15][0] = "Пол.ОА";
        frmPoint->stringGrid->Cells[16][0] = "Пол.Зав";
        frmPoint->stringGrid->Cells[17][0] = "Тип Зав";
    }

    frmPoint->FormResize(frmPoint);

    frmPoint->stringGrid->RowCount = cbxUnwantedTx->Items->Count + 1;
    frmPoint->Caption = /*"Контрольна точка " + */dmMain->coordToStr(X, 'X') + " - " + dmMain->coordToStr(Y, 'Y');

    frmPoint->pnlCpReq->Visible = true;
    frmPoint->lblCPDistance->Caption = "Расстояние до точки: " + FormatFloat("0.0", distance) + ", км.";
    frmPoint->lblCPNumber->Caption = "Номер точки: " + IntToStr((int)(floor(azimuth/BCCalcParams.degreeStep + 0.5)));
    //frmPoint->lblCPESum->Caption = "Напряженность поля: " + FloatToStrF((double)(TVariant)saCp[(int)(floor(azimuth/step))][cpdiE01], ffGeneral, 4, 3);

    frmPoint->maxInterferenceRow = -1;
    frmPoint->unwantedRows.clear();
    double maxInterference = -MAXDOUBLE;

    /*****************************
    */

    int rowIndex = 1;

    double eWithInterference = 0;
    double eWithoutInterference = 0;
    long sfnIdW = 0, sfnIdUw = 0;


    bool al = false;
    AnsiString ref_id_temp,ref_id = "";
    long idtx_from_Al;
    Lisbccalc_tlb::TControlPointCalcResult res;
    AnsiString allot_name;
    std::vector<double> *allot_params = new std::vector<double> [4];
    double pr_continuous = 0, pr_tropospheric = 0;

    for ( int i = 0; i < cbxUnwantedTx->Items->Count; i++ )
    {
        int txIdx = (int)cbxUnwantedTx->Items->Objects[i];
        if ((i == cbxWantedTx->ItemIndex) || (txList.get_TxUseInCalc(txIdx) == false))
            continue;

        TCOMILISBCTx txUnwanted(txList.get_Tx(txIdx), true);
        //if (txUnwanted.systemcast == ttAllot)
        //    continue;



        AnsiString name = cbxUnwantedTx->Items->Strings[i]; //

        // unwanted highlight
        if (selectedUnwanted.Size == 0)
        {
            if (i == cbxUnwantedTx->ItemIndex)
                frmPoint->unwantedRows.insert(rowIndex);

        } else {

            bool isPresent = false;
            int selCnt = selectedUnwanted.Size;
            while (selCnt-- > 0)
                if (isPresent = (txUnwanted == selectedUnwanted.get_Tx(selCnt)))
                    break;

            if (isPresent)
                frmPoint->unwantedRows.insert(rowIndex);
        }



        if (txUnwanted.IsBound() /*&& txUnwanted.id != txWanted.id*/)
        {
            double eIntfr = -999.;
            if (txWanted.systemcast == ttAM)
            {
                ILisBcLfMfPtr uTx1;
                OleCheck(txWanted->QueryInterface(IID_ILisBcLfMf, (void**)&uTx1));
                long systemW = uTx1->lfmf_system;

                frmPoint->stringGrid->Cells[0][rowIndex] = cbxUnwantedTx->Items->Strings[i];
                ILisBcLfMfPtr lfmf;
                if (txUnwanted.systemcast == ttAM)
                    txUnwanted->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
                double azm = txAnalyzer.GetAzimuth(txUnwanted.longitude, txUnwanted.latitude, X, Y);
                double dist = txAnalyzer.GetDistance(txUnwanted.longitude, txUnwanted.latitude, X, Y);
                frmPoint->stringGrid->Cells[1][rowIndex] = FormatFloat("0.0", azm);
                frmPoint->stringGrid->Cells[2][rowIndex] = FormatFloat("0.0", dist);
                frmPoint->stringGrid->Cells[3][rowIndex] = FormatFloat("0.00", txUnwanted.epr_sound_max_primary);
                frmPoint->stringGrid->Cells[4][rowIndex] = FormatFloat("0.00",  txUnwanted.get_erp(azm));
                unsigned char at = '?';
                if (lfmf.IsBound())
                    lfmf->get_ant_type(&at);
                frmPoint->stringGrid->Cells[5][rowIndex] = String().sprintf("%c", at);
                frmPoint->stringGrid->Cells[6][rowIndex] = IntToStr(txUnwanted.heightantenna);
                char it = '?';
                eIntfr = txAnalyzer.GetE(txUnwanted, X, Y, &it);
                frmPoint->stringGrid->Cells[7][rowIndex] = FormatFloat("0.00", eIntfr);
                double pr;
                pr = txAnalyzer.GetPr(txWanted, txUnwanted);
                if(systemW == 1)
                {
                    txWanted->get_identifiersfn(&sfnIdW);
                    txUnwanted->get_identifiersfn(&sfnIdUw);
                    if(sfnIdW == sfnIdUw && sfnIdW != 0)
                    {
                        pr -= 19.;
                    }
                    else
                    {
                        if(it == 'G')
                            pr += 3;
                    }
                }
                frmPoint->stringGrid->Cells[8][rowIndex] = pr;
                frmPoint->stringGrid->Cells[9][rowIndex] = String().sprintf("%c", it);
            } else {
                Lisbccalc_tlb::TControlPointCalcResult cp_result;
                BCCalcParams.FCalcSrv->GetFieldStrengthControlPoint(
                                        txWanted,
                                        txUnwanted,
                                        X,
                                        Y,
                                        &cp_result);

                long idtx;
                txUnwanted->get_id(&idtx);
                std::auto_ptr<TIBSQL> selSQL(new TIBSQL(Application));
                AnsiString sql;
                sql = "select  TR.ID, TR.associated_adm_allot_id, al.adm_ref_id, al.allot_name from TRANSMITTERS TR left outer join"
                      " dig_allotment al on (tr.associated_adm_allot_id = al.adm_ref_id)"
                      " where TR.ID = :ID";
                selSQL->SQL->Text = sql;
                selSQL->Database = dmMain->dbMain;
                selSQL->ParamByName("ID")->AsFloat = idtx;
                selSQL->ExecQuery();
                if(!selSQL->Eof && !selSQL->FieldByName("associated_adm_allot_id")->IsNull && selSQL->FieldByName("associated_adm_allot_id")->AsString != "")
                {

                    ref_id_temp = selSQL->FieldByName("associated_adm_allot_id")->AsString;
                    if(al)
                    {
                        if(ref_id_temp != ref_id)
                        {
                            frmPoint->stringGrid->RowCount += 1;
                            ref_id = ref_id_temp;
                            CalcAllotRes( idtx_from_Al, chPoint, allot_params, &res);
                            frmPoint->stringGrid->Cells[0][rowIndex] = allot_name;
                            frmPoint->stringGrid->Cells[1][rowIndex] = FormatFloat("0.0",   res.azimuth);
                            frmPoint->stringGrid->Cells[2][rowIndex] = FormatFloat("0.0",   res.distance);
                            frmPoint->stringGrid->Cells[3][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[4][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[5][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[6][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[7][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[8][rowIndex] = FormatFloat("0.0", res.e_50);
                            frmPoint->stringGrid->Cells[9][rowIndex] = FormatFloat("0.0", res.e_10);
                            frmPoint->stringGrid->Cells[10][rowIndex] = FormatFloat("0.0", res.e_1);
                            frmPoint->stringGrid->Cells[11][rowIndex] = FormatFloat("0.0", pr_continuous);
                            frmPoint->stringGrid->Cells[12][rowIndex] = FormatFloat("0.0", pr_tropospheric);
                            frmPoint->stringGrid->Cells[13][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[14][rowIndex] = FormatFloat("0.0", res.e_usable);
                            frmPoint->stringGrid->Cells[15][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[16][rowIndex] = "- / -";
                            frmPoint->stringGrid->Cells[17][rowIndex] = "- / -";
                            rowIndex++;
                            for(int i = 0; i < 4; ++i)
                                allot_params[i].clear();
                        }
                    }
                    else
                    {

                        ref_id = ref_id_temp;
                    }
                    al = true;
                    idtx_from_Al = idtx;
                    allot_name = selSQL->FieldByName("allot_name")->AsString;
                    pr_continuous = cp_result.pr_continuous;
                    pr_tropospheric = cp_result.pr_tropospheric;
                    allot_params[0].push_back(cp_result.e_50);
                    allot_params[1].push_back(cp_result.e_10);
                    allot_params[2].push_back(cp_result.e_1);
                    allot_params[3].push_back(cp_result.e_usable);
                }
                else
                {
                    if(al)
                    {
                        frmPoint->stringGrid->RowCount += 1;
                        al = false;
                        CalcAllotRes( idtx_from_Al, chPoint, allot_params, &res);
                        frmPoint->stringGrid->Cells[0][rowIndex] = allot_name;
                        frmPoint->stringGrid->Cells[1][rowIndex] = FormatFloat("0.0",   res.azimuth);
                        frmPoint->stringGrid->Cells[2][rowIndex] = FormatFloat("0.0",   res.distance);
                        frmPoint->stringGrid->Cells[3][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[4][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[5][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[6][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[7][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[8][rowIndex] = FormatFloat("0.0", res.e_50);
                        frmPoint->stringGrid->Cells[9][rowIndex] = FormatFloat("0.0", res.e_10);
                        frmPoint->stringGrid->Cells[10][rowIndex] = FormatFloat("0.0", res.e_1);
                        frmPoint->stringGrid->Cells[11][rowIndex] = FormatFloat("0.0", pr_continuous);
                        frmPoint->stringGrid->Cells[12][rowIndex] = FormatFloat("0.0", pr_tropospheric);
                        frmPoint->stringGrid->Cells[13][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[14][rowIndex] = FormatFloat("0.0", res.e_usable);
                        frmPoint->stringGrid->Cells[15][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[16][rowIndex] = "- / -";
                        frmPoint->stringGrid->Cells[17][rowIndex] = "- / -";
                        rowIndex++;
                        for(int i = 0; i < 4; ++i)
                                allot_params[i].clear();
                    }
                }
                frmPoint->stringGrid->Cells[0][rowIndex] = cbxUnwantedTx->Items->Strings[i];
                frmPoint->stringGrid->Cells[1][rowIndex] = FormatFloat("0.0",   cp_result.azimuth);
                frmPoint->stringGrid->Cells[2][rowIndex] = FormatFloat("0.0",   cp_result.distance);
                frmPoint->stringGrid->Cells[3][rowIndex] = FormatFloat("0.0",   cp_result.erp);
                frmPoint->stringGrid->Cells[4][rowIndex] = FormatFloat("0",     cp_result.heff);
                frmPoint->stringGrid->Cells[5][rowIndex] = FormatFloat("0.0", cp_result.tx_clearance);
                frmPoint->stringGrid->Cells[6][rowIndex] = FormatFloat("0.0",   cp_result.rx_clearance);
                frmPoint->stringGrid->Cells[7][rowIndex] = FormatFloat("0", cp_result.sea_percent);
                frmPoint->stringGrid->Cells[8][rowIndex] = FormatFloat("0.0", cp_result.e_50);
                frmPoint->stringGrid->Cells[9][rowIndex] = FormatFloat("0.0", cp_result.e_10);
                frmPoint->stringGrid->Cells[10][rowIndex] = FormatFloat("0.0", cp_result.e_1);
                frmPoint->stringGrid->Cells[11][rowIndex] = FormatFloat("0.0", cp_result.pr_continuous);
                frmPoint->stringGrid->Cells[12][rowIndex] = FormatFloat("0.0", cp_result.pr_tropospheric);
                frmPoint->stringGrid->Cells[13][rowIndex] = FormatFloat("0.0", cp_result.ant_discrimination);
                frmPoint->stringGrid->Cells[14][rowIndex] = FormatFloat("0.0", cp_result.e_usable);
                if(cp_result.pol_wanted == plVER)
                    frmPoint->stringGrid->Cells[15][rowIndex] =  'В';
                else if(cp_result.pol_wanted == plHOR)
                    frmPoint->stringGrid->Cells[15][rowIndex] =  'Г';
                else if(cp_result.pol_wanted == plMIX)
                    frmPoint->stringGrid->Cells[15][rowIndex] =  'М';  
                if(cp_result.pol_unwanted == plVER)
                    frmPoint->stringGrid->Cells[16][rowIndex] =  'В';
                else if(cp_result.pol_unwanted == plHOR)
                    frmPoint->stringGrid->Cells[16][rowIndex] =  'Г';
                else if(cp_result.pol_unwanted == plMIX)
                    frmPoint->stringGrid->Cells[16][rowIndex] =  'М';
                frmPoint->stringGrid->Cells[17][rowIndex] = (cp_result.interf_type == itTropo) ? 'Т' : 'П';

                eIntfr = cp_result.e_usable;

            }

            if ( maxInterference < eIntfr )
            {
                maxInterference = eIntfr;
                frmPoint->maxInterferenceRow = i + 1;
            }

        } else {
            for (int c = 1; c < frmPoint->stringGrid->ColCount; c++)
                frmPoint->stringGrid->Cells[c][rowIndex] = emptyVal;
        }

        rowIndex++;
    }

    if(al)
    {
        frmPoint->stringGrid->RowCount += 1;         
        al = false;
        CalcAllotRes( idtx_from_Al, chPoint, allot_params, &res);
        frmPoint->stringGrid->Cells[0][rowIndex] = allot_name;
        frmPoint->stringGrid->Cells[1][rowIndex] = FormatFloat("0.0",   res.azimuth);
        frmPoint->stringGrid->Cells[2][rowIndex] = FormatFloat("0.0",   res.distance);
        frmPoint->stringGrid->Cells[3][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[4][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[5][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[6][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[7][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[8][rowIndex] = FormatFloat("0.0", res.e_50);
        frmPoint->stringGrid->Cells[9][rowIndex] = FormatFloat("0.0", res.e_10);
        frmPoint->stringGrid->Cells[10][rowIndex] = FormatFloat("0.0", res.e_1);
        frmPoint->stringGrid->Cells[11][rowIndex] = FormatFloat("0.0", pr_continuous);
        frmPoint->stringGrid->Cells[12][rowIndex] = FormatFloat("0.0", pr_tropospheric);
        frmPoint->stringGrid->Cells[13][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[14][rowIndex] = FormatFloat("0.0", res.e_usable);
        frmPoint->stringGrid->Cells[15][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[16][rowIndex] = "- / -";
        frmPoint->stringGrid->Cells[17][rowIndex] = "- / -";
        rowIndex++;
        for(int i = 0; i < 4; ++i)
            allot_params[i].clear();
    }

    if ( rowIndex == 1 )
        frmPoint->stringGrid->RowCount = 2;
    else
        frmPoint->stringGrid->RowCount = rowIndex;

    {
        if (curTxList.IsBound())
        {
            double UsableFieldStrengthW, UsableFieldStrengthWO;

            curTxList.set_TxUseInCalc(1, true);
            UsableFieldStrengthW = txAnalyzer.GetUsableE(curTxList, X, Y);

            //  без нас
            if (cbxWantedTx->ItemIndex != cbxUnwantedTx->ItemIndex) {
            //if (curTxList.get_Tx(1) == txList.get_Tx(0)) {
                //  должен быть нулевой передатчик из основного списка
                curTxList.set_TxUseInCalc(1, false);
            }
            UsableFieldStrengthWO = txAnalyzer.GetUsableE(curTxList, X, Y);

            frmPoint->lbEu->Caption = FormatFloat("0.##", UsableFieldStrengthWO);
            frmPoint->lbEuIntfr->Caption = FormatFloat("0.##", UsableFieldStrengthW);
            frmPoint->lbDiff->Caption = FormatFloat("0.##", UsableFieldStrengthW - UsableFieldStrengthWO);
            curTxList.set_TxUseInCalc(0, false);

            if(txWanted.systemcast == ttDVB)
            {
                double sum = 0.;
                long cid1, cid2;
                txWanted->get_channel_id(&cid2);

                    TCOMILISBCTxList list(curTxList, true);
                    for (int i = 0; i < list.Size; i++)
                    {
                        if (list.get_TxUseInCalc(i))
                        {
                            TCOMILISBCTx tx(list.get_Tx(i), true);
                            tx->get_channel_id(&cid1);
                            if (tx.systemcast == ttDVB && cid1 == cid2)
                            {
                                char it = '\0';
                                double e = txAnalyzer.GetE(tx, X, Y, &it);
                                sum += pow(10., e / 10.);
                            }
                        }
                    }
                    sum = sum > 0 ? 10. * log10(sum) : 0.;


                frmPoint->lbSumIntfr->Caption = FormatFloat("0.##", sum);  // txAnalyzer.GetSumE(curTxList, X, Y)
            }
            else
                frmPoint->lbSumIntfr->Caption = "";

            if(selectedUnwanted.IsBound())
                frmPoint->lbSumSelected->Caption = FormatFloat("0.##", txAnalyzer.GetSumE(selectedUnwanted, X, Y));
            else
                frmPoint->lbSumSelected->Caption = "";

        } else
        {
            frmPoint->lbEu->Caption = "";
            frmPoint->lbEuIntfr->Caption = "";
            frmPoint->lbDiff->Caption = "";
            frmPoint->lbSumIntfr->Caption = "";
            frmPoint->lbSumSelected->Caption = "";
        }

        frmPoint->pnSum->Visible = true;
    }

    frmPoint->Show();

    /*
    *****************************/
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::pcCalcResultChange(TObject *Sender)
{
    tbtNone->Down = true;
    actNoneExecute(this);

    cmf->bmf->Cursor = pcCalcResult->ActivePage == tshPoint ? crGetE : crGetTx;

    if (pcCalcResult->ActivePage == tshCoordination)
    {
        lblUnwantedTx->Enabled = false;
        cbxUnwantedTx->Enabled = false;
        cbxUnwantedTx->Font->Color = cbxUnwantedTx->Color;

        chbTwoUnwantedTxs->Enabled = false;
        lblUnwantedTx2->Enabled = false;
        cbxUnwantedTx2->Enabled = false;
        cbxUnwantedTx2->Font->Color = cbxUnwantedTx2->Color;

        GetCoordination();
    } else {
        lblUnwantedTx->Enabled = true;
        cbxUnwantedTx->Enabled = true;
        cbxUnwantedTx->Font->Color = clWindowText;

        chbTwoUnwantedTxs->Enabled = true;

        lblUnwantedTx2->Enabled = chbTwoUnwantedTxs->Checked;
        cbxUnwantedTx2->Enabled = chbTwoUnwantedTxs->Checked;
        cbxUnwantedTx2->Font->Color = chbTwoUnwantedTxs->Checked ? clWindowText : cbxUnwantedTx2->Color;

        CoordinationPointsHide();
    }

    if (pcCalcResult->ActivePage == tshDuel)
    {
        actCalcDuelExecute(Sender);
    }

    cmf->bmf->Map->Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::GetCoordination()
{
    cordinationPoints.clear();
    
    memCountryList->Lines->Clear();
    int idx = (int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex];

    int numOfPoints = 36;
    std::vector<double> zone;
    zone.reserve(numOfPoints);
    for (int i = 0; i < numOfPoints; i++) zone.push_back(0);
    TCOMILISBCTx tx(txList.get_Tx(idx), true);

    TDateTime stages[5];
    stages[0] = Now();

    double e_trig = -MAXDOUBLE;
    std::auto_ptr<TfrmFS> frmFS(new TfrmFS(Application));
    if (frmFS->ShowModal() == mrOk)
    {
        if(frmFS->edFieldStr->Text != "")
            try
            {
                e_trig = StrToFloat(frmFS->edFieldStr->Text);
            }
            catch(Exception &e)
            {
                e.Message = "Введено недопустимое значение: "+e.Message;
                Application->ShowException(&e);
                e_trig = -MAXDOUBLE;
            }
    } 

    txAnalyzer.GetCoordinationZone(tx, &(zone[0]), e_trig);
    {//test коррекция координационных расстояний
        for ( int i = 0; i < numOfPoints; i++ )
            if ( zone[i] > 2000 )
                zone[i] = 2000;
    }

    stages[3] = Now();

    TRSAGeoPoint zoneGeoPoint, centreGeoPoint, cpGeoPoint;
    centreGeoPoint.H = tx.latitude;
    centreGeoPoint.L = tx.longitude;

    cmf->ShowCoordZone(centreGeoPoint.L, centreGeoPoint.H, zone);

    std::set<int> countryIdSet;

    dmMain->ibdsCheckPoints->Open();

    if (!sphereCalc.IsBound())
        sphereCalc.CreateInstance(CLSID_RSASpherics);

    double step = 360.0 / numOfPoints;
    double zoneAzimuth, cpAzimuth;
    double cpDistance;
    TDataSet *ds = dmMain->ibdsCheckPoints;
    for (ds->First(); !ds->Eof; ds->Next())
    {
        cpGeoPoint.L = dmMain->ibdsCheckPointsLONGITUDE->AsFloat;
        cpGeoPoint.H = dmMain->ibdsCheckPointsLATITUDE->AsFloat;
        sphereCalc->Azimuth(centreGeoPoint, cpGeoPoint, &cpAzimuth);

        double azLo = int(cpAzimuth / step) * step;
        double azHi = azLo + step;
        if (azHi >= 360) azHi -= 360;

        double interpZoneDist = zone[int(azLo/step)] + (zone[int(azHi/step)] - zone[int(azLo/step)]) * (cpAzimuth - azLo) / step;
        sphereCalc->Distance(centreGeoPoint, cpGeoPoint, &cpDistance);

        if (cpDistance <= interpZoneDist)
        {
            countryIdSet.insert(dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger);
            cordinationPoints.push_back(CoordinationPoint(cpGeoPoint, true, dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger, dmMain->ibdsCheckPointsNUMBOUND->AsInteger));
        }
        else
            cordinationPoints.push_back(CoordinationPoint(cpGeoPoint, false, dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger, dmMain->ibdsCheckPointsNUMBOUND->AsInteger));

    }
    /*  нахуй этот бред
    for (int i = 0; i < numOfPoints; i++) {
        zoneAzimuth = i * step;
        sphereCalc->PolarToGeo(zone.get()[i], zoneAzimuth, centreGeoPoint, &zoneGeoPoint);
        dmMain->ibdsCheckPoints->First();
        while (!dmMain->ibdsCheckPoints->Eof)
        {
        //  bool cordinationPointInZone = false;

            cpGeoPoint.L = dmMain->ibdsCheckPointsLONGITUDE->AsFloat;
            cpGeoPoint.H = dmMain->ibdsCheckPointsLATITUDE->AsFloat;
            sphereCalc->Azimuth(centreGeoPoint, cpGeoPoint, &cpAzimuth);
            if ( abs(cpAzimuth - zoneAzimuth) <= step )
            {
                //  наш клиент, проверим разницу расстояний
                sphereCalc->Distance(centreGeoPoint, cpGeoPoint, &cpDistance);

                if (zone.get()[i] >= cpDistance)
                {//проверили -- контрольная точка попала
                    countryIdSet.insert(dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger);

                //  cordinationPointInZone = true;

                    cordinationPoints.push_back(CoordinationPoint(cpGeoPoint, true, dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger, dmMain->ibdsCheckPointsNUMBOUND->AsInteger));
                }
                else
                    cordinationPoints.push_back(CoordinationPoint(cpGeoPoint, false, dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger, dmMain->ibdsCheckPointsNUMBOUND->AsInteger));
            }

            // cordinationPoints.push_back(CoordinationPoint(cpGeoPoint, cordinationPointInZone, dmMain->ibdsCheckPointsCOUNTRY_ID->AsInteger, dmMain->ibdsCheckPointsNUMBOUND->AsInteger));

            dmMain->ibdsCheckPoints->Next();
        }
    }
    */

    CoordinationPointsFillCountryName(cordinationPoints);

    std::map<int, StandRecord>::iterator sri = standRecords.find(tx.stand_id);
    int native_country_id = sri->second.countryId;

    dmMain->ibdsCountries->Open();
    if (!countryIdSet.empty()) {
        lblCoordination->Caption = "Координація потрібна з наступними країнами:";
        std::set<int>::iterator si = countryIdSet.begin();
        while (si != countryIdSet.end()) {
            if (*si != native_country_id)
                if (dmMain->ibdsCountries->Locate("ID", *si, TLocateOptions()))
                    memCountryList->Lines->Add(dmMain->ibdsCountriesNAME->AsString);

            si++;
        }
    } else {
        lblCoordination->Caption = "Координація не потрібна";
    }

    if ( coordinationPointsShow )
        CoordinationPointsShow();
    else
        CoordinationPointsHide();

    stages[4] = Now();
    /*
    Application->MessageBox((AnsiString("Подготовка GetCoordZone - ") + FormatDateTime("ss:zzzz", stages[1] - stages[0]) +
                        "\nСнятие ЭИМ и ЭВА в GetCoordZone - " + FormatDateTime("ss:zzzz", stages[2] - stages[1]) +
                        "\nСнятие профилей в GetCoordZone - " + FormatDateTime("ss:zzzz", stages[3] - stages[2]) +
                        "\nОпределение списка стран - " + FormatDateTime("ss:zzzz", stages[4] - stages[3])
                        ).c_str(),"Stages", MB_ICONINFORMATION);
    */
}

void __fastcall TfrmSelection::GetCoordZone (ILISBCTx* ptrTx, double* zone, int num, TDateTime* stageTimes, int timesNum)
{
    if (ptrTx) {
        for (double *ptr = zone; ptr < zone + num; ptr++)
            *ptr = 0.0;

        int timeCount = 0;

        TCOMILISBCTx tx(ptrTx, true);

        int systemcast_id = tx.systemcast; //  это ещё не systemcast_id
        dmMain->ibdsScList->Open();
        if (dmMain->ibdsScList->Locate("ENUMVAL", systemcast_id, TLocateOptions()))
            systemcast_id = dmMain->ibdsScListID->AsInteger;
        else
            return; //  "Невідома система"

        //  настроим профиль
        TCOMIRSAGeoPath gp;
        gp.CreateInstance(CLSID_RSAGeoPath,  0, CLSCTX_INPROC_SERVER);
        gp.Init(BCCalcParams.FTerrInfoSrv);

        TRSAPathParams pathParams;
        pathParams.CalcHEff = false;
        pathParams.CalcTxClearance = false;
        pathParams.CalcRxClearance = false;
        pathParams.CalcSeaPercent =  true;
        pathParams.Step = BCCalcParams.Step;

        gp.Set_Params(pathParams);

        TRSAGeoPoint geoPoint;
        geoPoint.H = tx.latitude;
        geoPoint.L = tx.longitude;

        TRSAGeoPathData pathData;
        pathData.RxHeight = 0;

        TRSAGeoPathResults pathRes;

        // 2
        if (timeCount < timesNum)
            stageTimes[timeCount++] = Now();

        // шаг равномерный
        double step = 360.0 / num;
        for (int i = 0; i < num; i++) {

            int idx36 = i * 36 / num;
            //  ЭИМ и ЭВА
            double anH = tx.get_effectheight(idx36);
            //double anH = tx.height_eft_max;
            long erp; //   в mW, а в передатчике - в dBkW
            double erp_dB;
            double erp_dB_aux;
            TBCPolarization pol = tx.polarization;
            if (pol == plVER)
                erp_dB = tx.get_effectpowervert(idx36);
            else if (pol == plHOR)
                erp_dB = tx.get_effectpowerhor(idx36);
            else {
                erp_dB = tx.get_effectpowervert(idx36);
                erp_dB_aux = tx.get_effectpowerhor(idx36);
                erp_dB = max(erp_dB, erp_dB_aux);
            }
            erp = pow(10, erp_dB / 10) * 1000000;

            //  найдём строку набора данных, в которой записаны подходящие коорд расстояния
            int appropId = 0;
            int maxId = 0;
            long appropErp = MAXLONG;
            long maxErp = MAXLONG;
            double appropEah = MAXDOUBLE;
            double maxEah = MAXDOUBLE;

            dmMain->ibdsCoordDist->Open();

            //  1
            if (timeCount < timesNum)
                stageTimes[timeCount++] = Now();

            dmMain->ibdsCoordDist->First();
            while (!dmMain->ibdsCoordDist->Eof) {
                if (dmMain->ibdsCoordDistSYSTEMCAST_ID->AsInteger == systemcast_id) {
                    long curErp = dmMain->ibdsCoordDistEFFECTRADIATEPOWER->AsInteger;
                    double curEah = dmMain->ibdsCoordDistHEIGHTANTENNA->AsFloat;
                    if (curErp >= erp && curErp <= appropErp
                     && curEah >= anH && curEah <= appropEah ) {
                        appropId = dmMain->ibdsCoordDistID->AsInteger;
                        appropErp = curErp;
                        appropEah = curEah;
                    }
                    if (curErp >= maxErp && curEah >= maxEah ) {
                        maxId = dmMain->ibdsCoordDistID->AsInteger;
                        maxErp = curErp;
                        maxEah = curEah;
                    }
                }
                dmMain->ibdsCoordDist->Next();
            }

            if (appropId == 0) {
                if (maxId == 0)
                    return;
                else
                    appropId = maxId;
            }
            if (!dmMain->ibdsCoordDist->Locate("ID", appropId, TLocateOptions()))
                return;

            int landDist = dmMain->ibdsCoordDistOVERLAND->AsInteger;
            int seaDist = dmMain->ibdsCoordDistOVERWARMSEA->AsInteger;

            if (tx.systemcast == ttTV && tx.video_carrier > 300) {
                //  для UHF расстояния меньше в 2 раза
                landDist /= 2;
                seaDist /= 2;
            }

            try {
                pathData.TxHeight = anH;
                gp.RunOnAzimuth(geoPoint, step * i, seaDist, pathData, &pathRes);
            } catch (...) {};
            /*
            if (pathRes.SeaPercent > 50)
                zone[i] = seaDist;
            else
                zone[i] = landDist;
            */
            zone[i] = landDist + (seaDist - landDist) * pathRes.SeaPercent / 100;
        }
    }
}

void __fastcall TfrmSelection::actRemoveLessThanZeroExecute(TObject *Sender)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
      sql->Database = dmMain->dbMain;
      sql->SQL->Text = "delete from SELECTEDTRANSMITTERS where TRANSMITTERS_ID = :TX and SELECTIONS_ID = " + IntToStr(FId);

    int i = txList.Size;
    while (i-- > 1)
    {
        TCOMILISBCTx tx(txList.get_Tx(i), true);
        if (tx.systemcast != ttAllot && txList.get_TxUnwantInterfere(i) <= BCCalcParams.minSelInterf  && txList.get_TxWantInterfere(i) <= BCCalcParams.minSelInterf)
        {
            double tmp1 = txList.get_TxUnwantInterfere(i);
            double tmp2 = txList.get_TxWantInterfere(i);

            int txId = txList.get_TxId(i);
            txList.RemoveId(txId);
            curTxList.RemoveId(txId);
            curSfn.RemoveId(txId);
            curSfnUw1.RemoveId(txId);
            curSfnUw2.RemoveId(txId);
            tags.erase(&tags[i-1]);
            sql->Params->Vars[0]->AsInteger = txId;
            sql->ExecQuery();
        }
        sql->Transaction->CommitRetaining();
        grid->Refresh();
    }
    wasChanges = true;
}
//---------------------------------------------------------------------------

AnsiString __fastcall TfrmSelection::GetTxName(ILISBCTx* iTx)
{
    TCOMILISBCTx tx(iTx, true);
    TBCTxType txType = tx.systemcast;

    std::map<int, StandRecord>::iterator sri = standRecords.find(tx.stand_id);
    AnsiString txName = sri == standRecords.end() ? AnsiString("???") :
                                                    AnsiString(sri->second.siteName.c_str()) + ' ';

    WideString ws;
    switch(tx.systemcast) {
        case ttTV: case ttDVB:
            txName = txName + dmMain->getChannelName(tx.channel_id); break;
        case ttAM:
        case ttFM:
            txName = txName + FormatFloat("0.###", tx.sound_carrier_primary); break;
        case ttDAB:
            txName = txName + dmMain->getDabBlockName(tx.blockcentrefreq); break;
        case ttAllot:
            {
            TCOMILisBcDigAllot allot;
            OleCheck(tx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot));
            long ch;
            allot->get_channel_id(&ch);
            txName = AnsiString(tx.station_name) + ' ' + dmMain->getChannelName(ch) + " (Allot)";
            }
            break;
        default: break;
    }
    return txName;
}

AnsiString __fastcall TfrmSelection::GetTxIdx(int i)
{
    if (i == 0)
        return (grid->dg->RowCount > 0 && grid->dg->Cells[0][0] == "0" ?
                    String("0.1") :
                    String("0") );
    else if (tags.size() > i-1)
        return tags[i-1];
    else
        return "???";
}

void __fastcall TfrmSelection::grdDuelPointsDrawCell(TObject *Sender,
      int ACol, int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;

    AnsiString text = Trim(sg->Cells[ACol][ARow]);

    int xOffset = 1;


   if(ttAMType)
   {
        if (ARow == 1 ||
            ARow == 2 ||
            ARow == 3 ||
            ARow == 4 )
        {
            //  right align
            xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2;
        }
        if (
            ARow == 0 ||
            ACol == 0)
        {
            //  center align
            xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
        }
    }
    else
    {
        if (ARow == 1 ||
            ARow == 2 ||
            ARow == 3 ||
            ARow == 4 ||
            ARow == 5)
        {
            //  right align
            xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2;
        }
        if (
            ARow == 0 ||
            ARow == 6 ||
            ACol == 0)
        {
            //  center align
            xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
        }
    }
    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actAnalyzeExecute(TObject *Sender)
{
    int idx = (pcSelection->ActivePage == tshMap && cmf->Visible) ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    checkIndex(idx);

    TCOMILISBCTx tx(txList.get_Tx(idx), true);

    TfrmExplorer *expl = FormProvider.ShowExplorer();

    bool inTree = false;
    TTreeNode *tn = expl->trvExplorer->Items->GetFirstNode();
    while (tn) {
        if (tn->Data == (void*)tx.id) {
            expl->trvExplorer->Selected = tn;
            inTree = true;
            break;
        }
        tn = tn->getNextSibling();
    }

    if (inTree) {
        expl->actNewSelectionExecute(Sender);
    } else {
        //  нет в списке, нужно добавить
        SendMessage(expl->Handle, WM_LIST_ELEMENT_SELECTED, 39, tx.id);

        tn = expl->trvExplorer->Items->GetFirstNode();
        while (tn) {
            if (tn->Data == (void*)tx.id) {
                expl->trvExplorer->Selected = tn;
                expl->actNewSelectionExecute(Sender);
                break;
            }
            tn = tn->getNextSibling();
        }
    }
    
    wasChanges = true;
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::cbxUnwantedTxChange(TObject *Sender)
{
    //  Удалить расчитаные зоны - они не имеют смысла
    DropCalculatedData();

    showCurTxZones();
    fillCalcTxList();

    // раскраска помехи
    cmf->UnHighlihtAll();
    SetTxColor(txList.get_TxId((int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex]), ccTxSelected);
    SetTxColor(txList.get_TxId((int)cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex]), ccTxZero);
    if (chbTwoUnwantedTxs->Checked)
        SetTxColor(txList.get_TxId((int)cbxUnwantedTx2->Items->Objects[cbxUnwantedTx2->ItemIndex]), BCCalcParams.lineColorZoneInterfere2);

    cmf->bmf->Map->Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::selectTxByID(int id)
{
    for (int i = 0; i < txList.Size; i++)
        if (txList.get_TxId(i) == id) {
            selectAnalyzeTx(i);
            return;
        }
}

void __fastcall TfrmSelection::showTestPoints(int txId)
{
    try {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "select ID, NAME, LATITUDE, LONGITUDE, TESTPOINT_TYPE from TESTPOINTS where TRANSMITTERS_ID = :TX_ID ";
        sql->Params->Vars[0]->AsInteger = txId;
        sql->ExecQuery();
        while (!sql->Eof) {
            // DONE: show points
            /*
            frmMap->drawPoint(
                        sql->FieldByName("LONGITUDE")->AsDouble,
                        sql->FieldByName("LATITUDE")->AsDouble,
                        5, clNavy, NULL, NULL, 'o',
                        sql->FieldByName("ID")->AsInteger
                        );
            */
            sql->Next();
        }
    } catch (...) {}
}

void __fastcall TfrmSelection::actShowTestPointsExecute(TObject *Sender)
{
    int idx = (pcSelection->ActivePage == tshMap && cmf->Visible) ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    checkIndex(idx);
    showTestPoints(txList.get_TxId(idx));
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actReloadUpdate(TObject *Sender)
{
    bool enabled = true;
    actClear->Enabled = enabled;
    actZoomIn->Enabled = enabled;
    actZoomOut->Enabled = enabled;
    actPan->Enabled = enabled;
    actNone->Enabled = enabled;
    actLayers->Enabled = enabled;
    actDistance->Enabled = enabled;
    actSaveBmp->Enabled = enabled;
    actCalcCoverSector->Enabled = enabled;
    actSetTP->Enabled = enabled;
    actGetRelief->Enabled = enabled;
}

void __fastcall TfrmSelection::actClearExecute(TObject *Sender)
{
    cmf->Clear(-1);
    txs.clear();
    coverZones.clear();
    noiseZones.clear();
    interfZones.clear();
    interfZones2.clear();

    DrawTxs();
    DrawCoverage();

    cmf->bmf->Map->Refresh();

    actNoneExecute(Sender);
}

void __fastcall TfrmSelection::actPanExecute(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->actPanExecute(Sender);
}

void __fastcall TfrmSelection::actLayersExecute(TObject *Sender)
{
    cmf->bmf->actConfExecute(Sender);
}
void __fastcall TfrmSelection::actZoomInExecute(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->Map->CurrentTool = miZoomInTool;
}
void __fastcall TfrmSelection::actZoomOutExecute(TObject *Sender)
{
    cmf->bmf->Cursor = crDefault;
    cmf->bmf->Map->CurrentTool = miZoomOutTool;
}
void __fastcall TfrmSelection::actDistanceExecute(TObject *Sender)
{
    cmf->bmf->actDistanceExecute(Sender);
    cmf->bmf->Cursor = crDefault;
}
void __fastcall TfrmSelection::actNoneExecute(TObject *Sender)
{
    cmf->bmf->actNoneExecute(Sender);
    cmf->bmf->Cursor = pcCalcResult->ActivePage == tshPoint ? crGetE : crGetTx;
}

void __fastcall TfrmSelection::actSaveBmpExecute(TObject *Sender)
{
    std::auto_ptr<TSaveDialog> sd(new TSaveDialog(this));
    sd->DefaultExt = "jpg";
    sd->Filter = "JPEG файлы|*.jpg;*.jpeg|"
                "BMP файлы|*.bmp|"
                "PNG файлы|*.png|"
                "TIFF файлы|*.tiff;*tif|"
                "GIF файлы|*.gif|"
                "WMF файлы|*.wmf|"
                "PSD файлы|*.psd|";
    sd->Title = "Укажите имя файла";
    if (sd->Execute())
    {
        String ext = sd->FileName.SubString(sd->FileName.Length() - 2, 3);
        ExportFormatConstants format;
        if (ext == "jpg" || ext == "peg")
            format = miFormatJPEG;
        else if (ext == "png")
            format = miFormatPNG;
        else if (ext == "iff" || ext == "tif")
            format = miFormatTIF;
        else if (ext == "gif")
            format = miFormatGIF;
        else if (ext == "wmf")
            format = miFormatWMF;
        else if (ext == "psd")
            format = miFormatPSD;
        else // assume "bmp"
            format = miFormatBMP;

        cmf->bmf->Map->ExportMap(WideString(sd->FileName), format, Variant(), Variant());
        //ShowMessage("Карта эскпортирована в буфер обмена");
        if (MessageBox(NULL, "Открыть файл?", "Готово", MB_ICONQUESTION | MB_YESNO) == IDYES)
            ShellExecute(NULL, "open", sd->FileName.c_str(), NULL, NULL, SW_SHOWDEFAULT);
    }
}

void __fastcall TfrmSelection::actGetReliefExecute(TObject *Sender)
{
    cmf->bmf->Map->CurrentTool = miReliefTool;
    cmf->bmf->Cursor = crGetRelief;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actZoomFitExecute(TObject *Sender)
{
    cmf->bmf->FitObjects();
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::DoSelection(double X1, double Y1, double X2, double Y2)
{
    checkIndex(currentTxIndex);

    if ( !doSelectionStarted )
    try
    {
        doSelectionStarted = true;

        //tbtNone->Down = true;
        //tbtNone->Click();

        //Application->ProcessMessages();

        /*
        ShowMessage(
            AnsiString("zoneBeg.ppX = ") + frmMap->zoneBeg.ppX +
            "\nzoneBeg.ppY = " + frmMap->zoneBeg.ppY +
            "\nzoneEnd.ppX = " + frmMap->zoneEnd.ppX +
            "\nzoneEnd.ppY = " + frmMap->zoneEnd.ppY
        );

        return;
        */

        if (!dlgSector)
            dlgSector = new TdlgSector(Application);

        TCOMILISBCTx tx(txList.get_Tx(currentTxIndex), true);
        std::map<int, StandRecord>::iterator sri = standRecords.find(tx.stand_id);

        dlgSector->lblTxName->Caption = cbxWantedTx->Text;

        if (dlgSector->edtAzStep->Text.IsEmpty())
            dlgSector->edtAzStep->Text = "5";
        if (dlgSector->edtRadStep->Text.IsEmpty())
            dlgSector->edtRadStep->Text = FloatToStr(0.2);

        //  нормализуем - левый верхний угол и правый нижний
        if (X1 > X2) {
            double temp = X1;
            X1 = X2;
            X2 = temp;
        }
        if (Y1 < Y2) {
            double temp = Y1;
            Y1 = Y2;
            Y2 = temp;
        }

        TRSAGeoPoint center;
        center.L = tx.longitude;
        center.H = tx.latitude;

        double maxRad, minRad, maxAz, minAz;
        {
            double curRad, curAz;
            TRSAGeoPoint curPoint;
            curPoint.L = X1;
            curPoint.H = Y1;

            sphereCalc.Distance(center, curPoint, &maxRad);
            minRad = maxRad;
            sphereCalc.Azimuth(center, curPoint, &maxAz);
            minAz = maxAz;

            curPoint.L = X2;
            sphereCalc.Distance(center, curPoint, &curRad);
            sphereCalc.Azimuth(center, curPoint, &curAz);
            if (maxRad < curRad) maxRad = curRad;
            if (maxAz < curAz) maxAz = curAz;
            if (minRad > curRad) minRad = curRad;
            if (minAz > curAz) minAz = curAz;
            curPoint.H = Y2;
            sphereCalc.Distance(center, curPoint, &curRad);
            sphereCalc.Azimuth(center, curPoint, &curAz);
            if (maxRad < curRad) maxRad = curRad;
            if (maxAz < curAz) maxAz = curAz;
            if (minRad > curRad) minRad = curRad;
            if (minAz > curAz) minAz = curAz;
            curPoint.L = X1;
            sphereCalc.Distance(center, curPoint, &curRad);
            sphereCalc.Azimuth(center, curPoint, &curAz);
            if (maxRad < curRad) maxRad = curRad;
            if (maxAz < curAz) maxAz = curAz;
            if (minRad > curRad) minRad = curRad;
            if (minAz > curAz) minAz = curAz;
        }

        dlgSector->cseRadEnd->Value = ceil(maxRad);

        if (center.L > X1 && center.L < X2
         && center.H < Y1 && center.H > Y2) {
            // передатчик сам попадает в указанный прямоугольник
            // полный круг, нужно только максимальный радиус найти
            dlgSector->cseAzBeg->Value = 0;
            dlgSector->cseAzEnd->Value = 360;
            dlgSector->cseRadBeg->Value = 0;
        } else {
            //  сектор, найти макс/мин азимут и расстояние, и нормализовать
            if (maxAz - minAz > 180)
            {
                double a = maxAz;
                maxAz = minAz;
                minAz = a - 360.0;
            }
            dlgSector->cseAzBeg->Value = floor(minAz);
            dlgSector->cseAzEnd->Value = ceil(maxAz);
            dlgSector->cseRadBeg->Value = floor(minRad);
        }

        double eMin = txAnalyzer.GetEmin(tx);

        dlgSector->edtEmin->Text = FormatFloat("0.#", eMin);

        if (dlgSector->ShowModal() == mrOk) {

            double azBeg = dlgSector->cseAzBeg->Value;
            double azEnd = dlgSector->cseAzEnd->Value;
            double azStep = dlgSector->edtAzStep->Text.ToDouble();
            double radBeg = dlgSector->cseRadBeg->Value;
            double radEnd = dlgSector->cseRadEnd->Value;
            double radStep = dlgSector->edtRadStep->Text.ToDouble();

            // не помогает
            //cmf->bmf->Map->Refresh();
            //Application->ProcessMessages();

            SetTime();

            try {
                eMin = dlgSector->edtEmin->Text.ToDouble();
            } catch(Exception &e) {
                Application->ShowException(&e);
            }

            LPSAFEARRAY lpsaRes = NULL;

            TempCursor tc(crHourGlass);

            double* resArray = NULL;

            try {

                txAnalyzer.ShowProgress("Розрахунок покриття...");
                try {
                    lpsaRes = txAnalyzer.GetCoverage(tx, azBeg, azEnd, azStep, radBeg, radEnd, radStep);
                } __finally {
                    txAnalyzer.HideProgress();
                }

                long alb, aub, rlb, rub;
                SafeArrayGetLBound(lpsaRes, 1, &alb);
                SafeArrayGetUBound(lpsaRes, 1, &aub);
                SafeArrayGetLBound(lpsaRes, 2, &rlb);
                SafeArrayGetUBound(lpsaRes, 2, &rub);

                int numA = aub - alb + 1;
                int numR = rub - rlb + 1;

                resArray = new double[8 + numA*numR];
                double* data = resArray;
                //  заголовок
                *data++ = tx.longitude; //  долгота
                *data++ = tx.latitude; //  широта
                *data++ = azBeg; //  нач азимут
                *data++ = azStep; //  шаг
                *data++ = numA;  //  количество
                *data++ = radBeg; //  нач радиус
                *data++ = radStep; //  шаг
                *data++ = numR; //  количество

                long bounds[2];
                for (long i = alb; i <= aub; i++)
                    for (long j = rub; j >= rlb; j--)
                    {   //  радиусы в обратном порядке - так удобнее снимать параметры трасс
                        bounds[0] = i;
                        bounds[1] = j;
                        SafeArrayGetElement(lpsaRes, bounds, data++);
                    }

                //*******************************************************************************************
                //{
                double *array = resArray;
                bool gradient = dlgSector->chbGradient->Checked;
                int id = 0;

                //double *data = array + 6;
                double maxE = MINDOUBLE, minE = MAXDOUBLE;
                for(int i = 0; i <  ((int)array[4])*((int)array[7]); i++) {
                    if (array[8+i] > maxE)
                        maxE = array[8+i];
                    if (array[8+i] > 0 && array[8+i] < minE)
                        minE = array[8+i];
                }

                //double color_koef = maxE -= minE;
                double LongBeg, LatBeg, LongEnd, LatEnd;
                TRSAGeoPoint rezGeoPoint, centerGeoPoint;
                TRSAGeoPoint point1, point2, point3, point4;

                centerGeoPoint.L = *(array); centerGeoPoint.H = *(array + 1);

                for (int azimuth = 0; azimuth < (int)array[4]; azimuth ++) {
                    int radius = 0, radiusBeg = 0;
                    while( radius < (int)array[7] )
                    {
                        //пропускаем зону с "нулевой" напряженностью
                        while( (radius < (int)array[7]) && (array[8 + (int)array[7] * azimuth + radius]<eMin) )
                            radius++;

                        radiusBeg = radius;

                        //проходим зону с "единичной" напряженностью или доходим до конца тестируемого отрезка
                        if (gradient)
                        {
                            if (radius < (int)array[7])
                               radius++;
                        }
                        else
                            while( (radius < (int)array[7]) && (array[8 + (int)array[7] * azimuth + radius]>eMin) )
                                radius++;
                        radius--;//мы за пределами сектора

                        //пошла отрисовка
                        int pos = 8 + (int)array[7] * azimuth + radius;
                        if ( !( (radius+1 == radiusBeg) && (radius+1 == (int)array[7]) ) )
                        {
                            //  нарисовать вокруг точки трапецию
                            //  array[2] - стартовая позиция азимута
                            //  array[3] - шаг азимута
                            //  array[5] - стартовая позиция радиуса
                            //  array[6] - шаг радиуса
                            sphereCalc->PolarToGeo(array[5] + (array[6]* radiusBeg) - array[6] / 2,
                                                  array[2] + array[3] * azimuth - array[3] / 2, centerGeoPoint, &point1);
                            sphereCalc->PolarToGeo(array[5] + (array[6]* radius) + array[6] / 2,
                                                  array[2] + array[3] * azimuth - array[3] / 2, centerGeoPoint, &point2);
                            sphereCalc->PolarToGeo(array[5] + (array[6]* radius) + array[6] / 2,
                                                  array[2] + array[3] * azimuth + array[3] / 2, centerGeoPoint, &point3);
                            sphereCalc->PolarToGeo(array[5] + (array[6]* radiusBeg) - array[6] / 2,
                                                  array[2] + array[3] * azimuth + array[3] / 2, centerGeoPoint, &point4);

                            int LineColor = (gradient) ?
                                            (255.0 * array[pos] / maxE) :
                                            (255.0);
                                            /*
                                            (255.0 * (array[pos] - minE) / maxE) :
                                            (255.0 * (maxE - minE) / maxE);
                                            */
                                            //(200);

                            MapPolygon *mpg = (MapPolygon*)cmf->bmf->NewShape(stPolygon);

                            mpg->points.push_back(Lis_map::Point(point1.L, point1.H));
                            mpg->points.push_back(Lis_map::Point(point2.L, point2.H));
                            mpg->points.push_back(Lis_map::Point(point3.L, point3.H));
                            mpg->points.push_back(Lis_map::Point(point4.L, point4.H));
                            mpg->color = RGB(255, 255 - LineColor, 255 - LineColor);
                            mpg->filled = gradient;

                            /*
                            if (gradient) {
                                ptrPolygonMapObj->Set_Bland(255);
                            } else {
                                ptrPolygonMapObj->Set_Bland(100);
                            }
                            */

                            HDC dc = ::GetDC(cmf->bmf->Map->Handle);
                            mpg->Paint(dc);
                            ::ReleaseDC(cmf->bmf->Map->Handle, dc);
                        }
                    radius ++;
                    }
                }
                cmf->bmf->Map->Refresh();
                //}
                //*******************************************************************************************

            } __finally {
                //  мы ответственны за освобождение массива;
                SafeArrayDestroy(lpsaRes);
                if (resArray)
                    delete[] resArray;
            }
        }
    }
    __finally
    {
        doSelectionStarted = false;
    }

}

void __fastcall TfrmSelection::actExportExecute(TObject *Sender)
{
    if (txList.Size == 0) {
        Application->MessageBox("Список пустий.", Application->Title.c_str(), MB_ICONEXCLAMATION);
        return;
    }

    if (!dlgExport)
        dlgExport = new TdlgExport(Application);

    TCOMILISBCTx tx(txBroker.GetTx(FTxId, dmMain->GetObjClsid(dmMain->GetSc(FTxId))), true);

    TBCTxType txType = tx.get_systemcast();
    // настроим диалог
    if (txType == ttTV) {
        dlgExport->rgFormat->ItemIndex = 0;
        dlgExport->rgFormatClick(dlgExport);
    } else if (txType == ttDVB) {
        dlgExport->rgFormat->ItemIndex = 1;
        dlgExport->rgFormatClick(dlgExport);
    }

    if (dlgExport->ShowModal() == mrOk)
        txExporter.exportTxList(dlgExport->rgFormat->ItemIndex, dlgExport->rgList->ItemIndex, txList, dlgExport->edtFilename->Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actShowTxExecute(TObject *Sender)
{
    TCOMILISBCTx tx(txList.get_Tx(0), true);
    FormProvider.ShowTx(tx);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actTruncateSelectionExecute(TObject *Sender)
{
    actRemoveLessThanZeroExecute(this);
    refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actTruncateSelectionFromCurrentTxExecute(TObject *Sender)
{
    if (grid->dg->Row > 0 || grid->dg->Row < txList.Size)
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "delete from SELECTEDTRANSMITTERS where TRANSMITTERS_ID = :TX and SELECTIONS_ID = " + IntToStr(FId);

        int i = txList.Size;
        while (i-- > grid->dg->Row + 1 )
        {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            int txId = tx.id;
            txList.RemoveId(txId);
            curTxList.RemoveId(txId);
            curSfn.RemoveId(txId);
            curSfnUw1.RemoveId(txId);
            curSfnUw2.RemoveId(txId);
            tags.erase(&tags[i-1]);
            sql->Params->Vars[0]->AsInteger = tx.id;
            sql->ExecQuery();
        }
        sql->Transaction->CommitRetaining();
        grid->Refresh();

        refresh();

        wasChanges = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::ClearZonesCache(std::map<int, LPSAFEARRAY>& cacheMap)
{
    std::map<int, LPSAFEARRAY>::iterator mi = cacheMap.begin();
    while (mi != cacheMap.end()) {
        SafeArrayDestroy(mi->second);
        mi->second = NULL;
        mi++;
    }
    cacheMap.clear();
}
//---------------------------------------------------------------------------

LPSAFEARRAY __fastcall TfrmSelection::getCheckPoints(ILISBCTxList* pTxList,
    //ILISBCTx* pTx, ILISBCTx* pTxIntf,
    LPSAFEARRAY zone)
{
    if (zone == NULL)
        return NULL;

    txAnalyzer.ShowProgress("Розрахунок конетрольних точок");

    long upIdx = -1;
    long loIdx = 0;
    SafeArrayGetLBound(zone, 1, &loIdx);
    SafeArrayGetUBound(zone, 1, &upIdx);

    int num = upIdx - loIdx + 1;
    TSafeArrayDim2 dim2(num, cpdiLast);
    TSafeArrayT<TVariant, VT_VARIANT, 2> saCpRes(dim2);

    //  для одной строчки, будет передаваться в txAnalyzer
    TSafeArrayDim1 dim1(cpdiLast);
    TSafeArrayT<TVariant, VT_VARIANT, 1> saCp(dim1);

    //  оболочки для указателей
    /*
    TCOMILISBCTxList txList(pTxList, true);
    TCOMILISBCTx tx(pTx, true);
    TCOMILISBCTx txIntf(pTxIntf, true);

    TCOMILISBCTxList txList2(pTxList, true);;
    txList2.CreateInstance(CLSID_LISBCTxList);
    txList2.AddTx(pTx);
    txList2.set_TxUseInCalc(0, true);
    txList2.AddTx(pTxIntf);
    for (int i = 1; i < txList.Size; i++) {
        if (txList.get_TxUseInCalc(i)) {
            int newIdx = txList2.AddTx(txList.get_Tx(i));
            txList2.set_TxUseInCalc(newIdx, true);
        }
    }
    */

    TCOMILISBCTxList txList2(pTxList, true);
    TCOMILISBCTx tx(txList2.get_Tx(0), true);
    TCOMILISBCTx txIntf(txList2.get_Tx(1), true);

    TCOMIRSASpherics sphere;
    sphere.CreateInstance(CLSID_RSASpherics);

    //TVariant val;
    //long place[2];
        /*
        cpdiAzimuth = 0,
        cpdiDist,
        cpdiE370,
        cpdiE1546,
        cpdiEu,
        cpdiPr,
        cpdiDa,
        cpdiDaO,
        cpdiStandName,
        cpdiLongitude,
        cpdiLatitude,
        cpdiHeff,
        cpdiPol,
        cpdiRegNum,
        cpdiEi,
        cpdiEii,
        cpdiLast
        */
    WideString stName = tx.station_name;
    WideString regNum = tx.numregion;
    WideString txNum = IntToStr(tx.adminid);
    WideString  zero(L"0");
    while (txNum.Length() < 4)
        txNum.Insert(zero, 0);

    TRSAGeoPoint center;
    center.L = tx.longitude;
    center.H = tx.latitude;
    TRSAGeoPoint cp;
    cp.L = 0;
    cp.H = 0;

    for (int i = 0; i < num; i++) {

        if (txAnalyzer.DoProgress((i+1) * 100 / num))
            break;

        //place[0] = i;
        double d = ((double*)zone->pvData)[i];
        double a = i * 360 / num;

        if (sphere.IsBound())
            sphere.PolarToGeo(d, a, center, &cp);

        saCpRes[i][cpdiStandName] = stName;
        saCpRes[i][cpdiNumReg] = regNum;
        saCpRes[i][cpdiTxNum] = txNum;
        saCpRes[i][cpdiLongitude] = cp.L;
        saCpRes[i][cpdiLatitude] = cp.H;
        saCpRes[i][cpdiAzimuth] = a;
        saCpRes[i][cpdiDist] = d;

        LPSAFEARRAY psa = saCp.Detach();
        txAnalyzer.GetCpE(pTxList, cp.L, cp.H, psa);
        saCp.Attach(psa);

        saCpRes[i][cpdiE01] = (TVariant)saCp[cpdiE01];
        saCpRes[i][cpdiE10] = (TVariant)saCp[cpdiE10];
        saCpRes[i][cpdiE50] = (TVariant)saCp[cpdiE50];
        saCpRes[i][cpdiEu1] = (TVariant)saCp[cpdiEu1];
        saCpRes[i][cpdiEu2] = (TVariant)saCp[cpdiEu2];
        saCpRes[i][cpdiPrC] = (TVariant)saCp[cpdiPrC];
        saCpRes[i][cpdiPrT] = (TVariant)saCp[cpdiPrT];
        saCpRes[i][cpdiDa]  = (TVariant)saCp[cpdiDa];
        saCpRes[i][cpdiDaO] = (TVariant)saCp[cpdiDaO];

        TBCPolarization pol = tx.polarization;
        saCpRes[i][cpdiPol] =
                pol == plVER ? 'V' :
                pol == plHOR ? 'H' :
                pol == plMIX ? 'M' :
                pol == plCIR ? 'C' : '?';
        saCpRes[i][cpdiHeff] = tx.get_h_eff(a);

    }

    txAnalyzer.HideProgress();

    return saCpRes.Detach();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::copyTxToBeforeBase(long TxId)
{
    bool found = false;

    TCOMILISBCTx tx(txList.get_Tx(TxId), true);

    //  найти форму с такими iTx среди MDIChildCount
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ((dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i])) &&
            (dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i])->GetTx() == (ILISBCTx*)tx))
        {
            found = true;
            break;
        }

    if ( !found )
        FormProvider.ShowTx(txList.get_Tx(currentTxIndex));

    //  и еще раз ( нам нужны методы карточки )
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ((dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i])) &&
            (dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i])->GetTx() == (LISBCTx*)tx))
        {
            {
                TfrmTxBase * tf = dynamic_cast<TfrmTxBase*>(frmMain->MDIChildren[i]);
                tf->change_systemcast = false;
                tf->NewTx();
                int txId = tf->NewElementId;

                //старую карточку можно прятать
                if ( !found )
                    tf->Close();//если мы создали -- закрыть
                else
                    tf->SendToBack();

                addTx(tf->NewElementId);//NewElementId -- Id нового передатчика
            }
            break;
        }
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelection::N34Click(TObject *Sender)
{
    checkIndex(currentTxIndex);
    copyTxToBeforeBase(currentTxIndex);
}
//---------------------------------------------------------------------------

//  преобразование цифрового индекса колонки в буквенный
AnsiString __fastcall TfrmSelection::nToAz(int n)
{
    static const int base = 'Z' - 'A' + 1;
    AnsiString res;
    int len = 1;
    int val = n;
    while (val /= base)
        len++;
    res.SetLength(len);
    val = n;
    while (len > 0) {
        res[len--] = 'A' + val % base;
        val /= base;
        //  коррекция. без неё ненулевые разряды начнутся с 'B'
        val--;
    }
    return res;
}
//---------------------------------------------------------------------------           


void __fastcall TfrmSelection::btnCoordinatesClick(TObject *Sender)
{
    if (dlgEnterCoord == NULL)
        dlgEnterCoord = new TdlgEnterCoord(Application);

    if (dlgEnterCoord->ShowModal() == mrOk)
        GetE(dmMain->strToCoord(dlgEnterCoord->edtLon->Text), dmMain->strToCoord(dlgEnterCoord->edtLat->Text));
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::checkIndex(int idx)
{
    if (idx < 0 || idx >= txList.Size)
        throw *(new Exception("Поточний індекс поза межами списка"));
}

void __fastcall TfrmSelection::grdPointDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (Sender == NULL)
        return;

    AnsiString text = Trim(sg->Cells[ACol][ARow]);
    int xOffset = 1;

    if (ACol == 0 || ARow == 0 )
    {
        //  center align
        xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
        sg->Canvas->Brush->Color = clBtnFace;
    } else {
        //  right align
        xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2 ;
    }

    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::chbShowInDetailClick(TObject *Sender)
{
    if (chbShowInDetail->Checked)
    {
        if (lastCpLon != 0 && lastCpLat != 0 )
        {
            ShowCheckPoint(lastCpLon, lastCpLat);
            if (frmPoint)
                frmPoint->pnlCpReq->Visible = false;
        }
    } else {
        if (frmPoint)
            frmPoint->Close();
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelection::fillCalcTxList()
{
    curTxList.Clear();
    curSfn.Clear();
    curSfnUw1.Clear();
    curSfnUw2.Clear();

    TCOMILISBCTx analyzeTx(txList.get_Tx(currentTxIndex), true);

    curTxList.AddTx(analyzeTx);
    curTxList.set_TxUseInCalc(0, true);
    int interfIdx = cbxUnwantedTx->ItemIndex;
    if (interfIdx > -1) {
        interfIdx = (int)cbxUnwantedTx->Items->Objects[interfIdx];
        int newIdx = curTxList.AddTx(txList.get_Tx(interfIdx));
        curTxList.set_TxUseInCalc(newIdx, txList.get_TxUseInCalc(interfIdx));
    }
    int interfIdx2 = cbxUnwantedTx2->ItemIndex;
    if (interfIdx2 > -1) {
        interfIdx2 = (int)cbxUnwantedTx2->Items->Objects[interfIdx2];
        int newIdx = curTxList.AddTx(txList.get_Tx(interfIdx2));
        curTxList.set_TxUseInCalc(newIdx, txList.get_TxUseInCalc(interfIdx2));
    }
    for (int i = 0; i < txList.Size; i++) {
        if (txList.get_TxUseInCalc(i))
        {
            int newIdx = curTxList.AddTx(txList.get_Tx(i));
            curTxList.set_TxUseInCalc(newIdx, true);
        }
    }

    GetSfn(curSfn, currentTxIndex);
    GetSfn(curSfnUw1, interfIdx);
    GetSfn(curSfnUw2, interfIdx2);
}

void __fastcall TfrmSelection::showCurTxZones()
{
    if (!txList.IsBound())
        return;

    TCOMILISBCTx tx(txList.get_Tx(currentTxIndex), true);
    int ID = tx.id;

    std::map<int, LPSAFEARRAY>::iterator ic;
    std::map<int, LPSAFEARRAY>::iterator io;
    std::map<int, LPSAFEARRAY>::iterator iw;    

    bool compareIntfs = chbTwoUnwantedTxs->Checked;

    std::map<int, LPSAFEARRAY> &z1 = compareIntfs ? interfereLimited : noiseLimited;
    std::map<int, LPSAFEARRAY> &z2 = compareIntfs ? interfereLimited2 : interfereLimited;

    TColor col1 = compareIntfs ? BCCalcParams.lineColorZoneInterfere : BCCalcParams.lineColorZoneNoise;
    TColor col2 = compareIntfs ? BCCalcParams.lineColorZoneInterfere2 : BCCalcParams.lineColorZoneInterfere;
    int thick1 = compareIntfs ? BCCalcParams.lineThicknessZoneInterfere : BCCalcParams.lineThicknessZoneNoise;
    int thick2 = BCCalcParams.lineThicknessZoneInterfere;

    polarDiagramPanel->clear();
    if ((ic = coverage.find(ID)) != coverage.end()) {
        polarDiagramPanel->setCoverage((double*)ic->second->pvData,
                                        BCCalcParams.lineColorZoneCover,
                                        BCCalcParams.lineThicknessZoneCover,
                                        ic->second->rgsabound[0].cElements);
    }
    if ((io = z1.find(ID)) != z1.end()) {
        polarDiagramPanel->setNoiseLimited((double*)io->second->pvData,
                                        col1,
                                        thick1,
                                        io->second->rgsabound[0].cElements);
    }
    if ((iw = z2.find(ID)) != z2.end()) {
        polarDiagramPanel->setInterfereLimited((double*)iw->second->pvData,
                                        col2,
                                        thick2,
                                        iw->second->rgsabound[0].cElements);
    }

    if (compareIntfs) {
        grdZones->Cells[2][0] = "зав. 1";
        grdZones->Cells[3][0] = "зав. 2";
    } else {
        grdZones->Cells[2][0] = "без зав.";
        grdZones->Cells[3][0] = "із зав.";
    }

    char res[10];
    int pointNum = 360 / BCCalcParams.degreeStep;
    grdZones->RowCount = pointNum + 1;

    for (int i = 0; i < pointNum; i++) {
        sprintf(res, " %3d ", i * 360 / pointNum);
        grdZones->Cells[0][i+1] = res;
        if (ic != coverage.end() && ic->second->rgsabound[0].cElements == pointNum) {
            sprintf(res, " %5.1f ", ((double*)ic->second->pvData)[i]);
            grdZones->Cells[1][i+1] = res;
        } else
            grdZones->Cells[1][i+1] = "";
        if (io != z1.end() && io->second->rgsabound[0].cElements == pointNum) {
            sprintf(res, " %5.1f ", ((double*)io->second->pvData)[i]);
            grdZones->Cells[2][i+1] = res;
        } else
            grdZones->Cells[2][i+1] = "";
        if (iw != z2.end() && iw->second->rgsabound[0].cElements == pointNum) {
            sprintf(res, " %5.1f ", ((double*)iw->second->pvData)[i]);
            grdZones->Cells[3][i+1] = res;
        } else
            grdZones->Cells[3][i+1] = "";
    }

    cmf->Clear(checkPointsLayer);

    std::map<int, LPSAFEARRAY>::iterator cpi;
    if (BCCalcParams.showCp && ((cpi = checkPoints.find(ID)) != checkPoints.end()))
    {

        TSafeArrayT<TVariant, VT_VARIANT, 2> saCp;
        saCp.Attach(cpi->second);
        long num = saCp.get_BoundsLength(0);

        //  ищем точку с максимальной разницей напряженностей помехи
        //  она будет выделена жирно
        int maxDifIdx = 0;
        double dif, maxDif;

        for (int i = 0; i < num; i++)
        {
            dif = (double)(TVariant)saCp[i][cpdiEu2] - (double)(TVariant)saCp[i][cpdiEu1];
            if (i == 0 || maxDif < dif)
            {
                maxDifIdx = i;
                maxDif = dif;
            }
        }

        //  рисуем каждую точку
        for (int i = 0; i < num; i++) {

            AnsiString hint;

            //  coord
            double longitude = (double)(TVariant)saCp[i][cpdiLongitude];
            double latitude = (double)(TVariant)saCp[i][cpdiLatitude];

            double eu1 = (double)(TVariant)saCp[i][cpdiEu1];
            double eu2 = (double)(TVariant)saCp[i][cpdiEu2];

            AnsiString name = (wchar_t*)(TVariant)saCp[i][cpdiStandName];
            AnsiString regnum = (wchar_t*)(TVariant)saCp[i][cpdiNumReg];
            AnsiString txnum = (wchar_t*)(TVariant)saCp[i][cpdiTxNum];

            TBCTxType txType = tx.systemcast;
            int color = (txType == ttTV || txType == ttDVB) ?
                    ((eu2 - eu1 > BCCalcParams.treshVideo) ? clRed : clBlue) :
                    ((eu2 - eu1 > BCCalcParams.treshAudio) ? clRed : clBlue);

            int weight = i == maxDifIdx ? 9 : 5;
            char symbol = '\0';

            hint = AnsiString().sprintf("Азімут, град = %.1f\n"
                                        "Відстань, км = %.1f\n"
                                        "ЕВП (без) = %.2f дбкВт\n"
                                        "ЕВП (зав) = %.2f дбкВт\n"
                                        "Е  1%% = %.2f дБмкВ/м\n"
                                        "Е 10%% = %.2f дБмкВ/м\n"
                                        "Е 50%% = %.2f дБмкВ/м\n"
                                        "ЗО c = %.1f дБ\n"
                                        "ЗО t = %.1f дБ\n"
                                        "D ант = %.1f дБ\n"
                                        "D ант орт = %.1f дБ\n"
                                        "Опора '%s'\n"
                                        "Регіон, №Tx '%s %s'\n"
                                        "Довгота = %s\n"
                                        "Широта = %s\n"
                                        "Нефф, м = %d\n"
                                        "Пол = '%c'"
                                         , (double)(TVariant)saCp[i][cpdiAzimuth]
                                         , (double)(TVariant)saCp[i][cpdiDist]
                                         , eu1
                                         , eu2
                                         , (double)(TVariant)saCp[i][cpdiE01]
                                         , (double)(TVariant)saCp[i][cpdiE10]
                                         , (double)(TVariant)saCp[i][cpdiE50]
                                         , (double)(TVariant)saCp[i][cpdiPrC]
                                         , (double)(TVariant)saCp[i][cpdiPrT]
                                         , (double)(TVariant)saCp[i][cpdiDa]
                                         , (double)(TVariant)saCp[i][cpdiDaO]
                                         , name.c_str()
                                         , regnum.c_str(), txnum.c_str()
                                         , dmMain->coordToStr(longitude, 'X').c_str()
                                         , dmMain->coordToStr(latitude, 'Y').c_str()
                                         , (long)(TVariant)saCp[i][cpdiHeff]
                                         , (char)(TVariant)saCp[i][cpdiPol]
                                        );

            MapPoint* p = cmf->ShowPoint(longitude, latitude, color, weight, ptPoint, AnsiString(), hint);
            p->SetLayer(checkPointsLayer);

        }
        saCp.Detach();

    }
}

void __fastcall TfrmSelection::actExportSelectionToExcelExecute(TObject *Sender)
{
    Variant excel = CreateOleObject("Excel.Application");

    excel.OlePropertyGet("Application").OlePropertySet<int>("SheetsInNewWorkbook", 1);
    excel.OlePropertyGet("Workbooks").OleProcedure("Add");

    excel.OlePropertyGet<WideString>("Rows", "1:1").OleProcedure("Select");
    excel.OlePropertyGet("Selection").OlePropertyGet("Font").OlePropertySet<VARIANT_BOOL>("Bold", true);

    int colArrayLen = grid->columnsInfo.size();
    for ( int colIndex = 0; colIndex < colArrayLen; colIndex++ )
    {
        excel.OlePropertyGet<WideString>("Range", nToAz(colIndex) + '1').OleProcedure("Select");
        excel.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1", grid->columnsInfo[colIndex].title);
    }

    for ( int rowIndex = 2; rowIndex <= txList.Size + 1; rowIndex++ )
        for ( int colIndex = 0; colIndex < colArrayLen; colIndex++ )
        {
            excel.OlePropertyGet<WideString>("Range", nToAz(colIndex) + rowIndex).OleProcedure("Select");
            excel.OlePropertyGet("ActiveCell").OlePropertySet<WideString>("FormulaR1C1",
                                                selProxi->GetVal(grid, rowIndex-2, grid->columnsInfo[colIndex])
                                                );
        }

    {
        excel.OlePropertyGet<WideString>("Columns", AnsiString("A:")+nToAz(colArrayLen)).OlePropertyGet("EntireColumn").OleProcedure("AutoFit");
        excel.OlePropertyGet<WideString>("Range", "A1").OleProcedure("Select");
    }

    excel.OlePropertySet<VARIANT_BOOL>("Visible", true);
    excel = Unassigned;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::UpdateLines()
{
    if ( pcSelection->ActivePage == tshMap )
        ActivateMapSheet();
}
//---------------------------------------------------------------------------  

void __fastcall TfrmSelection::UpdateTxNames()
{
    if ( pcSelection->ActivePage == tshMap )
        ActivateMapSheet();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actDegradationSectorSelectionExecute(TObject *Sender)
{
    if (BCCalcParams.FCalcSrv.IsBound())
    {
        fillCalcTxList();

        BCCalcParams.FCalcSrv.SetTxListServer(curTxList);

        TfrmTable36 *TableForm = new TfrmTable36(this, t36DegradationSector, txList.get_Tx((int)(cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex])));
        TableForm->Show();
    } else {
        ShowMessage("CalcServer is absent");
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::CoordinationPointsHide()
{
    for ( std::vector<MapPoint*>::iterator coordinationPoint = mapCPoints.begin(); coordinationPoint != mapCPoints.end(); coordinationPoint++ )
        if (*coordinationPoint)
            try {
                (*coordinationPoint)->CheckIsValid();
                delete (*coordinationPoint);
            } catch (...) {}
    mapCPoints.clear();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::CoordinationPointsFillCountryName(std::vector<CoordinationPoint>& cordinationPoints)
{
    std::set<int> countryIds;

    for ( std::vector<CoordinationPoint>::iterator cordinationPoint = cordinationPoints.begin(); cordinationPoint != cordinationPoints.end(); cordinationPoint++ )
        countryIds.insert(cordinationPoint->countryId);

    dmMain->ibdsCountries->Open();
    if ( !countryIds.empty() )
    {
        std::set<int>::iterator si = countryIds.begin();
        while ( si != countryIds.end() )
        {
            if (dmMain->ibdsCountries->Locate("ID", *si, TLocateOptions()))
                for ( std::vector<CoordinationPoint>::iterator cordinationPoint = cordinationPoints.begin(); cordinationPoint != cordinationPoints.end(); cordinationPoint++ )
                    if ( cordinationPoint->countryId == *si )
                        cordinationPoint->countryName = dmMain->ibdsCountriesNAME->AsString;

            si++;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::CoordinationPointsGet(std::vector<CoordinationPoint>& cordinationPoints)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
      sql->Database = dmMain->dbMain;;
      sql->GoToFirstRecordOnExecute = true;
      sql->SQL->Text = "select CP.LATITUDE LATITUDE, LONGITUDE, CP.NUMBOUND SECTOR, CN.COUNTRY_ID COUNTRY_ID from COORDPOINTS CP left outer join COUNTRYPOINTS CN on (CP.NUMBOUND = CN.NUMBOUND)";
    sql->ExecQuery();

    std::set<int> countryIds;

    while ( !sql->Eof )
    {
        TRSAGeoPoint point;
        point.H = sql->FieldByName("LATITUDE")->AsDouble;
        point.L = sql->FieldByName("LONGITUDE")->AsDouble;

        cordinationPoints.push_back(CoordinationPoint(point, true, sql->FieldByName("COUNTRY_ID")->AsInteger, sql->FieldByName("SECTOR")->AsInteger));

        int countryId = sql->FieldByName("COUNTRY_ID")->AsInteger;
        countryIds.insert(sql->FieldByName("COUNTRY_ID")->AsInteger);

        sql->Next();
    }

    sql->Close();

    dmMain->ibdsCountries->Open();
    if ( !countryIds.empty() )
    {
        std::set<int>::iterator si = countryIds.begin();
        while ( si != countryIds.end() )
        {
            if (dmMain->ibdsCountries->Locate("ID", *si, TLocateOptions()))
                for ( std::vector<CoordinationPoint>::iterator cordinationPoint = cordinationPoints.begin(); cordinationPoint != cordinationPoints.end(); cordinationPoint++ )
                    if ( cordinationPoint->countryId == *si )
                        cordinationPoint->countryName = dmMain->ibdsCountriesNAME->AsString;

            si++;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::CoordinationPointsShow()
{
    /*
    if ( cordinationPoints.empty() )
        CoordinationPointsGet(cordinationPoints);
    */
    CoordinationPointsHide();

    int cordinationPointIndex = 0;
    for ( std::vector<CoordinationPoint>::const_iterator cordinationPoint = cordinationPoints.begin(); cordinationPoint != cordinationPoints.end(); cordinationPoint++, cordinationPointIndex++ )
        mapCPoints.push_back(cmf->ShowPoint(cordinationPoint->point.L, cordinationPoint->point.H,
                                         cordinationPoint->inZone ? BCCalcParams.coordinationPointsInZoneColor : BCCalcParams.coordinationPointsOutZoneColor,
                                         8, ptPlusCross, "",
                                         IntToStr(cordinationPoint->sector) + " / " + cordinationPoint->countryName));
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actCoordinationPointsShowExecute(TObject *Sender)
{
    coordinationPointsShow = !coordinationPointsShow;

    if ( coordinationPointsShow )
        CoordinationPointsShow();
    else
        CoordinationPointsHide();

    cmf->bmf->Map->Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::tmrInvalidateTimer(TObject *Sender)
{
    //  для перерисовки изменённых передатчиков
    grid->dg->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::chbTwoUnwantedTxsClick(TObject *Sender)
{
    lblUnwantedTx2->Enabled = chbTwoUnwantedTxs->Checked;
    cbxUnwantedTx2->Enabled = chbTwoUnwantedTxs->Checked;
    cbxUnwantedTx2->Font->Color = chbTwoUnwantedTxs->Checked ? clWindowText : cbxUnwantedTx2->Color;

    if (txList.IsBound())
    {
        DropCalculatedData();
        showCurTxZones();
        fillCalcTxList();

        cmf->bmf->Map->Refresh();
    }

    if (cbxUnwantedTx2->ItemIndex > -1)
    {
        SetTxColor(txList.get_TxId((int)cbxUnwantedTx2->Items->Objects[cbxUnwantedTx2->ItemIndex]),
            chbTwoUnwantedTxs->Checked ? BCCalcParams.lineColorZoneInterfere2 : ccTx);

        cmf->bmf->Map->Refresh();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::grdZonesDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;                                                 

    if (ARow == 0)
    {
        if (chbTwoUnwantedTxs->Checked)
        {
            if (ACol == 2)
                sg->Canvas->Font->Color = BCCalcParams.lineColorZoneInterfere;
            if (ACol == 3)
                sg->Canvas->Font->Color = BCCalcParams.lineColorZoneInterfere2;
        } else {
            if (ACol == 2)
                sg->Canvas->Font->Color = BCCalcParams.lineColorZoneNoise;
            if (ACol == 3)
                sg->Canvas->Font->Color = BCCalcParams.lineColorZoneInterfere;
        }

        //sg->Canvas->Font->Style = sg->Canvas->Font->Style >> fsBold;
    }

    AnsiString text = Trim(sg->Cells[ACol][ARow]);

    int xOffset = 1;

    if (ACol > 0 && ARow !=0)
    {   //  right align
        xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2 ;
    }
    if (ARow == 0 || ACol == 0)
    {   //  center align
        xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
    }
    if ( !State.Contains(gdSelected) )
    {
        TColor oldColor = sg->Canvas->Brush->Color;
        sg->Canvas->Brush->Color = clYellow;
        sg->Canvas->FillRect(Rect);
        sg->Canvas->Brush->Color = oldColor;
    }
    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::DropCalculatedData(bool refreshMap)
{
    ClearZonesCache(noiseLimited);
    ClearZonesCache(interfereLimited);
    ClearZonesCache(interfereLimited2);
    ClearZonesCache(checkPoints);
    clearPointData();
    clearPoint2Data();

    noiseZones.clear();
    interfZones.clear();
    interfZones2.clear();

    cmf->Clear(layer_default);
    cmf->Clear(checkPointsLayer, refreshMap);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::FormShow(TObject *Sender)
{
    // form children read their state from resource stream only when form is first time shown
    grid->RecreateHeader();
    actSortFromUsExecute(Sender);
    grid->Refresh();

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgDblClick(TObject *Sender)
{
    if (currentTxIndex <= 0)
        selectAnalyzeTx(grid->dg->Row + 1);

    if (actCalcDuel->Enabled) {
        actCalcDuelExecute(this);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgDragDrop(TObject *Sender,
      TObject *Source, int X, int Y)
{
    sgrSelectionDragDrop(Sender, Source, X, Y);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgDragOver(TObject *Sender,
      TObject *Source, int X, int Y, TDragState State, bool &Accept)
{
    sgrSelectionDragOver(Sender, Source, X, Y, State, Accept);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = grid->dg;

    grid->dgDrawCell(Sender, ACol, ARow, Rect, State);

    // draw bitmaps - ObjectList Grid can't do this

    TBCTxType tt = txAnalyzer.GetDiapason(txList);
    
    if (ACol == 1 || ACol == 2 || ACol == 3) {
        if (sg->Cells[ACol][ARow] == "(+)") {

            Graphics::TBitmap *bmp = useInCalcBitmap.get(); // (ACol == 2)
            if (ACol == 2)
                bmp = showOnMapBitmap.get();
            else if (ACol == 3)
            {
                if (tt != ttAM)
                    bmp = zoneOverlapBitmap.get();
                else
                    bmp = isDayBitmap.get();
            }


            sg->Canvas->Brush->Style = bsSolid;
            if (State.Contains(gdSelected))
                sg->Canvas->Brush->Color = clHighlight;
            else
                sg->Canvas->Brush->Color = clWindow;
            sg->Canvas->FillRect(Rect);
            TRect source(0, 0, bmp->Height, bmp->Width);
            TRect dest(Rect);
            dest.top += ((dest.bottom - dest.top - bmp->Height) / 2);
            dest.bottom = dest.top + bmp->Height;
            int xOffset = (Rect.Right - Rect.left - bmp->Width) / 2;
            if (xOffset < 0) {
                xOffset = -xOffset;
                source.left += xOffset;
                source.right -= xOffset;
            } else {
                dest.left += xOffset;
                dest.right = dest.left + bmp->Width;
            }
            if (State.Contains(gdSelected)) {
                sg->Canvas->CopyMode = cmMergePaint;
            } else {
                sg->Canvas->CopyMode = cmSrcAnd;
            }
            sg->Canvas->CopyRect(dest, bmp->Canvas, source);
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    grid->dgKeyDown(Sender, Key, Shift);
    if (Key != VK_ESCAPE)
        sgrSelectionKeyDown(Sender, Key, Shift);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgKeyUp(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    selectAnalyzeTx(grid->dg->Row + 1);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::griddgMouseUp(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    selectAnalyzeTx(grid->dg->Row + 1);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::ArrangeAllotGroups()
{
    // looks for allotment for every tx in list
    // tx's from the same allotment are grouped and located right after it
    // allotment is located just before the first tx of it

    //TCOMILISBCTxList txList(list, true);

    if (txList.Size < 2)
        return;

    TempCursor tc(crHourGlass);
    String msg("Растасовка передатчиков по выделениям...");
    TempStatusString tss(frmMain->StatusBar1, msg);
    frmMain->StatusBar1->Update();

    TCOMILISBCTxList tempTxList;
    tempTxList.CreateInstance(CLSID_LISBCTxList);

    int size = txList.Size;
    for (int i = 0; i < size; i++)
        txAnalyzer.CopyTx(tempTxList, txList, i);

    //cache
    allots.clear();
    txGroups.clear();
    allotGroups.clear();
    curAllot.clear();

    std:auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = " select t.ID, t.ASSOCIATED_ADM_ALLOT_ID"
                ", a.ID from TRANSMITTERS t"
                " left join DIG_ALLOTMENT a on (t.ASSOCIATED_ADM_ALLOT_ID = a.ADM_REF_ID) "
                " where t.ID = " + IntToStr(FTxId);
    sql->ExecQuery();
    String SelectionOwnerAllotName;
    if (!sql->Eof)
    {
        int txId = sql->Fields[0]->AsInteger;
        AnsiString allotName = SelectionOwnerAllotName = sql->Fields[1]->AsString.Trim();
        int allotId = sql->Fields[2]->AsInteger;

        allots[txId] = allotName;

        if (allotName.Length() > 0 && allotId > 0)
        {
            allotGroups[allotName].insert(allotId);

            txGroups[allotName].insert(txId);

            if (curAllot.find(allotName) == curAllot.end())
                curAllot.insert(pair<AnsiString,int>(allotName, allotId));
        }

    }

    sql->Close();
    sql->SQL->Text = " select t.ID, t.ASSOCIATED_ADM_ALLOT_ID"
                ", a.ID from TRANSMITTERS t"
                " right join SELECTEDTRANSMITTERS s on (t.id = s.TRANSMITTERS_ID)"
                " left join DIG_ALLOTMENT a on (t.ASSOCIATED_ADM_ALLOT_ID = a.ADM_REF_ID) "
                " where s.SELECTIONS_ID = :ID";
                //" and s.TRANSMITTERS_ID <> :ID)";

    sql->Params->Vars[0]->AsInteger = FId;


    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        int txId = sql->Fields[0]->AsInteger;
        AnsiString allotName = sql->Fields[1]->AsString.Trim();
        int allotId = sql->Fields[2]->AsInteger;

        allots[txId] = allotName;

        if (allotName.Length() > 0 && allotId > 0)
        {
            allotGroups[allotName].insert(allotId);

            txGroups[allotName].insert(txId);

            if (curAllot.find(allotName) == curAllot.end())
                curAllot.insert(pair<AnsiString,int>(allotName, allotId));
        }

    }
    sql->Close();

    //clear list
    txList.Clear();
    tags.clear();

    // first ("zero") tx
    ILISBCTx* ptx = tempTxList.get_Tx(0);
    txList.AddTx(ptx);
    //tempTxList.RemoveTx(ptx);  -- don't do that, next step will be adding Allot and all of it's txs,
    //if this ne has
    ptx = NULL;

    int majorNo = 1;

    // now we will consequently fill the list, packing tx's belonging to same allotment
    while (tempTxList.Size > 0)
    {
        int elemIdx = 0;
        TCOMILISBCTx tx(tempTxList.get_Tx(elemIdx), true);
        bool showItOnMap = elemIdx == 0 || tempTxList.get_TxShowOnMap(elemIdx);
        AnsiString allotName;
        if (tx.systemcast == ttAllot)
        {
            TCOMILisBcDigAllot allot;
            OleCheck(tx->QueryInterface(IID_ILisBcDigAllot, (void**) &allot));
            WideString ws;
            OleCheck(allot->get_adm_ref_id(&ws));
            allotName = AnsiString(ws).Trim();
            if (SelectionOwnerAllotName.Length() > 0 && SelectionOwnerAllotName == allotName)
                majorNo = 0;
        } else {
            // check if tx references to allotment and if this allotment is present in list
            // if not present - add it
            IntToStrMap::iterator ali = allots.find(tx.id);
            if (ali != allots.end())
            {
                allotName = ali->second;
                // check if there is any allotment with such name
                StrToIntMap::iterator cai = curAllot.find(allotName);
                if (cai != curAllot.end())
                {
                    if (txList.Size == 1) // zero tx has allotments and this is one
                        majorNo = 0;
                    // get allotment and find it's position in list
                    int allotId = cai->second;
                    tx.Bind(txBroker.GetTx(allotId, CLSID_LisBcDigAllot), true);

                    showItOnMap = false; // if we'll find it in list, it will be set up
                    for (int i = 1; i < tempTxList.Size; i++)
                    {
                        if (tempTxList.get_Tx(i) == tx)
                        {
                            elemIdx = i;
                            showItOnMap = tempTxList.get_TxShowOnMap(elemIdx);
                            break;
                        }
                    }

                } else {
                    // no such allot in DB. reset allot name
                    allotName = "";
                }
            } else {
                // no such allot in DB. reset allot name
                allotName = "";
            }
        }

        int newIdx = txList.AddTx(tx);
        if (newIdx > 0)
            tags.push_back(IntToStr(majorNo)); // нулевой - не нада. а то проскакивает иногда
        else if (majorNo > 0)                  // и сбросить счётчик заодно
            majorNo = 0;                       //TODO: вот тут корректировать надо будет, если 0 объект - выделение 

        // elemIdx is 0 if element is ordinary tx, or
        // if tx is allotment and we havent found it in list (got from broker),
        //      so in the second case just copy it's flags from 0 element
        txList.set_TxAzimuth(newIdx, tempTxList.get_TxAzimuth(elemIdx));
        txList.set_TxDistance(newIdx, tempTxList.get_TxDistance(elemIdx));
        if (tx.systemcast == ttAllot)
        {
            txList.set_TxUseInCalc(newIdx, false);
            txList.set_TxShowOnMap(newIdx, showItOnMap);
        } else {
            txList.set_TxUseInCalc(newIdx, tempTxList.get_TxUseInCalc(elemIdx));
            txList.set_TxShowOnMap(newIdx, tempTxList.get_TxShowOnMap(elemIdx));
        }
        txList.set_TxUnwantInterfere(newIdx, tempTxList.get_TxUnwantInterfere(elemIdx));
        txList.set_TxUnwantedKind(newIdx, tempTxList.get_TxUnwantedKind(elemIdx));
        txList.set_TxWantedKind(newIdx, tempTxList.get_TxWantedKind(elemIdx));
        txList.set_TxWantInterfere(newIdx, tempTxList.get_TxWantInterfere(elemIdx));
        txList.set_TxZoneOverlapping(newIdx, tempTxList.get_TxZoneOverlapping(elemIdx));

        tempTxList.RemoveTx(tx);

        if (allotName.Length() > 0 && tempTxList.Size > 0)
        {
            // tx added is an allotment. it's name (adm_ref_id) is in allotName
            // get all txs referensing this allot and place them right after it
            std::set<int>& txIds = txGroups[allotName];
            int minorNo = 1;
            for (std::set<int>::iterator idi = txIds.begin(); idi != txIds.end(); idi++)
            {
                for (int i = 0; i < tempTxList.Size; i++)
                {
                    int txId = tempTxList.get_TxId(i);
                    if (*idi == txId)
                    {
                        // this id belongs to tx that belongs to allotment with adm_ref_if == allotName
                        if (txId != FTxId) // already in list
                        {
                            txAnalyzer.CopyTx(txList, tempTxList, i);
                            tags.push_back(IntToStr(majorNo) + '.' + IntToStr(minorNo));
                        }
                        tempTxList.RemoveId(txId);

                        minorNo++;
                        break;
                    }
                }
            }
        }

        majorNo++;
    }

}

int GetTxIdxFromName(String txName, TStrings *sl)
{
    int i = txName.Pos("&");
    if (i > 0)
        txName.Delete(i,1);
    for (int i = 0; i < sl->Count; i ++)
        if (sl->Strings[i].Pos(txName) == 1)
            return (int)sl->Objects[i];
    return -1; // хуй его знает
}

void __fastcall TfrmSelection::OnObjectSelection(TObject * Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift)
{
    int currentCursor = cmf->bmf->Map->Cursor;
    pmnTx->Items->Clear();
    if (Button == Controls::mbLeft && (currentCursor == crGetTx || currentCursor == crCross) && shapes.size() > 0)
    {
        if (shapes.size() == 1)
        {
            MapShape *msh = shapes[0];
            MapPoint *mp = dynamic_cast<MapPoint*>(msh);
            if (msh->name.Length() > 0)
            {
                selectAnalyzeTx(GetTxIdxFromName(msh->name, cbxWantedTx->Items));
                clearPointData();
                clearPoint2Data();
            } else if (mp) // name is empty, so this is checkpoint
                ShowCheckPoint(mp->x, mp->y);
        }
        else // if (shapes.size() > 1) bring selection menu
        {
            bool cpAdded = false;
            for (int i = 0; i < shapes.size(); i++)
            {
                MapShape *msh = shapes[i];
                MapPoint *mp = dynamic_cast<MapPoint*>(msh);
                if (msh->name.Length() > 0)
                {
                    TMenuItem *mni = new TMenuItem(pmnTx);
                    mni->Caption = msh->name;
                    mni->OnClick = SelectTxClick;
                    pmnTx->Items->Add(mni);
                } else if (mp && !cpAdded)
                {
                    clearPointData();
                    clearPoint2Data();
                    TMenuItem *mni = new TMenuItem(pmnTx);
                    mni->Caption = "Контрольная точка";
                    lastCpLon = mp->x;
                    lastCpLat = mp->y;
                    mni->OnClick = SelectCpClick;
                    pmnTx->Items->Add(mni);
                    cpAdded = true;
                }
            }
            POINT point;
            if (::GetCursorPos(&point))
                pmnTx->Popup(point.x, point.y);
        }
    }
    if (Button == Controls::mbRight)
    {
        // bring tx menu (cascaded if (shapes.size() > 1))
        for (int i = 0; i < shapes.size(); i++)
        {
            TMenuItem *mni = new TMenuItem(pmnTx);
            mni->Caption = shapes[i]->name;
            pmnTx->Items->Add(mni);
            CreateTxSubMenu((shapes.size() > 1) ? mni : pmnTx->Items, shapes[i]);
            if (shapes.size() == 1)
                mni->Enabled = false;
        }
        /*
        if (shapes.size() == 1)
            CreateTxSubMenu(pmnTx->Items, shapes[0]);
        else
            for (int i = 0; i < shapes.size(); i++)
            {
                TMenuItem *mni = new TMenuItem(pmnTx);
                mni->Caption = shapes[i]->name;
                CreateTxSubMenu(mni, shapes[i]);
                pmnTx->Items->Add(mni);
            }
        */
    }

}

void __fastcall TfrmSelection::SetTxColor(int tx_id, TColor color)
{
    //MapShapeMulti::iterator shi = txs.find(tx_id);
    pair<MapShapeMulti::iterator, MapShapeMulti::iterator> range = txs.equal_range(tx_id);

    //if (shi != txs.end())
    if (range.second != txs.end())
        range.second++;
    AnsiString msg("Не тот Id: ");
    for (MapShapeMulti::iterator shi = range.first; shi != range.second; shi++)
    {
        if (shi->first == tx_id)
        {
            cmf->Highlight(shi->second->GetId());
            shi->second->color = color;
        } /* else
            MessageBox(NULL,
                       (msg+"tx_id = "+IntToStr(tx_id)+", shi->first = "+IntToStr(shi->first)).c_str(),
                       "Achtung!", MB_ICONEXCLAMATION);
        */
    }
}

void __fastcall TfrmSelection::MapMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    pmnTx->Items->Clear();
    if (oldMouseDown)
        oldMouseDown(Sender, Button, Shift, X, Y);

    if ((int)cmf->bmf->Map->Cursor == crGetE) {
        double x;
        double y;
        cmf->bmf->CoordScreenToMap(X, Y, &x, &y);
        GetE(x, y);
    }

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::MapMouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y)
{
    if (oldMouseMove)
        oldMouseMove(Sender, Shift, X, Y);

    double x;
    double y;
    cmf->bmf->CoordScreenToMap(X, Y, &x, &y);

    TBCTxType tt = ttUNKNOWN;                 
    TCOMILISBCTx tx;
    if (currentTxIndex > -1)
    {
        tx.Bind(txList.get_Tx(currentTxIndex), true);
        tt = tx.systemcast;
    }

    TRSAGeoPoint gp;
    gp.L = x;
    gp.H = y;
    cmf->sb->Panels->Items[2]->Text = "";
    cmf->sb->Panels->Items[3]->Text = "";
    try {
        if (tt == ttAM)
            cmf->sb->Panels->Items[3]->Text = String().sprintf("%.2f mS/m", txAnalyzer.GetGndCond(x,y));
        else if (BCCalcParams.FTerrInfoSrv.IsBound())
        {
            TRSAMorpho m;
            BCCalcParams.FTerrInfoSrv.Get_Morpho(gp, &m);
            cmf->sb->Panels->Items[3]->Text = AnsiString().sprintf("Покр.: %d", (unsigned)m.Kind);
        }
        #ifndef _DEBUG
        if (BCCalcParams.FTerrInfoSrv.IsBound())
        {
            short a;
            BCCalcParams.FTerrInfoSrv.Get_Altitude(gp, &a);
            cmf->sb->Panels->Items[2]->Text = AnsiString().sprintf("%d м", a);
        }
        #endif
    } catch(...) {}

    cmf->sb->Panels->Items[6]->Text = "";
    cmf->sb->Panels->Items[7]->Text = "";
    if (tx.IsBound())
    {
        try {

            Rsageography_tlb::TRSAGeoPoint A;
            Rsageography_tlb::TRSAGeoPathData Data;
            Rsageography_tlb::TRSAGeoPathResults Results;

            tx->get_latitude(&A.H);
            tx->get_longitude(&A.L);
            long ha;
            tx->get_heightantenna(&ha);
            Data.TxHeight = ha;
            Data.RxHeight = 10;

            if (tt == ttAM)
            {
              cmf->sb->Panels->Items[6]->Text = String().sprintf("%.1f км | %.0f\xB0",
                              txAnalyzer.GetDistance(A.L, A.H, x, y), txAnalyzer.GetAzimuth(A.L, A.H, x, y));
            } else if (BCCalcParams.FPathSrv.IsBound()) {
                BCCalcParams.FPathSrv->RunPointToPoint(A, gp, Data, &Results);
                cmf->sb->Panels->Items[6]->Text = String().sprintf("%.1f км | %.0f\xB0",
                                                    Results.Distance, Results.Azimuth);
                cmf->sb->Panels->Items[7]->Text = String().sprintf("уг %.1f\xB0 : %.1f\xB0",
                                                    Results.TxClearance, Results.RxClearance);
            }
        } catch(...) {}
    } 

    if (tt == ttAM || BCCalcParams.FCalcSrv.IsBound()) {
        try {
            String val;
            if (tx.IsBound())
            {
                double e;
                char it = '\0';
                if (tx.systemcast == ttAllot)
                    e = txAnalyzer.GetSumE(curSfn, x, y);
                else
                    e = txAnalyzer.GetE(tx, x, y, &it);
                val = String().sprintf("W: %.2f dB %c ", e, it);
            }
            cmf->sb->Panels->Items[4]->Text = val;

            val.SetLength(0);
            double e = -99.;
            char it = '\0';
            if (selectedUnwanted.Size > 0)
                e = txAnalyzer.GetSumE(selectedUnwanted, x, y);
            else if (cbxUnwantedTx->ItemIndex > -1)
            {
                tx.Bind(txList.get_Tx((int)cbxUnwantedTx->Items->Objects[cbxUnwantedTx->ItemIndex]), true);
                if (tx.systemcast == ttAllot)
                    e = txAnalyzer.GetSumE(curSfnUw1, x, y);
                else
                    e = txAnalyzer.GetE(tx, x, y, &it);
            }
            val = String().sprintf("Uw: %.2f %c ", e, it);
            if (chbTwoUnwantedTxs->Checked && cbxUnwantedTx2->ItemIndex > -1)
            {
                char it = '\0';
                tx.Bind(txList.get_Tx((int)cbxUnwantedTx2->Items->Objects[cbxUnwantedTx2->ItemIndex]), true);
                if (tx.systemcast == ttAllot)
                    e = txAnalyzer.GetSumE(curSfnUw1, x, y);
                else
                    e = txAnalyzer.GetE(tx, x, y, &it);
                val += String().sprintf("- %.2f %c", e, it);
            } else
                val;

            cmf->sb->Panels->Items[5]->Text = val;
        } catch(...) {}
    } else {
        cmf->sb->Panels->Items[4]->Text = "";
        cmf->sb->Panels->Items[5]->Text = "";
    }

}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::MapMouseUp(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (oldMouseUp)
        oldMouseUp(Sender, Button, Shift, X, Y);
}
//---------------------------------------------------------------------------

inline void AddAction(TMenuItem* mni, TAction* act, int id)
{
    TMenuItem *mi = new TMenuItem(mni);
    if (act)
    {
        mi->Action = act;
        mi->Action = NULL;
    } else
        mi->Caption = "-";
    mi->Tag = id;
    mni->Add(mi);
}

void __fastcall TfrmSelection::CreateTxSubMenu(TMenuItem* mni, MapShape* sp)
{
    int id = GetTxIdxFromName(sp->name, cbxWantedTx->Items);
    if (id > -1)
    {
        AddAction(mni, actSetAsObject, id);
        AddAction(mni, actSetAsInterfere, id);
        actUsedInCalc->Checked = txList.get_TxUseInCalc(id);
        AddAction(mni, actUsedInCalc, id);
        TCOMILISBCTx tx (txList.get_Tx(id), true);
        if (tx.systemcast == ttAM)
        {
            ILisBcLfMfPtr lfmf; tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
            bool isDay = lfmf->is_day;
            actDayNight->Visible = true;
            actDayNight->Caption = String(isDay ? "*" : "") + " День <-> Нiч " + (isDay ? "" : "*");
            AddAction(mni, NULL, id);
            AddAction(mni, actDayNight, id);
            AddAction(mni, actDropEtalonZones, id);
            AddAction(mni, NULL, id);
        }
        AddAction(mni, actEdit, id);
        AddAction(mni, actAnalyze, id);
        AddAction(mni, actShowTestPoints, id);
    } else {
        TMenuItem *mi = new TMenuItem(mni);
        mi->Caption = "Cannot locate this Tx. Sorry.";
        mi->Enabled = true;
        mni->Add(mi);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::SelectTxClick(TObject *Sender)
{
    TMenuItem* mi = dynamic_cast<TMenuItem*>(Sender);
    if (mi)
    {
        selectAnalyzeTx(GetTxIdxFromName(mi->Caption, cbxWantedTx->Items));
        clearPointData();
        clearPoint2Data();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::SelectCpClick(TObject *Sender)
{
    ShowCheckPoint(lastCpLon, lastCpLat);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::mniDelAllAllotZonesClick(TObject *Sender)
{
    if (MessageBox(NULL, "Удалить все контура этого выделения?", "Вопрос, однако",
                        MB_ICONQUESTION | MB_YESNO) == IDYES)
    {
        int analyzeID = (int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex];
        int tx_id = txList.get_TxId(analyzeID);

        dmMain->DelAllotZones(tx_id);
        currentAllotZones[tx_id] = 0;
        btnDelZonesClick(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::DelZones()
{
    /*
        удалить зоны
    */
    int txId = txList.get_TxId((int)cbxWantedTx->Items->Objects[cbxWantedTx->ItemIndex]);
    std::map<int, LPSAFEARRAY>::iterator zi = noiseLimited.find(txId);
    if (zi != noiseLimited.end()) {
        SafeArrayDestroy(zi->second);
        noiseLimited.erase(zi);
    }
    zi = interfereLimited.find(txId);
    if (zi != interfereLimited.end()) {
        SafeArrayDestroy(zi->second);
        interfereLimited.erase(zi);
    }
    zi = interfereLimited2.find(txId);
    if (zi != interfereLimited2.end()) {
        SafeArrayDestroy(zi->second);
        interfereLimited2.erase(zi);
    }
    zi = checkPoints.find(txId);
    if (zi != checkPoints.end()) {
        SafeArrayDestroy(zi->second);
        checkPoints.erase(zi);
    }

    modified[txId] = true;
    /* только отображение зон*/
    showCurTxZones();

    MapShapeMap::iterator mshi = NULL;
    if ((mshi = noiseZones.find(txId)) != noiseZones.end())
    {
        delete mshi->second;
        noiseZones.erase(mshi);
    }
    if ((mshi = interfZones.find(txId)) != interfZones.end())
    {
        delete mshi->second;
        interfZones.erase(mshi);
    }
    if ((mshi = interfZones2.find(txId)) != interfZones2.end())
    {
        delete mshi->second;
        interfZones2.erase(mshi);
    }

    cmf->Clear(checkPointsLayer, true); // заодно и карту перерисуем
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::MapToolUsed(TObject *Sender,
      short ToolNum, double X1, double Y1, double X2, double Y2,
      double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
      VARIANT_BOOL *EnableDefault)
{
    switch (ToolNum) {
        case miGetSectorTool:
            DoSelection(X1, Y1, X2, Y2);
            break;
        default:
            txAnalyzer.MapToolUsed(Sender, ToolNum, X1, Y1, X2, Y2, Distance, Shift, Ctrl, EnableDefault);
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::btUnwantedListClick(TObject *Sender)
{
    if (dgSelectStations == NULL)
        dgSelectStations = new TdgSelectStations(Application);
    dgSelectStations->lbList->Clear();
    for (int i = 0; i < cbxWantedTx->Items->Count; i++)
    {
        TCOMILISBCTx tx(txList.get_Tx((int)cbxWantedTx->Items->Objects[i]));
        if (tx.systemcast < ttAllot)
        {
            dgSelectStations->lbList->Items->AddObject(cbxWantedTx->Items->Strings[i], (TObject*)(ILISBCTx*)tx);

            //check out if Tx is present in selectedUnwanted
            bool isPresent = false;
            int selCnt = selectedUnwanted.Size;
            while (selCnt-- > 0)
                if (isPresent = (tx == selectedUnwanted.get_Tx(selCnt)))
                    break;

            if (isPresent)
                dgSelectStations->lbList->Checked[dgSelectStations->lbList->Items->Count - 1] = true;
        }
    }

    if (dgSelectStations->ShowModal() == mrOk)
    {
        String uwList;
        selectedUnwanted.Clear();
        for (int i = 0; i < dgSelectStations->lbList->Items->Count; i++)
        {
            if (dgSelectStations->lbList->Checked[i])
            {
                selectedUnwanted.AddTx((ILISBCTx*)dgSelectStations->lbList->Items->Objects[i]);
                if (uwList.Length() > 1)
                    uwList += ", ";
                uwList += dgSelectStations->lbList->Items->Strings[i];
            }
        }
        lbUnwantedList->Caption = uwList.IsEmpty() ? "<поточна>" : uwList.c_str();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::cbxWantedTxDrawItem(TWinControl *Control,
      int Index, TRect &Rect, TOwnerDrawState State)
{
    TComboBox* cbx = dynamic_cast<TComboBox*>(Control);
    if (cbx)
    {
        cbx->Canvas->Brush->Style = bsSolid;
        if (State.Contains(odSelected))
            cbx->Canvas->Brush->Color = clHighlight;
        cbx->Canvas->FillRect(Rect);
        if (Index > -1)
        {
            String text = cbx->Items->Strings[Index];
            int spacePos = text.Pos(" ");
            String numText = spacePos ? text.SubString(0, spacePos-1) : text;
            TFont *font = cbx->Canvas->Font;
            font->Style = cbx->Canvas->Font->Style << fsBold;
            if (State.Contains(odSelected))
                font->Color = clHighlightText;
            else if (State.Contains(odDisabled) || State.Contains(odGrayed))
                font->Color = clGrayText;
            else if (cbx == cbxUnwantedTx)
                font->Color = BCCalcParams.lineColorZoneInterfere;
            else if (cbx == cbxUnwantedTx2)
                font->Color = BCCalcParams.lineColorZoneInterfere2;
            else
                font->Color = ccTx;

            cbx->Canvas->TextRect(Rect, Rect.left, Rect.Top, numText);
            int offset = cbx->Canvas->TextWidth(numText);
            cbx->Canvas->Font->Style = cbx->Canvas->Font->Style >> fsBold;
            if (State.Contains(odSelected))
                font->Color = clHighlightText;
            else if (State.Contains(odDisabled) || State.Contains(odGrayed))
                font->Color = clGrayText;
            else
                cbx->Canvas->Font->Color = clWindowText;
            Rect.left += offset;
            cbx->Canvas->TextRect(Rect, Rect.left, Rect.Top, text.SubString(spacePos, text.Length()-spacePos+1));
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::GetSfn(ILISBCTxList* isfn, int allotPos)
{
    // трактуем выделения как сети, состоящие из привязанных к выделениям передатчиков
    TCOMILISBCTxList sfn(isfn, true);
    sfn.Clear();
    if (allotPos < 0)
        return;
    TCOMILISBCTx tx(txList.get_Tx(allotPos), true);
    if (tx.systemcast < ttAllot)
        return;
    sfn.AddTx(tx);
    sfn.set_TxUseInCalc(0, txList.get_TxUseInCalc(allotPos));
    if (allotPos < tags.size() && allotPos > 0)
    {
        String tag = tags[allotPos-1];
        // just in case when 1 item is allot and the 0 is it's tx, take previous item into account
        if (allotPos == 1 && tags[0] == "0")
        {
            int idx = sfn.AddTx(txList.get_Tx(0));
            sfn.set_TxUseInCalc(idx, txList.get_TxUseInCalc(0));
        }
        for (int i = allotPos + 1; i < txList.Size && i-1 < tags.size() && tags[i-1].Pos(tag) == 1; i++)
        {
            int idx = sfn.AddTx(txList.get_Tx(i));
            sfn.set_TxUseInCalc(idx, txList.get_TxUseInCalc(i));
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSetAsInterfereExecute(TObject *Sender)
{
    int idx = reinterpret_cast<TComponent*>(Sender)->Tag;
    checkIndex(idx);
    idx = cbxUnwantedTx->Items->IndexOfObject((TObject*)idx);
    if (idx > -1)
    {
        cbxUnwantedTx->ItemIndex = idx;
        cbxUnwantedTxChange(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actSetAsObjectExecute(TObject *Sender)
{
    int idx = reinterpret_cast<TComponent*>(Sender)->Tag;
    checkIndex(idx);
    idx = cbxWantedTx->Items->IndexOfObject((TObject*)idx);
    if (idx > -1)
    {
        cbxWantedTx->ItemIndex = idx;
        cbxWantedTxChange(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::btProjectionClick(TObject *Sender)
{
    TMxCoordSys* cs = new TMxCoordSys(this);
    cs->ConnectTo(cmf->bmf->Map->DisplayCoordSys);
    cs->PickCoordSys();
    #ifdef _DEBUG
    String s;
    s += ("Azm = " + FloatToStr(cs->get_Azimuth()) + '\n');
    s += ("Type = " + IntToStr(cs->get_Type()) + '\n');
    TMxDatum* dt = new TMxDatum(this);
    dt->ConnectTo(cs->get_Datum());
    //s += ("Datum.PrimeMeridian = " + FloatToStr(dt->get_PrimeMeridian()) + '\n');
    s += ("OriginLatitude = " + FloatToStr(cs->get_OriginLatitude()) + '\n');
    s += ("OriginLongitude = " + FloatToStr(cs->get_OriginLongitude()) + '\n');
    delete dt;
    delete cs;
  //  ShowMessage(s);
    #endif
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::dpTimeChange(TObject *Sender)
{
    SetTime();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::tpTimeChange(TObject *Sender)
{
    SetTime();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::SetTime()
{
    txAnalyzer.time0 = floor(dpTime->Date) + (tpTime->Time - floor(tpTime->Time));
}
void __fastcall TfrmSelection::FormResize(TObject *Sender)
{
    pnTime->Left = ClientWidth - pnTime->Width;    
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actDayNightExecute(TObject *Sender)
{
    bool fromMap = pcSelection->ActivePage == tshMap && cmf->Visible;
    int idx = fromMap ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    if (txList.IsBound() && idx > -1 && idx < txList.Size)
    {
        TCOMILISBCTx tx(txList.get_Tx(idx), true);
        if (tx.systemcast == ttAM)
        {
            ILisBcLfMfPtr lfmf;
            tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
            if (lfmf.IsBound())
            {
                bool isDay = lfmf->is_day;
                if (isDay && lfmf->night_op || !isDay && lfmf->day_op)
                {
                    lfmf->set_is_day(!isDay);

                    int txid = tx.id;
                    SafeArrayDestroy(coverage[txid]);
                    coverage[txid] = NULL;
                    MapShapeMap::iterator msi = coverZones.find(txid);
                    if (msi != coverZones.end()) {
                        delete msi->second;
                        coverZones.erase(msi);
                        if (fromMap)
                        {
                            LPSAFEARRAY arr = txAnalyzer.GetEtalonZone(tx, lfmf->is_day);
                            coverage[txid] = arr;

                            std::vector<double> zone;
                            for (int i = 0; i < arr->rgsabound[0].cElements; i++)
                                zone.push_back(((double*)arr->pvData)[i]);
                            MapPolygon* pgn = cmf->ShowCoverageZone(tx.longitude, tx.latitude, zone);
                            coverZones[tx.id] = pgn;
                            pgn->width = BCCalcParams.lineThicknessZoneCover;
                            pgn->color = ccZoneCover;
                            if (!txList.get_TxUseInCalc(idx))
                                pgn->color = ccZoneCoverNotUsed, pgn->width = 1;
                            else if (lfmf->is_day)
                                pgn->color = ccIsDay;

                            cmf->bmf->Map->Refresh();
                        }
                    }
                }
            }
        }
        refresh_row(idx);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actDayNightUpdate(TObject *Sender)
{
    bool fromMap = pcSelection->ActivePage == tshMap && cmf->Visible;
    if (fromMap)
        return;
    int idx = fromMap ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    if (txList.IsBound() && idx > -1 && idx < txList.Size)
    {
        TCOMILISBCTx tx (txList.get_Tx(idx), true);
        if (tx.systemcast == ttAM)
        {
            actDayNight->Visible = true;
            ILisBcLfMfPtr lfmf;
            tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
            if (lfmf.IsBound())
            {
                bool isDay = lfmf->is_day;
                actDayNight->Caption = String(isDay ? "*" : "") + " День <-> Нiч " + (isDay ? "" : "*");
            } else {
                actDayNight->Caption = String("???") + " День <-> Нiч " + ("???");
            }
        } else {
            actDayNight->Visible = false;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::actDropEtalonZonesExecute(TObject *Sender)
{
    if (MessageBox(NULL, "Удалить эталонные зоны?", "Подтверждение", MB_ICONQUESTION | MB_YESNO) == IDNO)
        return;
    bool fromMap = pcSelection->ActivePage == tshMap && cmf->Visible;
    int idx = fromMap ? reinterpret_cast<TComponent*>(Sender)->Tag : currentTxIndex;
    if (txList.IsBound() && idx > -1 && idx < txList.Size)
    {
        TCOMILISBCTx tx (txList.get_Tx(idx), true);
        if (tx.systemcast == ttAM)
        {
            ILisBcLfMfPtr lfmf;
            tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
            if (lfmf.IsBound())
            {
                txAnalyzer.DropEtalonZone(tx, lfmf->is_day);
                txAnalyzer.DropEtalonZone(tx, !lfmf->is_day);

                int txid = tx.id;
                SafeArrayDestroy(coverage[txid]);
                coverage[txid] = NULL;
                MapShapeMap::iterator msi = coverZones.find(txid);
                if (msi != coverZones.end()) {
                    delete msi->second;
                    coverZones.erase(msi);
                    if (fromMap)
                    {
                        LPSAFEARRAY arr = txAnalyzer.GetEtalonZone(tx, lfmf->is_day);
                        coverage[txid] = arr;

                        std::vector<double> zone;
                        for (int i = 0; i < arr->rgsabound[0].cElements; i++)
                            zone.push_back(((double*)arr->pvData)[i]);
                        MapPolygon* pgn = cmf->ShowCoverageZone(tx.longitude, tx.latitude, zone);
                        coverZones[tx.id] = pgn;
                        pgn->width = BCCalcParams.lineThicknessZoneCover;
                        pgn->color = ccZoneCover;
                        if (!txList.get_TxUseInCalc(idx))
                            pgn->color = ccZoneCoverNotUsed, pgn->width = 1;
                        else if (lfmf->is_day)
                            pgn->color = ccIsDay;

                        cmf->bmf->Map->Refresh();
                    }
                }
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::btNowClick(TObject *Sender)
{
    tpTime->Time = Time();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::btNoonClick(TObject *Sender)
{
    tpTime->Time = 13./24.;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::btMidnightClick(TObject *Sender)
{
    tpTime->Time = 1./24.;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::btOpModeClick(TObject *Sender)
{
    double time = tpTime->Time - floor(tpTime->Time);
    bool isDay = time > 7./24. && time < 19./24.;
    for (int i = 0; i < txList.Size; i++)
    {
        TCOMILISBCTx tx(txList.get_Tx(i), true);
        if (tx.systemcast == ttAM)
        {
            ILisBcLfMfPtr lfmf; tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
            if (isDay && lfmf->day_op || !isDay && lfmf->night_op)
                lfmf->set_is_day(isDay); 
            else
                txList.set_TxUseInCalc(i, false);
        }
    }
    if (pcSelection->ActivePage == tshSelection)
        refresh();
    else if (pcSelection->ActivePage == tshMap)
        ActivateMapSheet();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelection::CalcAllotRes(long txId_, TRSAGeoPoint chPoint, std::vector<double> *params, TControlPointCalcResult* res)
{
    TControlPointCalcResult res_;
    IntToStrMap::iterator ai = allots.find(txId_);
    if (ai != allots.end())
    {
        StrToIntMap::iterator aci = curAllot.find(ai->second);
        if (aci != curAllot.end())
        {
            int allotId = aci->second;
            TCOMILISBCTx txAllot(txBroker.GetTx(allotId, CLSID_LisBcDigAllot), true);
            TRSAGeoPoint txPoint;
            txPoint.H = txAllot.latitude;
            txPoint.L = txAllot.longitude;
            double azimuth = 0, distance = 0;
            sphereCalc->Azimuth(txPoint, chPoint, &azimuth);
            sphereCalc->Distance(txPoint, chPoint, &distance);
            res_.azimuth = azimuth;
            res_.distance = distance;

            for(int i = 0; i < 4; ++i)
            {
                double Summ = 0;
                for(int j = 0; j < params[i].size(); ++j)
                    Summ += pow(10, params[i][j]/10.);
                Summ = 10*Log10(Summ);
                switch(i)
                {
                    case 0: res_.e_50 = Summ; break;
                    case 1: res_.e_10 = Summ; break;
                    case 2: res_.e_1 = Summ; break;
                    case 3: res_.e_usable = Summ; break;
                }
            }
        }
    }
    *res = res_;
}
//---------------------------------------------------------------------------



