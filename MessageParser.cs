using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherCockpit
{
    public enum TokenType
    {
        Text,
        BoldOn,
        BoldOff,
        ItalicOn,
        ItalicOff,
        Color,      // #RRGGBB
        Pause,      // /pN
        Speed       // /sN
    }

    public class MessageToken
    {
        public TokenType Type { get; set; }
        public string? Value { get; set; }
    }

    public static class MessageParser
    {
        public static List<MessageToken> Parse(string message)
        {
            var tokens = new List<MessageToken>();
            var sb = new StringBuilder();

            void FlushText()
            {
                if (sb.Length > 0)
                {
                    tokens.Add(new MessageToken
                    {
                        Type = TokenType.Text,
                        Value = sb.ToString()
                    });
                    sb.Clear();
                }
            }

            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];

                if (c == '/')
                {
                    FlushText();

                    if (i + 1 < message.Length)
                    {
                        char cmd = message[i + 1];
                        i++;

                        switch (cmd)
                        {
                            case 'b':
                                tokens.Add(new MessageToken { Type = TokenType.BoldOn });
                                break;
                            case 'B':
                                tokens.Add(new MessageToken { Type = TokenType.BoldOff });
                                break;
                            case 'i':
                                tokens.Add(new MessageToken { Type = TokenType.ItalicOn });
                                break;
                            case 'I':
                                tokens.Add(new MessageToken { Type = TokenType.ItalicOff });
                                break;
                            case 'p':
                                {
                                    var num = ReadNumber(message, ref i);
                                    tokens.Add(new MessageToken { Type = TokenType.Pause, Value = num });
                                    break;
                                }
                            case 's':
                                {
                                    var num = ReadNumber(message, ref i);
                                    tokens.Add(new MessageToken { Type = TokenType.Speed, Value = num });
                                    break;
                                }
                            default:
                                sb.Append('/').Append(cmd);
                                break;
                        }
                    }
                }
                else if (c == '#')
                {
                    FlushText();

                    var color = ReadColor(message, ref i);
                    tokens.Add(new MessageToken { Type = TokenType.Color, Value = color });
                }
                else
                {
                    sb.Append(c);
                }
            }

            FlushText();
            return tokens;
        }

        private static string ReadNumber(string s, ref int i)
        {
            int start = i + 1;
            int j = start;
            while (j < s.Length && char.IsDigit(s[j])) j++;
            var num = s.Substring(start, j - start);
            i = j - 1;
            return num;
        }

        private static string ReadColor(string s, ref int i)
        {
            int start = i;
            int j = start + 1;
            while (j < s.Length && (char.IsLetterOrDigit(s[j]) || s[j] == '#')) j++;
            var col = s.Substring(start, j - start);
            i = j - 1;
            return col;
        }

        public static string StripCodes(string raw)
        {
            var tokens = Parse(raw);
            return string.Concat(tokens
                .Where(t => t.Type == TokenType.Text)
                .Select(t => t.Value));
        }

    }
}
