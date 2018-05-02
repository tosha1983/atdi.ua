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
    '�': result := 'a';
    '�': result := 'b';
    '�': result := 'v';
    '�': result := 'g';
    '�': result := 'd';
    '�': result := 'e';
    '�': result := 'e';
    '�': result := 'zh';
    '�': result := 'z';
    '�': result := 'i';
    '�': result := 'y';
    '�': result := 'k';
    '�': result := 'l';
    '�': result := 'm';
    '�': result := 'n';
    '�': result := 'o';
    '�': result := 'p';
    '�': result := 'r';
    '�': result := 's';
    '�': result := 't';
    '�': result := 'u';
    '�': result := 'f';
    '�': result := 'kh';
    '�': result := 'ts';
    '�': result := 'ch';
    '�': result := 'sh';
    '�': result := 'sch';
    '�': result := '';
    '�': result := 'y';
    '�': result := '';
    '�': result := 'e';
    '�': result := 'yu';
    '�': result := 'ya';
    '�': result := 'i';
    '�': result := 'i';
    '�': result := 'e';
  else
    result := ch;
  end;
end;


{
  ������� ����������, �������� �� ������ �������.
  ���� ���-�� ������� (�������������) �������� ������ ���
  ��������� - ������ �������.
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
  ���� ���� ��������� ���� ������ � ��� ���� !!!
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
  ���� �� ������� ��� ����� �������, �������� "H" (����. ���) �� ������� "�" (���. ��)
}
    if b and ((ch = '?') or (ch = 'H')) then ch := '?';
    eng := eng + RusCharToEng(ch);
  end;
  result := AnsiUpperCase(eng);
end;

end.