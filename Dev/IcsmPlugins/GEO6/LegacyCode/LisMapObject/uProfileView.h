//---------------------------------------------------------------------------


#ifndef uProfileViewH
#define uProfileViewH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
#include <vector>
#include <memory>
#include <Graphics.hpp>
//---------------------------------------------------------------------------

typedef void __fastcall (__closure *COnMouseMove)(int X, int Y);

extern __declspec(selectany) int COLOR_GROUND = RGB(128,64,0);
extern __declspec(selectany) int COLOR_ANTENNA = clNavy;
extern __declspec(selectany) int COLOR_DIRECTLINE = clRed;
extern __declspec(selectany) int COLOR_ANGLES = clGreen;
extern __declspec(selectany) int COLOR_EARTH = clBlack;
extern __declspec(selectany) int COLOR_SEA = clBlue;
extern __declspec(selectany) int COLOR_TITLE = clNavy;

typedef struct {
  int height;
  unsigned char morpho;
} TReliefPoint;

class TfmProfileView : public TFrame
{
__published:	// IDE-managed Components
    TCheckBox *chbEarthCurve;
    TStaticText *txtHeight;
    TStaticText *txtDist;
    void __fastcall FrameResize(TObject *Sender);
    void __fastcall chbEarthCurveClick(TObject *Sender);
    void __fastcall FrameMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
private:	// User declarations
    double angle12, angle34;
    double deltaXLeft, deltaXRight;
  double dist1, dist2, dist3, dist0; 
  int m_txAntHeight;
  int m_rcAntHeight;
  int m_drawMode;
  bool m_showAngles;
  bool m_useEarthCurve;
  bool resetState;
  double m_lon1, m_lat1, m_lon2, m_lat2;
  std::vector <TReliefPoint> reliefHeight;
  std::vector <int> earthCurve;
  int normaHeightIdx;
  double dist_gorb1, dist_gorb2, angle1, angle2, h_angle1, h_angle2;
  double reliefStep, reliefDistance;
  int oldMouseX;
  std::auto_ptr<Graphics::TBitmap> oldBmp;
  void __fastcall SetTxAntHeight(int value);
  void __fastcall SetRcAntHeight(int value);
  void __fastcall SetDrawMode(int value);
  void __fastcall SetShowAngles(bool value);
  void __fastcall SetEarthCurve(bool value);
  Classes::TWndMethod oldWndProc;
  void __fastcall newWndProc(Messages::TMessage &Message);
protected:
public:
    long leftTxX, rightTxX;
    long leftTxHeight, rightTxHeight;
    long leftTxBaseHeight, rightTxBaseHeight;


    //  высота подвеса передающей антенны
    __property int txAntHeight  = { read=m_txAntHeight,
                                    write=SetTxAntHeight};
    //  высота подвеса приёмной антенны

    __property int rcAntHeight  = { read=m_rcAntHeight,
                                    write=SetRcAntHeight };
    //  режим отображения профиля: 0 - дискретный,
    //                             1 - интерполированный
    __property int drawMode  = { read=m_drawMode,
                                 write=SetDrawMode };
    //  режим отрисоки углов закрытия
    __property bool showAngles  = { read=m_showAngles,
                                    write=SetShowAngles };
    //  режим кривизны Земли
    __property bool useEarthCurve  = { read=m_useEarthCurve,
                                    write=SetEarthCurve };

    //  получить профиль
    void __fastcall RetreiveProfile(double lon1, double lat1,
                                    double lon2, double lat2);

    void __fastcall RetreiveProfile(double lon1, double lat1,
                                    double lon2, double lat2,
                                    int *normaHeight
                                   );

    //профиль на четырех точках
    void __fastcall RetreiveProfile(double lon1, double lat1,
                                    double lon2, double lat2,
                                    double lon3, double lat3,
                                    double lon4, double lat4
                                   );

    //  сброс
    void __fastcall Reset();
    void __fastcall Paint();
    void __fastcall External_OnMouseMove(int X, int Y);
    COnMouseMove Import_OnMouseMove;
    void __fastcall Frame_DoMouseMove(int X, int Y);
    __fastcall TfmProfileView(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TfmProfileView *fmProfileView;
//---------------------------------------------------------------------------
#endif
