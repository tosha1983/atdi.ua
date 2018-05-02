//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFrmTxTVA.h"

#include <IBSQL.hpp>
#include "uFrmTxBase.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include <values.h>
#include <math>
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uFrmTxBaseAirAnalog"
#pragma link "NumericEdit"
#pragma link "CustomMap"
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"
TfrmTxTVA *frmTxTVA;
    
//---------------------------------------------------------------------------
__fastcall TfrmTxTVA::TfrmTxTVA(TComponent* Owner, ILISBCTx *in_Tx)
        : TfrmTxBaseAirAnalog(Owner, in_Tx)
{
}

//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::acceptListElementSelection(Messages::TMessage &Message)
{
    TfrmTxBaseAirAnalog::acceptListElementSelection(Message);
}

void __fastcall TfrmTxTVA::TxDataLoad()
{
    TfrmTxBaseAirAnalog::TxDataLoad();

    ibdsTVA->Active = false;
    ibdsTVA->ParamByName("ID")->AsInteger = ibdsStantionsBaseID->AsInteger;
    ibdsTVA->Active = true;

    TIBSQL *sql;
    sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    long LValue;
    double DValue;

    mono_stereo_click = true;

    TBCTvSystems SYSValue;
    int sys_id;
    Tx->get_typesystem(&SYSValue);
    sql->SQL->Text = "select ENUMVAL, NAMESYSTEM, ID  from ANALOGTELESYSTEM";
    sql->ExecQuery();
    int nom;
    systemIdx = -1;
    int i = 0;
    cbxTypeSysName->Items->Clear();
    while (!sql->Eof) {
        cbxTypeSysName->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[2]->AsInteger);
        if ((int)SYSValue == sql->Fields[0]->AsInteger) {
            sys_id = sql->Fields[2]->AsInteger;
            systemIdx = i;
        }
        i++;
        sql->Next();
    }
    sql->Close();

    cbxTypeSysName->ItemIndex = systemIdx;

    Tx->get_channel_id(&LValue);
    sql->SQL->Text = "select ID, NAMECHANNEL from CHANNELS where FREQUENCYGRID_ID = (select FREQUENCYGRID_ID from ANALOGTELESYSTEM where ID = " + AnsiString(sys_id) + ") order by NAMECHANNEL";
    sql->ExecQuery();
    cbxChannel->Items->Clear();

    int NumIndex = 0;
    channelIdx = -1;
    cbxChannel->Items->Clear();
    while (!sql->Eof) {
        cbxChannel->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);

        if (sql->Fields[0]->AsInteger == LValue)
            channelIdx = NumIndex;
        NumIndex++;
        sql->Next();
    }
    cbxChannel->ItemIndex = channelIdx;
    sql->Close();


    TBCTvStandards ColValue;
    Tx->get_systemcolor(&ColValue);
    if (ColValue == csPAL) cbSystemcolor->ItemIndex = 1;
    else if (ColValue == csSECAM) cbSystemcolor->ItemIndex = 2;
    else if (ColValue == csNTSC) cbSystemcolor->ItemIndex = 3;
    else cbSystemcolor->ItemIndex = 0;

    colorIdx = cbSystemcolor->ItemIndex;

    Tx->get_video_offset_line(&LValue);
    sql->SQL->Text = "select ID, OFFSETLINES, CODEOFFSET, OFFSET from OFFSETCARRYFREQTVA";
    sql->ExecQuery();
    offsetIdx = -1;
    i = 0;
    cbxVideoOffsetLine->Items->Clear();
    cbxVideoOffsetHerz->Items->Clear();
    while (!sql->Eof) {
        cbxVideoOffsetLine->Items->AddObject(sql->Fields[2]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        cbxVideoOffsetHerz->Items->AddObject(AnsiString(sql->Fields[3]->AsFloat/1000.0), (TObject*)sql->Fields[0]->AsInteger);
        if (LValue == sql->Fields[1]->AsInteger) {
            offsetIdx = i;
            }
        i++;
        sql->Next();
    }
    sql->Close();

    cbxVideoOffsetLine->ItemIndex = offsetIdx;
    cbxVideoOffsetHerz->ItemIndex = offsetIdx;

    Tx->get_video_carrier(&DValue);
    edtFreqVideo->OldValue = FormatFloat("0.###", DValue);

    Tx->get_sound_carrier_primary(&DValue);
    edtFreqAudio1->OldValue = FormatFloat("0.###", DValue);

    Tx->get_sound_carrier_second(&DValue);
    edtFreqAudio2->OldValue = FormatFloat("0.###", DValue);

    Tx->get_epr_video_max(&DValue);
    edtEPRmaxVideo->OldValue = FormatFloat("0.##", DValue);

    Tx->get_epr_video_hor(&DValue);
    edtEPRGVideo->OldValue = FormatFloat("0.##", DValue);

    Tx->get_epr_video_vert(&DValue);
    edtEPRVVideo->OldValue = FormatFloat("0.##", DValue);

    Tx->get_power_video(&DValue);
    edtPowerVideo->OldValue = FormatFloat("0.####", DValue);

    Tx->get_v_sound_ratio_primary(&DValue);
    edtVSoundRatio1->OldValue = FormatFloat("0.###", DValue);
////////////// ///////////////================================================
    Tx->get_epr_sound_max_second(&DValue);
    edtEPRmaxAudio2->OldValue = FormatFloat("0.##", DValue);

    Tx->get_epr_sound_hor_second(&DValue);
    edtEPRGAudio2->OldValue = FormatFloat("0.##", DValue);

    Tx->get_epr_sound_vert_second(&DValue);
    edtEPRVAudio2->OldValue = FormatFloat("0.##", DValue);

    Tx->get_power_sound_second(&DValue);
    edtPowerAudio2->OldValue = FormatFloat("0.####", DValue);

    Tx->get_v_sound_ratio_second(&DValue);
    edtVSoundRatio2->OldValue = FormatFloat("0.###", DValue);

    mono_stereo_click = false;
// Tx->get_monostereo_primary(&LValue);
// if (LValue) {
//     chbMonoStereo->State = cbChecked;
//     edtClassRadiationAudio2->Visible = true;
//     edtFreqAudio2->Visible = true;
//     edtEPRGAudio2->Visible = true;
//     edtEPRmaxAudio2->Visible = true;
//     edtEPRVAudio2->Visible = true;
//     edtPowerAudio2->Visible = true;
//     edtVSoundRatio2->Visible = true;
// }
// else {
         chbMonoStereo->State = cbUnchecked;
         edtClassRadiationAudio2->Visible = false;
         edtFreqAudio2->Visible = false;
         edtEPRGAudio2->Visible = false;
         edtEPRmaxAudio2->Visible = false;
         edtEPRVAudio2->Visible = false;
         edtPowerAudio2->Visible = false;
         edtVSoundRatio2->Visible = false;
// }

    if (cbxPolarization->Text == "H")  {
        edtEPRVVideo->Visible = false;
        edtEPRGVideo->Visible = true;
        edtEPRVAudio2->Visible = false;
        if (LValue) edtEPRGAudio2->Visible = false;
    } else if (cbxPolarization->Text == "V")  {
        edtEPRGVideo->Visible = false;
        edtEPRGAudio2->Visible = false;
        edtEPRVVideo->Visible = true;
        if (LValue) edtEPRVAudio2->Visible = false;
    }

    lblEditing->Visible = false;

    if (ibdsStantionsBaseSTATUS->AsInteger == 0) {
        btnNullChannel->Enabled = false;
        dbcbTypeOffset->Field->ReadOnly = true;
        cbxFreqStability->Field->ReadOnly = true;
    } else {
        btnNullChannel->Enabled = true;
        dbcbTypeOffset->Field->ReadOnly = false;
        cbxFreqStability->Field->ReadOnly = false;
    }

     bool b = (ibdsStantionsBaseSTATUS->AsInteger == tsDraft);

    cbxChannel->Enabled = b;
    cbSystemcolor->Enabled = b;
    cbxTypeSysName->Enabled = b;
    btnNullChannel->Enabled = b;

   delete sql;
}

