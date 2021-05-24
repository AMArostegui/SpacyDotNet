# SpacyDotNet

SpacyDotNet is a .NET wrapper for the popular natural language library [spaCy](https://spacy.io/)

## Project scope and limitations

This project is not meant to be a complete and exhaustive implementation of all spaCy features and [APIs](https://spacy.io/api). Altough it should be enough for basic tasks, I think of it as a starting point, if the user needs to build a complex project using spaCy in .NET 

Most of the basic features in _Spacy101 section_ of the docs are available. All **Containers** classes are present (_Doc_, _DocBin_, _Token_, _Span_ and _Lexeme_) with their basic properties/methods running and also _Vocab_ and _StringStore_ in a limited form.

Furthermore, any developer should be ready to add the missing properties or classes in a very straightforward manner.

## Requirements

This projects relies on [Python.NET](http://pythonnet.github.io/) to interop with spaCy, which is written in Python/Cython.

It's been tested under **Windows 10** and **Ubuntu Linux 20.04**, using the following environment

- .NET Core 3.1 / .NET Standard 2.1
- spaCy 3.0.5
- Python 3.8
- Python.NET: Latest official NuGet: [3.0.0-preview2021-04-03](https://www.nuget.org/packages/pythonnet/3.0.0-preview2021-04-03)

Furthermore, it might work under different libraries:

- .NET Core 3.0 and 2.1 should be fine. .NET 5.0 is a major release that I haven't tried so far. I haven't tried .NET Framework either
- It should work with spaCy 2.3.5 and any other spaCy version that changes only its minor/patch version number

The current version of Python.NET has been compiled against Python 3.8 so the virtual environment must be created under this version. In general we should honor the specified Python.NET compiled CPython version

## Setup

### 1) Create a Python virtual environment and install spaCy

It's advised to create a virtual environment to install spaCy. Depending on the host system this is done in different ways.

The spaCy official [installation guide](https://spacy.io/usage) is fine, but keep in mind Python 3.8 restriction.

To run the examples, we'll also need to install the correspoding language package (_es_core_news_sm_) as shown in the guide.

### 2) Check for Python shared library

Python.NET makes use of Python as a shared library. Sadly, seems like the shared library is not copied with recent versions of _virtualenv_ and it's not even distributed in some flavours of Linux/Python >= 3.8

While I don't understand the rationale behind those changes, we should check the following:

**Windows**

Check whether _python38.dll_ in located under _<venv_root>\Scripts_ folder. Otherwise, go to your main Python folder and copy all dlls. In my case: _python3.dll_, _python38.dll_ and the _vcruntime140.dll_

**Linux**

Check whether a libpython shared object is located under _<venv_root>/bin_ folder.

If not, we first need to check if the shared object is present on our system. [find_libpython](https://pypi.org/project/find-libpython/) can help with this task.

If library is nowhere to be found, it's likely that installing _python-dev_ package with the package manager of your favorite distribution will place the file in your system.

Once we locate the library, drop it to the _bin_ folder. In my case, the file is named _libpython3.8.so.1.0_

## Usage

SpaCyDotNet is built to be used as a library. However I provide an example project as a CLI program.

### 1) Compile and Build

If using the CLI to run .NET, (Linux), we should simply browse to _Test/cs_ folder and compile the project with `dotnet build`. Under Visual Studio, just load _Test.sln_ solution

### 2) Run the project

The program expects two parameters

- **interpreter:** Name of Python shared library file. Usually _python38.dll_ on Windows, _libpython3.8.so_ on Linux and _libpython3.8.dylib_ on Mac
- **venv:** Location of the virtual environment create with python 3.8 and a spaCy version

Run the example with `dotnet run --interpreter <name_of_intepreter> --venv <path_to_virtualenv>` or if using Visual Studio, set the command line in _Project => Properties => Debug => Application arguments_

In my case:

**Linux**

    dotnet run --interpreter libpython3.8.so.1.0 --venv /home/user/Dev/venvSpaCyPy38

**Windows**

    dotnet run --interpreter python38.dll --venv C:\Users\user\Dev\venvSpaCyPy38

## Code comparison

I've tried to mimic spaCy API as much as possible, considering the different nature of both C# and Python languages

### C# SpacyDotNet code

```c#
var nlp = spacy.Load("en_core_web_sm");
var doc = nlp.GetDocument("Apple is looking at buying U.K. startup for $1 billion");

foreach (Token token in doc.Tokens)
    Console.WriteLine($"{token.Text} {token.Lemma} {token.PoS} {token.Tag} {token.Dep} {token.Shape} {token.IsAlpha} {token.IsStop}");

Console.WriteLine("");
foreach (Span ent in doc.Ents)
    Console.WriteLine($"{ent.Text} {ent.StartChar} {ent.EndChar} {ent.Label}");

nlp = spacy.Load("en_core_web_md");
var tokens = nlp.GetDocument("dog cat banana afskfsd");

Console.WriteLine("");
foreach (Token token in tokens.Tokens)
    Console.WriteLine($"{token.Text} {token.HasVector} {token.VectorNorm}, {token.IsOov}");

tokens = nlp.GetDocument("dog cat banana");
Console.WriteLine("");
foreach (Token token1 in tokens.Tokens)
{
    foreach (Token token2 in tokens.Tokens)
        Console.WriteLine($"{token1.Text} {token2.Text} {token1.Similarity(token2) }");
}

doc = nlp.GetDocument("I love coffee");
Console.WriteLine("");
Console.WriteLine(doc.Vocab.Strings["coffee"]);
Console.WriteLine(doc.Vocab.Strings[3197928453018144401]);

Console.WriteLine("");
foreach (Token word in doc.Tokens)
{
    var lexeme = doc.Vocab[word.Text];
    Console.WriteLine($@"{lexeme.Text} {lexeme.Orth} {lexeme.Shape} {lexeme.Prefix} {lexeme.Suffix} 
{lexeme.IsAlpha} {lexeme.IsDigit} {lexeme.IsTitle} {lexeme.Lang}");
}
```
### Python spaCy code

```python
nlp = spacy.load("en_core_web_sm")
doc = nlp("Apple is looking at buying U.K. startup for $1 billion")

for token in doc:
    print(token.text, token.lemma_, token.pos_, token.tag_, token.dep_,
          token.shape_, token.is_alpha, token.is_stop)

print("")
for ent in doc.ents:
    print(ent.text, ent.start_char, ent.end_char, ent.label_)

nlp = spacy.load("en_core_web_md")
tokens = nlp("dog cat banana afskfsd")

print("")
for token in tokens:
    print(token.text, token.has_vector, token.vector_norm, token.is_oov)

tokens = nlp("dog cat banana")
print("")
for token1 in tokens:
    for token2 in tokens:
        print(token1.text, token2.text, token1.similarity(token2))

doc = nlp("I love coffee")
print("")
print(doc.vocab.strings["coffee"])  # 3197928453018144401
print(doc.vocab.strings[3197928453018144401])  # 'coffee'

print("")
for word in doc:
    lexeme = doc.vocab[word.text]
    print(lexeme.text, lexeme.orth, lexeme.shape_, lexeme.prefix_, lexeme.suffix_,
            lexeme.is_alpha, lexeme.is_digit, lexeme.is_title, lexeme.lang_)
```
### Output

![Output](https://github.com/AMArostegui/SpacyDotNet/blob/master/Output.png)
