//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include <IBSQL.hpp>
#include "uFrmTxBase.h"
#include "uFrmTxVHF.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include <values.h>
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uFrmTxBaseAirAnalog"
#pragma link "xlcClasses"
#pragma link "xlEngine"
#pragma link "xlReport"
#pragma link "NumericEdit"
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"
TfrmTxVHF *frmTxVHF;

//---------------------------------------------------------------------------
__fastcall TfrmTxVHF::TfrmTxVHF(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBaseAirAnalog(Owner, in_Tx)
{
}                                     

//---------------------------------------------------------------------------

void __fastcall TfrmTxVHF::acceptListElementSelection(Messages::TMessage &Message)
{
    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    TfrmTxBaseAirAnalog::acceptListElementSelection(Message);

    switch (Message.WParam) {
         case -1 :
//                  try {
//                    Tx->set_typesystem((TBCTvSystems)(sql->Fields[0]->AsInteger));
//                  } catch (...) {;}
              break;
         default : ;
    }
    sql->Close();
    delete sql;

}


void __fastcall TfrmTxVHF::TxDataLoad()
{
    TfrmTxBaseAirAnalog::TxDataLoad();

    ibqTypeSystemName->Active = false; ibqTypeSystemName->Active = true;
    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    TBCFMSystem FMValue;
    Tx->get_fm_system(&FMValue);
    sql->SQL->Text = "select ID, CODSYSTEM, ENUMVAL, TYPESYSTEM from ANALOGRADIOSYSTEM";
    sql->ExecQuery();
    int nom = -1 , i = 0;
    cbxTypeSysName->Items->Clear();
    while (!sql->Eof) {
        cbxTypeSysName->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        if ((int)FMValue == sql->Fields[2]->AsInteger)
        {
            nom = i;
        }
        i++;
        sql->Next();
    }
    sql->Close();
    delete sql;
    cbxTypeSysName->ItemIndex = nom;

    long LValue;
    double DValue;
    Tx->get_monostereo_primary(&LValue);
    cbMonoStereo->ItemIndex = LValue;

    Tx->get_sound_carrier_primary(&DValue);
    edtFreq->OldValue = FormatFloat("0.###", DValue);

    lblEditing->Visible = false;

    bool isDraft = ibdsStantionsBaseSTATUS->AsInteger == tsDraft;
    edtFreq->ReadOnly = !isDraft;
    cbxTypeSysName->Enabled = isDraft;
    pnlForVHF->Enabled = isDraft;
    cbMonoStereo->Enabled = isDraft;
}

void __fastcall TfrmTxVHF::TxDataSave()
{
    TfrmTxBaseAirAnalog::TxDataSave();
}

void __fastcall TfrmTxVHF::cbMonoStereoChange(TObject *Sender)
{
    if (cbMonoStereo->ItemIndex >-1)
        Tx->set_monostereo_primary(cbMonoStereo->ItemIndex);
    else {
        long LValue;
        Tx->get_monostereo_primary(&LValue);
        if (LValue ==0)
            cbMonoStereo->ItemIndex = 0;
        else
            cbMonoStereo->ItemIndex = 1;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxVHF::FormCreate(TObject *Sender)
{
    Caption = "Передавач аналогового радіомовлення";
    Width = 775;
    TfrmTxBaseAir::FormCreate(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxVHF::btnSystemVHFCastClick(TObject *Sender)
{
    FormProvider.ShowList(3, this->Handle, 0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxVHF::cbxTypeSysNameChange(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        /* TODO: восстановить значение cbxTypeSysName->ItemIndex */
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ENUMVAL, TYPESYSTEM from ANALOGRADIOSYSTEM where ID = " +
                    AnsiString((int)cbxTypeSysName->Items->Objects[cbxTypeSysName->ItemIndex]);
    sql->ExecQuery();
    Tx->set_fm_system(sql->Fields[0]->AsInteger);
    if (sql->Fields[1]->AsString == "Stereo")
        cbMonoStereo->ItemIndex = 1;
    else
        cbMonoStereo->ItemIndex = 0;
    cbMonoStereoChange(this);
    sql->Close();

    SetRadiationClass();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxVHF::SetRadiationClass()
{
    //класс излчения
    AnsiString ClassRadiation = "";

/*
    if ( cbxTypeSysName->Text != "" )
    {
        if ( ( cbxTypeSysName->Text == "System 1" ) || (cbxTypeSysName->Text == "System 4") )
            ClassRadiation += Passband2Str( 150 * 1000);//полоса излучения [Гц]
        else
            ClassRadiation += Passband2Str( 100 * 1000);//полоса излучения [Гц]

        //первый символ
        ClassRadiation += "F";

        //второй символ
        ClassRadiation += "3";

        //третий символ
        ClassRadiation += "F";

        //четвертый символ
        ClassRadiation += "G";

        //пятый символ
        ClassRadiation += "N";
    }

*/
    if ( cbxTypeSysName->Text != "" )
    {
        if ( cbxTypeSysName->Text == "System 1" )
            ClassRadiation = "180KF3EGN";
        if ( cbxTypeSysName->Text == "System 2" )
            ClassRadiation = "130KF3EGN";
        if ( cbxTypeSysName->Text == "System 3" )
            ClassRadiation = "180KF3EHN";
        if ( cbxTypeSysName->Text == "System 4" )
            ClassRadiation = "300KP9EHF";
        if ( cbxTypeSysName->Text == "System 5" )
            ClassRadiation = "300KF8EHF";
    }

    edtClassRadiationVideo->Text = ClassRadiation;
}

void __fastcall TfrmTxVHF::edtFreqValueChange(TObject *Sender)
{
    double freq, DValue;
    try {
        if (edtFreq->Text.Length() > 0)
            freq = StrToFloat(edtFreq->Text);
        else
            freq = 0;
        /*
        if ((freq < 66.0) || (freq > 107.9))
            throw *(new Exception(AnsiString("Помилка диапазону АРМ!")));
        */
        Tx->set_sound_carrier_primary(freq);
        edtFreq->OldValue = FormatFloat("0.###", freq);
    } catch (Exception &e) {
        Tx->get_sound_carrier_primary(&DValue);
        edtFreq->OldValue = AnsiString(DValue);
    }
    if (DValue > 300.0)
        edtClassWave->Text = "UHF";
    else
        edtClassWave->Text = "VHF";
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxVHF::actIntoBaseExecute(TObject *Sender)
{
    double freq;
    if (edtFreq->Text.Length() > 0)
        freq = StrToFloat(edtFreq->Text);
    else
        freq = 0;

    if ((freq < 65.9) || (freq > 107.9))
        throw *(new Exception(AnsiString("Невірний діапазон")));

    TfrmTxBaseAirAnalog::actIntoBaseExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxVHF::btnSoundEmissionPrimaryClick(TObject *Sender)
{
  ibdsStantionsBase->Edit();
  ibdsStantionsBaseSOUND_EMISSION_PRIMARY->AsString = CalcSoundEmission();
        
}
//---------------------------------------------------------------------------



