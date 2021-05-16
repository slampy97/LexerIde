using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ConsoleApp1
{
    [TestFixture]
    public class LexerTest
    {
        [Test]
        public static void Test()
        {
            var lexer = new Lexer();
            var tableTest = new List<(string, string, TokenType)>()
            {
                ("_1var 2_var 3_var", "_1var", TokenType.Identifiers),
                ("va2r dsfdsfs", "va2r", TokenType.Identifiers),
                ("ca_3r dfsfs sdfs", "ca_3r", TokenType.Identifiers),
                ("_111 dfdsf f", "_111", TokenType.Identifiers),
                ("11111 232f df", "11111", TokenType.Numbers),
                ("+1123 df ff ", "+1123", TokenType.Numbers),
                ("%010101 sfds 23", "%010101", TokenType.Numbers),
                ("-%010101 fd df ", "-%010101", TokenType.Numbers),
                ("&0182 fd df3 3", "&0182", TokenType.Numbers),
                ("+&11 df  fdf", "+&11", TokenType.Numbers),
                ("$12390 FDFD ", "$12390", TokenType.Numbers),
                ("$Ad12F DFS", "$Ad12F", TokenType.Numbers),
                ("-$1ff fdf d ds23", "-$1ff", TokenType.Numbers),
                ("  'abc''bcd'#15#24'fff' 124# FDFSd dfdf", "'abc''bcd'#15#24'fff'", TokenType.CharacterString),
                (" #123'dfdf' df d sfs s f", "#123'dfdf'", TokenType.CharacterString),
                (" #123#4444 FDFSF343 sf234 ", "#123#4444", TokenType.CharacterString),
                ("'DFSFSFSFSFS  F' F sd s fs f", "'DFSFSFSFSFS  F'", TokenType.CharacterString),
                ("{ fd(*fd f \nd\n {d//f} *) } DF DF DF { dfd}", "{ fd(*fd f \nd\n {d//f} *) }", TokenType.Comments),
                ("(* \n dd\n { dfd} d *) fd { } *) DFD { { DFD }}", "(* \n dd\n { dfd} d *)", TokenType.Comments),
                ("// comment 1 (* co{m//(*m*)ent} 2 *) \n dfdf ds{ }", "// comment 1 (* co{m//(*m*)ent} 2 *)",
                    TokenType.Comments),
                ("//FDFDF {} DFDF {{}}F DFD \n fdfd ", "//FDFDF {} DFDF {{}}F DFD", TokenType.Comments)
            };

            foreach (var (input, expected, type) in tableTest)
            {
                Token getToken = lexer.go(input, 0);
                Assert.AreEqual(expected, getToken.Output());
                Assert.AreEqual(type, getToken.GetTypeKind());
            }
        }
    }
}