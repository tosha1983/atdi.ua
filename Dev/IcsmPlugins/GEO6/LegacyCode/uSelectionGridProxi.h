//---------------------------------------------------------------------------

#ifndef uSelectionGridProxiH
#define uSelectionGridProxiH

#include "uLisObjectGrid.h"
//---------------------------------------------------------------------------

class TSelectionProxi: public TComponent, public TLisObjectGridProxi
{
  public:
    int curPos;
    int maxPos;
    __fastcall TSelectionProxi(TComponent* owner): TComponent(owner), curPos(-1), maxPos(-1) {};

    String RunQuery(TLisObjectGrid* sender, String query);
    bool RunListQuery(TLisObjectGrid* sender, String query);
    bool Next(TLisObjectGrid* sender);
    String GetVal(TLisObjectGrid* sender, int row, LisColumnInfo info);
    bool IsEof(TLisObjectGrid* sender) { return curPos >= maxPos; } ;
    int GetId(TLisObjectGrid* sender, int row);
    String GetFieldSpecFromAlias(TLisObjectGrid* sender, String alias) { return String(); } ;
    void FormatCanvas(TLisObjectGrid* sender, int aCol, int aRow, TCanvas *Canvas, bool &draw);
    void SortGrid(TLisObjectGrid* sender, int colIdx);
    int CreateObject(TLisObjectGrid* sender) { return 0; };
    int CopyObject(TLisObjectGrid* sender, int objId) { return 0; };
    bool DeleteObject(TLisObjectGrid* sender, int objId) { return 0; };
    bool EditObject(TLisObjectGrid* sender, int objId) { return false; };
    void PickObject(TLisObjectGrid* sender, int objId) {};
};

extern TSelectionProxi* selProxi;

#endif
