//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uSelectTxTree.h"
#include "LISBCTxServer_TLB.h"
#include "TxBroker.h"
#include "FormProvider.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmSelectTxTree *frmSelectTxTree;
//---------------------------------------------------------------------------
//__fastcall TfrmSelectTxTree::TfrmSelectTxTree(TComponent* Owner)
//    : TForm(Owner)
//{
//}
using namespace std;

bool __fastcall TfrmSelectTxTree::IdExist(vector<int> &list, int id)
{
 for(vector<int>::iterator i = list.begin(); i != list.end(); i++)
   if (*i == id) return true;
 return false;
}


int __fastcall TfrmSelectTxTree::SelectIdRootTx(int id)
{
  TIBSQL *sql = new TIBSQL(this);
  sql->Database = dmMain->dbMain;
  sql->SQL->Text = "select ORIGINALID from TRANSMITTERS where id = " + AnsiString(id);
  sql->ExecQuery();
  if (IdExist(list_id,id)) return id;
  else  if (sql->Fields[0]->AsInteger == 0) return id;
  else if (sql->Fields[0]->AsInteger == id) return id;
  else {list_id.push_back(id); return SelectIdRootTx(sql->Fields[0]->AsInteger);}
}

__fastcall TfrmSelectTxTree::TfrmSelectTxTree(int SelectTxId, TComponent* Owner)
    : TForm(Owner)
{
  in_id = SelectTxId;
  list_id.clear();
  int rootId = SelectIdRootTx(SelectTxId);
  list_id.clear();
  fillTree(rootId);

}

void __fastcall TfrmSelectTxTree::fillTree(int root_id)
{
    trvTxTree->Items->Clear();
    TTreeNode *nn = trvTxTree->Items->Add(NULL, SelectTx(root_id));
    nn->Data = (void*)root_id;
    if (root_id != in_id) {
        nn->ImageIndex = 0;
        nn->SelectedIndex = 0;
        } else {
        nn->ImageIndex = 1;
        nn->SelectedIndex = 1;
        }
    list_id.push_back(root_id);
    fillNode(nn, root_id);
    trvTxTree->FullExpand();
}

void __fastcall TfrmSelectTxTree::fillNode(TTreeNode* node, int id)
{
        TIBSQL* sql = new TIBSQL(this);
        sql->Database = dmMain->dbMain;
        try {
            int select_id  = (int)node->Data;
            sql->SQL->Text = "select ID from TRANSMITTERS where ORIGINALID = " + AnsiString(select_id);
            sql->ExecQuery();
            if (IdExist(list_id, sql->Fields[0]->AsInteger)) return;
            else while (!sql->Eof) {
                TTreeNode *nn = trvTxTree->Items->AddChild(node, SelectTx(sql->Fields[0]->AsInteger));
                nn->Data = (void*)sql->Fields[0]->AsInteger;
            if (sql->Fields[0]->AsInteger == in_id) {
                nn->ImageIndex = 1;
                nn->SelectedIndex = 1;
                nn->Selected = true;
                } else {
                nn->ImageIndex = 0;
                nn->SelectedIndex = 0;
                }
                list_id.push_back((int)nn->Data);
                fillNode(nn, (int)nn->Data);
                sql->Next();
            }
            sql->Close();
        } __finally {
            sql->Free();
        }
}

AnsiString __fastcall TfrmSelectTxTree::SelectTx(int TxId)
{
    sqlSelectTx->Close();
    sqlSelectTx->Params->Vars[0]->AsInteger = TxId;
    sqlSelectTx->ExecQuery();
    //if (!sqlSelectTx->Eof) {
        AnsiString BranchText =
            sqlSelectTx->FieldByName("NUMREGION")->AsString + "-"+
            sqlSelectTx->FieldByName("ADMINISTRATIONID")->AsString + " " +
            sqlSelectTx->FieldByName("NAMESITE")->AsString + ": " +
            sqlSelectTx->FieldByName("SC_CODE")->AsString;
            ;
        switch (sqlSelectTx->FieldByName("SC_ENUMVAL")->AsInteger) {
            case ttTV: case ttDVB:
                BranchText = BranchText + ", “¬  " + sqlSelectTx->FieldByName("NAMECHANNEL")->AsString + ' ';
                break;
            case -1:
            case ttAM:
            case ttFM:
                BranchText = BranchText + FormatFloat(", #.#",
                    sqlSelectTx->FieldByName("SOUND_CARRIER_PRIMARY")->AsDouble)+ " ћ√ц ";
                break;
            case ttDAB:
                BranchText = BranchText + ", Ѕлок " + sqlSelectTx->FieldByName("BD_NAME")->AsString+ ' ';
                break;
            case ttCTV:
                BranchText = BranchText + ' ';
                break;
            default:
                BranchText = BranchText + " нев≥дома система <"+
                    sqlSelectTx->FieldByName("SC_ENUMVAL")->AsString+"> ";
                break;
        }
        BranchText = BranchText + "("+
        dmMain->coordToStr(sqlSelectTx->FieldByName("LATITUDE")->AsDouble, 'Y')+" - "+
        dmMain->coordToStr(sqlSelectTx->FieldByName("LONGITUDE")->AsDouble, 'X')+", "+
        sqlSelectTx->FieldByName("CO_CODE")->AsString + ")";
        sqlSelectTx->Close();

        return BranchText;
}

//---------------------------------------------------------------------------
void __fastcall TfrmSelectTxTree::FormDeactivate(TObject *Sender)
{
 Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectTxTree::FormCreate(TObject *Sender)
{
  Width = 540;
  Height = 320;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectTxTree::trvTxTreeDblClick(TObject *Sender)
{
      //trvTxTree->FullExpand();
  if (trvTxTree->Selected) {
      int id = (int)trvTxTree->Selected->Data;
      ILISBCTx* newTx = txBroker.GetTx(id, dmMain->GetObjClsid(dmMain->GetSc(id)));
      FormProvider.ShowTx(newTx);
  }    
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectTxTree::trvTxTreeKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
        if (Key == 13 && Shift == TShiftState())
            this->trvTxTreeDblClick(this);
}
//---------------------------------------------------------------------------

