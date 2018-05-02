//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uPlanning.h"
#include "uAnalyzer.h"
#include "uMainDm.h"
#include "uLayoutMngr.h"
#include "FormProvider.h"
#include <set>
#include "uExplorer.h"
#include "IBQuery.hpp"
#include "TxBroker.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmPlanning *frmPlanning;
//---------------------------------------------------------------------------
char *commonColNames[] = {"К/Ч/Б", "Кількість" };
int commonColWidths[]  = {35,      50};
char *interfColNames[] = {"Завада", "№ рег", "№ прд", "Населений пункт", "ЕВП max", "К/Ч/Б", "Стан", "Відстань", "Азимут"};
int interfColWidths[]  = {50,       35,      35,      120,               50,        50,      40,      50,        50};
int interfArrayLen     = 9;

__fastcall TfrmPlanning::TfrmPlanning(TComponent* Owner)
    : TForm(Owner)
{
    pcPlanning->ActivePage = tshPlan;
    ActiveControl = grdPlan; 

    LayoutManager.loadLayout(this);
    LayoutManager.EnsureShortcut(this);

    //  заголовки колонок
    int colIdx = 0;
    grdPlan->ColCount = 2 + interfArrayLen * 2;

    grdPlan->ColWidths[colIdx] = commonColWidths[0];
    grdPlan->Cells[colIdx++][0]= commonColNames[0];
    grdPlan->ColWidths[colIdx] = commonColWidths[1];
    grdPlan->Cells[colIdx++][0]= commonColNames[1];

    grdUnwantedSort->ColCount = 1 + interfArrayLen;
    grdWantedSort->ColCount = 1 + interfArrayLen;

    grdUnwantedSort->ColWidths[0] = 35;
    grdWantedSort->ColWidths[0] = 35;
    grdUnwantedSort->Cells[0][0] = "№";
    grdWantedSort->Cells[0][0] = "№";

    for (int nameIdx = 0; nameIdx < interfArrayLen; nameIdx++) {
        grdPlan->ColWidths[colIdx + nameIdx] = interfColWidths[nameIdx];
        grdPlan->ColWidths[colIdx + nameIdx + interfArrayLen] = interfColWidths[nameIdx];

        if (nameIdx == 0) {
            grdPlan->Cells[colIdx + nameIdx][0] = "Нам";
            grdPlan->Cells[colIdx + nameIdx + interfArrayLen][0] = "Від нас";
        } else {
            grdPlan->Cells[colIdx + nameIdx][0] = interfColNames[nameIdx];
            grdPlan->Cells[colIdx + nameIdx + interfArrayLen][0] = interfColNames[nameIdx];
        }

        grdUnwantedSort->Cells[nameIdx + 1][0] = interfColNames[nameIdx];
        grdUnwantedSort->ColWidths[nameIdx + 1] = interfColWidths[nameIdx];
        grdWantedSort->Cells[nameIdx + 1][0] = interfColNames[nameIdx];
        grdWantedSort->ColWidths[nameIdx + 1] = interfColWidths[nameIdx];
    }

    //  по тэгам отслеживается, какой из списков отображён сейчас в сетке
    grdUnwantedSort->Tag = -1;
    grdWantedSort->Tag = -1;

    //lblName->Font->Style = lblName->Font->Style << fsBold;
    lblChannel->Font->Style = lblChannel->Font->Style << fsBold;
    lblChannel->Caption = "";
}
//---------------------------------------------------------------------------
void __fastcall TfrmPlanning::DrawPlan()
{
    //  подготовить список Ид опор и извлечь опоры
    //  можно было сразу записывать в строку, однако вероятны повторы,
    //  поэтому сначала используем класс std::set<long>
    std::set<long> standIdSet;
    standIdSet.insert(txAnalyzer.planningTx.stand_id);
    TxAnalyzer::PlanVector::iterator pvi = txAnalyzer.planVector.begin();
    while (pvi < txAnalyzer.planVector.end()) {
        //  нулевой уже вставлен, начнём со второго
        TCOMILISBCTxList txList(pvi->txList, true);
        for (int i = 1; i < txList.Size; i++) {
            TCOMILISBCTx tx(txList.get_Tx(i), true);
            standIdSet.insert(tx.stand_id);
        }
        pvi++;
    }


    dmMain->cacheSites(standIdSet, standRecords, true);

    grdPlan->RowCount = 1;

    AnsiString siteName;
    AnsiString regNo;
    std::map<int, StandRecord>::iterator sri = standRecords.find(txAnalyzer.planningTx.stand_id);
    if (sri != standRecords.end())
        siteName = sri->second.siteName.c_str();
    else
        siteName = txAnalyzer.planningTx.station_name;

    Caption = AnsiString("Планування ") + dmMain->GetSystemCastName(txAnalyzer.planningTx.systemcast) +": "+ siteName
//    lblName->Caption = AnsiString("Планування ") + siteName
                        + " ( " + dmMain->coordToStr(txAnalyzer.planningTx.latitude, 'Y')
                        + " : " + dmMain->coordToStr(txAnalyzer.planningTx.longitude, 'X')
                        + " )";

    grdPlan->Cells[0][0] = "";
    switch (txAnalyzer.planningTx.systemcast) {
        case ttTV:
        case ttDVB:
            grdPlan->Cells[0][0] = "К";
            break;
        case ttDAB:
            grdPlan->Cells[0][0] = "Б";
            break;
        default:
            grdPlan->Cells[0][0] = "Ч";
            break;
    }

    int vectorSize = txAnalyzer.planVector.size();
    grdPlan->RowCount = vectorSize + 1;
    if (vectorSize > 0)
        grdPlan->FixedRows = 1;

    pvi = txAnalyzer.planVector.begin();
    int i = 1;
    while (pvi < txAnalyzer.planVector.end()) {
        grdPlan->Cells[0][i] = pvi->name.c_str();
        TCOMILISBCTxList txList(pvi->txList, true);
        grdPlan->Cells[1][i] = txList.Size - 1;

        for (int offset = 0; offset <= interfArrayLen; offset += interfArrayLen) {

            int idx;
            double interference;

            if (offset == 0) {
                idx = pvi->maxUnwantIdx;
                interference = idx < txList.Size ? txList.get_TxUnwantInterfere(idx) : -999.0;
            } else {
                idx = pvi->maxWantIdx;
                interference = idx < txList.Size ? txList.get_TxWantInterfere(idx) : -999.0;
            }

            if (idx > 0 && idx < txList.Size) {
                TCOMILISBCTx tx(txList.get_Tx(idx), true);

                std::map<int, StandRecord>::iterator sri = standRecords.find(tx.stand_id);
                if (sri != standRecords.end())
                {
                    siteName = sri->second.siteName.c_str();
                    regNo = sri->second.regionNum.c_str();
                } else {
                    siteName = tx.station_name;
                    regNo = "";
                }

                grdPlan->Cells[offset + 2][i] = FormatFloat("0.0", interference);
                grdPlan->Cells[offset + 3][i] = regNo;
                grdPlan->Cells[offset + 4][i] = AnsiString(tx.adminid);
                grdPlan->Cells[offset + 5][i] = siteName;

                switch(tx.systemcast) {
                    case ttTV: case ttDVB:
                        grdPlan->Cells[offset + 6][i] = FormatFloat("0.0", tx.epr_video_max);
                        break;
                    case ttAM: case ttFM: case ttDAB:
                        grdPlan->Cells[offset + 6][i] = FormatFloat("0.0", tx.epr_sound_max_primary);
                        break;
                    default:
                        grdPlan->Cells[offset + 6][i] = "-?-";
                        break;
                }

                AnsiString freq;
                switch(tx.systemcast) {
                    case ttTV: case ttDVB:
                        freq = dmMain->getChannelName(tx.channel_id);
                        break;
                    case ttAM: case ttFM:
                        freq = FormatFloat("0.###", tx.sound_carrier_primary);
                        break;
                    case ttDAB:
                        freq = dmMain->getDabBlockName(tx.blockcentrefreq);
                        break;
                    case ttCTV:
                        freq = " -- ";
                        break;
                    default:
                        break;
                }
                grdPlan->Cells[offset + 7][i] = freq;
                grdPlan->Cells[offset + 8][i] = dmMain->getConditionName(tx.acin_id) + " / " + dmMain->getConditionName(tx.acout_id);
                grdPlan->Cells[offset + 9][i] = FormatFloat("0.0", txList.get_TxDistance(idx));
                grdPlan->Cells[offset + 10][i] = FormatFloat("0", txList.get_TxAzimuth(idx));

            } else {
                for (int j = 2; j <= 10; j++)
                    grdPlan->Cells[offset + j][i] = "-/-";
            }

        }
        pvi++;
        i++;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmPlanning::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;
    LayoutManager.saveLayout(this);
    LayoutManager.DeleteShortcut(this);

    Screen->Cursor = crHourGlass;
    try {
        if ( txAnalyzer.wasChanges )
            txAnalyzer.SaveToDb();
    } __finally {
        Screen->Cursor = crDefault;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::grdPlanDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;

    if (ACol == 2 || ACol == 2 + interfArrayLen)
        //  колонка помехи
        sg->Canvas->Font->Style = sg->Canvas->Font->Style << fsBold;
    else
        sg->Canvas->Font->Style = sg->Canvas->Font->Style >> fsBold;

    AnsiString text = grdPlan->Cells[ACol][ARow];
    int xOffset = 1;

    if (ACol == 1 ||
        ACol == 2 + 0 ||
        ACol == 2 + 0 + interfArrayLen ||
        ACol == 2 + 4 ||
        ACol == 2 + 4 + interfArrayLen ||
        ACol == 2 + 7 ||
        ACol == 2 + 7 + interfArrayLen ||
        ACol == 2 + 8 ||
        ACol == 2 + 8 + interfArrayLen)
    {
        //  right align
        xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2 ;
    }
    if (ACol == 0 ||
        ACol == 2 + 5 ||
        ACol == 2 + 5 + interfArrayLen ||
        ACol == 2 + 6 ||
        ACol == 2 + 6 + interfArrayLen ||
        ARow == 0)
    {
        //  center align
        xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
    }
    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::grdSortDrawCell(TObject *Sender,
      int ACol, int ARow, TRect &Rect, TGridDrawState State)
{
    TStringGrid *sg = dynamic_cast<TStringGrid*>(Sender);
    if (!sg)
        return;

    if (ACol == 1)
        //  колонка помехи
        sg->Canvas->Font->Style = sg->Canvas->Font->Style << fsBold;
    else
        sg->Canvas->Font->Style = sg->Canvas->Font->Style >> fsBold;

    AnsiString text = sg->Cells[ACol][ARow];
    int xOffset = 1;

    if (ACol == 0 ||
        ACol == 1 ||
        ACol == 5 ||
        ACol == 8 ||
        ACol == 9)
    {
        //  right align
        xOffset = Rect.Right - Rect.left - sg->Canvas->TextWidth(text) - 2 ;
    }
    if (ACol == 6 ||
        ACol == 7 ||
        ARow == 0)
    {
        //  center align
        xOffset = (Rect.Right - Rect.left - sg->Canvas->TextWidth(text)) / 2;
    }
    sg->Canvas->TextRect(Rect, Rect.left + xOffset, Rect.Top + 1, text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::pcPlanningChange(TObject *Sender)
{
    TStringGrid* sgr = NULL;
    if (pcPlanning->ActivePage == tshListWantSort) {
        sgr = grdWantedSort;
    } else if (pcPlanning->ActivePage == tshListUnwantSort) {
        sgr = grdUnwantedSort;
    } else {
        lblChannel->Visible = false;
        return;
    }

    lblChannel->Visible = true;
    lblChannel->Caption = AnsiString(" ") + grdPlan->Cells[0][grdPlan->Selection.Top] + " ";
    switch (txAnalyzer.planningTx.systemcast) {
        case ttTV: case ttDVB:
            lblChannel->Caption = lblChannel->Caption + "ТВК";
            break;
        case ttAM: case ttFM:
            lblChannel->Caption = lblChannel->Caption + "МГц";
            break;
    }

    if (sgr->Tag != grdPlan->Selection.Top) {
        TxAnalyzer::PlanVector::iterator pvi = txAnalyzer.planVector.begin() + (grdPlan->Selection.Top - 1);
        //  помехи выведем в отдельный вектор и его затем отсортируем
        TCOMILISBCTxList txList(pvi->txList, true);
        if (pvi < txAnalyzer.planVector.end() && txList.IsBound()) {
            VoltageVector voltageVector;
            for (int i = 1; i < txList.Size; i++) {
                voltageVector.push_back(VoltagePair(i, sgr == grdWantedSort ?
                                                    txList.get_TxWantInterfere(i) :
                                                    txList.get_TxUnwantInterfere(i)
                                                    ));
            }
            if (voltageVector.size() > 0)
                SortVoltageVector(voltageVector, 0, voltageVector.size() - 1);
            sgr->RowCount = voltageVector.size() + 1;
            if (sgr->RowCount > 1)
                sgr->FixedRows = 1;
            VoltageVector::iterator vvi = voltageVector.begin();
            for (int i = 0; i < voltageVector.size(); i++)
                sgr->Objects[0][i + 1] = (TObject *)(vvi++)->first;

            DrawSortedList(sgr, grdPlan->Selection.Top - 1);

            sgr->Tag = grdPlan->Selection.Top;
        } else {
            sgr->RowCount = 1;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::DrawSortedList(TStringGrid *sgr, int vectorIdx)
{
    TxAnalyzer::PlanVector::reference pvr = txAnalyzer.planVector[vectorIdx];
    TCOMILISBCTxList txList(pvr.txList, true);
    sgr->RowCount = txList.Size;
    for (int row = 1; row < sgr->RowCount; row++) {

        int idx = (int)sgr->Objects[0][row];

        TCOMILISBCTx tx(txList.get_Tx(idx), true);

        sgr->Cells[0][row] = row;
        sgr->Cells[1][row] = FormatFloat("0.0", sgr == grdWantedSort ?
                                                txList.get_TxWantInterfere(idx) :
                                                txList.get_TxUnwantInterfere(idx)
                                         );
        sgr->Cells[3][row] = AnsiString(tx.adminid);

        std::map<int, StandRecord>::iterator sri = standRecords.find(tx.stand_id);
        if (sri != standRecords.end())
        {
            sgr->Cells[4][row] = sri->second.siteName.c_str();
            sgr->Cells[2][row] = sri->second.regionNum.c_str();
        } else {
            sgr->Cells[4][row] = tx.station_name;
            sgr->Cells[2][row] = "";
        }

        switch(tx.systemcast) {
            case ttTV: case ttDVB:
                sgr->Cells[5][row] = FormatFloat("0.0", tx.epr_video_max);
                break;
            case ttAM: case ttFM: case ttDAB:
                sgr->Cells[5][row] = FormatFloat("0.0", tx.epr_sound_max_primary);
                break;
            default:
                sgr->Cells[5][row] = "-?-";
                break;
        }


        AnsiString freq;
        switch(tx.systemcast) {
            case ttTV: case ttDVB:
                freq = dmMain->getChannelName(tx.channel_id);
                break;
            case ttAM: case ttFM:
                freq = FormatFloat("0.###", tx.sound_carrier_primary);
                break;
            case ttDAB:
                freq = dmMain->getDabBlockName(tx.blockcentrefreq);
                break;
            case ttCTV:
                freq = " -- ";
                break;
            default:
                break;
        }
        sgr->Cells[6][row] = freq;
        sgr->Cells[7][row] = dmMain->getConditionName(tx.acin_id) + " / " + dmMain->getConditionName(tx.acout_id);
        sgr->Cells[8][row] = FormatFloat("0.0", txList.get_TxDistance(idx));
        sgr->Cells[9][row] = FormatFloat("0", txList.get_TxAzimuth(idx));
    }
}

void __fastcall TfrmPlanning::actCh2TxExecute(TObject *Sender)
{
    if (Application->MessageBox("Підставити канал/частоту/блок в плануємий передавач?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES) {
        doMakeSelection(txAnalyzer.planningTx.id);
        txAnalyzer.isNewPlan = true;
    }
}

void __fastcall TfrmPlanning::actCh2CopyExecute(TObject *Sender)
{
    int newId = copyTx(txAnalyzer.planningTx.id);
    doMakeSelection(newId);
    txAnalyzer.isNewPlan = true;
}
//---------------------------------------------------------------------------


int __fastcall TfrmPlanning::copyTx(int id)
{
    std::auto_ptr<TIBQuery> selectQuery(new TIBQuery(Application));
    std::auto_ptr<TIBQuery> insertQuery(new TIBQuery(Application));

    selectQuery->Database = dmMain->dbMain;
    insertQuery->Database = dmMain->dbMain;

    selectQuery->SQL->Text = "select * from TRANSMITTERS where ID = " + IntToStr(id);
    selectQuery->Open();

    AnsiString asFieldList = selectQuery->Fields->Fields[0]->FieldName;
    AnsiString asParamList = ":" + selectQuery->Fields->Fields[0]->FieldName;

    for (int i = 1; i < selectQuery->Fields->Count; i++) {
        asFieldList = asFieldList + ", " + selectQuery->Fields->Fields[i]->FieldName;
        asParamList = asParamList + ", :" + selectQuery->Fields->Fields[i]->FieldName;
    }

    insertQuery->SQL->Text = "insert into TRANSMITTERS (" + asFieldList + ") values (" + asParamList +")";

    int newId = dmMain->getNewId();

    AnsiString asFieldName;
    for (int i = 0; i < selectQuery->Fields->Count; i++) {
        asFieldName = selectQuery->Fields->Fields[i]->FieldName;
        if (asFieldName == "ID")
            insertQuery->ParamByName(asFieldName)->AsInteger = newId;
        else if (asFieldName == "ORIGINALID")
            insertQuery->ParamByName(asFieldName)->AsInteger = id;
        else if (asFieldName == "STATUS")
            insertQuery->ParamByName(asFieldName)->AsInteger = 1;
        else if (asFieldName == "WAS_IN_BASE")
            insertQuery->ParamByName(asFieldName)->AsInteger = 0;
        else if (!selectQuery->Fields->Fields[i]->IsNull) {
            if (selectQuery->Fields->Fields[i]->DataType == ftBlob) {
                TStream* blobStreamR = selectQuery->CreateBlobStream(selectQuery->Fields->Fields[i], bmRead);
                insertQuery->ParamByName(asFieldName)->LoadFromStream(blobStreamR, ftBlob);
            } else {
                insertQuery->ParamByName(asFieldName)->Value = selectQuery->Fields->Fields[i]->Value;
            }
        }
    }

    insertQuery->ExecSQL();
    insertQuery->Transaction->CommitRetaining();

    return newId;
}

void __fastcall TfrmPlanning::doMakeSelection(int txId)
{
    int idx = grdPlan->Selection.Top - 1;
    if (idx < 0)
        throw *(new Exception("Не вибраний канал/частота"));

    TCOMILISBCTx tx(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))), true);

    switch (tx.systemcast) {
        case ttTV:
        case ttDVB:
            tx.set_video_carrier(txAnalyzer.planVector[idx].frequency);
            tx.set_channel_id(txAnalyzer.planVector[idx].channelId);
            break;
        case ttAM: case ttFM:
            tx.set_video_carrier(txAnalyzer.planVector[idx].frequency);
            tx.set_sound_carrier_primary(txAnalyzer.planVector[idx].frequency);
            break;
        case ttDAB:
            tx.set_blockcentrefreq(txAnalyzer.planVector[idx].frequency);
            break;
        default:
            break;
    }

    tx.save();

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
        expl->actNewSelectionExecute(this);
    } else {
        //  нет в списке, нужно добавить
        SendMessage(expl->Handle, WM_LIST_ELEMENT_SELECTED, 39, tx.id);

        tn = expl->trvExplorer->Items->GetFirstNode();
        while (tn) {
            if (tn->Data == (void*)tx.id) {
                expl->trvExplorer->Selected = tn;
                expl->actNewSelectionExecute(this);
                break;
            }
            tn = tn->getNextSibling();
        }
    }


}
//---------------------------------------------------------------------------


void __fastcall TfrmPlanning::actSaveExecute(TObject *Sender)
{
    if ( txAnalyzer.wasChanges )
        if (Application->MessageBox("Зберігти результати планування в базі?", Application->Title.c_str(), MB_YESNO | MB_ICONQUESTION) == IDYES) {
            Screen->Cursor = crHourGlass;
            try {
                txAnalyzer.SaveToDb();
            } __finally {
                Screen->Cursor = crDefault;
            }
        }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::actReplanExecute(TObject *Sender)
{
    if (Application->MessageBox("Перепланувати передавач?", Application->Title.c_str(), MB_YESNO | MB_ICONQUESTION) == IDYES) {
        txAnalyzer.PerformPlanning(txAnalyzer.planningTx.id);
        DrawPlan();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::actAnalyzeExecute(TObject *Sender)
{
    if (Application->MessageBox("Перерахувати дуельні завади плана?", Application->Title.c_str(), MB_YESNO | MB_ICONQUESTION) == IDYES) {
        txAnalyzer.DoAnalysis();
        DrawPlan();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::FormShow(TObject *Sender)
{
    DrawPlan();
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::grdPlanMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (Shift == TShiftState() << ssLeft << ssDouble) {
        CheckCurrentList();
        txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, txAnalyzer.planVector[grdPlan->Selection.Top - 1].maxUnwantIdx);
    } else if (Shift == TShiftState() << ssCtrl << ssLeft << ssDouble) {
        CheckCurrentList();
        txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, txAnalyzer.planVector[grdPlan->Selection.Top - 1].maxWantIdx);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::grdPlanKeyDown(TObject *Sender, WORD &Key, TShiftState Shift)
{
    if (Key == 13) {
        if (Shift == TShiftState()) {
            CheckCurrentList();
            txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, txAnalyzer.planVector[grdPlan->Selection.Top - 1].maxUnwantIdx);
        } else if (Shift == TShiftState() << ssCtrl) {
            CheckCurrentList();
            txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, txAnalyzer.planVector[grdPlan->Selection.Top - 1].maxWantIdx);
        }
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmPlanning::grdSortMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (Shift == TShiftState() << ssLeft << ssDouble) {
        CheckCurrentList();
        TStringGrid* grd = dynamic_cast<TStringGrid*>(Sender);
        TCOMILISBCTxList txList(txAnalyzer.planVector[grdPlan->Selection.Top - 1].txList, true);
        if (grd == NULL
        || grd->Selection.Top == 0
        || (int)grd->Objects[0][grd->Selection.Top] == 0
        || (int)grd->Objects[0][grd->Selection.Top] > txList.Size
        ) {
            MessageBeep(0xFFFFFFFF);
            return;
        }
        txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, (int)grd->Objects[0][grd->Selection.Top]);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::grdSortKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if (Key == 13 && Shift == TShiftState()) {
        CheckCurrentList();
        TStringGrid* grd = dynamic_cast<TStringGrid*>(Sender);
        TCOMILISBCTxList txList(txAnalyzer.planVector[grdPlan->Selection.Top - 1].txList, true);
        if (grd == NULL
        || grd->Selection.Top == 0
        || (int)grd->Objects[0][grd->Selection.Top] == 0
        || (int)grd->Objects[0][grd->Selection.Top] > txList.Size
        ) {
            MessageBeep(0xFFFFFFFF);
            return;
        }
        txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, (int)grd->Objects[0][grd->Selection.Top]);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmPlanning::actShowUnwantedTxExecute(TObject *Sender)
{
    long txId = 0;
    int row = 0;

    if ( pcPlanning->ActivePage == tshPlan )
    {
        row = grdPlan->Row - 1;
        TCOMILISBCTxList txList(txAnalyzer.planVector[row].txList, true);
        txId = txList.get_TxId(txAnalyzer.planVector[row].maxUnwantIdx);
    }
    else if ( pcPlanning->ActivePage == tshListUnwantSort )
    {
        TCOMILISBCTxList txList(txAnalyzer.planVector[0].txList, true);
        txId = txList.get_TxId((int)grdUnwantedSort->Objects[0][grdUnwantedSort->Row]);
    }

    FormProvider.ShowTx(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))));
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::actShowWantedTxExecute(TObject *Sender)
{
    long txId = 0;
    int row = 0;

    if ( pcPlanning->ActivePage == tshPlan )
    {
        row = grdPlan->Row - 1;
        TCOMILISBCTxList txList(txAnalyzer.planVector[row].txList, true);
        txId = txList.get_TxId(txAnalyzer.planVector[row].maxWantIdx);
    }
    else if ( pcPlanning->ActivePage == tshListWantSort )
    {
        TCOMILISBCTxList txList(txAnalyzer.planVector[0].txList, true);
        txId = txList.get_TxId((int)grdWantedSort->Objects[0][grdWantedSort->Row]);
    }

    FormProvider.ShowTx(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))));
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::N7Click(TObject *Sender)
{
    TStringGrid* grd = NULL;

    if ( pcPlanning->ActivePage == tshListUnwantSort )
        grd = grdUnwantedSort;                                               
    else if ( pcPlanning->ActivePage == tshListWantSort )
        grd = grdWantedSort;

    CheckCurrentList();
    TCOMILISBCTxList txList(txAnalyzer.planVector[grdPlan->Selection.Top - 1].txList, true);
    if (grd == NULL
        || grd->Selection.Top == 0
        || (int)grd->Objects[0][grd->Selection.Top] == 0
        || (int)grd->Objects[0][grd->Selection.Top] > txList.Size
       )
    {
        MessageBeep(0xFFFFFFFF);
        return;
    }

    txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, (int)grd->Objects[0][grd->Selection.Top]);
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::N8Click(TObject *Sender)
{
    CheckCurrentList();
    
    txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, txAnalyzer.planVector[grdPlan->Selection.Top - 1].maxUnwantIdx);
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::N9Click(TObject *Sender)
{
    CheckCurrentList();

    txAnalyzer.CalcDuelPlan(grdPlan->Selection.Top - 1, txAnalyzer.planVector[grdPlan->Selection.Top - 1].maxWantIdx);
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::tshPlanEnter(TObject *Sender)
{
    actShowUnwantedTx->Enabled = true;
    actShowWantedTx->Enabled = true;

    N7->Visible = false;
    N8->Visible = true;
    N9->Visible = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::tshListUnwantSortEnter(TObject *Sender)
{
    actShowUnwantedTx->Enabled = true;
    actShowWantedTx->Enabled = false;

    N7->Visible = true;
    N8->Visible = false;
    N9->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::tshListWantSortEnter(TObject *Sender)
{
    actShowUnwantedTx->Enabled = false;
    actShowWantedTx->Enabled = true;

    N7->Visible = true;
    N8->Visible = false;
    N9->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmPlanning::CheckCurrentList()
{
    long size = 0;
    if (grdPlan->Selection.Top > 0)
        txAnalyzer.planVector[grdPlan->Selection.Top - 1].txList->get_Size(&size);
    if (grdPlan->Selection.Top == 0
    || grdPlan->Selection.Top > txAnalyzer.planVector.size()
    || size <= 1 ) {
        throw *(new Exception("Список пуст"));
        //MessageBeep(0xFFFFFFFF);
        //return;
    }
}
