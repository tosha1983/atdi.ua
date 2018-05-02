//---------------------------------------------------------------------------


#pragma hdrstop

#include "uExplorer.h"
#include "uLayoutMngr.h"
#include "uParams.h"
#include <Registry.hpp>
#include "uMainForm.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

char *sLayuotPath = "\\WindowsLayout";
TLayoutManager LayoutManager;

//---------------------------------------------------------------------------
void __fastcall TLayoutManager::saveLayout(TForm* form)
{
    if (!form)
        return;
    //  сохранение параметров (размеры, положение) в реестр
//    if (form->WindowState == wsMaximized)
//        return;
    if (form->Name.IsEmpty())
        return;

    //определение прилепленности
    if ( form->ClassNameIs("TfrmMain") )
        layout.ShowExplorer = dynamic_cast<TfrmMain*>(form)->actExplorer->Checked;

    //для отображения окна Explorer' a при старте гланой формы
    if ( form->ClassNameIs("TfrmExplorer") )
    {
        layout.docked = ( dynamic_cast<TPanel*>(form->HostDockSite) != NULL );
        if ( layout.docked )
            layout.dockedLeft = form->HostDockSite->Name == "pnlLeftDockPanel";
    }

    WINDOWPLACEMENT wpl;
    memset(&wpl, '\0', sizeof(WINDOWPLACEMENT));
    wpl.length = sizeof(WINDOWPLACEMENT);
    if (!GetWindowPlacement(form->Handle, &wpl))
        ShowMessage("GetWindowPlacement() failed");

    layout.left = wpl.rcNormalPosition.left;
    layout.top = wpl.rcNormalPosition.top;
    layout.right = wpl.rcNormalPosition.right;
    layout.bottom = wpl.rcNormalPosition.right;

    bool res = true;
    TRegistry *reg = new TRegistry;
    try {
        reg->Access = KEY_WRITE;
        reg->RootKey = HKEY_CURRENT_USER;
        if (reg->OpenKey(AnsiString(sAppRegPath)+sLayuotPath, true)) {
            reg->WriteBinaryData(form->Name, &layout, sizeof(layout));
            reg->CloseKey();
        } else
            res = false;
    } __finally {
        reg->Free();
    }

    if (res){
    };
}

//---------------------------------------------------------------------------
void __fastcall TLayoutManager::loadLayout(TForm* form)
{
    if (!form)
        return;

    if (form->Name.IsEmpty())
        return;

    //  восстановление параметров (размеры, положение) из реестра
    bool res = true;
    TRegistry *reg = new TRegistry;
    try {
        reg->Access = KEY_READ;
        reg->RootKey = HKEY_CURRENT_USER;
        if (reg->OpenKeyReadOnly(AnsiString(sAppRegPath)+sLayuotPath)) {
            if (reg->ReadBinaryData(form->Name, &layout, sizeof(layout)) == 0)
                res = false;
            reg->CloseKey();
        } else
            res = false;
    } __finally {
        reg->Free();
    }

    if (res){

        WINDOWPLACEMENT wpl;
        memset(&wpl, '\0', sizeof(WINDOWPLACEMENT));
        wpl.length = sizeof(WINDOWPLACEMENT);
        if (!GetWindowPlacement(form->Handle, &wpl))
        {
            ShowMessage("GetWindowPlacement() failed");
            wpl.showCmd = SW_SHOW;
            wpl.ptMinPosition.x = -1;
            wpl.ptMinPosition.y = -1;
            wpl.ptMaxPosition.x = -1;
            wpl.ptMaxPosition.y = -1;
        }

        wpl.rcNormalPosition.left = layout.left;
        wpl.rcNormalPosition.top = layout.top;
        wpl.rcNormalPosition.right = layout.right;
        wpl.rcNormalPosition.right = layout.bottom;
        SetWindowPlacement(form->Handle, &wpl);

        if ( form->ClassNameIs("TfrmMain") && ( layout.ShowExplorer ) )
        {
            frmMain->showExplorer();
            frmMain->actExplorer->Checked = true;
        }

    }

    //для прилепания
    if (form->ClassNameIs("TfrmExplorer"))
    {
        if (!res) {
            layout.docked = true;
            layout.dockedLeft = true;
        }
        if (layout.docked) {
            //показываем панели
            if ( layout.dockedLeft )
            {
                frmMain->pnlLeftDockPanel->DockSite = true;
                frmMain->pnlLeftDockPanel->Width = dynamic_cast<TfrmExplorer*>(form)->width;

                frmMain->leftSplitter->Left = frmMain->pnlLeftDockPanel->Width + frmMain->leftSplitter->Width;
                frmMain->leftSplitter->Show();

                form->ManualDock(frmMain->pnlLeftDockPanel, frmMain->pnlLeftDockPanel, alLeft);
            }
            else
            {
                frmMain->pnlRightDockPanel->DockSite = true;
                frmMain->pnlRightDockPanel->Width = dynamic_cast<TfrmExplorer*>(form)->width;

                frmMain->rightSplitter->Left = frmMain->ClientWidth - dynamic_cast<TfrmExplorer*>(form)->width;
                frmMain->rightSplitter->Show();

                form->ManualDock(frmMain->pnlRightDockPanel, frmMain->pnlRightDockPanel, alLeft);
            }
        }
    }

    form->Show();
}