void __fastcall TfrmTxTVA::TxDataSave()
{
    if (ibdsTVA->State == dsEdit)
        ibdsTVA->Post();
    TfrmTxBaseAirAnalog::TxDataSave();
}

void __fastcall TfrmTxTVA::chbMonoStereoClick(TObject *Sender)
{
    long LValue;

    Tx->get_monostereo_primary(&LValue);

    if (chbMonoStereo->State == cbChecked) {
        edtClassRadiationAudio2->Visible = true;
        edtFreqAudio2->Visible = true;
        edtEPRmaxAudio2->Visible = true;
        edtEPRVAudio2->Visible = true;
        edtPowerAudio2->Visible = true;
        edtVSoundRatio2->Visible = true;
        edtEPRGAudio2->Visible = true;
        if (cbxPolarization->Text == "V")
            edtEPRGAudio2->Visible = false;
        if (cbxPolarization->Text == "H")
            edtEPRVAudio2->Visible = false;


        if (mono_stereo_click)
            if (!LValue)
                Tx->set_monostereo_primary(1);
    } else {
        edtClassRadiationAudio2->Visible = false;
        edtFreqAudio2->Visible = false;
        edtEPRGAudio2->Visible = false;
        edtEPRmaxAudio2->Visible = false;
        edtPowerAudio2->Visible = false;
        edtVSoundRatio2->Visible = false;
        edtEPRVAudio2->Visible = false;

        if (mono_stereo_click)
            if (LValue)
                Tx->set_monostereo_primary(0);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::dbcbClassRadiationAudio2Exit(TObject *Sender)
{
   try {
     Tx->set_sound_carrier_second(StrToFloat(edtFreqAudio2->Text));
   } catch (Exception &e) {
    double DValue;
    Tx->get_sound_carrier_second(&DValue);
    edtFreqAudio2->Text = AnsiString(DValue);
  }

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::FormCreate(TObject *Sender)
{
    if (ibdsTVA->Active != true )ibdsTVA->Active = true;
    Caption = "Передавач аналогового телебачення";
    Width = 800;
    TfrmTxBaseAir::FormCreate(Sender);

    bool b = (ibdsStantionsBaseSTATUS->AsInteger == tsDraft);

    cbxChannel->Enabled = b;
    cbSystemcolor->Enabled = b;
    cbxTypeSysName->Enabled = b;
    btnNullChannel->Enabled = b;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::cbxTypeSysNameChange(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbxTypeSysName->ItemIndex = systemIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }

    systemIdx = cbxTypeSysName->ItemIndex;

    cbxChannel->Items->Clear();
    int idx = cbxTypeSysName->ItemIndex;
    if (idx == -1) {
        cbxChannelChange(this);
        return;
    }

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    int sys_id;
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ENUMVAl, ID from ANALOGTELESYSTEM where ID = "
            + AnsiString((int)cbxTypeSysName->Items->Objects[idx]);
    sql->ExecQuery();
    Tx->set_typesystem((TBCTvSystems)sql->Fields[0]->AsInteger);
    sys_id = sql->Fields[1]->AsInteger;
    sql->Close();

    //sql->SQL->Text = "select ID, NAMECHANNEL from CHANNELS where ANALOGTELESYSTEM_ID = " + AnsiString(sys_id) + " order by NAMECHANNEL ";
    sql->SQL->Text = "select ID, NAMECHANNEL from CHANNELS where FREQUENCYGRID_ID = (select FREQUENCYGRID_ID from ANALOGTELESYSTEM where ID = " + AnsiString(sys_id) + ") order by NAMECHANNEL";
    sql->ExecQuery();

    while (!sql->Eof) {
        cbxChannel->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        sql->Next();
    }
    cbxChannel->ItemIndex = 0;
    sql->Close();
    cbxChannelChange(this);

    SetRadiationClass();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::cbSystemcolorChange(TObject *Sender)
{
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbSystemcolor->ItemIndex = colorIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }

    colorIdx = cbSystemcolor->ItemIndex;

    if (cbSystemcolor->ItemIndex == 1)
        Tx->set_systemcolor(csPAL);
    else if (cbSystemcolor->ItemIndex == 2)
        Tx->set_systemcolor(csSECAM);
    else if (cbSystemcolor->ItemIndex == 3)
        Tx->set_systemcolor(csNTSC);
    else
        Tx->set_systemcolor(csUNKNOWN);
    SetRadiationClass();
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxTVA::cbxChannelChange(TObject *Sender)
{

    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbxChannel->ItemIndex = channelIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }

    channelIdx = cbxChannel->ItemIndex;

    if (cbxChannel->ItemIndex == -1) {
        Tx->set_channel_id(0);
        edtFreqVideo->Text = "";
        edtFreqAudio1->Text = "";
        edtFreqAudio2->Text = "";
        return;
    }

    Tx->set_channel_id((long)cbxChannel->Items->Objects[cbxChannel->ItemIndex]);

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select FREQCARRIERVISION, FREQCARRIERSOUND, "
                       " FREQCARRIERNICAM from CHANNELS where ID = " +
                        AnsiString((int)cbxChannel->Items->Objects[cbxChannel->ItemIndex]);
    sql->ExecQuery();
    edtFreqVideo->Text = sql->FieldByName("FREQCARRIERVISION")->AsString;
    if (edtFreqVideo->Text != "") {
        try {
            Tx->set_video_carrier(StrToFloat(edtFreqVideo->Text));
        } catch (Exception &e) {
            double DValue;
            Tx->get_video_carrier(&DValue);
            edtFreqVideo->Text = AnsiString(DValue);
        }
    }

    edtFreqAudio1->Text = sql->FieldByName("FREQCARRIERSOUND")->AsString;
    if (edtFreqAudio1->Text != "") {
        try {
            Tx->set_sound_carrier_primary(StrToFloat(edtFreqAudio1->Text));
        } catch (Exception &e) {
            double DValue;
            Tx->get_sound_carrier_primary(&DValue);
            edtFreqAudio1->Text = AnsiString(DValue);
        }
    }

    edtFreqAudio2->Text = sql->FieldByName("FREQCARRIERNICAM")->AsString;
    if (edtFreqAudio2->Text != "") {
        try {
            Tx->set_sound_carrier_second(StrToFloat(edtFreqAudio2->Text));
        } catch (Exception &e) {
            double DValue;
            Tx->get_sound_carrier_second(&DValue);
            edtFreqAudio2->Text = AnsiString(DValue);
        }
    }

    if (sql->FieldByName("FREQCARRIERVISION")->AsFloat > 300.0)
        edtClassWave->Text = "UHF";
    else
        edtClassWave->Text = "VHF";
    RefreshAll();    
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::btnTypeSystemClick(TObject *Sender)
{
    FormProvider.ShowList(4, this->Handle, 0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::ibdsTVAAfterEdit(TDataSet *DataSet)
{
    actApply->Enabled = true;
    actLoad->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::ibdsTVATYPEOFFSETGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->AsString == "Normal")
        Text = "Нормальне";
    else if (Sender->AsString == "Precision")
        Text = "Точне";
    else if (Sender->AsString == "Synchronised")
        Text = "Синхронізоване";
    else
        Text = "Невизначене";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::ibdsTVATYPEOFFSETSetText(TField *Sender,
      const AnsiString Text)
{
    if (ibdsTVA->State != dsEdit)
        return;

    if (Text == "Нормальне")
        Sender->AsString = "Normal";
    else if (Text == "Точне")
        Sender->AsString = "Precision";
    else if (Text == "Синхронізоване")
        Sender->AsString = "Synchronised";
    else
        Sender->AsString = "Unspecified";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::ibdsTVAFREQSTABILITYGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (ibdsTVA->State != dsEdit)
        return;

    if (Sender->AsString == "RELAXED")
        Text = "Послаблений";
    else if (Sender->AsString == "PRECISION")
        Text = "Точний";
    else
        Text = "Нормальний";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::ibdsTVAFREQSTABILITYSetText(TField *Sender,
      const AnsiString Text)
{
    if (Text == "RELAXED")
        Sender->AsString = "Послаблений";
    else if (Text == "Точний")
        Sender->AsString = "PRECISION";
    else
        Sender->AsString = "NORMAL";
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::cbxPolarizationChange(TObject *Sender)
{

    if (cbxPolarization->Text == "H")  {
        edtEPRVVideo->Visible = false;
        edtEPRVAudio1->Visible = false;
        edtEPRGVideo->Visible = true;
        edtEPRGAudio1->Visible = true;
        if (chbMonoStereo->State == cbChecked) {
            edtEPRVAudio2->Visible = false;
            edtEPRGAudio2->Visible = true;
        }

    } else if (cbxPolarization->Text == "V")  {
        edtEPRVVideo->Visible = true;
        edtEPRVAudio1->Visible = true;
        edtEPRGVideo->Visible = false;
        edtEPRGAudio1->Visible = false;
        if (chbMonoStereo->State == cbChecked) {
            edtEPRVAudio2->Visible = true;
            edtEPRGAudio2->Visible = false;
        }

   } else {
        edtEPRVVideo->Visible = true;
        edtEPRVAudio1->Visible = true;
        edtEPRGVideo->Visible = true;
        edtEPRGAudio1->Visible = true;
        if (chbMonoStereo->State == cbChecked) {
            edtEPRVAudio2->Visible = true;
            edtEPRGAudio2->Visible = true;
        }

    }

    TfrmTxBaseAir::cbxPolarizationChange(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::cbxVideoOffsetHerzChange(TObject *Sender)
{
/********** valick ***************
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbxVideoOffsetLine->ItemIndex = offsetIdx;
        cbxVideoOffsetHerz->ItemIndex = offsetIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }
********** valick ****************/

    offsetIdx = cbxVideoOffsetHerz->ItemIndex;

    cbxVideoOffsetLine->ItemIndex = cbxVideoOffsetHerz->ItemIndex;

    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    sql->SQL->Text = "select OFFSETLINES, OFFSET from OFFSETCARRYFREQTVA where id = " + AnsiString((int)cbxVideoOffsetHerz->Items->Objects[cbxVideoOffsetHerz->ItemIndex]);
    sql->ExecQuery();
    Tx->set_video_offset_line(sql->Fields[0]->AsInteger);
    Tx->set_video_offset_herz(sql->Fields[1]->AsInteger);
    sql->Close();
    delete sql;
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::cbxVideoOffsetLineChange(TObject *Sender)
{
/*********** valick ***************
    if (ibdsStantionsBaseSTATUS->AsInteger != tsDraft) {
        cbxVideoOffsetLine->ItemIndex = offsetIdx;
        cbxVideoOffsetHerz->ItemIndex = offsetIdx;
        throw *(new Exception(AnsiString("Редагування передавача в базі неможливе")));
    }
********** valick ****************/
    offsetIdx = cbxVideoOffsetLine->ItemIndex;

    cbxVideoOffsetHerz->ItemIndex = cbxVideoOffsetLine->ItemIndex;

    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;

    sql->SQL->Text = "select OFFSETLINES, OFFSET from OFFSETCARRYFREQTVA where id = " + AnsiString((int)cbxVideoOffsetLine->Items->Objects[cbxVideoOffsetLine->ItemIndex]);
    sql->ExecQuery();
    Tx->set_video_offset_line(sql->Fields[0]->AsInteger);
    Tx->set_video_offset_herz(sql->Fields[1]->AsInteger);
    sql->Close();
    delete sql;

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::actLoadExecute(TObject *Sender)
{
    Tx->invalidate();
    TxDataLoad();
    SetRadiationClass();
    ibdsStantionsBase->Active = false;
    ibdsTVA->Active = false;
    ibdsStantionsBase->Active = true;
    ibdsTVA->Active = true;

    this->actApply->Enabled = false;
    this->actLoad->Enabled = false;
    lblEditing->Visible = false;
}
//---------------------------------------------------------------------------


void __fastcall TfrmTxTVA::btnNullChannelClick(TObject *Sender)
{
    cbxChannel->ItemIndex = -1;
    cbxChannelChange(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::SetRadiationClass()
{
    AnsiString ClassRadiation = "";

    if ( cbxTypeSysName->Text != "" )
    {
        ClassRadiation += "750K";

        //первый символ
        ClassRadiation += "F";

        //второй символ
        ClassRadiation += "3";

        //третий символ
        ClassRadiation += "E";

        //четвертый символ
        ClassRadiation += "G";

        //пятый символ
        ClassRadiation += "M";
    }

    edtClassRadiationAudio1->Text = ClassRadiation;

    ClassRadiation = "";

    if ( cbxTypeSysName->Text != "" )
    {
        if ( ( cbxTypeSysName->Text == "B" ) || ( cbxTypeSysName->Text == "B1" ) )
            ClassRadiation += "6M25";
        else
            ClassRadiation += "7M25";

        //первый символ
        ClassRadiation += "C";

        //второй символ
        ClassRadiation += "3";

        //третий символ
        ClassRadiation += "F";

        /*
        //четвертый символ
        ClassRadiation += "N";

        //пятый символ
        ClassRadiation += "N";
        */
    }

    edtClassRadiationVideo->Text = ClassRadiation;
}

void __fastcall TfrmTxTVA::edtEPRmaxVideoValueChange(TObject *Sender)
{    
    try {
        Tx->set_epr_video_max(StrToFloat(edtEPRmaxVideo->Text));
        if(cbxPolarization->Text == "M")
        {
            double eprgv = StrToFloat(edtEPRmaxVideo->Text) - 3;//10 * Log10(2);
            Tx->set_epr_video_hor(eprgv);
            Tx->set_epr_video_vert(eprgv);
            edtEPRVVideo->Text = FloatToStr(eprgv);
            edtEPRGVideo->Text = FloatToStr(eprgv);
        }

    } catch (Exception &e) {
        double DValue;
        Tx->get_epr_video_max(&DValue);
        edtEPRmaxVideo->Text = AnsiString(DValue);
    }


    TBCPolarization pol;
    Tx->get_polarization(&pol);

    if (pol==plHOR) {
        edtEPRGVideo->Text = edtEPRmaxVideo->Text;
        double DValue;
        DValue = StrToFloat(edtEPRGVideo->Text);
        Tx->set_epr_video_hor(DValue);
        Tx->set_epr_video_vert(-999);
        edtEPRVVideo->Text = "-999";
    }

    if (pol==plVER) {
        edtEPRVVideo->Text = edtEPRmaxVideo->Text;
        double DValue;
        DValue = StrToFloat(edtEPRVVideo->Text);
        Tx->set_epr_video_vert(DValue);
        Tx->set_epr_video_hor(-999);
        edtEPRGVideo->Text = "-999";
    }

    RefreshAll();

}
//---------------------------------------------------------------------------




void __fastcall TfrmTxTVA::edtEPRGVideoValueChange(TObject *Sender)
{
    try {
        if(cbxPolarization->Text == "M")
        {
            double val, val1, max;
            val1 = StrToFloat(edtEPRVVideo->Text);
            val =  StrToFloat(edtEPRGVideo->Text);
            max = 10 * Log10(pow(10, val / 10.) + pow(10, val1 / 10.));
            Tx->set_epr_video_max(max);
            edtEPRmaxVideo->Text = FormatFloat("0.##", max);
        }
     Tx->set_epr_video_hor(StrToFloat(edtEPRGVideo->Text));
   } catch (Exception &e) {
    double DValue;
    Tx->get_epr_video_hor(&DValue);
    edtEPRGVideo->Text = AnsiString(DValue);
  }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::edtEPRVVideoValueChange(TObject *Sender)
{
    try {
        if(cbxPolarization->Text == "M")
        {
            double val, val1, max;
            val1 = StrToFloat(edtEPRVVideo->Text);
            val =  StrToFloat(edtEPRGVideo->Text);
            max = 10 * Log10(pow(10, val / 10.) + pow(10, val1 / 10.));
            Tx->set_epr_video_max(max);
            edtEPRmaxVideo->Text = FormatFloat("0.##", max);
        }
     Tx->set_epr_video_vert(StrToFloat(edtEPRVVideo->Text));
   } catch (Exception &e) {
    double DValue;
    Tx->get_epr_video_vert(&DValue);
    edtEPRGVideo->Text = AnsiString(DValue);
  }
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::edtPowerVideoValueChange(TObject *Sender)
{
    double DValue, DValue0;
    Tx->get_power_video(&DValue0);
    try {
        Tx->set_power_video(DValue = StrToFloat(edtPowerVideo->Text));
        double vsr = 1.;
        Tx->get_v_sound_ratio_primary(&vsr);
        if(cbxPolarization->Text == "M")
        {
            double eprgv = 10. * log10(StrToFloat(edtPowerVideo->Text));
            Tx->set_epr_video_max(eprgv);
            edtEPRmaxVideo->Text = FloatToStr(eprgv);
            eprgv -= 3;
            Tx->set_epr_video_hor(eprgv);
            Tx->set_epr_video_vert(eprgv);
            edtEPRVVideo->Text = FloatToStr(eprgv);
            edtEPRGVideo->Text = FloatToStr(eprgv);

        }
        if (vsr != 0.)
        {
            Tx->set_power_sound_primary(DValue / vsr);
            double ps = 0.;
            Tx->get_power_sound_primary(&ps);
            edtPowerAudio1->Text = FormatFloat("0.##", ps);
        }
    } catch (Exception &e) {
        Tx->get_power_video(&DValue);
        edtPowerVideo->Text = AnsiString(DValue);
    }

    RefreshAll();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::edtVSoundRatio1ValueChange(TObject *Sender)
{
    if (((TNumericEdit*)Sender)->Text.IsEmpty())
        ((TNumericEdit*)Sender)->Text = "1";
    double DValue, DValue0;
    try {
        Tx->set_v_sound_ratio_primary(DValue0 = StrToFloat(edtVSoundRatio1->Text));
        if (DValue0 != 0.)
        {
            double pv = 0.;
            Tx->get_power_video(&pv);
            Tx->set_power_sound_primary(pv / DValue0);
            double ps = 0.;
            Tx->get_power_sound_primary(&ps);
            edtPowerAudio1->Text = FormatFloat("0.##", ps);
        }
    } catch (Exception &e) {
        Tx->get_v_sound_ratio_primary(&DValue);
        edtVSoundRatio1->Text = AnsiString(DValue);
    }

    RefreshAll();
}
//---------------------------------------------------------------------------

void _fastcall TfrmTxTVA::SetPowerInput(TPowerInput pi)
{
/*
  В методе TfrmTxBaseAir::SetPowerInput() блокируются
  едиты edtPowerAudio1, edtEPRxxxAudio1 и др.
*/
        TfrmTxBaseAir::SetPowerInput(pi);

        TColor erpcolor;
        TColor powercolor;
        bool flag_power_input = (pi == piPOWER);

        if (flag_power_input) {
                erpcolor = clDisabledEdit;
                powercolor = clEnabledEdit;
        } else{
                powercolor = clDisabledEdit;
                erpcolor = clEnabledEdit;
        }


        edtEPRmaxVideo->Enabled = !flag_power_input;
        edtEPRmaxAudio2->Enabled = !flag_power_input;

        edtEPRVVideo->Enabled = !flag_power_input;
        edtEPRVAudio2->Enabled = !flag_power_input;

        edtEPRGVideo->Enabled = !flag_power_input;
        edtEPRGAudio2->Enabled = !flag_power_input;

        edtPowerVideo->Enabled = flag_power_input;
        edtPowerAudio2->Enabled = flag_power_input;


        edtEPRmaxVideo->Color = erpcolor;
        edtEPRmaxAudio2->Color = erpcolor;

        edtEPRVVideo->Color = erpcolor;
        edtEPRVAudio2->Color = erpcolor;

        edtEPRGVideo->Color = erpcolor;
        edtEPRGAudio2->Color = erpcolor;

        edtPowerVideo->Color = powercolor;
        edtPowerAudio2->Color = powercolor;

/*
  А эти эдиты отключаем, потому что звук вручную
  непосредственно не вводим, а вводим только отношение
  звук / видео

*/
        edtPowerAudio1->Color = clDisabledEdit;
        edtPowerAudio2->Color = clDisabledEdit;
        edtEPRVAudio1->Color = clDisabledEdit;
        edtEPRGAudio1->Color = clDisabledEdit;
        edtEPRVAudio2->Color = clDisabledEdit;
        edtEPRGAudio2->Color = clDisabledEdit;
        edtEPRmaxAudio1->Color = clDisabledEdit;
        edtEPRmaxAudio2->Color = clDisabledEdit;

        edtPowerAudio1->Enabled = false;
        edtPowerAudio2->Enabled = false;
        edtEPRVAudio1->Enabled = false;
        edtEPRGAudio1->Enabled = false;
        edtEPRVAudio2->Enabled = false;
        edtEPRGAudio2->Enabled = false;
        edtEPRmaxAudio1->Enabled = false;
        edtEPRmaxAudio2->Enabled = false;
}

/*
  Данные из объекта передатчика отображаем на форму
*/

void _fastcall TfrmTxTVA::TxToForm()
{
    TfrmTxBaseAir::TxToForm();
    double erp;
    double power;

    Tx->get_epr_video_hor(&erp);
    edtEPRGVideo->OldValue = FormatFloat("0.##", erp);

    Tx->get_epr_video_vert(&erp);
    edtEPRVVideo->OldValue = FormatFloat("0.##", erp);

    Tx->get_power_video(&power);
    edtPowerVideo->OldValue = FormatFloat("0.####", power);

    Tx->get_epr_video_max(&power);
    edtEPRmaxVideo->OldValue = FormatFloat("0.##", power);
}


/*
Обрабатывваем всю форму и записываем значения в объект передатчика
*/

void _fastcall TfrmTxTVA::FormToTx()
{
    TfrmTxBaseAir::FormToTx();
    TBCPolarization pol;
    Tx->get_polarization(&pol);

    ILisBcAntPattPtr antPatt;
    Tx->QueryInterface<ILisBcAntPatt>(&antPatt);
    if (!antPatt.IsBound())
        throw *(new Exception("Интерфейс ILisBcAntPatt не поддерживается сервером передатчиков"));

    if (PowerInput == piPOWER){
        if (edtPowerVideo->Text.IsEmpty()) edtPowerVideo->Text = 0;
        if (edtVSoundRatio1->Text.IsEmpty()) edtVSoundRatio1->Text = 0;

        double p_kw = StrToFloat(edtPowerVideo->Text);
        double p_dbkw;

        if (p_kw <= 0){
            edtEPRmaxVideo->Text = "-999";
            edtEPRmaxAudio1->Text = "-999";
            p_dbkw = -999;
        }
        else
        {
            double gain = StrToFloat(edtGain->Text);
            double floss = StrToFloat(edtFiderLoss->Text);
            double flength = StrToFloat(edtFiderLength->Text);
            p_dbkw = 10 * log10(p_kw);

            double summAtten = 0;
            if (edtSummAtten->Field)
                summAtten = edtSummAtten->Field->AsFloat;

            double erp = p_dbkw + gain - floss * flength - summAtten;
            double avratio = StrToFloat(edtVSoundRatio1->Text);
            double erpaudio1 = erp - avratio;

            double maxerp=-999;
            for (int i=0; i<36; i++){
                double gain_h;
                double gain_v;
                antPatt->get_gain_h(i, &gain_h);
                antPatt->get_gain_v(i, &gain_v);
                double erp_h = p_dbkw + gain_h - floss * flength - summAtten;
                double erp_v = p_dbkw + gain_v - floss * flength - summAtten;

                if ((pol == plHOR || pol == plMIX) && erp_h > maxerp)
                    maxerp = erp_h;
                if ((pol == plVER || pol == plMIX) && erp_v > maxerp)
                    maxerp = erp_v;

                Tx->set_effectpowerhor(i, -999);
                Tx->set_effectpowervert(i, -999);
                if (pol == plHOR || pol == plMIX)
                    Tx->set_effectpowerhor(i, erp_h);
                if (pol == plVER || pol == plMIX)
                    Tx->set_effectpowervert(i, erp_v);
            }

            Tx->set_epr_video_max(maxerp);
            Tx->set_epr_sound_max_primary(maxerp-avratio);

            Tx->set_epr_video_hor(-999);
            Tx->set_epr_video_vert(-999);
            Tx->set_epr_sound_hor_primary(-999);
            Tx->set_epr_sound_vert_primary(-999);

            if (pol == plHOR )
            {
                Tx->set_epr_video_hor(maxerp);
                Tx->set_epr_sound_hor_primary(maxerp-avratio);
            }
            if (pol == plVER)
            {
                Tx->set_epr_video_vert(maxerp);
                Tx->set_epr_sound_vert_primary(maxerp-avratio);
            }
            if (pol == plMIX)
            {
                Tx->set_epr_video_max(erp);
                double erp = maxerp - 3;
                Tx->set_epr_video_hor(erp);
                Tx->set_epr_video_vert(erp);
            }
        }
    }
    /*
    Если вводится непосредственно ЭИМ, то надо убрать все
    лишние параметры - мощность кВт, фидер, усиление и т.п.
    */
    if (PowerInput == piERP){
        if (edtEPRmaxAudio1->Text.IsEmpty()) edtEPRmaxAudio1->Text = 0;
        double erp = StrToFloat(edtEPRmaxVideo->Text);
        double avratio = StrToFloat(edtVSoundRatio1->Text);
        double erpaudio1 = erp - avratio;
        Tx->set_epr_video_max(erp);
        Tx->set_epr_sound_max_primary(erpaudio1);
        Tx->set_power_sound_primary(0);
        Tx->set_power_video(0);
        Tx->set_fiderloss(0);
        Tx->set_fiderlenght(0);
        double gainmax;
        Tx->get_antennagain(&gainmax);
        for (int i=0; i<36; i++)
        {
            double gain_h;
            double gain_v;
            antPatt->get_gain_h(i, &gain_h);
            antPatt->get_gain_v(i, &gain_v);
            if (pol == plHOR) {
                Tx->set_effectpowerhor(i, erp + gain_h - gainmax);
                Tx->set_effectpowervert(i, -999);
                antPatt->set_gain_h(i, gain_h - gainmax);
                antPatt->set_gain_v(i, 0);
            }
            else if (pol == plVER) {
                Tx->set_effectpowervert(i, erp + gain_v - gainmax);
                Tx->set_effectpowerhor(i, -999);
                antPatt->set_gain_h(i, 0);
                antPatt->set_gain_v(i, gain_v - gainmax);
            }
            else {
                Tx->set_effectpowervert(i, erp + gain_v - gainmax);
                Tx->set_effectpowerhor(i, erp + gain_h - gainmax);
                antPatt->set_gain_h(i, gain_h - gainmax);
                antPatt->set_gain_v(i, gain_v - gainmax);
            }
        }
        Tx->set_antennagain(0);
        if (pol == plHOR) {
            Tx->set_epr_video_hor(erp);
            Tx->set_epr_video_vert(-999);
            Tx->set_epr_sound_hor_primary(erpaudio1);
            Tx->set_epr_sound_vert_primary(-999);
        }
        else if (pol == plVER) {
            Tx->set_epr_video_vert(erp);
            Tx->set_epr_video_hor(-999);
            Tx->set_epr_sound_vert_primary(erpaudio1);
            Tx->set_epr_sound_hor_primary(-999);
        }
        else if (pol == plMIX)
            {
                Tx->set_epr_sound_vert_primary(erpaudio1);
                Tx->set_epr_sound_hor_primary(erpaudio1);
                double val, val1;
                val1 = StrToFloat(edtEPRVVideo->Text);
                val =  StrToFloat(edtEPRGVideo->Text);
                Tx->set_epr_video_hor(val);
                Tx->set_epr_video_vert(val1);
            }
        else {
            Tx->set_epr_video_vert(erp);
            Tx->set_epr_video_hor(erp);
            Tx->set_epr_sound_vert_primary(erpaudio1);
            Tx->set_epr_sound_hor_primary(erpaudio1);
        }
    }

    actLoad->Enabled = true;
    if (ibdsStantionsBaseSTATUS->AsInteger == tsDraft) actApply->Enabled = true;
}



void __fastcall TfrmTxTVA::btnVideoEmissionClick(TObject *Sender)
{
    ibdsStantionsBase->Edit();
    ibdsStantionsBaseVIDEO_EMISSION->AsString = CalcVideoEmission();
}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::btnSoundEmissionPrimaryClick(TObject *Sender)
{
  ibdsStantionsBase->Edit();
  ibdsStantionsBaseSOUND_EMISSION_PRIMARY->AsString = CalcSoundEmission();

}
//---------------------------------------------------------------------------

void __fastcall TfrmTxTVA::btnSoundEmissionSecondClick(TObject *Sender)
{
  ibdsStantionsBase->Edit();
  ibdsStantionsBaseSOUND_EMISSION_SECOND->AsString  = CalcSoundEmission();

}
//---------------------------------------------------------------------------







