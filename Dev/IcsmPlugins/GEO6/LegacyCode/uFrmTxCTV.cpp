//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop                       

#include "uFrmTxCTV.h"   
#include "uMainDm.h"
#include "FormProvider.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uFrmTxBase"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma link "NumericEdit"
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"
TfrmTxCTV *frmTxCTV;
//---------------------------------------------------------------------------
__fastcall TfrmTxCTV::TfrmTxCTV(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBase(Owner, in_Tx)
{
}

void __fastcall TfrmTxCTV::acceptListElementSelection(Messages::TMessage &Message)
{
    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    TfrmTxBase::acceptListElementSelection(Message);

    switch (Message.WParam) {
         case 10 :
              sql->SQL->Text = "select NAMECHANNEL, FREQFROM, FREQTO from CHANNELS where ID = " + AnsiString(Message.LParam);
              sql->ExecQuery();
              if ((ibdsChannelCtv->State != dsEdit)&& (ibdsChannelCtv->State != dsInsert))
                  ibdsChannelCtv->Edit();
              if (FLAG_RX_TX == rxtxRX) {
              ibdsChannelCtvRX_CHANNEL->AsString = sql->Fields[0]->AsString;
              ibdsChannelCtvRX_FREQUENCY->AsFloat = (sql->Fields[1]->AsFloat + sql->Fields[2]->AsFloat) / 2.0;
              } else {
              ibdsChannelCtvTX_CHANNEL->AsString = sql->Fields[0]->AsString;
              ibdsChannelCtvTX_FREQUENCY->AsFloat = (sql->Fields[1]->AsFloat + sql->Fields[2]->AsFloat) / 2.0;
              }
              break;
         default : ;
    }
    sql->Close();
    delete sql;
}


void __fastcall TfrmTxCTV::TxDataLoad()
{
    TfrmTxBase::TxDataLoad();

    ibdsLicenseCTV->Active = false;
    ibdsLicenseCTV->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsLicenseCTV->Active = true;

    ibdsChannelCtv->Active = false;
    ibdsChannelCtv->ParamByName("TX_ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsChannelCtv->Active = true;
    ibdsChannelCtv->FetchAll();

    dsChannelCtv->Enabled= false;
    num_tv_channel = 0;
    while (!ibdsChannelCtv->Eof)
    {
        if (ibdsChannelCtvTYPESYSTEM->AsInteger)
            num_tv_channel++;
        ibdsChannelCtv->Next();
    }
    num_radio_channel = ibdsChannelCtv->RecordCount - num_tv_channel;
    edtChannelTVCount->OldValue = AnsiString(num_tv_channel);
    edtChannelFMCount->OldValue = AnsiString(num_radio_channel);


    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;
    sql->SQL->Text =    "select distinct(NAMEPROGRAMM) from TRANSMITTERS";
    sql->ExecQuery();
    while (!sql->Eof)
    {
        dbgChannelCTV->Columns->Items[5]->PickList->Add(sql->Fields[0]->AsString);
        sql->Next();
    }
    sql->Close();
    delete sql;

    ibdsChannelCtv->First();
    dsChannelCtv->Enabled= true;

    if ((ibdsLicenseCTVREGIONALCOUNCIL->IsNull)||(!ibdsLicenseCTVREGIONALCOUNCIL->AsFloat))
        dtpPermUseBeg->Date = Date();
    else
        dtpPermUseBeg->Date = ibdsLicenseCTVREGIONALCOUNCIL->AsDateTime;

    lblEditing->Visible = false;
}


void __fastcall TfrmTxCTV::TxDataSave()
{
    if ((ibdsLicenseCTV->State == dsEdit)||(ibdsLicenseCTV->State == dsInsert))
        ibdsLicenseCTV->Post();
    if ((ibdsChannelCtv->State == dsEdit)||(ibdsChannelCtv->State == dsInsert))
        ibdsChannelCtv->Post();
    TfrmTxBase::TxDataSave();
}

void __fastcall TfrmTxCTV::FormCreate(TObject *Sender)
{
    Caption = "Передавач кабельного телебачення";
    Width = 790;
    TfrmTxBase::FormCreate(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::ibdsLicenseCTVAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::ibdsChannelCtvBeforePost(TDataSet *DataSet)
{

    if (ibdsChannelCtvID->IsNull)
    if (ibdsChannelCtvTYPESYSTEM->AsInteger)
    {
        num_tv_channel++;
        edtChannelTVCount->Text = AnsiString(num_tv_channel);
    } else {
        num_radio_channel++;
        edtChannelFMCount->Text = AnsiString(num_radio_channel);
    }

    if (ibdsChannelCtvID->IsNull)
    {
        int newId = dmMain->getNewId();
        if (newId > 0)
            ibdsChannelCtvID->AsInteger = newId;
    }
    if (ibdsChannelCtvTRANSMITTERS_ID->IsNull)
        ibdsChannelCtvTRANSMITTERS_ID->AsInteger = ibdsStantionsBaseID->AsInteger;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::ibdsChannelCtvTYPESYSTEMGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else if (Sender->AsInteger)
        Text = "канал";
    else
        Text = "частота";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::ibdsChannelCtvTYPESYSTEMSetText(TField *Sender,
      const AnsiString Text)
{
    if (Text == "канал")
        Sender->AsInteger = 1;
    else
        Sender->AsInteger = 0;
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxCTV::ibdsChannelCtvBeforeDelete(TDataSet *DataSet)
{
    if (ibdsChannelCtvTYPESYSTEM->AsInteger)
    {
        num_tv_channel--;
        edtChannelTVCount->Text = AnsiString(num_tv_channel);
    } else {
        num_radio_channel--;
        edtChannelFMCount->Text = AnsiString(num_radio_channel);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::dbgChannelCTVEditButtonClick(TObject *Sender)
{
    if (dbgChannelCTV->SelectedField->FieldName == "RX_CHANNEL") {
        FLAG_RX_TX = rxtxRX;
        FormProvider.ShowList(10, this->Handle, 0);
    } else if (dbgChannelCTV->SelectedField->FieldName == "TX_CHANNEL") {
        FLAG_RX_TX = rxtxTX;
        FormProvider.ShowList(10, this->Handle, 0);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::dtpPermRegionChange(TObject *Sender)
{
    if (ibdsLicenseCTV->State != dsEdit)
        ibdsLicenseCTV->Edit();
    ibdsLicenseCTVREGIONALCOUNCIL->AsDateTime = dtpPermUseBeg->Date;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxCTV::SetRadiationClass()
{
    //unabstract this method
}


