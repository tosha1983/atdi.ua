//---------------------------------------------------------------------------

#ifndef uLisObjectGridFormH
#define uLisObjectGridFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uLisObjectGrid.h"
#include <IBSQL.hpp>
#include <Menus.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <ImgList.hpp>
#include <ActnList.hpp>
#include <ToolWin.hpp>
#include <memory>
//---------------------------------------------------------------------------
class TLisObjectGridForm : public TForm, public TLisObjectGridProxi
{
__published:	// IDE-managed Components
    TLisObjectGrid *grid;
    TIBSQL *selSql;
    TIBSQL *updSql;
    TPanel *panTree;
    TTreeView *tvList;
    TStaticText *txListQuantity;
    TSplitter *splTree;
    TImageList *ilList;
    TIBSQL *sqlFindGrp;
    TToolBar *tbrTx;
    TToolButton *tbtAnalyze;
    TToolButton *tbtExport;
    TToolButton *tbtRecalcCoordDists;
    TToolButton *tbtMoveToSect;
    TToolButton *tbtCopyToSect;
    TToolButton *tbtExpGt1Gs1;
    TImageList *imlTx;
    TActionList *alTx;
    TAction *actToBase;
    TAction *actArchive;
    TAction *actToDraft;
    TAction *actAnalyze;
    TAction *actExport;
    TAction *actRecalcTbtCoordDists;
    TAction *actMoveToSection;
    TAction *actCopyToSection;
    TAction *actExpGt1Gs1;
    TPanel *panDbSection;
    TLabel *lblDbSection;
    TComboBox *cbxDbSection;
    TToolButton *tbtSearch;
    TToolButton *tbtSearchCancel;
    TPanel *panSearch;
    TEdit *edtIncSearch;
    TLabel *lblSearchParam;
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall MenuClick(TObject *Sender);
    void __fastcall tvListDblClick(TObject *Sender);
    void __fastcall tvListKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall actAnalyzeExecute(TObject *Sender);
    void __fastcall actExportExecute(TObject *Sender);
    void __fastcall actRecalcTbtCoordDistsExecute(TObject *Sender);
    void __fastcall actMoveToSectionExecute(TObject *Sender);
    void __fastcall actCopyToSectionExecute(TObject *Sender);
    void __fastcall actExpGt1Gs1Execute(TObject *Sender);
    void __fastcall cbxDbSectionChange(TObject *Sender);
    void __fastcall tbtSearchClick(TObject *Sender);
    void __fastcall tbtSearchCancelClick(TObject *Sender);
    void __fastcall edtIncSearchExit(TObject *Sender);
    void __fastcall edtIncSearchKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
private:	// User declarations
    int refreshingRow;
    int m_elementId;
    std::vector<int> dbSectIds;
    int syscast;
public:		// User declarations
    int objType;

    __fastcall TLisObjectGridForm(TComponent* Owner);

    void __fastcall changeBranch(TTreeNode *newNode);
    void __fastcall fillNode(TTreeNode* node, int level);
    void __fastcall fillTree();
    void __fastcall TLisObjectGridForm::findBranch();
    std::auto_ptr<TStringList> treeQryList;
    typedef std::map<String, Variant> ParamMap;
    ParamMap params;
    AnsiString queryParam;
    AnsiString colNameParam;
    AnsiString oldQuery;
    int colN;

    void RunQuery(TIBSQL* sql, String query);

    bool RunListQuery(TLisObjectGrid* sender, String query);
    String RunQuery(TLisObjectGrid* sender, String query);
    bool Next(TLisObjectGrid* sender);
    String GetVal(TLisObjectGrid* sender, int row, LisColumnInfo info);
    bool IsEof(TLisObjectGrid* sender);
    int GetId(TLisObjectGrid* sender, int row);
    String GetFieldSpecFromAlias(TLisObjectGrid* sender, String alias);
    void FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw);
    void SortGrid(TLisObjectGrid* sender, int colIdx);
    int CreateObject(TLisObjectGrid* sender);
    int CopyObject(TLisObjectGrid* sender, int objId);
    bool DeleteObject(TLisObjectGrid* sender, int objId);
    bool EditObject(TLisObjectGrid* sender, int objId);
    void PickObject(TLisObjectGrid* sender, int objId);
    void __fastcall Init(int objType, int elementId = 0, int extraTag = 0,  int systemcast = 0);
};
//---------------------------------------------------------------------------
extern PACKAGE TLisObjectGridForm *LisObjectGridForm;
//---------------------------------------------------------------------------
#endif
