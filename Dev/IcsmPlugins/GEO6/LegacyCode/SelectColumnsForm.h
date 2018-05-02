//---------------------------------------------------------------------------

#ifndef SelectColumnsFormH
#define SelectColumnsFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <Forms.hpp>
#include <IniFiles.hpp>
#include <StdCtrls.hpp>

//---------------------------------------------------------------------------
class TfrmSelectColumns : public TForm
{
__published:	// IDE-managed Components
        TLabel *Label1;
        TListBox *lbAccessibleFields;
        TListBox *lbSelectedFields;
        TLabel *Label2;
        TButton *btnAdd;
        TButton *btnAddAll;
        TButton *btnRemove;
        TButton *btnRemoveAll;
        TButton *btnUp;
        TButton *btnDown;
        TButton *btnOk;
        TButton *btnCancel;
        void __fastcall btnAddClick(TObject *Sender);
        void __fastcall btnUpClick(TObject *Sender);
        void __fastcall btnDownClick(TObject *Sender);
        void __fastcall btnAddAllClick(TObject *Sender);
        void __fastcall btnRemoveAllClick(TObject *Sender);
        void __fastcall btnCancelClick(TObject *Sender);
        void __fastcall btnRemoveClick(TObject *Sender);
        void __fastcall btnOkClick(TObject *Sender);
private:
public:
        __fastcall TfrmSelectColumns(TComponent* Owner);
};
//---------------------------------------------------------------------------
#endif
