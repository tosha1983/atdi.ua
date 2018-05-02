#ifndef LisTypesH
#define LisTypesH

// aliases for list interface to allow objectgrid operate with any type of objects

namespace LisProp {

enum LisPropType { ptString, ptCoord, ptDouble, ptInt, ptkHz, ptdBWt, ptdBkWt };

struct LisColumnInfo
{
    AnsiString title;
    AnsiString propName;
    TAlignment am;
    LisPropType propType;
    int width;
    TFontStyles fontStyle;

    LisColumnInfo() {};
    LisColumnInfo(const LisColumnInfo& src) { *this = src; }
    LisColumnInfo& operator=(const LisColumnInfo& src)
    {
        title = src.title; propName = src.propName; am = src.am; propType = src.propType;
        width = src.width; fontStyle = src.fontStyle;
        return *this;
    }
};

};

#endif
