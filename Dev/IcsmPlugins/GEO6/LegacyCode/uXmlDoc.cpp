//---------------------------------------------------------------------------
#include <iomanip>
#include <iostream>
#include <istream>
#include <sstream>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <sys\timeb.h>
#include <Windows.h>

#include "uXmlDoc.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

char *szUnknownChar = "Неизвестный символ";
char *szUnexpectedEof = "Неожиданный конец файла";
char *szBadStream = "Сбой потока чтения";
char *szWrongTagName = "Неправильное имя тэга";

void __fastcall XmlDoc::DeleteNodes()
{
    for (std::vector<XmlDocNode*>::iterator li = rootNodes.begin(); li != rootNodes.end(); li++)
        delete *li;
    rootNodes.clear();
}

void __fastcall XmlDoc::Reset()
{
    DeleteNodes();

    comment.clear();
/*
    //  текущее время
    timeb timebuf;
    ftime(&timebuf);
    char *ct = ctime(&timebuf.time);
    ct[24] = '\0';
    comment = comment + ' ' + ct;
*/
}

void __fastcall XmlDoc::SetComment(const std::string& newComment)
{
    comment = newComment;
}

XmlDocNode* __fastcall XmlDoc::SetRootNode(const std::string& name)
{
    DeleteNodes();
    return AddRootNode(name);
}

XmlDocNode* __fastcall XmlDoc::AddRootNode(const std::string& name)
{
    XmlDocNode* newNode = new XmlDocNode(name);
    rootNodes.push_back(newNode);
    return newNode;
}

LexemeType __fastcall XmlDoc::GetLexemeType(std::string lexeme)
{
    switch (lexeme.size()) {

        case 0:
            return ltEmpty;
        case 1:
            if (IsSymbol(lexeme[0]))
                return ltSymbol;
            else
                return ltString;
        default:
            return ltString;
    }
}

int __fastcall XmlDoc::LoadFrom(const std::string& fileName)
{
    DeleteNodes();

    #ifdef _DEBUG
    xmlLog.Clear();
    #endif

    std::ifstream src(fileName.c_str());

    line = 1; symbol = 0;

    std::string lexeme;

    int errorCode = 0;

    const char* startLabel = "<HEAD>";
    const int startLabelLen = strlen(startLabel);
    int labelPos = 0;

    while (!src.eof() && labelPos < startLabelLen)
    {
        if (!src.good())
        {
            errorMsg = szBadStream;
            return  -1;
        }

        char chr = src.get();
        symbol++;

        if (chr == startLabel[labelPos])
            labelPos++;
        else
        {
            labelPos = 0;
            if (IsLineFeed(chr))
            {
                line++;
                symbol = 0;
            }
        }

        /*
        else
        {
            src.putback(chr);
            char str[7]; ZeroMemory(str, 7);
            src.read(str, 6);
            if ( strcmpi(str, startLabel) == 0 )
            {
                #ifdef _DEBUG
                LogEvent("srcln "+IntToStr(__LINE__)+": <HEAD> found");
                #endif
                break;
            }
        }
        */
    }

    // roll back start label
    while (labelPos--)
        src.unget();

    try {
        while (!src.eof()) {

            if (!src.good()) {
                errorMsg = szBadStream;
                errorCode = -1;
                break;
            }

            //  читаем в цикле корневые узлы
            //  допущения: описания нет, комментариев нет

            PassSeparators(src);
            lexeme.clear();
            lexeme = GetLexeme(src);
            // между тегами хай буде що завгодно
            while (lexeme.size() > 0 && lexeme[0] != '<')
                lexeme = GetLexeme(src);

            #ifdef _DEBUG
            LogEvent("srcln "+IntToStr(__LINE__)+": Processing tag");
            #endif

            if (GetLexemeType(lexeme) == ltSymbol && lexeme[0] == '<') {
                errorCode = ReadTag(src, NULL);
                if (errorCode)
                    break;
            } else if (GetLexemeType(lexeme) == ltEmpty) {
                errorCode = 0;
                break;
            } else {
                errorMsg = "Неизвестный символ в корневом тэге";
                errorCode = -1;
                break;
            }
        }
    } catch (std::exception &e) {
    }
    return errorCode;
}

