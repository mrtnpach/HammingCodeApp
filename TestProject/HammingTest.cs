using HammingCode.App.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestProject
{
    public class HammingTest
    {
        HammingBlock hb = new HammingBlock();

        [Theory]
        [InlineData(1, 2)]
        [InlineData(4, 3)]
        [InlineData(3, 3)]
        [InlineData(7, 4)]
        [InlineData(12, 5)]
        public void CalculateParityBits(int n, int expected)
        {
            int p = hb.CalculateRequiredParityBits(n);

            Assert.Equal(expected, p);
        }

        [Fact]
        public void CalculateHammingCodeOfBaseMessage()
        {
            var m = new Message("01001101", isAscii: true);

            var result = hb.GetResult(m);

            Assert.Equal("010011100101", result.ASCIIString);
        }

        [Fact]
        public void CalculateHammingCodeOfChannelMessage()
        {
            var m = new Message("a", isAscii: false);

            var result = hb.GetResult(m);

            Assert.Equal("011000000110", result.ASCIIString);
        }

        [Theory]
        [InlineData("10001100101", 8)]
        [InlineData("001100110001", 1)]
        [InlineData("001100110010", 2)]
        [InlineData("001100111000", 4)]
        [InlineData("011000000111", 1)]
        [InlineData("011000000100", 2)]
        [InlineData("011000000010", 3)]
        public void DetermineErrorPosition(string message, int position)
        {
            var m = new Message(message, isAscii: true);

            int result = hb.DetermineErrorPosition(m);

            Assert.Equal(position, result);
        }

        [Fact]
        public void ExtractContent()
        {
            var m = new Message("a", isAscii: false);
            string ascii = m.ASCIIString;
            var h = new Message("011000000110", isAscii: true);

            string result = hb.ExtractInnerMessage(h);

            Assert.Equal(ascii, result);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("a2")]
        [InlineData("a2!")]
        [InlineData("a)1!")]
        [InlineData("a%c4&")]
        [InlineData("abc4&a")]
        [InlineData("ab$c4&a")]
        public void GetExtractedContent(string originalContent)
        {
            var m = new Message(originalContent, isAscii: false);
            string ascii = m.ASCIIString;
            var h = hb.GetResult(m);
            string content = hb.ExtractInnerMessage(h);

            var result = hb.TranslateAsciiStringToString(content);

            Assert.Equal(originalContent, result);
        }
    }
}
