//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmTxDAB.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include <values.h>
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
TfrmTxDAB *frmTxDAB;

//---------------------------------------------------------------------------
__fastcall TfrmTxDAB::TfrmTxDAB(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBaseAirDigital(Owner, in_Tx)
{
}

//---------------------------------------------------------------------------

void __fastcall TfrmTxDAB::acceptListElementSelection(Messages::TMessage &Message)
{
    TfrmTxBaseAirDigital::acceptListElementSelection(Message);

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;

    double cFreq = 0.0;

    switch (Message.WParam) {
         case otBLOCK_DAB :
            if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
                throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));

            if (ibdsDAB->State != dsEdit)
                    ibdsDAB->Edit();
            ibdsDABALLOTMENTBLOCKDAB_ID->AsInteger =  Message.LParam;

            sql->SQL->Text = "select BD.CENTREFREQ BD_CENTREFREQ, BD.FREQFROM BD_FREQFROM, BD.FREQTO BD_FREQTO, BD.NAME BLOCK_NAME "
                                " from BLOCKDAB BD where BD.ID = " + ibdsDABALLOTMENTBLOCKDAB_ID->AsString;
            sql->ExecQuery();
            cFreq = sql->Fields[0]->AsDouble;

            ibdsDABBLOCK_NAME->AsString = sql->Fields[3]->AsString;
            ibdsDABBD_FREQFROM->AsFloat = sql->Fields[1]->AsDouble;
            ibdsDABBD_FREQTO->AsFloat = sql->Fields[2]->AsDouble;
            edtFreqCentre->Text = FormatFloat("0.###", cFreq);
            ibdsDAB->Post();

            Tx->set_blockcentrefreq(cFreq);

            SetRadiationClass();
            break;
         default :
            break;
    }
    sql->Close();
}

void __fastcall TfrmTxDAB::TxDataLoad()
{
    TfrmTxBaseAirDigital::TxDataLoad();

    cbxRpc->Items->Clear();
    cbxRpc->Items->AddObject(" 4", (TObject*)Lisbctxserver_tlb::rpc4);
    cbxRpc->Items->AddObject(" 5", (TObject*)Lisbctxserver_tlb::rpc5);

    cbxRxMode->Items->Clear();
    cbxRxMode->Items->AddObject("MO", (TObject*)Lisbctxserver_tlb::rmMo);
    cbxRxMode->Items->AddObject("PI", (TObject*)Lisbctxserver_tlb::rmPi);
    cbxRxMode->Items->AddObject("PO", (TObject*)Lisbctxserver_tlb::rmPo);

    cbSm->Items->Clear();
    cbSm->Items->AddObject("1", (TObject*)'1');
    cbSm->Items->AddObject("2", (TObject*)'2');
    cbSm->Items->AddObject("3", (TObject*)'3');

    TBcRpc rpc;
    Tx->get_rpc(&rpc);
    cbxRpc->ItemIndex = cbxRpc->Items->IndexOfObject((TObject*)rpc);

    ibdsDAB->Active = false;
    ibdsDAB->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsDAB->Active = true;

    double DValue;
    Tx->get_blockcentrefreq(&DValue);
    edtFreqCentre->Text = FormatFloat("0.###", DValue);

    lblEditing->Visible = false;
}

void __fastcall TfrmTxDAB::TxDataSave()
{
    TfrmTxBaseAirDigital::TxDataSave();
}

void __fastcall TfrmTxDAB::FormCreate(TObject *Sender)
{
    Caption = "Передавач цифрового радіомовлення";
    Width = 775;
    TfrmTxBaseAir::FormCreate(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDAB::btnBlockClick(TObject *Sender)
{
    FormProvider.ShowList(otBLOCK_DAB, this->Handle, ibdsDABID->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxDAB::ibdsDABAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------
void __fastcall TfrmTxDAB::SetRadiationClass()
{
    //класс излчения
    AnsiString ClassRadiation = "";

    if ( ( edtFreqFrom->Text != "" ) && ( edtFreqTo->Text != "" ) )
    {
        ClassRadiation += Passband2Str((StrToFloat(edtFreqTo->Text) - StrToFloat(edtFreqFrom->Text)) * 1000000);//полоса излучения [Гц]

        //первый символ
        ClassRadiation += "X";

        //второй символ
        ClassRadiation += "2";

        //третий символ
        ClassRadiation += "E";

        //четвертый символ
        ClassRadiation += "H";
    }
    edtClassRadiationVideo->Text = ClassRadiation;
}
void __fastcall TfrmTxDAB::btnSoundEmissionPrimaryClick(TObject *Sender)
{
    ibdsStantionsBase->Edit();
    ibdsStantionsBaseSOUND_EMISSION_PRIMARY->AsString = CalcSoundEmission();
}
//---------------------------------------------------------------------------