int __fastcall XmlDoc::ReadTag(std::istream& src, XmlDocNode* node)
{
    std::string lexeme, tagName, lastParam, lastVal;
    XmlDocNode* currentNode = NULL;

    parserState = psStart;

    while (parserState != psFinish)
    {
        lexeme = GetLexeme(src);
        #ifdef _DEBUG
        LogEvent("srcln "+IntToStr(__LINE__)+": Next round");
        #endif
        switch (parserState)
        {
            case psStart:
                //  начинаем не с открывающей скобки, а сразу с имени тэга
                if (GetLexemeType(lexeme) == ltString)
                {
                    tagName = lexeme;
                    if (node)
                        currentNode = node->AddNode(tagName);
                    else
                        currentNode = AddRootNode(tagName);
                    parserState = psNameRead;
                } else
                    parserState = SetParserError(-1, szWrongTagName, lexeme);
                break;
            case psNameRead:
                switch (GetLexemeType(lexeme)) {
                    case ltString:
                        //  пошли параметры-значения
                        lastParam = lexeme;
                        parserState = psParamRead;
                        break;
                    case ltSymbol:
                        //  или закрываем тэг
                        switch (lexeme[0]) {
                            case '>':
                                //  открытый
                                parserState = psTagClosed;
                                break;
                            case '/':
                                //  или закрытый
                                lexeme = GetLexeme(src);
                                #ifdef _DEBUG
                                LogEvent("srcln "+IntToStr(__LINE__)+": Closed tag name read");
                                #endif
                                if (GetLexemeType(lexeme) == ltSymbol && lexeme[0] == '>')
                                    parserState = psFinish;
                                else
                                    parserState = SetParserError(-1, szUnknownChar, lexeme);
                                break;
                            default:
                                parserState = SetParserError(-1, szUnknownChar, lexeme);
                        }
                        break;
                    default:
                        parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                }
                break;
            case psParamRead:
                switch (GetLexemeType(lexeme)) {
                    case ltString:
                        parserState = SetParserError(-1, "Нет знака '='", lexeme);
                        break;
                    case ltSymbol:
                        if (lexeme[0] == '=') {
                            lexeme = GetLexeme(src);
                            #ifdef _DEBUG
                            LogEvent("srcln "+IntToStr(__LINE__)+": Expecting param value");
                            #endif
                            if (GetLexemeType(lexeme) == ltString || GetLexemeType(lexeme) == ltEmpty) {
                                currentNode->AddElement(lastParam, lexeme);
                                parserState = psValRead;
                            } else
                                parserState = SetParserError(-1, szUnknownChar, lexeme);
                        }
                        break;
                    default:
                        parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                }
                break;
            case psValRead:
                switch (GetLexemeType(lexeme)) {
                    case ltString:
                        //  ещё один параметр-значение
                        lastParam = lexeme;
                        parserState = psParamRead;
                        break;
                    case ltSymbol:
                        //  или закрываем тэг
                        switch (lexeme[0]) {
                            case '>':
                                //  открытый
                                parserState = psTagClosed;
                                break;
                            case '/':
                                //  или закрытый
                                lexeme = GetLexeme(src);
                                #ifdef _DEBUG
                                LogEvent("srcln "+IntToStr(__LINE__)+": Close tag val read");
                                #endif
                                if (GetLexemeType(lexeme) == ltSymbol && lexeme[0] == '>')
                                    parserState = psFinish;
                                else
                                    parserState = SetParserError(-1, szUnknownChar, lexeme);
                                break;
                            case '<':
                                //  подсмотрим следующий символ
                                if (src.good() && !src.eof())
                                {
                                    char chr = src.get();
                                    if (chr == '/')
                                    {
                                        std::string str = GetLexeme(src);
                                        #ifdef _DEBUG
                                        LogEvent("srcln "+IntToStr(__LINE__)+": Tag open bracket");
                                        #endif

                                        // конец?
                                        if (str == tagName)
                                        {
                                            std::string lex = GetLexeme(src);
                                            #ifdef _DEBUG
                                            LogEvent("srcln "+IntToStr(__LINE__)+": Tag closing");
                                            #endif
                                            if (lex == ">")
                                            {
                                                if ( strcmpi(str.c_str(), "TAIL") == 0)
                                                {
                                                    src.seekg(0, std::ios::end);
                                                    return 0;
                                                }

                                                errorCode = 0;
                                                parserState = psFinish;
                                            } else
                                                parserState = SetParserError(-1, szUnknownChar, lexeme);
                                        }
                                        else
                                            parserState = SetParserError(-1, szWrongTagName, lexeme);
                                    }
                                    else
                                    {// не, субтэг
                                        src.putback(chr);

                                        ReadTag(src, currentNode);

                                        parserState = psSubTag;
                                    }
                                } else
                                    parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                                break;
                            default:
                                parserState = SetParserError(-1, szUnknownChar, lexeme);
                        }
                        break;
                    default:
                        parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                }
                break;
            case psTagClosed:
                switch (GetLexemeType(lexeme)) {
                    case ltString:
                        //  параметр-значение
                        lastParam = lexeme;
                        parserState = psParamRead;
                        break;
                    case ltSymbol:
                        //  только откр. скобка
                        if (lexeme[0] != '<')
                            parserState = SetParserError(-1, szUnknownChar, lexeme);
                        //  подсмотрим следующий символ
                        if (src.good() && !src.eof())
                        {
                            char chr = src.get();
                            if (chr == '/') {
                                // конец?
                                std::string lex = GetLexeme(src);
                                #ifdef _DEBUG
                                LogEvent("srcln "+IntToStr(__LINE__)+": Tag closing");
                                #endif
                                if (lex == tagName) {
                                    lex = GetLexeme(src);
                                    #ifdef _DEBUG
                                    LogEvent("srcln "+IntToStr(__LINE__)+": Tag closing");
                                    #endif
                                    if (lex == ">") {
                                        errorCode = 0;
                                        parserState = psFinish;
                                    } else
                                        parserState = SetParserError(-1, szUnknownChar, lexeme);
                                } else
                                    parserState = SetParserError(-1, szWrongTagName, lexeme);
                            } else {
                                // не, субтэг
                                src.putback(chr);

                                ReadTag(src, currentNode);

                                parserState = psSubTag;
                            }
                        }
                        else
                            parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                        break;
                    default:
                        parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                }
                break;
            case psSubTag:
                switch (GetLexemeType(lexeme)) {
                    case ltString:
                        parserState = SetParserError(-1, szUnknownChar, lexeme);
                        break;
                    case ltSymbol:
                        //  только откр. скобка
                        if (lexeme[0] != '<')
                            parserState = SetParserError(-1, szUnknownChar, lexeme);
                        //  подсмотрим следующий символ
                        if (src.good() && !src.eof())
                        {
                            char chr = src.get();
                            if (chr == '/') {
                                // конец?
                                std::string lex = GetLexeme(src);
                                #ifdef _DEBUG
                                LogEvent("srcln "+IntToStr(__LINE__)+": Tag closing");
                                #endif
                                if (lex == tagName) {
                                    lex = GetLexeme(src);
                                    #ifdef _DEBUG
                                    LogEvent("srcln "+IntToStr(__LINE__)+": Tag closing");
                                    #endif
                                    if (lex == ">") {
                                        errorCode = 0;
                                        parserState = psFinish;
                                    } else
                                        parserState = SetParserError(-1, szUnknownChar, lexeme);
                                } else
                                    parserState = SetParserError(-1, szWrongTagName, lexeme);
                            } else {
                                // не, субтэг
                                src.putback(chr);

                                ReadTag(src, currentNode);

                                parserState = psSubTag;
                            }
                        } else
                            parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                        break;
                    default:
                        parserState = SetParserError(-1, szUnexpectedEof, lexeme);
                }
                break;
        }
    }

    return errorCode;
}

