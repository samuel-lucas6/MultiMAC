[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/samuel-lucas6/MultiMAC/blob/main/LICENSE)

# MultiMAC
Authenticate multiple inputs easily using keyed BLAKE2b in [libsodium](https://doc.libsodium.org/).

## Justification
The [standard](https://github.com/samuel-lucas6/Cryptography-Guidelines#notes-2) way of [safely](https://soatok.blog/2021/07/30/canonicalization-attacks-against-macs-and-signatures/) computing a MAC for multiple inputs requires worrying about concatenating arrays and converting the length of each array to a fixed number of bytes, such as 4 bytes to represent an integer, consistently in either big- or little-endian, regardless of the endianness of the machine. This is annoying to implement and possibly less efficient than the following approach [discussed](https://neilmadden.blog/2021/10/27/multiple-input-macs/) by [Neil Madden](https://neilmadden.blog/), author of [API Security in Action](https://www.manning.com/books/api-security-in-action?a_aid=api_security_in_action).

## Installation
### NuGet
You can find the NuGet package [here](https://www.nuget.org/packages/MultiMAC).

The easiest way to install this is via the NuGet Package Manager in [Visual Studio](https://visualstudio.microsoft.com/vs/), as explained [here](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio). [JetBrains Rider](https://www.jetbrains.com/rider/) also has a package manager, and instructions can be found [here](https://www.jetbrains.com/help/rider/Using_NuGet.html).

### Manual
1. Install the [Sodium.Core](https://www.nuget.org/packages/Sodium.Core) NuGet package in [Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio).
2. Download the latest [release](https://github.com/samuel-lucas6/MultiMAC/releases/latest).
3. Move the downloaded `.dll` file into your Visual Studio project folder.
4. Click on the `Project` tab and `Add Project Reference...` in Visual Studio.
5. Go to `Browse`, click the `Browse` button, and select the downloaded `.dll` file.
6. Add `using MultiMAC;` to the top of each code file that will use the library.

### Requirements
Note that the [libsodium](https://doc.libsodium.org/) library requires the [Visual C++ Redistributable for Visual Studio 2015-2019](https://support.microsoft.com/en-us/help/2977003/the-latest-supported-visual-c-downloads) to work on Windows. If you want your program to be portable, then you must keep the relevant (x86 or x64) `vcruntime140.dll` file in the same folder as your executable on Windows.

## Usage
⚠️**WARNING: Never use the same key for `key1` and `key2`.**
```c#
const TagLength tagLength = TagLength.BLAKE2b256;

// Both keys should be derived using a KDF in practice (e.g. Argon2, HKDF, etc)
byte[] key1 = SodiumCore.GetRandomBytes((int)tagLength);

// The keys must be the same size as the tag length
byte[] key2 = SodiumCore.GetRandomBytes((int)tagLength);

// Gather up the byte arrays to authenticate
byte[] input1 = Encoding.UTF8.GetBytes("Po");

byte[] input2 = Encoding.UTF8.GetBytes("ta");

byte[] input3 = Encoding.UTF8.GetBytes("toes");

byte[] input4 = Encoding.UTF8.GetBytes("Boil 'em, mash 'em, stick 'em in a stew");

// Compute a 256-bit tag
byte[] tag = MultiMac.Compute(key1, key2, tagLength, input1, input2, input3, input4);

// Verify a tag
bool validTag = MultiMac.Verify(tag, key1, key2, input1, input2, input3, input4);
```
