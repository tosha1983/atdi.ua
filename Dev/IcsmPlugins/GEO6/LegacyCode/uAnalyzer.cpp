//---------------------------------------------------------------------------
#include <math>
#include <memory>
#include <values.h>
#pragma hdrstop

#include "FormProvider.h"
#include "uAnalyzer.h"
#include "uDuelResult.h"
#include "uNewSelection.h"
#include "uMainDm.h"
#include "uMainForm.h"
#include "uParams.h"
#include "uSelection.h"
#include "TxBroker.h"
#include "tempvalues.h"
#include "COCALCPROGRESSIMPL.H"
#include <safearry.h>
#include <Clipbrd.hpp>
#include "uReliefFrm.h"
#include <LisIdwm_TLB.h>
#include <LisLfMfEmin_TLB.h>
#include <LisLFMFFieldStrength_TLB.h>
#include <LisLfMfZone_TLB.h>
#include <p1147_2_TLB.h>
#include <p368_7_TLB.h>
#include "uCoordZoneFieldStr.h"

//---------------------------------------------------------------------------
char* szCalcDuelInterfError = "Помилка '%s' при розрахунку дуельних завад: %s";
char* szCalcZoneError = "Помилка '%s' при розрахунку зони: %s";
//---------------------------------------------------------------------------

#pragma package(smart_init)

TxAnalyzer txAnalyzer;
TCOMIRSASpherics TxAnalyzer::FSpherics;
TCOMILISProgress TxAnalyzer::FProgress;
const ILisGeoSpherePtr geoSphServ;
const ILisGndInfoPtr gndInfoServ;
const ILisIdwmParamPtr idwmParamServ;

const ILisLFMFEminCalcPtr lfmfEmin;
const ILisLFMFFieldStrengthPtr lfmfFs;
const ILisLFMFZoneCalcPtr lfmfCalc;

const IP1147Ptr p1147_2;
const IP368Ptr p368_7;

char *szTxNotDefined = "Не визначений передавач";

struct ZoneKey
{
    long txid; long flag;
    ZoneKey() {}; ZoneKey(long id, long fl): txid(id), flag(fl) {};
    bool operator < (const ZoneKey&) const;
};
bool ZoneKey::operator < (const ZoneKey& z) const
{
    if (this->txid < z.txid)
        return true;
    else if (this->txid > z.txid)
        return false;
    else
        return this->flag < z.flag;
}

typedef std::map<ZoneKey, LPSAFEARRAY> EtalonMap;
EtalonMap etalonMap;

//---------------------------------------------------------------------------
__fastcall TxAnalyzer::TxAnalyzer()
 : isNewPlan(false), wasChanges(false), time0(Now())
{
}
//---------------------------------------------------------------------------

void __fastcall TxAnalyzer::OnProgressNotify(TObject *Sender)
{
    if (dynamic_cast<TForm*>(Sender))
    {
        // form's OnShow Event
        TForm *fm = (TForm*)Sender;
        TLabel *lbl1 = dynamic_cast<TLabel*>(fm->FindComponent("lbl1"));
        if (lbl1)
            lbl1->Caption = FormatDateTime("'Начало 'hh:mm:ss', Прошло '", startedAt = Now());
        TLabel *lbl2 = dynamic_cast<TLabel*>(fm->FindComponent("lbl2"));
        if (lbl2)
        {
            lbl2->Left = lbl1->Left + lbl1->Width;
            lbl2->Caption = "--:--:--";
        }
        TProgressBar* pb = dynamic_cast<TProgressBar*>(fm->FindComponent("pb"));
        if (pb)
            pb->Position = pb->Min;
    }
}

void __fastcall TxAnalyzer::OnCancel(TObject *Sender)
{
    if (dynamic_cast<TButton*>(Sender))
    {
        // CANCEL  button pressed;
        cancelled = true;
        TForm *f = dynamic_cast<TForm*>((TComponent*)Sender);
        if (f)
            f->Close();
    }
}
//---------------------------------------------------------------------------

void __fastcall TxAnalyzer::PerformPlanning(int txId)
{
    int txType = dmMain->GetSc(txId);
    if (txType > ttCTV)
        throw *(new Exception("Тип об'єкту не дозволяє провести планування"));
    if (txType == ttUNKNOWN)
        throw *(new Exception("Невизначений тип передавача - планування неможливе"));

    ClearPlan();
    planningTx.Bind(txBroker.GetTx(txId, dmMain->GetObjClsid(txType)), true);

    //  планируем только сохранённый передатчик
    if (planningTx.data_changes) {
        int reply = Application->MessageBox("Планируемый передатчик изменён.\nСохранить изменения?",
                                            Application->Title.c_str(),
                                            MB_ICONQUESTION | MB_YESNOCANCEL);
        switch (reply) {
            case IDYES:
                planningTx.save();
                break;
            case IDNO:
                planningTx.invalidate();
                planningTx.get_id();
                break;
            case IDCANCEL:
            default:
                return;
        }
    }

    wasChanges = true;
    isNewPlan = false;

    MakeNewSelection(txId, nsPlanning);

}

void __fastcall TxAnalyzer::DoAnalysis()
{
    bool panelWasSimple = frmMain->StatusBar1->SimplePanel;

    ShowProgress("Розрахунок дуельних завад...", planVector.size());
    DoProgress(0);
    unsigned trackProgress = 0;
    if (BCCalcParams.FCalcSrv.IsBound())
        BCCalcParams.FCalcSrv.SetProgressServer(NULL);

    try {

        TCOMIRSAGeoPath geoPathSrv;
        if (BCCalcParams.DisableReliefAtPlanning && BCCalcParams.FCalcSrv.IsBound())
            // поставим пустой
            BCCalcParams.FCalcSrv->SetReliefServer(geoPathSrv);

        PlanVector::iterator pvi;
        for (pvi = planVector.begin(); pvi < planVector.end() && !cancelled; pvi++)
        {
            switch (planningTx.systemcast) {
                case ttTV:
                case ttDVB:
                    planningTx.set_video_carrier(pvi->frequency);
                    planningTx.set_channel_id(pvi->channelId);
                    break;
                case ttAM:
                case ttFM:
                    planningTx.set_video_carrier(pvi->frequency);
                    planningTx.set_sound_carrier_primary(pvi->frequency);
                    break;
                case ttDAB:
                    planningTx.set_blockcentrefreq(pvi->frequency);
                    break;
                default:
                    break;
            }

            txBroker.EnsureList(pvi->txList, NULL);

            TCOMILISBCTxList txList(pvi->txList, true);

            CalcDuelInterfere(txList, String());

            if (txList.Size > 1) { //  один передатчик есть в любом случае
                pvi->maxWantIdx = 1;
                pvi->maxUnwantIdx = 1;
                for (int i = 2; i < txList.Size; i++) {
                    if (txList.get_TxWantInterfere(pvi->maxWantIdx) < txList.get_TxWantInterfere(i))
                        pvi->maxWantIdx = i;
                    if (txList.get_TxUnwantInterfere(pvi->maxUnwantIdx) < txList.get_TxUnwantInterfere(i))
                        pvi->maxUnwantIdx = i;
                }
            } else {
                pvi->maxWantIdx = 0;
                pvi->maxUnwantIdx = 0;
            }

            DoProgress((++trackProgress + 1) * 100. / progressBar->Max);
        }
    }
    __finally
    {
        HideProgress();
    }

    if (BCCalcParams.DisableReliefAtPlanning && BCCalcParams.FCalcSrv.IsBound())
        // восстановим
        BCCalcParams.FCalcSrv->SetReliefServer(BCCalcParams.FPathSrv);

    // приведём в порядок
    planningTx.invalidate();
    planningTx.get_id();

    frmMain->StatusBar1->SimpleText = "";
    frmMain->StatusBar1->SimplePanel = panelWasSimple;
    frmMain->StatusBar1->Update();

    wasChanges = true;
    isNewPlan = false;
}

void __fastcall TxAnalyzer::SaveToDb()
{
    if(!planningTx.IsBound())
        throw *(new Exception(szTxNotDefined));
    if(planningTx.id == 0)
        throw *(new Exception(szTxNotDefined));

    std::auto_ptr<TIBSQL> selSql(new TIBSQL(Application));
    selSql->Database = dmMain->dbMain;
    selSql->SQL->Text = "delete from SELECTIONS where TRANSMITTERS_ID = :TX_ID and SELTYPE = 'P' ";
    selSql->Params->Vars[0]->AsInteger = planningTx.id;
    selSql->ExecQuery();

    selSql->SQL->Text = "insert into SELECTIONS (ID, TRANSMITTERS_ID, NAMEQUERIES, CREATEDATE, USERID, FREQUENCY, SELTYPE, CHANNEL_ID, MAX_WANT_IDX, MAX_UNWANT_IDX) "
                        " values (:ID, :TX, :NAME, 'today', :USER, :FREQ, 'P', :CHANNEL, :MAX_WANT_IDX, :MAX_UNWANT_IDX) ";
    selSql->Prepare();
    selSql->ParamByName("TX")->AsInteger = planningTx.id;
    selSql->ParamByName("USER")->AsInteger = dmMain->UserId;

    std::auto_ptr<TIBSQL> newTxSql(new TIBSQL(Application));
    newTxSql->Database = dmMain->dbMain;
    newTxSql->SQL->Text = "insert into SELECTEDTRANSMITTERS (SELECTIONS_ID, TRANSMITTERS_ID, USED_IN_CALC, E_WANT, E_UNWANT, DISTANCE, AZIMUTH, SORTINDEX) "
                          " values (:SELECTION, :TX, :USED, :E_WANT, :E_UNWANT, :DISTANCE, :AZIMUTH, :SORTINDEX) ";
    newTxSql->Prepare();

    ShowProgress("Збереження планування...", planVector.size());
    DoProgress(0);
    unsigned trackProgress = 0;

    try {
        PlanVector::iterator pvi;
        for (pvi = planVector.begin(); pvi < planVector.end(); pvi++) {
            if (pvi->id == 0)
                pvi->id = dmMain->getNewId();
            selSql->Close();
            selSql->ParamByName("ID")->AsInteger = pvi->id;
            selSql->ParamByName("NAME")->AsString = pvi->name.c_str();
            selSql->ParamByName("FREQ")->AsDouble = pvi->frequency;
            selSql->ParamByName("CHANNEL")->AsInteger = pvi->channelId;
            selSql->ParamByName("MAX_WANT_IDX")->AsInteger = pvi->maxWantIdx;
            selSql->ParamByName("MAX_UNWANT_IDX")->AsInteger = pvi->maxUnwantIdx;
            selSql->ExecQuery();
            TCOMILISBCTxList txList(pvi->txList, true);
            for (int i = 1; i < txList.Size; i++) {
                newTxSql->Close();
                newTxSql->ParamByName("SELECTION")->AsInteger = pvi->id;
                newTxSql->ParamByName("TX")->AsInteger = txList.get_TxId(i);
                newTxSql->ParamByName("USED")->AsInteger = 1;
                newTxSql->ParamByName("E_WANT")->AsDouble = txList.get_TxWantInterfere(i);
                newTxSql->ParamByName("E_UNWANT")->AsDouble = txList.get_TxUnwantInterfere(i);
                newTxSql->ParamByName("DISTANCE")->AsInteger = txList.get_TxDistance(i);
                newTxSql->ParamByName("AZIMUTH")->AsInteger = txList.get_TxAzimuth(i);
                newTxSql->ParamByName("SORTINDEX")->AsInteger = i;
                newTxSql->ExecQuery();
            }

            DoProgress((++trackProgress + 1) * 100. / progressBar->Max);
        }
    }
    __finally
    {
        HideProgress();
    }

    selSql->Transaction->CommitRetaining();

    wasChanges = false;
    isNewPlan = false;
}

