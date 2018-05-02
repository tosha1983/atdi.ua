//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uLisObjectGrid.h"
#include "tempvalues.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
//---------------------------------------------------------------------------
__fastcall TLisObjectGrid::TLisObjectGrid(TComponent* Owner)
    : TFrame(Owner), gridProxi(NULL)
{
    oldLeftIdx = 0;
    m_caller = 0;
    gridProxi = NULL;
    sql = "";
    onlyFromList = false;
}
//---------------------------------------------------------------------------

__fastcall TLisObjectGrid::~TLisObjectGrid()
{
    gridProxi = NULL;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::SetQuery(AnsiString query)
{
    sql = query;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::SetCallerInfo(TForm* caller, int callerTag, int id)
{
    m_caller = caller;
    m_callerTag = callerTag;
    SetCurrentRow(id);
}
//---------------------------------------------------------------------------

bool __fastcall TLisObjectGrid::SetCurrentRow(unsigned elementId)
{
    bool res = false;
    if (elementId)
    {
        long idx = -1;
            for (int curRow = 0; curRow < dg->RowCount; )
            {
                if (curRow < ids.size())
                {
                    if (ids[curRow] == elementId)
                    {
                        idx = curRow;
                        break;
                    } else
                        curRow++;
                }
                else if (!gridProxi->IsEof(this))
                    LoadData(ids.size() + 100);
                else
                    break;
            }
        if (idx > -1)
        {
            dg->Row = idx;
            int newTop = dg->Row - (dg->ClientHeight / dg->DefaultRowHeight) / 2;
            dg->TopRow = newTop > 0 ? newTop : 0;
            res = true;
        }
    }
    return res;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::hdSectionResize(
      THeaderControl *HeaderControl, THeaderSection *Section)
{
    if (Section->Width < 1)
        Section->Width = 1;
    dg->ColWidths[Section->Index + dg->LeftCol] = Section->Width - 1;
    columnsInfo[Section->Index + dg->LeftCol].width = Section->Width;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::dgTopLeftChanged(TObject *Sender)
{
    // запомнить старую первую колонку, дабы не перерисовывать заголовок лишний раз
    if (oldLeftIdx != dg->LeftCol)
    {
        RecreateHeader();
        oldLeftIdx = dg->LeftCol;
    }
    // если сдвиг вверх-вниз, кешируем объекты списком
    LoadData(dg->ClientHeight / dg->DefaultRowHeight + dg->TopRow);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::RecreateHeader()
{
    hd->Sections->Clear();
    for (int i = 0; i < dg->ColCount - dg->LeftCol; i++)
    {
        THeaderSection *sect = hd->Sections->Add();
        int idx = i + dg->LeftCol;
        static String asterisk("* ");
        sect->Text = (columnsInfo[idx].fltrCond.IsEmpty() ? String() : asterisk) + columnsInfo[idx].title;
        sect->Width = columnsInfo[idx].width;
        sect->ImageIndex = columnsInfo[idx].imgIdx;
        sect->Alignment = taCenter;
    }
    #ifdef _DEBUG
    //    ShowMessage("TLisObjectGrid::RecreateHeader()");
    #endif
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::dgDblClick(TObject *Sender)
{
    if (m_caller)
        PickElement();
    else
        EditElement();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::dgKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (Key == VK_RETURN && Shift == TShiftState())
    {
        if (m_caller)
            PickElement();
        else
            EditElement();
    }

    TForm *owner = NULL;
    if (Key == VK_ESCAPE && Shift == TShiftState() && m_caller && (owner = dynamic_cast<TForm*>(Owner)))
        owner->Close();

    if (Key == VK_END)
    {
        while (gridProxi && !gridProxi->IsEof(this))
            LoadData(ids.size() + 1000);
    }
    if (Key >= '0' && Key <= '9' && Shift == TShiftState() << ssCtrl)
    {
        int idx = Key - '0';
        if (idx == 0)
            idx = 10;
        if (idx <= columnsInfo.size())
            hdSectionClick(hd, hd->Sections->Items[idx-1 - (columnsInfo.size() - hd->Sections->Count)]);
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::Clear()
{
    for (int i = 0; i < dg->ColCount; i++)
    {
        for (int j = 0; j < dg->RowCount; j++)
        {
            dg->Cells[i][j] = "";
            dg->Objects[i][j] = NULL;
        }
    }
    dg->RowCount = 0;
    ids.clear();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::Refresh()
{
    unsigned lastId = GetCurrentId();
    int oldLeft = dg->LeftCol;
    Clear();
    if (gridProxi)
        gridProxi->RunListQuery(this, sql);
    if(gridProxi && !gridProxi->IsEof(this))
        dg->RowCount = 1;
    dg->LeftCol = oldLeft;
    ids.clear();
    dgTopLeftChanged(this);
    SetCurrentRow(lastId);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::EditElement()
{
    int id = 0;
    if (gridProxi && (id = GetCurrentId()) > 0)
    {
        gridProxi->EditObject(this, id);
        Refresh();
        SetCurrentRow(id);
        //TODO: RefreshCurrentRow();
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::DeleteElement()
{
    int objId = GetCurrentId();
    if(!onlyFromList && gridProxi && objId)
        if (!gridProxi->DeleteObject(this, objId))
            return;

    if (dg->RowCount == 1)
    {
        for (int j = 0; j < dg->ColCount; j++)
        {
            dg->Cells[j][0] = "";
            dg->Objects[j][0] = NULL;
        }
    }
    else if (dg->Row < dg->RowCount - 1)
        for (int i = dg->Row; i < dg->RowCount - 1; i++)
            for (int j = 0; j < dg->ColCount; j++)
            {
                dg->Cells[j][i] = dg->Cells[j][i+1];
                dg->Objects[j][i] = dg->Objects[j][i+1];
            }
    if (ids.size() > dg->Row)
        ids.erase(&ids[dg->Row]);
    dg->RowCount = dg->RowCount - 1;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::PickElement()
{
    //TODO: notify caller
    //if (gridProxi)
    //    gridProxi->?????
    TForm* f = dynamic_cast<TForm*>(this->Owner);
    if (f)
    {
        f->ModalResult = mrOk;
        f->Hide();
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::ClearColumns()
{
    Clear();
    columnsInfo.clear();
    hd->Sections->Clear();
    dg->ColCount = 1;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::AddColumn(LisColumnInfo& newcol)
{
    columnsInfo.push_back(newcol);
    dg->ColCount = columnsInfo.size();
    dg->ColWidths[columnsInfo.size() - 1] = columnsInfo[columnsInfo.size() - 1].width - 1;
    RecreateHeader();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::AddColumn(AnsiString title, AnsiString prop, AnsiString fld, TAlignment am, LisPropType propType, int width)
{
    LisColumnInfo newcol;
    newcol.title = title;
    newcol.propName = prop;
    newcol.fldName = fld;
    newcol.am = am;
    newcol.propType = propType;
    newcol.width = width;

    AddColumn(newcol);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::dgDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
    if(columnsInfo.size() == 0)
        return;
    if (dg->Objects[ACol][ARow] == NULL)
        LoadData(ARow);
    AnsiString text = dg->Cells[ACol][ARow];
    //TODO: set font, color
    dg->Canvas->Font->Style = columnsInfo[ACol].fontStyle;
    bool draw = true;
    if (gridProxi)
        gridProxi->FormatCanvas(this, ACol, ARow, dg->Canvas, draw);
    if (columnsInfo.size() > 0 && text.Length() > 0 && draw)
    {
        if (dg->Objects[ACol][ARow] == reinterpret_cast<TObject*>(2)) // if no such property...
            dg->Canvas->Font->Color = clDkGray;

        int offset = 2;
        TAlignment am = columnsInfo[ACol].am;
        if (am != taLeftJustify)
        {
            offset = Rect.Width() - 2 - dg->Canvas->TextWidth(text); //taRightJustify
            if (am == taCenter)
                offset /= 2;
        }
        dg->Canvas->TextRect(Rect, Rect.left + offset, Rect.top + 1, text);
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::hdSectionClick(
      THeaderControl *HeaderControl, THeaderSection *Section)
{
    Sort(Section->Index + (columnsInfo.size() - hd->Sections->Count));
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::FrameResize(TObject *Sender)
{
    // дорисовать
    dgTopLeftChanged(this);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::LoadData(int lastRow)
{
    if (gridProxi == NULL)
        return;

    while (!gridProxi->IsEof(this) && ids.size() < lastRow + 1)
    {
        long id = gridProxi->GetId(this, ids.size());
        ids.push_back(id);
        int curRow = ids.size()-1;
        for (int i = 0; i < columnsInfo.size(); i++)
        {
            String text;
            try {
                text = gridProxi->GetVal(this, curRow, columnsInfo[i]);
                dg->Objects[i][curRow] = (TObject*)1;
            } catch (Exception& e) {
                text = e.Message;
                dg->Objects[i][curRow] = (TObject*)2;
            }
            dg->Cells[i][curRow] = text;
        }

        if (!gridProxi->Next(this) && dg->RowCount <= ids.size())
            dg->RowCount = ids.size()+1;
    }
}
//---------------------------------------------------------------------------

void TLisObjectGrid::SetOrderBy(AnsiString orderStr, AnsiString fldName)
{
    if(sql.Trim().IsEmpty())
        return;

    fldName = GetFieldSpecFromAlias(fldName);
    if (fldName.IsEmpty())
        MessageBox(NULL, ("Source of field '"+fldName+"' is not found").c_str(), "Warning", MB_ICONHAND);
    else
    {
        String new_sql = sql;
        int pos = sql.Pos(" order by ") - 1;
        if(pos > 0)
            new_sql = sql.SubString(1, pos);

        sql = new_sql + (orderStr.Trim().IsEmpty() || fldName.Trim().IsEmpty() ?
                        String("") : String(" order by " + fldName + " "+orderStr));
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actCalcRowsExecute(TObject *Sender)
{
    //возвращаем количество строк
    String res;
    if (gridProxi)
    {
        AnsiString old_query = sql.UpperCase();
        AnsiString calcQry = old_query.SubString(0, old_query.Pos("SELECT ")+6);
        if (old_query.Pos("T1") > 0)
            calcQry += "count(t1.id) ";
        else
            calcQry += "count(id) ";
        if(old_query.Pos("ORDER BY") > 0)
        {
            calcQry = calcQry + old_query.SubString(old_query.Pos("FROM"), old_query.Pos("ORDER BY")-old_query.Pos("FROM"))+"\"";
        }
        else
        {
            calcQry += old_query.SubString(old_query.Pos("FROM"), old_query.Length()-old_query.Pos("FROM")+1);
        }
        res = gridProxi->RunQuery(this, calcQry);
    }
    if (res.Length() > 0)
        try { dg->RowCount = res.ToInt(); } catch (...) {}
    else
        dg->RowCount = 0;
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actEditExecute(TObject *Sender)
{
    EditElement();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actSelectExecute(TObject *Sender)
{
    if (m_caller)
        PickElement();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actRefreshExecute(TObject *Sender)
{
    Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actNewExecute(TObject *Sender)
{
    int id = gridProxi->CreateObject(this);
    Refresh();
    SetCurrentRow(id);
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actDeleteExecute(TObject *Sender)
{
    if (GetCurrentId() > 0)
    {
        DeleteElement();
        actRefreshExecute(Sender);
        //actCalcRowsExecute(Sender);
    }
}
//---------------------------------------------------------------------------

void __fastcall TLisObjectGrid::actCopyExecute(TObject *Sender)
{
    if (GetCurrentId() > 0)
    {
        int id = gridProxi->CopyObject(this, GetCurrentId());
        Refresh();
        SetCurrentRow(id);
    }
}
//---------------------------------------------------------------------------

unsigned __fastcall TLisObjectGrid::GetCurrentId()
{
    unsigned id = 0;
    if (dg->RowCount > 0 && ids.size() > dg->Row)
        id = (unsigned)ids[dg->Row];
    //todo: throw an exception
    return id;
}
//---------------------------------------------------------------------------

String __fastcall TLisObjectGrid::GetValue(String colName)
{
    colName = colName.UpperCase();
    for (int i = 0; i < columnsInfo.size(); i++)
        if (columnsInfo[i].fldName.UpperCase() == colName)
            return dg->Cells[i][dg->Row];

    return String();
}

String __fastcall TLisObjectGrid::GetFieldSpecFromAlias(String alias)
{
    String result = sql.UpperCase();
    int pos = result.Pos(" ORDER BY ");
    if (pos > 0)
        result.Delete(pos, result.Length() - pos + 1);
        
    pos = 0;
    // name string could appear several times in query string.
    // search for that one wich is trimmed with delimiters
    bool notFound = true;
    #define islex(c) (c != ' ' && c != '\n' && c != ',' && c != ';')
    while (notFound)
    {
        int f = result.Pos(alias);
        if (f == 0)
            break;
        int b = f + alias.Length();
        if (f > 1 && islex(result[f-1]) || b <= result.Length() && islex(result[b]))
        {
            result.Delete(1, b-1);
            pos += b-1;
        } else {
            notFound = false;
            pos += f-1;
        }
    }

    if (!notFound)
    {
        result = sql;
        while (--pos > 0 && !islex(result[pos]))
            ;
        result.Delete(pos+1, result.Length()-pos+1);
        while (--pos > 0 && islex(result[pos]))
            ;
        result.Delete(1, pos);
    } else if (gridProxi)
        result = gridProxi->GetFieldSpecFromAlias(this, alias);
    #undef islex

    return result;
}

void __fastcall TLisObjectGrid::Sort(int colIdx)
{
    int idxOff = columnsInfo.size() - hd->Sections->Count;
    for (int i = 0; i < hd->Sections->Count; i++)
        if (i != colIdx - idxOff)
        {
            hd->Sections->Items[i]->ImageIndex = -1;
            columnsInfo[colIdx].imgIdx = -1;
        }

    THeaderSection *Section = hd->Sections->Items[colIdx - idxOff];

    int newIdx = Section->ImageIndex + 1;
    if (newIdx > 1)
        newIdx = -1;
    if (columnsInfo[colIdx].customSort)
        gridProxi->SortGrid(this, colIdx);
    else
        SetOrderBy(newIdx == 1 ? "DESC" : newIdx == -1 ? "" : /*0*/"ASC", columnsInfo[colIdx].fldName);

    Section->ImageIndex = newIdx;
    columnsInfo[colIdx].imgIdx = newIdx;
    Refresh();
}

void __fastcall TLisObjectGrid::RefreshRow(int row)
{
    for (int i = 0; i < columnsInfo.size(); i++)
    {
        String text;
        try {
            text = gridProxi->GetVal(this, row, columnsInfo[i]);
            dg->Objects[i][row] = (TObject*)1;
        } catch (Exception& e) {
            text = e.Message;
            dg->Objects[i][row] = (TObject*)2;
        }
        dg->Cells[i][row] = text;
    }
}

std::vector<unsigned> __fastcall TLisObjectGrid::GetSelectedIdList()
{
    std::vector<unsigned> idList;
    for (int i = dg->Selection.Top; i <= dg->Selection.Bottom; i++)
        idList.push_back(ids[i]);
    return idList;
}
//---------------------------------------------------------------------------

