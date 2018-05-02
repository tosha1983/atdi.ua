//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uLisObjectGridForm.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include "TxBroker.h"
#include "uDlgList.h"
#include <dialogs.hpp>
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uLisObjectGrid"
#pragma resource "*.dfm"

TLisObjectGridForm *LisObjectGridForm;
//---------------------------------------------------------------------------
__fastcall TLisObjectGridForm::TLisObjectGridForm(TComponent* Owner)
    : TForm(Owner), objType(0), refreshingRow(-1)
{
    treeQryList.reset(new TStringList());
    grid->SetProxi(this);
}
//---------------------------------------------------------------------------

void TLisObjectGridForm::RunQuery(TIBSQL* sql, String query)
{
    sql->Close();
    if (sql->SQL->Text != query)
        sql->SQL->Text = query;
    refreshingRow = -1;
    if (!sql->Prepared)
        sql->Prepare();
    for (int i = 0; i < sql->Params->Count; i++)
    {
        TIBXSQLVAR *param = sql->Params->Vars[i];
        ParamMap::iterator pi = params.find(param->Name);
        if (pi != params.end() && !(pi->second.IsNull()))
            param->AsVariant = pi->second;
    }
    try {
        sql->ExecQuery();
    } catch (Exception &e) {
        throw *(new Exception(e.Message+"\n\nQuery:\n"+sql->SQL->Text));
    }
}
//---------------------------------------------------------------------------

String TLisObjectGridForm::RunQuery(TLisObjectGrid* sender, String query)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Transaction = selSql->Transaction;
    RunQuery(sql.get(), query);
    return (sql->Eof || sql->Current()->Count == 0) ? String() : sql->Fields[0]->AsString;
}

bool TLisObjectGridForm::RunListQuery(TLisObjectGrid* sender, String query)
{
    RunQuery(selSql, query);
    return selSql->Eof;
}

bool TLisObjectGridForm::Next(TLisObjectGrid* sender)
{
    selSql->Next();
    return selSql->Eof;
}

String TLisObjectGridForm::GetVal(TLisObjectGrid* sender, int row, LisColumnInfo info)
{
    TIBSQL *sql = NULL;
    if (row == sender->ids.size()-1)
    {
        sql = selSql;
    }
    else // refresh row
    if (refreshingRow == row)
    {
        if(!updSql->Open)
            updSql->ExecQuery();
        sql = updSql;
    } else if (sender->ids[row] != 0)
    {
        // compose update query
        String updQry = selSql->SQL->Text;
        int pos = updQry.UpperCase().Pos("WHERE ");
        if (pos > 1 && (updQry[pos-1] == ' ' || updQry[pos-1] == '\n' || updQry[pos-1] == '\t'))
            updQry.Delete(pos, updQry.Length() - pos + 1);
        else
        {
            int pos = updQry.UpperCase().Pos("ORDER ");
            if (pos > 1 && (updQry[pos-1] == ' ' || updQry[pos-1] == '\n' || updQry[pos-1] == '\t'))
                updQry.Delete(pos, updQry.Length() - pos + 1);
        }
        updSql->Close();
        updSql->SQL->Text = updQry + " where "+GetFieldSpecFromAlias(sender, "ID")+" = " + IntToStr(sender->ids[row]);
        refreshingRow = row;
        // ask ourselves recursive 
        return GetVal(sender, row, info); 
    }

    TIBXSQLVAR* fld = NULL;
    if (!sql->Eof)
        fld = sql->FieldByName(info.fldName);

    if (fld)
    {
        if (info.fldName == "ANSWERIS" || info.fldName == "WAS_IN_BASE")
            return fld->AsInteger ? "так" : "";
        else if (info.fldName == "TYPELETTER")
            return fld->AsInteger ? "вх" : "вих";
        else if (info.fldName == "CHANNELFREQBLOCK")
        {
            double freq;
            TBCTxType tt = sql->FieldByName("S_ENUMVAL")->AsInteger;
            switch (tt)
            {
                case ttTV:
                case ttDVB: return sql->FieldByName("NAMECHANNEL")->AsString;
                case -1:
                case ttAM:
                case ttFM: freq = sql->FieldByName("SOUND_CARRIER_PRIMARY")->AsDouble;
                        return (freq > 0.) ?
                        FormatFloat("0.### ", freq < 3. ? freq*1000. : freq) + (freq < 3. ? "кГц" : "МГц")
                        : String();
                case ttDAB: return sql->FieldByName("BD_NAME")->AsString;
                default: return  " - ";
            }
        }
        else if (info.fldName == "ERP_MAX")
        {
            TBCTxType tt = sql->FieldByName("S_ENUMVAL")->AsInteger;
            switch (tt) {
                case ttTV: return FormatFloat("0.##", sql->FieldByName("EPR_VIDEO_MAX")->AsFloat);
                case ttFM:
                case ttDAB:
                case ttDVB: return FormatFloat("0.##", sql->FieldByName("EPR_SOUND_MAX_PRIMARY")->AsFloat);
                default: return "";
            }
        }
        else if (info.fldName == "POWER")
        {
            TBCTxType tt = sql->FieldByName("S_ENUMVAL")->AsInteger;
            switch (tt) {
                case ttTV: return FormatFloat("0.##", sql->FieldByName("POWER_VIDEO")->AsFloat);
                case ttFM:
                case ttDAB:
                case ttDVB: return FormatFloat("0.##", sql->FieldByName("POWER_SOUND_PRIMARY")->AsFloat);
                default: return "";
            }
        }
        else if (info.fldName == "")
            return "";
        else if (info.fldName == "")
            return "";
        else switch (info.propType)
        {
            case ptLat: return dmMain->coordToStr(fld->AsDouble, 'Y');
            case ptLon: return dmMain->coordToStr(fld->AsDouble, 'X');
            case ptdBkWt: return FormatFloat("0.##", fld->AsDouble);
            case ptdBWt: return FormatFloat("0.##", fld->AsDouble + 30.);
            default: return fld->AsString;
        }
    }
    else
        return String();
}

