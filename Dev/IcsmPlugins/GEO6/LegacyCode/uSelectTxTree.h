//---------------------------------------------------------------------------

#ifndef uSelectTxTreeH
#define uSelectTxTreeH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <ImgList.hpp>
#include <IBSQL.hpp>
#include <vector>

//---------------------------------------------------------------------------
using namespace std;
class TfrmSelectTxTree : public TForm
{
__published:	// IDE-managed Components
    TTreeView *trvTxTree;
    TImageList *ImageList1;
    TIBSQL *sqlSelectTx;
    void __fastcall FormDeactivate(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall trvTxTreeDblClick(TObject *Sender);
    void __fastcall trvTxTreeKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
private:	// User declarations
    int in_id;
    void __fastcall fillTree(int root_id);
    void __fastcall fillNode(TTreeNode* node, int id);
    AnsiString __fastcall SelectTx(int TxId);
    vector <int> list_id;
    bool __fastcall IdExist(vector<int> &list, int id);
public:		// User declarations
//    __fastcall TfrmSelectTxTree(TComponent* Owner);
    __fastcall TfrmSelectTxTree(int SelectTxId, TComponent* Owner);
    int __fastcall SelectIdRootTx(int id);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmSelectTxTree *frmSelectTxTree;
//---------------------------------------------------------------------------
#endif
