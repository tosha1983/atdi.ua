//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uAllotmentForm.h"
#include "FormProvider.h"
#include "TxBroker.h"
#include "uParams.h"
#include "uMainForm.h"
#include "uNewSelection.h"
#include "uSelection.h"
#include "uAnalyzer.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "NumericEdit"
#pragma resource "*.dfm"

#include <IBSQL.hpp>
#include <memory>
#include <values.h>

__fastcall TfrmAllotment::TfrmAllotment(TComponent* Owner)
    : TForm(Owner)
{
    throw *(new Exception("TfrmAllotment(TComponent* Owner) - нельзя"));
}
//---------------------------------------------------------------------------
__fastcall TfrmAllotment::TfrmAllotment(TComponent* Owner, ILISBCTx *iTx)
    : TForm(Owner)
{
    pcRemark->ActivePageIndex = 0;
    edtName->Font->Style = edtName->Font->Style << fsBold;

    cbxRrc06Code->Items->Clear();
    cbxRrc06Code->Items->Add("3 - Виділення");
    cbxRrc06Code->Items->Add("4 - Виділення (+ОЧМ)");
    cbxRrc06Code->Items->Add("5 - Виділення (-ОЧМ)");

    cbxPol->Items->Clear();
    cbxPol->Items->Add("H");
    cbxPol->Items->Add("V");
    cbxPol->Items->Add("M");
    cbxPol->Items->Add("U");

    grdContour->Cells[0][0] = "Довгота";
    grdContour->Cells[1][0] = "Широта";


    mapForm = new TForm(panMap);
    mapForm->Parent = panMap;
    mapForm->ParentWindow = panMap;
    mapForm->Align = alClient;
    mapForm->Visible = true;
    mapForm->BorderStyle = bsNone;
    mapForm->OnPaint = FormPaint;

    HrCheck(iTx->QueryInterface(IID_ILisBcDigAllot, (void**)&allot));

    HrCheck(allot->get_notice_type(&dab_dvb));

    if (dab_dvb == WideString(L"GS2") || dab_dvb == WideString(L"DS2"))
        lblDabDvb->Caption = "T-DAB";
    else if (dab_dvb == WideString(L"GT2") || dab_dvb == WideString(L"DT2"))
        lblDabDvb->Caption = "DVB-T";
    else
    {
        Close();
        throw *(new Exception("Неизвестный тип выделения: " + AnsiString(dab_dvb)));
    }

    tx.Bind(iTx, true);

    txtId->Caption = IntToStr(tx.id);

    cbxRpc->Items->Clear();
    cbxRn->Items->Clear();
    cbxSm->Items->Clear();
    cbxChanBlock->Items->Clear();
    chn_blk_ids.clear();
    freqs.clear();

    std::auto_ptr<TIBSQL> sql (new TIBSQL(this));
    sql->Database = dmMain->dbMain;

    if (dab_dvb == WideString(L"GS2") || dab_dvb == WideString(L"DS2"))
    {
        cbxRpc->Items->AddObject("RPC4", (TObject*)rpc4);
        cbxRpc->Items->AddObject("RPC5", (TObject*)rpc5);

        cbxRn->Items->AddObject("RN5", (TObject*)rn5);
        cbxRn->Items->AddObject("RN6", (TObject*)rn6);

        cbxSm->Items->Add("1");
        cbxSm->Items->Add("2");
        cbxSm->Items->Add("3");

        //cbxRn->Visible = false;
        //lblRn->Visible = false;

        lblChanBlock->Caption = "Частотний блок";

        sql->SQL->Text = "select b.ID, b.NAME, b.CENTREFREQ as FREQ from blockdab b order by b.ID";

    } else {

        cbxRpc->Items->AddObject("RPC1", (TObject*)rpc1);
        cbxRpc->Items->AddObject("RPC2", (TObject*)rpc2);
        cbxRpc->Items->AddObject("RPC3", (TObject*)rpc3);

        cbxRn->Items->AddObject("RN1", (TObject*)rn1);
        cbxRn->Items->AddObject("RN2", (TObject*)rn2);
        cbxRn->Items->AddObject("RN3", (TObject*)rn3);
        cbxRn->Items->AddObject("RN4", (TObject*)rn4);

        cbxSm->Items->Add("N");
        cbxSm->Items->Add("S");

        lblChanBlock->Caption = "Канал";

        sql->SQL->Text = " select C.ID, C.NAMECHANNEL || '  (' || F.NAME || ') ' as NAME, (C.FREQFROM + C.FREQTO) / 2 as FREQ "
                        " from CHANNELS C "
                        " left outer join FREQUENCYGRID F on (C.FREQUENCYGRID_ID = F.ID) "
                        /*" where (F.NAME LIKE 'B%' or F.NAME LIKE 'D%' or F.NAME LIKE 'UHF') and F.NAME <> 'DAB' "*/
                        " order by C.ID ";
    }

    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        cbxChanBlock->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        chn_blk_ids.push_back(sql->Fields[0]->AsInteger);
        freqs.push_back(sql->Fields[2]->AsDouble);
    }

    sql->Close();
    sql->SQL->Text = " select CODE from COUNTRY order by CODE ";
    cbxCountry->Items->Clear();
    cbxGeoArea->Items->Clear();
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        cbxCountry->Items->Add(sql->Fields[0]->AsString);
        cbxGeoArea->Items->Add(sql->Fields[0]->AsString);
    }

    cd.SetWc(mapForm);
    cd.contours = &contoursData;

    popupMenu = new TPopupMenu(this);

    ShowData();
}