bool TLisObjectGridForm::IsEof(TLisObjectGrid* sender)
{
    return selSql->Eof;
}

int TLisObjectGridForm::GetId(TLisObjectGrid* sender, int row)
{
    return selSql->FieldByName("ID")->AsInteger;
}

String TLisObjectGridForm::GetFieldSpecFromAlias(TLisObjectGrid* sender, String alias)
{
    for (int i = 0; i < selSql->Current()->Count; i++)
        if (strcmp(selSql->Fields[0]->AsXSQLVAR->aliasname, alias.c_str()) == 0)
            return String(selSql->Fields[0]->AsXSQLVAR->relname, selSql->Fields[0]->AsXSQLVAR->relname_length) + '.' +
                String(selSql->Fields[0]->AsXSQLVAR->sqlname, selSql->Fields[0]->AsXSQLVAR->sqlname_length);
    return alias; // куку
}

void TLisObjectGridForm::FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw)
{
}

void TLisObjectGridForm::SortGrid(TLisObjectGrid* sender, int colIdx)
{
}

int TLisObjectGridForm::CreateObject(TLisObjectGrid* sender)
{
    return FormProvider.ShowForm(objType, 0);
}

int TLisObjectGridForm::CopyObject(TLisObjectGrid* sender, int objId)
{

    std::vector<unsigned> idList;
    idList = grid->GetSelectedIdList();
    if (idList.size() > 0)
    {
        AnsiString TableName = dmMain->GetTableName(objType);
        std::map<AnsiString, Variant> id;
        std::map<AnsiString, Variant> params;
        if(objType == otTx)
        {
            params["STATUS"] = cbxDbSection->ItemIndex;
            if(cbxDbSection->ItemIndex == 1)
                params["ADMINISTRATIONID"] = Variant();
            int new_id;
            for(int i = 0; i < idList.size(); ++i)
            {
                id["ID"] = idList[i];
                new_id = dmMain->RecordCopy(TableName, id, params);
                if(new_id < 1)
                {
                    dmMain->trMain->RollbackRetaining();
                    return 0;
                }
            }
        }
        else
        {
            int new_id;
            for(int i = 0; i < idList.size(); ++i)
            {
                id["ID"] = idList[i];
                if(cbxDbSection->ItemIndex == 1)
                    params["ADMINISTRATIONID"] = Variant();
                new_id = dmMain->RecordCopy(TableName, id, params);
                if(new_id < 1)
                {
                    dmMain->trMain->RollbackRetaining();
                    return 0;
                }
            }
        }
        dmMain->trMain->CommitRetaining();
    }
    return 0;
}

