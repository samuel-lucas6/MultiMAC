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

## Benchmarks
The following benchmarks were done using [BenchmarkDotNet](https://benchmarkdotnet.org/) in a .NET 6 console application. A plaintext file and 16 random bytes were used as inputs, and the key size was equal to the tag length.

- BLAKE2b refers to concatenating multiple inputs via array copying (`BLAKE2b(message: additionalData || fileBytes || additionalDataLength || fileBytesLength, key)`).
- MultiMAC-BLAKE2b refers to using this library, which employs a cascade construction (`BLAKE2b(message: BLAKE2b(message: fileBytes, key: BLAKE2b(message: additionalData, key1)), key2)`).

In sum, the MultiMAC cascade approach is noticeably faster for large inputs, slightly faster for medium inputs, and slower for very small inputs. It is also significantly easier and safer to use, which suggests it should be offered in cryptographic libraries.

### 512-byte file
|     Method |     Mean |   Error |  StdDev |
|:----------:|:--------:|:-------:|:-------:|
| BLAKE2b256 | 646.3 ns | 0.77 ns | 0.68 ns |
| BLAKE2b384 | 649.3 ns | 2.77 ns | 2.32 ns |
| BLAKE2b512 | 660.8 ns | 0.67 ns | 0.53 ns |
| MultiMAC-BLAKE2b256 | 1.030 us | 0.0041 us | 0.0032 us |
| MultiMAC-BLAKE2b384 | 1.024 us | 0.0036 us | 0.0033 us |
| MultiMAC-BLAKE2b512 | 1.031 us | 0.0062 us | 0.0058 us |

### 16 KiB file
|     Method |     Mean |   Error |  StdDev |
|:----------:|:--------:|:-------:|:-------:|
| BLAKE2b256 | 12.18 us | 0.055 us | 0.046 us |
| BLAKE2b384 | 12.36 us | 0.242 us | 0.226 us |
| BLAKE2b512 | 12.10 us | 0.155 us | 0.137 us |
| MultiMAC-BLAKE2b256 | 11.42 us | 0.031 us | 0.029 us |
| MultiMAC-BLAKE2b384 | 11.43 us | 0.018 us | 0.015 us |
| MultiMAC-BLAKE2b512 | 11.71 us | 0.043 us | 0.033 us |

### 32 KiB file
|     Method |     Mean |   Error |  StdDev |
|:----------:|:--------:|:-------:|:-------:|
| BLAKE2b256 | 23.70 us | 0.120 us | 0.107 us |
| BLAKE2b384 | 23.82 us | 0.205 us | 0.191 us |
| BLAKE2b512 | 24.16 us | 0.197 us | 0.185 us |
| MultiMAC-BLAKE2b256 | 22.30 us | 0.101 us | 0.095 us |
| MultiMAC-BLAKE2b384 | 22.26 us | 0.116 us | 0.103 us |
| MultiMAC-BLAKE2b512 | 22.26 us | 0.087 us | 0.073 us |

### 64 KiB file
|     Method |     Mean |   Error |  StdDev |
|:----------:|:--------:|:-------:|:-------:|
| BLAKE2b256 | 47.03 us | 0.056 us | 0.044 us |
| BLAKE2b384 | 47.87 us | 0.207 us | 0.193 us |
| BLAKE2b512 | 47.08 us | 0.112 us | 0.099 us |
| MultiMAC-BLAKE2b256 | 43.73 us | 0.029 us | 0.025 us |
| MultiMAC-BLAKE2b384 | 43.91 us | 0.174 us | 0.163 us |
| MultiMAC-BLAKE2b512 | 43.92 us | 0.193 us | 0.181 us |

### 128 KiB file
|     Method |     Mean |   Error |  StdDev |
|:----------:|:--------:|:-------:|:-------:|
| BLAKE2b256 | 144.7 us | 1.20 us | 1.12 us |
| BLAKE2b384 | 144.8 us | 1.48 us | 1.39 us |
| BLAKE2b512 | 145.2 us | 1.09 us | 1.02 us |
| MultiMAC-BLAKE2b256 | 87.41 us | 0.069 us | 0.061 us |
| MultiMAC-BLAKE2b384 | 86.78 us | 0.101 us | 0.094 us |
| MultiMAC-BLAKE2b512 | 86.90 us | 0.228 us | 0.202 us |

### 32 MiB file
|     Method |     Mean |   Error |  StdDev |
|:----------:|:--------:|:-------:|:-------:|
| BLAKE2b256 | 34.50 ms | 0.561 ms | 0.524 ms |
| BLAKE2b384 | 35.56 ms | 0.264 ms | 0.247 ms |
| BLAKE2b512 | 35.52 ms | 0.233 ms | 0.194 ms |
| MultiMAC-BLAKE2b256 | 22.77 ms | 0.066 ms | 0.061 ms |
| MultiMAC-BLAKE2b384 | 22.58 ms | 0.023 ms | 0.020 ms |
| MultiMAC-BLAKE2b512 | 22.62 ms | 0.058 ms | 0.055 ms |
