//---------------------------------------------------------------------------
#include <DBGrids.hpp>
#include <memory>
#include <vcl.h>
#pragma hdrstop

#include "uAnalyzer.h"
#include "uExplorer.h"
#include "uLayoutMngr.h"
#include "uListTransmitters.h"
#include "uMainForm.h"
#include "uNewSelection.h"
#include "uPlanning.h"
#include "uSelection.h"

#include "FormProvider.h"
#include <LISBCTxServer_TLB.h>
#include "TxBroker.h"
#include "tempvalues.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmExplorer *frmExplorer;
enum NodeType { ntNone = 0, ntTx = 1, ntSelectionList, ntSelection, ntTestPointList, ntTestPoint, ntPlan, ntIcstSel };
//  string literals
char* szNoItems =
        "Немає вибраних об'єктів аналізу.\n"
        "Для вибору об'єкта перетягнiть його мишкою з будь-якого вікна списку передавачів";
//        "або перетягнiть його мишкою з будь-якого вікна списку передавачів";
char* szDelObjQuestion = "Видалити об'єкт \n'%s'\nзі списку аналізу?";
char* szTxBranchText = "";
char* szPlanBranch = "Планування";
char* szSelectionsBranch = "Експертиза";
char* szTestPointBranch = "Контрольні точки";
char* szEObjectExists = "Цей об'єкт вже иснує в списку";
char* szDeleteSelectionQuestion = "Видалити вибірку '%s'?";
char* szCantFindTransmitter = "В базі даних немає передавача, для якого створена вибірка (!00!)";
char* szSelectionInterCaption = ": експертиза ";
char* szTestPointCaption = ": контрольна точка ";
char* szRefToIcsTel = "(ICS Tel)";

struct NodeData
{
    NodeType nodeType;
    int id;
    bool alien;
    NodeData(NodeType nt, int iid, bool a=false): nodeType(nt), id(iid), alien(a) {};
};

NodeData dummyData(ntNone, 0);

inline NodeData* GetNodeData(TTreeNode* tn)
    { return (tn && tn->Data) ? (NodeData*)tn->Data : &dummyData; }
inline bool IsObjectNode(TTreeNode* tn, int id)
    { return (GetNodeData(tn)->nodeType == ntTx) && (GetNodeData(tn)->id == id); }
//---------------------------------------------------------------------------