bool XmlDoc::IsLineFeed(char chr)
{
    return ( chr == '\r' || chr == '\n' );
}

std::string __fastcall XmlDoc::GetLexeme(std::istream& src)
{
    PassSeparators(src);

    char chr;
    std::string str;

    if ( src.bad() || src.eof() )
        return str;

    bool quoted = false;

    if ( (docStyle == dsBcNotify) && (parserState == psParamRead) )
    {
        while ( src.good() && !src.eof() )
        {
            chr = src.get();
            if (IsSymbol(chr))
            {
                if (str.empty())
                {
                    str += chr;
                    symbol++;
                } else {
                    src.putback(chr);
                }
                break;
            }
            else if (IsLineFeed(chr))
            {
                line++;
                symbol = 0;
                break;
            } else {
                str += chr;
                //  счётчик знаков
                symbol++;
            }
        }
    }
    else
    {
        while (src.good() && !src.eof())
        {
            chr = src.get();
            if (str.length() == 0)
            {
                symbol++;
                if (chr == '\"') {
                    if (quoted)
                        //  ПУСТОЙ
                        break;
                    else
                        quoted = true;
                } else {
                    str = chr;
                    if (IsSymbol(chr))
                    {
                        // выгрести все пробелы и вывалиться
                        do
                            chr = src.get();
                        while (IsSeparator(chr) && src.good() && !src.eof());
                        src.unget();
                        break;
                    }
                }

            } else {

                if (quoted && chr == '\"') {
                    //  счётчик знаков
                    symbol++;
                    break;

                } else if (IsSymbol(chr) || (!quoted && IsSeparator(chr))) {
                    src.putback(chr);
                    break;
                } else {
                    str += chr;
                    //  счётчик знаков
                    symbol++;
                }
            }
        }
    }

    if ( docStyle == dsBcNotify )
    {
        //удалим лидирующие пробелы
        while (str.size() && str[0] == ' ' )
            str.erase(0, 1);
        //удалим замыкающие пробелы
        while (str.size() && str[str.size()-1] == ' ' )
            str.erase(str.size()-1, 1);
    }

    //MessageBox(NULL, ('"' + str + '"').c_str(), "Lexeme: ", MB_ICONINFORMATION);
    #ifdef _DEBUG
    lastLexeme = str;
    #endif
    return str;
}

