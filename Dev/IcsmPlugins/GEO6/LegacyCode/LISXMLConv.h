//---------------------------------------------------------------------------

#ifndef LISXMLConvH
#define LISXMLConvH

/*
	TLISXMLConv - класс для выполнения преобразований
между форматами XML и FXM(T11...).
	Эквивалентные теги для форматов хранятся в файле
equi.txt.
теги для описания соответствия FXM->XML:

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
	WideString eqvDBName;//имя файла БД эквивалентных пар
	TComponent* tempOwner;//контейнер класса
	_di_IXMLNode fxm_to;//узел документа XML с парами FXM-XML
	_di_IXMLNode ds_info;//узел документа XML с описаниями датасетов
	//std::auto_ptr<Xmldoc::TXMLDocument> xml;//XML для хранения конфигурации (пар значений)
    Xmldoc::TXMLDocument* xml;
	std::map<AnsiString,TDataSet*> mDS;//набор наборов данных
	std::map<AnsiString, TClientDataSet*> mTempDS;//временные буфера данных
	bool start;//признак начала процесса создания сети
	std::string __fastcall FindParametr(//выполняет поиск параметра в FXM
		AnsiString nameParam,       //и возвращает значение
		std::vector<std::pair<std::string, std::string> > elements);
	void __fastcall StartNet(void);//начинает процесс создания сети
	void __fastcall DocumentAddField(AnsiString AttrName,//заполняет временные датасеты данными
									 AnsiString Dataset_name,
									 AnsiString Field_name,
									 AnsiString Field_type,
									 AnsiString Default,
									 AnsiString value,
									 int iteration,
									 bool add);
	void __fastcall DocumentCorrect(TClientDataSet* cds, int iteration, bool add);//корректирует записи датасета
	void __fastcall ParsingSubNode(std::vector<XmlDocNode*> vSub, int iteration);//выполняет разбор вложенных узлов
	void __fastcall CreateNet(void);//завершает процесс создания объектов
	void __fastcall LoadCDS(void);
	AnsiString __fastcall ConvertCordToNumber(AnsiString cord);
	AnsiString __fastcall GetOnlyDigit(AnsiString str);
public:
	__fastcall TLISXMLConv(TComponent* Owner, Xmldoc::TXMLDocument* xmlTmp);
	__fastcall ~TLISXMLConv();
	void __fastcall AddDataSetToList(TDataSet* ds);//добавляет DS в список
	void __fastcall ClearDataSetList(void);//очищает список DS
	void __fastcall ConvertToDatabase(AnsiString FileName);//конвертирует FXM в IB
	TClientDataSet* __fastcall GetDataSet(AnsiString name);//возвращает указатель на датасет по его имени
	void __fastcall CreateFXM(TIBDatabase* ibDB,
							  TIBDataSet* netDS,
							  AnsiString FileName);//выполняет преобразование IB->T11

};
//---------------------------------------------------------------------------
#endif
