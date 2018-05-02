//---------------------------------------------------------------------------

#ifndef uContourFormH
#define uContourFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <ExtCtrls.hpp>
#include <ComCtrls.hpp>
#include <Grids.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <IBSQL.hpp>
#include <vector>
#include "uMainDm.h"
//#include "BcDrawer.h"
#include "CustomMap.h"

//---------------------------------------------------------------------------
using namespace Lis_map;

class TfrmContour : public TForm
{
__published:	// IDE-managed Components
    TPanel *panData;
    TPanel *panBottom;
    TBitBtn *btnOk;
    TBitBtn *btnSave;
    TBitBtn *btnLoad;
    TBitBtn *btnCancel;
    TActionList *ActionList1;
    TAction *actOk;
    TAction *actSave;
    TAction *actLoad;
    TAction *actClose;
    TLabel *lblItuId;
    TEdit *edtItuId;
    TLabel *lblAdmin;
    TLabel *lblCountry;
    TEdit *edtAdmin;
    TComboBox *cbxCountry;
    TPanel *panContours;
    TStringGrid *grdContour;
    TPanel *panMap;
    TToolBar *tbContours;
    TToolButton *tbtAddCntr;
    TToolButton *tbtDelCntr;
    TToolButton *ToolButton5;
    TToolButton *tbtZoomIn;
    TToolButton *tbtZoomOut;
    TToolButton *tbtLeft;
    TToolButton *tbtRight;
    TImageList *imageList;
    TToolButton *tbtDown;
    TToolButton *tbtUp;
    TToolButton *ToolButton1;
    TToolButton *ToolButton2;
    TToolButton *tbtNearestPoint;
    TIBSQL *sqlContour;
    TToolButton *tbtFit;
    TToolButton *ToolButton4;
    TLabel *lblHint;
    void __fastcall actOkExecute(TObject *Sender);
    void __fastcall actSaveExecute(TObject *Sender);
    void __fastcall actLoadExecute(TObject *Sender);
    void __fastcall actCloseExecute(TObject *Sender);
    void __fastcall actSaveUpdate(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall tbtAddPointClick(TObject *Sender);
    void __fastcall tbtDelPointClick(TObject *Sender);
    void __fastcall grdContourSetEditText(TObject *Sender, int ACol,
          int ARow, const AnsiString Value);
    void __fastcall MapOnMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall MapOnMouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y);
    void __fastcall MapOnMouseUp(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall OnObjectSelection(TObject * Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift);
    void __fastcall edtItuIdExit(TObject *Sender);
    void __fastcall cbxCountryChange(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall grdContourKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall grdContourMouseDown(TObject *Sender,
          TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall tbtFitClick(TObject *Sender);
private:	// User declarations
    AnsiString contourId;
    Lis_map::Points contour;
    Lis_map::Points existingPoints;
    AnsiString ctry;
    bool changed;

    TCustomMapFrame *cmf;

    TMouseEvent oldMouseDown;
    TMouseEvent oldMouseUp;
    TMouseMoveEvent oldMouseMove;

    void __fastcall ShowContour();

    bool dragging;
    int selectedPoint;

    int __fastcall GetDbSection();

protected:
public:		// User declarations
    __fastcall TfrmContour(TComponent* Owner);
    void __fastcall ShowData();
    int id;
    void __fastcall AddPointEx(int x, int y);
    void __fastcall AddPoint(double x, double y);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmContour *frmContour;
//---------------------------------------------------------------------------
#endif