int __fastcall XmlDoc::PassSeparators(std::istream& src)
{
    if ( src.bad() || src.eof() )
        return 0;

    char chr = ' ';

    int locSymbols = 0;
    while (src.good() && !src.eof())
    {
        chr = src.get();
        if (!IsSeparator(chr)) {
            src.putback(chr);
            break;
        } else if (IsLineFeed(chr)) {
            if (docStyle == dsBcNotify && parserState == psValRead)
            {
                src.unget();
                break;
            } else {
                line++;
                symbol = 0;
            }
        } else
            symbol++;

        locSymbols++;
    }

    return locSymbols;
}

bool XmlDoc::IsSymbol(char chr)
{

    if (parserState == psParamRead)
        return ((chr == '<') || (chr == '>') || (chr == '='));
    else
        return ( (chr == '<') || (chr == '>') || (chr == '=') || (chr == '/'));
}

bool XmlDoc::IsSeparator(char chr)
{
    if ( docStyle == dsBcNotify )
        return ( IsLineFeed(chr) || (chr == '\t') /*|| (chr == '\0')*/ );
    else
        return ( IsLineFeed(chr) || (chr == '\t') || (chr == ' ') );
}

void __fastcall XmlDoc::SaveTo(const std::string& fileName)
{
    if (rootNodes.empty())
        return;

    std::ofstream logStream(fileName.c_str());

    if (docStyle == dsXml) {
        logStream << "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" << std::endl;
        logStream << "<!-- " << comment.c_str() << " -->" << std::endl;
    }

    for (std::vector<XmlDocNode*>::iterator li = rootNodes.begin(); li != rootNodes.end(); li++)
        (*li)->SaveToStream(logStream, docStyle);

    logStream.flush();
    logStream.close();
}