void __fastcall TfrmAllotment::actOkExecute(TObject *Sender)
{
    if (actSave->Enabled)
        actSaveExecute(Sender);
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmAllotment::actSaveExecute(TObject *Sender)
{
    if (GetDbSection() != tsDraft)
    {
        actLoadExecute(Sender);
        throw *(new Exception("Изменить выделение можно только в Предбазе"));
    }

    HrCheck(tx.save());
    std::auto_ptr<TStringList> sl(new TStringList);
    dmMain->GetList("select ID, 'Emin = ' || cast(Emin as varchar(6)) || ' : ' || NOTE from DIG_ALLOT_ZONE "
                        " where ALLOT_ID = " + IntToStr(tx.id) + "ORDER BY ID ", sl.get());
    if (sl->Count > 0
    && MessageBox(NULL, "Характеристики выделения изменены.\nУдалить все его контуры помех?", "Вопрос, однако",
    MB_ICONQUESTION | MB_YESNO) == IDYES)
        dmMain->DelAllotZones(tx.id);
    FormProvider.UpdateTransmitters(tx.id);
}
//---------------------------------------------------------------------------
void __fastcall TfrmAllotment::actLoadExecute(TObject *Sender)
{
    HrCheck(tx.invalidate());
    ShowData();
}
//---------------------------------------------------------------------------
void __fastcall TfrmAllotment::actCloseExecute(TObject *Sender)
{
    if (actSave->Enabled && GetDbSection() != tsDraft)
        tx.invalidate();
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmAllotment::actSaveUpdate(TObject *Sender)
{
    bool enable = tx.IsBound() && tx.data_changes;
    actSave->Enabled = enable;
    actLoad->Enabled = enable;
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::ShowData()
{
    WideString ws;
    HrCheck(allot->get_adm_ref_id(&ws));
    edtItuId->Text = AnsiString(ws);

    ws.Empty();
    allot->get_allot_name(&ws);
    edtName->Text = ws; ws.Empty();
    long l;
    allot->get_adm_id(&l);
    edtAdmin->Text = dmMain->getCountryCode(l);
    allot->get_ctry(&ws);
    cbxCountry->ItemIndex = cbxCountry->Items->IndexOf(AnsiString(ws));
    ws.Empty();
    allot->get_geo_area(&ws);
    cbxGeoArea->ItemIndex = cbxGeoArea->Items->IndexOf(AnsiString(ws));
    ws.Empty();

    WideString rpc;
    allot->get_ref_plan_cfg(&rpc);
    cbxRpc->ItemIndex = cbxRpc->Items->IndexOf(rpc);
    WideString  rn;
    allot->get_typ_ref_netwk(&rn);
    cbxRn->ItemIndex = cbxRn->Items->IndexOf(rn);

    double d;
    allot->get_freq(&d);
    edtFreq->OldValue = FormatFloat("##0.### МГц", d);
    allot->get_offset(&l);
    edtFreqOffset->OldValue = AnsiString().sprintf("%d кГц", l);

    char c = 0;
    allot->get_polar(&c);
    cbxPol->ItemIndex = cbxPol->Items->IndexOf(AnsiString(&c, 1));
    allot->get_spect_mask(&c);
    cbxSm->ItemIndex = cbxSm->Items->IndexOf(AnsiString(&c, 1));

    l = 0;
    allot->get_channel_id(&l);
    cbxChanBlock->ItemIndex = cbxChanBlock->Items->IndexOfObject((TObject*)l);

    allot->get_sfn_id(&l);
    dstSfn->Params->Vars[0]->AsInteger = l;
    dstSfn->Close(); dstSfn->Open();
    edtSfn->Font->Color = clGray;

    allot->get_plan_entry(&l);
    try { cbxRrc06Code->ItemIndex = l - 3; } catch (...) { cbxRrc06Code->ItemIndex = -1; };

    ws.Empty();
    tx->get_coord(&ws);
    memCoord->Lines->Text = AnsiString(ws);

    allot->get_remarks1(&ws);
    memRemark1->Lines->Text = ws; ws.Empty();
    allot->get_remarks2(&ws);
    memRemark2->Lines->Text = ws; ws.Empty();
    allot->get_remarks3(&ws);
    memRemark3->Lines->Text = ws; ws.Empty();

    // contours
    if (cbxGeoArea->Text.IsEmpty())
    {
        int oldTabIdx = tcContours->TabIndex;
        l = 0;
        allot->get_nb_sub_areas(&l);
        while (tcContours->Tabs->Count > l) tcContours->Tabs->Delete(tcContours->Tabs->Count - 1);
        while (tcContours->Tabs->Count < l) tcContours->Tabs->Add(tcContours->Tabs->Count + 1);
        for (int i = 0; i < tcContours->Tabs->Count; i++)
        {
            long cntrId = 0;
            HrCheck(allot->get_subareaTag(i, &cntrId));
            tcContours->Tabs->Strings[i] = IntToStr(cntrId);
        }
        tcContours->TabIndex = tcContours->Tabs->Count == 0 ? -1 :
                            oldTabIdx == -1 ? 0 :
                            oldTabIdx >= tcContours->Tabs->Count ? tcContours->Tabs->Count - 1 :
                                oldTabIdx;
    } else
        tcContours->Tabs->Clear();

    CacheContours();
    cd.FitContours();
    mapForm->Invalidate();

    tcContoursChange(this);

    CheckPlanEntry();

    // load list of associated txs
    lbxTxList->Items->Clear();
    sqlTxList->Close();
    sqlTxList->Params->Vars[0]->AsString = edtItuId->Text;
    for (sqlTxList->ExecQuery(); !sqlTxList->Eof; sqlTxList->Next())
    {
        int txId = sqlTxList->Fields[0]->AsInteger;
        TCOMILISBCTx tx(txBroker.GetTx(txId), true);
        AnsiString erpPatt("0.00' дБкВт, '");
        AnsiString textLabel = sqlTxList->Fields[1]->AsString + ": " +
                (tx.systemcast == ttFM ? FormatFloat(erpPatt, tx.epr_sound_max_primary) : FormatFloat(erpPatt, tx.epr_video_max)) +
                IntToStr(tx.height_eft_max) + " м, " + (tx.direction == drND ? "ND" : "D");
        lbxTxList->Items->AddObject(textLabel, (TObject*)txId);
    }
    sqlTxList->Close();

}
void __fastcall TfrmAllotment::edtFreqValueChange(TObject *Sender)
{
    AnsiString val = edtFreq->Text;
    for (int i = val.Length(); i > 0; i--)
        if (!isdigit(val[i]) && val[i] != DecimalSeparator)
            val.Delete(i, 1);

    if (val.Length() > 0)
        HrCheck(allot->set_freq(StrToFloat(val)));
    else
        HrCheck(allot->set_freq(0.0));
    double dv = 0;
    HrCheck(allot->get_freq(&dv));
    edtFreq->OldValue = FormatFloat("##0.### МГц", dv);
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::edtFreqOffsetValueChange(TObject *Sender)
{
    AnsiString val = edtFreqOffset->Text;
    for (int i = val.Length(); i > 0; i--)
        if (!isdigit(val[i]) && val[i] != '-')
            val.Delete(i, 1);

    if (val.Length() > 0)
        HrCheck(allot->set_offset(val.ToInt()));
    else
        HrCheck(allot->set_offset(0));
    long l = 0;
    HrCheck(allot->get_offset(&l));
    edtFreqOffset->OldValue = AnsiString().sprintf("%d кГц", l);
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::edtItuIdValueChange(TObject *Sender)
{
    //
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::edtNameExit(TObject *Sender)
{
    allot->set_allot_name(WideString(edtName->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxRrc06CodeChange(TObject *Sender)
{
    allot->set_plan_entry(3 + cbxRrc06Code->ItemIndex);
    CheckPlanEntry();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxRpcChange(TObject *Sender)
{
    if (cbxRpc->ItemIndex > -1)
        allot->set_ref_plan_cfg(WideString(cbxRpc->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxRnChange(TObject *Sender)
{
    if (cbxRn->ItemIndex > -1)
        allot->set_typ_ref_netwk(WideString(cbxRn->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxChanBlockChange(TObject *Sender)
{
    if (cbxChanBlock->ItemIndex > -1)
    {
        allot->set_channel_id(chn_blk_ids[cbxChanBlock->ItemIndex]);
        allot->set_freq(freqs[cbxChanBlock->ItemIndex]);
        edtFreq->OldValue = FormatFloat("##0.### МГц", freqs[cbxChanBlock->ItemIndex]);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxPolChange(TObject *Sender)
{
    if (cbxPol->Text.Length() > 0)
        allot->set_polar(cbxPol->Text[1]);
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxSmChange(TObject *Sender)
{
    if (cbxSm->Text.Length() > 0)
        allot->set_spect_mask(cbxSm->Text[1]);
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::edtAdminExit(TObject *Sender)
{
    //allot->set_admin(WideString(edtAdmin->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;    
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tcContoursChange(TObject *Sender)
{
    if (tcContours->Tabs->Count == 0)
    {
        grdContour->Enabled = false;
        grdContour->Color = clBtnFace;
        grdContour->RowCount = 1;
        oldIndex = -1;
    } else {
        grdContour->Enabled = true;
        grdContour->Color = clWindow;
        long cId;
        HrCheck(allot->get_subareaId(tcContours->TabIndex, &cId));
        long pn = 0;
        HrCheck(allot->get_points_num(cId, &pn));
        grdContour->RowCount = pn + 1;
        if (grdContour->RowCount > 1)
            grdContour->FixedRows = 1;
        for (int i = 0; i < pn; i++)
        {
            BcCoord bc = {0.0, 0.0};
            HrCheck(allot->get_point(cId, i, &bc));
            grdContour->Cells[0][i+1] = dmMain->coordToStr(bc.lon, 'X');
            grdContour->Cells[1][i+1] = dmMain->coordToStr(bc.lat, 'Y');
        }

        if (oldIndex > -1)
            cd.DrawContour(contoursData[oldIndex], clBlue, clDkGray);
        if (tcContours->TabIndex > -1)
        {
            cd.DrawContour(contoursData[tcContours->TabIndex], clLime, clWindowText);
            oldIndex = tcContours->TabIndex;
        }
    }

    //mapForm->Repaint();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtAddCntrClick(TObject *Sender)
{
    if (tcContours->Tabs->Count < 9)
    {
        addSubArea = true;
        if (!cbxGeoArea->Text.IsEmpty())
            throw *(new Exception("Невозможно редактировать список контуров.\nОчистите поле 'Контур-країна'"));
        FormProvider.ShowList(otDIG_SUBAREAS, this->Handle, 0);
    } else
        throw *(new Exception("Максимальное количество контуров выделения - 9"));

    mapForm->Update();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtDelCntrClick(TObject *Sender)
{
    if (!cbxGeoArea->Text.IsEmpty())
        throw *(new Exception("Невозможно редактировать список контуров.\nОчистите поле 'Контур-країна'"));

    if (tcContours->Tabs->Count > 0 && MessageBox(NULL, "Удалить контур?", "Подтверждение", MB_YESNO) == IDYES)
    {
        long cntrId = 0;
        AnsiString tg = tcContours->Tabs->Strings[tcContours->TabIndex];
        long tag_f, tag;
        tag = tg.ToInt();
        long count;
        HrCheck(allot->get_SubareaCount(&count));
        for( int i = 0; i < count; ++i)
        {
            HrCheck(allot->get_subareaTag(i, &tag_f));
            if(tag == tag_f)
            {
                HrCheck(allot->get_subareaId(i, &cntrId));
                break;
            }
        }
        HrCheck(allot->DelSubarea(cntrId));
        int newTabIdx = tcContours->Tabs->Count == 1 ? -1 :
                        tcContours->Tabs->Count == tcContours->TabIndex + 1 ? tcContours->TabIndex - 1 :
                        tcContours->TabIndex;
        tcContours->Tabs->Delete(tcContours->TabIndex);
        tcContours->TabIndex = newTabIdx;
        if (tcContours->TabIndex > -1)
            for (int i = tcContours->TabIndex; i < tcContours->Tabs->Count; i++)
            {
                HrCheck(allot->get_subareaTag(tcContours->TabIndex, &cntrId));
                tcContours->Tabs->Strings[i] = IntToStr(cntrId);
            }

        CacheContours();
        mapForm->Invalidate();

        tcContoursChange(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtAddPointClick(TObject *Sender)
{
    if (grdContour->RowCount < 100)
    {
        /* TODO: remove
        BcCoord bc = { 0.0, 0.0 };
        allot->AddPoint(tcContours->TabIndex, grdContour->RowCount - 1, bc);
        grdContour->RowCount = grdContour->RowCount + 1;
        grdContour->FixedRows = 1;
        grdContour->Cells[0][grdContour->RowCount - 1] = dmMain->coordToStr(bc.lon, 'X');
        grdContour->Cells[1][grdContour->RowCount - 1] = dmMain->coordToStr(bc.lat, 'Y');
        */
    } else
        throw *(new Exception("Максимальное число точек - 99"));

    mapForm->Update();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtDelPointClick(TObject *Sender)
{
    if (grdContour->RowCount > 1 && MessageBox(NULL, "Удалить точку?", "Подтверждение", MB_YESNO) == IDYES)
    {
        /* TODO: remove
        HrCheck(allot->DelPoint(tcContours->TabIndex, grdContour->Selection.Top - 1));
        grdContour->RowCount = grdContour->RowCount - 1;
        for (int i = grdContour->Selection.Top; i < grdContour->RowCount - 1; i++)
        {
            BcCoord bc = {0.0, 0.0};
            HrCheck(allot->get_point(tcContours->TabIndex, i - 1, &bc));
            grdContour->Cells[0][i] = dmMain->coordToStr(bc.lon, 'X');
            grdContour->Cells[1][i] = dmMain->coordToStr(bc.lat, 'Y');
        }

        mapForm->Update();
        */
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::grdContourSetEditText(TObject *Sender,
      int ACol, int ARow, const AnsiString Value)
{
    /*
    BcCoord bc = { 0.0, 0.0 };
    HrCheck(allot->get_point(tcContours->TabIndex, ARow - 1, &bc));
    if (ACol == 0)
        bc.lon = dmMain->strToCoord(Value);
    else
        bc.lat = dmMain->strToCoord(Value);
    HrCheck(allot->set_point(tcContours->TabIndex, ARow - 1, bc));
    */
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::FormPaint(TObject *Sender)
{
    cd.SetWc(mapForm);
    cd.contours = &contoursData;
    //cd.FitContours();
    cd.DrawContours(tcContours->TabIndex);
    if (tcContours->TabIndex > -1)
        oldIndex = tcContours->TabIndex;
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtSelCountryClick(TObject *Sender)
{
    /*TODO: select country */
    dmMain->bordSectors.clear();
    mapForm->Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::btnDropSfnClick(TObject *Sender)
{
    allot->set_sfn_id(0);
    dstSfn->Params->Vars[0]->AsInteger = 0;
    dstSfn->Close();
    CheckPlanEntry();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::btnGeoAreaClearClick(TObject *Sender)
{
    allot->set_geo_area(NULL);
    cbxGeoArea->Text = "";
    allot->DelSubarea(0);
    ShowData();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::edtItuIdExit(TObject *Sender)
{
    allot->set_adm_ref_id(WideString(edtItuId->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxCountryChange(TObject *Sender)
{
    allot->set_ctry(WideString(cbxCountry->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::cbxGeoAreaChange(TObject *Sender)
{
    allot->set_geo_area(WideString(cbxGeoArea->Text));
    ShowData();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::btnSfnClick(TObject *Sender)
{
    long sfn;
    allot->get_sfn_id(&sfn);
    FormProvider.ShowList(otSFN, this->Handle, sfn);
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::acceptListElementSelection(Messages::TMessage &Message)
{
    long l = 0;
    long data_ch = 0;
    bool match = false;
    long cn = 0;
    long tempId;
    switch (Message.WParam) {
        case otSFN :
            allot->set_sfn_id(Message.LParam);
            dstSfn->Params->Vars[0]->AsInteger = Message.LParam;
            dstSfn->Close(); dstSfn->Open();
            CheckPlanEntry();
            break;
        case otDIG_SUBAREAS:
            tx->get_data_changes(&data_ch);
            
            HrCheck(allot->get_nb_sub_areas(&cn));
            
            for (int i = 0; i < cn; i++)
            {
                long cId = 0;
                HrCheck(allot->get_subareaId(i, &cId));
                if(cId == Message.LParam)
                {
                    match = true;
                    MessageBox(NULL, "Контур вже присутній у даному виділенні!",
                                        "Попередження", MB_ICONWARNING | MB_OK);
                    i = cn;
                }
            }
            if(!match)
            {
                if (addSubArea && MessageBox(NULL, "З доданням контуру трэба зберiгти видiлення\nПродовжити?*",
                                        "Пiдтвердження", MB_ICONQUESTION | MB_YESNO) == IDYES)
                {

                    HrCheck(allot->AddSubarea(Message.LParam));
                    tx->save();
                    dmMain->LoadDetails(tx);
                    data_ch = 0;
                    addSubArea = 0;
                }

                if (!data_ch && !addSubArea)
                {
                    HrCheck(allot->get_SubareaCount(&l));

                    for( int i = 0; i < l; ++i)
                    {
                        HrCheck(allot->get_subareaId(i, &tempId));
                        if(tempId == Message.LParam)
                        {
                            HrCheck(allot->get_subareaTag(i, &l));
                            break;
                        }
                    }
                    
                    tcContours->Tabs->Add(l);
                    tcContours->TabIndex = tcContours->Tabs->Count - 1;

                    CacheContours();
                    mapForm->Invalidate();

                    tcContoursChange(this);
                }
            }
            break;
        default : break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::CacheContours()
{
    contoursData.clear();

    long cn = 0;
    HrCheck(allot->get_nb_sub_areas(&cn));
    for (int i = 0; i < cn; i++)
    {
        long cId = 0;
        long cTag = 0;
        HrCheck(allot->get_subareaId(i, &cId));
        HrCheck(allot->get_subareaTag(i, &cTag));
        long pn = 0;

        DrawContourData contourData;
        contourData.id = i;
        contourData.name = IntToStr(cTag);
        contourData.contour.clear();

        HrCheck(allot->get_points_num(cId, &pn));
        for (int j = 0; j < pn; j++)
        {
            BcCoord bcCurr = { 0.0, 0.0 };
            HrCheck(allot->get_point(cId, j, &bcCurr));
            contourData.contour.push_back(bcCurr);
        }
        contoursData.push_back(contourData);

    }
}

int __fastcall TfrmAllotment::GetDbSection()
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select DB_SECTION_ID from DIG_ALLOTMENT where ID = " + IntToStr(tx.id);
    sql->ExecQuery();
    if (sql->Eof)
        return tsBase;
    else
        return sql->Fields[0]->AsInteger;
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::FormResize(TObject *Sender)
{
    cd.AjustScale();
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtFitClick(TObject *Sender)
{
    cd.FitContours();
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtZoomInClick(TObject *Sender)
{
    cd.SetScaleX(cd.GetScaleX() / 1.5);
    cd.SetScaleY(cd.GetScaleY() / 1.5);
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtZoomOutClick(TObject *Sender)
{
    cd.SetScaleX(cd.GetScaleX() * 1.5);
    cd.SetScaleY(cd.GetScaleY() * 1.5);
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtLeftClick(TObject *Sender)
{
    double shift = cd.GetLon(mapForm->ClientWidth / 3) - cd.GetLon(0);
    cd.maxlon += shift;
    cd.minlon += shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtRightClick(TObject *Sender)
{
    double shift = cd.GetLon(mapForm->ClientWidth / 3) - cd.GetLon(0);
    cd.maxlon -= shift;
    cd.minlon -= shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtDownClick(TObject *Sender)
{
    double shift = cd.GetLat(0) - cd.GetLat(mapForm->ClientHeight / 3);
    cd.maxlat += shift;
    cd.minlat += shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::tbtUpClick(TObject *Sender)
{
    double shift = cd.GetLat(0) - cd.GetLat(mapForm->ClientHeight / 3);
    cd.maxlat -= shift;
    cd.minlat -= shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::CheckPlanEntry()
{
    long pe = 0;
    allot->get_plan_entry(&pe);
    long si = 0;
    allot->get_sfn_id(&si);
    
    if (pe == 3 && si > 0 || pe == 4 && si > 0 || pe == 5 && si == 0)
    {
        cbxRrc06Code->Font->Color = clWindowText;
        cbxRrc06Code->Hint = "";
    } else {
        cbxRrc06Code->Font->Color = clRed;
        cbxRrc06Code->Hint = "Поле 'plan_entry' ('"+lblRrc06Code->Caption+"') назначено неправильно.\nПроверьте назначение ОЧС.";
    }
}
void __fastcall TfrmAllotment::memCoordExit(TObject *Sender)
{
    tx->set_coord(WideString(memCoord->Lines->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::lbxTxListDblClick(TObject *Sender)
{
    TListBox *lbx = dynamic_cast<TListBox*>(Sender);
    if (lbx && lbx->ItemIndex > -1)
        FormProvider.ShowTx(txBroker.GetTx((int)lbx->Items->Objects[lbx->ItemIndex], CLSID_LISBCTx));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::lbxTxListKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (Key == VK_RETURN && TShiftState() == Shift)
        lbxTxListDblClick(Sender);
}
//---------------------------------------------------------------------------
void __fastcall TfrmAllotment::memRemark1Exit(TObject *Sender)
{
    allot->set_remarks1(WideString(memRemark1->Lines->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::memRemark2Exit(TObject *Sender)
{
    allot->set_remarks2(WideString(memRemark2->Lines->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::memRemark3Exit(TObject *Sender)
{
    allot->set_remarks3(WideString(memRemark3->Lines->Text));
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::sbExaminationClick(TObject *Sender)
{
     std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
      sql->Database = dmMain->dbMain;
      sql->SQL->Text = "select ID, CREATEDATE, NAMEQUERIES from SELECTIONS where TRANSMITTERS_ID = " + txtId->Caption + " and SELTYPE = 'E' order by NAMEQUERIES";
      sql->Transaction = dmMain->trMain;
    sql->ExecQuery();

    popupMenu->Items->Clear();
    while (!sql->Eof)
    {//заполнение меню экспертизами
        TMenuItem *menuItem = new TMenuItem(popupMenu);

        menuItem->Caption = sql->FieldByName("NAMEQUERIES")->AsString + " ["+sql->FieldByName("CREATEDATE")->AsString + "]";
        menuItem->OnClick = MenuItem_OnClick;
        menuItem->Tag = sql->FieldByName("ID")->AsInteger;
        popupMenu->Items->Add(menuItem);

        sql->Next();
    }
    sql->Close();

    if ( popupMenu->Items->Count > 0 )
    {
        TMenuItem *menuItem = new TMenuItem(popupMenu);
        menuItem->Caption = "-";
        popupMenu->Items->Add(menuItem);
    }

    {
        TMenuItem *menuItem = new TMenuItem(popupMenu);
        menuItem->Caption = "Експертиза...";
        menuItem->OnClick = MenuItem_OnClick;
        menuItem->Tag = 0;
        popupMenu->Items->Add(menuItem);
    }

    POINT point;
    if ( GetCursorPos(&point) != 0 )
        popupMenu->Popup(point.x, point.y);
}
//---------------------------------------------------------------------------

void __fastcall TfrmAllotment::MenuItem_OnClick(TObject *Sender)
{
    if ( dynamic_cast<TMenuItem *>(Sender)->Tag != 0 )
    {
        TfrmSelection* fs = NULL;
        for (int i = 0; i < frmMain->MDIChildCount; i++)
            if ((fs = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i])) && (dynamic_cast<TMenuItem *>(Sender)->Tag == fs->GetId()) )
                fs->Show();

        //  если нет, то создать
  //      TempCursor tc(crHourGlass);

        fs = new TfrmSelection(Application, (void*)(dynamic_cast<TMenuItem *>(Sender)->Tag));
        fs->Left = 0;
        fs->Top = 0;
        fs->Width = frmMain->ClientWidth
                    - frmMain->leftSplitter->Width
                    - frmMain->rightSplitter->Width
                    - frmMain->pnlLeftDockPanel->Width
                    - frmMain->pnlRightDockPanel->Width
                    - GetSystemMetrics(SM_CXFRAME);

        fs->Height = frmMain->ClientHeight
                    - frmMain->tbrShortcut->Height
                    - frmMain->StatusBar1->Height
                    - GetSystemMetrics(SM_CYFRAME);
    }
    else
    {
        int id = tx.id;
        txAnalyzer.MakeNewSelection(id, nsExpertise);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmAllotment::sbIntoProjectClick(TObject *Sender)
{
    SendMessage(frmMain->showExplorer(), WM_LIST_ELEMENT_SELECTED, 39, tx.id);
}
//---------------------------------------------------------------------------

