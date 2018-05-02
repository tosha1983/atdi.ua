//$$---- Active Form HDR ---- (stActiveFormHdr)
//---------------------------------------------------------------------------


#ifndef LisMapImplH
#define LisMapImplH
//---------------------------------------------------------------------------
#include <classes.hpp>
#include <controls.hpp>
#include <stdCtrls.hpp>
#include <forms.hpp>
#include "LisMap_TLB.h"
#include <AxCtrls.hpp>
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include "CustomMap.h"
#include <Forms.hpp>
//---------------------------------------------------------------------------
#define VariantBoolToBool(x) (x == VARIANT_TRUE)
//---------------------------------------------------------------------------
class TLisMapX : public TActiveForm
{
__published:	// IDE-managed Components
   TCustomMapFrame *cmf;
private:	// User declarations
public:		// User declarations
    __fastcall TLisMapX(HWND ParentWindow);
    __fastcall TLisMapX(TComponent* AOwner): TActiveForm(AOwner) {};
    void __fastcall Init();
    void __fastcall SetCenter(double lon, double lat);
    void __fastcall SetScale(double scale);
    void __fastcall Clear(int layer, bool refresh = false);
    void __fastcall ShowStation(double lon, double lat, AnsiString label, AnsiString hint);
    void __fastcall ShowCoordZone(double centLon, double centLat, LPSAFEARRAY zone);
    void __fastcall ShowPoint(double lon, double lat, int colour, int width, PointType type, BSTR label, BSTR hint);
    void __fastcall ShowStation(double lon, double lat, BSTR label, BSTR hint);
    void __fastcall Refresh();
    void __fastcall ShowContour(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label, BSTR hint);
    void __fastcall ShowZone(double centLon, double centLat, LPSAFEARRAY zone,
                              int width, int color, int style, int objType, int layer);
    void __fastcall ShowLink(double lon1, double lat1, double lon2, double lat2, int trim, MapArrowType mat = matNone);
    void __fastcall FitObjects();
    void __fastcall CloseMap();
    //----
    void __fastcall ShowPointEx(double lon, double lat, PointType type, int pointWidth,
                                BSTR label, BSTR hint, OLE_COLOR pointColour, BSTR fontName, int fontSize, OLE_COLOR textColour,
                                VARIANT_BOOL isBold, VARIANT_BOOL isItalic, VARIANT_BOOL isUnderline);

    void __fastcall ShowStationEx(double lon, double lat, BSTR label, BSTR hint,
                                  BSTR fontName, int fontSize, OLE_COLOR textColour,
                                  VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
                                  VARIANT_BOOL isUnderline);

    void __fastcall ShowContourEx(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label, BSTR hint,
                                  BSTR fontName, int fontSize, OLE_COLOR textColour,
                                  VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
                                  VARIANT_BOOL isUnderline);

    void __fastcall ShowLabelEx(double lon, double lat, BSTR label, BSTR hint,
                                BSTR fontName, int fontSize, OLE_COLOR textColour,
                                VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
                                VARIANT_BOOL isUnderline);
};
//---------------------------------------------------------------------------
extern PACKAGE TLisMapX *LisMapX;
//---------------------------------------------------------------------------