__fastcall XmlDocNode::XmlDocNode(const std::string& name)
{
    this->name = name;
}

/*************************************
    AddElement(...) serie
*/
void __fastcall XmlDocNode::AddElement(const std::string& name, const std::string& value)
{
    elements.push_back(std::pair<std::string, std::string>(name, value));
}

void __fastcall XmlDocNode::AddElement(const std::string& name, const float& value)
{
    std::ostringstream oss; oss << value; AddElement(name, oss.str().c_str());
}

void __fastcall XmlDocNode::AddElement(const std::string& name, const double& value)
{
    std::ostringstream oss; oss << value; AddElement(name, oss.str().c_str());
}

void __fastcall XmlDocNode::AddElement(const std::string& name, const int& value)
{
    std::ostringstream oss; oss << value; AddElement(name, oss.str().c_str());
}
void __fastcall XmlDocNode::AddElement(const std::string& name, const unsigned int& value)
{
    std::ostringstream oss; oss << value; AddElement(name, oss.str().c_str());
}
//******************************************************************************

XmlDocNode* __fastcall XmlDocNode::AddNode(const std::string& name)
{
    XmlDocNode* newNode = new XmlDocNode(name);
    subNodes.push_back(newNode);
    return newNode;
}

void __fastcall XmlDocNode::SaveToStream(std::ostream& stream, DocStyle docStyle)
{
    stream << '<' <<  name.c_str();

    if (docStyle == dsXml) {

        //  сохраняем в XML-стиле - елементы в тэге, тэги без субтэгов закрыты

        if (!elements.empty()) {
            for (std::vector<std::pair<std::string, std::string> >::iterator li = elements.begin(); li != elements.end(); li++) {
                stream << ' ' << li->first.c_str() << "=\"" << li->second.c_str() << '\"' ;
            }
        }

        if (!subNodes.empty()) {

            stream << '>' << std::endl;
            for (std::vector<XmlDocNode*>::iterator li = subNodes.begin(); li != subNodes.end(); li++) {
                (*li)->SaveToStream(stream, docStyle);
            }
            stream << "</" << name.c_str() << '>' << std::endl;

        } else {
            stream << "/>" << std::endl;
        }

    } else if (docStyle == dsBcNotify) {

        //  сохраняем в DA/DT-стиле - построчно

        stream << '>' << std::endl;
        for (std::vector<std::pair<std::string, std::string> >::iterator li = elements.begin(); li != elements.end(); li++) {
            stream << li->first.c_str() << "=" << li->second.c_str() << std::endl;
            }
        for (std::vector<XmlDocNode*>::iterator li = subNodes.begin(); li != subNodes.end(); li++) {
            (*li)->SaveToStream(stream, docStyle);
        }
        stream << "</" << name.c_str() << '>' << std::endl;

    }

}

__fastcall XmlDocNode::~XmlDocNode()
{
    for (std::vector<XmlDocNode*>::iterator li = subNodes.begin(); li != subNodes.end(); li++)
        delete *li;
    subNodes.clear();
}

#ifdef _DEBUG
void __fastcall XmlDoc::LogEvent(String msg)
{
    xmlLog.LogEvent(String().sprintf("line %d, symbol %d, state %s, lex '%s': ", line, symbol, stateName[parserState], lastLexeme.c_str()) + msg);
}
#endif