void __fastcall TxAnalyzer::LoadFromDb()
{
    if(!planningTx.IsBound())
        throw *(new Exception(szTxNotDefined));
    if(planningTx.id == 0)
        throw *(new Exception(szTxNotDefined));

    ClearPlan();

    
    std::auto_ptr<TIBQuery> selQr(new TIBQuery(Application));
    selQr->Database = dmMain->dbMain;
    selQr->SQL->Text = "select count(ID) "
                        "from SELECTIONS "
                        "where TRANSMITTERS_ID = :TX_ID and SELTYPE = 'P' ";
    selQr->Params->Items[0]->AsInteger = planningTx.id;
    selQr->Open();
    unsigned planSize = selQr->Fields->Fields[0]->AsInteger;

    ShowProgress("Загрузка планування...", planSize);
    DoProgress(0);
    unsigned trackProgress = 0;

    try {

        selQr->Close();
        selQr->SQL->Text = "select ID, NAMEQUERIES, FREQUENCY, CHANNEL_ID, MAX_WANT_IDX, MAX_UNWANT_IDX "
                            "from SELECTIONS "
                            "where TRANSMITTERS_ID = :TX_ID and SELTYPE = 'P' "
                            "order by FREQUENCY ";
        selQr->Params->Items[0]->AsInteger = planningTx.id;
        selQr->Open();
        selQr->FetchAll();

        std::auto_ptr<TIBQuery> txQr(new TIBQuery(Application));
        txQr->Database = dmMain->dbMain;
        txQr->SQL->Text = "select st.SELECTIONS_ID, st.TRANSMITTERS_ID, st.E_WANT, st.E_UNWANT, st.DISTANCE, "
                          "st.AZIMUTH, st.SORTINDEX, sc.ENUMVAL "
                              " from SELECTEDTRANSMITTERS st "
                              " left outer join TRANSMITTERS tx on (st.TRANSMITTERS_ID = tx.id) "
                              " left outer join SYSTEMCAST sc on (tx.systemcast_id = sc.id) "
                              " where SELECTIONS_ID = :SEL "
                              " order by SORTINDEX ";
        txQr->Prepare();

        selQr->First();
        while (!selQr->Eof && !cancelled) {
            PlanVector::reference pvr = AddPlanEntry(
                                                selQr->FieldByName("ID")->AsInteger,
                                                selQr->FieldByName("NAMEQUERIES")->AsString.c_str(),
                                                selQr->FieldByName("CHANNEL_ID")->AsInteger,
                                                selQr->FieldByName("FREQUENCY")->AsFloat,
                                                selQr->FieldByName("MAX_WANT_IDX")->AsInteger,
                                                selQr->FieldByName("MAX_UNWANT_IDX")->AsInteger
                                            );

            TCOMILISBCTxList txList(pvr.txList, true);
            txList.AddTx(planningTx);
            txQr->Close();
            txQr->Params->Items[0]->AsInteger = pvr.id;
            txQr->Open();
            txQr->FetchAll();
            for (txQr->First(); !txQr->Eof && !cancelled; txQr->Next())
            {
                int newIdx = txList.AddTx(txBroker.GetTx(txQr->FieldByName("TRANSMITTERS_ID")->AsInteger,
                                                         dmMain->GetObjClsid(txQr->FieldByName("ENUMVAL")->AsInteger)));
                txList.set_TxUseInCalc(newIdx, true);
                txList.set_TxWantInterfere(newIdx, txQr->FieldByName("E_WANT")->AsFloat);
                txList.set_TxUnwantInterfere(newIdx, txQr->FieldByName("E_UNWANT")->AsFloat);
                txList.set_TxDistance(newIdx, txQr->FieldByName("DISTANCE")->AsFloat);
                txList.set_TxAzimuth(newIdx, txQr->FieldByName("AZIMUTH")->AsFloat);
            }
            txBroker.EnsureList(txList, NULL);

            selQr->Next();

            DoProgress((++trackProgress + 1) * 100. / progressBar->Max + 0.5);
        }
    }
    __finally
    {
        HideProgress();
    }

    wasChanges = false;
    isNewPlan = false;
}

void __fastcall TxAnalyzer::ShowDuelResult(ILISBCTx *pTxA, ILISBCTx *pTxB, int idxA, int idxB, const TDuelResult2& duelResult, const TPointDuelResult* duelRes)
{
    if (!TxAnalyzer::FSpherics.IsBound())
        TxAnalyzer::FSpherics.CreateInstance(CLSID_RSASpherics);

    TCOMILISBCTx txA(pTxA, true);
    TCOMILISBCTx txB(pTxB, true);

    bool ttAMType = (txA.systemcast == ttAM);
    TfrmDuelResult *dr = new TfrmDuelResult(Application, ttAMType);
    long height;
    txA->get_heightantenna(&height);
    dr->fmProfileView->leftTxHeight = height;
    txB->get_heightantenna(&height);
    dr->fmProfileView->rightTxHeight = height;

    long num = 0;
    if (SafeArrayGetUBound(duelResult.Tx1_NoiseLimited, 1, &num) == S_OK)
        num++;

    dr->Import_OnMouseMove = dr->fmProfileView->External_OnMouseMove;
    dr->fmProfileView->Import_OnMouseMove = dr->External_OnMouseMove;

    dr->pdpA->clear();
    dr->pdpA->showAxis = false;
    dr->pdpA->setNoiseLimited((double*)duelResult.Tx1_NoiseLimited->pvData, BCCalcParams.lineColorZoneNoise, BCCalcParams.lineThicknessZoneNoise, num);
    dr->pdpA->setInterfereLimited((double*)duelResult.Tx1_InterferenceLimited->pvData, BCCalcParams.lineColorZoneInterfere, BCCalcParams.lineThicknessZoneInterfere, num);
    dr->pdpA->setNoiseLimited2((double*)duelResult.Tx2_NoiseLimited->pvData, num);
    dr->pdpA->setInterfereLimited2((double*)duelResult.Tx2_InterferenceLimited->pvData, num);
    dr->pdpA->label = AnsiString(idxA);// + ": " + dmMain->GetSystemCastName(txA.systemcast);
    dr->pdpA->label2 = AnsiString(idxB);// + ": " + dmMain->GetSystemCastName(txB.systemcast);

    dr->pdpB->clear();
    dr->pdpB->showAxis = false;
    dr->pdpB->setNoiseLimited((double*)duelResult.Tx2_NoiseLimited->pvData, BCCalcParams.lineColorZoneNoise, BCCalcParams.lineThicknessZoneNoise, num);
    dr->pdpB->setInterfereLimited((double*)duelResult.Tx2_InterferenceLimited->pvData, BCCalcParams.lineColorZoneInterfere, BCCalcParams.lineThicknessZoneInterfere, num);
    dr->pdpB->label = AnsiString(idxB);// + ": " + dmMain->GetSystemCastName(txB.systemcast);

    if (dr->pdpA->norma < dr->pdpB->norma)
        dr->pdpA->norma = dr->pdpB->norma;
    else
        dr->pdpB->norma = dr->pdpA->norma;

    TRSAGeoPoint pointA, pointB;
    pointA.H = txA.latitude;
    pointA.L = txA.longitude;
    pointB.H = txB.latitude;
    pointB.L = txB.longitude;

    double dist = 0.0;
    double azm = 0.0;

    if ( BCCalcParams.ReliefServerArrayGUID != "" )
    {
        BCCalcParams.FTerrInfoSrv->Distance(pointA, pointB, &dist);
        BCCalcParams.FTerrInfoSrv->Azimuth(pointA, pointB, &azm);
    }
    else
    {
        FSpherics.Distance(pointA, pointB, &dist);
        FSpherics.Azimuth(pointA, pointB, &azm);
    }

    dr->pdpA->duelDistance = dist;
    dr->pdpA->duelAzimuth = azm;

    int leftPointNo = (duelRes[0].radius + dist > duelRes[2].radius) ? 0 : 2;
    int rightPointNo = (duelRes[1].radius > dist + duelRes[3].radius) ? 1 : 3;

    //  максимальноке расстояние. оно же - масштаб
    ////////////////////////
        //  начало координат - точка установки первого передатчика
        //  найдём макс. значения X и Y в этих координатах.
        double minX = 0.0;
        double minY = 0.0;
        double maxX = 0.0;
        double maxY = 0.0;

        double duelCorrX = 0.0;
        double duelCorrY = 0.0;

        for (int zoneIdx = 0; zoneIdx < 4; zoneIdx++) {
            double* zone = NULL;
            switch (zoneIdx) {
                case 0: zone = (double*)duelResult.Tx1_NoiseLimited->pvData;
                        break;
                case 1: zone = (double*)duelResult.Tx1_InterferenceLimited->pvData;
                        break;
                case 2: zone = (double*)duelResult.Tx2_NoiseLimited->pvData;
                        break;
                case 3: zone = (double*)duelResult.Tx2_InterferenceLimited->pvData;
                        break;
                default: continue;
            }
            if (zoneIdx == 2 || zoneIdx == 3) {
                duelCorrX = dist;
            }

            int i = 0;
            for (double *f = zone; f < zone + num; f++, i++) {
                //  double angle = i / 18.0 * M_PI;
                //  развернём ось дуэли вдоль горизонтального края экрана
                double angle = (i * 360 / num - azm + 90) / 180.0 * M_PI;
                double x = (*f) * sin(angle) + duelCorrX;
                double y = - (*f) * cos(angle) + duelCorrY;
                if (minX > x) minX = x;
                if (minY > y) minY = y;
                if (maxX < x) maxX = x;
                if (maxY < y) maxY = y;
            }
        }

    ////////////////////////
    /*
    double zoom = duelRes[leftPointNo].radius + duelRes[rightPointNo].radius;
    if (leftPointNo == 0 && rightPointNo == 3)
        zoom += dist;
    else if (leftPointNo == 2 && rightPointNo == 1)
        zoom -= dist;
    */

    double zoom = maxX - minX;
    // реальные границы зон (а не та хрень шо Дима в duelRes[0] и duelRes[3] возвращает
    TRSAGeoPoint point0, point3;
    if (minX < 0)
        minX *= -1;
    FSpherics.PolarToGeo(minX, azm + 180, pointA, &point0);
    if (maxX < 0)
        maxX *= -1;
    FSpherics.PolarToGeo(maxX, azm, pointA, &point3);

    dr->dist03 = zoom;

    dr->lblA->Caption = AnsiString(idxA) + ':';
    dr->lblB->Caption = AnsiString(idxB) + ':';
    dr->lblEminA->Caption = AnsiString("E мін ") + AnsiString(idxA) + " = ";
    dr->lblEminB->Caption = AnsiString("E мін ") + AnsiString(idxB) + " = ";
    dr->lblEa->Caption = AnsiString("Сигнал в точці ") + dr->lblA->Caption;
    dr->lblEb->Caption = AnsiString("Сигнал в точці ") + dr->lblB->Caption;

    dr->lblAData->Caption = GetDuelObjString(txA, duelRes[1].azimuth) + ", азимут " + (int)duelRes[1].azimuth + "\xB0, радіус зони " + FormatFloat("0.0", duelRes[1].radius);
    dr->lblBData->Caption = GetDuelObjString(txB, duelRes[2].azimuth) + ", азимут " + (int)duelRes[2].azimuth + "\xB0, радіус зони " + FormatFloat("0.0", duelRes[2].radius);

    dr->lblEminAData->Caption = FormatFloat("0.0", GetEmin(txA));
    dr->lblEminBData->Caption = FormatFloat("0.0", GetEmin(txB));


    dr->lblAData->Left = dr->lblA->Left + dr->lblA->Width + 2;
    dr->lblBData->Left = dr->lblB->Left + dr->lblB->Width + 2;
    dr->lblEminAData->Left = dr->lblEminA->Left + dr->lblEminA->Width + 2;
    dr->lblEminBData->Left = dr->lblEminB->Left + dr->lblEminB->Width + 2;

    char *pointNo[] = {"-1", "-2", "-3", "-4"};
    if(ttAMType)
    {
        dr->grdPoints->RowCount = 5;
        dr->grdPoints->Cells[0][1] = AnsiString(idxA) + pointNo[0];
        dr->grdPoints->Cells[0][2] = AnsiString(idxA) + pointNo[1];
        dr->grdPoints->Cells[0][3] = AnsiString(idxB) + pointNo[2];
        dr->grdPoints->Cells[0][4] = AnsiString(idxB) + pointNo[3];

        for (int row = 0; row < 4; row++) {
            dr->grdPoints->Cells[1][row + 1] = FormatFloat("0.0", duelRes[row].eInt);
            dr->grdPoints->Cells[2][row + 1] = FormatFloat("0.0", duelRes[row].aPR);
            dr->grdPoints->Cells[3][row + 1] = FormatFloat("0.0", duelRes[row].eUsable);
            dr->grdPoints->Cells[4][row + 1] = FormatFloat("0.0", duelRes[row].emin);
        }
    } else {
        dr->grdPoints->RowCount = 17;
        for (int iRow = 1; iRow <= 16; iRow++)
        {
            if ((txA.polarization == plHOR) && ((iRow-1)%4 >= 2)
                || (txA.polarization == plVER) && ((iRow-1)%4 < 2)
                || (txB.polarization == plHOR) && ((iRow/2)*2 == iRow)
                || (txB.polarization == plVER) && ((iRow/2)*2 != iRow))
            {
                // не загромождаем
                dr->grdPoints->Rows[iRow]->Clear();
                continue;
            }
            
            dr->grdPoints->Cells[0][iRow] = (iRow <= 8 ? AnsiString(idxA) : AnsiString(idxB)) +
                                            pointNo[(iRow-1)/4] + "   " +
                                            ((iRow-1)%4 < 2 ? "H" : "V") +
                                            (iRow <= 8 ? " <- " : " -> ") +
                                            ((iRow/2)*2 != iRow ? "H" : "V")
                                            ;

            dr->grdPoints->Cells[1][iRow] = FormatFloat("0.0", duelRes[iRow-1].eInt);
            dr->grdPoints->Cells[2][iRow] = FormatFloat("0.0", duelRes[iRow-1].aPR);
            dr->grdPoints->Cells[3][iRow] = FormatFloat("0.0", duelRes[iRow-1].aDiscr);
            dr->grdPoints->Cells[4][iRow] = FormatFloat("0.0", duelRes[iRow-1].eUsable);
            dr->grdPoints->Cells[5][iRow] = FormatFloat("0.0", duelRes[iRow-1].emin);
            dr->grdPoints->Cells[6][iRow] = duelRes[iRow-1].intType == 0 ? 'Т' : 'П';
        }
    }

    dr->Show();
    dr->Update();

    if (txA.systemcast == ttAM)
        dr->panRelief->Visible = false;
    else
    {
        dr->panRelief->Visible = true;
        dr->fmProfileView->RetreiveProfile( point0.L, point0.H,
                                            pointA.L, pointA.H,
                                            pointB.L, pointB.H,
                                            point3.L, point3.H);
    }

}

