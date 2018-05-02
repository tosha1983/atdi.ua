//----------------------------------------------------------------------------
#ifndef uDlgMapConfH
#define uDlgMapConfH
//----------------------------------------------------------------------------
#include <vcl\System.hpp>
#include <vcl\Windows.hpp>
#include <vcl\SysUtils.hpp>
#include <vcl\Classes.hpp>
#include <vcl\Graphics.hpp>
#include <vcl\StdCtrls.hpp>
#include <vcl\Forms.hpp>
#include <vcl\Controls.hpp>
#include <vcl\Buttons.hpp>
#include <vcl\ExtCtrls.hpp>
#include <Dialogs.hpp>

#include <vector>
//----------------------------------------------------------------------------
class TdlgMapConf : public TForm
{
__published:
    TButton *btnOk;
    TButton *btnCancel;
    TListBox *lbxParams;
    TEdit *edtFilePath;
    TBitBtn *btnUp;
    TBitBtn *btnDown;
    TButton *btnSelectFilePath;
    TGroupBox *gbxZoom;
    TOpenDialog *opd;
    TLabel *Label1;
    TEdit *edtName;
    TCheckBox *chbVisible;
    TCheckBox *chbAutoLabels;
    TLabel *Label2;
    TLabel *Label3;
    TEdit *edtMinZoom;
    TEdit *edtMaxZoom;
    TButton *btnReplace;
    TButton *btnAdd;
    TButton *btnDelete;
    void __fastcall btnUpClick(TObject *Sender);
    void __fastcall btnDownClick(TObject *Sender);
    void __fastcall btnSelectFilePathClick(TObject *Sender);
    void __fastcall btnReplaceClick(TObject *Sender);
    void __fastcall btnAddClick(TObject *Sender);
    void __fastcall btnDeleteClick(TObject *Sender);
    void __fastcall btnOkClick(TObject *Sender);
    void __fastcall lbxParamsClick(TObject *Sender);
    void __fastcall lbxParamsKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall lbxParamsKeyUp(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall chbVisibleClick(TObject *Sender);
    void __fastcall chbAutoLabelsClick(TObject *Sender);
    void __fastcall edtNameChange(TObject *Sender);
    void __fastcall edtMinZoomChange(TObject *Sender);
    void __fastcall edtMaxZoomChange(TObject *Sender);
private:
    struct LayerParams {
        AnsiString name;
        AnsiString path;
        bool visible;
        bool labels;
        AnsiString minZoom;
        AnsiString maxZoom;
        LayerParams(): visible(false), labels(false) {};
        LayerParams(const LayerParams& src)
        {
            name = src.name;
            path = src.path;
            visible = src.visible;
            labels = src.labels;
            minZoom = src.minZoom;
            maxZoom = src.maxZoom;
        }
        LayerParams(LayerParams& src)
        {
            name = src.name;
            path = src.path;
            visible = src.visible;
            labels = src.labels;
            minZoom = src.minZoom;
            maxZoom = src.maxZoom;
        }
        LayerParams& operator=(LayerParams& src)
        {
            name = src.name;
            path = src.path;
            visible = src.visible;
            labels = src.labels;
            minZoom = src.minZoom;
            maxZoom = src.maxZoom;
            return *this;
        }
    };

    typedef std::vector<LayerParams> ParamsVector;
    ParamsVector params;

    bool changed;

	virtual __fastcall TdlgMapConf(TComponent* AOwner);
    void __fastcall ShowData();

    AnsiString filename;
public:

	virtual __fastcall TdlgMapConf(TComponent* AOwner, AnsiString confFile);

};
//----------------------------------------------------------------------------
extern PACKAGE TdlgMapConf *dlgMapConf;
//----------------------------------------------------------------------------
#endif
