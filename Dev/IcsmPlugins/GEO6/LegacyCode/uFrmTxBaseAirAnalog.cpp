 //---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmTxBaseAirAnalog.h"
#include "uMainDm.h"
#include "FormProvider.h"
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
TfrmTxBaseAirAnalog *frmTxBaseAirAnalog;
//---------------------------------------------------------------------------
__fastcall TfrmTxBaseAirAnalog::TfrmTxBaseAirAnalog(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBaseAir(Owner, in_Tx)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirAnalog::TxDataLoad()
{
    TfrmTxBaseAir::TxDataLoad();
}

void __fastcall TfrmTxBaseAirAnalog::TxDataSave()
{
    if (ibdsRetranslate->State == dsEdit)
        ibdsRetranslate->Post();
    TfrmTxBaseAir::TxDataSave();
}


void __fastcall TfrmTxBaseAirAnalog::acceptListElementSelection(Messages::TMessage &Message)
{
    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    TfrmTxBaseAir::acceptListElementSelection(Message);

    switch (Message.WParam) {
        case 39 :
            ibdsRetranslate->Edit();
            ibdsRetranslateRELAYSTATION_ID->AsInteger = Message.LParam;
            ibdsRetranslateTYPERECEIVE_ID->AsInteger = ibdsStantionsBaseID->AsInteger;
            ibdsRetranslate->Post();
            ibdsRetranslate->Active = false;
            ibdsRetranslate->Active = true;
            if (ibdsTestpoint->Active == false) {
                ibdsTestpoint->ParamByName("TRANSMITTERS_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
                ibdsTestpoint->Active = true;
            }
            ibdsTestpoint->Append();
            ibdsTestpointID->AsInteger = dmMain->getNewId();
            ibdsTestpointTRANSMITTERS_ID->AsInteger = Message.LParam;
            double DValue;
            Tx->get_latitude(&DValue);
            ibdsTestpointLATITUDE->AsFloat = DValue;
            Tx->get_longitude(&DValue);
            ibdsTestpointLONGITUDE->AsFloat = DValue;
            ibdsTestpointNAME->AsString = ibqStandNUMREG->AsString + "-" +
                                          ibdsStantionsBaseADMINISTRATIONID->AsString + "-" + ibqStandNAMESITE->AsString;

            ibdsTestpointTESTPOINT_TYPE->AsFloat = 1;
            ibdsTestpoint->Post();
            //ibdsTestpoint->Active = false;
            //ibdsTestpoint->ParamByName("TRANSMITTERS_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
            //ibdsTestpoint->Active = true;
            break;
         default :
            break;
    }
    sql->Close();
    delete sql;
}

void __fastcall TfrmTxBaseAirAnalog::dbcbTypeReciveChange(TObject *Sender)
{
    /*
    if (ibdsRetranslate->State != dsEdit)
        ibdsRetranslate->Edit();
    ibdsRetranslateTYPERECEIVE_ID->AsInteger = (int)dbcbTypeRecive->Items->Objects[dbcbTypeRecive->ItemIndex];
    */
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirAnalog::ibdsRetranslateAfterEdit(
      TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;       
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirAnalog::dbcbTypeReciveDropDown(
      TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft)
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxBaseAirAnalog::btnDeleteRelayClick(TObject *Sender)
{ 

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "delete from TESTPOINTS where TESTPOINT_TYPE = 1 and TRANSMITTERS_ID = " + ibdsRetranslateRELAYSTATION_ID->AsString;
    sql->ExecQuery();

    ibdsRetranslate->Edit();
    ibdsRetranslateRELAYSTATION_ID->SetData(NULL);
    ibdsRetranslate->Post();
    ibdsRetranslate->Active = false;
    ibdsRetranslate->Active = true;
}
//---------------------------------------------------------------------------