__fastcall TfrmExplorer::TfrmExplorer(TComponent* Owner)
    : TForm(Owner)
{
    UserId = 0;
    txtNoItems->Color = trvExplorer->Color;
    txtNoItems->Caption = szNoItems;
    trvExplorer->Font->Style = trvExplorer->Font->Style << fsBold;

    ResizeStarted = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actDeleteObjectExecute(TObject *Sender)
{
    for (int i = 0; i < trvExplorer->SelectionCount; i++)
    {
        if (TTreeNode *tn = trvExplorer->Selections[i])
        {
            switch (GetNodeData(tn)->nodeType)
            {
                case ntTx: // object
                    if (Application->MessageBox(AnsiString().sprintf(szDelObjQuestion,tn->Text).c_str(), Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
                    {
                        sqlDeleteObject->Close();
                        sqlDeleteObject->ParamByName("USERID")->AsInteger = UserId;
                        sqlDeleteObject->ParamByName("OBJECTID")->AsInteger = GetNodeData(tn)->id;
                        try
                        {
                            sqlDeleteObject->ExecQuery();
                            trvExplorer->Items->Delete(tn);
                            sqlDeleteObject->Transaction->CommitRetaining();
                        }
                        catch (Exception& e)
                        {
                            sqlDeleteObject->Transaction->RollbackRetaining();
                            throw *(new Exception(e.Message));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    txtNoItems->Visible = (trvExplorer->Items->Count == 0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actDeleteAllExecute(TObject *Sender)
{
    if (Application->MessageBox("Видалити всі об'єкти \nзі списку аналізу?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) == IDYES)
    {
        while( TTreeNode *tn = trvExplorer->Items->GetFirstNode() )
        {
            sqlDeleteObject->Close();
            sqlDeleteObject->ParamByName("USERID")->AsInteger = UserId;
            sqlDeleteObject->ParamByName("OBJECTID")->AsInteger = GetNodeData(tn)->id;
            try
            {
                sqlDeleteObject->ExecQuery();
                trvExplorer->Items->Delete(tn);
                sqlDeleteObject->Transaction->CommitRetaining();
            }
            catch (Exception& e)
            {
                sqlDeleteObject->Transaction->RollbackRetaining();
                throw *(new Exception(e.Message));
            }
        }
    }
    txtNoItems->Visible = (trvExplorer->Items->Count == 0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actNewObjectExecute(TObject *Sender)
{
    FormProvider.ShowTxList(Handle, 0, 0 /* все */);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::AddTx(int TxId)
{
    for (TTreeNode *tn = trvExplorer->Items->GetFirstNode(); tn; tn = tn->getNextSibling())
        if (IsObjectNode(tn, TxId))
            throw *(new Exception(szEObjectExists));

    int txCount = 0;
    /*
    {
        TTreeNode *tn = trvExplorer->Items->GetFirstNode();
        if ( tn )
        {
            txCount = 1;
            while( tn = tn->getNextSibling() )
                txCount++;
        }
    }
    */

    try
    {
        selMaxObjNo->ParamByName("USERID")->AsInteger = UserId;
        selMaxObjNo->ExecQuery();
        txCount = selMaxObjNo->Fields[0]->AsInteger;
        selMaxObjNo->Close();

        sqlAddObject->Close();
        sqlAddObject->ParamByName("USERID")->AsInteger = UserId;
        sqlAddObject->ParamByName("OBJECTID")->AsInteger = TxId;
        sqlAddObject->ParamByName("OBJECTNO")->AsInteger = txCount + 1;

        sqlAddObject->ExecQuery();
        AddTxToTree(TxId);
        sqlAddObject->Transaction->CommitRetaining();
    }
    catch (Exception& e)
    {
        sqlAddObject->Transaction->RollbackRetaining();
        throw *(new Exception(e.Message));
    }
    catch (Exception* e)
    {
        sqlAddObject->Transaction->RollbackRetaining();
        throw *(new Exception(e->Message));
    }

    this->Show();
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::AddTxToTree(int txId)
{
    if (!frmExplorer->Visible)
        frmMain->actExplorerExecute(this);

        AnsiString BranchText = GetTxBranchText(txId);

        TTreeNode* tn = trvExplorer->Items->AddObject(NULL, BranchText, new NodeData(ntTx, txId));
        tn->HasChildren = true;
        //tn->ImageIndex = ;
        //tn->SelectedIndex = ;
        TTreeNode *tchn;
        tchn = trvExplorer->Items->AddChildObject(tn, szPlanBranch, new NodeData(ntPlan, txId));
        tchn->HasChildren = true;
        tchn = trvExplorer->Items->AddChildObject(tn, szSelectionsBranch, new NodeData(ntSelectionList, txId));
        tchn->HasChildren = true;
        tchn = trvExplorer->Items->AddChildObject(tn, szTestPointBranch, new NodeData(ntTestPointList, txId));
        tchn->HasChildren = true;

        txtNoItems->Visible = false;
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::FormClose(TObject *Sender, TCloseAction &Action)
{
    try {
        LayoutManager.saveLayout(this);
    } catch (...) {
    }

    if ( (FormStyle == fsMDIChild) || ( dynamic_cast<TPanel*>(dynamic_cast<TForm*>(Sender)->HostDockSite) != NULL ) )
    {
        Action = caFree;
        frmExplorer = NULL;
    }
    frmMain->actExplorer->Checked = false;

    //прячем панель ( к которой прилеплено окно )
    if (dynamic_cast<TPanel*>(HostDockSite) != NULL)
    {
        if ( HostDockSite->Name == "pnlLeftDockPanel" )
        {
            frmMain->pnlLeftDockPanel->Width = 0;
            frmMain->leftSplitter->Hide();
            //form->ManualDock(frmMain->LeftDockPanel, frmMain->LeftDockPanel, alLeft);
        }
        else
        {
            frmMain->pnlRightDockPanel->Width = 0;
            frmMain->rightSplitter->Hide();
            //form->ManualDock(frmMain->RightDockPanel, frmMain->RightDockPanel, alLeft);
        }
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::acceptSelection(Messages::TMessage& Message)
{
    switch (Message.WParam) {
        case 39:
                AddTx(Message.LParam);
            break;
        case 32: // selection
                //  find transmitter id;
                sqlGetTxId->Close();
                sqlGetTxId->Transaction->CommitRetaining();
                sqlGetTxId->Params->Vars[0]->AsInteger = Message.LParam; // selection Id
                sqlGetTxId->ExecQuery();
                int TxId = 0;
                if (!sqlGetTxId->Eof)
                    TxId = sqlGetTxId->Fields[0]->AsInteger;
                else
                    throw *(new Exception(szCantFindTransmitter));
                sqlGetTxId->Transaction->CommitRetaining();
                sqlGetTxId->Close();

                TTreeNode *tn = trvExplorer->Items->GetFirstNode();
                while (tn && !IsObjectNode(tn, TxId))
                    tn = tn->getNextSibling();

                if (IsObjectNode(tn, TxId))
                {
                    //  найдём ветвь с выборками
                    TTreeNode *ts = tn->getFirstChild();
                    while (ts)
                        if (GetNodeData(ts)->nodeType == ntSelectionList)
                            break;
                        else
                            ts = tn->GetNextChild(ts);

                    if (GetNodeData(ts)->nodeType != ntSelectionList) {
                        ts = trvExplorer->Items->AddChildObject(tn,
                                            szSelectionsBranch,
                                            new NodeData(ntSelectionList, TxId)
                                            );
                        //ts->ImageIndex =
                        //ts->SelectedIndex =
                    }
                    //  сымитируем ситуацию открытия неоткрытой ветви
                    //  (выборки подтягиваютсчя только когда пользовательтыньцает на соотв. ветви)
                    ts->DeleteChildren();
                    ts->HasChildren = true;
                    ts->Expand(true);
                    /*
                    ts = trvExplorer->Items->AddChildObject(ts,
                                            sqlNewSelection->FieldByName("OUT_NAME")->AsString + " ["+sqlNewSelection->FieldByName("OUT_DATE")->AsString + "]",
                                            (void*)sqlNewSelection->FieldByName("OUT_ID")->AsInteger
                        );
                    //ts->ImageIndex =
                    //ts->SelectedIndex =
                    */
                    //  и теперь найдём ветвь
                    ts = ts->getFirstChild();
                    while (ts) {
                        if (GetNodeData(ts)->id == Message.LParam) {
                            trvExplorer->Selected = ts;
                            trvExplorerDblClick(this);
                            break;
                        }
                        ts = ts->getNextSibling();
                    }
                    //   ну а если нет - то хер его знает

                } else {
                    //  передатчика нет в дереве. добавить его
                    Messages::TMessage newMessage;
                    newMessage.Msg = WM_LIST_ELEMENT_SELECTED;
                    newMessage.WParam = 39;
                    newMessage.LParam = TxId;
                    acceptSelection(newMessage);
                    //  и заново ;)
                    newMessage.WParam = Message.WParam;
                    newMessage.LParam = Message.LParam;
                    acceptSelection(newMessage);
                }

                if (trvExplorer->Items->Count > 0)
                    tn = trvExplorer->Items->Item[0];
            break;
    }
}
void __fastcall TfrmExplorer::find_branch(TTreeNode* tn, TStringList* sl)
{
    TTreeNode* ts;
    if (sl->Count) {
        if (tn)
            ts = tn->getFirstChild();
        else
            ts = trvExplorer->Items->GetFirstNode();
        while (ts) {
            if (ts->Text == sl->Strings[0]) {
                trvExplorer->Selected = ts;
                ts->Expanded = true;
                sl->Delete(0);
                find_branch(ts, sl);
                break;
            } else {
                ts = ts->getNextSibling();
            }
        }
    }
}
void __fastcall TfrmExplorer::actRefreshExecute(TObject *Sender)
{
    std::auto_ptr<TStringList> sl(new TStringList);
    TTreeNode* tn = trvExplorer->Selected;
    //bool expanded = tn ? tn->Expanded : false;
    while(tn) sl->Insert(0, tn->Text), tn = tn->Parent;
    Refresh();
    find_branch(NULL, sl.get());
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::Refresh()
{
    trvExplorer->Items->Clear();
    txtNoItems->Visible = false;
    UserId = dmMain->UserId;

    //check if icst selection present and add it's branch in this case
    sqlSelIcst->Close();
    sqlSelIcst->ParamByName("USERID")->AsInteger = UserId;
    sqlSelIcst->ExecQuery();
    if (!sqlSelIcst->Eof)
       trvExplorer->Items->AddObject(NULL
                                    , String(szRefToIcsTel) + ' ' + GetTxBranchText(sqlSelIcst->FieldByName("TX_ID")->AsInteger)
                                    , new NodeData(ntIcstSel, sqlSelIcst->FieldByName("ID")->AsInteger));
    sqlSelIcst->Close();

    sqlSelectIDs->Close();
    sqlSelectIDs->Params->Vars[0]->AsInteger = UserId;
    sqlSelectIDs->ExecQuery();//USEROBJECTS

    //  загрузим передатчики списком
    TCOMILISBCTxList objectList;
    objectList.CreateInstance(CLSID_LISBCTxList);
    while (!sqlSelectIDs->Eof) {
        objectList.AddTx(txBroker.GetTx(sqlSelectIDs->Fields[0]->AsInteger,
                                        dmMain->GetObjClsid(sqlSelectIDs->Fields[1]->AsInteger)));
        sqlSelectIDs->Next();
    }

    //  из списка - в ветки
    if (objectList.Size > 0) {
        //ShowMessage("загрузим передатчики списком");
        txBroker.EnsureList(objectList, NULL);
        //ShowMessage("список загружен");
        for (int i = 0; i < objectList.Size; i++)
            try {
                AddTxToTree(objectList.get_TxId(i));
            } catch (...) {}
    } else {
        txtNoItems->Visible = true;
    }
    //ShowMessage("готово");
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::trvExplorerDblClick(TObject *Sender)
{
    if (TTreeNode *tn = trvExplorer->Selected)
    {
        int id = GetNodeData(tn)->id;
        switch (GetNodeData(tn)->nodeType) {
            case ntTx: // object
                FormProvider.ShowTx(txBroker.GetTx(id, dmMain->GetObjClsid(dmMain->GetSc(id))));
                break;
            case ntPlan:
                actPlanExecute(this);
                break;
            case ntSelectionList:
                actNewSelectionExecute(this);
                break;
            case ntSelection:
                {
                    TempCursor tc(crHourGlass);
                    TfrmSelection* fs = FormProvider.ShowSelection(id);
                    fs->Caption = tn->Parent->Parent->Text + szSelectionInterCaption + tn->Text;
                }
                break;
            case ntTestPoint:
                {
                    TempCursor tc(crHourGlass);
                    TfrmSelection* fs = FormProvider.ShowSelection(id);
                    fs->Caption = tn->Parent->Parent->Text + szTestPointCaption + tn->Text;
                }
                break;
            case ntIcstSel:
                {
                    TempCursor tc(crHourGlass);
                    TfrmSelection* fs = FormProvider.ShowSelection(id);
                    fs->Caption = tn->Text;
                }
                break;
            default:
                break;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::trvExplorerKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if (Key == 13) {
        if (Shift.Empty()) {
            trvExplorerDblClick(Sender);
            Key = 0;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::trvExplorerDragOver(TObject *Sender,
      TObject *Source, int X, int Y, TDragState State, bool &Accept)
{
    TDBGrid* grd;
    if (
        (grd = dynamic_cast<TDBGrid*>(Source))
        && (grd->Name == "dgrList" && (dynamic_cast<TfrmListTransmitters*>(grd->Owner))
            || grd->Name == "grdTx")
    )
        Accept = true;
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::trvExplorerDragDrop(TObject *Sender,
      TObject *Source, int X, int Y)
{
    TDBGrid* grd;
    if (
        (grd = dynamic_cast<TDBGrid*>(Source))
        && (grd->Name == "dgrList" && (dynamic_cast<TfrmListTransmitters*>(grd->Owner))
      ||
        grd->Name == "grdTx"
      ||
        grd->Name == "dbgRsults")
    ) {
        //  grd - сетка с передатчиками. нулевое поле соотв. датасета содержит Ид передатчика.
        int txId = grd->DataSource->DataSet->FieldByName("TX_ID")->AsInteger;
        AddTx(txId);
        TTreeNode *tn = trvExplorer->Items->GetFirstNode();
        while (tn)
        {
            if (IsObjectNode(tn, txId))
            {
                trvExplorer->Selected = tn;
                actNewSelectionExecute(this);
                break;
            }
            tn = tn->getNextSibling();
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::trvExplorerExpanding(TObject *Sender,
      TTreeNode *Node, bool &AllowExpansion)
{
    //  since we force every Node's HasChildren property
    //  we have to really check it here
    if (Node->getFirstChild() == NULL) {
        Node->HasChildren = false;
        if (Node->Text == szSelectionsBranch)
            GetSelectionBranches(Node);
        if (Node->Text == szTestPointBranch)
            GetTestPointBranches(Node);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::GetSelectionBranches(TTreeNode* root)
{
    if (GetNodeData(root)->nodeType != ntSelectionList)
        // sometging wrong
        return;
    sqlSelSelections->Close();
    int txId = GetNodeData(root)->id;
    sqlSelSelections->Params->Vars[0]->AsInteger = txId;
    sqlSelSelections->ExecQuery();
    while (!sqlSelSelections->Eof) {
        bool alien = sqlSelSelections->FieldByName("USERID")->AsInteger != UserId;
        String text = sqlSelSelections->FieldByName("NAMEQUERIES")->AsString + " ["+sqlSelSelections->FieldByName("CREATEDATE")->AsString + "]" +
                        + (alien ? " (чужая)" : "")
                        ;
        if (sqlSelSelections->FieldByName("SELTYPE")->AsString == "I") text = text + " " + szRefToIcsTel;
        trvExplorer->Items->AddChildObject(root,
                                    text,
                                    new NodeData((sqlSelSelections->FieldByName("SELTYPE")->AsString == "I") ? ntIcstSel : ntSelection
                                                   ,sqlSelSelections->FieldByName("ID")->AsInteger
                                                   ,alien
                                                )
                                    );
        sqlSelSelections->Next();
    }
    sqlSelSelections->Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::GetTestPointBranches(TTreeNode* root)
{
    if (GetNodeData(root)->nodeType != ntTestPointList)
        // sometging wrong
        return;
    sqlTPSelections->Close();
    int txId = GetNodeData(root)->id;
    sqlTPSelections->Params->Vars[0]->AsInteger = txId;
    sqlTPSelections->ExecQuery();
    while (!sqlTPSelections->Eof) {
        trvExplorer->Items->AddChildObject(root,
                                    sqlTPSelections->FieldByName("NAME")->AsString + " ("+
                                    dmMain->coordToStr(sqlTPSelections->FieldByName("LATITUDE")->AsDouble,  'Y') + ", "+
                                    dmMain->coordToStr(sqlTPSelections->FieldByName("LONGITUDE")->AsDouble, 'X') + ")",
                                    new NodeData(ntTestPoint, sqlTPSelections->FieldByName("ID")->AsInteger)
                                    );
        sqlTPSelections->Next();
    }
    sqlTPSelections->Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::actNewSelectionExecute(TObject *Sender)
{
    TTreeNode *tn = trvExplorer->Selected;
    while (tn && tn->Parent)
        tn = tn->Parent;
    if (GetNodeData(tn)->nodeType == ntTx)
        txAnalyzer.MakeNewSelection(GetNodeData(tn)->id, nsExpertise);
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::actDeleteSelectionExecute(TObject *Sender)
{
    TTreeNode* tn = trvExplorer->Selected;
    if (GetNodeData(tn)->nodeType != ntSelection)
        return;
    if (Application->MessageBox(AnsiString().sprintf(szDeleteSelectionQuestion, tn->Text).c_str(),
                                Application->Title.c_str(),
                                MB_ICONQUESTION | MB_YESNO
                                ) == IDYES)
    {
        DeleteSelection(GetNodeData(tn)->id);
        trvExplorer->Items->Delete(tn);
    }
}

void __fastcall TfrmExplorer::actDeleteSelectionUpdate(TObject *Sender)
{
    TTreeNode* tn = trvExplorer->Selected;
    actDeleteSelection->Enabled = (GetNodeData(tn)->nodeType == ntSelection);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::FecthTx1Click(TObject *Sender)
{
    if (TTreeNode *tn = trvExplorer->Selected) {
        if (GetNodeData(tn)->nodeType == ntTx)
            //  просто выдернуть
            txBroker.GetTx(GetNodeData(tn)->id, dmMain->GetObjClsid(dmMain->GetSc(GetNodeData(tn)->id)));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actPlanExecute(TObject *Sender)
{
    if (TTreeNode *tn = trvExplorer->Selected) {
        while (tn->Parent)
            tn = tn->Parent;

        if (GetNodeData(tn)->nodeType != ntTx)
            throw *(new Exception("Can't locate Tx"));

        int txId = GetNodeData(tn)->id;
        if ( txAnalyzer.planningTx.IsBound() )
        {
            if ( txAnalyzer.planningTx.id != txId)
                if ( !txAnalyzer.isNewPlan && txAnalyzer.wasChanges )
                {
                    int reply = Application->MessageBox("Зберігти результати поточного планування?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNOCANCEL);
                    switch (reply) {
                        case IDYES:
                            txAnalyzer.SaveToDb();
                            break;
                        case IDCANCEL:
                            //FormProvider.ShowPlanning();
                            return;
                        default:
                            break;
                    }
                }

            txAnalyzer.planningTx.Bind(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))), true);
            txAnalyzer.LoadFromDb();
        }
        else
        {
            txAnalyzer.planningTx.Bind(txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId))), true);
            txAnalyzer.LoadFromDb();
        }

        FormProvider.ShowPlanning();
        if (txAnalyzer.planVector.empty())
            txAnalyzer.PerformPlanning(txId);
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmExplorer::FormCreate(TObject *Sender)
{
    width = Width;
    height = Height;
    try {
        LayoutManager.loadLayout(this);
    } catch (...) {
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::FormShow(TObject *Sender)
{
    if (frmExplorer != NULL)
        Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::FormResize(TObject *Sender)
{
    if ( Height != 0 )
        height = Height;
    if ( Width != 0 )
        width = Width;
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actMoveUp1Click(TObject *Sender)
{
    moveTreeItem(0);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actMoveDown1Click(TObject *Sender)
{
    moveTreeItem(1);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::moveTreeItem(int Direction)
{
    TTreeNode * tn = trvExplorer->Selected;
    if ( tn )
        if ( ((Direction == 0)&&(tn->getPrevSibling() != NULL)&&(tn->getPrevSibling()->getPrevSibling() != NULL))
        || ((Direction == 1)&&(tn->getNextSibling() != NULL)) )
        {
            int ID = GetNodeData(tn)->id;

            TIBSQL * sql = new TIBSQL(this);
              sql->Database = dmMain->dbMain;
              sql->GoToFirstRecordOnExecute = true;
              sql->Transaction = dmMain->trMain;

            sql->SQL->Clear();
            sql->SQL->Add("update USEROBJECTS");
            if ( Direction == 0 )
                sql->SQL->Add("set OBJECTNO = OBJECTNO - 1");
            else
                sql->SQL->Add("set OBJECTNO = OBJECTNO + 1");
            sql->SQL->Add("where ( (USERID = " + IntToStr(UserId) + ") AND (OBJECTID = " + IntToStr(ID) + ") )");

            sql->ExecQuery();
            sql->Close();

            sql->SQL->Clear();
            sql->SQL->Add("update USEROBJECTS");
            if ( Direction == 0 )
            {
                sql->SQL->Add("set OBJECTNO = OBJECTNO + 1");
                sql->SQL->Add("where ( (USERID = " + IntToStr(UserId) + ") AND (OBJECTID = " + IntToStr(GetNodeData(tn->getPrevSibling())->id) + ") )");
            }
            else
            {
                sql->SQL->Add("set OBJECTNO = OBJECTNO - 1");
                sql->SQL->Add("where ( (USERID = " + IntToStr(UserId) + ") AND (OBJECTID = " + IntToStr(GetNodeData(tn->getNextSibling())->id) + ") )");
            }

            sql->ExecQuery();

            Refresh();

            {
                for (TTreeNode *tn = trvExplorer->Items->GetFirstNode(); tn; tn = tn->getNextSibling())
                    if (GetNodeData(tn)->id == ID)
                    {
                        tn->Selected = true;
                        break;
                    }
            }
        }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::trvExplorerMouseDown(TObject *Sender, TMouseButton Button, TShiftState Shift, int X, int Y)
{
    actSelectionDeleteAll->Enabled = false;
    actTestPointDeleteAll->Enabled = false;

    if ( Button == Controls::mbRight )
    {
        if (TTreeNode *tn = trvExplorer->Selected)
        {
            switch(GetNodeData(tn)->nodeType)
            {
                case ntSelectionList:
                case ntSelection:
                    actSelectionDeleteAll->Enabled = true; break;
                case ntTestPointList:
                case ntTestPoint:
                    actTestPointDeleteAll->Enabled = true; break;
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actSelectionDeleteAllExecute(TObject *Sender)
{
    if ( Application->MessageBoxA("Видалити всі експертизи?", "Видалення", MB_YESNO | MB_ICONEXCLAMATION) == IDYES )
    {
        if (TTreeNode *tn = trvExplorer->Selected)
        {
            while (GetNodeData(tn)->nodeType != ntTx)
                tn = tn->Parent;

            if (GetNodeData(tn)->nodeType == ntTx)
            {
                tn = tn->getFirstChild();
                while (GetNodeData(tn)->nodeType != ntSelectionList)
                    tn = tn->getNextSibling();

                if (GetNodeData(tn)->nodeType == ntSelectionList)
                {
                    tn = tn->getFirstChild();
                    while (tn)
                    {
                        TTreeNode* nextNode = tn->getNextSibling();
                        if (GetNodeData(tn)->nodeType == ntSelection)
                        {
                            DeleteSelection(GetNodeData(tn)->id);
                            trvExplorer->Items->Delete(tn);
                        }
                        tn = nextNode;
                    }
                }
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::DeleteSelection(int id)
{
    //находим и закрываем окно этой выборки
    TfrmSelection* fs = NULL;
    for ( int i = 0; i < frmMain->MDIChildCount; i++ )
        if ( (fs = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i])) && (fs->GetId() == id) )
        {
            fs->wasChanges = false;
            fs->Close();
        }

    //собсвенно удаление
    sqlDeleteSelection->Params->Vars[0]->AsInteger = id;
    sqlDeleteSelection->ExecQuery();
    sqlDeleteSelection->Transaction->CommitRetaining();
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::actTestPointDeleteAllExecute(TObject *Sender)
{
    if ( Application->MessageBoxA("Видалити всі КТ?", "Видалення", MB_YESNO | MB_ICONEXCLAMATION) == IDYES )
    {
        if (TTreeNode *tn = trvExplorer->Selected)
        {
            while (GetNodeData(tn)->nodeType != ntTx)
                tn = tn->Parent;

            if (GetNodeData(tn)->nodeType == ntTx)
            {
                int txId = GetNodeData(tn)->id;
                tn = tn->getFirstChild();
                while (GetNodeData(tn)->nodeType != ntTestPointList)
                    tn = tn->getNextSibling();

                if (GetNodeData(tn)->nodeType == ntTestPointList)
                    tn->DeleteChildren();

                if ( txId > 0 )
                {
                    std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
                    sql->Database = dmMain->dbMain;
                    if (sql->Transaction->Active)
                        sql->Transaction->CommitRetaining();
                    else
                        sql->Transaction->StartTransaction();

                    //  очистить таблицу тестовых точек
                    sql->SQL->Text = "delete from TESTPOINTS where ( (TESTPOINT_TYPE = 0 or TESTPOINT_TYPE = 2) and (TRANSMITTERS_ID = :TRANSMITTERS_ID) )";
                    sql->ParamByName("TRANSMITTERS_ID")->Value = txId;

                    try
                    {
                        sql->ExecQuery();
                    }
                    catch(Exception& e)
                    {
                        Application->ShowException(&e);
                    }
                }
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::trvExplorerDeletion(TObject *Sender,
      TTreeNode *Node)
{
    delete (NodeData*)(Node->Data);
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::pmnExplorerPopup(TObject *Sender)
{
    if (SelectedIsSelection())
    {
        miIcst->Enabled = true;
        miIcst->Checked = GetNodeData(trvExplorer->Selected)->nodeType == ntIcstSel;
    } else {
        miIcst->Enabled = false;
        miIcst->Checked = false;
    }
}
//---------------------------------------------------------------------------

bool __fastcall TfrmExplorer::SelectedIsSelection()
{
    if (TTreeNode *tn = trvExplorer->Selected)
    {
        NodeData *nd = GetNodeData(tn);
        return (nd->nodeType == ntSelection || nd->nodeType == ntIcstSel);
    }
    else
        return false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmExplorer::miIcstClick(TObject *Sender)
{
    if (SelectedIsSelection())
    {
        NodeData *nd = GetNodeData(trvExplorer->Selected);
        if (nd->alien)
            throw *(new Exception("Не ваша вибірка - не вам по неї і клацати"));
        dmMain->SetSelectionIcst(nd->id, nd->nodeType != ntIcstSel);
        actRefreshExecute(Sender);
    } else
        throw *(new Exception("Вибраний елемент дерева не є вибіркою"));
}
//---------------------------------------------------------------------------

String __fastcall TfrmExplorer::GetTxBranchText(int txId)
{
    ILISBCTx *itx = txBroker.GetTx(txId, dmMain->GetObjClsid(dmMain->GetSc(txId)));

    TCOMILISBCTx tx(itx, true);

    TBCTxType tt;
    tx->get_systemcast(&tt);

    std::set<long> standIdSet;
    standIdSet.insert(tx.stand_id);
    dmMain->cacheSites(standIdSet, stands, false);
    std::map<int, StandRecord>::iterator si = stands.find(tx.stand_id);

    TempVal<char> tdc(DecimalSeparator, '.');

    String BranchText;

    if (tx.status_code != tsDraft)
    {
        if(tt != ttAllot)
            BranchText = si->second.regionNum.c_str() + String().sprintf("%04d", tx.adminid);
        else
        {
            std::auto_ptr<TIBSQL> sqlSel(new TIBSQL(Application));
            sqlSel->Database = dmMain->dbMain;
            sqlSel->SQL->Text = "select * from DIG_ALLOTMENT where ID = "+IntToStr(txId);
            sqlSel->ExecQuery();
            BranchText = sqlSel->FieldByName("ALLOT_NAME")->AsString;
            AnsiString str = sqlSel->FieldByName("ADM_REF_ID")->AsString;
            sqlSel->Close();
            sqlSel->SQL->Text = "select tx.systemcast_id, sc.enumval from transmitters tx LEFT OUTER JOIN SYSTEMCAST SC on (SC.ID = TX.SYSTEMCAST_ID) where tx.ASSOCIATED_ADM_ALLOT_ID = '" + str + "'";
            sqlSel->ExecQuery();
            if(!sqlSel->Eof)
                BranchText += ": " + dmMain->GetSystemCastName(sqlSel->FieldByName("enumval")->AsInteger);
        }
    }
    else
        BranchText = "[Планир]";
    if(tt != ttAllot)
    {
        if (BranchText.Length() > 0)
            BranchText += " ";
        BranchText = BranchText +
            si->second.siteName.c_str() + ": " +
            dmMain->GetSystemCastName(tx.systemcast);
            ;
    }
    String nom = txAnalyzer.GetTxNominalString(tx);
    if (nom.Length() > 0)
        BranchText = BranchText + ", " + nom;
    BranchText += ' ';

    dmMain->ibdsCountries->Open();
    AnsiString countryCode;

    if (dmMain->ibdsCountries->Locate("ID", si->second.countryId, TLocateOptions()))
        countryCode = dmMain->ibdsCountriesCODE->AsString;

    BranchText = BranchText +
    dmMain->coordToStr(tx.longitude, 'X')+" "+
    dmMain->coordToStr(tx.latitude, 'Y')+" "+
    countryCode;

    return BranchText;
}
