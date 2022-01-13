/*
    MultiMAC: Authenticate multiple inputs easily.
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

using Sodium;

namespace MultiMAC;

public static class MultiMac
{
    public static byte[] Compute(byte[] key1, byte[] key2, TagLength tagLength, params byte[][] inputs)
    {
        ParameterValidation.Key(key1, (int)tagLength);
        ParameterValidation.Key(key2, (int)tagLength);
        ParameterValidation.Inputs(inputs);
        var tag = new byte[(int)tagLength];
        Array.Copy(key1, tag, key1.Length);
        foreach (var input in inputs)
        {
            tag = GenericHash.Hash(input, tag, (int)tagLength);
        }
        return GenericHash.Hash(tag, key2, (int)tagLength);
    }
    
    public static bool Verify(byte[] tag, byte[] key1, byte[] key2, params byte[][] inputs)
    {
        ParameterValidation.Tag(tag, tag?.Length ?? 0);
        ParameterValidation.Key(key1, tag.Length);
        ParameterValidation.Key(key2, tag.Length);
        ParameterValidation.Inputs(inputs);
        byte[] computedTag = Compute(key1, key2, (TagLength)tag.Length, inputs);
        return Utilities.Compare(tag, computedTag);
    }
}