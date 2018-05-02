//---------------------------------------------------------------------------

#ifndef uXmlDocH
#define uXmlDocH
//---------------------------------------------------------------------------
#include <Classes.hpp>
//---------------------------------------------------------------------------
#include <fstream>
#include <string>
#include <utility>
#include <vector>
#include <exception>
#ifdef _DEBUG
#include "dstring.h"
#include "uLogger.h"
#endif

enum LexemeType { ltEmpty, ltString, ltSymbol };
enum States { psWaitingCloseBracket, psWaitingName, psWaitingOpenBracket, psWaitingValue, psWaitingValueName };

class xml_parser_error : public std::exception
{
    std::string reason;
  public:
    explicit xml_parser_error (const std::string& what_arg): reason(what_arg) {};
    const char * what () const
    {
      return reason.c_str();
    }
};


//  стиль оформления документа
enum DocStyle { dsXml, dsBcNotify };

enum ParserState { psStart, psNameRead, psParamRead, psValRead, psTagClosed, psSubTag, psFinish };

__declspec(selectany)
char* stateName[] = { "psStart", "psNameRead", "psParamRead", "psValRead", "psTagClosed", "psSubTag", "psFinish" };


class XmlDocNode;

class XmlDoc
{
private:
    std::string comment;
    std::string tagName;
    std::string tagParam;
    std::string lastLexeme;

    States state;

    int line, symbol;
    std::string errorMsg;
    int errorCode;

public:
    std::vector<XmlDocNode*> rootNodes;

    #ifdef _DEBUG
    Logger xmlLog;
    #endif
    #ifdef _DEBUG
    void __fastcall LogEvent(String msg);
    #endif

    DocStyle docStyle;

    ParserState parserState;

    ParserState SetParserError(int ec, std::string& msg, std::string& lxm)
                                            {
                                            errorCode = ec;
                                            errorMsg = msg;
                                            lastLexeme = lxm;
                                            return psFinish;
                                            //throw xml_parser_error(msg);
                                            }

    std::string GetLastLexeme() { return lastLexeme; }
    std::string GetErrorMsg() { return errorMsg; }
    int GetErrorCode() { return errorCode; }
    int GetLine() { return line; }
    int GetSymbol() { return symbol; }


    __fastcall XmlDoc(): docStyle(dsXml) { Reset(); }
    __fastcall ~XmlDoc() { Reset(); }

    void __fastcall Reset();
    void __fastcall SetComment(const std::string& newComment);
    void __fastcall DeleteNodes();
    XmlDocNode* __fastcall SetRootNode(const std::string& name);
    XmlDocNode* __fastcall AddRootNode(const std::string& name);

    int __fastcall LoadFrom(const std::string& fileName);
    void __fastcall SaveTo(const std::string& fileName);

    int __fastcall PassSeparators(std::istream& src);
    int __fastcall ReadTag(std::istream& src, XmlDocNode* dest);

    std::string __fastcall GetLexeme(std::istream& src);
    inline bool IsLineFeed(char chr);
    bool IsSymbol(char chr);
    bool IsSeparator(char chr);
    LexemeType __fastcall GetLexemeType(std::string lexeme);
};


class XmlDocNode
{
public:
    typedef std::vector<std::pair<std::string, std::string> > Elements;
private:
    std::string name;
    Elements elements;
    std::vector<XmlDocNode*> subNodes;
protected:
  __fastcall XmlDocNode();
  __fastcall XmlDocNode(const std::string& name);

public:
  __fastcall ~XmlDocNode();
  __fastcall XmlDocNode(const XmlDocNode& src): name(src.name), elements(src.elements), subNodes(src.subNodes) {};
  XmlDocNode& __fastcall operator=(const XmlDocNode& src) { name = src.name, elements = src.elements, subNodes = src.subNodes; return *this;};

  void __fastcall AddElement(const std::string& name, const std::string& value);
  void __fastcall AddElement(const std::string& name, const double& value);
  void __fastcall AddElement(const std::string& name, const float& value);
  void __fastcall AddElement(const std::string& name, const int& value);
  void __fastcall AddElement(const std::string& name, const unsigned int& value);
  XmlDocNode* __fastcall AddNode(const std::string& name);
  const std::string& __fastcall GetName()  { return name; };
  void __fastcall SetName(const std::string& name)  { this->name = name; };

  std::vector<XmlDocNode*>& GetSubNodes() { return subNodes; };
  Elements& GetElements() { return elements; };
  void __fastcall SaveToStream(std::ostream& stream, DocStyle docStyle);

  friend XmlDocNode* __fastcall XmlDoc::SetRootNode(const std::string& name);
  friend XmlDocNode* __fastcall XmlDoc::AddRootNode(const std::string& name);
};

#endif