AnsiString __fastcall TxAnalyzer::GetDuelObjString(ILISBCTx* iTx, double azimuth)
{
    TCOMILISBCTx tx(iTx, true);
    AnsiString repres = txAnalyzer.GetTxNominalString(tx);

    double erp;
    double erp_aux;
    TBCPolarization pol = tx.polarization;
    char cPol;

    if(tx.systemcast == ttAM)
    {
        ILisBcLfMfPtr lfmf;
        HrCheck(tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf), "Запрос интерфейса ILisBcLfMf");
        unsigned char ucv = '\0';
        HrCheck(lfmf->get_ant_type(&ucv));
        cPol = ucv;
        erp = tx.get_erp(azimuth);
        repres = repres + ", Ha = " + IntToStr((int)tx.get_heightantenna()) + " м";
        repres = repres + ", Pеф = " + FormatFloat("0.0 дБкВт", erp) + ", " + cPol;
    }
    else
    {
        erp = tx.get_erp(azimuth);

        if (pol == plVER) {
            cPol = 'В';
        } else if (pol == plHOR) {
            cPol = 'Г';
        } else {
            cPol = 'З';
        }

        repres = repres + ", Hеф = " + IntToStr((int)tx.get_h_eff(azimuth)) + " м";
        if (tx.systemcast == ttTV)
            repres = repres + ", ЗНЧ = " + FormatFloat("0.0 кГц", tx.video_offset_herz / 1000.0);
        repres = repres + ", Pеф = " + FormatFloat("0.0 дБкВт", erp) + ", " + cPol;
    }
    return repres;
}


void __fastcall TxAnalyzer::CalcDuelPlan(int veId, int duelId)
{
    TCOMILISBCTxList txList(planVector[veId].txList, true);
    TCOMILISBCTx duelTx(txList.get_Tx(duelId), true);
    txList.Bind(NULL);
    txList.CreateInstance(CLSID_LISBCTxList);                                           

    txList.AddTx(planningTx);
    txList.AddTx(duelTx);

    switch (planningTx.systemcast) {
        case ttTV:
        case ttDVB:
            planningTx.set_video_carrier(planVector[veId].frequency);
            planningTx.set_channel_id(planVector[veId].channelId);
            break;
        case ttAM:
        case ttFM:
            planningTx.set_video_carrier(planVector[veId].frequency);
            planningTx.set_sound_carrier_primary(planVector[veId].frequency);
            break;
        case ttDAB:
            planningTx.set_blockcentrefreq(planVector[veId].frequency);
            break;
        default:
            break;
    }

    TDuelResult2 duelResult;

    TPointDuelResult duelRes[16];
    //TPointDuelResultArray& pdra = *((TPointDuelResultArray*)duelRes);

    Screen->Cursor = crHourGlass;
    try {
        BCCalcParams.FCalcSrv->SetTxListServer(txList);
        BCCalcParams.FCalcSrv->CalcDuel2(planningTx, duelTx, &duelResult);
        CalcDuel3(planningTx, duelTx, duelResult, duelRes);
        //BCCalcParams.FCalcSrv->CalcDuel3(planningTx, duelTx, &pdra);
        ShowDuelResult(planningTx, duelTx, 0, 1, duelResult, duelRes);

    } __finally {
        Screen->Cursor = crDefault;
        // приведём в порядок
        planningTx.invalidate();
        planningTx.get_id();
    }

    //  удалить массивы
    SafeArrayDestroy(duelResult.Tx1_NoiseLimited);
    SafeArrayDestroy(duelResult.Tx1_InterferenceLimited);
    SafeArrayDestroy(duelResult.Tx2_NoiseLimited);
    SafeArrayDestroy(duelResult.Tx2_InterferenceLimited);

}
//---------------------------------------------------------------------