//---------------------------------------------------------------------------
class ATL_NO_VTABLE TLisMapXImpl:
  VCLCONTROL_IMPL(TLisMapXImpl, LisMapX, TLisMapX, ILisMapX2, DIID_ILisMapXEvents)
{
  void __fastcall ActivateEvent(TObject *Sender);
  void __fastcall ClickEvent(TObject *Sender);
  void __fastcall CreateEvent(TObject *Sender);
  void __fastcall DblClickEvent(TObject *Sender);
  void __fastcall DeactivateEvent(TObject *Sender);
  void __fastcall DestroyEvent(TObject *Sender);
  void __fastcall KeyPressEvent(TObject *Sender, char &Key);
  void __fastcall PaintEvent(TObject *Sender);
public:

  void InitializeControl()
  {
    m_VclCtl->OnActivate = ActivateEvent;
    m_VclCtl->OnClick = ClickEvent;
    m_VclCtl->OnCreate = CreateEvent;
    m_VclCtl->OnDblClick = DblClickEvent;
    m_VclCtl->OnDeactivate = DeactivateEvent;
    m_VclCtl->OnDestroy = DestroyEvent;
    m_VclCtl->OnKeyPress = KeyPressEvent;
    m_VclCtl->OnPaint = PaintEvent;
  }

// The COM MAP entries declares the interfaces your object exposes (through
// QueryInterface). CComRootObjectEx::InternalQueryInterface only returns
// pointers for interfaces in the COM map. VCL controls exposed as OCXes
// have a minimum set of interfaces defined by the
// VCL_CONTROL_COM_INTERFACE_ENTRIES macro. Add other interfaces supported
// by your object with additional COM_INTERFACE_ENTRY[_xxx] macros.
//
BEGIN_COM_MAP(TLisMapXImpl)
  VCL_CONTROL_COM_INTERFACE_ENTRIES(ILisMapX)
  VCL_CONTROL_COM_INTERFACE_ENTRIES(ILisMapX2)
END_COM_MAP()



// The PROPERTY map stores property descriptions, property DISPIDs,
// property page CLSIDs and IDispatch IIDs. You may use use
// IPerPropertyBrowsingImpl, IPersistPropertyBagImpl, IPersistStreamInitImpl,
// and ISpecifyPropertyPageImpl to utilize the information in you property
// map.
//
// NOTE: The BCB Wizard does *NOT* maintain your PROPERTY_MAP table. You must
//       add or remove entries manually.
//
BEGIN_PROPERTY_MAP(TLisMapXImpl)
  // PROP_PAGE(CLSID_LisMapXPage)
END_PROPERTY_MAP()

/* DECLARE_VCL_CONTROL_PERSISTENCE(CppClass, VclClass) is needed for VCL
 * controls to persist via the VCL streaming mechanism and not the ATL mechanism.
 * The macro adds static IPersistStreamInit_Load and IPersistStreamInit_Save
 * methods to your implementation class, overriding the methods in IPersistStreamImpl. 
 * This macro must be manually undefined or removed if you port to C++Builder 4.0. */

DECLARE_VCL_CONTROL_PERSISTENCE(TLisMapXImpl, TLisMapX);

// The DECLARE_ACTIVEXCONTROL_REGISTRY macro declares a static 'UpdateRegistry' 
// routine which registers the basic information about your control. The
// parameters expected by the macro are the ProgId & the ToolboxBitmap ID of
// your control.
//
DECLARE_ACTIVEXCONTROL_REGISTRY("LisMap.LisMapX", 1);

protected: 
  STDMETHOD(_set_Font(IFontDisp** Value));
  STDMETHOD(get_Active(VARIANT_BOOL* Value));
  STDMETHOD(get_AlignDisabled(VARIANT_BOOL* Value));
  STDMETHOD(get_AutoScroll(VARIANT_BOOL* Value));
  STDMETHOD(get_AutoSize(VARIANT_BOOL* Value));
  STDMETHOD(get_AxBorderStyle(TxActiveFormBorderStyle* Value));
  STDMETHOD(get_BorderWidth(long* Value));
  STDMETHOD(get_Caption(BSTR* Value));
  STDMETHOD(get_Color(::OLE_COLOR* Value));
  STDMETHOD(get_DoubleBuffered(VARIANT_BOOL* Value));
  STDMETHOD(get_DropTarget(VARIANT_BOOL* Value));
  STDMETHOD(get_Enabled(VARIANT_BOOL* Value));
  STDMETHOD(get_Font(IFontDisp** Value));
  STDMETHOD(get_HelpFile(BSTR* Value));
  STDMETHOD(get_KeyPreview(VARIANT_BOOL* Value));
  STDMETHOD(get_PixelsPerInch(long* Value));
  STDMETHOD(get_PrintScale(TxPrintScale* Value));
  STDMETHOD(get_Scaled(VARIANT_BOOL* Value));
  STDMETHOD(get_Visible(VARIANT_BOOL* Value));
  STDMETHOD(get_VisibleDockClientCount(long* Value));
  STDMETHOD(set_AutoScroll(VARIANT_BOOL Value));
  STDMETHOD(set_AutoSize(VARIANT_BOOL Value));
  STDMETHOD(set_AxBorderStyle(TxActiveFormBorderStyle Value));
  STDMETHOD(set_BorderWidth(long Value));
  STDMETHOD(set_Caption(BSTR Value));
  STDMETHOD(set_Color(::OLE_COLOR Value));
  STDMETHOD(set_DoubleBuffered(VARIANT_BOOL Value));
  STDMETHOD(set_DropTarget(VARIANT_BOOL Value));
  STDMETHOD(set_Enabled(VARIANT_BOOL Value));
  STDMETHOD(set_Font(IFontDisp* Value));
  STDMETHOD(set_HelpFile(BSTR Value));
  STDMETHOD(set_KeyPreview(VARIANT_BOOL Value));
  STDMETHOD(set_PixelsPerInch(long Value));
  STDMETHOD(set_PrintScale(TxPrintScale Value));
  STDMETHOD(set_Scaled(VARIANT_BOOL Value));
  STDMETHOD(set_Visible(VARIANT_BOOL Value));
  STDMETHOD(Init());
  STDMETHOD(Show());
  STDMETHOD(SetCenter(double lon, double lat));
  STDMETHOD(SetScale(double scale));
  STDMETHOD(Clear(int layer, long refresh));
  STDMETHOD(ShowCoordZone(double centLon, double centLat, LPSAFEARRAY zone));
  STDMETHOD(ShowPoint(double lon, double lat, int colour, int width,
      PointType type, BSTR label, BSTR hint));
  STDMETHOD(ShowStation(double lon, double lat, BSTR label, BSTR hint));
  STDMETHOD(refresh());
  STDMETHOD(ShowContour(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label,
      BSTR hint));
  STDMETHOD(ShowZone(double centLon, double centLat, LPSAFEARRAY zone,
      int width, int Color, int style, int objType, int layer));
  STDMETHOD(ShowLink(double lon1, double lat1, double lon2, double lat2,
      int trim, MapArrowType arrowType));
  STDMETHOD(FitObjects());
  STDMETHOD(CloseMap());
  //--
  STDMETHOD(ShowPointEx(double lon, double lat, PointType type, int pointWidth,
      BSTR label, BSTR hint, OLE_COLOR pointColour, BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic, VARIANT_BOOL isUnderline));
  STDMETHOD(ShowStationEx(double lon, double lat, BSTR label, BSTR hint,
      BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
      VARIANT_BOOL isUnderline));
  STDMETHOD(ShowContourEx(LPSAFEARRAY lon, LPSAFEARRAY lat, BSTR label, BSTR hint,
      BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
      VARIANT_BOOL isUnderline));
  STDMETHOD(ShowLabelEx(double lon, double lat, BSTR label, BSTR hint,
      BSTR fontName, int fontSize, OLE_COLOR textColour,
      VARIANT_BOOL isBold, VARIANT_BOOL isItalic,
      VARIANT_BOOL isUnderline));
};
//---------------------------------------------------------------------------
#endif
