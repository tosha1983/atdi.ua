//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmTxDVB.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include <values.h>
#include "uParams.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uFrmTxBaseAirDigital"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma link "NumericEdit"
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"
TfrmTxDVB *frmTxDVB;

//---------------------------------------------------------------------------
__fastcall TfrmTxDVB::TfrmTxDVB(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBaseAirDigital(Owner, in_Tx)
{
    if (!iDvbt2.IsBound() && Tx.IsBound())
        HrCheck(Tx->QueryInterface(IID_ILisBcDvbt2, (void**)&iDvbt2), "Запрос интерфейса ILisBcDvbt2");

    pnDvbt->BevelOuter = bvNone;
    pnDvbt->Caption = "";
    pnDvbt->Top = 0;
    pnDvbt->Left = 216;
    pnDvbt2->BevelOuter = bvNone;
    pnDvbt2->Caption = "";
    pnDvbt2->Top = 0;
    pnDvbt2->Left = 216;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::acceptListElementSelection(Messages::TMessage &Message)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;

    TfrmTxBaseAirDigital::acceptListElementSelection(Message);
    double centre_freq;
       switch (Message.WParam) {
         case 10 :
            if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
                throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));

            if (ibdsDVB->State != dsEdit)
                ibdsDVB->Edit();
//                   sql->SQL->Text = "update TRANSMITTERS set CHANNEL_ID = " + AnsiString(Message.LParam) + " where ID = " + ibdsStantionsBaseID->AsString;
//                   sql->ExecQuery();
//                   sql->Close();
                ibdsDVBCHANNEL_ID->AsInteger = Message.LParam;
                ibdsDVB->Post();
                sql->SQL->Text = "select NAMECHANNEL, FREQFROM, FREQTO from CHANNELS where ID = " + AnsiString(Message.LParam);
                sql->ExecQuery();
                centre_freq = (sql->Fields[1]->AsFloat + sql->Fields[2]->AsFloat) / 2.0;
                edtFreqCentre->OldValue = AnsiString(centre_freq);
                Tx->set_video_carrier(centre_freq);
                channel_id = Message.LParam;
                Tx->set_channel_id(channel_id);
                edtChannel->OldValue = sql->Fields[0]->AsString;
                sql->Close();

                SetRadiationClass();
            break;
        default:
            break;
    }
}



