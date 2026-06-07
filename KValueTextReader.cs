using System.Text;

internal class KValueTextReader : StreamReader
{
    internal static Dictionary<char, char> EscapedMapping = new Dictionary<char, char>
    {
        { '\\', '\\' },
        { 'n', '\n' },
        { 'r', '\r' },
        { 't', '\t' }
    };

    public KValueTextReader(KValue kv, Stream input)
        : base(input)
    {
        KValue kValue = kv;
        do
        {
            string text = ReadToken(out var wasQuoted, out var wasConditional);
            if (string.IsNullOrEmpty(text))
            {
                break;
            }
            if (kValue == null)
            {
                kValue = new KValue(text);
            }
            else
            {
                kValue.Name = text;
            }
            text = ReadToken(out wasQuoted, out wasConditional);
            if (wasConditional)
            {
                text = ReadToken(out wasQuoted, out wasConditional);
            }
            if (!text.StartsWith("{") || wasQuoted)
            {
                break;
            }
            RecursiveLoadFromBuffer(kValue, this);
            kValue = null;
        }
        while (!base.EndOfStream);
    }

    private void RecursiveLoadFromBuffer(KValue k, KValueTextReader kvr)
    {
        while (true)
        {
            bool wasQuoted;
            bool wasConditional;
            string text = kvr.ReadToken(out wasQuoted, out wasConditional);
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("RecursiveLoadFromBuffer: got EOF or empty keyname");
            }
            if (text.StartsWith("}") && !wasQuoted)
            {
                return;
            }
            KValue kValue = new KValue(text)
            {
                Children = new List<KValue>()
            };
            k.Children.Add(kValue);
            string text2 = kvr.ReadToken(out wasQuoted, out wasConditional);
            if (wasConditional && text2 != null)
            {
                text2 = kvr.ReadToken(out wasQuoted, out wasConditional);
            }
            if (text2 == null)
            {
                throw new Exception("RecursiveLoadFromBuffer:  got NULL key");
            }
            if (text2.StartsWith("}") && !wasQuoted)
            {
                throw new Exception("RecursiveLoadFromBuffer:  got } in key");
            }
            if (text2.StartsWith("{") && !wasQuoted)
            {
                RecursiveLoadFromBuffer(kValue, kvr);
                continue;
            }
            if (wasConditional)
            {
                break;
            }
            kValue.Value = text2;
            kValue.KType = KValueType.String;
        }
        throw new Exception("RecursiveLoadFromBuffer:  got conditional between key and value");
    }

    private void EatWhiteSpace()
    {
        while (!base.EndOfStream && char.IsWhiteSpace((char)Peek()))
        {
            Read();
        }
    }

    private bool EatCppComment()
    {
        if (!base.EndOfStream)
        {
            if ((ushort)Peek() == 47)
            {
                ReadLine();
                return true;
            }
            return false;
        }
        return false;
    }

    public string ReadToken(out bool wasQuoted, out bool wasConditional)
    {
        wasQuoted = false;
        wasConditional = false;
        do
        {
            EatWhiteSpace();
            if (base.EndOfStream)
            {
                return null;
            }
        }
        while (EatCppComment());
        if (!base.EndOfStream)
        {
            char c = (char)Peek();
            switch (c)
            {
                case '"':
                    {
                        wasQuoted = true;
                        Read();
                        StringBuilder stringBuilder2 = new StringBuilder();
                        while (!base.EndOfStream)
                        {
                            if (Peek() == 92)
                            {
                                Read();
                                char c4 = (char)Read();
                                if (EscapedMapping.TryGetValue(c4, out var value))
                                {
                                    stringBuilder2.Append(value);
                                }
                                else
                                {
                                    stringBuilder2.Append(c4);
                                }
                            }
                            else
                            {
                                if (Peek() == 34)
                                {
                                    break;
                                }
                                stringBuilder2.Append((char)Read());
                            }
                        }
                        Read();
                        return stringBuilder2.ToString();
                    }
                default:
                    {
                        bool flag = false;
                        int num = 0;
                        StringBuilder stringBuilder = new StringBuilder();
                        while (!base.EndOfStream)
                        {
                            c = (char)Peek();
                            char c2 = c;
                            char c3 = c2;
                            if ((uint)c3 <= 91u)
                            {
                                if (c3 == '"')
                                {
                                    break;
                                }
                                if (c3 == '[')
                                {
                                    flag = true;
                                }
                            }
                            else if (c3 == '{' || c3 == '}')
                            {
                                break;
                            }
                            if (c == ']' && flag)
                            {
                                wasConditional = true;
                            }
                            if (char.IsWhiteSpace(c))
                            {
                                break;
                            }
                            if (num >= 1023)
                            {
                                throw new Exception("ReadToken overflow");
                            }
                            stringBuilder.Append(c);
                            Read();
                        }
                        return stringBuilder.ToString();
                    }
                case '{':
                case '}':
                    Read();
                    return c.ToString();
            }
        }
        return null;
    }
}