void __fastcall TxAnalyzer::GetCoordinationZoneFromBase(ILISBCTx* ptrTx, double* zone)
{
    if (ptrTx) {
        for (double *ptr = zone; ptr < zone + 36; ptr++)
            *ptr = 0.0;

        TCOMILISBCTx tx(ptrTx, true);

        int systemcast_id = tx.systemcast; //  это ещё не systemcast_id
        if (!dmMain->ibdsScList->Active) {
            dmMain->ibdsScList->Open();
            dmMain->ibdsScList->FetchAll();
        }
                        
        if (dmMain->ibdsScList->Locate("ENUMVAL", systemcast_id, TLocateOptions()))
            systemcast_id = dmMain->ibdsScListID->AsInteger;
        else
            return; //  "Невідома система"

        //  настроим профиль
        TCOMIRSAGeoPath gp;
        OLECHECK(gp.CreateInstance(CLSID_RSAGeoPath,  0, CLSCTX_INPROC_SERVER));
        gp.Init(BCCalcParams.FTerrInfoSrv);

        TRSAPathParams pathParams;
        pathParams.CalcHEff = false;
        pathParams.CalcTxClearance = false;
        pathParams.CalcRxClearance = false;
        pathParams.CalcSeaPercent =  true;
        pathParams.Step = BCCalcParams.Step;

        gp->Set_Params(pathParams);

        TRSAGeoPoint geoPoint;
        geoPoint.H = tx.latitude;
        geoPoint.L = tx.longitude;

        TRSAGeoPathData pathData;
        pathData.RxHeight = 0;

        TRSAGeoPathResults pathRes;

        // шаг равномерный
        double step = 10.0;
        for (int i = 0; i < 36; i++) {

            int idx36 = i;
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
        dmMain->ibdsCoordDist->Close();
        dmMain->ibdsCoordDist->Transaction->CommitRetaining();
    }
}
//---------------------------------------------------------------------
void __fastcall TxAnalyzer::GetCoordinationZone(ILISBCTx* ptrTx, double* zone, double e_trig)
{
    TempCursor tc(crHourGlass);
    TBCTxType tt = ttUNKNOWN;
    ptrTx->get_systemcast(&tt);
    if (tt == ttAM)
        for (int i = 0; i < 36; i++)
            zone[i] = 10000.;
    else if ( BCCalcParams.GetCoordinatesFromBase )
        GetCoordinationZoneFromBase(ptrTx, zone);
    else {
        LPSAFEARRAY iZone;
        ICoordZonePtr coordZone;
        OleCheck(BCCalcParams.FCalcSrv->QueryInterface(IID_ICoordZone, (void**)&coordZone));
        coordZone->setCoordFieldStrength(e_trig);
        e_trig = 0;
        coordZone->getCoordFieldStrength(&e_trig);
        BCCalcParams.FCalcSrv.GetCoordinationZone(ptrTx, &iZone);
        for (int i = 0; i < 36; i++)
            zone[i] = ((double*)(iZone->pvData))[i];
        SafeArrayDestroy(iZone);
    }
}
//---------------------------------------------------------------------

bool __fastcall TxAnalyzer::DoProgress(long perc)
{
    if ( progressBar != NULL )
        progressBar->Position = perc * progressBar->Max / 100.;

    TLabel *lbl2 = dynamic_cast<TLabel*>(frmProgress->FindComponent("lbl2"));
    if (lbl2)
    {
        TDateTime ex = Now() - startedAt;
        lbl2->Caption = FormatDateTime("hh:mm:ss', '", ex) + IntToStr(perc) + "%"
                        ", Осталось " + (perc == 0 ? String("--:--:--")
                            : FormatDateTime("hh:mm:ss", (double)ex * (100 - perc) / perc))
                        ;
    }

    Application->ProcessMessages();

    return cancelled;
}

TForm * __fastcall TxAnalyzer::CreateProgressForm(AnsiString FormCaption)
{
    frmProgress = new TForm(Application);
      frmProgress->BorderStyle = bsDialog;
      frmProgress->BorderIcons = TBorderIcons();
      frmProgress->Caption = FormCaption;
      frmProgress->FormStyle = fsStayOnTop;
      frmProgress->Height = 90;
      frmProgress->Visible = false;
      frmProgress->Width = 400;
      frmProgress->OnShow = OnProgressNotify;

    #ifdef _DEBUG
    frmProgress->Left = Screen->Width - frmProgress->Width - 5;
    frmProgress->Top = Screen->Height - frmProgress->Height - 5;
    HWND tbh = FindWindow("Shell_TrayWnd", "");
    if (tbh != NULL)
    {
        TRect tbRect; // taskbar rectangle
        if (GetWindowRect(tbh, &tbRect))
            frmProgress->Top = frmProgress->Top - (tbRect.Bottom - tbRect.Top);
    }
    #else
    frmProgress->Position = poScreenCenter;
    #endif

    progressBar = new TProgressBar(frmProgress);
      progressBar->Left = 4;
      progressBar->Max = 100;
      progressBar->Min = 0;
      progressBar->Parent = frmProgress;
      progressBar->Step = 1;
      progressBar->Top = 20;
      progressBar->Width = frmProgress->ClientWidth - progressBar->Left * 2;
      progressBar->Name = "pb";

    TButton *btn = new TButton(frmProgress);
      btn->Cancel = true;
      btn->Caption = "Зупинити";
      btn->Height = 20;
      btn->Left = (frmProgress->ClientWidth - btn->Width) / 2;
      btn->OnClick = OnCancel;
      btn->Parent = frmProgress;
      btn->Top = 40;

    TLabel *lbl1 = new TLabel(frmProgress);
      lbl1->Parent = frmProgress;
      lbl1->Top = 4;
      lbl1->Left = 8;
      lbl1->Name = "lbl1";

    TLabel *lbl2 = new TLabel(frmProgress);
      lbl2->Parent = frmProgress;
      lbl2->Top = 4;
      lbl2->Name = "lbl2";
      return frmProgress;
}

void __fastcall TxAnalyzer::ShowProgress(AnsiString ProgressFormCaption, unsigned max)
{
    if ( frmProgress == NULL )
        CreateProgressForm(ProgressFormCaption);
    else
        frmProgress->Caption = ProgressFormCaption;

    frmProgress->Show();
    progressBar->Min = 0;
    progressBar->Position = 0;
    progressBar->Max = max;

    if (!FProgress.IsBound())
        FProgress.CreateInstance(CLSID_CoCalcProgress);

    cancelled = false;

    if (BCCalcParams.FCalcSrv.IsBound())
        BCCalcParams.FCalcSrv->SetProgressServer(FProgress);
    if (lfmfCalc)
        lfmfCalc->set_Progress(ProgressFormCaption.Length() > 0 ? (IUnknown*)FProgress : (IUnknown*)NULL);
        
    FormProvider.DisableApplicationControls();
}

void __fastcall TxAnalyzer::HideProgress()
{
    if (BCCalcParams.FCalcSrv.IsBound())
        BCCalcParams.FCalcSrv->SetProgressServer(NULL);

    if ( frmProgress != NULL )
        frmProgress->Hide();

    FormProvider.EnableApplicationControls();
}

void __fastcall TxAnalyzer::GetTxZone(TCOMILISBCTxList &txList, LPSAFEARRAY* zone, AnsiString pfCaption)
{
    if (pfCaption.Length() > 0)
        ShowProgress(pfCaption);
    try
    {
        TBCTxType tt = GetDiapason(txList);
        if (tt == ttAM)
        {
            ILISBCTx* tx = NULL;
            HrCheck(txList->get_Tx(0, &tx));
            CheckLfMfCalc();
            lfmfCalc->set_Progress(pfCaption.Length() > 0 ? (IUnknown*)FProgress : (IUnknown*)NULL);
            HrCheck(lfmfCalc->GetZone(tx, txList, time0, zone));
        } else  {
            HrCheck(BCCalcParams.FCalcSrv->SetTxListServer(txList));
            HrCheck(BCCalcParams.FCalcSrv->GetZone_InterferenceLimited(BCCalcParams.degreeStep, zone));
        }
    }

    catch(Exception &E)
    {
        Application->MessageBox(AnsiString().sprintf(szCalcZoneError, AnsiString(E.ClassName()).c_str(), E.Message.c_str()).c_str(),
                            Application->Title.c_str(),
                            MB_ICONHAND
                            );
    }
    if (pfCaption.Length() > 0)
        HideProgress();
}

void __fastcall TxAnalyzer::GetSfnZone(TCOMILISBCTxList &sfn, double* eMin, LPSAFEARRAY* zone)
{
    try
    {
        ShowProgress("Расчёт зоны уверенного приёма ОЧС...");
        try
        {
            ISFNCalcPtr sfnCalc;
            OleCheck(BCCalcParams.FCalcSrv->QueryInterface(IID_ISFNCalc, (void**)&sfnCalc));
            TCOMILISBCTx allot(sfn.get_Tx(0), true);
            double lat = allot.latitude;
            double lon = allot.longitude;
            sfnCalc->GetSfnZone(sfn, &lon, &lat, eMin, zone);
        }
        catch(Exception &E)
        {
            Application->MessageBox(AnsiString().sprintf("Ошибка расчёта покрытия ОЧС. \n%s: %s",
                                        AnsiString(E.ClassName()).c_str(), E.Message.c_str()).c_str(),
                                        Application->Title.c_str(),
                                        MB_ICONHAND
                                        );
        }
    }
    __finally
    {
        HideProgress();
    }
}

void __fastcall TxAnalyzer::CalcDuelInterfere(TCOMILISBCTxList &txList, AnsiString pfCaption)
{
    if (pfCaption.Length() > 0)
        ShowProgress(pfCaption);
    try
    {
        try
        {
            TBCTxType diapason = GetDiapason(txList);
            if (diapason == ttAM)
            {
                CheckLfMfCalc();

                ILISBCTxPtr tx0(txList.get_Tx(0), true);
                double lon0 = 0.0;
                double lat0 = 0.0;
                tx0->get_longitude(&lon0);
                tx0->get_latitude(&lat0);

                int listSize = txList.Size;
                for (int i = 1; i < listSize; i++)
                {
                    txList.set_TxUnwantInterfere(i, -999.);
                    txList.set_TxUnwantedKind(i, 'G');
                    txList.set_TxWantInterfere(i, -999.);
                    txList.set_TxWantedKind(i, 'G');
                }

                ILisBcLfMfPtr uTx0;
                OleCheck(tx0->QueryInterface(IID_ILisBcLfMf, (void**)&uTx0));
                long system0 = uTx0->lfmf_system;
                long system;

                for (int i = 1; i < listSize; i++)
                {
                    ILISBCTxPtr tx(txList.get_Tx(i), true);
                    double lon = 0.0;
                    double lat = 0.0;
                    tx->get_longitude(&lon);
                    tx->get_latitude(&lat);

                    double lonm = (lon0 + lon) / 2.;
                    if (fabs(lonm-lon) > 90.)
                        lonm = lonm > 0. ? lonm-180. : lonm+180.;
                    double latm = (lat0 + lat) / 2.;

                    char tw = '\0'; char tu = '\0';
                    double cw = -999.;
                    double cu = -999.;
                    double prw = -999.;
                    double pru = -999.;
                    long sfnIdW = 0, sfnIdUw = 0;
                    ILisBcLfMfPtr uTx;


                    switch (BCCalcParams.duelType)
                    {
                        case dtTxLocation:
                            cw = GetE(tx0, lon, lat, &tw);
                            prw = GetPr(tx0, tx);
                            OleCheck(tx->QueryInterface(IID_ILisBcLfMf, (void**)&uTx));
                            system = uTx->lfmf_system;
                            uTx->Release();
                            if(system0 == 1)
                            {
                                tx->get_identifiersfn(&sfnIdW);
                                tx0->get_identifiersfn(&sfnIdUw);
                                if(sfnIdW == sfnIdUw && sfnIdW != 0)
                                {
                                    prw -= 19;
                                }
                                else
                                {
                                    if(tw == 'G')
                                        prw += 3;
                                }
                            }

                            cu = GetE(tx, lon0, lat0, &tu);
                            pru = GetPr(tx, tx0);
                            if(system == 1)
                            {
                                if(sfnIdW == sfnIdUw && sfnIdW != 0)
                                {
                                    pru -= 19;
                                }
                                else
                                {
                                    if(tu == 'G')
                                        pru += 3;
                                }
                            }
                            break;
                        case dtMiddlePoint:
                            cw = GetE(tx0, lonm, latm, &tw);
                            prw = GetPr(tx0, tx);
                            if(system0 == 1)
                            {
                                tx->get_identifiersfn(&sfnIdW);
                                tx0->get_identifiersfn(&sfnIdUw);
                                if(sfnIdW == sfnIdUw && sfnIdW != 0)
                                {
                                    prw -= 19;
                                }
                                else
                                {
                                    if(tw == 'G')
                                        prw += 3;
                                }
                            }
                            cu = GetE(tx, lonm, latm, &tu);
                            pru = GetPr(tx, tx0);
                            OleCheck(tx->QueryInterface(IID_ILisBcLfMf, (void**)&uTx));
                            system = uTx->lfmf_system;
                            uTx->Release();
                            if(system == 1)
                            {
                                if(sfnIdW == sfnIdUw && sfnIdW != 0)
                                {
                                    pru -= 19;
                                }
                                else
                                {
                                    if(tu == 'G')
                                        pru += 3;
                                }
                            }
                            break;
                        case dtZoneContraction:
                            {
                                ILISBCTxListPtr txList; txList.CreateInstance(CLSID_LISBCTxList);
                                long idx = 0;
                                float az;
                                geoSphServ->GetAzimuth(lon0, lat0, lon, lat, &az);
                                double d1 = GetZoneByDir(tx0, txList, az);
                                txList->AddTx(tx, &idx);
                                double d2 = GetZoneByDir(tx0, txList, az);
                                tu = d2 / d1;

                                geoSphServ->GetAzimuth(lon, lat, lon0, lat0, &az);
                                txList->Clear();
                                d1 = GetZoneByDir(tx, txList, az);
                                txList->AddTx(tx0, &idx);
                                d2 = GetZoneByDir(tx, txList, az);
                                tw= d2 / d1;
                            }
                            break;
                    }
                    double freq;
                    tx->get_sound_carrier_primary(&freq);
                    txList.set_TxUnwantInterfere(i, cu + pru);
                    txList.set_TxUnwantedKind(i, tu);
                    txList.set_TxWantInterfere(i, cw + prw);
                    txList.set_TxWantedKind(i, tw);

                    float d = 0., a = 0.;
                    geoSphServ->GetAzDist(lon0, lat0, lon, lat, &a, &d);
                    txList.set_TxAzimuth(i, a);
                    txList.set_TxDistance(i, d);

                    bool cancelled = pfCaption.Length() > 0 && DoProgress(i * 100. / (listSize));
                    if (cancelled)
                        break;
                }
            }
            else
            {
                BCCalcParams.FCalcSrv->SetTxListServer(txList);
                BCCalcParams.FCalcSrv->CalcDuelInterf();
            }
        }
        catch(Exception &E)
        {
            Application->MessageBox(AnsiString().sprintf(szCalcDuelInterfError, AnsiString(E.ClassName()).c_str(), E.Message.c_str()).c_str(),
                                Application->Title.c_str(),
                                MB_ICONHAND
                                );
        }
    }
    __finally
    {
        if (pfCaption.Length() > 0)
            HideProgress();
    }
}

void __fastcall TxAnalyzer::Shutdown()
{
    if (FProgress.IsBound())
        FProgress.Unbind();
}

/******
 *  параметры контрольной точки
 */
void __fastcall TxAnalyzer::GetCpE(ILISBCTxList* pTxList,
                        double lon, double lat, LPSAFEARRAY res)
{
    TCOMILISBCTxList txList(pTxList, true);
    TCOMILISBCTx tx1(txList.get_Tx(0), true);
    TCOMILISBCTx tx2(txList.get_Tx(1), true);

    typedef TSafeArrayT<TVariant, VT_VARIANT, 1> TSafeArrayVariant1;

    TSafeArrayVariant1 sad; sad.Attach(res);

    if (!BCCalcParams.FCalcSrv.IsBound())
        throw *(new Exception("Calc Server is not initialized"));

    sad[cpdiE01] = BCCalcParams.FCalcSrv->GetFieldStrength(txList.get_Tx(0), lon, lat, 1);
    sad[cpdiE10] = BCCalcParams.FCalcSrv->GetFieldStrength(txList.get_Tx(0), lon, lat, 10);
    sad[cpdiE50] = BCCalcParams.FCalcSrv->GetFieldStrength(txList.get_Tx(0), lon, lat, 50);

    BCCalcParams.FCalcSrv->SetTxListServer(pTxList);

    //  нужно найти индекс второго передатчика
    txList.set_TxUseInCalc(0, true);

    txList.set_TxUseInCalc(1, false);
    sad[cpdiEu1] = BCCalcParams.FCalcSrv->GetUsableFieldStrength(lon, lat);
    txList.set_TxUseInCalc(1, true);
    sad[cpdiEu2] = BCCalcParams.FCalcSrv->GetUsableFieldStrength(lon, lat);

    if (tx1.systemcast != ttAM)
    {
        double pr_c, pr_t;
        BCCalcParams.FCalcSrv->GetProtectRatio(tx1, txList.get_Tx(1), &pr_c, &pr_t);
        sad[cpdiPrC] = pr_c;
        sad[cpdiPrT] = pr_t;
    } else {
        double pr = GetPr(tx1, tx2);
        ILisBcLfMfPtr uTx1;
        OleCheck(tx1->QueryInterface(IID_ILisBcLfMf, (void**)&uTx1));
        long system = uTx1->lfmf_system;
        if(system == 1)
        {
            long sfnIdW = 0, sfnIdUw = 0;
            tx1->get_identifiersfn(&sfnIdW);
            tx2->get_identifiersfn(&sfnIdUw);
            if(sfnIdW == sfnIdUw && sfnIdW != 0)
            {
                pr -= 19;
            }
        }
        sad[cpdiPrC] = pr;
        sad[cpdiPrT] = pr;
    }

    static IRSASphericsPtr spherics;
    if (!spherics.IsBound())
        spherics.CreateInstance(CLSID_RSASpherics);

    TRSAGeoPoint cp, tp1, tp2;
    cp.L = lon, cp.H = lat;
    tx1->get_longitude(&tp1.L);
    tx1->get_latitude(&tp1.H);
    tx2->get_longitude(&tp2.L);
    tx2->get_latitude(&tp2.H);

    double a1, a2, da;
    spherics->Azimuth(cp, tp1, &a1);
    spherics->Azimuth(cp, tp2, &a2);
    spherics->AzimuthDifference(a1, a2, &da);

    double vc = 0.0;
    tx2->get_video_carrier(&vc);
    sad[cpdiDa]  = BCCalcParams.FCalcSrv->GetAntennaDiscrimination(vc, da);
    sad[cpdiDaO] = 0.0;

    sad.Detach();
}


void __fastcall TxAnalyzer::GetInterfZones(ILISBCTxList *ilist, int uw1pos, int uw2pos,
                                        ILISBCTxList *isfn1, ILISBCTxList *isfn2,
                                        LPSAFEARRAY* zone1, LPSAFEARRAY* zone2)
{
    TCOMILISBCTxList list(ilist, true);
    if (ilist == NULL || list.Size == 0)
        throw *(new Exception("Список расчёта пуст"));

    bool wu = list.get_TxUseInCalc(0);
    bool uw1u = list.get_TxUseInCalc(uw1pos);
    bool uw2u = list.get_TxUseInCalc(uw2pos);

    AnsiString msg /*= AnsiString().sprintf("Size = %d, uw1pos = %d, uw2pos = %d\n", list.Size, uw1pos, uw2pos);
    for (int i = 0; i < list.Size; i++)
        msg += (IntToStr(i)+": ("+(list.get_TxUseInCalc(i)==1 ? '+' : '-')+") "+
                AnsiString(TCOMILISBCTx(list.get_Tx(i)).station_name) + '\n');
    ShowMessage(msg)*/;

    AnsiString mess;

    TCOMILISBCTx tx(list.get_Tx(0), true);
    TBCTxType txType = tx.systemcast;

    if (uw2pos)
    {
        if (uw2pos)
            list.set_TxUseInCalc(uw2pos, false), SwitchSfn(list, isfn2, false);
        if (uw1pos)
            list.set_TxUseInCalc(uw1pos, true), SwitchSfn(list, isfn1, true);
        mess = "Розрахунок зон покриття ( завада 1 )...";
    } else {
        if (uw1pos)
            list.set_TxUseInCalc(uw1pos, false), SwitchSfn(list, isfn1, false);
        mess = "Розрахунок зон покриття ( без завади )...";
    }

    GetTxZone(list, zone1, mess);

    if (uw2pos)
    {
        if (uw1pos)
            list.set_TxUseInCalc(uw1pos, false), SwitchSfn(list, isfn1, false);
        if (uw2pos)
            list.set_TxUseInCalc(uw2pos, true), SwitchSfn(list, isfn2, true);
        mess = "Розрахунок зон покриття ( завада 2 )...";
    } else {
        if (uw1pos)
            list.set_TxUseInCalc(uw1pos, true), SwitchSfn(list, isfn1, true);
        mess = "Розрахунок зон покриття ( із завадою )...";
    }
    GetTxZone(list, zone2, mess);




    list.set_TxUseInCalc(uw1pos, uw1u), SwitchSfn(list, isfn1, true);
    list.set_TxUseInCalc(uw2pos, uw2u), SwitchSfn(list, isfn2, true);
    list.set_TxUseInCalc(0, wu);

    /*msg = AnsiString().sprintf("Size = %d, uw1pos = %d, uw2pos = %d\n", list.Size, uw1pos, uw2pos);
    for (int i = 0; i < list.Size; i++)
        msg += (IntToStr(i)+": ("+(list.get_TxUseInCalc(i)==1 ? '+' : '-')+") "+
                AnsiString(TCOMILISBCTx(list.get_Tx(i)).station_name) + '\n');
    ShowMessage(msg)*/;
}

void __fastcall TxAnalyzer::SortTxList(ILISBCTxList* list, String attrib, String dir)
{
    bool sortUp = dir.UpperCase().Pos("DESC") == 0;
    attrib = attrib.UpperCase();
    if (attrib == "WANTED" || attrib == "UNWANTED" )
    {
        typedef std::multimap<double, int, std::less<double> > SortUpMap;
        SortUpMap supm;
        typedef std::multimap<double, int, std::greater<double> > SortDnMap;
        SortDnMap sdnm;

        TCOMILISBCTxList oList(list, true);
        TCOMILISBCTxList aList;
        aList.CreateInstance(CLSID_LISBCTxList);
        for (int i = 0; i < oList.Size; i++)
            CopyTx(aList, oList, i);

        for (int i = 1; i < aList.Size; i++)
            if (sortUp)
                supm.insert(std::pair<double,int>(
                            attrib == "WANTED" ? oList.get_TxWantInterfere(i) : oList.get_TxWantInterfere(i),
                            i));
            else
                sdnm.insert(std::pair<double,int>(
                            attrib == "WANTED" ? oList.get_TxWantInterfere(i) : oList.get_TxUnwantInterfere(i),
                            i));

        oList.Clear();
        CopyTx(oList, aList, 0);

        int i = 0;
        if (sortUp)
            for (SortUpMap::iterator smi = supm.begin(); smi != supm.end(); smi++, i++)
                CopyTx(oList, aList, smi->second);
        else
            for (SortDnMap::iterator smi = sdnm.begin(); smi != sdnm.end(); smi++, i++)
                CopyTx(oList, aList, smi->second);
    }
}
//---------------------------------------------------------------------------

long __fastcall TxAnalyzer::CopyTx(ILISBCTxList *to, ILISBCTxList *from, int idx)
{
    TCOMILISBCTxList fromList(from, true);
    long i = 0;
    to->AddTx(fromList.get_Tx(idx), &i);
    to->set_TxAzimuth(i, fromList.get_TxAzimuth(idx));
    to->set_TxDistance(i, fromList.get_TxDistance(idx));
    to->set_TxShowOnMap(i, fromList.get_TxShowOnMap(idx));
    to->set_TxUnwantedKind(i, fromList.get_TxUnwantedKind(idx));
    to->set_TxUnwantInterfere(i, fromList.get_TxUnwantInterfere(idx));
    to->set_TxUseInCalc(i, fromList.get_TxUseInCalc(idx));
    to->set_TxWantedKind(i, fromList.get_TxWantedKind(idx));
    to->set_TxWantInterfere(i, fromList.get_TxWantInterfere(idx));
    to->set_TxZoneOverlapping(i, fromList.get_TxZoneOverlapping(idx));
    return i;
}
//---------------------------------------------------------------------------

void __fastcall TxAnalyzer::MapToolUsed(TObject *Sender,
      short ToolNum, double X1, double Y1, double X2, double Y2,
      double Distance, VARIANT_BOOL Shift, VARIANT_BOOL Ctrl,
      VARIANT_BOOL *EnableDefault)
{
    static TfrmRelief* frmRelief = NULL;
    switch (ToolNum) {
        case miReliefTool:
            if (!frmRelief)
                frmRelief = new TfrmRelief(Application);
            frmRelief->Caption = "Профіль траси " + dmMain->coordToStr(Y1, 'Y') + ':' + dmMain->coordToStr(X1, 'X')
                                                  + " - "
                                                  + dmMain->coordToStr(Y2, 'Y') + ':' + dmMain->coordToStr(X2, 'X');
            frmRelief->fmProfileView1->RetreiveProfile(X1, Y1, X2, Y2);
            frmRelief->Show();
        break;
        case miDistanceTool:
        break;
    }
}
//---------------------------------------------------------------------------

double __fastcall TxAnalyzer::GetSumE(ILISBCTxList* ilist, double x, double y)
{
    double sum = 0.;
    if (ilist != NULL)
    {
        TCOMILISBCTxList list(ilist, true);
        for (int i = 0; i < list.Size; i++)
        {
            if (list.get_TxUseInCalc(i))
            {
                TCOMILISBCTx tx(list.get_Tx(i), true);
                if (tx.systemcast < ttAllot)
                {
                    char it = '\0';
                    double e = GetE(tx, x, y, &it);
                    sum += pow(10., e / 20.);
                }
            }
        }
        sum = sum > 0 ? 20. * log10(sum) : 0.;
    }
    return sum;
}

TForm * __fastcall TxAnalyzer::MakeNewSelection(int txId, NewSelectionType nsType)
{
    TCOMILISBCTx tx;
    tx.Bind(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))), true);

    TdlgNewSelection* ns = NULL;
    for (int i = 0; i < frmMain->MDIChildCount; i++)
        if ((ns = dynamic_cast<TdlgNewSelection*>(frmMain->MDIChildren[i])) && ns->Tag == nsType) {
            ns->Show();
            break;
        }
    if (!ns)
        ns = new TdlgNewSelection(Application);

    if (tx.systemcast == ttAM)
        ns->chbSelectBrIfic->Checked = true;

    ns->Tag = nsType;
    ns->TxId = txId;
    ns->edtLon->Text = dmMain->coordToStr(ns->lon = tx.longitude, 'X');
    ns->edtLat->Text = dmMain->coordToStr(ns->lat = tx.latitude, 'Y');
    ns->edtMaxRadius->Text = (tx.systemcast == ttAM) ? "10000" : "500";

    if (nsType == nsPlanning)
    {
        ns->Caption = "Підбір частоти/каналу нового передавача";
        ns->chbAdjanced->Checked = true;
        ns->chbImage->Checked = true;
        ns->chbAdjanced->Visible = true;
        ns->chbImage->Visible = true;
        ns->Height = ns->Height + 45;
        ns->chbMaxRadius->Checked = true;
    }

    std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select SD.NAMESITE, SC.CODE SC_CODE, ats.FREQUENCYGRID_ID, ars.DEVIATION, SC.ENUMVAL, A.NUMREGION "
                        "FROM TRANSMITTERS TX "
                            "LEFT OUTER JOIN SYSTEMCAST SC ON (TX.SYSTEMCAST_ID = SC.ID) "
                            "LEFT OUTER JOIN STAND SD ON (TX.STAND_ID = SD.ID) "
                            "LEFT OUTER JOIN ANALOGTELESYSTEM ats ON (tx.TYPESYSTEM = ats.ID) "
                            "LEFT OUTER JOIN ANALOGRADIOSYSTEM ars ON (tx.TYPESYSTEM = ars.ID) "
                            "LEFT OUTER JOIN AREA a ON (sd.AREA_ID = a.ID) "
                           "WHERE TX.ID = :ID ";

    sql->Params->Vars[0]->AsInteger = txId;
    sql->ExecQuery();

    AnsiString asTxName = sql->Fields[0]->AsString + ','+' ' + sql->Fields[1]->AsString;
    if (nsType == nsExpertise)
    {
        asTxName = ((tx.status_code == tsDraft) ?
                        String("[План.] ") :
                        sql->FieldByName("NUMREGION")->AsString + String().sprintf("%04d ", tx.adminid)
                        ) +
                    asTxName + ' ' + GetTxNominalString(tx);
    }
    ns->lblTxName->Caption = String().sprintf(szNewSelectionQuestion, asTxName.c_str());

    if (nsType == nsPlanning)
    {
        std::auto_ptr<TIBSQL> sqlGrid(new TIBSQL(Application));
        sqlGrid->Database = dmMain->dbMain;
        std::auto_ptr<TIBSQL> sqlGridList(new TIBSQL(Application));
        sqlGridList->Database = dmMain->dbMain;

        TBCTxType txType = tx.systemcast;
        switch (txType) {
            case ttUNKNOWN:
                // бросок исключения в начале метода
                break;
            case ttAM:
            case ttFM:
                if (txType == ttFM)
                {
                if (sql->FieldByName("DEVIATION")->AsFloat == 50.0)
                    ns->rgrGrid->ItemIndex = 0;
                else if (sql->FieldByName("DEVIATION")->AsFloat == 75.0)
                    ns->rgrGrid->ItemIndex = 1;
                }
                // в діапазонах I (66 - 74) або II (87.5 - 107.9) МГц

                ns->panFreqGrid->Visible = true;
                ns->panChBlockGrid->Visible = false;
                ns->rgBand->Visible = (txType == ttAM);
                ns->rgrGrid->Items->Strings[0] = FloatToStr(txType == ttFM ? 30. : 4.5);
                ns->rgrGrid->Items->Strings[1] = txType == ttFM ? "100" : "9";
                txType == ttFM ? ns->rgrGridClick(ns->rgrGrid) : ns->rgBandClick(ns->rgBand);
                // нет в базе
                break;
            case ttDAB:
                // перебор будет по таблице блоков
                ns->panFreqGrid->Visible = false;
                ns->panChBlockGrid->Visible = true;
                ns->lblChFrom->Caption = "Блоки з ";

                ns->cbxChFrom->Items->Clear();
                ns->cbxChTo->Items->Clear();

                sqlGrid->SQL->Text = "SELECT ID, NAME, CENTREFREQ  FROM BLOCKDAB order by 3 ";
                sqlGrid->ExecQuery();
                while (!sqlGrid->Eof) {
                    ns->cbxChFrom->Items->AddObject(sqlGrid->Fields[1]->AsString + FormatFloat(" (0.0##)", sqlGrid->Fields[2]->AsDouble), (TObject*)sqlGrid->Fields[0]->AsInteger);
                    ns->cbxChTo->Items->AddObject(sqlGrid->Fields[1]->AsString + FormatFloat(" (0.0##)", sqlGrid->Fields[2]->AsDouble), (TObject*)sqlGrid->Fields[0]->AsInteger);
                    sqlGrid->Next();
                }
                if (ns->cbxChFrom->Items->Count > 0)
                    ns->cbxChFrom->ItemIndex = 0;
                if (ns->cbxChTo->Items->Count > 0)
                    ns->cbxChTo->ItemIndex = ns->cbxChTo->Items->Count - 1;
                break;
            case ttTV:
            case ttDVB:
            case ttFxm :
            case ttCTV:
                ns->panFreqGrid->Visible = false;
                ns->panChBlockGrid->Visible = true;
                ns->lblChFrom->Caption = "Канали з ";

                ns->cbxChannelGrid->Items->Clear();

                if (tx.systemcast == ttDVB) {
                    ns->lblChannelGrid->Visible = true;
                    ns->cbxChannelGrid->Visible = true;
                    sqlGridList->SQL->Text = "SELECT fg.ID, fg.NAME, fg.DESCRIPTION FROM FREQUENCYGRID fg order by 2";
                    sqlGridList->ExecQuery();
                    while (!sqlGridList->Eof) {
                        AnsiString name(sqlGridList->FieldByName("NAME")->AsString);
                        if (!sqlGridList->FieldByName("DESCRIPTION")->IsNull)
                            name = name + " - " + sqlGridList->FieldByName("DESCRIPTION")->AsString;
                        ns->cbxChannelGrid->Items->AddObject(name, (TObject*)sqlGridList->FieldByName("ID")->AsInteger);
                        sqlGridList->Next();
                    }
                    ns->cbxChannelGrid->ItemIndex = ns->cbxChannelGrid->Items->IndexOfObject((TObject*)sql->FieldByName("FREQUENCYGRID_ID")->AsInteger);

                } else {
                    ns->lblChannelGrid->Visible = false;
                    ns->cbxChannelGrid->Visible = false;

                    ns->cbxChannelGrid->Items->AddObject("Default grid", (TObject*)sql->FieldByName("FREQUENCYGRID_ID")->AsInteger);
                    ns->cbxChannelGrid->ItemIndex = 0;
                    ns->cbxChannelGridChange(ns->cbxChannelGrid);
                }
                break;

            default:
                throw *(new Exception("Тип об'єкту не дозволяє провести планування"));
        }
    }

    return ns;
}

