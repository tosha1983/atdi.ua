//---------------------------------------------------------------------------
//
//  обеспечивает сохранение и восстановление расположени€ обњектов на Їкране
//

#ifndef uLayoutMngrH
#define uLayoutMngrH

#include <vcl.h>
#pragma hdrstop

class TLayoutManager {
    struct {
        int left, top, right, bottom;
        bool docked;
        bool dockedLeft;
        bool ShowExplorer;
        int reserve[100];
    } layout;
public:
    void __fastcall saveLayout(TForm*);
    void __fastcall loadLayout(TForm*);
    void __fastcall saveApplicationDesktop(void);
    void __fastcall loadApplicationDesktop(void);
    void __fastcall EnsureShortcut(TForm*);
    void __fastcall DeleteShortcut(TForm*);
};

extern TLayoutManager LayoutManager;
//---------------------------------------------------------------------------
#endif
