//---------------------------------------------------------------------------

#ifndef uDiagramPanelH
#define uDiagramPanelH

#include <ExtCtrls.hpp>
#include <vector>
#include <memory>

enum DiagramShowMode {
    smZone = 1,
    smLine = 2,
    smDuel = 3
};

//---------------------------------------------------------------------------
class TPolarDiagramPanel : public TPanel {
private:
    int mouseX;
    
    std::vector<double> coverageZone;
    std::vector<double> noiseLimitedZone;
    std::vector<double> interfereLimitedZone;
    std::vector<double> noiseLimitedZone2;
    std::vector<double> interfereLimitedZone2;
    double m_norma;
    bool m_showCenter;
    bool m_showAxis;
    double m_duelAzimuth;
    double m_duelDistance;
    TComponent* Owner;    
    DiagramShowMode m_showMode;
    std::auto_ptr<Graphics::TBitmap> oldBmp;
    int oldMouseX;

    TColor colCoverage;
    TColor colNoise;
    TColor colInterf;

    int thickCoverage;
    int thickNoise;
    int thickInterf;

    void __fastcall SetShowCenter(bool value);
    void __fastcall SetShowAxis(bool value);
    void __fastcall SetNorma(double value); // maximum value
    void __fastcall SetShowMode(DiagramShowMode newShowMode);
    void __fastcall SetDuelDistance(double value);
    void __fastcall SetDuelAzimuth(double value);
protected:
    void __fastcall findNorma();
    void __fastcall drawZone(std::vector<double>& zone, int centerX, int centerY, TColor color = clBlack, int lineWeight = 1);
    inline __fastcall int screenValue(double value) { return value / norma; };
public:

    TFrame *fmProfileView;

    AnsiString label;
    AnsiString label2;
    __fastcall TPolarDiagramPanel(TComponent* AOwner);
    void __fastcall clear(void);
    void __fastcall DrawMarker(int X);
	virtual void __fastcall Paint(void);
    void __fastcall setCoverage(double*, TColor col, int thickness, int num = 36);
    void __fastcall setNoiseLimited(double*, TColor col, int thickness, int num = 36);
    void __fastcall setInterfereLimited(double*, TColor col, int thickness, int num = 36);
    void __fastcall setNoiseLimited2(double*, int num = 36);
    void __fastcall setInterfereLimited2(double*, int num = 36);
    __property bool showCenter = { read=m_showCenter, write=SetShowCenter};
    __property bool showAxis = { read=m_showAxis, write=SetShowAxis};
    __property double norma = { read=m_norma, write=SetNorma};
    __property double duelAzimuth = { read=m_duelAzimuth, write=SetDuelAzimuth};
    __property double duelDistance = { read=m_duelDistance, write=SetDuelDistance};
    __property DiagramShowMode showMode = { read=m_showMode, write=SetShowMode};
};
#endif