//---------------------------------------------------------------------------
void __fastcall TLayoutManager::saveApplicationDesktop(void)
{
}

//---------------------------------------------------------------------------
void __fastcall TLayoutManager::loadApplicationDesktop(void)
{
}

void __fastcall TLayoutManager::EnsureShortcut(TForm* form)
{
    bool shortcutExists = false;
    for (int i = 0; i < frmMain->mniForms->ComponentCount; i++) {
        if ((HANDLE)(dynamic_cast<TMenuItem*>(frmMain->mniForms->Items[i]))->Tag == form->Handle) {
            shortcutExists = true;
            break;
        }
    }
    if (!shortcutExists) {
        TMenuItem* mni = new TMenuItem(frmMain);
        frmMain->mniForms->Add(mni);
        mni->Caption = form->Caption;
        mni->Tag = (int)form->Handle;
        mni->OnClick = frmMain->mniChildWindowClick;
        mni->GroupIndex = 10;
        mni->RadioItem = true;
        mni->Checked = true;

        TToolButton* tbt = new TToolButton(frmMain);
        tbt->Parent = frmMain->tbrShortcut;
        //tbt->Index = frmMain->tbrShortcut->ButtonCount - 1;
        frmMain->tbrShortcut->Visible = true;
        AnsiString caption = " " + form->Caption + " ";
        tbt->Caption = caption;
        tbt->Hint = form->Caption;

        tbt->Tag = (int)form->Handle;
        tbt->OnClick = frmMain->mniChildWindowClick;
        tbt->Grouped = true;
        tbt->Style = tbsCheck;
        tbt->Down = true;
    }
}

void __fastcall TLayoutManager::DeleteShortcut(TForm* form)
{
    //  активное окно
    TForm* activeForm = frmMain;
    if (frmMain->MDIChildCount > 0)
        activeForm = frmMain->MDIChildren[0];

    int i = 0;
    while (i < frmMain->mniForms->Count) {
        if ((HANDLE)frmMain->mniForms->Items[i]->Tag == activeForm->Handle)
            frmMain->mniForms->Items[i]->Checked = true;
        if ((HANDLE)frmMain->mniForms->Items[i]->Tag == form->Handle)
            delete frmMain->mniForms->Items[i];
        else
            i++;
    }
    i = 0;
    while (i < frmMain->tbrShortcut->ButtonCount) {
        if ((HANDLE)frmMain->tbrShortcut->Buttons[i]->Tag == activeForm->Handle)
            frmMain->tbrShortcut->Buttons[i]->Down = true;
        if ((HANDLE)frmMain->tbrShortcut->Buttons[i]->Tag == form->Handle)
            delete frmMain->tbrShortcut->Buttons[i];
        else
            i++;
    }
}
