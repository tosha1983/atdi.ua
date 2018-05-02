//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uContourForm.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"

#include <IBSQL.hpp>
#include <memory>
#include <math.h>
#include <values.h>
#include "CustomMap.h"

//---------------------------------------------------------------------------
__fastcall TfrmContour::TfrmContour(TComponent* Owner)
    : TForm(Owner)
{
    grdContour->ColWidths[0] = 25;
    grdContour->Cells[0][0] = "№";
    grdContour->Cells[1][0] = "Довгота";
    grdContour->Cells[2][0] = "Широта";

    cmf = new TCustomMapFrame(this);
    cmf->Parent = panMap;
    cmf->ParentWindow = panMap;
    cmf->Align = alClient;
    cmf->Visible = true;
    cmf->Init();
    cmf->ColorStationUnH = clRed;
    cmf->ColorStationH = clBlue;
    oldMouseDown = cmf->bmf->Map->OnMouseDown;
    oldMouseUp = cmf->bmf->Map->OnMouseUp;
    oldMouseMove = cmf->bmf->Map->OnMouseMove;
    cmf->bmf->Map->OnMouseDown = MapOnMouseDown;
    cmf->bmf->Map->OnMouseUp = MapOnMouseUp;
    cmf->bmf->Map->OnMouseMove = MapOnMouseMove;
    cmf->omsCallBack = OnObjectSelection;

    cmf->bmf->SetLayerSelectable(0, false);
    cmf->bmf->SetLayerSelectable(1, false);
    cmf->bmf->SetLayerSelectable(2, true);

    cmf->tb->Visible = false;
    cmf->bmf->actPanButtons->Checked = false;
    cmf->bmf->actPanButtonsExecute(this);

    tbtZoomIn->OnClick = cmf->bmf->actZoomInTwiceExecute;
    tbtZoomOut->OnClick = cmf->bmf->actZoomOutTwiceExecute;
    tbtLeft->OnClick = cmf->bmf->btnL->OnClick;
    tbtRight->OnClick = cmf->bmf->btnR->OnClick;
    tbtUp->OnClick = cmf->bmf->btnU->OnClick;
    tbtDown->OnClick = cmf->bmf->btnD->OnClick;

    std::auto_ptr<TIBSQL> sql (new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = " select CODE from COUNTRY order by CODE ";
    cbxCountry->Items->Clear();
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
        cbxCountry->Items->Add(sql->Fields[0]->AsString);

    changed = false;
    id = 0;

    contour.clear();
}
//---------------------------------------------------------------------------
void __fastcall TfrmContour::FormShow(TObject *Sender)
{
    actLoadExecute(Sender);
}

void __fastcall TfrmContour::actOkExecute(TObject *Sender)
{
    if (actSave->Enabled)
        actSaveExecute(Sender);
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmContour::actSaveExecute(TObject *Sender)
{
    if (GetDbSection() != tsDraft)
    {
        throw *(new Exception("Изменить контур можно только в Предбазе"));
    }

    sqlContour->Close();
    sqlContour->SQL->Text = "update DIG_CONTOUR set CONTOUR_ID = :CONTOUR_ID, CTRY = :CTRY, NB_TEST_PTS = :NB_TEST_PTS "
                            "where ID = :ID ";
    sqlContour->ParamByName("CONTOUR_ID")->AsString = contourId;
    sqlContour->ParamByName("CTRY")->AsString = ctry;
    sqlContour->ParamByName("ID")->AsInteger = id;
    sqlContour->ParamByName("NB_TEST_PTS")->AsInteger = contour.size();
    sqlContour->ExecQuery();

    sqlContour->Close();
    sqlContour->SQL->Text = "delete  from DIG_SUBAREAPOINT where CONTOUR_ID = :ID ";
    sqlContour->ParamByName("ID")->AsInteger = id;
    sqlContour->ExecQuery();
    sqlContour->Close();
    sqlContour->SQL->Text = "insert into DIG_SUBAREAPOINT (CONTOUR_ID, POINT_NO, LAT, LON) "
                                                  "values (:ID, :POINT_NO, :LAT, :LON)";
    for (int i = 0; i < contour.size(); i++)
    {
        sqlContour->ParamByName("ID")->AsInteger = id;
        sqlContour->ParamByName("POINT_NO")->AsInteger = i+1;
        sqlContour->ParamByName("LON")->AsDouble = contour[i].first;
        sqlContour->ParamByName("LAT")->AsDouble = contour[i].second;
        sqlContour->ExecQuery();
        sqlContour->Close();
    }
    sqlContour->Transaction->CommitRetaining();

    changed = false;

}
//---------------------------------------------------------------------------
void __fastcall TfrmContour::actLoadExecute(TObject *Sender)
{
    sqlContour->Close();
    sqlContour->SQL->Text = "select cy.CODE, CONTOUR_ID, CTRY, c.ADM_ID from DIG_CONTOUR c "
                            "left outer join COUNTRY cy on (c.ADM_ID = cy.ID) "
                            "where c.ID = :ID ";
    sqlContour->ParamByName("ID")->AsInteger = id;
    sqlContour->ExecQuery();
    if (sqlContour->Eof)
        throw *(new Exception(AnsiString().sprintf("Контур с ID = %d не обнаружен в БД", id)));
    edtAdmin->Text = sqlContour->FieldByName("CODE")->AsString;
    contourId = sqlContour->FieldByName("CONTOUR_ID")->AsString;
    ctry = sqlContour->FieldByName("CTRY")->AsString;
    int admId = sqlContour->FieldByName("ADM_ID")->AsInteger;

    existingPoints.clear();
    sqlContour->Close();
    sqlContour->SQL->Text = "select p.LAT, p.LON, c.CONTOUR_ID from DIG_SUBAREAPOINT p "
                            "left join DIG_CONTOUR c on (c.ID = p.CONTOUR_ID) "
                            "where p.CONTOUR_ID <> :ID and c.ADM_ID = :ADM_ID order by c.CONTOUR_ID, p.POINT_NO ";
    sqlContour->ParamByName("ID")->AsInteger = id;
    sqlContour->ParamByName("ADM_ID")->AsInteger = admId;
    int cId = -1;
    cmf->Clear(-1);
    Lis_map::Points pts;
    for (sqlContour->ExecQuery(); !sqlContour->Eof; sqlContour->Next())
    {
        Lis_map::Point p(sqlContour->FieldByName("LON")->AsDouble, sqlContour->FieldByName("LAT")->AsDouble);
        int curId = sqlContour->FieldByName("CONTOUR_ID")->AsInteger;
        if (cId != curId)
        {
            if (pts.size() > 0)
            {
                AnsiString name = IntToStr(cId);
                MapPolygon *pgn = cmf->ShowContour(pts, name, name);
                pgn->color = clGray;
                pgn->SetLayer(0);
                pts.clear();
            }
            cId = curId;
        }
        pts.push_back(p);
        existingPoints.push_back(p);
    }
    if (pts.size() > 0)
    {
        AnsiString name = IntToStr(cId);
        MapPolygon *pgn = cmf->ShowContour(pts, "", "");
        pgn->color = clGray;
        pgn->SetLayer(0);
        pts.clear();
    }

    sqlContour->Close();
    sqlContour->SQL->Text = "select LAT, LON  from DIG_SUBAREAPOINT where CONTOUR_ID = :ID order by POINT_NO";
    sqlContour->ParamByName("ID")->AsInteger = id;
    contour.clear();
    for (sqlContour->ExecQuery(); !sqlContour->Eof; sqlContour->Next())
        contour.push_back(Lis_map::Point(sqlContour->FieldByName("LON")->AsDouble, sqlContour->FieldByName("LAT")->AsDouble));
    ShowData();

    cmf->bmf->SetLayerVisible(0, false);
    cmf->bmf->FitObjects();
    cmf->bmf->SetLayerVisible(0, true);
    cmf->bmf->Refresh();

    changed = false;
}
//---------------------------------------------------------------------------
void __fastcall TfrmContour::actCloseExecute(TObject *Sender)
{
    Close();
}
//---------------------------------------------------------------------------
void __fastcall TfrmContour::actSaveUpdate(TObject *Sender)
{
    actSave->Enabled = changed;
    actLoad->Enabled = changed;
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::ShowData()
{
    edtItuId->Text = contourId;

    cbxCountry->ItemIndex = cbxCountry->Items->IndexOf(ctry);
    // contour
    grdContour->RowCount = contour.size() + 1;
    if (grdContour->RowCount == 1)
        grdContour->RowCount = 0;
    if (grdContour->RowCount > 1)
        grdContour->FixedRows = 1;
    for (int i = 0; i < contour.size(); i++)
    {
        grdContour->Cells[0][i+1] = i+1;
        grdContour->Cells[1][i+1] = dmMain->coordToStr(contour[i].first, 'X');
        grdContour->Cells[2][i+1] = dmMain->coordToStr(contour[i].second, 'Y');
    }
    ShowContour();
}

void __fastcall TfrmContour::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::tbtAddPointClick(TObject *Sender)
{
    if (grdContour->RowCount < 100)
    {
        if (contour.size() == 0)
        {
            AddPointEx(cmf->bmf->ClientWidth / 2 - 35, cmf->bmf->ClientHeight / 2 - 60);
            AddPointEx(cmf->bmf->ClientWidth / 2 - 35, cmf->bmf->ClientHeight / 2 - 60);
            AddPointEx(cmf->bmf->ClientWidth / 2 - 75, cmf->bmf->ClientHeight / 2);
            AddPointEx(cmf->bmf->ClientWidth / 2 - 35, cmf->bmf->ClientHeight / 2 + 60);
            AddPointEx(cmf->bmf->ClientWidth / 2 + 35, cmf->bmf->ClientHeight / 2 + 60);
            AddPointEx(cmf->bmf->ClientWidth / 2 + 75, cmf->bmf->ClientHeight / 2);
            AddPointEx(cmf->bmf->ClientWidth / 2 + 35, cmf->bmf->ClientHeight / 2 - 60);

            ShowContour();
        }
        else
        {
            cmf->bmf->Cursor = crCross;
            lblHint->Visible = true;
        }
    } else
        throw *(new Exception("Максимальное число точек - 99"));
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::tbtDelPointClick(TObject *Sender)
{
    if (grdContour->RowCount > 1 && MessageBox(NULL, "Удалить точку?", "Подтверждение", MB_YESNO) == IDYES)
    {
        contour.erase(&(contour[grdContour->Selection.Top - 1]));
        grdContour->RowCount = grdContour->RowCount - 1;
        if (grdContour->RowCount == 1)
            grdContour->RowCount = 0;
        for (int i = grdContour->Selection.Top; i < grdContour->RowCount - 1; i++)
        {
            grdContour->Cells[0][i] = i;
            grdContour->Cells[1][i] = dmMain->coordToStr(contour[i].first, 'X');
            grdContour->Cells[2][i] = dmMain->coordToStr(contour[i].second, 'Y');
        }
        changed = true;
        ShowContour();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::grdContourSetEditText(TObject *Sender,
      int ACol, int ARow, const AnsiString Value)
{
    Lis_map::Point p = contour[ARow - 1];
    if (ACol == 1)
        p.first = dmMain->strToCoord(Value);
    else
        p.second = dmMain->strToCoord(Value);
    if (contour[ARow - 1].first != p.first || contour[ARow - 1].second != p.second)
    {
        contour[ARow - 1] = p;
        changed = true;
        ShowContour();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::edtItuIdExit(TObject *Sender)
{
    if (contourId != edtItuId->Text)
    {
        contourId = edtItuId->Text;
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::cbxCountryChange(TObject *Sender)
{
    if (ctry != cbxCountry->Text)
    {
        ctry = cbxCountry->Text;
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::ShowContour()
{
    cmf->Clear(1);
    cmf->Clear(2);

    for (int i = 0 ; i < contour.size(); i++)
    {
        MapPoint *pnt = cmf->ShowPoint(contour[i].first, contour[i].second,
                        (i == selectedPoint) ? cmf->ColorStationH : cmf->ColorStationUnH,
                        5, ptPoint, IntToStr(i), IntToStr(i));
        pnt->userTag = cotStation;
        pnt->SetLayer(2);
    }
    MapPolygon *pgn = cmf->ShowContour(contour, edtItuId->Text, edtItuId->Text);
    pgn->color = clLime;
    pgn->width = 2;
    pgn->SetLayer(1);

    cmf->bmf->Map->Refresh();
}

void __fastcall TfrmContour::OnObjectSelection(TObject * Sender, TBaseMapFrame::Shapes shapes, TMouseButton Button, TShiftState Shift)
{
    cmf->UnHighlihtAll();
    if (shapes.size() > 0)
    {
        MapPoint *p = dynamic_cast<MapPoint*>(shapes[0]);
        if (p)
        {
            for( int i = 0; i < contour.size()-1; i++ )
                if (contour[i].first == p->x && contour[i].second == p->y)
                {
                    cmf->Highlight(p->GetId());
                    cmf->bmf->Map->Refresh();
                    selectedPoint = i;
                    dragging = true;
                    break;
                }
        }
    }
}

void __fastcall TfrmContour::MapOnMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (oldMouseDown)
        oldMouseDown(Sender, Button, Shift, X, Y);

    if (lblHint->Visible)
    {
        double x = 0.0;
        double y = 0.0;
        cmf->bmf->CoordScreenToMap(X, Y, &x, &y);
        AddPoint(x, y);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::MapOnMouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y)
{
    if (oldMouseMove)
        oldMouseMove(Sender, Shift, X, Y);

    double x = 0.0;
    double y = 0.0;
    cmf->bmf->CoordScreenToMap(X, Y, &x, &y);

    if ( dragging )
    {
        if ( selectedPoint >= 0 )
        {
            // find nearest point and stick current one to it
            if (tbtNearestPoint->Down)
            {
                int idx = -1;
                double minDist = 0;
                for (int i = 0; i < existingPoints.size(); i++)
                {
                    // upper limit = at least 10 points
                    int xPos, yPos;
                    cmf->bmf->CoordMapToScreen(existingPoints[i].first, existingPoints[i].second, &xPos, &yPos);
                    if (abs(xPos - X) <= 10 && abs(yPos - Y) <= 10)
                    {
                        double dist = cmf->bmf->Map->Distance(x, y, existingPoints[i].first, existingPoints[i].second);
                        if (idx == -1 || dist < minDist)
                        {
                            idx = i;
                            minDist = dist;
                        }
                    }
                }

                if (idx != -1)
                {
                    x = existingPoints[idx].first;
                    y = existingPoints[idx].second;
                }
            }

            if ( selectedPoint == 0 )
            {
                contour[contour.size()-1].first = x;
                contour[contour.size()-1].second = y;
                grdContour->Cells[0][contour.size()] = contour.size();
                grdContour->Cells[1][contour.size()] = dmMain->coordToStr(x, 'X');
                grdContour->Cells[2][contour.size()] = dmMain->coordToStr(y, 'Y');
            }

            contour[selectedPoint].first = x;
            contour[selectedPoint].second = y;
            grdContour->Cells[0][selectedPoint+1] = selectedPoint+1;
            grdContour->Cells[1][selectedPoint+1] = dmMain->coordToStr(x, 'X');
            grdContour->Cells[2][selectedPoint+1] = dmMain->coordToStr(y, 'Y');

            ShowContour();
            changed = true;
        }
    }
    else
    {
        int selectedPoint = -1;

        for( int i = 0; i < contour.size(); i++ )
        {
            int x = 0.0;
            int y = 0.0;
            cmf->bmf->CoordMapToScreen(contour[i].first, contour[i].second, &x, &y);
            if ((abs(X - x) < 1) && (abs(Y - y) < 1))
            {
                selectedPoint = i;
                break;
            }
        }
        if ( selectedPoint >= 0 )
            cmf->bmf->Map->Cursor = crSizeAll;
        else
            cmf->bmf->Map->Cursor = crDefault;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::MapOnMouseUp(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    if (oldMouseUp)
        oldMouseUp(Sender, Button, Shift, X, Y);

    dragging = false;
}

void __fastcall TfrmContour::grdContourKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    selectedPoint = grdContour->Row - 1;
    ShowContour();
}
//--------------------------------------------------------------------------

void __fastcall TfrmContour::grdContourMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    selectedPoint = grdContour->Row - 1;
    ShowContour();
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::tbtFitClick(TObject *Sender)
{
    cmf->bmf->SetLayerVisible(0, false);
    cmf->bmf->FitObjects();
    cmf->bmf->SetLayerVisible(0, true);
    cmf->bmf->Refresh();
}
//---------------------------------------------------------------------------

void __fastcall TfrmContour::AddPointEx(int x, int y)
{
    double X = 0.0;
    double Y = 0.0;
    cmf->bmf->CoordScreenToMap(x, y, &X, &Y);
    AddPoint(X, Y);
}

void __fastcall TfrmContour::AddPoint(double x, double y)
{
    Lis_map::Point p(x, y);
    contour.insert(&(contour[grdContour->Row]), p);
    lblHint->Visible = false;
    cmf->bmf->Map->Cursor = crCross;

    ShowData();
    changed = true;
}

int __fastcall TfrmContour::GetDbSection()
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select DB_SECTION_ID from DIG_CONTOUR where ID = " + IntToStr(id);
    sql->ExecQuery();
    if (sql->Eof)
        return tsBase;
    else
        return sql->Fields[0]->AsInteger;
}
//---------------------------------------------------------------------------



