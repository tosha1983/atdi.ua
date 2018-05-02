//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uDlgMapConf.h"
#include "MSXML_OCX.h"
//---------------------------------------------------------------------
#pragma resource "*.dfm"
TdlgMapConf *dlgMapConf;
//---------------------------------------------------------------------
__fastcall TdlgMapConf::TdlgMapConf(TComponent* AOwner)
	: TForm(AOwner)
{
}

__fastcall TdlgMapConf::TdlgMapConf(TComponent* AOwner, AnsiString confFile)
    : TForm(AOwner)
{
    if (!FileExists(confFile))
    {
        AnsiString path = ExtractFilePath(confFile);
        if (!DirectoryExists(path))
            throw *(new Exception(AnsiString().sprintf("Директория '%s' не существует", path.c_str())));
    }

    filename = confFile;
    if (FileExists(confFile))
    {
        TDOMDocument *conf = new TDOMDocument(this);
        if (!(bool)conf->load(TVariant(WideString(filename))))
            throw *(new Exception("Файл не читается"));
        IXMLDOMElementPtr root = conf->documentElement;
        if (!root.IsBound())
            throw *(new Exception("Корневой узел отсутствует"));

        IXMLDOMNodePtr section = root->firstChild;
        while (section.IsBound() && (
               section->nodeType != Msxml_tlb::NODE_ELEMENT ||
               wcscmp(section->nodeName, L"layers") != 0))
            section = section->nextSibling;

        if (section.IsBound())
        {
            IXMLDOMNodePtr node = section->firstChild;
            while (node)
            {
                if (wcscmp(node->nodeName, L"layer") == 0 && node->nodeType == Msxml_tlb::NODE_ELEMENT)
                {
                    IXMLDOMElementPtr element = node;
					TVariant vv = TVariant(element->getAttribute(L"file"));
                    if (vv.vt != VT_EMPTY)
                    {
                        LayerParams layerParams;

                        AnsiString fn = ExpandFileName((WideString)vv);
                        layerParams.path = fn;

                        TVariant v = TVariant(element->getAttribute(L"name"));
                        if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                            layerParams.name = WideString(v.bstrVal);

                        v = TVariant(element->getAttribute(L"visible"));
                        if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                            layerParams.visible = (WideString(v.bstrVal) != WideString(L"0"));

                        v = TVariant(element->getAttribute(L"autolabel"));
                        if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                            layerParams.labels = (WideString(v.bstrVal) != WideString(L"0"));

                        v = TVariant(element->getAttribute(L"zoomMin"));
                        if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                            layerParams.minZoom = AnsiString(v.bstrVal);

                        v = TVariant(element->getAttribute(L"zoomMax"));
                        if (v.vt != VT_EMPTY && v.vt != VT_NULL)
                            layerParams.maxZoom = AnsiString(v.bstrVal);

                        params.push_back(layerParams);

					}
				}
				node = node->nextSibling;
			}
			section = section->nextSibling;
		}
    }

    ShowData();
    changed = false;
}
//---------------------------------------------------------------------

void __fastcall TdlgMapConf::ShowData()
{
    while (lbxParams->Items->Count > params.size())
        lbxParams->Items->Delete(0);
    while (lbxParams->Items->Count < params.size())
        lbxParams->Items->Add("");

    for (int i = 0; i < params.size(); i++)
        lbxParams->Items->Strings[i] = params[i].name.Length() > 0 ? params[i].name : params[i].path;
        
    if (lbxParams->ItemIndex > -1) {
        LayerParams& p = params[lbxParams->ItemIndex];
        edtFilePath->Text = p.path;
        edtName->Text = p.name;
        chbVisible->Checked = p.visible;
        chbAutoLabels->Checked = p.labels;
        edtMaxZoom->Text = p.maxZoom;
        edtMinZoom->Text = p.minZoom;
    } else {
        edtFilePath->Text = "";
        edtName->Text = "";
        chbVisible->Checked = false;
        chbAutoLabels->Checked = false;
        edtMaxZoom->Text = "";
        edtMinZoom->Text = "";
    }
}
//---------------------------------------------------------------------

