//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmTxLfMf.h"
#include "uParams.h"
#include "uTable36.h"
#include "uAnalyzer.h"
#include "FormProvider.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CustomMap"
#pragma link "NumericEdit"
#pragma link "uFrmTxBaseAir"
#pragma link "uLisObjectGrid"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma link "uFrmTxBase"
#pragma resource "*.dfm"

#ifdef StrToInt
#undef StrToInt
#endif

TfrmTxLfMf *frmTxLfMf;
//---------------------------------------------------------------------------
__fastcall TfrmTxLfMf::TfrmTxLfMf(TComponent* Owner, ILISBCTx *in_Tx)
    : Inherited(Owner, in_Tx)
{
    if (!lfmf.IsBound() && Tx.IsBound())
        HrCheck(Tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf), "Запрос интерфейса ILisBcLfMf");
    //#ifdef _DEBUG
    btErp->Visible = true;
    //#endif
}

void __fastcall TfrmTxLfMf::acceptListElementSelection(Messages::TMessage &Message)
{
    switch (Message.WParam)
    {
        case otSFN:
            HrCheck(Tx->set_identifiersfn(Message.LParam), "Setting SFN identifier");
            ShowSfn();
            break;
        default:
            Inherited::acceptListElementSelection(Message);
            break;
    }
}

void __fastcall TfrmTxLfMf::TxDataLoad()
{
    Inherited::TxDataLoad();
    dmMain->GetList("select ENUM, CODE from LFMF_SYSTEM order by 2", cbSys->Items);

    if (!lfmf.IsBound())
    {
        if (Tx.IsBound())
            HrCheck(Tx->QueryInterface(IID_ILisBcLfMf, (void**)&lfmf), "Запрос интерфейса ILisBcLfMf");
        else
            throw *(new Exception("Запрос интерфейса ILisBcLfMf: Интерфейс ILISBCTx не готов"));
    }

    double dv;
    HrCheck(Tx->get_sound_carrier_primary(&dv));
    edFreq->OldValue = FormatFloat("0.###", dv * 1000.);
    HrCheck(lfmf->get_gnd_cond(&dv));
    edGndCond->OldValue = FormatFloat("0.##", dv);
    edNoiseZone->OldValue = IntToStr(lfmf->get_noise_zone());

    double lon, lat;
    HrCheck(Tx->get_longitude(&lon));
    HrCheck(Tx->get_latitude(&lat));
    edGndCondCalc->OldValue = FormatFloat("0.##", txAnalyzer.GetGndCond(lon, lat));
    edNoiseZoneCalc->OldValue = IntToStr(txAnalyzer.GetNoiseZone(lon, lat));

    ShowSfn();
    long value = 0;
    HrCheck(Tx->get_relativetimingsfn(&value));
    edSynchro->OldValue = IntToStr(value);

    VARIANT_BOOL bv;
    HrCheck(lfmf->get_day_op(&bv));
    chDay->Checked = bv;
    rbDay->Enabled = bv;
    HrCheck(lfmf->get_night_op(&bv));
    chNight->Checked = bv;
    rbNight->Enabled = bv;
    HrCheck(lfmf->get_is_day(&bv));
    if(bv != -1)
    {
        rbDay->Checked = bv && rbDay->Enabled;
        rbNight->Checked = !bv && rbNight->Enabled;
        
    }
    else
    {
        rbDay->Checked = false;
        rbNight->Checked = false;
    }
    ShowOperData();
    lblEditing->Visible = false;

    bool isDraft = ibdsStantionsBaseSTATUS->AsInteger == tsDraft;
}

void __fastcall TfrmTxLfMf::TxDataSave()
{
    txAnalyzer.DropEtalonZone(Tx, lfmf->is_day);
    txAnalyzer.DropEtalonZone(Tx, !lfmf->is_day);

    Inherited::TxDataSave();

    if (dstLfMfOper->State == dsEdit)
        dstLfMfOper->Post();
    dstLfMfOper->Transaction->CommitRetaining();
}

