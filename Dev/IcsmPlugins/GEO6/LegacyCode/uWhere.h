//---------------------------------------------------------------------------
#ifndef uWhereH
#define uWhereH

#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <Forms.hpp>
#include <Graphics.hpp>
#include <IBDataBase.hpp>
#include <IBQuery.hpp>
#include <IniFiles.hpp>
#include <Menus.hpp>
#include <StdCtrls.hpp>
#include <memory>
#include <Menus.hpp>
#include <StdCtrls.hpp>
#include <vector>
#include <vcl.h>

//---------------------------------------------------------------------------

typedef enum {
    ftOrdinaryNumeric=1,
    ftOrdinaryString,
    ftEnumerated,
    ftReference
    }
    FieldTypes;

//---------------------------------------------------------------------------
class CIniItem
{
public:
    TStringList         *allowableActions;
    AnsiString          fieldCaption;
    AnsiString          fieldName;
    AnsiString          fieldType;
    bool                Loaded;

    TIBDatabase *       lookupDataBase;
    AnsiString          lookupTable;
    AnsiString          lookupKey;
    AnsiString          lookupValue;

    TStringList         *Values;

     CIniItem();
     CIniItem(const CIniItem &value);
    ~CIniItem();
};

CIniItem::CIniItem(){
    allowableActions = new TStringList();

    lookupDataBase = NULL;
    lookupTable = "";
    lookupKey = "";
    lookupValue = "";

    Loaded = false;
    Values = new TStringList();
}

CIniItem::CIniItem(const CIniItem& value){
    allowableActions = new TStringList();
    allowableActions->Assign(value.allowableActions);

    fieldCaption = value.fieldCaption;

    fieldName = value.fieldName;

    fieldType = value.fieldType;

    Loaded = value.Loaded;

    lookupDataBase = value.lookupDataBase;
    lookupTable = value.lookupTable;
    lookupKey = value.lookupKey;
    lookupValue = value.lookupValue;

    Values = new TStringList();
    Values->Assign(value.Values);
}

CIniItem::~CIniItem(){
    delete allowableActions;
    delete Values;
}

//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
class CNodeParam
{
public:
    bool Assigned;
    HTREEITEM   ItemId;
    int         IniItemsIndex;//индекс в IniItems
    AnsiString  Operand;
    AnsiString  Operation;
    AnsiString  Value;
    CNodeParam();
    CNodeParam(const CNodeParam &value);
   ~CNodeParam();
};

CNodeParam::CNodeParam()
{
    Assigned = false;
    ItemId = 0;
    IniItemsIndex = 0;
    Operand = "";
    Operation = "";
    Value = "";
}

CNodeParam::CNodeParam(const CNodeParam &value)
{
    Assigned = value.Assigned;
    ItemId = value.ItemId;
    IniItemsIndex = value.IniItemsIndex;
    Operand = value.Operand;
    Operation = value.Operation;
    Value = value.Value;
}

CNodeParam::~CNodeParam()
{
}
//---------------------------------------------------------------------------
class TfmWhereCriteria : public TFrame
{
__published:	// IDE-managed Components
    TTreeView *tvwCriteria;
    TPopupMenu *pmnWhere;
    TMenuItem *mniAddCondition;
    TMenuItem *mniAddComposeCondition;
    TMenuItem *mniMakeComposeCondition;
    TMenuItem *DeleteCondition;
        TEdit *edit;
    void __fastcall mniAddConditionClick(TObject *Sender);
    void __fastcall tvwCriteriaMouseDown(TObject *Sender,
          TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall mniAddComposeConditionClick(TObject *Sender);
    void __fastcall mniMakeComposeConditionClick(TObject *Sender);
    void __fastcall DeleteConditionClick(TObject *Sender);
        void __fastcall tvwCriteriaKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
private:
    TComboBox * comboBox;
    void __fastcall EditOnKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
    void __fastcall EditOnExit(TObject *Sender);

    void __fastcall ComboBox_OnCloseUp(TObject *Sender);
    void __fastcall ComboBox_OnExit(TObject *Sender);
    void __fastcall ComboBox_OnKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);
    void __fastcall ComboBox_EditCriteria_OnKeyDown(TObject *Sender, WORD &Key, TShiftState Shift);

    std::vector<CIniItem> IniItems;
    std::vector<CNodeParam> paramNodes;
public:		// User declarations
    AnsiString sqlQuery;

    __fastcall TfmWhereCriteria(TComponent* Owner);
    void loadConfig(AnsiString iniFileName, TIBDatabase *db);
    AnsiString __fastcall getClause();
};
//---------------------------------------------------------------------------
#endif