void __fastcall TfrmTxDVB::TxDataLoad()
{
    TfrmTxBaseAirDigital::TxDataLoad();

    cbxRpc->Items->Clear();
    cbxRpc->Items->AddObject("  ", (TObject*)Lisbctxserver_tlb::rpc0);
    cbxRpc->Items->AddObject(" 1", (TObject*)Lisbctxserver_tlb::rpc1);
    cbxRpc->Items->AddObject(" 2", (TObject*)Lisbctxserver_tlb::rpc2);
    cbxRpc->Items->AddObject(" 3", (TObject*)Lisbctxserver_tlb::rpc3);
    
    cbxRxMode->Items->Clear();
    cbxRxMode->Items->AddObject("FX", (TObject*)Lisbctxserver_tlb::rmFx);
    cbxRxMode->Items->AddObject("MO", (TObject*)Lisbctxserver_tlb::rmMo);
    cbxRxMode->Items->AddObject("PI", (TObject*)Lisbctxserver_tlb::rmPi);
    cbxRxMode->Items->AddObject("PO", (TObject*)Lisbctxserver_tlb::rmPo);

    cbSm->Items->Clear();
    cbSm->Items->AddObject("N", (TObject*)'S');
    cbSm->Items->AddObject("S", (TObject*)'S');

    cbModulation->Items->CommaText = ",QPSK,16-QAM,64-QAM,256-QAM";
    cbCodeRate->Items->CommaText = ",1/2,3/5,2/3,3/4,4/5,5/6";
    cbFftSize->Items->CommaText = ",1K,2K,4K,8K,16K,32K";
    cbGuardInterval->Items->CommaText = ",1/128,1/32,1/16,19/256,1/8,19/128,1/4";

    bool b = (ibdsStantionsBaseSTATUS->AsInteger == tsDraft);
    btnNullChannel->Enabled = b;
    edtChannel->Enabled = b;
    btnChannel->Enabled = b;
    edtFreqCentre->Enabled = b;
    cbModulation->Enabled = b;
    cbxTypeSysName->Enabled = b;
    cbGiFftSize->Enabled = b;

    TBcRxMode rxMode;
    Tx->get_rxMode(&rxMode);
    cbxRxMode->ItemIndex = cbxRxMode->Items->IndexOfObject((TObject*)rxMode);
    
    TBcRpc rpc;
    Tx->get_rpc(&rpc);
    cbxRpc->ItemIndex = cbxRpc->Items->IndexOfObject((TObject*)rpc);
    if(rpc != rpc0)
    {
        cbxTypeSysName->Enabled = false;
        cbxRxMode->Enabled = false;
    }
    else
    {
        cbxTypeSysName->Enabled = true;
        cbxRxMode->Enabled = true;
    }   

    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;
    sql->Transaction = tr;

    long LValue, channel_cbx_num;
    double DValue;

    ibdsDVB->Active= false;
    ibdsDVB->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsDVB->Active = true;

    TBCDVBSystem DVBValue;
    Tx->get_dvb_system(&DVBValue);

    int sys_id;
    sql->SQL->Text = "select NAMESYSTEM, ID, ENUMVAL from DIGITALTELESYSTEM";
    sql->ExecQuery();
    int i = 0, num = -1;
    cbxTypeSysName->Items->Clear();
    while (!sql->Eof) {
        cbxTypeSysName->Items->AddObject(sql->Fields[0]->AsString, (TObject*)sql->Fields[1]->AsInteger);
        if (sql->Fields[2]->AsInteger == (int)DVBValue)
            num = i;
        i++;
        sql->Next();
    }
    sql->Close();
    cbxTypeSysName->ItemIndex = systemIdx = num;

    ibqDigTSys->Close();
    ibqDigTSys->Open();
    Set<TLocateOption,0,1> flags;
    ibqDigTSys->Locate("ID", (int)cbxTypeSysName->Items->Objects[cbxTypeSysName->ItemIndex],flags);  
    edDvbtSystemInfo->Text = ibqDigTSysMODULATION->AsString + ", code rate " + ibqDigTSysCODERATE->AsString;

    edtChannel->Text = ibdsDVBNAMECHANNEL->AsString;
    channel_id = ibdsDVBCHANNEL_ID->AsInteger;

    edDvbtGiInfo->Text = "";

    sql->SQL->Text = "select ID, CODE, NUMBERCARRIER, NAMECURRIERGUARD from CARRIERGUARDINTERVAL";
    sql->ExecQuery();

    i = 0, num = -1;
    cbGiFftSize->Items->Clear();
    while (!sql->Eof) {
        cbGiFftSize->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        if (sql->Fields[0]->AsInteger == ibdsDVBGUARDINTERVAL_ID->AsInteger)
        {
            num = i;
            edDvbtGiInfo->Text = sql->Fields[2]->AsString + ", " + sql->Fields[3]->AsString;
        }
        i++;
        sql->Next();
    }

    sql->Close();
    delete sql;

    cbGiFftSize->ItemIndex = giIdx = num;

    Tx->get_video_carrier(&DValue);
    edtFreqCentre->Text = FormatFloat("0.###", DValue);

    cbPilotPattern->ItemIndex = 0;
    cbDiversity->ItemIndex = 0;
    cbModulation->ItemIndex = 0;
    cbCodeRate->ItemIndex = 0;
    cbFftSize->ItemIndex = 0;
    cbGuardInterval->ItemIndex = 0;

    if (!iDvbt2.IsBound())
        HrCheck(Tx->QueryInterface(IID_ILisBcDvbt2, (void**)&iDvbt2), "Запрос интерфейса ILisBcDvbt2");

    if (iDvbt2->get_IsDvbt2())
        InitControlsForDvbt2();
    else
        InitControlsForDvbt();

    unsigned ival;
    if ((ival = iDvbt2->get_PilotPattern()) <= 8)
        cbPilotPattern->ItemIndex = ival;

    if ((ival = iDvbt2->get_Diversity()) <= 4)
        cbDiversity->ItemIndex = ival;

    chRotatedCnstls->Checked = iDvbt2->get_RotatedConstellations();
    chModeOfExtnts->Checked = iDvbt2->get_ModeOfExtentions();

    if ((ival = iDvbt2->get_Modulation()) <= modQam256)
        cbModulation->ItemIndex = ival;

    if ((ival = iDvbt2->get_CodeRate()) <= cr_5_6)
        cbCodeRate->ItemIndex = ival;

    if ((ival = iDvbt2->get_FftSize()) <= fsz32K)
        cbFftSize->ItemIndex = ival;

    if ((ival = iDvbt2->get_GuardInterval()) <= gi_1_4)
        cbGuardInterval->ItemIndex = ival;

    lblEditing->Visible = false;
}