void __fastcall TdlgMapConf::btnUpClick(TObject *Sender)
{
    if (lbxParams->ItemIndex != -1 && lbxParams->ItemIndex != 0)
    {
        LayerParams p = params[lbxParams->ItemIndex];
        params[lbxParams->ItemIndex] = params[lbxParams->ItemIndex - 1];
        params[lbxParams->ItemIndex - 1] = p;
        lbxParams->ItemIndex = lbxParams->ItemIndex - 1;
        ShowData();
        changed = true;
        //lbxParams->Items->Move(lbxParams->ItemIndex, lbxParams->ItemIndex - 1);
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::btnDownClick(TObject *Sender)
{
    if (lbxParams->ItemIndex != -1 && lbxParams->ItemIndex != lbxParams->Count - 1)
    {
        LayerParams p = params[lbxParams->ItemIndex];
        params[lbxParams->ItemIndex] = params[lbxParams->ItemIndex + 1];
        params[lbxParams->ItemIndex + 1] = p;
        lbxParams->ItemIndex = lbxParams->ItemIndex + 1;
        ShowData();
        changed = true;
        //lbxParams->Items->Move(lbxParams->ItemIndex, lbxParams->ItemIndex + 1);
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::btnSelectFilePathClick(TObject *Sender)
{
    opd->FileName = edtFilePath->Text;
    if (opd->Execute())
        edtFilePath->Text = opd->FileName;
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::btnReplaceClick(TObject *Sender)
{
    if (lbxParams->ItemIndex != -1)
    {
        params[lbxParams->ItemIndex].path = edtFilePath->Text;
        ShowData();
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::btnAddClick(TObject *Sender)
{
    params.push_back();
    params[params.size() - 1].path = edtFilePath->Text;
    ShowData();
    changed = true;
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::btnDeleteClick(TObject *Sender)
{
    if (lbxParams->ItemIndex != -1)
    {
        params.erase(&(params[lbxParams->ItemIndex]));
        ShowData();
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::btnOkClick(TObject *Sender)
{
    if (changed)
    {
        TDOMDocument *conf = new TDOMDocument(this);
        AnsiString stub = AnsiString().sprintf("<?xml version=\"1.0\" encoding=\"windows-1251\"?>\n\
<map>\n\
    <layers>\n\
    </layers>\n\
	<!-- разделитель дробной части - точка -->\n\
	<position centerx=\"0.0\" centery=\"0.0\" zoom=\"0.0\"/>\n\
</map>");

        if (FileExists(filename))
            conf->load(TVariant(WideString(filename)));
        else
            conf->loadXML(WideString(stub));

        IXMLDOMElementPtr root = conf->documentElement;
        if (!root.IsBound())
            throw *(new Exception("Корневой узел отсутствует"));

        IXMLDOMNodePtr section = root->firstChild;
        while (section.IsBound() && (
               section->nodeType != Msxml_tlb::NODE_ELEMENT ||
               wcscmp(section->nodeName, L"layers") != 0))
            section = section->nextSibling;

        if (!section.IsBound())
        {
            section = conf->createCDATASection(L"layers");
            root->appendChild(section);
        }

        if (section.IsBound())
        {
            while (section->firstChild)
                section->removeChild(section->firstChild);

            for (int i = 0; i < params.size(); i++)
            {
                IXMLDOMNodePtr node = conf->createElement(L"layer");
                section->appendChild(node);

                IXMLDOMAttributePtr attr = conf->createAttribute(L"file");
                attr->set_value(Variant(WideString(params[i].path)));
                node->attributes->setNamedItem(attr);

                attr = conf->createAttribute(L"name");
                attr->set_value(Variant(WideString(params[i].name)));
                node->attributes->setNamedItem(attr);

                attr = conf->createAttribute(L"visible");
                attr->set_value(Variant(params[i].visible ? L"1" : L"0"));
                node->attributes->setNamedItem(attr);

                attr = conf->createAttribute(L"autolabel");
                attr->set_value(Variant(params[i].labels ? L"1" : L"0"));
                node->attributes->setNamedItem(attr);

                if (params[i].minZoom.Length() > 0)
                {
                    attr = conf->createAttribute(L"zoomMin");
                    attr->set_value(Variant(WideString(params[i].minZoom)));
                    node->attributes->setNamedItem(attr);
                }
                if (params[i].maxZoom.Length() > 0)
                {
                    attr = conf->createAttribute(L"zoomMax");
                    attr->set_value(Variant(WideString(params[i].maxZoom)));
                    node->attributes->setNamedItem(attr);
                }

			}
		}
        conf->save(TVariant(WideString(filename)));
        changed = false;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::lbxParamsClick(TObject *Sender)
{
    ShowData();
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::lbxParamsKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    ShowData();
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::lbxParamsKeyUp(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    ShowData();
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::chbVisibleClick(TObject *Sender)
{
    if (lbxParams->ItemIndex > -1)
    {
        params[lbxParams->ItemIndex].visible = chbVisible->Checked;
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::chbAutoLabelsClick(TObject *Sender)
{
    if (lbxParams->ItemIndex > -1)
    {
        params[lbxParams->ItemIndex].labels = chbAutoLabels->Checked;
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::edtNameChange(TObject *Sender)
{
    if (lbxParams->ItemIndex > -1)
    {
        params[lbxParams->ItemIndex].name = edtName->Text;
        ShowData();
        changed = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::edtMinZoomChange(TObject *Sender)
{
    if (lbxParams->ItemIndex > -1) try {
        if (edtMinZoom->Text.Length() > 0)
            edtMinZoom->Text.ToDouble();
        params[lbxParams->ItemIndex].minZoom = edtMinZoom->Text;
        changed = true;
    } catch (...) {}
}
//---------------------------------------------------------------------------

void __fastcall TdlgMapConf::edtMaxZoomChange(TObject *Sender)
{
    if (lbxParams->ItemIndex > -1) try {
        if (edtMaxZoom->Text.Length() > 0)
            edtMaxZoom->Text.ToDouble();
        params[lbxParams->ItemIndex].maxZoom = edtMaxZoom->Text;
        changed = true;
    } catch (...) {}
}
//---------------------------------------------------------------------------

