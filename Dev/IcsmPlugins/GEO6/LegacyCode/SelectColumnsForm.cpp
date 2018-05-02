//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "SelectColumnsForm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
//---------------------------------------------------------------------------
__fastcall TfrmSelectColumns::TfrmSelectColumns(TComponent* Owner) : TForm(Owner){}
//---------------------------------------------------------------------------

void __fastcall MoveField(TListBox * lbFrom, TListBox * lbTo)
{
    if ( lbFrom->Items->Count > 0 )
    {

        if ( lbTo->ItemIndex < 0)
            lbTo->ItemIndex = 0;

        int lbToSelectedItem = lbTo->ItemIndex;
        if ( lbTo->SelCount > 1 )
           for(int i = 0; i < lbTo->Items->Count; i++)
               if ( lbTo->Selected[i] )
               {
                   lbToSelectedItem = i;
                   break;
               }

        if ( lbFrom->SelCount > 1 )
        {
            while( lbFrom->SelCount > 0 )
                for(int i = 0; i < lbFrom->Items->Count; i++)
                    if ( lbFrom->Selected[i] )
                    {
                        lbTo->Items->AddObject(lbFrom->Items->Strings[i], lbFrom->Items->Objects[i]);
                        lbToSelectedItem++;
                        lbFrom->Items->Delete(i);
                        break;
                    }
        }
        else
        {
            if ( lbFrom->ItemIndex < 0 )
                lbFrom->ItemIndex = 0;
            int lbFromSelectedItem = lbFrom->ItemIndex;

            lbTo->Items->AddObject(lbFrom->Items->Strings[lbFromSelectedItem], lbFrom->Items->Objects[lbFromSelectedItem]);

            lbFrom->Items->Delete(lbFromSelectedItem);
            if ( lbFrom->Items->Count > lbFromSelectedItem )
                lbFrom->ItemIndex = lbFromSelectedItem;
            else
                lbFrom->ItemIndex = lbFrom->Items->Count - 1;
        }
    }
}
//---------------------------------------------------------------------------
void __fastcall MoveFields(TListBox * lbFrom, TListBox * lbTo)
{
    lbTo->Items->AddStrings(lbFrom->Items);
    lbFrom->Clear();
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelectColumns::btnAddClick(TObject *)
{
    MoveField(lbAccessibleFields, lbSelectedFields);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectColumns::btnAddAllClick(TObject *)
{
    MoveFields(lbAccessibleFields, lbSelectedFields);
}
//---------------------------------------------------------------------------
void __fastcall TfrmSelectColumns::btnRemoveClick(TObject *)
{
    MoveField(lbSelectedFields, lbAccessibleFields);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectColumns::btnRemoveAllClick(TObject *)
{
    MoveFields(lbSelectedFields, lbAccessibleFields);
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectColumns::btnUpClick(TObject *)
{
    if ( lbSelectedFields->ItemIndex > 0 )
        lbSelectedFields->Items->Exchange(lbSelectedFields->ItemIndex, lbSelectedFields->ItemIndex - 1);
    lbSelectedFields->Selected[lbSelectedFields->ItemIndex] = true;    
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectColumns::btnDownClick(TObject *)
{
    if ( lbSelectedFields->ItemIndex < lbSelectedFields->Items->Count - 1 )
        lbSelectedFields->Items->Exchange(lbSelectedFields->ItemIndex, lbSelectedFields->ItemIndex + 1);
    lbSelectedFields->Selected[lbSelectedFields->ItemIndex] = true;    
}
//---------------------------------------------------------------------------


void __fastcall TfrmSelectColumns::btnCancelClick(TObject *)
{
    ModalResult = mrCancel;
}
//---------------------------------------------------------------------------

void __fastcall TfrmSelectColumns::btnOkClick(TObject *)
{
    ModalResult = mrOk;        
}
//---------------------------------------------------------------------------