void __fastcall TfrmTxDVB::TxDataSave()
{
    if (ibdsDVB->State == dsEdit)
        ibdsDVB->Post();

    TfrmTxBaseAirDigital::TxDataSave();
}

void __fastcall TfrmTxDVB::FormCreate(TObject *Sender)
{
    Caption = "Передавач цифрового телебачення";
    Width = 780;
    TfrmTxBaseAir::FormCreate(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbxTypeSysNameChange(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbxTypeSysName->ItemIndex = systemIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }
    systemIdx = cbxTypeSysName->ItemIndex;

    Set<TLocateOption,0,1> flags;
    ibqDigTSys->Locate("ID", (int)cbxTypeSysName->Items->Objects[cbxTypeSysName->ItemIndex], flags);
    edDvbtSystemInfo->Text = ibqDigTSysMODULATION->AsString + ", code rate " + ibqDigTSysCODERATE->AsString;

    Tx->set_dvb_system((TBCDVBSystem)ibqDigTSysENUMVAL->AsInteger);
    SetRadiationClass();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbGiFftSizeChange(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbGiFftSize->ItemIndex = giIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }
    giIdx = cbGiFftSize->ItemIndex;

    if (ibdsDVB->State != dsEdit)
        ibdsDVB->Edit();
    int giid = (int)cbGiFftSize->Items->Objects[cbGiFftSize->ItemIndex];
    ibdsDVBGUARDINTERVAL_ID->AsInteger = giid;

    std::auto_ptr<TIBSQL>sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->Transaction = tr;
    sql->SQL->Text = "select ID, CODE, NUMBERCARRIER, NAMECURRIERGUARD from CARRIERGUARDINTERVAL where ID = "+IntToStr(giid);
    sql->ExecQuery();
    edDvbtGiInfo->Text = sql->Fields[2]->AsString + ", " + sql->Fields[3]->AsString;

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::btnChannelClick(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger == 1)
        FormProvider.ShowList((ObjType)10, this->Handle, channel_id);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::ibdsDVBAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbxTypeSysNameDropDown(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != 1)
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::btnNullChannelClick(TObject *Sender)
{
    Tx->set_channel_id(0);
    Tx->set_video_carrier(0.0);
    edtFreqCentre->Text = "";
    edtChannel->Text = "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::SetRadiationClass()
{
    //класс излучения
    AnsiString ClassRadiation = "";

    if ( edtFreqCentre->Text != "" )
    {
        ClassRadiation += Passband2Str(8 * 1000000);//полоса излучения [Гц]

        //первый символ
        ClassRadiation += "C";

        //второй символ
        ClassRadiation += "7";

        //третий символ
        ClassRadiation += "F";

        //четвертый символ
        ClassRadiation += "H";

        //пятый символ
        ClassRadiation += "C";
    }

    edtClassRadiationVideo->Text = ClassRadiation;
}
void __fastcall TfrmTxDVB::btnVideoEmissionClick(TObject *Sender)
{
  ibdsStantionsBase->Edit();
  ibdsStantionsBaseVIDEO_EMISSION->AsString = CalcVideoEmission();
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxDVB::cbxRpcChange(TObject *Sender)
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
                
                bool find = false;
                int temp_sys = cbxTypeSysName->ItemIndex;
                if (rpc == rpc1 || rpc == rpc2 || rpc == rpc3)
                {
                    HrCheck(Tx->set_rxMode(rpc == rpc1 ? rmFx : rpc == rpc2 ? rmPo : rmPi));
                    for(int i = 0; i < cbxTypeSysName->Items->Count; ++i)
                    {
                        cbxTypeSysName->ItemIndex = i;
                        cbxTypeSysNameChange(Sender);
                        if ((rpc == rpc1 && ibqDigTSysMODULATION->AsString == "64-QAM" && ibqDigTSysCODERATE->AsString == "3/4") ||
                            (rpc == rpc2 && ibqDigTSysMODULATION->AsString == "16-QAM" && ibqDigTSysCODERATE->AsString == "3/4") ||
                            (rpc == rpc3 && ibqDigTSysMODULATION->AsString == "16-QAM" && ibqDigTSysCODERATE->AsString == "2/3") )
                        {
                            find = true;
                            cbxTypeSysName->Enabled = false;
                            cbxRxMode->Enabled = false;
                            break;
                        }
                    }
                }
                else // rpc0
                {
                    cbxTypeSysName->Enabled = true;
                    cbxRxMode->Enabled = true;
                    find = true;
                }

                if(!find)
                {
                    cbxTypeSysName->ItemIndex = temp_sys;
                    cbxTypeSysNameChange(Sender);
                    throw *(new Exception(AnsiString("Не знайдено потрiбної системи мовлення!")));
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


void TfrmTxDVB::InitControlsForDvbt()
{
    rbDvbt->Checked = true;
    rbDvbt->TabStop = true;
    rbDvbt2->TabStop = false;
    rbDvbt->TabOrder = 0;

    pnDvbt->Visible = true;
    pnDvbt2->Visible = false;

    chRotatedCnstls->Visible = false;
    chModeOfExtnts->Visible = false;
}

void TfrmTxDVB::InitControlsForDvbt2()
{
    rbDvbt2->Checked = true;
    rbDvbt->TabStop = false;
    rbDvbt2->TabStop = true;
    rbDvbt2->TabOrder = 0;

    pnDvbt->Visible = false;
    pnDvbt2->Visible = true;

    chRotatedCnstls->Visible = true;
    chModeOfExtnts->Visible = true;
}

void __fastcall TfrmTxDVB::rbDvbtClick(TObject *Sender)
{
    if (iDvbt2.IsBound())
        iDvbt2->set_IsDvbt2(false);
    InitControlsForDvbt();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::rbDvbt2Click(TObject *Sender)
{
    if (iDvbt2.IsBound())
    {
        iDvbt2->set_IsDvbt2(true);
        InitControlsForDvbt2();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbPilotPatternChange(TObject *Sender)
{
    if (iDvbt2.IsBound())
        iDvbt2->set_PilotPattern(cbPilotPattern->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbDiversityChange(TObject *Sender)
{
    if (iDvbt2.IsBound())
        iDvbt2->set_Diversity(cbDiversity->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::chRotatedCnstlsClick(TObject *Sender)
{
    if (iDvbt2.IsBound())
        iDvbt2->set_RotatedConstellations(chRotatedCnstls->Checked);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::chModeOfExtntsClick(TObject *Sender)
{
    if (iDvbt2.IsBound())
        iDvbt2->set_ModeOfExtentions(chModeOfExtnts->Checked);
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxDVB::cbModulationChange(TObject *Sender)
{
    iDvbt2->set_Modulation(cbModulation->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbCodeRateChange(TObject *Sender)
{
     iDvbt2->set_CodeRate(cbCodeRate->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbGuardIntervalChange(TObject *Sender)
{
    iDvbt2->set_GuardInterval(cbGuardInterval->ItemIndex);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDVB::cbFftSizeChange(TObject *Sender)
{
    iDvbt2->set_FftSize(cbFftSize->ItemIndex);
}
//---------------------------------------------------------------------------