String __fastcall TxAnalyzer::GetTxNominalString(ILISBCTx *ptx)
{
    TCOMILISBCTx tx (ptx, true);
    switch (tx.systemcast) {
        case ttTV: case ttDVB:
            return "ТВК " + dmMain->getChannelName(tx.channel_id);
        case ttAM:
            return FormatFloat("#.### kГц", tx.sound_carrier_primary * 1000.);
        case ttFM:
            return FormatFloat("#.# МГц", tx.sound_carrier_primary);
        case ttDAB:
            return "Блок " + dmMain->getDabBlockName(tx.blockcentrefreq);
        case ttCTV:
            return "КТБ";
        case ttAllot:
            {
                TCOMILisBcDigAllot allot;
                OleCheck(tx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot));
                long ch;
                allot->get_channel_id(&ch);
                return dmMain->getChannelName(ch);
            }
        case -1: //ttFXM:
            return FormatFloat("#.### МГц", tx.sound_carrier_primary);
        default:
            return String().sprintf("невідома система (type = %d)", tx.systemcast);
    }
}

void __fastcall TxAnalyzer::Clear()
{
    ClearPlan();
    if (planningTx.IsBound())
        try { planningTx.Bind(NULL); } catch (...) {}
}

void __fastcall TxAnalyzer::ClearPlan()
{
    for (PlanVector::iterator pvi = planVector.begin(); pvi < planVector.end(); pvi++)
        try { pvi->txList->Release(); } catch (...) {}
    planVector.clear();
}

