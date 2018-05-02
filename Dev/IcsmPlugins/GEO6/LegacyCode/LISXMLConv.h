//---------------------------------------------------------------------------

#ifndef LISXMLConvH
#define LISXMLConvH

/*
	TLISXMLConv - ����� ��� ���������� ��������������
����� ��������� XML � FXM(T11...).
	������������� ���� ��� �������� �������� � �����
equi.txt.
���� ��� �������� ������������ FXM->XML:

*/
//---------------------------------------------------------------------------
#include <vcl.h>
#include <XMLDoc.hpp>
#include <IBCustomDataSet.hpp>
#include "CoordConv.hpp"
#include <ctype.h>
#include <memory>
#include <map>
#include "uProgress.h"
using std::auto_ptr;
//---------------------------------------------------------------------------

class TLISXMLConv
{
private:
	WideString eqvDBName;//��� ����� �� ������������� ���
	TComponent* tempOwner;//��������� ������
	_di_IXMLNode fxm_to;//���� ��������� XML � ������ FXM-XML
	_di_IXMLNode ds_info;//���� ��������� XML � ���������� ���������
	//std::auto_ptr<Xmldoc::TXMLDocument> xml;//XML ��� �������� ������������ (��� ��������)
    Xmldoc::TXMLDocument* xml;
	std::map<AnsiString,TDataSet*> mDS;//����� ������� ������
	std::map<AnsiString, TClientDataSet*> mTempDS;//��������� ������ ������
	bool start;//������� ������ �������� �������� ����
	std::string __fastcall FindParametr(//��������� ����� ��������� � FXM
		AnsiString nameParam,       //� ���������� ��������
		std::vector<std::pair<std::string, std::string> > elements);
	void __fastcall StartNet(void);//�������� ������� �������� ����
	void __fastcall DocumentAddField(AnsiString AttrName,//��������� ��������� �������� �������
									 AnsiString Dataset_name,
									 AnsiString Field_name,
									 AnsiString Field_type,
									 AnsiString Default,
									 AnsiString value,
									 int iteration,
									 bool add);
	void __fastcall DocumentCorrect(TClientDataSet* cds, int iteration, bool add);//������������ ������ ��������
	void __fastcall ParsingSubNode(std::vector<XmlDocNode*> vSub, int iteration);//��������� ������ ��������� �����
	void __fastcall CreateNet(void);//��������� ������� �������� ��������
	void __fastcall LoadCDS(void);
	AnsiString __fastcall ConvertCordToNumber(AnsiString cord);
	AnsiString __fastcall GetOnlyDigit(AnsiString str);
public:
	__fastcall TLISXMLConv(TComponent* Owner, Xmldoc::TXMLDocument* xmlTmp);
	__fastcall ~TLISXMLConv();
	void __fastcall AddDataSetToList(TDataSet* ds);//��������� DS � ������
	void __fastcall ClearDataSetList(void);//������� ������ DS
	void __fastcall ConvertToDatabase(AnsiString FileName);//������������ FXM � IB
	TClientDataSet* __fastcall GetDataSet(AnsiString name);//���������� ��������� �� ������� �� ��� �����
	void __fastcall CreateFXM(TIBDatabase* ibDB,
							  TIBDataSet* netDS,
							  AnsiString FileName);//��������� �������������� IB->T11

};
//---------------------------------------------------------------------------
#endif
