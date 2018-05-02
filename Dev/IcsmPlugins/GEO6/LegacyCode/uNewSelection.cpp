//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include <ctype.h>
#include <Dialogs.hpp>
#include "FormProvider.h"
#include "TxBroker.h"
#include "uAnalyzer.h"
#include "uMainForm.h"
#include "uNewSelection.h"
#include "uPlanning.h"
#include "uSelection.h"
//---------------------------------------------------------------------
#pragma link "uWhere"
#pragma link "NumericEdit"
#pragma resource "*.dfm"
//char* szFreqQuerycaption = "Вибір частоти";
//char* szFreqQueryPrompt = "Введіть частоту в мегагерцах";
char* szSelectionName = "%.3d";
char* szNewSelectionQuestion = "Об'єкт:\n%s";
//---------------------------------------------------------------------
__fastcall TdlgNewSelection::TdlgNewSelection(TComponent* AOwner)
	: TForm(AOwner), sqlSelect(new TIBSQL(Application)), sqlInsert(new TIBSQL(Application))
{
    lblTxName->Font->Style = lblTxName->Font->Style << fsBold;
    chbMaxRadiusClick(this);

    pcSelectionCriteria->ActivePage = tshCommon;

    edtLon->Text = dmMain->coordToStr(lon, 'X');
    edtLat->Text = dmMain->coordToStr(lat, 'Y');

    panFreqGrid->BevelOuter = bvNone;
    panChBlockGrid->BevelOuter = bvNone;
    panChBlockGrid->Left = panFreqGrid->Left;

    AnsiString path = ExtractFilePath(Application->ExeName);
    String searchIni = path + "Search\\" + "whereTransmitters.ini";
    if (FileExists(searchIni))
        fmWhereCriteria1->loadConfig(searchIni, sqlGetSelection->Database);

    sqlDbSection->Close();
    lbDbSection->Items->Clear();
    dbSectionIds.clear();
    for (sqlDbSection->ExecQuery(); !sqlDbSection->Eof; sqlDbSection->Next())
    {
        lbDbSection->Items->Add(sqlDbSection->Fields[1]->AsString);
        dbSectionIds.push_back(sqlDbSection->Fields[0]->AsInteger);
        if (sqlDbSection->Fields[0]->AsInteger == 1 || sqlDbSection->Fields[0]->AsInteger == 0)
            lbDbSection->Checked[lbDbSection->Items->Count - 1] = true;
    }
    sqlDbSection->Close();
}
//---------------------------------------------------------------------
void __fastcall TdlgNewSelection::edtMaxRadiusExit(TObject *Sender)
{
    if (edtMaxRadius->Enabled && edtMaxRadius->Text.ToInt() <= 0)
        throw *(new Exception("Радіус вибірки повинен бути більше 0, інакше вибірка проводитиметься за максимальною координаційною відстанню"));
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::FormShow(TObject *Sender)
{
    edtMaxRadius->SetFocus();
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::btnCondAddClick(TObject *Sender)
{
    FormProvider.ShowList(1, Handle, 0);
}
//---------------------------------------------------------------------------
void __fastcall TdlgNewSelection::btnRegionsAddClick(TObject *Sender)
{
    FormProvider.ShowList(5, Handle, 0);
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::btnCondRmClick(TObject *Sender)
{
    lbxCond->Items->Delete(lbxCond->ItemIndex);
}
//---------------------------------------------------------------------------


void __fastcall TdlgNewSelection::btnRegionsRmClick(TObject *Sender)
{
    lbxRegions->Items->Delete(lbxRegions->ItemIndex);
}
//---------------------------------------------------------------------------


void __fastcall TdlgNewSelection::btnCondRmAllClick(TObject *Sender)
{
    lbxCond->Items->Clear();
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::btnRegionsRmAllClick(TObject *Sender)
{
    lbxRegions->Items->Clear();
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::btnOkClick(TObject *Sender)
{
    if (Tag == nsPlanning) {
        /*
        очистить старый план
        */
        txAnalyzer.ClearPlan();
        /*
        сформировать сетку частот/каналов
        */
        std::auto_ptr<TIBSQL> sqlGrid(new TIBSQL(Application));
        sqlGrid->Database = dmMain->dbMain;

        double gridStep;
        double fBeg;
        double fEnd;
        AnsiString freqList;
        AnsiString num;
        bool isDiapason;

        TBCTxType txType = txAnalyzer.planningTx.systemcast;
        switch (txType) {
            case ttUNKNOWN:
                throw *(new Exception("Невизначений тип передавача - планування неможливе"));
            case ttFM:
            case ttAM:
                gridStep = rgrGrid->Items->Strings[rgrGrid->ItemIndex].ToDouble() / 1000. ;//  в Мгц
                isDiapason = false;
                fBeg = -1;
                fEnd = -1;
                freqList = edtDiapason->Text;
                while (!freqList.IsEmpty()) {
                    //  в течение каждого прохода определяются два числа - нижняя и верхняя границы
                    //  выгрести мусор - разделители
                    while (!freqList.IsEmpty() && !isdigit(freqList[1]) && freqList[1] != DecimalSeparator && freqList[1] != '-') {
                        freqList.Delete(1, 1);
                    }
                    //  первое число
                    while (!freqList.IsEmpty() && (isdigit(freqList[1]) || freqList[1] == DecimalSeparator)) {
                        num += freqList[1];
                        freqList.Delete(1, 1);
                    }
                    if (!num.IsEmpty())
                        fBeg = num.ToDouble();

                    num = "";
                    //  выгрести мусор - разделители
                    while (!freqList.IsEmpty() && !isdigit(freqList[1]) && freqList[1] != DecimalSeparator) {
                        if (freqList[1] == '-')
                            isDiapason = true;
                        freqList.Delete(1, 1);
                    }

                    if (isDiapason) {
                        //  считываем второе число
                        while (!freqList.IsEmpty() && (isdigit(freqList[1]) || freqList[1] == DecimalSeparator)) {
                            num += freqList[1];
                            freqList.Delete(1, 1);
                        }
                        if (!num.IsEmpty())
                            fEnd = num.ToDouble();
                        num = "";
                    }

                    if (fBeg != -1 || fEnd != -1) {
                        //  если какого-то значения нет, то подставим его
                        if (fBeg == -1)
                        {
                            fBeg = txType == ttFM ? 66.0 : rgBand->ItemIndex == 0 ? .153 : .531;
                        }
                        if (isDiapason) {
                            if (fEnd == -1)
                            {
                                fEnd = txType == ttFM ? 107.9 : rgBand->ItemIndex == 0 ? .279 : 1.61;
                            }
                        } else {
                            fEnd = fBeg;
                        }

                        // заполнить
                        for (double f = fBeg; f <= fEnd; f += gridStep) {
                            txAnalyzer.AddPlanEntry(0, FormatFloat("0.0##", f).c_str(), 0, f, 0, 0);
                        }

                        fBeg = -1;
                        fEnd = -1;
                    }
                }
                break;
            case ttDAB:
                // перебор по таблице блоков
                sqlGrid->SQL->Text = AnsiString("SELECT ID, NAME, CENTREFREQ  FROM BLOCKDAB where ") +
                        "ID between " + AnsiString((int)cbxChFrom->Items->Objects[cbxChFrom->ItemIndex]) +
                        " and " + AnsiString((int)cbxChTo->Items->Objects[cbxChTo->ItemIndex]) +
                        " order by 3";

                sqlGrid->ExecQuery();
                while (!sqlGrid->Eof) {
                    txAnalyzer.AddPlanEntry(0, sqlGrid->FieldByName("NAME")->AsString.c_str(),
                                            sqlGrid->FieldByName("ID")->AsInteger,
                                            sqlGrid->FieldByName("CENTREFREQ")->AsDouble, 0, 0);
                    sqlGrid->Next();
                }
                break;
            case ttTV:
            case ttDVB:
            case ttCTV:
                if (cbxChannelGrid->ItemIndex == -1)
                    throw *(new Exception("Не визначена сітка каналів"));
                sqlGrid->SQL->Text = AnsiString("select ID, NAMECHANNEL, FREQCARRIERVISION from CHANNELS where FREQUENCYGRID_ID = ") +
                    (int)cbxChannelGrid->Items->Objects[cbxChannelGrid->ItemIndex] +
                    " and ("
                        "ID between " + AnsiString((int)cbxChFrom->Items->Objects[cbxChFrom->ItemIndex]) +
                        " and " + AnsiString((int)cbxChTo->Items->Objects[cbxChTo->ItemIndex]) +
                    ")";

                sqlGrid->ExecQuery();
                while (!sqlGrid->Eof) {
                    txAnalyzer.AddPlanEntry(0, sqlGrid->FieldByName("NAMECHANNEL")->AsString.c_str(),
                                            sqlGrid->FieldByName("ID")->AsInteger,
                                            sqlGrid->FieldByName("FREQCARRIERVISION")->AsDouble,
                                            0, 0);
                    sqlGrid->Next();
                }
                break;
        }
    }

    std::auto_ptr<TIBSQL> sqlAux(new TIBSQL(Application));
    sqlAux->Database = dmMain->dbMain;

    int selId = dmMain->getNewId();

    if (Tag == nsExpertise) {
        /*
        имя для новой выборки
        */
        sqlAux->SQL->Text = AnsiString("select MAX(NAMEQUERIES) from SELECTIONS where SELTYPE = 'E' and TRANSMITTERS_ID = ") + TxId;
        sqlAux->ExecQuery();
        AnsiString asName(sqlAux->Fields[0]->AsString);
        int pos = asName.Length();
        while (pos && isdigit(asName[pos]))
            pos--;
        asName.Delete(1,pos);
        asName.sprintf(szSelectionName, asName.Length() ? asName.ToInt()+1 : 1);

        /*
        запись новой выборки
        */
        sqlAux->Close();
        sqlAux->SQL->Text = AnsiString("insert into SELECTIONS (ID, TRANSMITTERS_ID, NAMEQUERIES) values (")
                                    + selId + ", "
                                    + TxId + ", '"
                                    + asName + "')";
        sqlAux->ExecQuery();
    }

    //задание дополнительных параметров
    {
        AdditionalParams parameters;
        parameters.sorting = 0;
        parameters.NewSelection = true;

        std::auto_ptr<TIBQuery> sql(new TIBQuery(this));
          sql->Database = dmMain->dbMain;
          sql->SQL->Text = "update SELECTIONS set RESULT = :RESULT where ID = " + IntToStr(selId);
          sql->Transaction = dmMain->trMain;

        std::auto_ptr<TStream> stream(new TMemoryStream());
        stream->Write(&parameters, sizeof(parameters));

        sql->Params->ParamByName("RESULT")->LoadFromStream(stream.get(), ftBlob);

        sql->ExecSQL();
    }

    sqlSelect->Database = dmMain->dbMain;
    sqlSelect->Close();

    TBCTxType tt = dmMain->GetSc(TxId);
    if (tt == ttAM)
    {
        TCOMILISBCTx tx(txBroker.GetTx(TxId, dmMain->GetObjClsid(tt)), true);
        double carrier = (tt == ttTV || tt == ttDVB) ? tx.video_carrier : tx.sound_carrier_primary;

        String qry = "select tx.ID OUT_TX_ID, sc.ENUMVAL, st.LONGITUDE LON, st.LATITUDE LAT"
                        ",UDF_DISTANCE(tx.LATITUDE, tx.LONGITUDE, :WANT_LAT, :WANT_LON) OUT_DISTANCE"
                        ",TX.CARRIER ,TX.BANDWIDTH ,ATS.ENUMVAL ATS_ENUMVAL ,ar.NUMREGION"
                        ",tx.MAX_COORD_DIST"
                        " from TRANSMITTERS tx"
                        " left outer join SYSTEMCAST sc on (tx.SYSTEMCAST_ID = sc.ID)"
                        " left outer join STAND st on (tx.STAND_ID = st.ID)"
                        " left outer join AREA ar on (st.AREA_ID = ar.ID)"
                        " left outer join ANALOGTELESYSTEM ats on (TX.TYPESYSTEM = ats.ID)"
                        " where tx.ID <> :WANT_ID\n"
                        ;
        if (tt != ttAM)
            qry += (" and (tx.MAX_COORD_DIST is null or tx.MAX_COORD_DIST = 0"
                    " or UDF_DISTANCE(tx.LATITUDE, tx.LONGITUDE, :WANT_LAT, :WANT_LON)/1000 <= tx.MAX_COORD_DIST"
                    //TODO: " or UDF_DISTANCE(tx.LATITUDE, tx.LONGITUDE, :WANT_LAT, :WANT_LON)/1000 <= "......
                    ")");

        if (chbMaxRadius->Checked || tt == ttAM)
            qry += (" and UDF_DISTANCE(tx.LATITUDE, tx.LONGITUDE, :WANT_LAT, :WANT_LON)/1000 <= "+
                    (chbMaxRadius->Checked ? IntToStr(edtMaxRadius->Text.ToInt()) : String("10000")));

        switch (tt)
        {
            case ttAM: qry += (" and (sc.ENUMVAL="+IntToStr(tt)+
                                    " and (tx.CARRIER - :CARRIER) between -0.020 and 0.020"
                                    ")"); break;
            default: break;
        }

        String criteria;
        String sList;
        for (int i = 0; i < lbDbSection->Items->Count && i < dbSectionIds.size(); i++)
            if (lbDbSection->Checked[i])
            {
                if (!sList.IsEmpty())
                    sList += ',';
                sList += IntToStr(dbSectionIds[i]);
            }
        if (!sList.IsEmpty())
            criteria += " and (tx.STATUS in ("+sList+"))";

        sList.SetLength(0);
        for (int i = 0; i < lbxCond->Items->Count; i++) {
            if (sList.Length() > 0)
                sList += ',';
            sList += IntToStr((int)lbxCond->Items->Objects[i]);
        }
        if (!sList.IsEmpty())
            criteria += " and (tx.ACCOUNTCONDITION_IN in ("+sList+") or (tx.ACCOUNTCONDITION_OUT in ("+sList+")))";

        sList.SetLength(0);
        for (int i = 0; i < lbxRegions->Items->Count; i++) {
            if (sList.Length() > 0)
                sList += ',';
            sList += IntToStr((int)lbxRegions->Items->Objects[i]);
        }
        if (!sList.IsEmpty())
            criteria += " and (st.AREA_ID in ("+sList+"))";

        if (!chbSelectBrIfic->Checked)
            criteria += " and ar.NUMREGION not like '%BR'";

        if (chbOnlyRoot->Checked)
            criteria += " and (tx.ORIGINALID = 0 or tx.ORIGINALID is null) ";

        sqlSelect->SQL->Text = qry + criteria;

        #ifdef _DEBUG
      //  ShowMessage("sqlSelect->SQL->Text:\n\n"+sqlSelect->SQL->Text);
        #endif

        sqlSelect->ParamByName("WANT_ID")->AsFloat = TxId;
        sqlSelect->ParamByName("WANT_LON")->AsFloat = tx.longitude;
        sqlSelect->ParamByName("WANT_LAT")->AsFloat = tx.latitude;
        sqlSelect->ParamByName("CARRIER")->AsFloat = carrier;

        //todo: allotments for VHF/UHF
    }
    else
    {
        clearAuxTabes(selId);
        /*
        Заполнить вспомогательные таблицы
        */
        sqlAux->Close();
        sqlAux->SQL->Text = AnsiString("insert into SEL_CONDITION (SELECTION, CONDITION) ") +
                                       "values ("+AnsiString(selId)+", :CONDITION)";
        sqlAux->Prepare();
        for (int i = 0; i < lbxCond->Items->Count; i++) {
            sqlAux->Params->Vars[0]->AsInteger = (int)lbxCond->Items->Objects[i];
            sqlAux->ExecQuery();
        }

        sqlAux->Close();
        sqlAux->SQL->Text = AnsiString("insert into SEL_AREA (SELECTION, AREA) ") +
                                       "values ("+AnsiString(selId)+", :AREA)";
        sqlAux->Prepare();
        for (int i = 0; i < lbxRegions->Items->Count; i++) {
            sqlAux->Params->Vars[0]->AsInteger = (int)lbxRegions->Items->Objects[i];
            sqlAux->ExecQuery();
        }

        /*
        процедура формирования выборки
        */

        sqlSelect->SQL->Text = AnsiString("select * from ")
                            + cbxProcName->Text +
                            " ("
                            "    :ID,"
                            "    :TX_ID,"
                            "    :RADIUS,"
                            "    :LON,"
                            "    :LAT,"
                            "    :USE_CONDITIONS,"
                            "    :USE_AREAS,"
                            "    :USE_ADJANCED,"
                            "    :USE_IMAGE,"
                            "    :ONLY_ROOT,"
                            "    :CARRIER"
                            ") ";

        std:auto_ptr<TIBSQL> outParams(new TIBSQL(this));
        outParams->Database = sqlSelect->Database;
        outParams->SQL->Text = "select pp.rdb$parameter_name from RDB$PROCEDURE_PARAMETERS pp where pp.rdb$procedure_name = '"+cbxProcName->Text+"' and pp.rdb$parameter_type = 1";
        outParams->ExecQuery();
        AnsiString paramList;
        while (!outParams->Eof)
        {
            paramList += (outParams->Fields[0]->AsString.Trim() + ';');
            outParams->Next();
        }
        outParams->Close();

        AnsiString additionCriteria = fmWhereCriteria1->getClause();
        if (!chbSelectBrIfic->Checked)
        {
            if (paramList.Pos("OUT_AREA") == 0)
            {
                Application->MessageBox("Процедура не возвращает наименование региона 'OUT_AREA'.", "Ошибка в параметрах процедуры", MB_ICONEXCLAMATION);
            } else {
                if (!additionCriteria.IsEmpty())
                    additionCriteria += " and ";
                additionCriteria += " (OUT_AREA not like '%BR') ";
            }
        }

        AnsiString statusVals;
        for (int i = 0; i < lbDbSection->Items->Count && i < dbSectionIds.size(); i++)
        {
            if (lbDbSection->Checked[i])
            {
                if (!statusVals.IsEmpty())
                    statusVals += ',';
                statusVals += IntToStr(dbSectionIds[i]);
            }
        }

        if (!statusVals.IsEmpty())
        {
            if (paramList.Pos("OUT_STATUS") == 0)
            {
                Application->MessageBox("Фильтрация по разделу БД (параметр 'OUT_STATUS') невозможна.", "Ошибка в параметрах процедуры", MB_ICONEXCLAMATION);
            } else {
                if (!additionCriteria.IsEmpty())
                    additionCriteria += " and ";
                additionCriteria += " (OUT_STATUS in ("+statusVals+")) ";
            }
        }

        if (!additionCriteria.IsEmpty()) {
            sqlSelect->SQL->Add(" where ");
            sqlSelect->SQL->Add(additionCriteria);
        }

        sqlSelect->ParamByName("ID")->AsInteger = selId;
        sqlSelect->ParamByName("TX_ID")->AsInteger = TxId;
        sqlSelect->ParamByName("RADIUS")->AsInteger = edtMaxRadius->Text.ToInt();
        sqlSelect->ParamByName("LON")->AsFloat = lon;
        sqlSelect->ParamByName("LAT")->AsFloat = lat;
        sqlSelect->ParamByName("USE_CONDITIONS")->AsInteger = lbxCond->Items->Count > 0;
        sqlSelect->ParamByName("USE_AREAS")->AsInteger = lbxRegions->Items->Count > 0;
        sqlSelect->ParamByName("USE_ADJANCED")->AsInteger = chbAdjanced->Checked;
        sqlSelect->ParamByName("USE_IMAGE")->AsInteger = chbImage->Checked;
        sqlSelect->ParamByName("ONLY_ROOT")->AsInteger = chbOnlyRoot->Checked;

    }

    sqlInsert->Database = dmMain->dbMain;
    sqlInsert->SQL->Text = AnsiString("insert into SELECTEDTRANSMITTERS "
                    "(SELECTIONS_ID, TRANSMITTERS_ID, USED_IN_CALC, DISTANCE) "
                    "values (")+AnsiString(selId)+", :TX, 0, :DISTANCE)";
    sqlInsert->Prepare();

    Screen->Cursor = crHourGlass;
    frmMain->StatusBar1->SimplePanel = true;
    try {
        if (Tag == nsExpertise) {
            #ifdef _DEBUG
            TDateTime dt = Now();
            #endif

            frmMain->StatusBar1->SimpleText = "Формування вибірки";
            frmMain->StatusBar1->Update();
            int recNo = 0;
            if(tt == ttAllot)
            {

                TxInfoList fxmList =GetDigAllotSelection();
                int count = 0;
                while (count < fxmList.size()) {


                    sqlInsert->ParamByName("TX")->AsInteger = fxmList[count].id;
                    sqlInsert->ParamByName("DISTANCE")->AsDouble = fxmList[count].dist;
                    sqlInsert->ExecQuery();
                    ++recNo;
                    ++count;
                }
            }
            else {


            sqlSelect->ExecQuery();    

            while (!sqlSelect->Eof) {
                /* TODO :
                perform additional filtering*/
               frmMain->StatusBar1->SimpleText = AnsiString("Запис вибірки: ") + IntToStr(++recNo) + "-й передавач";
                frmMain->StatusBar1->Update();

                sqlInsert->ParamByName("TX")->AsInteger = sqlSelect->FieldByName("OUT_TX_ID")->AsInteger;
                sqlInsert->ParamByName("DISTANCE")->AsDouble = sqlSelect->FieldByName("OUT_DISTANCE")->AsInteger / 1000.0;
                sqlInsert->ExecQuery();
                sqlSelect->Next();
            }
            
            if(tt == ttDAB || tt == ttDVB || tt == ttFxm)
            {
                TxInfoList fxmList =GetFxmSelection();
                int count = 0;
                while (count < fxmList.size()) {


                    sqlInsert->ParamByName("TX")->AsInteger = fxmList[count].id;
                    sqlInsert->ParamByName("DISTANCE")->AsDouble = fxmList[count].dist;
                    sqlInsert->ExecQuery();
                    ++recNo;
                    ++count;
                }
            }
            //TODO: use GetFxmSelection()
            }
            #ifdef _DEBUG
            Application->MessageBox(
                (IntToStr(recNo) + " передавач(iв)\n"+
                FormatDateTime("nn:ss:zzz", Now() - dt)).c_str(), "Формування вибірки", 0
            );
            #endif

            frmMain->StatusBar1->Update();

            SendMessage(frmMain->showExplorer(), WM_LIST_ELEMENT_SELECTED, 32, selId);

        } else if (Tag == nsPlanning) {

            #ifdef _DEBUG
            TDateTime dt = Now();
            #endif
            std::auto_ptr<TIBSQL> sql(new TIBSQL(Application));
            sql->Database = dmMain->dbMain;

            for (int i = 0; i < txAnalyzer.planVector.size(); i++) {

                TCOMILISBCTxList txList(txAnalyzer.planVector[i].txList, true);

                txList.AddTx(txAnalyzer.planningTx);
                sqlSelect->Close();
                sqlSelect->ParamByName("CARRIER")->AsDouble = txAnalyzer.planVector[i].frequency;
                frmMain->StatusBar1->SimpleText = AnsiString("Формування вибірки: ")+txAnalyzer.planVector[i].name.c_str();
                frmMain->StatusBar1->Update();
                sqlSelect->ExecQuery();
                int cr = 1;
                std::vector<int> ids;
                TIBXSQLVAR *idFld = sqlSelect->FieldByName("OUT_TX_ID");
                // we don't know the type of each object here, so first we collect all ids
                for (; !sqlSelect->Eof; sqlSelect->Next()) {
                    ids.push_back(idFld->AsInteger);
                    frmMain->StatusBar1->SimpleText = AnsiString("Формування вибірки: ")+txAnalyzer.planVector[i].name.c_str() + " - " + cr++;
                    frmMain->StatusBar1->Update();
                }
                //TODO: use GetFxmSelection()
                if(tt == ttDAB || tt == ttDVB || tt == ttFxm)
                {
                    TxInfoList fxmList = GetFxmSelection();
                    int count = 0;
                    while (count < fxmList.size()) {

                        ids.push_back(fxmList[count].id);
                  //  ++recNo;
                        ++count;
                    }
                }
                
                std::vector<int>::iterator idi = ids.begin();
                // now we select types from db and create corresponding objects
                while (idi < ids.end())
                {
                    String idList;
                    // ib 6.0 cannot accept more than 1500 values in 'in(...)' clause
                    for (int i = 0; idi < ids.end() && i < 1499; idi++, i++)
                    {
                        if (i > 0) idList += ','; // косвенно
                        idList += IntToStr(*idi);
                    }
                    sql->SQL->Text = "select tx.ID, sc.ENUMVAL "
                                    "from transmitters tx left outer join systemcast sc on (tx.systemcast_id = sc.id) "
                                    "where tx.id in ("+idList+')';
                    for (sql->ExecQuery(); !sql->Eof; sql->Next())
                        txList.AddTx(txBroker.GetTx(sql->Fields[0]->AsInteger,
                                                    dmMain->GetObjClsid(sql->Fields[1]->AsInteger)));
                    sql->Close();
                }
            }
            

            #ifdef _DEBUG
            Application->MessageBox(
                FormatDateTime("nn:ss:zzz", Now() - dt).c_str(), "Forming planning selections", 0
            );
            dt = Now();
            #endif

            txAnalyzer.DoAnalysis();

            #ifdef _DEBUG
            Application->MessageBox(
                FormatDateTime("nn:ss:zzz", Now() - dt).c_str(), "Tx extraction + Analysis", 0
            );
            #endif
            frmMain->StatusBar1->SimpleText = "Вивід форми...";
            frmMain->StatusBar1->Update();

            TfrmPlanning *fp = dynamic_cast<TfrmPlanning*>(FormProvider.ShowPlanning());
            if (fp)
                fp->DrawPlan();
        }
        /* убрать за собой */
        clearAuxTabes(selId);
        sqlInsert->Transaction->CommitRetaining();

    } __finally {
        frmMain->StatusBar1->SimplePanel = false;
        Screen->Cursor = crDefault;
        sqlAux->Transaction->RollbackRetaining();
    }
    Close();
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::AcceptChoice(Messages::TMessage& Message)
{
    int newId;
    switch (Message.WParam) {
        case 1:
            sqlAddState->Close();
            sqlAddState->Transaction->CommitRetaining();
            sqlAddState->Params->Vars[0]->AsInteger = Message.LParam;
            sqlAddState->ExecQuery();
            if (!sqlAddState->Eof) {
                lbxCond->Items->AddObject(AnsiString("[")+
                                        sqlAddState->FieldByName("CODE")->AsString+"] "+
                                        sqlAddState->FieldByName("NAME")->AsString,
                                        (TObject*)Message.LParam);
            }
            sqlAddState->Close();
            break;
        case 5:
            sqlAddRegion->Close();
            sqlAddRegion->Transaction->CommitRetaining();
            sqlAddRegion->Params->Vars[0]->AsInteger = Message.LParam;
            sqlAddRegion->ExecQuery();
            if (!sqlAddRegion->Eof) {
                lbxRegions->Items->AddObject(
                                        sqlAddRegion->FieldByName("NAME")->AsString,
                                        (TObject*)Message.LParam);
            }
            sqlAddRegion->Close();
            break;
        case 10:
            break;
    }
}
void __fastcall TdlgNewSelection::btnCancelClick(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::chbMaxRadiusClick(TObject *Sender)
{
    if (chbMaxRadius->Checked) {
        edtMaxRadius->Enabled = true;
        edtMaxRadius->Font->Color = clWindowText;
    } else {
        edtMaxRadius->Enabled = false;
        edtMaxRadius->Text = 0;
        edtMaxRadius->Font->Color = clBtnFace;
    }
}
//---------------------------------------------------------------------------



void __fastcall TdlgNewSelection::edtLonExit(TObject *Sender)
{
    edtLon->Text = dmMain->coordToStr(lon = dmMain->strToCoord(edtLon->Text), 'X');
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::edtLatExit(TObject *Sender)
{
    edtLat->Text = dmMain->coordToStr(lat = dmMain->strToCoord(edtLat->Text), 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::clearAuxTabes(int selId)
{
    /*
    Очистить вспомогательные таблицы
    */
    std::auto_ptr<TIBSQL> sqlAux(new TIBSQL(Application));
    sqlAux->Database = dmMain->dbMain;
    sqlAux->SQL->Text = "delete from SEL_CONDITION where SELECTION = "+AnsiString(selId);
    sqlAux->ExecQuery();
    sqlAux->Close();
    sqlAux->SQL->Text = "delete from SEL_AREA where SELECTION = "+AnsiString(selId);
    sqlAux->ExecQuery();
}


void __fastcall TdlgNewSelection::cbxChannelGridChange(TObject *Sender)
{
    cbxChFrom->Items->Clear();
    cbxChTo->Items->Clear();
    if (cbxChannelGrid->ItemIndex == -1)
        return;

    std::auto_ptr<TIBSQL> sqlGrid(new TIBSQL(Application));
    sqlGrid->Database = dmMain->dbMain;
    sqlGrid->SQL->Text = "SELECT ID, NAMECHANNEL FROM CHANNELS where FREQUENCYGRID_ID = " + IntToStr((int)cbxChannelGrid->Items->Objects[cbxChannelGrid->ItemIndex]);
    sqlGrid->ExecQuery();
    while (!sqlGrid->Eof) {
        cbxChFrom->Items->AddObject(sqlGrid->Fields[1]->AsString, (TObject*)sqlGrid->Fields[0]->AsInteger);
        cbxChTo->Items->AddObject(sqlGrid->Fields[1]->AsString, (TObject*)sqlGrid->Fields[0]->AsInteger);
        sqlGrid->Next();
    }
    if (cbxChFrom->Items->Count > 0)
        cbxChFrom->ItemIndex = 0;
    if (cbxChTo->Items->Count > 0) {
        int chan69 = cbxChTo->Items->IndexOf("69");
        //  в дециметровой сетке по умолчанию поставить верхнюю границу 69-м каналом
        //  если его в списке нет, значит это просто нек дециметровая сетка :))
        if (chan69 > -1)
            cbxChTo->ItemIndex = chan69;
        else
            cbxChTo->ItemIndex = cbxChTo->Items->Count - 1;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::rgrGridClick(TObject *Sender)
{
    if (txAnalyzer.planningTx.systemcast == ttFM)
    {
        if (rgrGrid->ItemIndex == 0)
            edtDiapason->Text = "66 - 74";
        else
            edtDiapason->Text = FloatToStr(87.5) + " - " + FloatToStr(107.9);
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgNewSelection::rgBandClick(TObject *Sender)
{
    if (txAnalyzer.planningTx.systemcast == ttAM)
        edtDiapason->Text = rgBand->ItemIndex == 0 ?
            FormatFloat("0.000", 0.153) + " - " + FormatFloat("0.000", 0.279)
            :
            FormatFloat("0.000", 0.531) + " - " + FormatFloat("0.000", 1.602);
}
//---------------------------------------------------------------------------

TxInfoList __fastcall TdlgNewSelection::GetFxmSelection()
{
    TxInfoList resultList;
    TBCTxType tt = dmMain->GetSc(TxId);

    AnsiString typeParam = "";

    if (tt == ttFxm)
        typeParam = " in (" + IntToStr(ttDVB) + ", " + IntToStr(ttDAB) + ", " + IntToStr(ttFxm) + ")";
        //typeParam = " <> " + IntToStr(ttAM);
    else
        typeParam = " = " + IntToStr(ttFxm);

    

    TCOMILISBCTx tx(txBroker.GetTx(TxId, dmMain->GetObjClsid(tt)), true);

    ILisBcFxmPtr fxm;
    OleCheck(tx->QueryInterface(IID_ILisBcFxm, (void**)&fxm));

    std::auto_ptr<TIBSQL> selSQL(new TIBSQL(Application));
                                                                               
    AnsiString sql;

    sql = "select tx.ID OUT_TX_ID, st.LONGITUDE LON, st.LATITUDE LAT"
                        ",tx.LATITUDE txLAT, tx.LONGITUDE txLON"
                        ",TX.CARRIER ,TX.BANDWIDTH, TX.SOUND_CARRIER_PRIMARY, TX.MAX_COORD_DIST, sc.ENUMVAL "
                        " from TRANSMITTERS tx"
                        " left outer join SYSTEMCAST sc on (tx.SYSTEMCAST_ID = sc.ID)"
                        " left outer join STAND st on (tx.STAND_ID = st.ID)"
                        " left outer join AREA ar on (st.AREA_ID = ar.ID)"
                        " left outer join ANALOGTELESYSTEM ats on (TX.TYPESYSTEM = ats.ID)"
                        " where tx.ID <> :WANT_ID"
                        " and sc.ENUMVAL" + typeParam;

    String criteria;
    String sList;
    for (int i = 0; i < lbDbSection->Items->Count && i < dbSectionIds.size(); i++)
        if (lbDbSection->Checked[i])
            {
                if (!sList.IsEmpty())
                    sList += ',';
                sList += IntToStr(dbSectionIds[i]);
            }
    if (!sList.IsEmpty())
        criteria += " and (tx.STATUS in ("+sList+"))";

    sList.SetLength(0);
    for (int i = 0; i < lbxCond->Items->Count; i++) {
        if (sList.Length() > 0)
            sList += ',';
        sList += IntToStr((int)lbxCond->Items->Objects[i]);
    }
    if (!sList.IsEmpty())
        criteria += " and (tx.ACCOUNTCONDITION_IN in ("+sList+") or (tx.ACCOUNTCONDITION_OUT in ("+sList+")))";

    sList.SetLength(0);
    for (int i = 0; i < lbxRegions->Items->Count; i++) {
        if (sList.Length() > 0)
            sList += ',';
        sList += IntToStr((int)lbxRegions->Items->Objects[i]);
    }
    if (!sList.IsEmpty())
        criteria += " and (st.AREA_ID in ("+sList+"))";

    if (!chbSelectBrIfic->Checked)
        criteria += " and ar.NUMREGION not like '%BR'";

    if (chbOnlyRoot->Checked)
        criteria += " and (tx.ORIGINALID = 0 or tx.ORIGINALID is null) ";  


    selSQL->SQL->Text = sql + criteria;
    #ifdef _DEBUG
    //ShowMessage("sqlSelect->SQL->Text:\n\n"+selSQL->SQL->Text);
    #endif

    selSQL->Database = dmMain->dbMain;  

    selSQL->ParamByName("WANT_ID")->AsFloat = TxId;

    selSQL->ExecQuery();

    TxInfo temp;
    double maxdist;
    bool maxd = false;
    if(chbMaxRadius->Checked)
    {
        maxdist = StrToFloat(edtMaxRadius->Text);
        maxd = true;
    }
    double dist, lat, lon;
    double want_lat = tx.latitude;
    double want_lon = tx.longitude;
    double freqw;
    if(tt == ttFxm)
        freqw = tx.get_sound_carrier_primary();
    else
        freqw = tx.get_freq_carrier();
    //long idtx = tx.get_id();
    double bandwidthw;
    fxm->get_fxm_bandwidth(&bandwidthw);
    double frequw, bandwidthuw;
    //double max_coord_distw, max_coord_distuw;
    //tx->get_maxCoordDist(&max_coord_distw);
    while (!selSQL->Eof) {
                if(selSQL->FieldByName("ENUMVAL")->AsInteger == -1)
                    frequw = selSQL->FieldByName("SOUND_CARRIER_PRIMARY")->AsFloat;
                else
                    frequw = selSQL->FieldByName("CARRIER")->AsFloat;
                bandwidthuw = selSQL->FieldByName("BANDWIDTH")->AsFloat;
                lat = selSQL->FieldByName("txLAT")->AsFloat;
                lon = selSQL->FieldByName("txLON")->AsFloat;
                dist = txAnalyzer.GetDistance(lon, lat, want_lon, want_lat);
                //max_coord_distuw = selSQL->FieldByName("MAX_COORD_DIST")->AsFloat;
                if(CheckTX(freqw, bandwidthw, frequw, bandwidthuw)) //dist <= max_coord_distw && dist <= max_coord_distuw &&
                    if(!maxd || dist <= maxdist)
                    {
                        temp.id = selSQL->FieldByName("OUT_TX_ID")->AsInteger;
                        temp.dist = dist / 1000.0;

                        resultList.push_back(temp);
                    }
                selSQL->Next();
            }
    return resultList;
}

//---------------------------------------------------------------------------

bool __fastcall TdlgNewSelection::CheckTX(double freq1, double bandwith1, double freq2, double bandwith2)
{
    double ubound1 = freq1 + bandwith1 / 2.;
    double lbound1 = freq1 - bandwith1 / 2.;
    double lbound2 = freq2 - bandwith2 / 2.;
    double ubound2 = freq2 + bandwith2 / 2.;
    if( lbound1 > ubound2)
        return false;
    if( ubound1 < lbound2)
        return false;
    return true;
}

//---------------------------------------------------------------------------

TxInfoList __fastcall TdlgNewSelection::GetDigAllotSelection()
{
    TxInfoList resultList;

    std::auto_ptr<TIBSQL> allotSQL(new TIBSQL(Application));
    allotSQL->SQL->Text = "select * from DIG_ALLOTMENT where ID = " + IntToStr(TxId);
    allotSQL->Database = dmMain->dbMain;
    allotSQL->ExecQuery();

    TBCTxType tt = ttAllot;
    TCOMILISBCTx tx(txBroker.GetTx(TxId, dmMain->GetObjClsid(tt)), true);

    AnsiString typeParam = " <> " + IntToStr(ttAM);
    ILisBcDigAllotPtr allot;
    OleCheck(tx->QueryInterface<ILisBcDigAllot>(&allot));

    std::auto_ptr<TIBSQL> selSQL(new TIBSQL(Application));

    AnsiString sql;

    sql = "select tx.ID OUT_TX_ID, st.LONGITUDE LON, st.LATITUDE LAT"
                        ",tx.LATITUDE txLAT, tx.LONGITUDE txLON"
                        ",TX.CARRIER ,TX.BANDWIDTH, TX.SOUND_CARRIER_PRIMARY, TX.MAX_COORD_DIST, sc.ENUMVAL "
                        " from TRANSMITTERS tx"
                        " left outer join SYSTEMCAST sc on (tx.SYSTEMCAST_ID = sc.ID)"
                        " left outer join STAND st on (tx.STAND_ID = st.ID)"
                        " left outer join AREA ar on (st.AREA_ID = ar.ID)"
                        " left outer join ANALOGTELESYSTEM ats on (TX.TYPESYSTEM = ats.ID)"
                     //   " where tx.ID <> :WANT_ID"
                        " and sc.ENUMVAL" + typeParam;

    String criteria;
    String sList;
    for (int i = 0; i < lbDbSection->Items->Count && i < dbSectionIds.size(); i++)
        if (lbDbSection->Checked[i])
            {
                if (!sList.IsEmpty())
                    sList += ',';
                sList += IntToStr(dbSectionIds[i]);
            }
    if (!sList.IsEmpty())
        criteria += " and (tx.STATUS in ("+sList+"))";

    sList.SetLength(0);
    for (int i = 0; i < lbxCond->Items->Count; i++) {
        if (sList.Length() > 0)
            sList += ',';
        sList += IntToStr((int)lbxCond->Items->Objects[i]);
    }
    if (!sList.IsEmpty())
        criteria += " and (tx.ACCOUNTCONDITION_IN in ("+sList+") or (tx.ACCOUNTCONDITION_OUT in ("+sList+")))";

    sList.SetLength(0);
    for (int i = 0; i < lbxRegions->Items->Count; i++) {
        if (sList.Length() > 0)
            sList += ',';
        sList += IntToStr((int)lbxRegions->Items->Objects[i]);
    }
    if (!sList.IsEmpty())
        criteria += " and (st.AREA_ID in ("+sList+"))";

    if (!chbSelectBrIfic->Checked)
        criteria += " and ar.NUMREGION not like '%BR'";

    if (chbOnlyRoot->Checked)
        criteria += " and (tx.ORIGINALID = 0 or tx.ORIGINALID is null) ";  


    selSQL->SQL->Text = sql + criteria;
    #ifdef _DEBUG
    //ShowMessage("sqlSelect->SQL->Text:\n\n"+selSQL->SQL->Text);
    #endif

    selSQL->Database = dmMain->dbMain;

   // selSQL->ParamByName("WANT_ID")->AsFloat = TxId;

    selSQL->ExecQuery();

    TxInfo temp;
    double maxdist;
    bool maxd = false;
    if(chbMaxRadius->Checked)
    {
        maxdist = StrToFloat(edtMaxRadius->Text);
        maxd = true;
    }
    double dist, lat, lon;
    double want_lat;
    tx->get_latitude(&want_lat);
 //   allot->get_latitude(&want_lat);
    double want_lon;
    tx->get_longitude(&want_lon);
   //  allot->get_longitude(&want_lon);
    double freqw;

    //freqw = allotSQL->FieldByName("FREQ_ASSIGN")->AsFloat;

    allot->get_freq(&freqw);

    double bandwidthw = 8;

    double frequw, bandwidthuw;

    while (!selSQL->Eof) {
                if(selSQL->FieldByName("ENUMVAL")->AsInteger == 7)
                {
                    TCOMILISBCTx txAl(txBroker.GetTx(selSQL->FieldByName("OUT_TX_ID")->AsInteger, dmMain->GetObjClsid(ttAllot)), true);
                    ILisBcDigAllotPtr alTemp;
                    OleCheck(txAl->QueryInterface<ILisBcDigAllot>(&alTemp));
                    alTemp->get_freq(&frequw);
                    txAl->get_latitude(&lat);
                    txAl->get_longitude(&lon);
                    bandwidthuw = 8;
                }
                else {
                    if(selSQL->FieldByName("ENUMVAL")->AsInteger == -1)
                        frequw = selSQL->FieldByName("SOUND_CARRIER_PRIMARY")->AsFloat;
                    else
                        frequw = selSQL->FieldByName("CARRIER")->AsFloat;
                    bandwidthuw = selSQL->FieldByName("BANDWIDTH")->AsFloat;
                    lat = selSQL->FieldByName("txLAT")->AsFloat;
                    lon = selSQL->FieldByName("txLON")->AsFloat;
                }
                dist = txAnalyzer.GetDistance(lon, lat, want_lon, want_lat);
                if(CheckTX(freqw, bandwidthw, frequw, bandwidthuw))
                    if(!maxd || dist <= maxdist)
                    {
                        temp.id = selSQL->FieldByName("OUT_TX_ID")->AsInteger;
                        temp.dist = dist / 1000.0;

                        resultList.push_back(temp);
                    }
                selSQL->Next();
            }
    return resultList;
}