TxAnalyzer::PlanVector::reference __fastcall TxAnalyzer::AddPlanEntry(long id, std::string name,
                                                        int chId, double freq, int maxWant, int maxUnwant)
{
    planVector.push_back();
    PlanVector::reference pvr = planVector.back();
    pvr.id = id;
    pvr.name = name;
    pvr.frequency = freq;
    pvr.channelId = chId;
    pvr.maxWantIdx = maxWant;
    pvr.maxUnwantIdx = maxUnwant;
    TCOMILISBCTxList txList; txList.CreateInstance(CLSID_LISBCTxList);
    pvr.txList = txList;
    pvr.txList->AddRef();

    return pvr;
}

double __fastcall TxAnalyzer::GetAllotEmin(ILISBCTx* tx)
{
    double eMin = 30.0;
    try {
        ILisBcDigAllotPtr allot;
        OleCheck(tx->QueryInterface<ILisBcDigAllot>(&allot));
        double freq;
        OLECHECK(allot->get_freq(&freq));
        IEminCalcPtr eMinCalc;
        OLECHECK(BCCalcParams.FCalcSrv->QueryInterface<IEminCalc>(&eMinCalc));
        eMin = eMinCalc->GetEmin(freq);
    } catch (Exception &e) {
        e.Message = "Ошибка вызова диалога выбора Emin:\n" + e.Message;
        Application->ShowException(&e);
    }
    return eMin;
}

void __fastcall TxAnalyzer::SwitchSfn(ILISBCTxList* ilist, ILISBCTxList* isfn, bool on)
{
    if (ilist == NULL || isfn == NULL)
        return;
        
    TCOMILISBCTxList list (ilist, true);
    TCOMILISBCTxList sfn (isfn, true);
    for (int j = 0; j < sfn.Size; j++)
        for (int i = 0; i < list.Size; i++)
            if (list.get_Tx(i) == sfn.get_Tx(j))
            {
                list.set_TxUseInCalc(i, on ? sfn.get_TxUseInCalc(j) : false);
                break;
            }
}

float __fastcall TxAnalyzer::GetGndCond(float x, float y)
{
    CheckIdwm();                                                                   
    float gc = 0.;
    HrCheck(gndInfoServ->GetCond(x, y, &gc));
    return gc;
}

long __fastcall TxAnalyzer::GetNoiseZone(float x, float y)
{
    CheckIdwm();
    long nz = 0.;
    HrCheck(gndInfoServ->GetNoiseZone(x, y, &nz));
    return nz;
}

void __fastcall TxAnalyzer::CheckIdwm()
{
    if (!geoSphServ.IsBound())
        HrCheck(geoSphServ.CreateInstance(CLSID_CoLisIdwm), __FUNC__"(): инициализация CoLisIdwm");
    if (!gndInfoServ.IsBound())
        HrCheck(geoSphServ->QueryInterface(IID_ILisGndInfo, (void**)&gndInfoServ), __FUNC__"(): запрос интерфейса ILisGndInfo");
    if (!idwmParamServ.IsBound())
        HrCheck(geoSphServ->QueryInterface(IID_ILisIdwmParam, (void**)&idwmParamServ), __FUNC__"(): запрос интерфейса ILisIdwmParam");
}

TBCTxType __fastcall TxAnalyzer::GetDiapason(ILISBCTxList *ptxList)
{
    TCOMILISBCTxList txList(ptxList, true);
    if (txList.Size == 0)
        throw *(new Exception(__FUNC__"(): список пуст"));
    TCOMILISBCTx tx(txList.get_Tx(0), true);
    return tx.systemcast;
}

void __fastcall TxAnalyzer::CheckLfMfCalc()
{
    CheckIdwm();
    if (!p1147_2.IsBound())
        HrCheck(p1147_2.CreateInstance(CLSID_CoP1147), __FUNC__"(): инициализация CoP1147");
    if (!p368_7.IsBound())
        HrCheck(p368_7.CreateInstance(CLSID_CoP368_7), __FUNC__"(): инициализация CoP368_7");
    if (!lfmfEmin.IsBound())
        HrCheck(lfmfEmin.CreateInstance(CLSID_CoLisLFMFEminCalc), __FUNC__"(): инициализация CoLisLFMFEminCalc");
    if (!lfmfFs.IsBound())
    {
        HrCheck(lfmfFs.CreateInstance(CLSID_CoLisLFMFFieldStrength), __FUNC__"(): инициализация CoLisLFMFFadingCalc");
        lfmfFs->set_LisIDWM(geoSphServ);
        lfmfFs->set_p_1147_2(p1147_2);
        lfmfFs->set_p_368_7(p368_7);
    }
    if (!lfmfCalc.IsBound())
    {
        HrCheck(lfmfCalc.CreateInstance(CLSID_CoLisLFMFZoneCalc), __FUNC__"(): инициализация CoLisLFMFZoneCalc");
        lfmfCalc->set_LisIDWM(geoSphServ);
        lfmfCalc->set_p_1147_2(p1147_2);
        lfmfCalc->set_p_368_7(p368_7);
        lfmfCalc->set_Fault(0.2);
        /*
        lfmfCalc->set_DayNight(1);           
        lfmfCalc->set_FirstDistance(5000.);
        lfmfCalc->set_StepAngle(1);
        lfmfCalc->set_StepDistance(2500.);
        */
    }
}