void __fastcall TfrmTxLfMf::SetRadiationClass()
{
    String emCl = edBw->Text;
    if (emCl.Length() > 0)
    {
        int pos = emCl.Pos(DecimalSeparator);
        if (pos > 0)
            emCl[pos] = 'K';
        else
            emCl += 'K';
        while (emCl.Length() < 4)
            emCl += '0';
    }
    if (emCl.Length() > 0 && cbSys->ItemIndex > -1)
    {
        if (cbSys->Text == "AM")
            emCl += "A3EGN";
        else
            emCl += "Q----";
    }
    edEmissionClass->Text = emCl;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::FormCreate(TObject *Sender)
{
    Caption = "Передавач СХ/ДХ";
    Inherited::FormCreate(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::cbSysChange(TObject *Sender)
{
    long sys = -1;
    if (cbSys->ItemIndex > -1)
        sys = (long)cbSys->Items->Objects[cbSys->ItemIndex];
    HrCheck(lfmf->set_lfmf_system(sys));
    CheckControls();

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select BW from LFMF_SYSTEM where ENUM = "+IntToStr(sys);
    sql->ExecQuery();
    if (!sql->Eof)
    {
        edBw->Text = FormatFloat("0.#", sql->Fields[0]->AsDouble);
        edBwValueChange(Sender);
    }

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edFreqValueChange(TObject *Sender)
{
    double freq = 0.0;
    try {
        if (edFreq->Text.Length() > 0)
            freq = edFreq->Text.ToDouble() / 1000.;
        HrCheck(Tx->set_sound_carrier_primary(freq));
    } __finally {
        HrCheck(Tx->get_sound_carrier_primary(&freq));
        edFreq->OldValue = FormatFloat("0.###", freq * 1000.);
    }

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edBwValueChange(TObject *Sender)
{
    HrCheck(lfmf->set_lfmf_bw(edBw->Text.ToDouble()));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::chDayClick(TObject *Sender)
{
    HrCheck(lfmf->set_day_op((VARIANT_BOOL)chDay->Checked));
    if (!chDay->Checked && !chNight->Checked)
    {
        chNight->Checked = true;
        chNightClick(Sender);
    }
    CheckDayNight();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::chNightClick(TObject *Sender)
{
    HrCheck(lfmf->set_night_op((VARIANT_BOOL)chNight->Checked));
    if (!chDay->Checked && !chNight->Checked)
    {
        chDay->Checked = true;
        chDayClick(Sender);
    }
    CheckDayNight();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::rbDayNightClick(TObject *Sender)
{
    HrCheck(lfmf->set_is_day((long)rbDay->Checked));
    if (dstLfMfOper->State == dsEdit)
        dstLfMfOper->Post(), isChanged = true;
    ShowOperData();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::CheckDayNight()
{
    rbDay->Enabled = chDay->Checked;
    rbNight->Enabled = chNight->Checked;
    if (!rbDay->Enabled && !rbNight->Enabled && !rbDay->Checked)
    {
        rbDay->Checked = true;
        rbDayNightClick(rbDay);
    }
    if (rbNight->Enabled && !rbDay->Enabled && !rbNight->Checked)
    {
        rbNight->Checked = true;
        rbDayNightClick(rbNight);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmTxLfMf::cbAntTypeChange(TObject *Sender)
{
    char antType = cbAntType->ItemIndex == -1 ? '\0' : cbAntType->ItemIndex == 0 ? 'A' : 'B';
    HrCheck(lfmf->set_ant_type(antType));
    if (antType)
        btClearGainHClick(Sender);
    CheckControls();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edAdjRatValueChange(TObject *Sender)
{
    HrCheck(lfmf->set_adj_ratio(edAdjRat->Text.ToDouble()));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edNoiseZoneValueChange(TObject *Sender)
{
    HrCheck(lfmf->set_noise_zone(edNoiseZone->Text.ToDouble()));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edGndCondValueChange(TObject *Sender)
{
    HrCheck(lfmf->set_gnd_cond(edGndCond->Text.ToDouble()));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::cbModTypeChange(TObject *Sender)
{
    HrCheck(lfmf->set_mod_type(cbModType->ItemIndex));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::cbProtLevlChange(TObject *Sender)
{
    HrCheck(lfmf->set_prot_levl(cbProtLevl->ItemIndex));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edPwrKwValueChange(TObject *Sender)
{
    HrCheck(Tx->set_power_sound_primary(edPwrKw->Text.ToDouble()));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btGainHClick(TObject *Sender)
{
    TfrmTable36* tf = new TfrmTable36(this, t36GAIN_H, Tx);
    double maxGainH;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btClearGainHClick(TObject *Sender)
{
    if (MessageBox(NULL, "Очистити горизонтальну ДС антени?", "Пiдтвердження", MB_YESNO|MB_ICONQUESTION) == IDYES)
    {
        ILisBcAntPattPtr ap;
        OleCheck(Tx->QueryInterface<ILisBcAntPatt>(&ap));
        for (int i = 0; i < 36; i++)
            ap->set_gain_h(i, 0.);
        edMaxGainH->OldValue = "0";
        double emrp = -999.;
        double erp = -999.;
        for (long i = 0; i < 360; i+=10)
        {
            HrCheck(Tx->get_erp(i, &erp), __FUNC__"()");
       // Tx->get_erp(i, &erp);
            if (emrp < erp)
                emrp = erp;
        }
        edEmrp->OldValue = emrp > -999.? FormatFloat("0.##", emrp) : String();
        edEmrpValueChange(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edEmrpValueChange(TObject *Sender)
{
    double emrp = -999.;
    if (edEmrp->Text.Length() > 0)
        emrp = edEmrp->Text.ToDouble();
    HrCheck(Tx->set_epr_sound_max_primary(emrp));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edtAglValueChange(TObject *Sender)
{
    HrCheck(Tx->set_heightantenna(edtAgl->Text.ToInt()));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::CheckControls()
{
    long lv = 0;
    HrCheck(lfmf->get_lfmf_system(&lv));
    cbSys->ItemIndex = cbSys->Items->IndexOfObject((TObject*)lv);
    bool isAm = (lv == lmAm);
    cbModType->Visible = !isAm;
    lbModType->Visible = !isAm;
    cbProtLevl->Visible = !isAm;
    lbProtLevl->Visible = !isAm;
    cbMonoStereo->Visible = isAm;
    lbMonoStereo->Visible = isAm;
    //pnSfn->Visible = !isAm;

    unsigned char at = '\0';
    HrCheck(lfmf->get_ant_type(&at));
    bool customAnt = (at == 'B');
    lbGainH->Visible = customAnt;
    edMaxGainH->Visible = customAnt;
    btGainH->Visible = customAnt;
    btClearGainH->Visible = customAnt;
}

void __fastcall TfrmTxLfMf::ShowOperData()
{
    if(!rbDay->Checked && !rbNight->Checked)
    {
        gbEmission->Visible = false;
        gbAntenna->Visible = false;
        gbPower->Visible = false;
        pnOpTm->Visible = false;
    }
    else {
    bool isVisbl = rbDay->Checked || rbNight->Checked;
    gbEmission->Visible = isVisbl;
    gbAntenna->Visible = isVisbl;
    gbPower->Visible = isVisbl;
    pnOpTm->Visible = isVisbl;

    CheckControls();

    long lv = 0;
    Tx->get_monostereo_primary(&lv);
    cbMonoStereo->ItemIndex = lv;

    HrCheck(lfmf->get_mod_type(&lv));
    cbModType->ItemIndex = lv;
    HrCheck(lfmf->get_prot_levl(&lv));
    cbProtLevl->ItemIndex = lv;

    double dv = 0.;
    HrCheck(Tx->get_power_sound_primary(&dv));
    edPwrKw->OldValue = FormatFloat("0.##", dv);
    HrCheck(Tx->get_epr_sound_max_primary(&dv));
    edEmrp->OldValue = dv > -999.? FormatFloat("0.##", dv) : String();
    HrCheck(lfmf->get_lfmf_bw(&dv));
    edBw->OldValue = FormatFloat("0.#", dv);

    double maxGainH = -999.;
    double gain = 0.;
    ILisBcAntPattPtr antPatt;
    HrCheck(Tx->QueryInterface(IID_ILisBcAntPatt, (void**)&antPatt), __FUNC__"() - get antenna pattern");
    for (long i = 0; i < 36; i++)
    {
        HrCheck(antPatt->get_gain_h(i, &gain));
        if (maxGainH < gain)                                   
            maxGainH = gain;
    }
    edMaxGainH->OldValue = FormatFloat("0.#", maxGainH);

    HrCheck(lfmf->get_adj_ratio(&dv));
    edAdjRat->OldValue = FormatFloat("0.##", dv);

    HrCheck(Tx->get_heightantenna(&lv));
    edtAgl->OldValue = IntToStr(lv);
    unsigned char ucv = '\0';
    HrCheck(lfmf->get_ant_type(&ucv));
    cbAntType->ItemIndex = (ucv == 'A' ? 0 : ucv == 'B' ? 1 : -1);

    SetRadiationClass();

    dstLfMfOper->Close();
    dstLfMfOper->ParamByName("ID")->AsInteger = id;
    dstLfMfOper->ParamByName("DAYNIGHT")->AsString = rbDay->Checked ? "HJ" : "HN";
    dstLfMfOper->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::cbMonoStereoChange(TObject *Sender)
{
    if (cbMonoStereo->ItemIndex > -1)
        Tx->set_monostereo_primary(cbMonoStereo->ItemIndex);
    else
    {
        long LValue;
        Tx->get_monostereo_primary(&LValue);
        if (LValue == 0)
            cbMonoStereo->ItemIndex = 0;
        else
            cbMonoStereo->ItemIndex = 1;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btSetGndCondClick(TObject *Sender)
{
    double gc = edGndCondCalc->Text.ToDouble();
    HrCheck(lfmf->set_gnd_cond(gc));
    edGndCond->Text = edGndCondCalc->Text;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btEmissionClassClick(TObject *Sender)
{
    SetRadiationClass();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btSetNoiseZoneClick(TObject *Sender)
{
    long nz = edNoiseZoneCalc->Text.ToDouble();
    HrCheck(lfmf->set_noise_zone(nz));
    edNoiseZone->Text = edNoiseZoneCalc->Text;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::actOkUpdate(TObject *Sender)
{
    Inherited::actOkUpdate(Sender);
    
    edNoiseZoneCalc->Font->Color = edNoiseZoneCalc->Text == edNoiseZone->Text ? clWindowText : clRed;
    edGndCondCalc->Font->Color = edGndCondCalc->Text == edGndCond->Text ? clWindowText : clRed;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btErpClick(TObject *Sender)
{
    TfrmTable36* tf = new TfrmTable36(this, t36EPRH, Tx);
    tf->sgTable36->Enabled = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btnSetSfnClick(TObject *Sender)
{
    long sfnId = 0;
    Tx->get_identifiersfn(&sfnId);
    FormProvider.ShowList(otSFN, this->Handle, sfnId);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btnDropSfnIdClick(TObject *Sender)
{
    HrCheck(Tx->set_identifiersfn(0), "Setting SFN identifier");
    edtSfn->Text = "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::edSynchroValueChange(TObject *Sender)
{
    long value;
    try {
        if (edSynchro->Text.Length() > 0)
            value = StrToInt(edSynchro->Text);
        else
            value = 0;

        Tx->set_relativetimingsfn(value);
    } catch (Exception &e) {
        Tx->get_relativetimingsfn(&value);
    }
    edSynchro->OldValue = IntToStr(value);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::btSetEmrpClick(TObject *Sender)
{
    double emrp = -999.;
    double erp = -999.;
    for (long i = 0; i < 360; i+=10)
    {
        HrCheck(Tx->get_erp(i, &erp), __FUNC__"()");
        if (emrp < erp)
            emrp = erp;
    }
    edEmrp->OldValue = emrp > -999.? FormatFloat("0.##", emrp) : String();
    edEmrpValueChange(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::dstLfMfOpeTIMEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    double val = Sender->AsFloat;
    if (!Sender->IsNull)
        Text = String().sprintf("%02d:%02d", (int)floor(val), int((val - floor(val))*100.));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::dstLfMfOperTIMESetText(TField *Sender,
      const AnsiString Text)
{
    if (Text.IsEmpty())
        Sender->SetData(NULL);
    else
    {
        int pos = 1;
        while (Text.Length() >= pos && !isdigit(Text[pos]))
            pos++;
        float hr = 0.;
        while (hr < 3. && Text.Length() >= pos && isdigit(Text[pos]))
            hr = (hr*10. + (Text[pos++] - '0'));
        while (Text.Length() >= pos && !isdigit(Text[pos]))
            pos++;
        float mn = 0.;
        while (mn < 7. && Text.Length() >= pos && isdigit(Text[pos]))
            mn = (mn*10. + (Text[pos++] - '0'));

        while (mn >= 60.)
            mn -= 60., hr += 1.;
        while (hr > 24. || (hr == 24. && mn > 0.))
            hr -= 24.;
            
        Sender->AsFloat = hr + mn/100.;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxLfMf::ShowSfn()
{
    long sfnId = 0;
    Tx->get_identifiersfn(&sfnId);
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select S.SYNHRONETID from SYNHROFREQNET S where S.ID = " + AnsiString(sfnId);
    sql->ExecQuery();
    edtSfn->Text = sql->Eof ? String() : sql->Fields[0]->AsString;
}




