//$$---- Active Form CPP ---- (stActiveFormSource)
//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
#include <atl\atlvcl.h>

#include "LisMapImpl.h"
#include "uAnalyzer.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CustomMap"
#pragma resource "*.dfm"
TLisMapX *LisMapX;
//---------------------------------------------------------------------------
__fastcall TLisMapX::TLisMapX(HWND ParentWindow)
   : TActiveForm(ParentWindow)
{
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowStation(double lon, double lat, AnsiString label, AnsiString hint)
{
   cmf->ShowStation(lon, lat, label, hint);
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::CloseMap()
{
   cmf->bmf->ClearData(-1);
   delete cmf;
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::FitObjects()
{
   cmf->bmf->FitObjects();
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowLink(double lon1, double lat1, double lon2, double lat2, int trim, MapArrowType mat)
{
   /*MapLink* pgn = */cmf->ShowLink(lon1, lat1, lon2, lat2, trim, matArrow);
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowCoordZone(double centLon, double centLat, LPSAFEARRAY zone)
{
   std::vector<double> z;
   for (unsigned int i = 0; i < zone->rgsabound[0].cElements; i++)
            z.push_back(((double*)zone->pvData)[i]);
   /*MapPolygon* pgn = */cmf->ShowCoverageZone(centLon, centLat, z);
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowPoint(double lon, double lat, int colour, int width, PointType type, BSTR label, BSTR hint)
{
   /*MapPoint* p = */cmf->ShowPoint(lon, lat, colour, width, type, label, hint);
}
//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowPointEx(double lon, double lat, PointType type, int pointWidth,
                                BSTR label, BSTR hint, OLE_COLOR pointColour, BSTR fontName, int fontSize, OLE_COLOR textColour,
                                VARIANT_BOOL isBold, VARIANT_BOOL isItalic, VARIANT_BOOL isUnderline)
{
    MapPoint* p = cmf->ShowPoint(lon, lat, pointColour, pointWidth, type, label, hint);
    p->SetNewFont(fontName, fontSize, VariantBoolToBool(isBold),
                  VariantBoolToBool(isItalic), VariantBoolToBool(isUnderline), textColour);
}
void __fastcall TLisMapX::ShowStation(double lon, double lat, BSTR label, BSTR hint)
{
   /*MapPoint* mp = */cmf->ShowStation(lon, lat, label, hint);
}
//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowStationEx(double lon, double lat, BSTR label, BSTR hint,
                                  BSTR fontName, int fontSize, OLE_COLOR textColour,
                                  VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
                                  VARIANT_BOOL isUnderline)
{
   MapShape* p = cmf->ShowStation(lon, lat, label, hint);
   p->SetNewFont(fontName, fontSize, VariantBoolToBool(isBold),
                 VariantBoolToBool(isItalic), VariantBoolToBool(isUnderline), textColour);
}
//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowContourEx(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label, BSTR hint,
                                  BSTR fontName, int fontSize, OLE_COLOR textColour,
                                  VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
                                  VARIANT_BOOL isUnderline)
{
   DPoints pts;
   DPoint pnt;
   for (unsigned int i = 0; i < lon->rgsabound[0].cElements; i++)
   {
            pnt.first  = ((double*)lon->pvData)[i];
            pnt.second = ((double*)lat->pvData)[i];
            pts.push_back(pnt);
   }
   MapShape* p = cmf->ShowContour(pts, label, hint);
   p->SetNewFont(fontName, fontSize, VariantBoolToBool(isBold),
                 VariantBoolToBool(isItalic), VariantBoolToBool(isUnderline), textColour);
}
//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowLabelEx(double lon, double lat, BSTR label, BSTR hint,
                                BSTR fontName, int fontSize, OLE_COLOR textColour,
                                VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
                                VARIANT_BOOL isUnderline)
{
   ShowPointEx(lon, lat, ptPoint, 0, label, hint, 0, fontName, fontSize, textColour,
               isBold, isItalic, isUnderline);
}


//---------------------------------------------------------------------------
void __fastcall TLisMapX::Refresh()
{
   cmf->bmf->Map->Refresh();
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowContour(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label, BSTR hint)
{
   DPoints pts;
   DPoint pnt;
   for (unsigned int i = 0; i < lon->rgsabound[0].cElements; i++)
   {
            pnt.first  = ((double*)lon->pvData)[i];
            pnt.second = ((double*)lat->pvData)[i];
            pts.push_back(pnt);
   }
   /*MapPolygon *pgn = */cmf->ShowContour(pts, label, hint);
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::ShowZone(double centLon, double centLat, LPSAFEARRAY zone,
                              int width, int color, int style, int objType, int layer)
{
   std::vector<double> z;
   for (unsigned int i = 0; i < zone->rgsabound[0].cElements; i++)
            z.push_back(((double*)zone->pvData)[i]);
   /*MapPolygon *pgn = */cmf->ShowZone(centLon, centLat, z,
                            width, color, (TPenStyle)style, (CustomObjectType)objType, layer);
}

//---------------------------------------------------------------------------
void __fastcall TLisMapX::Init()
{
   try {
            cmf->Init();
            cmf->bmf->OnToolUsed  = txAnalyzer.MapToolUsed;
        } catch (...) {}
}
//---------------------------------------------------------------------------

void __fastcall TLisMapX::SetCenter(double lon, double lat)
{
   cmf->SetCenter(lon, lat);
}
//---------------------------------------------------------------------------

void __fastcall TLisMapX::SetScale(double scale)
{
    cmf->SetScale(scale);
}
//---------------------------------------------------------------------------

void __fastcall TLisMapX::Clear(int layer, bool refresh)
{
   bool refr = (refresh == 0)? 0 : 1;
   cmf->Clear(layer, refr);
}
//---------------------------------------------------------------------------

STDMETHODIMP TLisMapXImpl::_set_Font(IFontDisp** Value)
{
  try
  {
    const DISPID dispid = -512;
    if (FireOnRequestEdit(dispid) == S_FALSE)
      return S_FALSE;
    SetVclCtlProp(m_VclCtl->Font, Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Active(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->Active;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_AlignDisabled(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->AlignDisabled;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_AutoScroll(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->AutoScroll;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_AutoSize(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->AutoSize;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_AxBorderStyle(
  TxActiveFormBorderStyle* Value)
{
  try
  {
   *Value = (TxActiveFormBorderStyle)(m_VclCtl->AxBorderStyle);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_BorderWidth(long* Value)
{
  try
  {
   *Value = (long)(m_VclCtl->BorderWidth);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Caption(BSTR* Value)
{
  try
  {
    *Value = WideString(m_VclCtl->Caption).Copy();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Color(::OLE_COLOR* Value)
{
  try
  {
   *Value = (::OLE_COLOR)(m_VclCtl->Color);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_DoubleBuffered(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->DoubleBuffered;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_DropTarget(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->DropTarget;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Enabled(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->Enabled;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Font(IFontDisp** Value)
{
  try
  {
    GetVclCtlProp(m_VclCtl->Font, Value);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_HelpFile(BSTR* Value)
{
  try
  {
    *Value = WideString(m_VclCtl->HelpFile).Copy();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_KeyPreview(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->KeyPreview;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_PixelsPerInch(long* Value)
{
  try
  {
   *Value = (long)(m_VclCtl->PixelsPerInch);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_PrintScale(TxPrintScale* Value)
{
  try
  {
   *Value = (TxPrintScale)(m_VclCtl->PrintScale);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Scaled(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->Scaled;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_Visible(VARIANT_BOOL* Value)
{
  try
  {
   *Value = m_VclCtl->Visible;
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::get_VisibleDockClientCount(long* Value)
{
  try
  {
   *Value = (long)(m_VclCtl->VisibleDockClientCount);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_AutoScroll(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 2;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->AutoScroll = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_AutoSize(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 3;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->AutoSize = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_AxBorderStyle(TxActiveFormBorderStyle Value)
{
  try
  {
    const DISPID dispid = 4;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->AxBorderStyle = (TActiveFormBorderStyle)(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_BorderWidth(long Value)
{
  try
  {
    const DISPID dispid = 5;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->BorderWidth = (int)(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_Caption(BSTR Value)
{
  try
  {
    const DISPID dispid = -518;
    if (FireOnRequestEdit(dispid) == S_FALSE)
      return S_FALSE;
    m_VclCtl->Caption = AnsiString(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_Color(::OLE_COLOR Value)
{
  try
  {
    const DISPID dispid = -501;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->Color = (TColor)(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_DoubleBuffered(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 13;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->DoubleBuffered = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_DropTarget(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 11;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->DropTarget = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_Enabled(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = -514;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->Enabled = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_Font(IFontDisp* Value)
{
  try
  {
    const DISPID dispid = -512;
    if (FireOnRequestEdit(dispid) == S_FALSE)
      return S_FALSE;
    SetVclCtlProp(m_VclCtl->Font, Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_HelpFile(BSTR Value)
{
  try
  {
    const DISPID dispid = 12;
    if (FireOnRequestEdit(dispid) == S_FALSE)
      return S_FALSE;
    m_VclCtl->HelpFile = AnsiString(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_KeyPreview(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 6;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->KeyPreview = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_PixelsPerInch(long Value)
{
  try
  {
    const DISPID dispid = 7;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->PixelsPerInch = (int)(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_PrintScale(TxPrintScale Value)
{
  try
  {
    const DISPID dispid = 8;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->PrintScale = (TPrintScale)(Value);
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_Scaled(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 9;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->Scaled = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::set_Visible(VARIANT_BOOL Value)
{
  try
  {
    const DISPID dispid = 1;
    if (FireOnRequestEdit(dispid) == S_FALSE)
     return S_FALSE;
    m_VclCtl->Visible = Value;
    FireOnChanged(dispid);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



void __fastcall TLisMapXImpl::ActivateEvent(TObject *Sender)
{
  Fire_OnActivate();
}


void __fastcall TLisMapXImpl::ClickEvent(TObject *Sender)
{
  Fire_OnClick();
}


void __fastcall TLisMapXImpl::CreateEvent(TObject *Sender)
{
  Fire_OnCreate();
}


void __fastcall TLisMapXImpl::DblClickEvent(TObject *Sender)
{
  Fire_OnDblClick();
}


void __fastcall TLisMapXImpl::DeactivateEvent(TObject *Sender)
{
  Fire_OnDeactivate();
}


void __fastcall TLisMapXImpl::DestroyEvent(TObject *Sender)
{
  Fire_OnDestroy();
}


void __fastcall TLisMapXImpl::KeyPressEvent(TObject *Sender, char &Key)
{
  short TempKey;
  TempKey = (short)Key;
  Fire_OnKeyPress(&TempKey);
  Key = (short)TempKey;
}


void __fastcall TLisMapXImpl::PaintEvent(TObject *Sender)
{
  Fire_OnPaint();
}



STDMETHODIMP TLisMapXImpl::Init()
{
  try
  {
     m_VclCtl->Init();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::Show()
{
  try
  {
  m_VclCtl->Show();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::SetCenter(double lon, double lat)
{
  try
  {
     m_VclCtl->SetCenter(lon, lat);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};

 
STDMETHODIMP TLisMapXImpl::SetScale(double scale)
{
  try
  {
      m_VclCtl->SetScale(scale);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::Clear(int layer, long refresh)
{
  try
  {
      m_VclCtl->Clear(layer, refresh);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::ShowCoordZone(double centLon, double centLat,
  LPSAFEARRAY zone)
{
  try
  {
      m_VclCtl->ShowCoordZone(centLon, centLat,  zone);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::ShowPoint(double lon, double lat, int colour,
  int width, PointType type, BSTR label, BSTR hint)
{
  try
  {
     m_VclCtl->ShowPoint(lon, lat, colour, width, type, label, hint);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::ShowPointEx(double lon, double lat, PointType type, int pointWidth,
                                BSTR label, BSTR hint, OLE_COLOR pointColour, BSTR fontName, int fontSize, OLE_COLOR textColour,
                                VARIANT_BOOL isBold, VARIANT_BOOL isItalic, VARIANT_BOOL isUnderline)
{
  try
  {
     m_VclCtl->ShowPointEx(lon, lat, type, pointWidth, label, hint, pointColour, fontName,
                         fontSize, textColour, isBold, isItalic, isUnderline);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};

STDMETHODIMP TLisMapXImpl::ShowStation(double lon, double lat, BSTR label,
  BSTR hint)
{
  try
  {
      m_VclCtl->ShowStation(lon, lat, label, hint);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};

STDMETHODIMP TLisMapXImpl::ShowStationEx(double lon, double lat, BSTR label, BSTR hint,
      BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
      VARIANT_BOOL isUnderline)
{
  try
  {
      m_VclCtl->ShowStationEx(lon, lat, label, hint,
                              fontName, fontSize, textColour,
                              isBold, isItalic, isUnderline);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



STDMETHODIMP TLisMapXImpl::refresh()
{
  try
  {
      m_VclCtl->Refresh();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};


STDMETHODIMP TLisMapXImpl::ShowContour(LPSAFEARRAY lon, LPSAFEARRAY lat,
  BSTR label, BSTR hint)
{
  try
  {
       m_VclCtl->ShowContour(lon, lat, label, hint);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};

STDMETHODIMP TLisMapXImpl::ShowContourEx(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label, BSTR hint,
      BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
      VARIANT_BOOL isUnderline)
{
  try
  {
       m_VclCtl->ShowContourEx(lon, lat, label, hint,
                               fontName, fontSize, textColour,
                               isBold, isItalic, isUnderline);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};

STDMETHODIMP TLisMapXImpl::ShowLabelEx(double lon, double lat, BSTR label, BSTR hint,
      BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
      VARIANT_BOOL isUnderline)
{
  try
  {
       m_VclCtl->ShowLabelEx(lon, lat, label, hint,
                             fontName, fontSize, textColour,
                             isBold, isItalic, isUnderline);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



STDMETHODIMP TLisMapXImpl::ShowZone(double centLon, double centLat,
  LPSAFEARRAY zone, int width, int Color, int style, int objType,
  int layer)
{
  try
  {
       m_VclCtl->ShowZone(centLon, centLat, zone,
       width, Color, style, objType, layer);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



STDMETHODIMP TLisMapXImpl::ShowLink(double lon1, double lat1, double lon2,
  double lat2, int trim, MapArrowType arrowType)
{
  try
  {
      m_VclCtl->ShowLink(lon1, lat1, lon2, lat2, trim, arrowType);
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



STDMETHODIMP TLisMapXImpl::FitObjects()
{
  try
  {
       m_VclCtl->FitObjects();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



STDMETHODIMP TLisMapXImpl::CloseMap()
{
  try
  {
      m_VclCtl->CloseMap();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILisMapX);
  }
  return S_OK;
};