void __fastcall TxAnalyzer::CalcDuel(ILISBCTx* tx1, ILISBCTx* tx2, TDuelResult2* dr2, TPointDuelResult* pdr)
{
    TBCTxType tt = ttUNKNOWN;
    tx1->get_systemcast(&tt);
    if (tt == ttAM)
    {
        CheckLfMfCalc();

        ILISBCTxListPtr txList; HrCheck(txList.CreateInstance(CLSID_LISBCTxList), __FUNC__"(): создание списка");
        long idx = 0;
        txList->AddTx(tx1, &idx);
        txList->set_TxUseInCalc(idx, true);
        txList->AddTx(tx2, &idx);
        txList->set_TxUseInCalc(idx, true);
        GetInterfZones(txList, 1, 0, NULL, NULL, &(dr2->Tx1_NoiseLimited), &(dr2->Tx1_InterferenceLimited));

        if (dr2->Tx1_NoiseLimited == NULL && dr2->Tx1_InterferenceLimited == NULL)
            throw *(new Exception(__FUNC__"() - пустые зоны прд1"));

        txList->Clear();
        txList->AddTx(tx2, &idx);
        txList->set_TxUseInCalc(idx, true);
        txList->AddTx(tx1, &idx);
        txList->set_TxUseInCalc(idx, true);
        GetInterfZones(txList, 1, 0, NULL, NULL, &(dr2->Tx2_NoiseLimited), &(dr2->Tx2_InterferenceLimited));

        if (dr2->Tx2_NoiseLimited == NULL && dr2->Tx2_InterferenceLimited == NULL)
            throw *(new Exception(__FUNC__"() -  пустые зоны прд2"));

        if (dr2->Tx1_NoiseLimited->rgsabound[0].cElements != 36 || dr2->Tx1_NoiseLimited->rgsabound[0].cElements != 36 ||
            dr2->Tx2_InterferenceLimited->rgsabound[0].cElements != 36 || dr2->Tx2_InterferenceLimited->rgsabound[0].cElements != 36)
            throw *(new Exception(__FUNC__"() - Не того размера массивы зон"));

        setmem(pdr, sizeof(TPointDuelResult)*16, '\0');

        double lon1=0.; double lat1=0.; double lon2=0.; double lat2=0.;
        tx1->get_latitude(&lat1);
        tx1->get_longitude(&lon1);
        tx2->get_latitude(&lat2);
        tx2->get_longitude(&lon2);
        double az = geoSphServ->GetAzimuth(lon1, lat1, lon2, lat2);
        pdr[4*1].azimuth = az;
        pdr[4*1].radius = GetVal(dr2->Tx1_NoiseLimited, az);
        az += 180.; while (az >=360.) az -= 360.;
        pdr[4*0].azimuth = az;
        pdr[4*0].radius = GetVal(dr2->Tx1_NoiseLimited, az);
        az = geoSphServ->GetAzimuth(lon2, lat2, lon1, lat1);
        pdr[4*2].azimuth = az;
        pdr[4*2].radius = GetVal(dr2->Tx2_NoiseLimited, az);
        az += 180.; while (az >=360.) az -= 360.;
        pdr[4*3].azimuth = az;
        pdr[4*3].radius = GetVal(dr2->Tx2_NoiseLimited, az);

        long sfnIdW = 0, sfnIdUw = 0;
        tx1->get_identifiersfn(&sfnIdW);
        tx2->get_identifiersfn(&sfnIdUw);

        ILisBcLfMfPtr uTx1, uTx2;
        OleCheck(tx1->QueryInterface(IID_ILisBcLfMf, (void**)&uTx1));
        OleCheck(tx2->QueryInterface(IID_ILisBcLfMf, (void**)&uTx2));

        long system1 = uTx1->lfmf_system;
        long system2 = uTx2->lfmf_system;


        TPointDuelResult*p = pdr;
        for (int i = 1; i <=4; i++, p+=4)
        {
            float lon = 0.; float lat = 0.;
            geoSphServ->GetPoint( i<=2 ? lon1 : lon2, i<=2 ? lat1 : lat2, p->azimuth, p->radius, &lon, &lat);
            p->geoPoint.L = lon;
            p->geoPoint.H = lat;
            p->emin = GetEmin(i<=2 ? tx1 : tx2);
            char type = '?';
            p->eInt = GetE(i<=2 ? tx2 : tx1, lon, lat, &type);
            p->aPR = GetPr(i<=2 ? tx1 : tx2, i<=2 ? tx2 : tx1);
            if((i<=2 && system1 == 1) || (i>2 && system2 == 1))
            {
                if(sfnIdW == sfnIdUw && sfnIdW != NULL)
                {
                    p->aPR -= 19;
                }
                else
                {
                    if(type == 'G')
                        p->aPR += 3;
                }
            }
            p->aDiscr = 0.;
            p->eUsable = 10 * log10(pow(10, (p->eInt + p->aPR) / 10.) + pow(10, (p->emin / 10.)));//p->eInt + p->aPR;
            p->intType = type;
            p->aPolar = plVER;
        }

    } else {

        //HrCheck(BCCalcParams.FCalcSrv->SetTxListServer(txList));
        HrCheck(BCCalcParams.FCalcSrv->CalcDuel2(tx1, tx2, dr2), __FUNC__"() - вызов CalcDuel2()");
     //   HrCheck(BCCalcParams.FCalcSrv->CalcDuel3(tx1, tx2, pdra), __FUNC__"() - вызов CalcDuel3()");
        CalcDuel3(tx1, tx2, *dr2, pdr);
    }
}

double __fastcall TxAnalyzer::GetEmin(ILISBCTx *tx)
{
    TBCTxType tt = ttUNKNOWN;
    tx->get_systemcast(&tt);
    if (tt == ttAM)
    {
        CheckLfMfCalc();
        double emin;
        HrCheck(lfmfEmin->Get_Emin(tx, &emin), __FUNC__);
        return emin;
    } else if (tt == ttAllot)
        return GetAllotEmin(tx);
    else
        return BCCalcParams.FCalcSrv.GetEmin(tx);
}

double __fastcall TxAnalyzer::GetE(ILISBCTx* tx, double lon, double lat, char* type)
{
    TBCTxType tt = ttUNKNOWN;
    tx->get_systemcast(&tt);
    if (tt == ttAM)
    {
        CheckLfMfCalc();
        double e_g;
        double e_s;
        HrCheck(lfmfFs->Get_FieldStrengthByPoint(tx, lon, lat, time0+(lon/360.), &e_g, &e_s), __FUNC__"()");
        if(e_g > (e_s + 3))
        {
            *type = 'G';
            return e_g;
        }
        else
        {
            *type = 'S';
            return e_s;
        }
    }
    else
    {
        double et = BCCalcParams.FCalcSrv->GetFieldStrength(tx, lon, lat, 1);
        double ep = BCCalcParams.FCalcSrv->GetFieldStrength(tx, lon, lat, 50);
        if (ep > et)
        {
            if (type)
                *type = 'C';
            return ep;
        } else {
            if (type)
                *type = 'T';
            return et;
        }
    }
}

double __fastcall TxAnalyzer::GetE50(ILISBCTx* tx, double lon, double lat, char* type)
{
    TBCTxType tt = ttUNKNOWN;
    tx->get_systemcast(&tt);
    if (tt == ttAM)
    {
        *type = ' ';
        return 0.0;

    }
    else
    {
        double ep = BCCalcParams.FCalcSrv->GetFieldStrength(tx, lon, lat, 50);
        *type = 'C';
        return ep;
    }
}

double __fastcall TxAnalyzer::GetPr(ILISBCTx* tx1, ILISBCTx* tx2)
{
    TBCTxType tt = ttUNKNOWN;
    tx1->get_systemcast(&tt);
    if (tt == ttAM)
    {
        CheckLfMfCalc();
        double pr;
        HrCheck(lfmfEmin->Get_Pr(tx1, tx2, &pr), __FUNC__"()");
        return pr;
    }
    else
    {
        double pr_c = 0.; double pr_t = 0.;
        HrCheck(BCCalcParams.FCalcSrv->GetProtectRatio(tx1, tx2, &pr_c, &pr_t), __FUNC__"()");
        return min(pr_c, pr_t);
    }
}

double __fastcall TxAnalyzer::GetVal(LPSAFEARRAY zone, double az)
{
    long az1 = long(az / 10);
    long az2 = long(az / 10) + 1;
    if (az2 == 36) az2 = 0;
    double *dzone = (double*)zone->pvData;
    return dzone[az1] + (dzone[az2]-dzone[az1])*(az/10.-az1)/(az2-az1);
}

double __fastcall TxAnalyzer::GetZoneByDir(ILISBCTx* ptx, ILISBCTxList* ptxl, double az)
{
    ILISBCTxPtr tx(ptx, true);
    TBCTxType sc = ttUNKNOWN;
    HrCheck(tx->get_systemcast(&sc), __FUNC__"()");
    double dist = 0;
    if (sc == ttAM)
    {
        CheckLfMfCalc();
        HrCheck(lfmfCalc->GetZoneByAzimuth(ptx, ptxl, time0, az, &dist));
    } else
        throw *(new Exception(__FUNC__"() - тип передатика не поддерживается"));
    return dist;
}

void __fastcall TxAnalyzer::GetTxZone(ILISBCTx* ptx, LPSAFEARRAY* zone)
{
    ILISBCTxPtr tx(ptx, true);
    TBCTxType sc = ttUNKNOWN;
    HrCheck(tx->get_systemcast(&sc), __FUNC__"()");
    TempCursor(crHourGlass);
    if (sc == ttAM)
    {
        CheckLfMfCalc();
        lfmfCalc->set_Progress(NULL);
        HrCheck(lfmfCalc->GetZone(ptx, NULL, time0, zone), __FUNC__"() - вызов lfmfCalc->GetZone()");
    } else
        HrCheck(BCCalcParams.FCalcSrv.GetZone_NoiseLimited(ptx, BCCalcParams.degreeStep, zone));
}

double __fastcall TxAnalyzer::GetGndCond(double x, double y)
{
    CheckIdwm();
    float gc = 0.;
    HrCheck(gndInfoServ->GetCond(x, y, &gc),__FUNC__"()");
    return gc;
}

double __fastcall TxAnalyzer::GetUsableE(ILISBCTxList* plist, double lon, double lat)
{                                             
    if (plist == NULL)
        throw *(new Exception(__FUNC__"() - tx list is NULL"));
    TCOMILISBCTxList txList(plist, true);
    if (txList.Size == 0)
        throw *(new Exception(__FUNC__"() - tx list empty"));
    TCOMILISBCTx tx(txList.get_Tx(0), true);
    if (tx.systemcast == ttAM)
    {
        CheckLfMfCalc();
        double e = -999.;
        HrCheck(lfmfCalc->GetEUseByPoint(tx, plist, lon, lat, time0, &e));
        return e;
    } else {
        BCCalcParams.FCalcSrv.SetTxListServer(plist);
        return BCCalcParams.FCalcSrv.GetUsableFieldStrength(lon, lat);
    }
}

double __fastcall TxAnalyzer::GetZoneByAzm(ILISBCTxList* plist, double azm)
{
    if (plist == NULL)
        throw *(new Exception(__FUNC__"() - tx list is NULL"));
    TCOMILISBCTxList txList(plist, true);
    if (txList.Size == 0)
        throw *(new Exception(__FUNC__"() - tx list empty"));
    TCOMILISBCTx tx;
    txList->get_Tx(0, &tx);
    tx->AddRef();
    if (tx.systemcast == ttAM)
    {
        CheckLfMfCalc();
        double d = 0.;
        HrCheck(lfmfCalc->GetZoneByAzimuth(tx, plist, time0, azm, &d));
        return d;
    } else {
        BCCalcParams.FCalcSrv.SetTxListServer(plist);
        return BCCalcParams.FCalcSrv.GetMaxRadius(1., azm);
    }
}

double __fastcall TxAnalyzer::GetAzimuth(double lon1, double lat1, double lon2, double lat2)
{
    CheckIdwm();
    float az = 0.;
    geoSphServ->GetAzimuth(lon1, lat1, lon2, lat2, &az);
    return az;
}

double __fastcall TxAnalyzer::GetDistance(double lon1, double lat1, double lon2, double lat2)
{
    CheckIdwm();
    float az = 0.;
    geoSphServ->GetDistance(lon1, lat1, lon2, lat2, &az);
    return az;
}

