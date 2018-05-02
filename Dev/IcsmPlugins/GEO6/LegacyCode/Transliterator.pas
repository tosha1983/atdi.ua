unit Transliterator;

interface

function RusToEng(rus: string): string;

implementation

uses
    SysUtils;

function RusCharToEng(ch: char): string;
var s: string;
begin
  s := AnsiLowerCase(ch);
  ch := s[1];
  case ch of
    'а': result := 'a';
    'б': result := 'b';
    'в': result := 'v';
    'г': result := 'g';
    'д': result := 'd';
    'е': result := 'e';
    'ё': result := 'e';
    'ж': result := 'zh';
    'з': result := 'z';
    'и': result := 'i';
    'й': result := 'y';
    'к': result := 'k';
    'л': result := 'l';
    'м': result := 'm';
    'н': result := 'n';
    'о': result := 'o';
    'п': result := 'p';
    'р': result := 'r';
    'с': result := 's';
    'т': result := 't';
    'у': result := 'u';
    'ф': result := 'f';
    'х': result := 'kh';
    'ц': result := 'ts';
    'ч': result := 'ch';
    'ш': result := 'sh';
    'щ': result := 'sch';
    'ъ': result := '';
    'ы': result := 'y';
    'ь': result := '';
    'э': result := 'e';
    'ю': result := 'yu';
    'я': result := 'ya';
    'і': result := 'i';
    'ї': result := 'i';
    'є': result := 'e';
  else
    result := ch;
  end;
end;


{
  Функция определяет, является ли строка русской.
  Если кол-во русских (кирилличексих) символов больше чем
  остальных - строка русская.
}
function RusskoeSlovo(s: string): boolean;
var n: integer;
    r,e: integer;
    ch: char;
begin
  r := 0;
  e := 0;
  for n := 1 to Length(s) do
  begin
    ch := s[n];
    if Byte(ch) > 127 then r := r + 1 else e := e + 1;
  end;
{
  даже если нерусских букв больше в два раза !!!
}
  result := (r >= e div 2);

end;

function RusToEng(rus: string): string;
var n: integer;
    ch: char;
    eng: string;
    b: boolean;
begin
  eng := '';
  b := RusskoeSlovo(rus);

  for n := 1 to Length(rus) do
  begin
    ch := rus[n];
{
  Если мы уверены что слово русское, заменяем "H" (англ. ЄЙЧ) на русское "н" (рус. ЭН)
}
    if b and ((ch = '?') or (ch = 'H')) then ch := '?';
    eng := eng + RusCharToEng(ch);
  end;
  result := AnsiUpperCase(eng);
end;

end.