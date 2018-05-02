//---------------------------------------------------------------------------


#ifndef uLisObjectGridH
#define uLisObjectGridH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <Grids.hpp>
#include <ImgList.hpp>
#include <ActnList.hpp>
#include <Menus.hpp>
#include <ToolWin.hpp>
#include <memory>
#include <map>
#include <vector>
#include "CoordConv.hpp"
//---------------------------------------------------------------------------

class TLisObjectGrid;

enum LisPropType { ptString, ptLon, ptLat, ptDouble, ptInt, ptFreq, ptdBWt, ptdBkWt, ptDate, ptTime, ptDateTime };

struct LisColumnInfo
{
    String title;
    String propName;
    String fldName;
    TAlignment am;
    LisPropType propType;
    int width;
    TFontStyles fontStyle;
    int imgIdx;
    bool customSort;
    String fltrCond;

    LisColumnInfo(): imgIdx(-1), customSort(false) {};
    LisColumnInfo(const LisColumnInfo& src) { *this = src; }
    LisColumnInfo& operator=(const LisColumnInfo& src)
    {
        title = src.title; propName = src.propName; fldName = src.fldName;
        am = src.am; propType = src.propType;
        width = src.width; fontStyle = src.fontStyle; imgIdx = src.imgIdx;
        customSort = src.customSort; fltrCond = src.fltrCond;
        return *this;
    }
};

struct TLisObjectGridProxi
{
    virtual bool RunListQuery(TLisObjectGrid* sender, String query) = 0;
    virtual String RunQuery(TLisObjectGrid* sender, String query) = 0;
    virtual bool Next(TLisObjectGrid* sender) = 0;
    virtual String GetVal(TLisObjectGrid* sender, int row, LisColumnInfo info) = 0;
    virtual bool IsEof(TLisObjectGrid* sender) = 0;
    virtual int GetId(TLisObjectGrid* sender, int row)= 0;
    virtual String GetFieldSpecFromAlias(TLisObjectGrid* sender, String alias) = 0;
    virtual void FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw) = 0;
    virtual void SortGrid(TLisObjectGrid* sender, int colIdx) = 0;
    virtual int CreateObject(TLisObjectGrid* sender) = 0;
    virtual int CopyObject(TLisObjectGrid* sender, int objId) = 0;
    virtual bool DeleteObject(TLisObjectGrid* sender, int objId) = 0;
    virtual bool EditObject(TLisObjectGrid* sender, int objId) = 0;
    virtual void PickObject(TLisObjectGrid* sender, int objId) = 0;
};

class TLisObjectGrid : public TFrame
{
__published:	// IDE-managed Components
    TStringGrid *dg;
    THeaderControl *hd;
    TImageList *imlHd;
    TPopupMenu *pmn;
    TActionList *al;
    TAction *actCalcRows;
    TAction *actEdit;
    TAction *actSelect;
    TMenuItem *miEdit;
    TMenuItem *miSelect;
    TMenuItem *miCalcRows;
    TAction *actRefresh;
    TAction *actNew;
    TAction *actDelete;
    TAction *actCopy;
    TImageList *imlAct;
    TMenuItem *miNew;
    TMenuItem *miCopy;
    TMenuItem *miDel;
    TMenuItem *N4;
    TMenuItem *miRefresh;
    void __fastcall hdSectionResize(THeaderControl *HeaderControl,
          THeaderSection *Section);
    void __fastcall dgTopLeftChanged(TObject *Sender);
    void __fastcall dgDblClick(TObject *Sender);
    void __fastcall dgKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall dgDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall hdSectionClick(THeaderControl *HeaderControl,
          THeaderSection *Section);
    void __fastcall FrameResize(TObject *Sender);
    void __fastcall actCalcRowsExecute(TObject *Sender);
    void __fastcall actEditExecute(TObject *Sender);
    void __fastcall actSelectExecute(TObject *Sender);
    void __fastcall actRefreshExecute(TObject *Sender);
    void __fastcall actNewExecute(TObject *Sender);
    void __fastcall actDeleteExecute(TObject *Sender);
    void __fastcall actCopyExecute(TObject *Sender);

protected:

    AnsiString sql;
    std::vector<double> vOldParam;
    int oldLeftIdx;
    //  вызывающее окно
    TForm* m_caller;
    //  caller tag
    int m_callerTag;

    void SetOrderBy(AnsiString orderStr, AnsiString fldName);

    virtual void __fastcall EditElement();
    virtual void __fastcall DeleteElement();
    virtual void __fastcall PickElement();
    virtual void __fastcall LoadData(int lastRow);

    TLisObjectGridProxi* gridProxi;

public:

    __fastcall TLisObjectGrid(TComponent* Owner);
    __fastcall ~TLisObjectGrid();

    std::vector<LisColumnInfo> columnsInfo;
    typedef std::vector<int> IdVector;
    IdVector ids;

    long objCount;
    bool onlyFromList;

    void __fastcall Clear();
    void __fastcall Refresh();
    void __fastcall ClearColumns();
    void __fastcall RecreateHeader();

    unsigned __fastcall GetCurrentId();
    std::vector<unsigned> __fastcall GetSelectedIdList();
    bool __fastcall SetCurrentRow(unsigned id);
    void __fastcall AddColumn(LisColumnInfo&);
    void __fastcall AddColumn(AnsiString title, AnsiString prop, AnsiString fld, TAlignment am = taLeftJustify, LisPropType propType = ptString, int width = 64);

    void __fastcall SetCallerInfo(TForm* caller, int callerTag, int id);

    TForm* GetCaller() { return m_caller; };
    int GetCallerTag() { return m_callerTag; }
    TLisObjectGridProxi* GetProxi() { return gridProxi; };
    void __fastcall SetProxi(TLisObjectGridProxi* newProxi) { gridProxi = newProxi; };
    void __fastcall SetQuery(AnsiString query);
    String __fastcall GetQuery() { return sql; }
    String __fastcall GetValue(String colName);
    String __fastcall GetFieldSpecFromAlias(String alias);
    void __fastcall Sort(int colIdx);
    void __fastcall RefreshRow(int row);
};
//---------------------------------------------------------------------------
#endif
