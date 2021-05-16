using System;

namespace ConsoleApp1
{
    public class Token
    {
        private String _data;
        private TokenType _type;
        private int _shift;

        public Token(String str, TokenType type, int shift)
        {
            _data = str;
            _type = type;
            _shift = shift;
        }

        public String Output()
        {
            return _data;
        }

        public TokenType GetTypeKind()
        {
            return _type;
        }

        public int GetShift()
        {
            return _shift;
        }
    }
}