bool TLisObjectGridForm::DeleteObject(TLisObjectGrid* sender, int objId)
{
    if (MessageBox(NULL, "Удалить объект?", "Видалення об'єкту", MB_ICONQUESTION | MB_YESNO) != IDYES)
        return false;
        
    if(objType == otTx)
    {
        updSql->Close();
        updSql->SQL->Text = "select WAS_IN_BASE from " + dmMain->GetTableName(objType)+ " where ID = " + IntToStr(objId);
        updSql->ExecQuery();
        if(updSql->FieldByName("WAS_IN_BASE")->AsInteger == 1)
        {
            if (Application->MessageBox("Неможливо видалити даний передавач. Перемістити його в архів?", "Видалення об'єкту передавача", MB_ICONQUESTION | MB_YESNO) == IDYES)
            {
                updSql->Close();
                updSql->SQL->Text = "update " + dmMain->GetTableName(objType) + " set STATUS = -1 where ID = " + IntToStr(objId);
                updSql->ExecQuery();
                updSql->Transaction->CommitRetaining();
            }
        }
        else
        {
            updSql->Close();
            refreshingRow = -1;
            updSql->SQL->Text = "delete from " +dmMain->GetTableName(objType)+ " where ID = " + IntToStr(objId);
            updSql->ExecQuery();
            updSql->Transaction->CommitRetaining();
        }
    }
    else
    {
        updSql->Close();
        refreshingRow = -1;
        updSql->SQL->Text = "delete from " +dmMain->GetTableName(objType)+ " where ID = " + IntToStr(objId);
        updSql->ExecQuery();
        updSql->Transaction->CommitRetaining();
    }
    return true;
}

bool TLisObjectGridForm::EditObject(TLisObjectGrid* sender, int objId)
{
    return FormProvider.ShowForm(objType, objId) == objId;
}

void TLisObjectGridForm::PickObject(TLisObjectGrid* sender, int objId)
{
}

