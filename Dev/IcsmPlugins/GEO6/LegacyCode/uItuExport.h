#ifndef uItuExportH
#define uItuExportH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Grids.hpp>
#include <DBGrids.hpp>
#include <IBSQL.hpp>
#include <ExtCtrls.hpp>
#include <vector>
//---------------------------------------------------------------------------
typedef struct {char* elName; char* fldName; char fldType; char frmt; double delta; } ExportNoticeElements;
typedef enum { emNone = -1, emGa1, emGs1Gt1, emGs2Gt2, emLast } ExportMode;


class TfrmRrc06Export : public TForm
{
__published:
    TLabel *Label1;
    TEdit *edtFileName;
    TButton *btnFile;
    TButton *btnImport;
    TButton *btnCancel;
    TMemo *memComment;
    TLabel *Label2;
    TLabel *Label3;
    TStringGrid *sgHead;
    TLabel *Label4;
    TComboBox *cbxAction;
    TCheckBox *chbOpenFile;
    TRadioGroup *rgArt;
    TLabel *Label5;
    TComboBox *cbIsPubReq;
    TLabel *Label6;
    TComboBox *cbRemarkCondsMet;
    TLabel *Label7;
    TComboBox *cbIsResub;
    TLabel *Label8;
    TComboBox *cbSignedCommitment;
    void __fastcall btnFileClick(TObject *Sender);
    void __fastcall btnImportClick(TObject *Sender);
public:
    static int __fastcall ExportRrc006FromGrid(ExportMode type, TDBGrid*);
    static int __fastcall ExportRrc006(ExportMode type, std::vector<int> ids);
	__fastcall ~TfrmRrc06Export();

protected:
	__fastcall TfrmRrc06Export(TComponent* Owner);
    bool exported;
private:
    void __fastcall LoadConf();
    void SaveConf();
    static const char * __fastcall BoolToStr(bool val);
    static void __fastcall ExportCoord(TIBXSQLVAR* fld, std::ofstream& of);
    static void __fastcall ExportVector(TIBXSQLVAR* fld, std::ofstream& of, char* tag, char* pref, int k, double c);
};

#endif
