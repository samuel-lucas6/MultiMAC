/*
    MultiMAC: Authenticate multiple inputs easily using keyed BLAKE2b.
    Copyright (c) 2022 Samuel Lucas
    
    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

namespace MultiMAC;

internal static class ParameterValidation
{
    internal static void Key(byte[] key, int validKeyLength)
    {
        if (key == null) { throw new ArgumentNullException(nameof(key), "Neither key can be null."); }
        if (key.Length != validKeyLength) { throw new ArgumentOutOfRangeException(nameof(key), key.Length, $"Both keys must be {validKeyLength} bytes in length."); }
    }
    
    internal static void Inputs(params byte[][] inputs)
    {
        if (inputs == null) { throw new ArgumentNullException(nameof(inputs), "The input cannot be null."); }
        foreach (var input in inputs)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input), "None of the inputs can be null."); }
            if (input.Length == 0) { throw new ArgumentOutOfRangeException(nameof(input), "None of the inputs can be empty."); }
        }
    }
    
    internal static void Tag(byte[] tag)
    {
        if (tag == null) { throw new ArgumentNullException(nameof(tag), "The tag cannot be null."); }
        if (tag.Length != (int)TagLength.BLAKE2b256 & tag.Length != (int)TagLength.BLAKE2b384 & tag.Length != (int)TagLength.BLAKE2b512) { throw new ArgumentOutOfRangeException(nameof(tag), tag.Length, $"The tag must be {(int)TagLength.BLAKE2b256}, {(int)TagLength.BLAKE2b384}, or {(int)TagLength.BLAKE2b512} bytes in length."); }
    }
}