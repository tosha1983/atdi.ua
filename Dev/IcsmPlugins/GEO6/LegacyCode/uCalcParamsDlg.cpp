//---------------------------------------------------------------------
#include <vcl.h>
#include <memory>
#include <FileCtrl.hpp>
#pragma hdrstop

#include "uCalcParamsDlg.h"
#include "uSelectServer.h"
#include "uSaveServer.h"
//---------------------------------------------------------------------
#pragma link "CSPIN"
#pragma link "NumericEdit"
#pragma resource "*.dfm"
//  блядская ATL
#ifdef StrToInt
#undef StrToInt
#endif
TdlgCalcParams *dlgCalcParams;
//---------------------------------------------------------------------
__fastcall TdlgCalcParams::TdlgCalcParams(TComponent* AOwner)
	: TForm(AOwner)
{
     //  загрузка параметров из объекта
    try {


    } catch (Exception &e) {
        Application->MessageBox("Помилка читання даних з реєстру", Application->Title.c_str(), MB_ICONERROR | MB_OK);
        // throw *(new Exception("Помилка читання даних з реєстру"));
    }

    pcParams->ActivePage = tshCalcParams;
}
//---------------------------------------------------------------------
void __fastcall TdlgCalcParams::btnOkClick(TObject *Sender)
{
    try {
        BCCalcParams.CalcServerArrayGUID = cbxCalcServer->ItemIndex > -1 ?
            BCCalcParams.CalcServerArray[cbxCalcServer->ItemIndex].guid : String();
        BCCalcParams.PropagServerArrayGUID = cbxPropagServer->ItemIndex > -1 ?
            BCCalcParams.PropagServerArray[cbxPropagServer->ItemIndex].guid : String();
        BCCalcParams.ReliefServerArrayGUID = cbxReliefServer->ItemIndex > -1 ?
            BCCalcParams.ReliefServerArray[cbxReliefServer->ItemIndex].guid : String();

        BCCalcParams.CalcServerName = cbxCalcServer->ItemIndex > -1 ?
            BCCalcParams.CalcServerArray[cbxCalcServer->ItemIndex].name : String();
        BCCalcParams.PropagServerName = cbxPropagServer->ItemIndex > -1 ?
            BCCalcParams.PropagServerArray[cbxPropagServer->ItemIndex].name : String();
        BCCalcParams.ReliefServerName = cbxReliefServer->ItemIndex > -1 ?
            BCCalcParams.ReliefServerArray[cbxReliefServer->ItemIndex].name : String();

        if (cbxReliefServer->ItemIndex > -1)
            BCCalcParams.ReliefServerArray[cbxReliefServer->ItemIndex].params = edtPathData->Text;
        BCCalcParams.ReliefPath = edtPathData->Text;

        BCCalcParams.calcMethod = rgrEminMethod->ItemIndex;

        BCCalcParams.UseHeff = chbHeff->Checked ? -1 : 0;
        BCCalcParams.UseTxClearence = chbTxClearance->Checked ? -1 : 0;
        BCCalcParams.UseRxClearence = chbRxClearance->Checked ? -1 : 0;
        BCCalcParams.UseMorfology = chbMorphology->Checked ? -1 : 0;
        BCCalcParams.Step = StrToFloat(edtStep->Text);

        BCCalcParams.UseHeffTheo = chbHeffTheo->Checked ? -1 : 0;
        BCCalcParams.UseTxClearenceTheo = chbTxClearanceTheo->Checked ? -1 : 0;
        BCCalcParams.UseRxClearenceTheo = chbRxClearanceTheo->Checked ? -1 : 0;
        BCCalcParams.UseMorfologyTheo = chbMorphologyTheo->Checked ? -1 : 0;
        BCCalcParams.StepTheo = StrToFloat(edtStepTheo->Text);
        BCCalcParams.TheoPathTheSame = chbTheoPathTheSame->Checked;

        BCCalcParams.SelectionAutotruncation = chbSelectionAutotruncation->Checked;
        BCCalcParams.minSelInterf = StrToFloat(edtMinSelInterf->Text);
        BCCalcParams.higherIntNum = StrToInt(edtHigherIntNum->Text);
        BCCalcParams.mapAutoFit = chbMapAutoFit->Checked;
        BCCalcParams.rpcRxModeLink = chRpcRxModeLink->Checked;
        BCCalcParams.duelAutoRecalc = chbDuelAutoRecalc->Checked;
        int ds = BCCalcParams.degreeStep = StrToInt(cbxDegreeStep->Text);
        if (ds != 1 && ds != 5 && ds!= 10) {
            if (ds < 1)
                BCCalcParams.degreeStep = 1;
            else if (ds > 10)
                BCCalcParams.degreeStep = 10;
            else
                BCCalcParams.degreeStep = 5;
        }
        cbxDegreeStep->Text = IntToStr(BCCalcParams.degreeStep);
        BCCalcParams.showCp = chbShowCp->Checked;
        BCCalcParams.treshVideo = edtTreshVideo->Text.ToDouble();
        BCCalcParams.treshAudio = edtTreshAudio->Text.ToDouble();

        BCCalcParams.QueryOnMainormClose = chbQueryOnMainormClose->Checked;
        BCCalcParams.GetCoordinatesFromBase = chbGetCoordinatesFromBase->Checked;
        BCCalcParams.DisableReliefAtPlanning = cbxDisableReliefAtPlanning->Checked;
        BCCalcParams.duelType = rgDuelType->ItemIndex;
        BCCalcParams.earthCurveInRelief = chbEarthCurveInRelief->Checked;
        BCCalcParams.ShowTxNames = chbShowTxNames->Checked;

        try{
        BCCalcParams.lineThicknessZoneCover = StrToInt(lineThicknessZoneCover->Text);
        } catch(...) {;}
        try{
        BCCalcParams.lineThicknessZoneNoise = StrToInt(lineThicknessZoneNoise->Text);
        } catch(...) {;}
        try{
        BCCalcParams.lineThicknessZoneInterfere = StrToInt(lineThicknessZoneInterfere->Text);
        } catch(...) {;}

        BCCalcParams.lineColorZoneCover = cbxLineColorZoneCover->Selected;
        BCCalcParams.lineColorZoneNoise = cbxLineColorZoneNoise->Selected;
        BCCalcParams.lineColorZoneInterfere = cbxLineColorZoneInterfere->Selected;
        BCCalcParams.lineColorZoneInterfere2 = cbxLineColorZoneInterfere2->Selected;
        BCCalcParams.coordinationPointsInZoneColor = cbxCoordinationPointsInZoneColor->Selected;
        BCCalcParams.coordinationPointsOutZoneColor = cbxCoordinationPointsOutZoneColor->Selected;
        BCCalcParams.changedTxColor = cbxChangedTxColor->Selected;

        BCCalcParams.Emin_dvb_200 = StrToFloat(edtEmin_dvb_200->Text);
        BCCalcParams.Emin_dvb_500 = StrToFloat(edtEmin_dvb_500->Text);
        BCCalcParams.Emin_dvb_700 = StrToFloat(edtEmin_dvb_700->Text);
        //BCCalcParams.Dvb_antenna_discrimination = cbxDvb_antenna_discrimination->Checked;
        BCCalcParams.Quick_calc_duel_interf = cbxQuick_calc_duel_interf->Checked;
        BCCalcParams.Quick_calc_max_dist = cbxQuick_calc_max_dist->Checked;
        BCCalcParams.RequestForCoordDist = chbRequestForCoordDist->Checked;
        BCCalcParams.Coord_dist_ini_file = edtCoord_dist_ini_file->Text;
        BCCalcParams.backLobeFmMono = StrToFloat(edtBackLobeFmMono->Text);
        BCCalcParams.backLobeFmStereo = StrToFloat(edtBackLobeFmStereo->Text);
        BCCalcParams.backLobeTvBand2 = StrToFloat(edtBackLobeTvBand2->Text);
        BCCalcParams.polarCorrectFm = StrToFloat(edtPolarCorrectFm->Text);
        BCCalcParams.tvSoundStereo = chbTvSoundStereo->Checked;
        BCCalcParams.stepCalcMaxDist = StrToFloat(edtStepCalcMaxDist->Text);

        if (BCCalcParams.FCalcSrv.IsBound())
            HrCheck(BCCalcParams.FCalcSrv->SetLogFileName(WideString(edtCalcLog->Text)));

        if (cbxNewStandArea->ItemIndex > -1)
            BCCalcParams.defArea = (int)cbxNewStandArea->Items->Objects[cbxNewStandArea->ItemIndex];
        else
            BCCalcParams.defArea = -1;
        if (BCCalcParams.defArea == -2) BCCalcParams.defArea = -1;
        if (cbxNewStandCity->ItemIndex > -1)
            BCCalcParams.defCity = (int)cbxNewStandCity->Items->Objects[cbxNewStandCity->ItemIndex];
        else
            BCCalcParams.defCity = -1;
        if (BCCalcParams.defCity == -2) BCCalcParams.defCity = -1;

        BCCalcParams.standRadius = seRadius->Value;
        BCCalcParams.filesNum = edtFilesNum->Text.ToInt();

        BCCalcParams.doMapInitDelay = cbMapInitDelay->Checked;
        BCCalcParams.mapInitDelay = edtMapInitDelay->Text.ToInt();
        if (BCCalcParams.mapInitDelay < 0) BCCalcParams.mapInitDelay = 0;
        BCCalcParams.doMapInitInfo = cbMapInitInfo->Checked;


    } catch(Exception& E) {
        ModalResult = mrNone;
        throw *(new Exception(AnsiString("Помилка збереження даних:\n") + E.Message));
    }

    BCCalcParams.save();
    BCCalcParams.load();

    Close();
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btnPathClick(TObject *Sender)
{
    AnsiString Dir = "C:\\";
    if (SelectDirectory(Dir, TSelectDirOpts() << sdAllowCreate << sdPerformCreate << sdPrompt,0))
        edtPathData->Text = Dir;
    BCCalcParams.ReliefServerArray[cbxReliefServer->ItemIndex].params=edtPathData->Text;
    BCCalcParams.ReliefPath = edtPathData->Text;
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btnCancelClick(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::cbxReliefServerChange(TObject *Sender)
{
    edtPathData->Text = BCCalcParams.ReliefServerArray[cbxReliefServer->ItemIndex].params;
    BCCalcParams.ReliefPath = edtPathData->Text;
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::edtStepExit(TObject *Sender)
{
    try { if (StrToFloat(edtStep->Text) <= 0) {
        Application->MessageBox("Шаг не може бути рівним або меньшим за нуль", Application->Title.c_str(), MB_ICONERROR | MB_OK);
        edtStep->Text = "1";
        }
    } catch (...) {
        edtStep->Text = "1";
        throw ;
    }
}
//---------------------------------------------------------------------------


void __fastcall TdlgCalcParams::btnCalcLogClick(TObject *Sender)
{
    if (OpenDialog1->Execute())
        edtCalcLog->Text = OpenDialog1->FileName;
}
//---------------------------------------------------------------------------


void __fastcall TdlgCalcParams::FormShow(TObject *Sender)
{

    if (BCCalcParams.FCalcSrv.IsBound()) {
        int cm = BCCalcParams.calcMethod;
        if (cm < -1 || cm > 2) cm = -1;
        rgrEminMethod->ItemIndex = cm;
    } else 
        rgrEminMethod->ItemIndex = -1;

    edtHigherIntNum->Text = IntToStr(BCCalcParams.higherIntNum);
    edtMinSelInterf->Text = FormatFloat("0.0#", BCCalcParams.minSelInterf);

    seRadius->Value = BCCalcParams.standRadius;

    int defArea = BCCalcParams.defArea;
    if (defArea == -1) defArea = -2;
    fillCombo(cbxNewStandArea, sqlArea, 0, defArea);

    int defCity = BCCalcParams.defCity;
    if (defCity == -1) defCity = -2;
    cbxNewStandCity->ItemIndex = cbxNewStandCity->Items->IndexOfObject((TObject*)BCCalcParams.defCity);

    edtFilesNum->Text = IntToStr(BCCalcParams.filesNum);

    cbxDegreeStep->ItemIndex = cbxDegreeStep->Items->IndexOf(IntToStr(BCCalcParams.degreeStep));
    chbShowCp->Checked = BCCalcParams.showCp;
    edtTreshVideo->OldValue = FormatFloat("0.##", BCCalcParams.treshVideo);
    edtTreshAudio->OldValue = FormatFloat("0.##", BCCalcParams.treshAudio);

    cbxLineColorZoneCover->Selected = BCCalcParams.lineColorZoneCover;
    cbxLineColorZoneNoise->Selected = BCCalcParams.lineColorZoneNoise;
    cbxLineColorZoneInterfere->Selected = BCCalcParams.lineColorZoneInterfere;
    cbxLineColorZoneInterfere2->Selected = BCCalcParams.lineColorZoneInterfere2;
    cbxCoordinationPointsInZoneColor->Selected = BCCalcParams.coordinationPointsInZoneColor;
    cbxCoordinationPointsOutZoneColor->Selected = BCCalcParams.coordinationPointsOutZoneColor;
    cbxChangedTxColor->Selected = BCCalcParams.changedTxColor;

    int idx=-1;
    cbxCalcServer->Items->Clear();
    for (int i = 0; i < BCCalcParams.CalcServerArray.size(); i++)
    {
         cbxCalcServer->Items->Add(BCCalcParams.CalcServerArray[i].name);
         if (BCCalcParams.CalcServerArray[i].guid == BCCalcParams.CalcServerArrayGUID) idx = i;
    }

    cbxCalcServer->ItemIndex = idx;

    idx = -1;
    cbxPropagServer->Items->Clear();
    for (int i = 0; i < BCCalcParams.PropagServerArray.size(); i++)
    {
         cbxPropagServer->Items->Add(BCCalcParams.PropagServerArray[i].name);
         if (BCCalcParams.PropagServerArray[i].guid == BCCalcParams.PropagServerArrayGUID) idx = i;
    }

    cbxPropagServer->ItemIndex = idx;

    idx = -1;
    cbxReliefServer->Items->Clear();
    for (int i = 0; i < BCCalcParams.ReliefServerArray.size(); i++)
    {
         cbxReliefServer->Items->Add(BCCalcParams.ReliefServerArray[i].name);
         if (BCCalcParams.ReliefServerArray[i].guid == BCCalcParams.ReliefServerArrayGUID)
         {
            idx = i;
            edtPathData->Text = BCCalcParams.ReliefServerArray[i].params;
         }
    }
    cbxReliefServer->ItemIndex = idx;

    chbHeff->Checked = BCCalcParams.UseHeff;
    chbTxClearance->Checked = BCCalcParams.UseTxClearence;
    chbRxClearance->Checked = BCCalcParams.UseRxClearence;
    chbMorphology->Checked = BCCalcParams.UseMorfology;
    edtStep->Text = FormatFloat("0.###",BCCalcParams.Step);

    chbHeffTheo->Checked = BCCalcParams.UseHeffTheo;
    chbTxClearanceTheo->Checked = BCCalcParams.UseTxClearenceTheo;
    chbRxClearanceTheo->Checked = BCCalcParams.UseRxClearenceTheo;
    chbMorphologyTheo->Checked = BCCalcParams.UseMorfologyTheo;
    edtStepTheo->Text = FormatFloat("0.###",BCCalcParams.StepTheo);
    chbTheoPathTheSame->Checked = BCCalcParams.TheoPathTheSame;
    chbTheoPathTheSameClick(NULL);

    chbSelectionAutotruncation->Checked = BCCalcParams.SelectionAutotruncation;
    chbMapAutoFit->Checked = BCCalcParams.mapAutoFit;
    chRpcRxModeLink->Checked = BCCalcParams.rpcRxModeLink;
    chbDuelAutoRecalc->Checked = BCCalcParams.duelAutoRecalc;

    chbQueryOnMainormClose->Checked = BCCalcParams.QueryOnMainormClose;
    chbGetCoordinatesFromBase->Checked = BCCalcParams.GetCoordinatesFromBase;
    cbxDisableReliefAtPlanning->Checked = BCCalcParams.DisableReliefAtPlanning;
    rgDuelType->ItemIndex = BCCalcParams.duelType;
    chbEarthCurveInRelief->Checked = BCCalcParams.earthCurveInRelief;
    chbShowTxNames->Checked = BCCalcParams.ShowTxNames;

    lineThicknessZoneCover->Text = IntToStr(BCCalcParams.lineThicknessZoneCover);
    lineThicknessZoneNoise->Text = IntToStr(BCCalcParams.lineThicknessZoneNoise);
    lineThicknessZoneInterfere->Text = IntToStr(BCCalcParams.lineThicknessZoneInterfere);

    edtEmin_dvb_200->Text = FormatFloat("0.0##", BCCalcParams.Emin_dvb_200);
    edtEmin_dvb_500->Text = FormatFloat("0.0##",BCCalcParams.Emin_dvb_500);
    edtEmin_dvb_700->Text = FormatFloat("0.0##",BCCalcParams.Emin_dvb_700);
    //cbxDvb_antenna_discrimination->Checked = BCCalcParams.Dvb_antenna_discrimination;
    cbxQuick_calc_duel_interf->Checked = BCCalcParams.Quick_calc_duel_interf;
    cbxQuick_calc_max_dist->Checked = BCCalcParams.Quick_calc_max_dist;
    chbRequestForCoordDist->Checked = BCCalcParams.RequestForCoordDist;
    chbRequestForCoordDistClick(this);
    edtCoord_dist_ini_file->Text = BCCalcParams.Coord_dist_ini_file;
    edtBackLobeFmMono->Text = FormatFloat("0.0##", BCCalcParams.backLobeFmMono);
    edtBackLobeFmStereo->Text = FormatFloat("0.0##", BCCalcParams.backLobeFmStereo);
    edtBackLobeTvBand2->Text = FormatFloat("0.0##", BCCalcParams.backLobeTvBand2);
    edtPolarCorrectFm->Text = FormatFloat("0.0##", BCCalcParams.polarCorrectFm);
    chbTvSoundStereo->Checked = BCCalcParams.tvSoundStereo;
    edtStepCalcMaxDist->OldValue = FormatFloat("0.0##", BCCalcParams.stepCalcMaxDist);

    cbMapInitDelay->Checked = BCCalcParams.doMapInitDelay;
    cbMapInitDelayClick(this);
    edtMapInitDelay->OldValue = IntToStr(BCCalcParams.mapInitDelay);
    cbMapInitInfo->Checked = BCCalcParams.doMapInitInfo;
    
}
//---------------------------------------------------------------------------


void __fastcall TdlgCalcParams::fillCombo(TComboBox* cbx, TIBSQL* sql, int parentId, int elementId)
{
    //  заполяем указанный TComboBox значениями из выборки указанного sql с параметром parentId
    //  и устанавливаем текущий элемент на elementId, если есть.
    cbx->Items->Clear();
    cbx->Text = "";
    if (cbx == cbxNewStandCity)
        cbx->Items->AddObject("<Не визначений>", (TObject*)(0));
    sql->Close();
    sql->Transaction->CommitRetaining();
    if (!sql->Params->Names.IsEmpty())
        sql->Params->Vars[0]->AsInteger = parentId;
    sql->ExecQuery();
    while (!sql->Eof) {
        int id = sql->Fields[0]->AsInteger;
        if (id == -1) id = -2; // иначе глючит при
        cbx->Items->AddObject(sql->Fields[1]->AsString, (TObject*)id);
        sql->Next();
    }
    sql->Close();

    //  установить нужный элемент
    if (elementId) {
        cbx->ItemIndex = cbx->Items->IndexOfObject((TObject*)elementId);
    }
    //else if (cbx->Items->Count > 0) {
    //    cbx->ItemIndex = 0;
    //}

    if (cbx->Items->Count > 20)
        cbx->DropDownCount = 20;
    else
        cbx->DropDownCount = cbx->Items->Count;

    if (cbx->OnChange)
        cbx->OnChange(this);

}
void __fastcall TdlgCalcParams::cbxNewStandAreaChange(TObject *Sender)
{
    if (cbxNewStandArea->ItemIndex != -1) {
        int parent = (int)cbxNewStandArea->Items->Objects[cbxNewStandArea->ItemIndex];
        if (parent == -2) parent == -1;
        fillCombo(cbxNewStandCity, sqlCity, parent, 0);
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::edtFilesNumExit(TObject *Sender)
{
    int temp = 90;
    try {
        temp = edtFilesNum->Text.ToInt();
    } __finally {
        edtFilesNum->Text = IntToStr(temp);
    }               
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btnDistPathClick(TObject *Sender)
{
    std::auto_ptr<TOpenDialog> openDialog(new TOpenDialog(this));
      openDialog->Filter = "Ini files (*.ini)|*.INI|All files (*.*)|*.*";
      openDialog->InitialDir = GetCurrentDir();

    if ( openDialog->Execute() )
        edtCoord_dist_ini_file->Text = openDialog->FileName;
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::chbSelectionAutotruncationClick(TObject *Sender)
{
    if ( chbSelectionAutotruncation->Checked )
    {
        edtMinSelInterf->Color = clWindow;
        edtMinSelInterf->Enabled = true;
        edtMinSelInterf->Font->Color = clWindowText;
    }
    else
    {
        edtMinSelInterf->Color = clBtnFace;
        edtMinSelInterf->Enabled = false;
        edtMinSelInterf->Font->Color = clBtnFace;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::edtTreshVideoExit(TObject *Sender)
{
    TNumericEdit* ne = dynamic_cast<TNumericEdit*>(Sender);

    try {
        if (StrToFloat(ne->Text) <= 0.0) {
            Application->MessageBox("Значення не може бути рівним або меньшим за нуль", Application->Title.c_str(), MB_ICONERROR | MB_OK);
            ne->Text = FloatToStr(0.1);
        }
    } catch (...) {
        ne->Text = FloatToStr(0.1);
        throw;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::panelLineColorZoneCoverClick(TObject *Sender)
{
    if ( colorDialog->Execute() )
        if ( dynamic_cast<TPanel*>(Sender) )
            dynamic_cast<TPanel*>(Sender)->Color = colorDialog->Color;
}
//---------------------------------------------------------------------------
void __fastcall TdlgCalcParams::cbxLineColorZoneCoverDblClick(
      TObject *Sender)
{
    if ( dynamic_cast<TColorBox*>(Sender) )
    {
        colorDialog->Color = dynamic_cast<TColorBox*>(Sender)->Selected;
        if ( colorDialog->Execute() )
            dynamic_cast<TColorBox*>(Sender)->Selected = colorDialog->Color;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::chbRequestForCoordDistClick(
      TObject *Sender)
{
    cbxQuick_calc_max_dist->Enabled = !chbRequestForCoordDist->Checked;
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::chbTheoPathTheSameClick(TObject *Sender)
{
    bool notTheSame = !chbTheoPathTheSame->Checked;
    chbHeffTheo->Enabled = notTheSame;
    chbTxClearanceTheo->Enabled = notTheSame;
    chbRxClearanceTheo->Enabled = notTheSame;
    chbMorphologyTheo->Enabled = notTheSame;
    gbxPathTheo->Enabled = notTheSame;
    edtStepTheo->Enabled = notTheSame;
    edtStepTheo->Font->Color = notTheSame ? clWindowText : clBtnFace;
    edtStepTheo->Color = notTheSame ? clWindow : clBtnFace;
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btCalcAddClick(TObject *Sender)
{
    AddServer(cbxCalcServer, BCCalcParams.CalcServerArray, IID_ILISBCCalc);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btPpgAddClick(TObject *Sender)
{
    AddServer(cbxPropagServer, BCCalcParams.PropagServerArray, IID_IPropagation);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btDtmAddClick(TObject *Sender)
{
    AddServer(cbxReliefServer, BCCalcParams.ReliefServerArray, IID_IRSATerrainInfo);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btCalcRmvClick(TObject *Sender)
{
    RemoveServer(cbxCalcServer, BCCalcParams.CalcServerArray);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btPpgRmvClick(TObject *Sender)
{
    RemoveServer(cbxPropagServer, BCCalcParams.PropagServerArray);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btDtmRmvClick(TObject *Sender)
{
    RemoveServer(cbxReliefServer, BCCalcParams.ReliefServerArray);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::AddServer(TComboBox *cbx, ServParamsArray& arr, GUID& iid)
{
    if (!dlgSelectServer)
        dlgSelectServer = new TdlgSelectServer(Application);
    dlgSelectServer->iid = iid;
    dlgSelectServer->arr = arr;
    dlgSelectServer->Clear();

    if(dlgSelectServer->ShowModal() == mrOk && dlgSelectServer->arr.size() > 0 )
    {
        arr = dlgSelectServer->arr;
        AnsiString buf;
        if(cbx->Items->Count > 1)
        {
            buf = cbx->Items->operator [](cbx->Items->Count-1);
            cbx->Items->Delete(cbx->Items->Count-1);
            for(int i=1; i<=dlgSelectServer->addCount; ++i)
                cbx->Items->Add(arr[arr.size()-i].name);
            cbx->Items->Add(buf);
        }
        else
        {
            for(int i=1; i<=dlgSelectServer->addCount; ++i)
                cbx->Items->Add(arr[arr.size()-i].name);
        }
        cbx->ItemIndex = 0;
        dlgSelectServer->ServerList->Clear();
    }
}

void __fastcall TdlgCalcParams::RemoveServer(TComboBox *cbx, ServParamsArray& arr)
{
    AnsiString s="Ви дійсно бажаєте видалити сервер "+cbx->Items->operator [](cbx->ItemIndex)+" зі списку?";
    if(Application->MessageBox(s.c_str(), Application->Title.c_str(),
                                    MB_ICONQUESTION | MB_YESNO) == IDYES)
    {
        for(int i=0;i<arr.size();++i)
            if(arr[i].name == cbx->Items->operator [](cbx->ItemIndex))
            {
                arr.erase(&arr[i]);
                break;
            }
        cbx->DeleteSelected();
    }
    cbx->ItemIndex = 0;
}

void __fastcall TdlgCalcParams::edtPathDataExit(TObject *Sender)
{
    BCCalcParams.ReliefServerArray[cbxReliefServer->ItemIndex].params = edtPathData->Text;    
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btCalcEdtClick(TObject *Sender)
{
    EditServer(cbxCalcServer, BCCalcParams.CalcServerArray);    
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btPrgEdtClick(TObject *Sender)
{
    EditServer(cbxPropagServer, BCCalcParams.PropagServerArray);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::btDtmEdtClick(TObject *Sender)
{
    EditServer(cbxReliefServer, BCCalcParams.ReliefServerArray);
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::EditServer(TComboBox * cbx, ServParamsArray& arr)
{
    if (cbx->ItemIndex == -1)
        return;

    if (dlgSaveServer == NULL)
        dlgSaveServer = new TdlgSaveServer(Application);
    dlgSaveServer->Caption = "Параметры сервера";

    int idx = 0;
    for (idx = 0; idx < arr.size(); idx++)
        if (arr[idx].name == cbx->Items->operator [](cbx->ItemIndex))
            break;
    if (idx == arr.size())
        throw*(new Exception("Error looking for server"));

    dlgSaveServer->edGuid->Text = arr[idx].guid;
    dlgSaveServer->memDescr->Lines->Text = arr[idx].name;

    if (dlgSaveServer->ShowModal() == mrOk)
    {
        int curIdx = cbx->ItemIndex;
        arr[idx].guid = dlgSaveServer->edGuid->Text;
        arr[idx].name = dlgSaveServer->memDescr->Lines->Text;
        cbx->Items->Strings[cbx->ItemIndex] = dlgSaveServer->memDescr->Lines->Text;
        cbx->ItemIndex = curIdx;
    }
}
void __fastcall TdlgCalcParams::edtMapInitDelayExit(TObject *Sender)
{
    //    
}
//---------------------------------------------------------------------------

void __fastcall TdlgCalcParams::cbMapInitDelayClick(TObject *Sender)
{
    edtMapInitDelay->Enabled = cbMapInitDelay->Checked;
    edtMapInitDelay->Color = cbMapInitDelay->Checked ? clWindow : clBtnFace;
    edtMapInitDelay->Font->Color = cbMapInitDelay->Checked ? clWindowText : clBtnFace;
}
//---------------------------------------------------------------------------

