//---------------------------------------------------------------------------
#include <vcl.h>

#include <memory>
#pragma hdrstop

#include "uBaseListTree.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmBaseListTree *frmBaseListTree;
//---------------------------------------------------------------------------
__fastcall TfrmBaseListTree::TfrmBaseListTree(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmBaseListTree::TfrmBaseListTree(bool MDIChild)
    : TfrmBaseList(MDIChild)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmBaseListTree::TfrmBaseListTree(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseList(Owner, callerWin, elementId)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::fillTree()
{
    trvList->Items->Clear();
    if (!dstList->Transaction->Active)
        dstList->Transaction->StartTransaction();
    fillNode(NULL, 0);
    dstList->Transaction->CommitRetaining();
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::fillNode(TTreeNode* node, int level)
{
    if (sqlTree->SQL->Count > level) {
        TIBSQL* sql = new TIBSQL(this);
        try {
            sql->Database = dstList->Database;
            sql->Transaction = dstList->Transaction;
            if (!sql->Transaction->Active)
                sql->Transaction->StartTransaction();
            sql->SQL->Text = sqlTree->SQL->Strings[level];
            TIBXSQLVAR *p = NULL;
            if (sql->Params->Names.Pos("GRP_ID"))
            try {
                p = sql->Params->ByName("GRP_ID");
            } catch (...) {};
            if (p)
                p->AsInteger = (int)node->Data;
            sql->ExecQuery();
            while (!sql->Eof) {
                TTreeNode *nn = trvList->Items->AddChild(node, sql->Fields[1]->AsString);
                nn->Data = (void*)sql->Fields[0]->AsInteger;
                nn->ImageIndex = 20;
                nn->SelectedIndex = 21;

                if (sqlTree->SQL->Count > level + 1)
                    fillNode(nn, level + 1);

                sql->Next();
            }
            sql->Close();
        } __finally {
            sql->Free();
        }

    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::changeBranch(TTreeNode *newNode)
{
    dstList->Close();
    TIBXSQLVAR *p;
    if (newNode && (p = dstList->ParamByName("GRP_ID"))) {
        p->AsInteger = (int)newNode->Data;
        dstList->Transaction->CommitRetaining();
        dstList->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::dstListNewRecord(TDataSet *DataSet)
{
    //  найти поле, по которому произведён отбор, и подставить в него текущее значение
    int posBeg, posEnd;
    AnsiString curString;
    TIBXSQLVAR *p;
    if (!(p = dstList->ParamByName("GRP_ID")))
        return;
    //  найти строку с параметром
    for (int i = 0; i < ((TIBDataSet*)DataSet)->SelectSQL->Count; i++) {
        curString = ((TIBDataSet*)DataSet)->SelectSQL->Strings[i];
        if ((posEnd = curString.Pos(":GRP_ID")) > 0) {
            //  найти  знак '='
            do
                posEnd--;
            while (posEnd && (curString[posEnd] != '='));
            //  пропустить пробелы
            do
                posEnd--;
            while ((posEnd > 1) && (curString[posEnd] == ' '));
            //  и искать начало лексемы
            posBeg = posEnd;
            while ((posBeg > 1) && (curString[posBeg-1] != ' '))
                posBeg--;

            //  вот и поле
            curString = curString.SubString(posBeg, posEnd - posBeg + 1);
            TField *f = DataSet->FindField(curString);
            if (f)
                f->AsInteger = p->AsInteger;

            break;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::trvListDblClick(TObject *Sender)
{
    if (!dstList->Transaction->Active)
        dstList->Transaction->StartTransaction();
    else
        dstList->Transaction->CommitRetaining();
    TTreeNode *cn = ((TTreeView *)Sender)->Selected;
    changeBranch(cn);
    if (cn) {
        if (cn->Expanded)
            cn->Collapse(false);
        else
            cn->Expand(false);
        AnsiString path = AnsiString('\\') + cn->Text;
        while (cn = cn->Parent)
            path = AnsiString('\\') + cn->Text + path;
        if (!path.IsEmpty())
            stPath->Caption = path;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::Initialize()
{
    fillTree();
    findBranch();
    TfrmBaseList::Initialize();
}

void __fastcall TfrmBaseListTree::trvListKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if ((Key == 13) && Shift == TShiftState())
        trvListDblClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::dstListAfterScroll(TDataSet *DataSet)
{
    stxListQuantity->Caption = AnsiString("Об'єктів в гілці: ") + IntToStr(DataSet->RecordCount) + " ";
}
//---------------------------------------------------------------------------
__fastcall TfrmBaseListTree::findBranch()
{
    TTreeNode *cn = NULL;

    //  найти ветвь, где находится элемент m_elementId
    if (!sqlFindGrp->SQL->Text.IsEmpty()) {
        sqlFindGrp->Close();
        if (!sqlFindGrp->Transaction->Active)
            sqlFindGrp->Transaction->StartTransaction();
        if (!m_elementId)
            m_elementId = last_id[Tag];
        sqlFindGrp->ParamByName("ID")->AsInteger = m_elementId;
        sqlFindGrp->ExecQuery();
        int branchID = 0;
        //  полагаем, что объекты во всех таблицах уникально проиндескированы
        try {
        for (int i = 0; i < sqlTree->SQL->Count; i++)
            if ((branchID = sqlFindGrp->Fields[i]->AsInteger) > 0)
                break;
        } catch (...) {};

        if (!sqlFindGrp->Eof) {
            for (int i = 0; i < trvList->Items->Count; i++)
                if (trvList->Items->Item[i]->Data == (void*)branchID) {
                    cn = trvList->Items->Item[i];
                    break;
                }
        }
        sqlFindGrp->Transaction->CommitRetaining();
    }

    //  сделать её активной
    if (!cn && trvList->Items->Count)
        cn = trvList->Items->Item[0];

    trvList->Selected = cn;
    trvListDblClick(trvList);

}
//---------------------------------------------------------------------------

void __fastcall TfrmBaseListTree::checkWasInBase(std::set<int>& standIds)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
      sql->Database = dmMain->dbMain;;
      sql->GoToFirstRecordOnExecute = true;
      sql->SQL->Text = "SELECT COUNT(*) FROM TRANSMITTERS WHERE ( (WAS_IN_BASE = 1) AND (STAND_ID = :stand) )";
      sql->Prepare();

    std::set<int> newIds;

    int wasInBaseCount = 0;

    for ( std::set<int>::const_iterator id = standIds.begin(); id != standIds.end(); id++ )
    {
        sql->ParamByName("stand")->Value = *id;
        sql->ExecQuery();
        if ( sql->Fields[0]->AsInteger == 0 )
            newIds.insert(*id);
        else
            wasInBaseCount++;
        sql->Close();
    }

    if ( wasInBaseCount > 0 )
    {
        AnsiString str = "Деякі опори (" + IntToStr(wasInBaseCount) + ") містять передавачі, що були в базі.\nТакі опори ( і їх передавачі ) перенесені не будуть.";
        Application->MessageBox(str.c_str(), "Попередження.", MB_ICONWARNING | MB_OK);
    }

    standIds = newIds;
}
//---------------------------------------------------------------------------

int __fastcall TfrmBaseListTree::getAreaId()
{
    if ( TTreeNode *tn = trvList->Selected )
    {
        //  выясним уровень
        int level = 0;
        TTreeNode *n = tn;
        while (n = n->Parent) level++;

        if ( level == 0 )
        {
            Application->MessageBox("Потрібно вибрати регіон.", "Помилка.", MB_ICONERROR | MB_OK);
            return 0;
        }
        else
            return (int)tn->Data;
    }
    else
        return 0 ;
}
//---------------------------------------------------------------------------