void __fastcall TLisObjectGridForm::Init(int objType, int elementId, int extraTag, int systemcast)
{
    this->objType = objType;
    this->m_elementId = elementId;
    this->syscast = systemcast;
    grid->Enabled = false;
    grid->ClearColumns();
    TMenuItem *mi = NULL;
    switch(objType)
    {
        case otTx:
            panTree->Visible = true;
            tbrTx->Visible = true;
            sqlFindGrp->SQL->Text = "select 0, ar.COUNTRY_ID, st.AREA_ID from TRANSMITTERS tr "
                                    "left outer join STAND st on (tr.STAND_ID = st.ID) "
                                    "left outer join AREA ar on (st.AREA_ID = ar.ID) "
                                    "where tr.ID=:ID";
            treeQryList->Add("");// ALL
            treeQryList->Add("select ID, NAME from COUNTRY order by NAME ");
            treeQryList->Add("select ID, NAME from AREA where COUNTRY_ID = :GRP_ID order by NAME");

            //grid->AddColumn("ID", "", "TX_ID", taLeftJustify, ptString, 70);
            grid->AddColumn("Адм", "", "CN_CODE", taCenter, ptString, 35);
            grid->AddColumn("Рег", "", "NUMREGION", taRightJustify, ptString, 40);
            grid->AddColumn("№", "", "ADMINISTRATIONID", taLeftJustify, ptString, 40);
            grid->AddColumn("Широта", "", "TX_LAT", taLeftJustify, ptLat, 70);
            grid->AddColumn("Долгота", "", "TX_LONG", taLeftJustify, ptLon, 70);
            grid->AddColumn("Опора", "", "NAMESITE", taLeftJustify, ptString, 150);
            grid->AddColumn("Тип", "", "SC_CODE", taCenter, ptString, 40);
            grid->AddColumn("К/Ч/Б", "", "CHANNELFREQBLOCK", taCenter, ptString, 70);
            grid->AddColumn("ВСт", "", "ACIN_CODE", taCenter, ptString, 35);
            grid->AddColumn("ЗСт", "", "ACOUT_CODE", taCenter, ptString, 35);
            if(systemcast != ttAM)
            {
                grid->AddColumn("ЕВП макс", "", "ERP_MAX", taRightJustify, ptdBkWt, 50);
                grid->AddColumn("Нефф макс", "", "HEIGHT_EFF_MAX", taRightJustify, ptdBkWt, 50);
            }
            grid->AddColumn("ЗНЧ", "", "OFFSET_KHZ", taCenter, ptDouble, 40);
            if(systemcast != ttAM)
                grid->AddColumn("Потужність", "", "POWER", taRightJustify, ptString, 50);
            grid->AddColumn("Пол", "", "POLARIZATION", taCenter, ptString, 40);
            grid->AddColumn("Регіон", "", "AREA_NAME", taLeftJustify, ptString, 100);
            grid->AddColumn("Нас. пункт", "", "CITY_NAME", taLeftJustify, ptString, 100);
            grid->AddColumn("Роздiл БД", "", "SECTIONNAME", taLeftJustify, ptString, 70);
            grid->AddColumn("з Бази", "", "WAS_IN_BASE", taCenter, ptString, 50);
            grid->AddColumn("Дата видалення", "", "DATE_DELETED", taLeftJustify, ptString, 70);
            grid->AddColumn("Хто видалив", "", "USERDELNAME", taLeftJustify, ptString, 70);
            break;
        case otDocTemplate:
            grid->AddColumn("ID", "", "ID", taRightJustify, ptInt, 50);
            grid->AddColumn("Код", "", "CODE", taRightJustify, ptInt, 50);
            grid->AddColumn("Назва", "", "NAME", taLeftJustify, ptString, 200);
            grid->AddColumn("Тип", "", "TTYPE", taLeftJustify, ptString, 80);
            break;
        case otDocument:
            grid->AddColumn("ID", "", "ID", taRightJustify, ptString, 50);
            grid->AddColumn("Тип", "", "TYPELETTER", taCenter, ptString, 50);
            grid->AddColumn("Дата", "", "CREATEDATEOUT", taLeftJustify, ptString, 70);
            grid->AddColumn("Номер", "", "NUMOUT", taLeftJustify, ptString, 70);
            grid->AddColumn("Вх. дата", "", "CREATEDATEIN", taLeftJustify, ptString, 70);
            grid->AddColumn("Вх. номер", "", "NUMIN", taLeftJustify, ptString, 70);
            grid->AddColumn("Організація", "", "ORGNAME", taLeftJustify, ptString, 200);
            grid->AddColumn("Документ", "", "TemplName", taLeftJustify, ptString, 120);
            grid->AddColumn("Сист.", "", "CODE", taCenter, ptString, 40);
            grid->AddColumn("Tx №", "", "ADMINISTRATIONID", taRightJustify, ptString, 50);
            grid->AddColumn("Стан", "", "STATE", taLeftJustify, ptString, 100);
            grid->AddColumn("Відп.", "", "ANSWERIS", taCenter, ptString, 70);
            grid->AddColumn("Дата вiдповiдi", "", "ANSWERDATE", taLeftJustify, ptString, 70);
            grid->actNew->Enabled = false;
            grid->actNew->Visible = false;
            grid->actCopy->Enabled = false;
            grid->actCopy->Visible = false;
            mi = new TMenuItem(this);
            grid->pmn->Items->Add(mi);
            mi->Caption = "Передатчик...";
            mi->Tag = otTx;
            mi->OnClick = MenuClick;
            mi->MenuIndex = 0;
            break;
    }
    if (objType == otTx)
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION ";
        dbSectIds.clear();
        cbxDbSection->Items->Clear();
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
        {
            dbSectIds.push_back(sql->Fields[0]->AsInteger);
            cbxDbSection->Items->Add(sql->Fields[1]->AsString);
            if (sql->Fields[0]->AsInteger == extraTag)
                cbxDbSection->ItemIndex = cbxDbSection->Items->Count - 1;
        }
        if (cbxDbSection->ItemIndex == -1 && cbxDbSection->Items->Count > 0)
            cbxDbSection->ItemIndex = 0;
        if (cbxDbSection->ItemIndex > -1)
            params["DB_SECTION_ID"] = dbSectIds[cbxDbSection->ItemIndex];
    }

    if (panTree->Visible)
    {
        fillTree();
        selSql->SQL->Text = grid->GetQuery();
        findBranch();
    } else
        grid->Refresh();
    grid->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::MenuClick(TObject *Sender)
{
    TMenuItem* mi = dynamic_cast<TMenuItem*>(Sender);
    TLisObjectGridForm* ogf = NULL;
    if (mi)
        ogf = dynamic_cast<TLisObjectGridForm*>(mi->Owner);
    if (ogf && ogf->objType == otDocument && mi && mi->Tag == otTx)
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "select TRANSMITTERS_ID from LETTERS where ID = " + IntToStr(ogf->grid->GetCurrentId());
        sql->ExecQuery();
        int txId = sql->Fields[0]->AsInteger;
        if (txId > 0)
            FormProvider.ShowTx(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))));
        else
            throw *(new Exception("Cannot determine Tx ID to display Tx form"));
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::tvListDblClick(TObject *Sender)
{
    if (!selSql->Transaction->Active)
        selSql->Transaction->StartTransaction();
    else
        selSql->Transaction->CommitRetaining();
    TTreeNode *cn = ((TTreeView *)Sender)->Selected;
    if (cn) {
        if (cn->Expanded)
            cn->Collapse(false);
        else
            cn->Expand(false);
        AnsiString path = AnsiString('\\') + cn->Text;
        TTreeNode *tn = cn;
        while (tn = tn->Parent)
            path = AnsiString('\\') + tn->Text + path;
        //if (!path.IsEmpty())
        //    stPath->Caption = path;
    }
    changeBranch(cn);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::tvListKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if ((Key == 13) && Shift == TShiftState())
        tvListDblClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::changeBranch(TTreeNode *newNode)
{
    selSql->Transaction->CommitRetaining();
    int level = 0;
    if (newNode)
    {   //  определим уровень
        TTreeNode* n = newNode;
        while (n = n->Parent)
            level++;
    }
    TStrings* sql = selSql->SQL;
    for (int i = 0; i < sql->Count; i++)
    {
        if (sql->Strings[i].Pos(":GRP_ID") > 0) {
            String newStr("1 = 1 /*no :GRP_ID*/");
            if (objType == otTx)
            {
                //TODO: вынести в отдельные конфигурационные структуры
                switch (level) {
                    case 0: newStr = "1 = 1 /*no :GRP_ID*/"; break;
                    case 1: newStr = "AREA.COUNTRY_ID = :GRP_ID"; break;
                    case 2: newStr = "STAND.AREA_ID = :GRP_ID"; break;
                }
            }
            if (sql->Strings[i] != newStr)
                sql->Strings[i] = newStr;
            break;
        }
    }
    if (newNode)
        params["GRP_ID"] = (int)newNode->Data;
    grid->SetQuery(sql->Text);
    grid->Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::fillTree()
{
    tvList->Items->Clear();
    if (!selSql->Transaction->Active)
        selSql->Transaction->StartTransaction();
    fillNode(NULL, 0);
    selSql->Transaction->CommitRetaining();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::fillNode(TTreeNode* node, int level)
{
    if (treeQryList->Count > level) {
        if (treeQryList->Strings[level].Trim().Length() > 0)
        {
            std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
            sql->Database = selSql->Database;
            sql->Transaction = selSql->Transaction;
            if (!sql->Transaction->Active)
                sql->Transaction->StartTransaction();
            sql->SQL->Text = treeQryList->Strings[level];
            TIBXSQLVAR *p = NULL;
            if (sql->Params->Names.Pos("GRP_ID"))
            try {
                p = sql->Params->ByName("GRP_ID");
            } catch (...) {};
            if (node && p)
                p->AsInteger = (int)node->Data;
            sql->ExecQuery();
            while (!sql->Eof) {
                TTreeNode *nn = tvList->Items->AddChild(node, sql->Fields[1]->AsString);
                nn->Data = (void*)sql->Fields[0]->AsInteger;
                nn->ImageIndex = 20;
                nn->SelectedIndex = 21;
                if (treeQryList->Count > level + 1)
                    fillNode(nn, level + 1);
                sql->Next();
            }
            sql->Close();
        } else {
            TTreeNode *nn = tvList->Items->AddChild(node, "Все");
            nn->ImageIndex = 20;
            nn->SelectedIndex = 21;
            fillNode(nn, level + 1);
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::findBranch()
{
    TTreeNode *cn = NULL;

    //  найти ветвь, где находится элемент m_elementId
    if (!sqlFindGrp->SQL->Text.IsEmpty()) {
        sqlFindGrp->Close();
        if (!sqlFindGrp->Transaction->Active)
            sqlFindGrp->Transaction->StartTransaction();
        //if (!m_elementId)
        //    m_elementId = last_id[Tag];
        sqlFindGrp->ParamByName("ID")->AsInteger = m_elementId;
        sqlFindGrp->ExecQuery();
        int branchID = 0;
        //  полагаем, что объекты во всех таблицах уникально проиндекcированы
        int levl = 0;
        int curgrp = 0;
        for (int i = 0; !sqlFindGrp->Eof && i < sqlFindGrp->Current()->Count; i++)
            if ((curgrp = sqlFindGrp->Fields[i]->AsInteger) > 0)
            {
                branchID = curgrp;
                levl = i;
            }

        if (!sqlFindGrp->Eof) {
            for (int i = 0; i < tvList->Items->Count; i++)
                if (tvList->Items->Item[i]->Data == (void*)branchID) {
                    cn = tvList->Items->Item[i];
                    break;
                }
        }
        sqlFindGrp->Transaction->CommitRetaining();
    }

    if (!cn && tvList->Items->Count)
    {
        cn = tvList->Items->Item[0];
        if (cn->Count > 0)
            cn = cn->operator [](0);
    }

    //  сделать её активной
    tvList->Selected = cn;
    tvListDblClick(tvList);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::actAnalyzeExecute(TObject *Sender)
{
    //TODO: inmplement
    //SendMessage(frmMain->showExplorer(), WM_LIST_ELEMENT_SELECTED, 39, dstListTX_ID->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::actExportExecute(TObject *Sender)
{
    /*TODO: inmplement
    if (dstList->RecordCount == 0) {
        Application->MessageBox("Список пустий.", Application->Title.c_str(), MB_ICONEXCLAMATION);
        return;
    }

    if (!dlgExport)
        dlgExport = new TdlgExport(Application);

    if (list_flags & (1 << ttTV)) {
        dlgExport->rgFormat->ItemIndex = 0;
        dlgExport->rgFormatClick(dlgExport);
    } else if (list_flags & (1 << ttDVB)) {
        dlgExport->rgFormat->ItemIndex = 1;
        dlgExport->rgFormatClick(dlgExport);
    }

    if (dlgExport->ShowModal() == mrOk)
        txExporter.exportTxGrid(dlgExport->rgFormat->ItemIndex, dlgExport->rgList->ItemIndex, dgrList, dlgExport->edtFilename->Text);
    */
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::actRecalcTbtCoordDistsExecute(
      TObject *Sender)
{
    /*TODO: inmplement
    if (Application->MessageBox("Recalculate coordination distancies?", "Shure?", MB_ICONQUESTION | MB_YESNO) == IDYES) {
        TCOMILISBCTxList txList;
        txList.CreateInstance(CLSID_LISBCTxList);

        std::auto_ptr<TIBSQL> sqlIds(new TIBSQL(Application));
        sqlIds->Database = dmMain->dbMain;
        sqlIds->SQL->Text = "select t.ID, s.ENUMVAL from TRANSMITTERS t left outer join systemcast s on (t.systemcast_id = s.id)";

        frmMain->StatusBar1->SimplePanel = true;
        frmMain->StatusBar1->SimpleText = "Формирование списка...";
        TDateTime begTime = Time();

        frmMain->StatusBar1->Update();
        int cnt = 1;
        try {
            sqlIds->ExecQuery();
            while(!sqlIds->Eof) {
                txList.AddTx(txBroker.GetTx(sqlIds->Fields[0]->AsInteger, dmMain->GetObjClsid(sqlIds->Fields[1]->AsInteger)));
                sqlIds->Next();
                cnt++;
                if ((cnt << 7) >> 7 == cnt) {
                    frmMain->StatusBar1->SimpleText = "Формирование списка: " + AnsiString(cnt);
                    Application->Title = frmMain->StatusBar1->SimpleText;
                    frmMain->StatusBar1->Update();
                }
            }
        } __finally {
            frmMain->StatusBar1->SimplePanel = false;
        }

        TDateTime listTime = Time();

        Application->ProcessMessages();

        sqlIds->Transaction->CommitRetaining();
        sqlIds->Close();

        txBroker.EnsureList(txList, frmMain->pb);

        TDateTime extractTime = Time();

        Application->ProcessMessages();

        sqlIds->SQL->Text = "update TRANSMITTERS set MAX_COORD_DIST = :MAX_COORD_DIST where ID = :ID";
        sqlIds->Prepare();

        frmMain->StatusBar1->SimplePanel = true;
        frmMain->StatusBar1->SimpleText = "Расчёт координационных зон...";
        frmMain->pb->Min = 0;
        frmMain->pb->Position = 0;
        frmMain->pb->Visible = true;
        frmMain->pb->Max = txList.Size;
        frmMain->StatusBar1->Update();

        Screen->Cursor = crHourGlass;

        double zone[36];
        int packetCount = 0;

        try {
            for (int i = 0; i < txList.Size; i++) {
                setmem (zone, 36*sizeof(double), '\0');
                double maxCoordDist = 15;
                txAnalyzer.GetCoordinationZone(txList.get_Tx(i), zone);
                for (int i = 0; i < 36; i++) {
                    if (maxCoordDist < zone[i]) {
                        maxCoordDist = zone[i];
                    }
                }
                sqlIds->ParamByName("MAX_COORD_DIST")->AsDouble = maxCoordDist;
                int txId = txList.get_TxId(i);
                sqlIds->ParamByName("ID")->AsInteger = txId;
                sqlIds->ExecQuery();
                if (packetCount++ >= 1000) {
                    packetCount = 0;
                    sqlIds->Transaction->Commit();
                    sqlIds->Transaction->StartTransaction();
                } else {
                    sqlIds->Transaction->CommitRetaining();
                }
                frmMain->pb->StepIt();
                frmMain->StatusBar1->SimpleText = "Расчёт координационных зон: " + AnsiString(i+1) + " (ID " + AnsiString(txId) + ')';
                Application->Title = frmMain->StatusBar1->SimpleText;
                frmMain->StatusBar1->Update();
            }
        } __finally {
            frmMain->StatusBar1->SimplePanel = false;
            frmMain->pb->Visible = false;
            Screen->Cursor = crDefault;
        }

        TDateTime calcTime = Time();
        Application->MessageBoxA((AnsiString(
                                "List forming time: ") + FormatDateTime("hh:nn:ss.zzz", listTime - begTime) +
                                "\nExtract time: " + FormatDateTime("hh:nn:ss.zzz", extractTime - listTime) +
                                "\nCalculation/update time: " + FormatDateTime("hh:nn:ss.zzz", calcTime - extractTime) +
                                "\nTotal time: " + FormatDateTime("hh:nn:ss.zzz", calcTime - begTime)
                                ).c_str(),
                                "Completed", MB_ICONINFORMATION
                                );
    }
    */
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::actMoveToSectionExecute(
      TObject *Sender)
{
    if (!dlgList)
        dlgList = new TdlgList(Application);
    TdlgList* dlg = dlgList;
    dlg->lb->Items->Clear();
    dlg->ids.clear();
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION";
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        dlg->lb->Items->Add(sql->Fields[1]->AsString);
        dlg->ids.push_back(sql->Fields[0]->AsInteger);
    }
    sql->Close();

    dlg->Caption = "Секция БД";
    dlg->lblComment->Caption = "Выберите секцию БД, в которую нужно переместить выбранные объекты";
    if (dlg->ShowModal() == mrOk && dlg->lb->ItemIndex > -1)
    {
        //TODO:!!!!!TBookmark bm = dstList->GetBookmark();
        AnsiString idListStr;
        std::vector<unsigned> idList;
        idList = grid->GetSelectedIdList();
        for(unsigned ind = 0; ind < idList.size(); ++ind)
            idListStr = idListStr + IntToStr(idList[ind]) + ',';
        if (idList.size() > 0)
        {
            if (idListStr[idListStr.Length()] == ',')
                idListStr.SetLength(idListStr.Length() - 1);
            sql->SQL->Text = AnsiString("update ") +  "TRANSMITTERS" + " set STATUS = "
                            + IntToStr(dlg->ids[dlg->lb->ItemIndex]) + " where ID in ("
                            + idListStr + ')';
            sql->ExecQuery();
            sql->Transaction->CommitRetaining();
        }
        grid->Refresh();
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::actCopyToSectionExecute(
      TObject *Sender)
{
    if (!dlgList)
        dlgList = new TdlgList(Application);
    TdlgList* dlg = dlgList;
    dlg->lb->Items->Assign(cbxDbSection->Items);
    dlg->ids = dbSectIds;
    dlg->Caption = "Секция БД";
    dlg->lblComment->Caption = "Выберите секцию БД, в которую нужно скопировать выбранные объекты";
    
    if (dlg->ShowModal() == mrOk && dlg->lb->ItemIndex > -1)
    {
        //TODO:!!!!!
        std::vector<unsigned> idList;
        idList = grid->GetSelectedIdList();
        if (idList.size() > 0)
        {

            AnsiString TableName = "TRANSMITTERS";
            std::map<AnsiString, Variant> id;
            std::map<AnsiString, Variant> params;
            params["STATUS"] = dlg->ids[dlg->lb->ItemIndex];
            if(dlg->ids[dlg->lb->ItemIndex] == 1)
                params["ADMINISTRATIONID"] = Variant();
            int new_id;
            for(int i = 0; i < idList.size(); ++i)
            {
                id["ID"] = idList[i];
                new_id = dmMain->RecordCopy(TableName, id, params);
                if(new_id < 1)
                {
                    dmMain->trMain->RollbackRetaining();
                    return;
                }
            }
            dmMain->trMain->CommitRetaining();
        }
    }

}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::actExpGt1Gs1Execute(TObject *Sender)
{
    //TODO: inmplement
    //TfrmRrc06Export::ExportRrc006FromGrid(emGs1Gt1, dgrList);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::cbxDbSectionChange(TObject *Sender)
{
    if (cbxDbSection->ItemIndex > -1)
    {
        params["DB_SECTION_ID"] = dbSectIds[cbxDbSection->ItemIndex];
        grid->Refresh();
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::tbtSearchClick(TObject *Sender)
{
    colN = grid->dg->Col;
    LisColumnInfo temp = grid->columnsInfo.operator [](colN);
    AnsiString text,param;
    param = temp.title;
    colNameParam = temp.fldName;
    AnsiString temp1, temp2, newQuery, qText;
    int pos;
    queryParam = text;
    TIBXSQLVAR* fld = NULL;
    fld = selSql->FieldByName(colNameParam);
    temp1 = fld->AsXSQLVAR->relname;

    AnsiString SearchParam = "(" + param + "='') ";
    lblSearchParam->Caption = SearchParam;

    tbtSearchCancel->Enabled = true;
    panSearch->Visible = false;
    edtIncSearch->Text = "";
    panSearch->Visible = true;
    edtIncSearch->SetFocus();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::tbtSearchCancelClick(TObject *Sender)
{
    if(oldQuery != "")
    {
        selSql->SQL->Text = oldQuery;
        grid->SetQuery(oldQuery);
        oldQuery = "";
        queryParam = "";
        grid->Refresh();
    }
    tbtSearchCancel->Enabled = false;
    panSearch->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::edtIncSearchExit(TObject *Sender)
{
   // panSearch->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGridForm::edtIncSearchKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if (Key == 13)    {
    colN = grid->dg->Col;
    LisColumnInfo temp = grid->columnsInfo.operator [](colN);
    AnsiString text,param;
    param = temp.title;
    colNameParam = temp.fldName;
    int pos;
    AnsiString sCast;
    AnsiString temp1, temp2, newQuery, qText;
    if (oldQuery == "")
        qText = selSql->SQL->Text;
    else
        qText = oldQuery;
    if( objType == otTx)
    {
    if(colNameParam == "CHANNELFREQBLOCK")
    {
        switch (syscast) {
            case ttTV:
            case ttDVB: colNameParam = "NAMECHANNEL"; break;
            case -1:
            case ttAM:
            case ttFM: colNameParam = "SOUND_CARRIER_PRIMARY"; break;
            case ttDAB:colNameParam = "BD_NAME"; break;
        }
    }
    else if(colNameParam == "ERP_MAX")
    {
        switch (syscast) {
            case ttTV: colNameParam = "EPR_VIDEO_MAX"; break;
            case ttFM:
            case ttDAB:
            case ttDVB: colNameParam = "EPR_SOUND_MAX_PRIMARY"; break;
        }
    }
    else if(colNameParam == "POWER")
    {
        switch (syscast) {
            case ttTV: colNameParam = "POWER_VIDEO"; break;
            case ttFM:
            case ttDAB:
            case ttDVB: colNameParam = "POWER_SOUND_PRIMARY"; break;
        }
    }
    }
    queryParam = text;
    TIBXSQLVAR* fld = NULL;
    fld = selSql->FieldByName(colNameParam);
    temp1 = fld->AsXSQLVAR->relname;

    AnsiString SearchParam = "(" + param + "='*" + edtIncSearch->Text + "*') ";
    lblSearchParam->Caption = SearchParam;

    temp2 = fld->AsXSQLVAR->sqlname;    
    if( objType == otTx)
    {
        if( colNameParam == "ACIN_CODE" )
            temp1 = "ACIN";
        else if( colNameParam == "ACOUT_CODE" )
            temp1 = "ACOUT";
    }
    if( colNameParam == "OFFSET_KHZ")
        queryParam = " and VIDEO_OFFSET_HERZ/1000.0 = " + edtIncSearch->Text + " ";
    else
        queryParam = " and " + temp1 + "." + temp2 + " like '%" + edtIncSearch->Text + "%' ";

    pos = qText.AnsiPos("order by");
    if(pos != 0)
        newQuery = qText.SubString(0, pos - 1) + queryParam + qText.SubString(pos, qText.Length());
    else
        newQuery = qText + queryParam;
    if (oldQuery == "")
        oldQuery = selSql->SQL->Text;
    selSql->SQL->Text = newQuery;
    grid->SetQuery(newQuery);
    grid->Refresh();
    tbtSearchCancel->Enabled = true;
    edtIncSearch->SetFocus();
    }
}
//---------------------------------------------------------------------------

