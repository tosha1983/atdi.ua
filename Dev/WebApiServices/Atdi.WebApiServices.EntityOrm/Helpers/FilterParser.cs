using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices.EntityOrm.Helpers
{
    class FilterParser
    {


        private class TokenReader
        {
            private readonly string _text;
            private int _position;
            private StringBuilder _token;
            public TokenReader(string text)
            {
                this._text = text;
                this._position = -1;
            }

            public bool Read()
            {
                ++this._position;
                if (_text.Length <= this._position)
                {
                    return false;
                }
                _token = new StringBuilder();

                while (this._position < _text.Length)
                {
                    var c = this._text[_position];
                    if (c == '(')
                    {
                        if (_token.Length > 0)
                        {
                            --this._position;
                            return true;
                        }

                        _token.Append(c);
                        return true;
                    }
                    if (c == ')')
                    {
                        if (_token.Length > 0)
                        {
                            --this._position;
                            return true;
                        }

                        _token.Append(c);
                        return true;
                    }
                    if (c == ' ' || char.IsWhiteSpace(c))
                    {
                        if (_token.Length > 0)
                        {
                            return true;
                        }
                        // игноре
                    }
                    else
                    {
                        _token.Append(c);
                    }

                    ++_position;
                }

                return true;
            }

            public string Token => _token.ToString();
        }

        private enum LexemeKind
        {
            Open,
            Close,
            Eq,
            Nq,
            Gt,
            Ge,
            Lt,
            Le,
            Value,
            Path,
            And,
            Or,
            Is,
            Null,
            Not,
            In,
            Between
        }

        private class Lexeme
        {
            public LexemeKind Kind { get; set; }

            public string Token { get; set; }

            public override string ToString()
            {
                return $"Kind = '{Kind}', Token = '{Token}'";
            }
        }
        public static Condition Parse(string[] filters)
        {
            return new ComplexCondition
            {
                Operator = LogicalOperator.And,
                Conditions = filters.Select(f => Parse(f)).ToArray()
            };
        }

        /// <summary>
        /// (f1 is null) or (f2 eq 2L) and (f3 in (1,2,3,4)) or  ()
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Condition Parse(string filter)
        {

            var tokens = new List<string>();
            var tokenReader = new TokenReader(filter);
            while (tokenReader.Read())
            {
                tokens.Add(tokenReader.Token);
            }

            var lexemes = new Lexeme[tokens.Count];
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                lexemes[i] = ParseLexeme(token);
            }

            var index = 0;
            var condition = ParseExpression(lexemes, ref index);

            return condition;
        }

        private static Condition ParseExpression(Lexeme[] lexemes, ref int index)
        {

            var orConditions = new List<Condition[]>();
            var lastConditions = new List<Condition>();

            do
            {
                var lexeme = lexemes[index];
                if (lexeme.Kind == LexemeKind.Close)
                {
                    --index;
                    orConditions.Add(lastConditions.ToArray());
                    break;
                }
                if (lexeme.Kind == LexemeKind.Open)
                {
                    ++index;
                    var lastCondition = ParseExpression(lexemes, ref index);
                    lastConditions.Add(lastCondition);
                    ++index;
                    if (index == lexemes.Length || lexemes[index].Kind != LexemeKind.Close)
                    {
                        throw new InvalidOperationException($"Expected closing bracket");
                    }
                    ++index;
                }
                else if (lexeme.Kind == LexemeKind.Path || lexeme.Kind == LexemeKind.Value)
                {
                    var lastCondition = ParseLogicalExpression(lexemes, ref index);
                    lastConditions.Add(lastCondition);
                    ++index;
                }
                else
                {
                    throw new InvalidOperationException($"Expected starting filter expression");
                }

                if (index == lexemes.Length)
                {
                    orConditions.Add(lastConditions.ToArray());
                    break;
                }

                lexeme = lexemes[index];
                if (lexeme.Kind == LexemeKind.And)
                {
                    ++index;
                }
                else if (lexeme.Kind == LexemeKind.Or)
                {
                    orConditions.Add(lastConditions.ToArray());
                    lastConditions = new List<Condition>();
                    ++index;
                }
            }
            while (index < lexemes.Length);



            var condition = new ComplexCondition
            {
                Operator = LogicalOperator.Or,
                Conditions = orConditions.Select(c => new ComplexCondition
                {
                    Operator = LogicalOperator.And,
                    Conditions = c
                }).ToArray()
            };

            if (condition.Conditions.Length == 1)
            {
                var orC = (ComplexCondition)condition.Conditions[0];
                if (orC.Conditions.Length == 1)
                {
                    return orC.Conditions[0];
                }
                return orC;
            }

            return condition;
        }

        private static ConditionExpression ParseLogicalExpression(Lexeme[] lexemes, ref int index)
        {
            var condition = new ConditionExpression();

            var leftOperandLexeme = lexemes[index];
            var rightOperandLexeme = default(Lexeme);

            ++index;
            var lexeme = lexemes[index];

            switch (lexeme.Kind)
            {
                case LexemeKind.Eq:
                    condition.Operator = ConditionOperator.Equal;
                    ++index;
                    rightOperandLexeme = lexemes[index];
                    break;
                case LexemeKind.Nq:
                    condition.Operator = ConditionOperator.NotEqual;
                    ++index;
                    rightOperandLexeme = lexemes[index];
                    break;
                case LexemeKind.Gt:
                    condition.Operator = ConditionOperator.GreaterThan;
                    ++index;
                    rightOperandLexeme = lexemes[index];
                    break;
                case LexemeKind.Ge:
                    condition.Operator = ConditionOperator.GreaterEqual;
                    ++index;
                    rightOperandLexeme = lexemes[index];
                    break;
                case LexemeKind.Lt:
                    condition.Operator = ConditionOperator.LessThan;
                    ++index;
                    rightOperandLexeme = lexemes[index];
                    break;
                case LexemeKind.Le:
                    condition.Operator = ConditionOperator.LessEqual;
                    ++index;
                    rightOperandLexeme = lexemes[index];
                    break;
                case LexemeKind.Is:
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind == LexemeKind.Null)
                    {
                        condition.Operator = ConditionOperator.IsNull;
                    }
                    else if (lexeme.Kind == LexemeKind.Not)
                    {
                        ++index;
                        lexeme = lexemes[index];
                        if (lexeme.Kind == LexemeKind.Null)
                        {
                            condition.Operator = ConditionOperator.IsNotNull;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Expected corrected expression");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    break;
                case LexemeKind.In:
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind != LexemeKind.Open)
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind != LexemeKind.Value)
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    var values = ValueParser.ParseValues(lexeme.Token);
                    condition.RightOperand = ValuesOperand.CreateBy(values);
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind != LexemeKind.Close)
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    condition.Operator = ConditionOperator.In;
                    break;
                case LexemeKind.Between:
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind != LexemeKind.Value)
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    var value1 = ValueParser.ParseValue(lexeme.Token);
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind != LexemeKind.And)
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    ++index;
                    lexeme = lexemes[index];
                    if (lexeme.Kind != LexemeKind.Value)
                    {
                        throw new InvalidOperationException($"Expected corrected expression");
                    }
                    var value2 = ValueParser.ParseValue(lexeme.Token);
                    condition.RightOperand = ValuesOperand.CreateBy(new object[] { value1, value2 });
                    condition.Operator = ConditionOperator.Between;
                    break;
                default:
                    throw new InvalidOperationException($"Expected corrected expression");
            }

            condition.LeftOperand = ParseOperand(leftOperandLexeme);
            if (rightOperandLexeme != null)
            {
                condition.RightOperand = ParseOperand(rightOperandLexeme);
            }
            return condition;
        }

        private static Operand ParseOperand(Lexeme lexeme)
        {
            if (lexeme.Kind == LexemeKind.Path)
            {
                return new ColumnOperand
                {
                    ColumnName = lexeme.Token
                };
            }
            if (lexeme.Kind == LexemeKind.Value)
            {
                return ValueOperand.CreateBy(ValueParser.ParseValue(lexeme.Token));

            }
            throw new InvalidOperationException($"Expected corrected expression");
        }
        private static Lexeme ParseLexeme(string token)
        {
            if (token.Equals("("))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Open,
                    Token = token
                };
            }
            if (token.Equals(")"))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Close,
                    Token = token
                };
            }
            if (token.Equals("And", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.And,
                    Token = token
                };
            }
            if (token.Equals("Or", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Or,
                    Token = token
                };
            }
            if (token.Equals("Eq", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Eq,
                    Token = token
                };
            }
            if (token.Equals("Ge", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Ge,
                    Token = token
                };
            }
            if (token.Equals("Gt", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Gt,
                    Token = token
                };
            }
            if (token.Equals("In", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.In,
                    Token = token
                };
            }
            if (token.Equals("Is", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Is,
                    Token = token
                };
            }
            if (token.Equals("Le", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Le,
                    Token = token
                };
            }
            if (token.Equals("Lt", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Lt,
                    Token = token
                };
            }
            if (token.Equals("Not", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Not,
                    Token = token
                };
            }
            if (token.Equals("Nq", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Nq,
                    Token = token
                };
            }
            if (token.Equals("Between", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Between,
                    Token = token
                };
            }
            if (token.Equals("Null", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Null,
                    Token = token
                };
            }
            if (token.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Value,
                    Token = token
                };
            }
            if (token.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Value,
                    Token = token
                };
            }
            if (char.IsLetter(token[0]))
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Path,
                    Token = token
                };
            }
            if (token[0] == '@')
            {
                return new Lexeme
                {
                    Kind = LexemeKind.Path,
                    Token = token.Substring(1, token.Length - 1)
                };
            }
            return new Lexeme
            {
                Kind = LexemeKind.Value,
                Token = token
            };


        }
    }
}
