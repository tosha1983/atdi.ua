//---------------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include <IBQuery.hpp>
#include <math>
#include <memory>

#include "FormProvider.h"
#include "LISBCTxServer_TLB.h"
#include "RSAGeography_TLB.h"
#include "TxBroker.h"

#include "uExplorer.h"
#include "uMainForm.h"
#include "uNewTxWizard.h"
#include "uParams.h"
#include "Transliterator.hpp"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CSPIN"
#pragma resource "*.dfm"
//  блядская ATL
#ifdef StrToInt
#undef StrToInt
#endif
TfrmNewTxWizard *frmNewTxWizard;
//---------------------------------------------------------------------------
__fastcall TfrmNewTxWizard::TfrmNewTxWizard(TComponent* Owner)
    : TForm(Owner)
{
    antennaDiagramPanel = new TAntennaDiagramPanel(panAntGainGraph);
    antennaDiagramPanel->Visible = false;
    antennaDiagramPanel->Parent = panAntGainGraph;
    antennaDiagramPanel->BevelOuter = bvNone;
    antennaDiagramPanel->Left = 10;
    antennaDiagramPanel->Width = panAntGainGraph->Width - antennaDiagramPanel->Left * 2;
    antennaDiagramPanel->Top = 10;
    antennaDiagramPanel->Height = antennaDiagramPanel->Width;
    antennaDiagramPanel->Anchors << akRight << akBottom;
    panAntGainGraph->Visible = false;
}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::FormCreate(TObject *Sender)
{
    Width = 600;
    Height = 400;
    cancelled = true;
    new_id = 0;
    lat = 50.5;
    lon = 30.5;
    edtLon->Text = dmMain->coordToStr(lon, 'X');
    edtLat->Text = dmMain->coordToStr(lat, 'Y');
    height_sea = -9999;

    panType->Left = 0;
    panType->Top = 0;
    panCoord->Left = 0;
    panCoord->Top = 0;
    panStand->Left = 0;
    panStand->Top = 0;
    panSystem->Left = 0;
    panSystem->Top = 0;
    panTech->Left = 0;
    panTech->Top = 0;

    Update();

    panType->Anchors << akRight << akBottom;
    panCoord->Anchors << akRight << akBottom;
    panStand->Anchors << akRight << akBottom;
    panSystem->Anchors << akRight << akBottom;
    panTech->Anchors << akRight << akBottom;

    //edtERPmax->Text.FormatFloat("0.0", 0);
    edtPower->Text.FormatFloat("0.0", 0);

    sqlSystemcast->ExecQuery();
    while (!sqlSystemcast->Eof) {
        rgrSystemCast->Items->AddObject(sqlSystemcast->Fields[2]->AsString,
                                        (TObject*)(unsigned)sqlSystemcast->Fields[1]->AsInteger);
        sqlSystemcast->Next();
    }
    sqlSystemcast->Close();
    if (rgrSystemCast->Items->Count > 0)
        rgrSystemCast->ItemIndex = 0;
    cbxDirectChange(this);
    
}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::CancelClick(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::FormCloseQuery(TObject *Sender,
      bool &CanClose)
{
/*
    if (cancelled && Application->MessageBox("Вийти з майстера вводу нового передавача?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDNO)
        CanClose = false;
*/
}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::btnFinishClick(TObject *Sender)
{
    if (panStand->Visible) {
        if (rbtnNewStand->Checked) {
            if (cbxNewStandArea->ItemIndex == -1)
                throw *(new Exception("Необхідно вибрати регіон"));

            if (cbxNewStandStreet->ItemIndex == -1 && !cbxNewStandStreet->Text.IsEmpty()) {
                int idx;
                if ((idx = cbxNewStandStreet->Items->IndexOf(cbxNewStandStreet->Text)) != -1) {
                    cbxNewStandStreet->ItemIndex = idx;
                } else {
                    if (cbxNewStandCity->ItemIndex == -1)
                        throw *(new Exception("Якщо вказано вулицю, то необхідно вибрати населений пункт"));
                    if (Application->MessageBox(
                    (AnsiString("Вулиці \'")+cbxNewStandStreet->Text+"\' населеного пункту \'"+cbxNewStandCity->Text+"\' в базі немає. Додати?").c_str(),
                    Application->Title.c_str(),
                    MB_ICONQUESTION | MB_YESNO) == IDYES){
                        int newId = dmMain->getNewId();
                        sqlNewStreet->Close();
                        sqlNewStreet->ParamByName("ID")->AsInteger = newId;
                        sqlNewStreet->ParamByName("NAME")->AsString = cbxNewStandStreet->Text;
                        sqlNewStreet->ParamByName("CITY_ID")->AsInteger = (int)cbxNewStandCity->Items->Objects[cbxNewStandCity->ItemIndex];
                        sqlNewStreet->ExecQuery();
                        sqlNewStreet->Transaction->CommitRetaining();
                        sqlNewStreet->Close();
                        cbxNewStandStreet->Items->AddObject(cbxNewStandStreet->Text, (TObject*)newId);
                        idx = cbxNewStandStreet->Items->IndexOfObject((TObject*)newId);
                        cbxNewStandStreet->ItemIndex = idx;
                    } else {
                        return;
                    }
                }
            }
            if (cbxNewStandStreet->ItemIndex != -1) {
//                throw *(new Exception("Необхідно вибрати вулицю"));

            //if (edtNewStandAddress->Text.IsEmpty())
            //    throw *(new Exception("Необхідно задати адресу"));

                if (height_sea == -9999)
                    btnGetHeightClick(this);
                else
                    height_sea = edtSiteHeight->Text.ToInt();
                city = (int)cbxNewStandCity->Items->Objects[cbxNewStandCity->ItemIndex];
                street = (int)cbxNewStandStreet->Items->Objects[cbxNewStandStreet->ItemIndex];
            }

        } else {
            //  існуюча опора
            if (dstStand->RecordCount == 0)
                throw *(new Exception("Не вибрано жодної опори"));

        }
    }

    //  параметры последней панели
//    power_video = edtPower->Text.ToDouble();
    //erp_video_max = edtERPmax->Text.ToDouble();
//    antenna_gain = edtAntennaGain->Text.ToDouble();
//    antenna_height = edtAntHeight->Text.ToInt();
    //h_eff_max = edtEffHeight->Text.ToInt();

    //  ввести новую опору, если надо
    int stand_id;
    if (rbtnExistingStand->Checked) {
        stand_id = dstStandID->AsInteger;
    } else {
        stand_id = dmMain->getNewId();
        sqlNewStand->Close();
        sqlNewStand->ParamByName("ID")->AsInteger = stand_id;
        sqlNewStand->ParamByName("NAMESITE")->AsString = edtNewStandName->Text;
        sqlNewStand->ParamByName("NAMESITE_ENG")->AsString = RusToEng(edtNewStandName->Text);
        sqlNewStand->ParamByName("LATITUDE")->AsDouble = standLat;
        sqlNewStand->ParamByName("LONGITUDE")->AsDouble = standLon;
        sqlNewStand->ParamByName("HEIGHT_SEA")->AsInteger = height_sea;
        sqlNewStand->ParamByName("DISTRICT_ID")->AsInteger = -1;
        sqlNewStand->ParamByName("CITY_ID")->AsInteger =    city;
        sqlNewStand->ParamByName("STREET_ID")->AsInteger =  street;
        sqlNewStand->ParamByName("ADDRESS")->AsString =     address;
        sqlNewStand->ParamByName("VHF_IS")->AsInteger =     0;

        sqlNewStand->ParamByName("AREA_ID")->AsInteger = area;

    try {
        sqlNewStand->ExecQuery();
        }
    catch (Exception& E) {
        throw *(new Exception(AnsiString("Помилка запису опори в базу: ")+E.Message));
        }
    }

    int tx_id = dmMain->getNewId();
    sqlNewTx->Close();
    sqlNewTx->ParamByName("ID")->AsInteger = tx_id;

//    if (edtAdminId->Text.Length() > 0)
//        sqlNewTx->ParamByName("ADMINISTRATIONID")->AsString = edtAdminId->Text;
//    else if (chbGenerateID->Checked) {

        //  генерируем новый ADMINISTRATIONID
//        AnsiString string_id1, string_id2;
//        int id;

//        sqlNewAdminId->Close();
//       sqlNewAdminId->Params->Vars[0]-> AsInteger = stand_id;
//        sqlNewAdminId->ExecQuery();
//        if (!sqlNewAdminId->Eof)
//            string_id1 = sqlNewAdminId->Fields[0]->AsString;
//        else
//            string_id1 = "0";
//        if (string_id1.Length() == 0)
//            string_id1 = "0";
        //  ищем числовую составляющую, буквы запоминаем
//        bool completed = false;
//        while (!completed) {
//            try {
//                id = string_id1.ToInt();
//                completed = true;
//            } catch (...) {
                //  убираем первый символ и пытаемся снова
//                if (string_id1.Length() == 0) {
//                    id = 0;
//                    completed = true;
//                } else {
//                    string_id2 = string_id2 + string_id1[1];
//                    string_id1.Delete(1,1);
//                }
//            }
//        }
        //  имеем префикс string_id2 и число id
//        string_id1 = IntToStr(++id);
//        while (sqlNewAdminId->Fields[0]->Size > string_id2.Length() + string_id1.Length())
//            string_id1.Insert("0",1);

        //  итого
//        sqlNewTx->ParamByName("ADMINISTRATIONID")->AsString = string_id2 + string_id1;
//    }

    sqlNewTx->ParamByName("ORIGINALID")->AsInteger = -1;
    sqlNewTx->ParamByName("STATUS")->AsInteger = 1;
    sqlNewTx->ParamByName("STAND_ID")->AsInteger = stand_id;
    if (rbtnExistingStand->Checked && chbCheckInCoord->Checked) {
        sqlNewTx->ParamByName("LATITUDE")->AsDouble = dstStandLATITUDE->AsFloat;
        sqlNewTx->ParamByName("LONGITUDE")->AsDouble = dstStandLONGITUDE->AsFloat;
    } else {
        sqlNewTx->ParamByName("LATITUDE")->AsDouble = lat;
        sqlNewTx->ParamByName("LONGITUDE")->AsDouble = lon;
    }

    sqlNewTx->ParamByName("SYSTEMCAST_ID")->AsInteger = systemcast_id;

    //  значения по умолчанию, зависящие от страны
    //  сначала сбросим
    sqlNewTx->ParamByName("TYPESYSTEM")->AsInteger = 0;
    sqlNewTx->ParamByName("SYSTEMCOLOUR")->AsString = "SECAM";

    sqlNewTx->ParamByName("POLARIZATION")->AsString = "V";

    if (systemcast == ttTV || systemcast == ttFM || systemcast == ttDVB) {

        std::auto_ptr<TIBSQL> defaulsSql(new TIBSQL(this));
        defaulsSql->Database = sqlNewTx->Database;
        defaulsSql->Transaction = sqlNewTx->Transaction;
        defaulsSql->SQL->Text = "select DEF_TVA_SYS, DEF_COLOR, DEF_FM_SYS, DEF_DVB_SYS from COUNTRY where ID = "
                         "(select COUNTRY_ID from AREA where ID = :ID)";
        defaulsSql->Params->Vars[0]->AsInteger = area;
        defaulsSql->ExecQuery();


        if (!defaulsSql->Eof) {
            sqlNewTx->ParamByName("SYSTEMCOLOUR")->AsString = defaulsSql->FieldByName("DEF_COLOR")->AsString;
            switch(systemcast) {
                case ttTV:
                    sqlNewTx->ParamByName("TYPESYSTEM")->AsInteger = defaulsSql->FieldByName("DEF_TVA_SYS")->AsInteger;
                    sqlNewTx->ParamByName("POLARIZATION")->AsString = "H";
                    break;
                case ttAM:
                    sqlNewTx->ParamByName("TYPESYSTEM")->AsInteger = 0;
                    break;
                case ttFM:
                    sqlNewTx->ParamByName("TYPESYSTEM")->AsInteger = defaulsSql->FieldByName("DEF_FM_SYS")->AsInteger;
                    break;
                case ttDVB:
                    sqlNewTx->ParamByName("TYPESYSTEM")->AsInteger = defaulsSql->FieldByName("DEF_DVB_SYS")->AsInteger;
                    break;
            }
        }
    }

//    sqlNewTx->ParamByName("TYPESYSTEM")->AsInteger = system;
//    sqlNewTx->ParamByName("CHANNEL_ID")->AsInteger = channel;
//    sqlNewTx->ParamByName("ALLOTMENTBLOCKDAB_ID")->AsInteger = allotment_block;
//    sqlNewTx->ParamByName("VIDEO_CARRIER")->AsDouble = freq_video;
//    sqlNewTx->ParamByName("SOUND_CARRIER_PRIMARY")->AsDouble = freq_sound;
//    sqlNewTx->ParamByName("SOUND_CARRIER_SECOND")->AsDouble = freq_sound_sec;
//    sqlNewTx->ParamByName("BLOCKCENTREFREQ")->AsDouble = freq_sound;
    sqlNewTx->ParamByName("POWER_VIDEO")->AsDouble =    0;// power_video;
    sqlNewTx->ParamByName("EPR_VIDEO_MAX")->AsDouble =   -999;//erp_video_max;
    sqlNewTx->ParamByName("EPR_VIDEO_HOR")->AsDouble =   -999;//erp_video_max;
    sqlNewTx->ParamByName("EPR_VIDEO_VERT")->AsDouble =   -999;//erp_video_max;
    sqlNewTx->ParamByName("POWER_SOUND_PRIMARY")->AsDouble = 0;//    power_video;
    sqlNewTx->ParamByName("POWER_SOUND_SECOND")->AsDouble = 0;//    power_video;
    sqlNewTx->ParamByName("EPR_SOUND_MAX_PRIMARY")->AsDouble =  -999;// erp_video_max;
    sqlNewTx->ParamByName("EPR_SOUND_MAX_SECOND")->AsDouble =  -999;// erp_video_max;
    sqlNewTx->ParamByName("EPR_SOUND_HOR_PRIMARY")->AsDouble = -999;//    power_video;
    sqlNewTx->ParamByName("EPR_SOUND_HOR_SECOND")->AsDouble = -999;//    power_video;
    sqlNewTx->ParamByName("EPR_SOUND_VERT_PRIMARY")->AsDouble =  -999;// erp_video_max;
    sqlNewTx->ParamByName("EPR_SOUND_VER_SECOND")->AsDouble =  -999;// erp_video_max;
    sqlNewTx->ParamByName("ANTENNAGAIN")->AsDouble = 0;//    antenna_gain;
    sqlNewTx->ParamByName("HEIGHTANTENNA")->AsInteger =  0;//antenna_height;
    sqlNewTx->ParamByName("HEIGHT_EFF_MAX")->AsInteger = 0;//h_eff_max;
    sqlNewTx->ParamByName("DIRECTION")->AsString = "ND";
    sqlNewTx->ParamByName("V_SOUND_RATIO_PRIMARY")->AsString = 10;
    sqlNewTx->ParamByName("V_SOUND_RATIO_SECOND")->AsString = 10;

    std::auto_ptr<TIBQuery> sql(new TIBQuery(dmMain));

        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "update TRANSMITTERS"
        "  set EFFECTPOWERHOR = :EFFECTPOWERHOR"
        ", EFFECTPOWERVER = :EFFECTPOWERVER where id = " + AnsiString(tx_id);
        TMemoryStream *memstream = new TMemoryStream;
        memstream->Clear();
        for(int i = 0; i < 36 ; i++) {
            double val;
            val = -999;
            memstream->Write(&val, sizeof(double));
        }
        sql->ParamByName("EFFECTPOWERHOR")->LoadFromStream(memstream, ftBlob);
        sql->ParamByName("EFFECTPOWERVER")->LoadFromStream(memstream, ftBlob);
        memstream->Clear();

        try {
                sqlNewTx->ExecQuery();
                sql->ExecSQL();
                dmMain->trMain->CommitRetaining();
                sql->Close();
            } catch (Exception &e) {
                throw *(new Exception(AnsiString("Помилка запису передавача в базу: ")+e.Message));
            }

    int sc = (int)rgrSystemCast->Items->Objects[rgrSystemCast->ItemIndex];
    FormProvider.ShowTx(txBroker.GetTx(tx_id, dmMain->GetObjClsid(sc)));
    frmMain->FormListTxRefresh();
    cancelled = false;

    frmExplorer->AddTx(tx_id);//добавить передатчик в консоль

    ModalResult = mrOk;
    //Close();
}

void __fastcall TfrmNewTxWizard::btnNextClick(TObject *Sender)
{
    if (panType->Visible) {

        systemcast = (int)rgrSystemCast->Items->Objects[rgrSystemCast->ItemIndex];
        sqlGetCS_ID->Close();
        sqlGetCS_ID->ParamByName("ENUMVAL")->AsInteger = systemcast;
        sqlGetCS_ID->ExecQuery();
        systemcast_id = sqlGetCS_ID->Eof ? 0 : sqlGetCS_ID->Fields[0]->AsInteger;
        sqlGetCS_ID->Close();

        panType->Visible = false;
        panCoord->Visible = true;
        edtLat->SetFocus();
        btnPrev->Enabled = true;

        switch(systemcast) {
            case ttTV: Caption = "Введення нового передавача TVA";
                break;
            case ttAM: Caption = "Введення нового передавача СХ/ДХ ";
                break;
            case ttFM: Caption = "Введення нового передавача SBFM ";
                break;
            case ttDVB: Caption = "Введення нового передавача DVB-T";
                break;
            case ttDAB: Caption = "Введення нового передавача T-DAB";
                break;
            case ttCTV: Caption = "Введення нової системи кабельного ТБ";
                break;
        }

   } else if (panCoord->Visible) {

        panCoord->Visible = false;
        panStand->Visible = true;

        fillCombo(cbxNewStandArea, sqlArea, 0,
                cbxNewStandArea->ItemIndex > -1 ? (int)cbxNewStandArea->Items->Objects[cbxNewStandArea->ItemIndex] : -1);
        edtNewLat->Text = dmMain->coordToStr(standLat = lat, 'Y');
        edtNewLon->Text = dmMain->coordToStr(standLon = lon, 'X');

        seRadius->Value = BCCalcParams.standRadius;

        btnRadiusClick(this);

        if (dstStand->Eof) {
            // опор нет, новая опора

            area = BCCalcParams.defArea;
            city = BCCalcParams.defCity;
            if (area == -1) area == -2;
            if (city == -1) city == -2;
            street = -1;

            numcbxRegion = cbxNewStandArea->Items->IndexOfObject((TObject*)area);
            cbxNewStandAreaChange(this);
            numcbxCity = cbxNewStandCity->Items->IndexOfObject((TObject*)city);
            cbxNewStandCityChange(this);

            numcbxStreet = -1;

            if (area == -2) area == -1;
            if (city == -2) city == -1;

            rbtnNewStand->Checked = true;
            btnGetHeightClick(this);

            rbtnNewStandClick(this);
            edtNewLat->SetFocus();
        } else {
            // опоры есть, существующая опора
            rbtnExistingStand->Checked = true;
            grdExistingStand->SetFocus();
            if (height_sea == -9999)
                btnGetHeightClick(this);
        }

        btnNext->Enabled = false;
        btnFinish->Enabled = true;

   }
/*
        panStand->Visible = false;
        panSystem->Visible = true;
        //  настройка внешнего вида панели систем вещания
        switch(systemcast) {
            case ttTV:
                Label9->Caption = "Телевізійна система";
                Label9->Visible = true;
                cbxSystem->Visible = true;
                cbxSystem->SetFocus();
                Label10->Visible = true;
                cbxChannel->Visible = true;
                Label11->Caption = "Несуча відео, МГц";
                Label11->Visible = true;
                edtVideoCarrier->Visible = true;
                Label12->Visible = false;
                cbxAllotment->Visible = false;
                Label18->Visible = false;
                edtBlock->Visible = false;
                Label13->Caption = "Несуча звуку, МГц";
                edtSoundCarrier->ReadOnly = true;
                Label13->Visible = true;
                edtSoundCarrier->Visible = true;
                Label26->Visible = false;
                cbxChannelGrid->Visible = false;

                fillCombo(cbxSystem, sqlSystem, 0,
                          cbxSystem->ItemIndex > -1 ? (int)cbxSystem->Items->Objects[cbxSystem->ItemIndex] : 0);
                break;
            case ttFM:
                Label9->Caption = "Система мовлення";
                Label9->Visible = true;
                cbxSystem->Visible = true;
                Label10->Visible = false;
                cbxChannel->Visible = false;
                Label11->Visible = false;
                edtVideoCarrier->Visible = false;
                Label12->Visible = false;
                cbxAllotment->Visible = false;
                Label18->Visible = false;
                edtBlock->Visible = false;
                Label13->Caption = "Несуча звуку, МГц";
                edtSoundCarrier->ReadOnly = false;
                edtSoundCarrier->SetFocus();
                Label13->Visible = true;
                edtSoundCarrier->Visible = true;
                Label26->Visible = false;
                cbxChannelGrid->Visible = false;

                fillCombo(cbxSystem, sqlRadioSystem, 0,
                          cbxSystem->ItemIndex > -1 ? (int)cbxSystem->Items->Objects[cbxSystem->ItemIndex] : 0);
                break;
            case ttDVB:
                Label26->Visible = true;
                cbxChannelGrid->Visible = true;
                Label9->Visible = false;
                cbxSystem->Visible = false;
                Label10->Visible = true;
                cbxChannel->Visible = true;
                Label11->Caption = "Центральна частота, МГц";
                Label11->Visible = true;
                edtVideoCarrier->Visible = true;
                Label12->Visible = false;
                cbxAllotment->Visible = false;
                Label18->Visible = false;
                edtBlock->Visible = false;
                Label13->Visible = false;
                edtSoundCarrier->Visible = false;
                fillCombo(cbxChannelGrid, sqlChannelGrid, 0,
                          cbxChannelGrid->ItemIndex > -1 ? (int)cbxChannelGrid->Items->Objects[cbxChannelGrid->ItemIndex] : 0);
                break;
            case ttDAB:
                Label9->Visible = false;
                cbxSystem->Visible = false;
                Label10->Visible = false;
                cbxChannel->Visible = false;
                Label11->Visible = false;
                edtVideoCarrier->Visible = false;
                Label12->Visible = true;
                cbxAllotment->Visible = true;
                cbxAllotment->SetFocus();
                Label18->Visible = true;
                edtBlock->Visible = true;
                Label13->Caption = "Центральна частота";
                Label13->Visible = true;
                edtSoundCarrier->Visible = true;
                edtSoundCarrier->ReadOnly = true;
                Label26->Visible = false;
                cbxChannelGrid->Visible = false;
                fillCombo(cbxAllotment, sqlAllotment, 0,
                          cbxAllotment->ItemIndex > -1 ? (int)cbxAllotment->Items->Objects[cbxAllotment->ItemIndex] : 0);
                break;
            case ttCTV:
                btnNextClick(this);
                break;
        }

    } else if (panSystem->Visible) {
        if (systemcast == ttFM) {
            system = (int)cbxSystem->Items->Objects[cbxSystem->ItemIndex];
        }
        if (systemcast == ttTV || systemcast == ttDVB) {
            if (cbxSystem->ItemIndex != -1) {
                system = (int)cbxSystem->Items->Objects[cbxSystem->ItemIndex];
            } else {
                system = -1;
            }
            if (cbxChannel->ItemIndex != -1) {
                channel = (int)cbxChannel->Items->Objects[cbxChannel->ItemIndex];
                freq_video = edtVideoCarrier->Text.ToDouble();
            } else {
                channel = -1;
                freq_video = 0;
            }
        }
        if (systemcast == ttDAB) {
            if (cbxAllotment->ItemIndex != -1)
                allotment_block = (int)cbxAllotment->Items->Objects[cbxAllotment->ItemIndex];
            else
                allotment_block = -1;
        }
        if (systemcast == ttTV || systemcast == ttDAB || systemcast == ttFM) {
            if (edtSoundCarrier->Text.Length() > 0)
                freq_sound = edtSoundCarrier->Text.ToDouble();
            else
                freq_sound = 0;
        }

//        panSystem->Visible = false;
        for (int i = 0 ; i < 36; i++)
                sgrAntennaGain->Cells[0][i] = AnsiString(i*10);
//        panTech->Visible = true;
//        btnNext->Enabled = false;
//        btnFinish->Enabled = true;
//        edtPower->SetFocus();
     */

}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::btnPrevClick(TObject *Sender)
{
    if (panCoord->Visible) {
        panType->Visible = true;
        panCoord->Visible = false;
        btnPrev->Enabled = false;
    } else if (panStand->Visible) {
        dstStand->Close();
        panStand->Visible = false;
        panCoord->Visible = true;
    } else if (panSystem->Visible) {
        panSystem->Visible = false;
        panStand->Visible = true;
    } else if (panTech->Visible) {
        panTech->Visible = false;
        panSystem->Visible = true;
        if (systemcast == ttCTV) 
            btnPrevClick(this);
    }
    btnNext->Enabled = true;
    btnFinish->Enabled = false;
}
//---------------------------------------------------------------------------
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::edtLatExit(TObject *Sender)
{
    edtLat->Text = dmMain->coordToStr(lat = dmMain->strToCoord(edtLat->Text), 'Y');
}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::edtLonExit(TObject *Sender)
{
    edtLon->Text = dmMain->coordToStr(lon = dmMain->strToCoord(edtLon->Text), 'X');
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::grdExistingStandCellClick(TColumn *Column)
{
    if (dstStand->RecordCount)
        rbtnExistingStand->Checked = true;
}
//---------------------------------------------------------------------------


void __fastcall TfrmNewTxWizard::dstStandLATITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::dstStandLONGITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------


void __fastcall TfrmNewTxWizard::edtNewStandAreaNumExit(TObject *Sender)
{
    findAreaByNum();
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtNewStandAreaNumKeyPress(
      TObject *Sender, char &Key)
{
    if (Key == 13)
        findAreaByNum();
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::rbtnExistingStandClick(TObject *Sender)
{
    grdExistingStand->Enabled = true;
    grdExistingStand->Font->Color = clWindowText;
    chbCheckInCoord->Enabled = true;
    lblRadius->Enabled = true;
    btnRadius->Enabled = true;
    seRadius->Enabled = true;
    seRadius->Color = clWindow;
    seRadius->Font->Color = clWindowText;

    Label3->Enabled = false;
    Label4->Enabled = false;
    Label5->Enabled = false;
    Label6->Enabled = false;
    Label7->Enabled = false;
    Label8->Enabled = false;
    Label17->Enabled = false;
    Label19->Enabled = false;
    edtNewLat->Enabled = false;
    edtNewLat->Font->Color = clBtnFace;
    edtNewLon->Enabled = false;
    edtNewLon->Font->Color = clBtnFace;
    edtNewStandName->Enabled = false;
    edtNewStandName->Font->Color = clBtnFace;
    edtNewStandAreaNum->Enabled = false;
    edtNewStandAreaNum->Font->Color = clBtnFace;
    cbxNewStandArea->Enabled = false;
    cbxNewStandArea->Font->Color = clBtnFace;
    cbxNewStandCity->Enabled = false;
    cbxNewStandCity->Font->Color = clBtnFace;
    cbxNewStandStreet->Enabled = false;
    cbxNewStandStreet->Font->Color = clBtnFace;
    edtNewStandAddress->Enabled = false;
    edtNewStandAddress->Font->Color = clBtnFace;
    edtSiteHeight->Enabled = false;
    edtSiteHeight->Font->Color = clBtnFace;

    sStandName = edtNewStandName->Text;
    sNumRegion = edtNewStandAreaNum->Text;
    sAddress = edtNewStandAddress->Text;
    numcbxRegion = cbxNewStandArea->ItemIndex;
    numcbxCity = cbxNewStandCity->ItemIndex;
    numcbxStreet = cbxNewStandStreet->ItemIndex;

    btnCity->Enabled = false;
    btnGetHeight->Enabled = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::rbtnNewStandClick(TObject *Sender)
{
    grdExistingStand->Enabled = false;
    grdExistingStand->Font->Color = clBtnFace;
    chbCheckInCoord->Enabled = false;

    lblRadius->Enabled = false;
    btnRadius->Enabled = false;
    seRadius->Enabled = false;
    seRadius->Color = clBtnFace;
    seRadius->Font->Color = clBtnFace;

    Label3->Enabled = true;
    Label4->Enabled = true;
    Label5->Enabled = true;
    Label6->Enabled = true;
    Label7->Enabled = true;
    Label8->Enabled = true;
    Label17->Enabled = true;
    Label19->Enabled = true;
    edtNewLat->Enabled = true;
    edtNewLat->Font->Color = clWindowText;
    edtNewLat->Text = dmMain->coordToStr(lat, 'Y');
    edtNewLon->Enabled = true;
    edtNewLon->Font->Color = clWindowText;
    edtNewLon->Text = dmMain->coordToStr(lon, 'X');
    edtNewStandName->Enabled = true;
    edtNewStandName->Font->Color = clWindowText;
    edtNewStandAreaNum->Enabled = true;
    edtNewStandAreaNum->Font->Color = clWindowText;
    cbxNewStandArea->Enabled = true;
    cbxNewStandArea->Font->Color = clWindowText;
    cbxNewStandCity->Enabled = true;
    cbxNewStandCity->Font->Color = clWindowText;
    cbxNewStandStreet->Enabled = true;
    cbxNewStandStreet->Font->Color = clWindowText;
    edtNewStandAddress->Enabled = true;
    edtNewStandAddress->Font->Color = clWindowText;
    edtSiteHeight->Enabled = true;
    edtSiteHeight->Font->Color = clWindowText;
    edtSiteHeight->Text = AnsiString(height_sea);


    edtNewStandName->Text = sStandName;
    edtNewStandAreaNum->Text = sNumRegion;
    edtNewStandAddress->Text = sAddress;

            area = BCCalcParams.defArea;
            if (area == -1) area == -2;

            numcbxRegion = cbxNewStandArea->Items->IndexOfObject((TObject*)area);


    cbxNewStandArea->ItemIndex = numcbxRegion;
    cbxNewStandAreaChange(this);

            city = BCCalcParams.defCity;
            if (city == -1) city == -2;
            numcbxCity = cbxNewStandCity->Items->IndexOfObject((TObject*)city);


            if (area == -2) area == -1;
            if (city == -2) city == -1;

            street = -1;
            numcbxStreet = -1;

    cbxNewStandCity->ItemIndex = numcbxCity;
    cbxNewStandCityChange(this);
    cbxNewStandStreet->ItemIndex = numcbxStreet;
    cbxNewStandStreetChange(this);


    btnCity->Enabled = true;
    btnGetHeight->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtNewLatExit(TObject *Sender)
{
    edtNewLat->Text = dmMain->coordToStr(standLat = dmMain->strToCoord(edtNewLat->Text), 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtNewLatKeyPress(TObject *Sender,
      char &Key)
{
    if (Key == 13)
        edtNewLat->Text = dmMain->coordToStr(standLat = dmMain->strToCoord(edtNewLat->Text), 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtNewLonExit(TObject *Sender)
{
    edtNewLon->Text = dmMain->coordToStr(standLon = dmMain->strToCoord(edtNewLon->Text), 'X');
}
//---------------------------------------------------------------------------


void __fastcall TfrmNewTxWizard::edtNewLonKeyPress(TObject *Sender,
      char &Key)
{
    if (Key == 13)
        edtNewLon->Text = dmMain->coordToStr(standLon = dmMain->strToCoord(edtNewLon->Text), 'X');
}
//---------------------------------------------------------------------------

int __fastcall TfrmNewTxWizard::findAreaByNum()
{
    if (edtNewStandAreaNum->Text.IsEmpty())
        return 0;
    int areaIdx = -1;

    sqlAreaByNum->Close();
    sqlAreaByNum->Params->Vars[0]->AsString = edtNewStandAreaNum->Text;
    sqlAreaByNum->ExecQuery();
    if (!sqlAreaByNum->Eof)


        for (int i = 1; i < cbxNewStandArea->Items->Count ; i++) {
            try {
              if (sqlAreaByNum->Fields[0]->AsInteger == (int)cbxNewStandArea->Items->Objects[i])
                    areaIdx = i;
            } catch (...) {;}
        }
//        areaIdx = cbxNewStandArea->Items->IndexOfObject((TObject *)sqlAreaByNum->Fields[0]->AsInteger);
    if (areaIdx > -1)
          cbxNewStandArea->ItemIndex = areaIdx;
    cbxNewStandArea->OnChange(this);
    //cbxNewStandAreaChange(
    return area;
}
void __fastcall TfrmNewTxWizard::cbxNewStandAreaChange(TObject *Sender)
{
    //  заполнить список нас. пунктов
    int parId = cbxNewStandArea->ItemIndex > -1 ? (int)cbxNewStandArea->Items->Objects[cbxNewStandArea->ItemIndex] : -1;
    if (parId == -2) parId = -1;
    int elId = cbxNewStandCity->ItemIndex > -1 ? (int)cbxNewStandCity->Items->Objects[cbxNewStandCity->ItemIndex] : -1;
    fillCombo(cbxNewStandCity, sqlCity, parId, elId);
    //  установить номер региона
    sqlAreaById->Close();
    sqlAreaById->Params->Vars[0]->AsInteger = parId;
    sqlAreaById->ExecQuery();
    if (!sqlAreaById->Eof)
        edtNewStandAreaNum->Text = sqlAreaById->Fields[0]->AsString;
    else
        edtNewStandAreaNum->Text = "";
    sqlAreaById->Close();
    area = parId;
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::cbxNewStandCityChange(TObject *Sender)
{
    int parId = cbxNewStandCity->ItemIndex > -1 ? (int)cbxNewStandCity->Items->Objects[cbxNewStandCity->ItemIndex] : -1;
    if (parId == -2) parId = -1;
    int elId = cbxNewStandStreet->ItemIndex > -1 ? (int)cbxNewStandStreet->Items->Objects[cbxNewStandStreet->ItemIndex] : -1;
    fillCombo(cbxNewStandStreet, sqlStreet, parId, elId);
    city = parId;
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::btnCityClick(TObject *Sender)
{
    // модальная версия справочника нас. пунктов
    if (cbxNewStandCity->ItemIndex > -1)
        FormProvider.ShowList(42, this->Handle, (int)cbxNewStandCity->Items->Objects[cbxNewStandCity->ItemIndex]);
    else
        FormProvider.ShowList(42, this->Handle, 0);
}
//---------------------------------------------------------------------------
void TfrmNewTxWizard::elementSelected(Messages::TMessage& msg)
{
    switch (msg.WParam) {
        case 42:
            sqlAreaByCity->Close();
            sqlAreaByCity->Params->Vars[0]->AsInteger = msg.LParam;
            sqlAreaByCity->ExecQuery();
            int areaId = sqlAreaByCity->Fields[0]->AsInteger;
            fillCombo(cbxNewStandArea, sqlArea, 0, areaId);
            sqlAreaByCity->Close();
            cbxNewStandCity->ItemIndex = cbxNewStandCity->Items->IndexOfObject((TObject*)msg.LParam);
            cbxNewStandCity->OnChange(this);
            break;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmNewTxWizard::fillCombo(TComboBox* cbx, TIBSQL* sql, int parentId, int elementId)
{
    //  заполяем указанный TComboBox значениями из выборки указанного sql с параметром parentId
    //  и устанавливаем текущий элемент на elementId, если есть.
    cbx->Items->Clear();
//    cbx->Text = "";
    sql->Close();
    sql->Transaction->CommitRetaining();
    if (!sql->Params->Names.IsEmpty())
        sql->Params->Vars[0]->AsInteger = parentId;
    sql->ExecQuery();
    while (!sql->Eof) {
        int id = sql->Fields[0]->AsInteger;
        if (id == -1) id = -2;
        cbx->Items->AddObject(sql->Fields[1]->AsString, (TObject*)id);
        sql->Next();
    }
    sql->Close();

    cbx->DropDownCount = Screen->Height / cbx->ItemHeight / 3;
    //if (cbx->DropDownCount > cbx->Items->Count)
    //    cbx->DropDownCount = cbx->Items->Count;

    //  установить нужный элемент
    if (elementId) {
        int newIdx = cbx->Items->IndexOfObject((TObject*)elementId);
        if (newIdx > -1 && newIdx < cbx->Items->Count)
            cbx->ItemIndex = newIdx;
    }
    //else if (cbx->Items->Count > 0) {
    //    cbx->ItemIndex = 0;
    //}
    if (cbx->OnChange)
        cbx->OnChange(this);
}


void __fastcall TfrmNewTxWizard::cbxSystemChange(TObject *Sender)
{
    int parId = cbxSystem->ItemIndex > -1 ? (int)cbxSystem->Items->Objects[cbxSystem->ItemIndex] : 0;
    int elId = cbxChannel->ItemIndex > -1 ? (int)cbxChannel->Items->Objects[cbxChannel->ItemIndex] : 0;
    sqlChannel->SQL->Text = "select ID, NAMECHANNEL from CHANNELS "
                            "where FREQUENCYGRID_ID = (select FREQUENCYGRID_ID from ANALOGTELESYSTEM where ID = :GRP_ID) "
                            "order by NAMECHANNEL";
    fillCombo(cbxChannel, sqlChannel, parId, elId);
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::cbxChannelChange(TObject *Sender)
{
    sqlVideoCarrier->Close();
    if (cbxChannel->ItemIndex == -1) {
        edtVideoCarrier->Text = "";
        edtSoundCarrier->Text = "";
        return;
    }
    sqlVideoCarrier->Params->Vars[0]->AsInteger = (int)cbxChannel->Items->Objects[cbxChannel->ItemIndex];
    sqlVideoCarrier->ExecQuery();
    if (!sqlVideoCarrier->Eof) {
        if (systemcast == ttTV) {
            freq_video = sqlVideoCarrier->Fields[0]->AsDouble;
            edtVideoCarrier->Text = FormatFloat("0.###", freq_video);
            freq_sound = sqlVideoCarrier->Fields[1]->AsDouble;
            edtSoundCarrier->Text = FormatFloat("0.###", freq_sound);
            freq_sound_sec = sqlVideoCarrier->Fields[4]->AsDouble;
        } else {
            freq_video = (sqlVideoCarrier->Fields[3]->AsDouble + sqlVideoCarrier->Fields[2]->AsDouble) / 2;
            edtVideoCarrier->Text = FormatFloat("0.###", freq_video);
        }
    } else {
        edtVideoCarrier->Text = "";
        edtSoundCarrier->Text = "";
    }
    sqlVideoCarrier->Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::cbxAllotmentChange(TObject *Sender)
{
    sqlSoundBlock->Close();
    if (cbxAllotment->ItemIndex == -1 ) {
        edtSoundCarrier->Text = "";
        edtBlock->Text = "";
        return;
    }
    sqlSoundBlock->Params->Vars[0]->AsInteger = (int)cbxAllotment->Items->Objects[cbxAllotment->ItemIndex];
    sqlSoundBlock->ExecQuery();
    if (!sqlSoundBlock->Eof) {
        edtSoundCarrier->Text = FormatFloat("0.###", sqlSoundBlock->Fields[1]->AsDouble);
        edtBlock->Text = sqlSoundBlock->Fields[0]->AsString;
    } else {
        edtSoundCarrier->Text = "";
        edtBlock->Text = "";
    }
    sqlSoundBlock->Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtSoundCarrierExit(TObject *Sender)
{
    if (!(edtSoundCarrier->Text.IsEmpty())) {
        freq_sound = StrToFloat(edtSoundCarrier->Text);
        if (systemcast == ttFM && !(freq_sound > 66 && freq_sound < 74) && !(freq_sound > 87.5 && freq_sound < 107.9))
            throw *(new Exception("Частота передавача НВЧ ЧМ повинна бути в діапазонах I (66 - 74) або II (87.5 - 107.9) МГц"));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtAntHeightExit(TObject *Sender)
{
    StrToInt(edtAntHeight->Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtFeederLossExit(TObject *Sender)
{
    StrToFloat(edtFeederLoss->Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtPowerExit(TObject *Sender)
{
    StrToFloat(edtPower->Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtSiteHeightExit(TObject *Sender)
{
    StrToInt(edtSiteHeight->Text);
}
//---------------------------------------------------------------------------



void __fastcall TfrmNewTxWizard::edtAdminIdExit(TObject *Sender)
{
    if (edtAdminId->Text.Length() > 4)
        throw *(new Exception("Довжина ID не повинна перевищувати 4 символи"));
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::chbGenerateIDClick(TObject *Sender)
{
    if (chbGenerateID->Checked) {
        lblID->Enabled = false;
        edtAdminId->Text = "";
        edtAdminId->Enabled = false;
    } else {
        lblID->Enabled = true;
        edtAdminId->Enabled = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::btnGetHeightClick(TObject *Sender)
{
    Rsageography_tlb::TRSAGeoPoint GeoP;
    GeoP.L = lon;
    GeoP.H = lat;

    Rsageography_tlb::TRSAAltitude Alt;

    try {
        BCCalcParams.FTerrInfoSrv->Get_Altitude(GeoP, &Alt);
        edtSiteHeight->Text = AnsiString(Alt);
        height_sea = Alt;
    } catch (...) {
        Application->MessageBox("Помилка визначення висоти", Application->Title.c_str(), MB_ICONERROR | MB_OK);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::cbxDirectChange(TObject *Sender)
{
    if (cbxDirect->Text == "D") {
        //  направленная
        Label25->Enabled = true;
        sgrAntennaGain->Enabled = true;
        sgrAntennaGain->Color = clWindow;
        sgrAntennaGain->Font->Color = clWindowText;
        panAntGainGraph->BevelOuter = bvRaised;
        antennaDiagramPanel->Visible = true;
        panAntGainGraph->Visible = true;
    } else {
        Label25->Enabled = false;
        sgrAntennaGain->Enabled = false;
        sgrAntennaGain->Color = clBtnFace;
        sgrAntennaGain->Font->Color = clBtnFace;
        panAntGainGraph->BevelOuter = bvNone;
        antennaDiagramPanel->Visible = false;
        panAntGainGraph->Visible = false;
    }

    if (cbxDirect->Text == "ND") {
        //  ненаправленная
        Label22->Enabled = true;
        edtAntennaGain->Enabled = true;
        edtAntennaGain->Color = clWindow;
        edtAntennaGain->Font->Color = clWindowText;
    } else {
        Label22->Enabled = false;
        edtAntennaGain->Enabled = false;
        edtAntennaGain->Color = clBtnFace;
        edtAntennaGain->Font->Color = clBtnFace;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::cbxChannelGridChange(TObject *Sender)
{
    int parId = cbxChannelGrid->ItemIndex > -1 ? (int)cbxChannelGrid->Items->Objects[cbxChannelGrid->ItemIndex] : 0;
    int elId = cbxChannel->ItemIndex > -1 ? (int)cbxChannel->Items->Objects[cbxChannel->ItemIndex] : 0;
    sqlChannel->SQL->Text = "select ID, NAMECHANNEL from CHANNELS "
                            "where FREQUENCYGRID_ID = :GRP_ID "
                            "order by NAMECHANNEL";
    fillCombo(cbxChannel, sqlChannel, parId, elId);
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::btnResetChannelClick(TObject *Sender)
{
    if (systemcast == ttTV || systemcast == ttDVB) {
        cbxChannel->ItemIndex = -1;
        cbxChannelChange(Sender);
    }
    if (systemcast == ttDAB) {
        cbxAllotment->ItemIndex = -1;
        cbxAllotmentChange(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TAntennaDiagramPanel::Paint(void)
{
    Canvas->Brush->Style = bsSolid;
    Canvas->Brush->Color = Color;
    Canvas->FillRect(ClientRect);
    Canvas->Pen->Color = clBlack;
    Canvas->Pen->Width = 1;
    Canvas->MoveTo(Width / 2, 0);
    Canvas->LineTo(Width / 2, Height);
    Canvas->MoveTo(0, Height / 2);
    Canvas->LineTo(Width, Height / 2);

    Canvas->MoveTo(Width / 2, Height / 2);
    Canvas->Pen->Width = 5;
    Canvas->LineTo(Width / 2, Height / 2);

    drawZone(coverageZone, clBlue, 2);

    Canvas->Font->Color = clGreen;
}

void __fastcall TAntennaDiagramPanel::drawZone(double* zone, TColor color, int lineWeight, int numOfPoints)
{
    if (numOfPoints && norma) {
        Canvas->Pen->Width = lineWeight;
        Canvas->Pen->Color = color;
        Canvas->MoveTo(Width / 2, Height / 2 - screenValue(zone[0]*0.98) /**4.0/3.0*/);
        for (int i = 1; i < numOfPoints; i++) {
            double angle = 360.0 / numOfPoints * i / 180 * M_PI;
            Canvas->LineTo(Width / 2 + screenValue(zone[i]*0.98)*sin(angle), Height / 2 - screenValue(zone[i]*0.98)/**4.0/3.0*/ * cos(angle));
        }
        Canvas->LineTo(Width / 2, Height / 2 - screenValue(zone[0]*0.98)/**4.0/3.0*/);
    }
}

void __fastcall TAntennaDiagramPanel::clear()
{
    for (double *f = coverageZone; f < coverageZone + 36; f++) *f = 0;
    norma = 0.0;
    Invalidate();
}
void __fastcall TAntennaDiagramPanel::setCoverage(double *zone)
{
    double *s = zone;
    for (double *d = coverageZone; d < coverageZone + 36; d++, s++) *d = *s;
    findNorma();
    Invalidate();
}

void __fastcall TAntennaDiagramPanel::findNorma()
{
    norma = 0.0;
    for (double *f = coverageZone; f < coverageZone + 36; f++) if (norma < *f) norma = *f;
}


void __fastcall TfrmNewTxWizard::sgrAntennaGainSetEditText(TObject *Sender,
      int ACol, int ARow, const AnsiString Value)
{
    for (int i = 0; i < 36; i++) {
      try {
        diagram[i] = StrToFloat(sgrAntennaGain->Cells[1][i]);
      } catch (...) {
        diagram[i] = 0;
      }
    }
    antennaDiagramPanel->setCoverage(diagram);
}
//---------------------------------------------------------------------------



void __fastcall TfrmNewTxWizard::grdExistingStandEnter(TObject *Sender)
{
 dstStandAfterScroll(dstStand);
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::dstStandAfterScroll(TDataSet *DataSet)
{
    if (rbtnExistingStand->Checked == true) {
        edtNewLat->Text = dmMain->coordToStr(dstStandLATITUDE->AsFloat, 'Y');
        edtNewLon->Text = dmMain->coordToStr(dstStandLONGITUDE->AsFloat, 'X');
        edtNewStandName->Text = dstStandNAMESITE->AsString;
        edtNewStandAreaNum->Text = dstStandA_NUMREGION->AsString;
        edtNewStandAddress->Text = dstStandADDRESS->AsString;
        edtSiteHeight->Text = dstStandS_HEIGHT_SEA->AsString;

        cbxNewStandArea->ItemIndex = cbxNewStandArea->Items->IndexOfObject((TObject*)dstStandAREA_ID->AsInteger);
        cbxNewStandAreaChange(this);
        cbxNewStandCity->ItemIndex = cbxNewStandCity->Items->IndexOfObject((TObject*)dstStandCITY_ID->AsInteger);
        cbxNewStandCityChange(this);
        cbxNewStandStreet->ItemIndex = cbxNewStandStreet->Items->IndexOfObject((TObject*)dstStandSTREET_ID->AsInteger);
        cbxNewStandStreetChange(this);
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmNewTxWizard::cbxNewStandStreetChange(TObject *Sender)
{
    if (cbxNewStandStreet->ItemIndex != -1)
        street = (int)cbxNewStandStreet->Items->Objects[cbxNewStandStreet->ItemIndex];
    if (street == -2) street = -1;
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::edtNewStandAddressExit(TObject *Sender)
{
  address = edtNewStandAddress->Text;
}
//---------------------------------------------------------------------------

void __fastcall TfrmNewTxWizard::btnRadiusClick(TObject *Sender)
{
    dstStand->Close();
    dstStand->ParamByName("LAT")->AsDouble = lat;
    dstStand->ParamByName("LON")->AsDouble = lon;
    dstStand->ParamByName("DIF")->AsDouble = dmMain->strToCoord(AnsiString().sprintf("00N%02d", seRadius->Value));
    try {
        dstStand->Open();
        Variant locvalues[2];
        locvalues[0] = Variant(lat);
        locvalues[1] = Variant(lon);
        dstStand->Locate("LATITUDE;LONGITUDE", VarArrayOf(locvalues, 1), TLocateOptions());
    } catch(Exception &E) {
        Application->ShowException(&E);
    }
}
//---------------------------------------------------------------------------