void __fastcall TxAnalyzer::GetPoint(double lon1, double lat1, double az, double dist, double* lon2, double* lat2)
{
    CheckIdwm();
    float lat, lon;
    geoSphServ->GetPoint(lon1, lat1, az, dist, &lon, &lat);
    *lon2 = lon;
    *lat2 = lat;
}

LPSAFEARRAY __fastcall TxAnalyzer::GetEtalonZone(ILISBCTx* ptx, long mode)
{
    TCOMILISBCTx tx(ptx, true);
    long txId = tx.id;
    EtalonMap::iterator ezi = etalonMap.find(ZoneKey(txId, mode));
    if (ezi == etalonMap.end())
    {
        //try to extract from db
        std::auto_ptr<TIBSQL> sql(new TIBSQL(dmMain));
        sql->Transaction = dmMain->trMain;
        sql->SQL->Text = "select ETALON_ZONE from LFMF_OPER where STA_ID = "+IntToStr(txId)+
                        " and DAYNIGHT = '"+(mode == 0 ? "HJ" : "HN")+'\'';
        sql->ExecQuery();
        if (!sql->Eof && !sql->Fields[0]->IsNull)
        {
            SAFEARRAYBOUND sab[1];
            sab[0].cElements = 36;
            sab[0].lLbound = 0;
            LPSAFEARRAY arr = SafeArrayCreate(VT_R8, 1, sab);

            std::auto_ptr<TMemoryStream> ms(new TMemoryStream());
            sql->Fields[0]->SaveToStream (ms.get());
            ms->Position = 0;
            ms->Read(arr->pvData, sab[0].cElements * sizeof(double));

            ezi = etalonMap.insert(std::pair<ZoneKey, LPSAFEARRAY>(ZoneKey(txId,mode),arr)).first;
        }
        else
        {
            //calculate
            TempVal<double> tempTime(time0);
            if (tx.systemcast == ttAM)
            {
                ILisBcLfMfPtr lfmf;
                tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf);
                if (lfmf.IsBound())
                    tempTime.SetVal(lfmf->is_day ? 13./24. : 1./24.);
            }

            LPSAFEARRAY arr;
            GetTxZone(ptx, &arr);

            tempTime.Reset();
            
            //save
            std::auto_ptr<TMemoryStream> ms(new TMemoryStream());
            ms->Write((void*)arr->pvData, arr->rgsabound[0].cElements * sizeof(double));
            ms->Position = 0;

            sql->Close();
            sql->SQL->Text = "update LFMF_OPER set ETALON_ZONE = :ETALON_ZONE where STA_ID = "+IntToStr(txId)+
                            " and DAYNIGHT = '"+(mode == 0 ? "HJ" : "HN")+'\'';
            sql->Params->Vars[0]->LoadFromStream(ms.get());

            sql->ExecQuery();
            sql->Transaction->CommitRetaining();

            ezi = etalonMap.insert(std::pair<ZoneKey, LPSAFEARRAY>(ZoneKey(txId,mode),arr)).first;
        }
    }
    LPSAFEARRAY arrret;
    SafeArrayCopy(ezi->second, &arrret);
    return arrret;

}

void __fastcall TxAnalyzer::DropEtalonZone(ILISBCTx* ptx, long mode)
{
    TCOMILISBCTx tx(ptx, true);
    long txId = tx.id;
    EtalonMap::iterator ezi = etalonMap.find(ZoneKey(txId, mode));
    if (ezi != etalonMap.end())
    {
        SafeArrayDestroy(ezi->second);
        etalonMap.erase(ezi);
    }
    std::auto_ptr<TIBSQL> sql(new TIBSQL(dmMain));
    sql->Transaction = dmMain->trMain;
    sql->SQL->Text = "update LFMF_OPER set ETALON_ZONE = null where STA_ID = "+IntToStr(txId)+
                    " and DAYNIGHT = '"+(mode == 0 ? "HJ" : "HN")+'\'';
    sql->ExecQuery();
    sql->Transaction->CommitRetaining();
}

LPSAFEARRAY __fastcall TxAnalyzer::GetCoverage(ILISBCTx* tx, double a1, double a2, double da,
                                                             double r1, double r2, double dr)
{
    CheckIdwm();
    if (a2 < a1)
    {
        double d = a1;
        a1 = a2;
        a2 = d;
    }
    if (r2 < r1)
    {
        double d = r1;
        r1 = r2;
        r2 = d;
    }

    int adim = floor((a2 - a1 + 0.5) / da);
    int rdim = floor((r2 - r1 + 0.5) / dr);

    SAFEARRAYBOUND bounds[2];
    bounds[0].lLbound = 0;
    bounds[0].cElements = adim;
    bounds[1].lLbound = 0;
    bounds[1].cElements = rdim;

    LPSAFEARRAY res = SafeArrayCreate(VT_R8, 2, bounds);

    double lon, lat;
    tx->get_longitude(&lon);
    tx->get_latitude(&lat);
    long idx[2];

    // "обнулим", ибо процесс м.б. прерван
    double e = -999.;
    for (int i = 0; i < adim; i++)
        for (int j = 0; j < rdim; j++)
            idx[0] = i, idx[1] = j, SafeArrayPutElement(res, idx, &e);

    bool interrupted = false;
    double a = a1;
    for (int i = 0; i < adim && !interrupted; i++)
    {
        idx[0] = i;
        double r = r2;
        for (int j = 0; j < rdim && !interrupted; j++)
        {
            char t = '\'';
            float lon1 = 0.; float lat1 = 0.;
            geoSphServ->GetPoint(lon, lat, a, r, &lon1, &lat1);
            double e = GetE50(tx, lon1, lat1, &t);

            interrupted = DoProgress((1. * rdim * i + j) / (adim * rdim) * 100);

            idx[1] = j;
            SafeArrayPutElement(res, idx, &e);
            r -= dr;
        }
        a += da;
    }
    return res;
}

void __fastcall TxAnalyzer::CalcDuel3(ILISBCTx *tx0, ILISBCTx *tx1, TDuelResult2 duel_result, TPointDuelResult *pointDuelResult)
{
    double az_to_want;
    double az_to_unwant;
    double az;
    double lon, lat, lon0, lat0, lon1, lat1;
    double d[4];
    //TControlPointResult cpr;
    TControlPointCalcResult cpr;
    tx0->get_longitude(&lon0);
    tx0->get_latitude(&lat0);
    tx1->get_longitude(&lon1);
    tx1->get_latitude(&lat1);

    TBCPolarization pol0, pol1;
    tx0->get_polarization(&pol0);
    tx1->get_polarization(&pol1);

    //  Теперь расчитываем азимуты в градусах
    long num = 0;
    if (SafeArrayGetUBound(duel_result.Tx1_NoiseLimited, 1, &num) == S_OK)
        num++;
    if (duel_result.Tx1_NoiseLimited && duel_result.Tx1_InterferenceLimited &&
        duel_result.Tx2_NoiseLimited && duel_result.Tx2_InterferenceLimited)
    {

        double *n1 = (double*)duel_result.Tx1_NoiseLimited->pvData;
        double *i1 = (double*)duel_result.Tx1_InterferenceLimited->pvData;
        double *n2 = (double*)duel_result.Tx2_NoiseLimited->pvData;
        double *i2 = (double*)duel_result.Tx2_InterferenceLimited->pvData;
   
        if(lat1 == lat0 && lon1 == lon0) {


                double min = 1;
                az = 0;
                double ratio1, ratio2;
                for (int i = 0; i < num; i++) {
                    ratio1 = i1[i] / n1[i];
                    ratio2 = i2[i] / n2[i];
                    if (ratio1 < min || ratio2 < min)
                    {
                        min = ratio1 < ratio2 ? ratio1 : ratio2;
                        az = i;
                    }

                }
                az_to_unwant = az * 10;
                if(az_to_unwant > 180)
                    az_to_want = az_to_unwant - 180;
                else
                    az_to_want = az_to_unwant + 180;
        }
        else
        {
            az_to_unwant = GetAzimuth(lon0, lat0, lon1, lat1);
            az_to_want = GetAzimuth(lon1, lat1, lon0, lat0);
        }

        int index = az_to_want / 10;
        d[0] = n1[index];
        d[2] = n2[index];
        index = az_to_unwant / 10;
        d[1] = n1[index];
        d[3] = n2[index];
    //   Расчитываем расстояния до границы зон:

    //            tx0(wanted)                  tx1(unwanted)
    //  d1 --------x-------- d2        d3 ------x------ d4

        for (int i = 0; i < 16; i++)
            pointDuelResult[i].radius = d[i/4];

    /*
      Расчитываем напряж. поля помехи в точках 1,2,3,4. Если радиус зоны
      меньше 3 км, то напряженность расчитываем в точке установки передатчика.

      Изменения 05.05.04:
        1. напряженность по-любому расчитываем на границе зоны
        2. если зона меньше километра - делаем ее равной 1 км.
    */

        if (!TxAnalyzer::FSpherics.IsBound())
            TxAnalyzer::FSpherics.CreateInstance(CLSID_RSASpherics);
        double _MIN_DISTANCE = 1;  // минимальная допустимая дистанция (км.) для радиуса зоны
        
        TPointDuelResult *pdr = pointDuelResult;

        //  GetNextCoordDeg(az_to_want, d1, lon, lat);
        // e1 = BCCalcParams.FCalcSrv.GetEControlPoint(tx0, tx1, lon, lat, @cpr);

        for (int pointNo = 0; pointNo < 4; pointNo++)
        {
            ILISBCTx *txA, *txB;
            if (pointNo == 0 || pointNo == 1)
            {
                txA = tx0; txB = tx1;
                lon = lon0; lat = lat0;
            } else {
                txA = tx1; txB = tx0;
                lon = lon1; lat = lat1;
            }

            if (d[pointNo] < _MIN_DISTANCE)
                d[pointNo] = _MIN_DISTANCE;
            TRSAGeoPoint pointA, pointB;
            pointA.H = lat;
            pointA.L = lon;
            char type = '?';
            double azm = (pointNo == 0 || pointNo == 2) ? az_to_want : az_to_unwant;
            FSpherics.PolarToGeo(d[pointNo], azm, pointA, &pointB);
            lat = pointB.H;
            lon = pointB.L;
            // for a case of mixed polarization do this two times per tx
            for (int ipol0 = 0; ipol0 < 2; ipol0++)
            {
                if (pol0 == plMIX)
                    tx0->set_polarization(ipol0 == 0 ? plHOR : plVER);
                try {
                    //the same is for tx1
                    for (int ipol1 = 0; ipol1 < 2; ipol1++)
                    {
                        if (pol1 == plMIX)
                            tx1->set_polarization(ipol1 == 0 ? plHOR : plVER);
                        try {
                            BCCalcParams.FCalcSrv.GetFieldStrengthControlPoint(txA, txB, lon, lat, &cpr);
                            //pdr->eInt = GetE(txB, lon, lat, &type);   //2012-10-09 - different results when int type changes during calculation
                            pdr->eInt = (cpr.interf_type == itCont) ? cpr.e_50 : cpr.e_1;
                            pdr->aDiscr = cpr.ant_discrimination;
                            pdr->aPolar = NaN;//cpr.a_polar;
                            pdr->intType = cpr.interf_type;
                            pdr->emin = BCCalcParams.FCalcSrv.GetEmin(txA);
                            pdr->azimuth = azm;
                            pdr->aPR = (cpr.interf_type == itCont) ? cpr.pr_continuous : cpr.pr_tropospheric;
                            pdr->eUsable = pdr->eInt + pdr->aPR + pdr->aDiscr; //cpr.e_usable;
                            pdr->geoPoint.H = lat;
                            pdr->geoPoint.L = lon;
                            pdr++;
                        } __finally {
                            tx1->set_polarization(pol1);
                        }
                    }
                } __finally {
                    tx0->set_polarization(pol0);
                }
            }
        }
        tx0->invalidate();
        tx1->invalidate();
    }
}




