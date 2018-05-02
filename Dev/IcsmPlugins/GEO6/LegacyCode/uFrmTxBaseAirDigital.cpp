 //---------------------------------------------------------------------------
  
#include <vcl.h>
#pragma hdrstop    

#include "uFrmTxBaseAirDigital.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include "TxBroker.h"
#include "uItuExport.h"
#include "uParams.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uFrmTxBaseAir"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma link "NumericEdit"
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"
//  блядская ATL
#ifdef StrToInt
#undef StrToInt
#endif

TfrmTxBaseAirDigital *frmTxBaseAirDigital;
//---------------------------------------------------------------------------
__fastcall TfrmTxBaseAirDigital::TfrmTxBaseAirDigital(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBaseAir(Owner, in_Tx)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::acceptListElementSelection(Messages::TMessage &Message)
{
    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    TfrmTxBaseAir::acceptListElementSelection(Message);

    switch (Message.WParam) {
        case otSFN :
            sql->SQL->Text = "select ID, SYNHRONETID from SYNHROFREQNET where ID = " + AnsiString(Message.LParam);
            sql->ExecQuery();
            if (ibdsTxDigital->State != dsEdit)
                ibdsTxDigital->Edit();
            ibdsTxDigitalIDENTIFIERSFN->AsInteger = sql->Fields[0]->AsInteger;
            ibdsTxDigitalSYNHRONETID->AsString = sql->Fields[1]->AsString;
            sql->Close();
            break;
        default :
            break;
    }
    sql->Close();
    delete sql;

}

void __fastcall TfrmTxBaseAirDigital::TxDataLoad()
{
    cbxRpc->Items->Clear();
    cbxRpc->Items->AddObject("  ", (TObject*)Lisbctxserver_tlb::rpc0);
    cbxRpc->Items->AddObject(" 1", (TObject*)Lisbctxserver_tlb::rpc1);
    cbxRpc->Items->AddObject(" 2", (TObject*)Lisbctxserver_tlb::rpc2);
    cbxRpc->Items->AddObject(" 3", (TObject*)Lisbctxserver_tlb::rpc3);
    cbxRpc->Items->AddObject(" 4", (TObject*)Lisbctxserver_tlb::rpc4);
    cbxRpc->Items->AddObject(" 5", (TObject*)Lisbctxserver_tlb::rpc5); 

    cbxRxMode->Items->Clear();
    cbxRxMode->Items->AddObject("FX", (TObject*)Lisbctxserver_tlb::rmFx);
    cbxRxMode->Items->AddObject("MO", (TObject*)Lisbctxserver_tlb::rmMo);
    cbxRxMode->Items->AddObject("PI", (TObject*)Lisbctxserver_tlb::rmPi);
    cbxRxMode->Items->AddObject("PO", (TObject*)Lisbctxserver_tlb::rmPo);

    TfrmTxBaseAir::TxDataLoad();

    /*
    double erp = -999.0;
    Tx->get_epr_video_hor(&erp);
    edtEPRGAudio1->OldValue = FormatFloat("0.##",erp);
    erp = -999.0;
    Tx->get_epr_video_vert(&erp);
    edtEPRVAudio1->OldValue = FormatFloat("0.##",erp);
    erp = -999.0;
    Tx->get_epr_video_max(&erp);
    edtEPRmaxAudio1->OldValue = FormatFloat("0.##",erp);
    */

    ibdsTxDigital->Active = false;
    ibdsTxDigital->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsTxDigital->Active = true;

    long LValue = 0;
    Tx->get_relativetimingsfn(&LValue);
    edtSynchronization->OldValue = AnsiString(LValue);

    LValue = 0;
    Tx->get_video_offset_herz(&LValue);
    edtOffset->OldValue = AnsiString().sprintf("%+d",LValue);

    TBcRpc rpc;
    Tx->get_rpc(&rpc);
    cbxRpc->ItemIndex = cbxRpc->Items->IndexOfObject((TObject*)rpc);
   
    TBcRxMode rxMode;
    Tx->get_rxMode(&rxMode);
    cbxRxMode->ItemIndex = cbxRxMode->Items->IndexOfObject((TObject*)rxMode);

    CheckSfn();
    CheckAssgnCode();

    double val = 0.0;
    Tx->get_pol_isol(&val);
    chPolIsol->Checked = (val != 0.0);
    chPolIsolClick(this);
}

void __fastcall TfrmTxBaseAirDigital::TxDataSave()
{
    if (ibdsTxDigital->State == dsEdit)
        ibdsTxDigital->Post();
    TfrmTxBaseAir::TxDataSave();
}

void __fastcall TfrmTxBaseAirDigital::btnSetSfnClick(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger == tsDraft)
        FormProvider.ShowList(otSFN, this->Handle, ibdsTxDigitalIDENTIFIERSFN->AsInteger);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBaseAirDigital::ibdsTxDigitalAfterEdit(
      TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;    
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBaseAirDigital::edtSynchronizationEnter(
      TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        ((TEdit *)Sender)->ReadOnly = true;
    else
        ((TEdit *)Sender)->ReadOnly = false;    
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::edtSynchronizationValueChange(
      TObject *Sender)
{
    long value;
    try {
        if (edtSynchronization->Text.Length() > 0)
            value = StrToInt(edtSynchronization->Text);
        else
            value = 0;

        Tx->set_relativetimingsfn(value);
    } catch (Exception &e) {
        Tx->get_relativetimingsfn(&value);
    }
    edtSynchronization->OldValue = AnsiString(value);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::cbxRpcChange(TObject *Sender)
{
    try {

        if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
            throw *(new Exception(AnsiString("Редагування передавача неможливе")));
        }

        if (cbxRpc->ItemIndex > -1)
        {
            TBcRpc rpc = (TBcRpc)cbxRpc->Items->Objects[cbxRpc->ItemIndex];
            HrCheck(Tx->set_rpc(rpc));

            if (BCCalcParams.rpcRxModeLink)
            {
                switch (rpc)
                {
                    case rpc1: HrCheck(Tx->set_rxMode(rmFx)); break;
                    case rpc2: HrCheck(Tx->set_rxMode(rmMo)); break;
                    case rpc3: HrCheck(Tx->set_rxMode(rmPi)); break;
                }
            }
        }
    } catch (Exception &e) {

        TBcRpc rpc;
        Tx->get_rpc(&rpc);
        cbxRpc->ItemIndex = cbxRpc->Items->IndexOfObject((TObject*)rpc);

        throw *(new Exception(e.Message));
    }

    TBcRpc rpc;
    Tx->get_rpc(&rpc);
    cbxRpc->ItemIndex = cbxRpc->Items->IndexOfObject((TObject*)rpc);

    TBcRxMode rxMode;
    Tx->get_rxMode(&rxMode);
    cbxRxMode->ItemIndex = cbxRxMode->Items->IndexOfObject((TObject*)rxMode);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::cbxRxModeChange(TObject *Sender)
{
    try {

        if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
            throw *(new Exception(AnsiString("Редагування передавача неможливе")));
        }

        if (cbxRxMode->ItemIndex > -1)
        {
            TBcRxMode rxMode = (TBcRxMode)cbxRxMode->Items->Objects[cbxRxMode->ItemIndex];
            HrCheck(Tx->set_rxMode(rxMode));
            /*
            if (BCCalcParams.rpcRxModeLink)
            {
                switch (rxMode)
                {
                    case rmFx: HrCheck(Tx->set_rpc((type_form == ttDVB) ? rpc1 : rpc4)); break;
                    case rmMo: HrCheck(Tx->set_rpc((type_form == ttDVB) ? rpc2 : rpc4)); break;
                    case rmPo: HrCheck(Tx->set_rpc((type_form == ttDVB) ? rpc2 : rpc5)); break;
                    case rmPi: HrCheck(Tx->set_rpc((type_form == ttDVB) ? rpc3 : rpc5)); break;
                }
            }   */
        }

    } catch (Exception &e) {

        TBcRxMode rxMode;
        Tx->get_rxMode(&rxMode);
        cbxRxMode->ItemIndex = cbxRxMode->Items->IndexOfObject((TObject*)rxMode);

        throw *(new Exception(e.Message));
    }

    TBcRpc rpc;
    Tx->get_rpc(&rpc);
    cbxRpc->ItemIndex = cbxRpc->Items->IndexOfObject((TObject*)rpc);

    TBcRxMode rxMode;
    Tx->get_rxMode(&rxMode);
    cbxRxMode->ItemIndex = cbxRxMode->Items->IndexOfObject((TObject*)rxMode);
}
//---------------------------------------------------------------------------

void _fastcall TfrmTxBaseAirDigital::TxToForm()
{
    TfrmTxBaseAir::TxToForm();
}

void _fastcall TfrmTxBaseAirDigital::FormToTx()
{
    TfrmTxBaseAir::FormToTx();
}


void __fastcall TfrmTxBaseAirDigital::edtOffsetValueChange(TObject *Sender)
{
    long value;
    try {
        if (edtOffset->Text.Length() > 0)
            value = StrToInt(edtOffset->Text);
        else
            value = 0;

        Tx->set_video_offset_herz(value);
    } catch (Exception &e) {
        Tx->get_video_offset_herz(&value);
    }
    edtOffset->OldValue = AnsiString().sprintf("%+d",value);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBaseAirDigital::btnDropSfnIdClick(TObject *Sender)
{
    if (!ibdsTxDigitalIDENTIFIERSFN->IsNull)
    {
        if (ibdsTxDigital->State != dsEdit)
            ibdsTxDigital->Edit();
        ibdsTxDigitalIDENTIFIERSFN->Clear();
        ibdsTxDigitalSYNHRONETID->Clear();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::dsTxDigitalDataChange(
      TObject *Sender, TField *Field)
{
    //CheckSfn();
    //CheckAssgnCode();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::CheckSfn()
{
    if (ibdsTxDigitalSYNHRONETID->AsString == ibdsTxDigitalASSOCIATED_ALLOT_SFN_ID->AsString)
    {
        edtSfn->Font->Color = clWindowText;
        edAssocSfnId->Font->Color = clWindowText;
        edtSfn->Hint = "";
        edAssocSfnId->Hint = "";
    } else {
        edtSfn->Font->Color = clRed;
        edAssocSfnId->Font->Color = clRed;
        edtSfn->Hint = "ОЧС присвоения не совпадает с ОЧС выделения";
        edAssocSfnId->Hint = edtSfn->Hint;
    }
}

void __fastcall TfrmTxBaseAirDigital::CheckAssgnCode()
{
    TIntegerField   *fldSfnId       = ibdsTxDigitalIDENTIFIERSFN;
    TIBStringField  *fldAllotId     = ibdsTxDigitalASSOCIATED_ADM_ALLOT_ID;
    TIBStringField  *fldAssCode     = ibdsTxDigitalASSGN_CODE;

    if (fldSfnId->IsNull && fldAllotId->AsString.Length() == 0 && fldAssCode->AsString != "S"
        || !fldSfnId->IsNull && fldAllotId->AsString.Length() == 0 && fldAssCode->AsString != "L"
        || !fldSfnId->IsNull && fldAllotId->AsString.Length() > 0 && fldAssCode->AsString != "L" && fldAssCode->AsString != "C"
        || fldSfnId->IsNull && fldAllotId->AsString.Length() > 0 && fldAssCode->AsString != "L"
        )
    {
        edAssgnCode->Font->Color = clRed;
        edAssgnCode->Hint = "Поле 'assgn_code' ('"+lblAssgnCode->Caption+"') назначено неправильно.\nПроверьте назначения ОЧС и выделения";
        cbAssgnCode->Hint = edAssgnCode->Hint;
        edAssgnCode->SelLength = 0;
    } else {
        edAssgnCode->Font->Color = clWindowText;
        cbAssgnCode->Hint = edAssgnCode->Hint;
        edAssgnCode->Hint = "";
    }
}

void __fastcall TfrmTxBaseAirDigital::SetPlanEntry()
{
    TIntegerField   *fldPlnEntr     = ibdsTxDigitalPLAN_ENTRY;
    TIntegerField   *fldSfnId       = ibdsTxDigitalIDENTIFIERSFN;
    TIBStringField  *fldAllotId     = ibdsTxDigitalASSOCIATED_ADM_ALLOT_ID;

    // autoset t_plan_entry
    if (fldSfnId->IsNull && fldAllotId->AsString.Length() == 0 && fldPlnEntr->AsInteger != 1
        || !fldSfnId->IsNull && fldAllotId->AsString.Length() == 0 && fldPlnEntr->AsInteger != 2
        || !fldSfnId->IsNull && fldAllotId->AsString.Length() > 0 && fldPlnEntr->AsInteger != 3 && fldPlnEntr->AsInteger != 4
        || fldSfnId->IsNull && fldAllotId->AsString.Length() > 0 && fldPlnEntr->AsInteger != 5
        )
    {
        if (ibdsTxDigital->State != dsEdit)
            ibdsTxDigital->Edit();
        fldPlnEntr->AsInteger =
             fldSfnId->IsNull && fldAllotId->AsString.Length() == 0 ? 1 :
            !fldSfnId->IsNull && fldAllotId->AsString.Length() == 0 ? 2 :
            !fldSfnId->IsNull && fldAllotId->AsString.Length() > 0 ? 3 :
            /* fldSfnId->IsNull && fldAllotId->AsString.Length() > 0 */ 5;
    }

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::ibdsTxDigitalSYNHRONETIDChange(
      TField *Sender)
{
    SetPlanEntry();
    CheckSfn();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::ibdsTxDigitalASSOCIATED_ALLOT_SFN_IDChange(
      TField *Sender)
{
    CheckSfn();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::ibdsTxDigitalIDENTIFIERSFNChange(
      TField *Sender)
{
    SetPlanEntry();
    CheckSfn();
    CheckAssgnCode();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::ibdsTxDigitalASSOCIATED_ADM_ALLOT_IDChange(
      TField *Sender)
{
    SetPlanEntry();
    CheckAssgnCode();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::ibdsTxDigitalASSGN_CODEChange(
      TField *Sender)
{
    CheckAssgnCode();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::cbAssgnCodeChange(TObject *Sender)
{
    if (ibdsTxDigital->State != dsEdit)
        ibdsTxDigital->Edit();
    ibdsTxDigitalASSGN_CODE->AsString = cbAssgnCode->Text;
    cbAssgnCode->SelLength = 0;
    cbAssgnCode->SelStart = 0;
}
//---------------------------------------------------------------------------
void __fastcall TfrmTxBaseAirDigital::btAssocAllotClick(TObject *Sender)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = ibdsStantionsBase->Database;
    sql->SQL->Text = "select a.ID, a.ADM_REF_ID || ' - ' || a.ALLOT_NAME || ' (' || db.SECTIONNAME || ')' "
                    "from DIG_ALLOTMENT a "
                    "left join DATABASESECTION db on (a.DB_SECTION_ID = db.id) "
                    "where a.ADM_REF_ID = :ADM_REF_ID ";
    mnShowAllotment->Items->Clear();
    if (edAssocAllotId->Field)
    {
        sql->Params->Vars[0]->AsString = edAssocAllotId->Field->AsString;
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
        {
            TMenuItem *mni = new TMenuItem(this);
            mni->Caption = sql->Fields[1]->AsString;
            mni->Tag = sql->Fields[0]->AsInteger;
            mni->OnClick = mniShowAllotmentClick;
            mnShowAllotment->Items->Add(mni);
        }
    }

    TPoint p = btAssocAllot->Parent->ClientToScreen(TPoint(btAssocAllot->Left + btAssocAllot->Width, btAssocAllot->Top));
    if (mnShowAllotment->Items->Count > 1)
        mnShowAllotment->Popup(p.x, p.y);
    else if (mnShowAllotment->Items->Count == 1)
        FormProvider.ShowTx(txBroker.GetTx(mnShowAllotment->Items->Items[0]->Tag, CLSID_LisBcDigAllot));
    else {
        TMenuItem *mni = new TMenuItem(this);
        mni->Caption = "<нет таких выделений>";
        mni->Enabled = false;
        mnShowAllotment->Items->Add(mni);
        mnShowAllotment->Popup(p.x, p.y);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::mniShowAllotmentClick(
      TObject *Sender)
{
    TMenuItem *mni = dynamic_cast<TMenuItem*>(Sender);
    if (mni)
        FormProvider.ShowTx(txBroker.GetTx(mni->Tag, CLSID_LisBcDigAllot));
}

//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::sbExpToGs1Gt1Click(TObject *Sender)
{
    std::vector<int> idv;
    idv.push_back(id);
    TfrmRrc06Export::ExportRrc006(emGs1Gt1, idv);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxBaseAirDigital::chPolIsolClick(TObject *Sender)
{
    double val;
    Tx->get_pol_isol(&val);
    if (chPolIsol->Checked)
    {
        edPolIsol->Color = clWindow;
        edPolIsol->Font->Color = clWindowText;
        if (val == 0)
            Tx->set_pol_isol(16.0);
    }
    else
    {
        edPolIsol->Color = clBtnFace;
        edPolIsol->Font->Color = clBtnFace;
        if (val != 0)
            Tx->set_pol_isol(0.0);
    }
    Tx->get_pol_isol(&val);
    edPolIsol->OldValue = FormatFloat("0.#", val);
    edPolIsol->Enabled = chPolIsol->Checked;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::edPolIsolValueChange(TObject *Sender)
{
    double val = edPolIsol->Text.Length() > 0 ? StrToFloat(edPolIsol->Text) : 0.0;
    Tx->set_pol_isol(val);
    if (val == 0.0)
    {
        chPolIsol->Checked = false;
        chPolIsol->SetFocus();
        chPolIsolClick(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirDigital::cbxPolarizationChange(
      TObject *Sender)
{
    char pol = cbxPolarization->Text.IsEmpty() ? 0 : cbxPolarization->Text[1];
    TBCPolarization p = plCIR;
    switch (pol)
    {
        case 'H': p = plHOR;
                    chPolIsol->Enabled = true;
                    chPolIsol->Checked = false;
                    chPolIsolClick(Sender);
                    break;
        case 'V': p = plVER;
                    chPolIsol->Enabled = true;
                    chPolIsol->Checked = false;
                    chPolIsolClick(Sender);
                    break;
        case 'M': p = plMIX;
                    chPolIsol->Enabled = false;
                    chPolIsol->Checked = false;
                    chPolIsolClick(Sender);
                    break;
        default : p = plCIR;
                    chPolIsol->Enabled = false;
                    chPolIsol->Checked = false;
                    chPolIsolClick(Sender);
                    break;
    }

    Tx->set_polarization(p);

    AdjustDirControls();

    if (btnAntPattH->Visible)
        btnAntPattClick(btnAntPattH);
    else if (btnAntPattV->Visible)
        btnAntPattClick(btnAntPattV);

    RefreshAll();
}
//---------------------------------------------------------------------------

